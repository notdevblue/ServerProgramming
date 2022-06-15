var Vector = require('./modules/Vector');
var Player = require('./entity/Player');
var Food = require('./entity/Food');
var Virus = require("./entity/Virus.js");

var cfg = require("./config.json");

const connect = require("./models");
connect();
const usr = require("./models/Users.js");

const WebSocket = require('ws');

const gameloop = require('./gameloop.js');
const players = [];

const foods = [];
const viruss = [];

var sockets = {};
let gameLoopId = null;
const serverFrameRate = 3.0; //10.0;
let lastPlayerId = 0;
let lastFoodId = 0;
let lastVirusId = 0;

//server op code
const START_COUNTDOWN_OP_CODE = 101;
const MOVE_PLAYER_OP_CODE = 102;
const JOIN_PLAYER_OP_CODE = 103;
const ADD_FOOD_OP_CODE = 105;
const ADD_VIRUS_OP_CODE = 106;
const GAMEOVER_OP_CODE = 110;

//client op code
const SCENE_READY_OP_CODE = 200;
const EAT_FOOD_OP_CODE = 202;
const PING_CHECK_OP_CODE = 205;
const EAT_VIRUS_OP_CODE = 203;
const DEATH_OP_CODE = 204;

function StartGame() {
    //loop create
    gameLoopId = gameloop.setGameLoop(function () {
        var foodToAdd = cfg.maxFood - foods.length;
        if (foodToAdd > 0)
            addFood(foodToAdd);
        
        var virusToAdd = cfg.maxVirus - viruss.length;
        if (virusToAdd > 0)
            addVirus(virusToAdd);
        
        players.forEach(function (p) {
            movePlayer(p);
        });

    }, 1000.0 / serverFrameRate);

    gameloop.setGameLoop(function () {
        sendUpdates();
    }, 2000.0 / serverFrameRate);
}

function StopGame() {
    //loop clear
    if (gameLoopId != null) {
        gameloop.clearGameLoop(gameLoopId);
        gameLoopId = null;
    }
}

function getRandomPosition() {
    return new Vector(
        Math.floor(Math.random() * 20 + 1),
        Math.floor(Math.random() * 10 + 1)
    );
}

function getRandomColor() {
    var colorRGB = [255, 10, Math.floor(Math.random() * 256)];
    colorRGB.sort(function () {
        return 0.5 - Math.random();
    });
    return {
        r: colorRGB[0],
        g: colorRGB[1],
        b: colorRGB[2]
    };
}

async function getNewPlayerId() {
    const uuid = await usr.countDocuments({ name: 'users' }); // name: 컬랙션의 이름, 아무것도 안 넘김녀 도큐먼트 안 모든 것
    console.log("Number of users: " + uuid);

    return uuid;
}

function movePlayer(player) {
    // var x = 0, y = 0;
    var target = {
        x: player.targetX - player.posX,
        y: player.targetY - player.posY
    };

    var deg = Math.atan2(target.y, target.x);
    var speed = 2;
    var deltaX = speed * Math.cos(deg);
    var deltaY = speed * Math.sin(deg);

    player.posX += deltaX;
    player.posY += deltaY;
}

function spawnPlayer(ws) {
    var ownerId = ws.clientId;
    var pos = getRandomPosition();
    var c = getRandomColor();
    var mass = 1;
    var cell = new Player(ownerId, pos, c.r, c.g, c.b, ws.nickname, mass); //nickname
    console.log('spawning player : ' + ownerId);

    msg.opCode = JOIN_PLAYER_OP_CODE;
    msg.socketId = ws.clientId;
    msg.player = cell;
    players[ownerId] = cell; // players.push(cell);

    // 이미 존재하던 플레이어들에게 새로접속한 나의 존재를 알림.
    for (var key in sockets) { 
        sockets[key].send(JSON.stringify(msg));
    }

    // 나(클라이언트)에게 이미 플레이하던 유저들의 정보를 보냄.
    players.forEach(function (p) { 
        msg.player = p;

        if (p.owner != ownerId)
            ws.send(JSON.stringify(msg));
    });

    const userinfo = usr.create({
        userid: ownerId,
        username: cell.nickname,
        score:0
    });
    
}

function addFood(toAdd) {
    if (toAdd < 0) return;

    while (toAdd--) {
        var pos = getRandomPosition();
        var c = getRandomColor();
        var food = new Food(lastFoodId++, pos, c.r, c.g, c.b);
        foods.push(food);
    }

    // for (var key in players) {
    //     msg.opCode == ADD_FOOD_OP_CODE;
    //     msg.foods = foods;
    //     sockets[key].send(JSON.stringify(msg));
    // }
}

function removeFood() {
}

function deleteFood(fid) {
    foods[fid] = {};
    console.log("delete food: " + fid);
    foods.splice(fid, 1); // 사직되는 요소부터 한칸 제거함
}

function addVirus(toAdd) { 
    if (toAdd < 0) return;

    while (toAdd--) {
        var pos = getRandomPosition();
        var v = new Virus(lastVirusId++, pos);
        viruss.push(v);
    }
}

function deleteVirus(vid) {
    viruss[vid] = {};
    console.log("Deleted virus: " + vid);
    viruss.splice(vid, 1);
}

function deathCheck(collisions) { // player 타입 list
    var sid = (collisions[0].mass.toFixed(2) < collisions[1].mass.toFixed(2)) ? collisions[0].owner : collisions[1].owner; // 제거되어야할 세포 id

    if (sockets[sid] == null) return;
    msg.socketId = sid;
    msg.opCode = GAMEOVER_OP_CODE;
    sockets[sid].send(JSONS.stringify(msg));
    console.log("Delete player: " + sockets[sid].clientId);
    delete sockets[sid];

    players.forEach(function (v) {
        if (v.owner == sid) {
            players[v] = {};
            players.splice(v, 1);
        }
    });

}

function pingCheck(ws) {
    msg.socketId = ws.clientId;
    msg.opCode = PING_CHECK_OP_CODE;
    ws.send(JSON.stringify(msg));
}

function sendUpdates() {
    players.forEach(function (p) {
        msg.socketId = p.owner;
        msg.opCode = MOVE_PLAYER_OP_CODE;
        msg.player = p;
        msg.foods = foods; // 서버에서 살아남은 foods
        msg.virus = viruss;
        msg.visibleCells = players;

        if (sockets[p.owner] == null) return;

        // console.log("sendUpdates: " + p.targetX);
        sockets[p.owner].send(JSON.stringify(msg));
    });
}

var msg = {
    socketId: -1,
    opCode: 0,
    foods: [], // 살아 있는 푸드 데이터
    player: null,
    visibleCells: [], // 사아 범위 내 모든 종류의 플레이어
    eatenFoodId: -1,
    eatenVirusId: -1,
    virus: [],
    collisions: [],
};

const wsserver = new WebSocket.Server({ port: process.env.PORT || 3003 }, () => {
    console.log("server started!");
    StartGame();
});

wsserver.on('connection', async function connection(ws, req) {
    //ws.send("Hello! I am a server");
    ws.clientId = msg.socketId = await getNewPlayerId();
    msg.opCode = START_COUNTDOWN_OP_CODE;
    sockets[ws.clientId] = ws;
    ws.send(JSON.stringify(msg));
    console.log("hello new client : " + ws.clientId);

    const ipAddr = req.headers["x-forwarded-for"] || req.headers.host;

    ws.on('message', (data) => {
        if (data.sender != 0) {
            msg = JSON.parse(data.toString());
            switch (msg.opCode) {
                case SCENE_READY_OP_CODE:
                    ws.nickname = msg.nickname;
                    spawnPlayer(ws);
                    ws.score = 0;
                    break;
                case PING_CHECK_OP_CODE:
                    pingCheck(ws);
                    break;
                case MOVE_PLAYER_OP_CODE:
                    players[msg.socketId] = msg.player;
                    break;
                case EAT_FOOD_OP_CODE:
                    ws.score += 10;
                    players[ws.clientId].score = ws.score;
                    console.log("eating food.");
                    deleteFood(msg.eatenFoodId);

                    // usr.findById(); // 해당하는 하나 받고 v.update(); 해도 되긴 함
                    // 결과적으로 find 돌리고 update 하면 서버에서 하나의 동작을 위해 두개의 쿼리가 돌아감
                    // 쿼리 겟수를 쓸대없이 늘리지 않는게 좋음
                    // usr.findOneAndUpdate(); 는 updateOne 과 다름. findOneAndUpdate() 는 find 하고 update(); 함. 결과적으로 쿼리 두번 날라감
                    // 만약에 sendUpdates 같이 db에 직접 접근해서 IO 하면 서버 fps 엄청 낮아짐
                    // db IO 는 하드 IO 와 거의 동일
                    // 지속적인 이벤트 부분에서는 사용을 지양


                    usr.updateOne(
                        { userid: ws.clientId }, // 조건을 충족하는 데이터 찾고 업데이트함, updateMany 는 조건 충족하는 다수를 업데이트
                        { score: ws.score }
                    ).then(result => {
                        console.log("@@result: ", result);
                    });

                    // usr.count({}, (err, count) => {
                    //     console.log("Number of users: ", count);
                    // });

                    break;
                
                case EAT_VIRUS_OP_CODE:
                    ws.score += 20;
                    players[ws.clientId].score = ws.score;
                    deleteVirus(msg.eatenVirusId);
                    console.log("eating virus");
                    
                    usr.updateOne(
                        { userid: ws.clientId }, // 조건을 충족하는 데이터 찾고 업데이트함, updateMany 는 조건 충족하는 다수를 업데이트
                        { score: ws.score }
                    ).then(result => {
                        console.log("@@result: ", result);
                    });

                    break;
                
                case DEATH_OP_CODE:
                    deathCheck(msg.collisions);
                    console.log("player death: " + msg.collisions);
                    break;
            
                default:
                    console.log("[Warning] Unrecognized opCode in msg");
            }
        }
    });

    ws.on('close', () => {
        console.log("connection close");
        delete sockets[ws.clientId];
        players.forEach(function (v) {
            if (v.ownerId == ws.clientId) {
                players[v] = {}
                players.splice(v, 1);
                console.log("delete player: " + ws.clientId);
            }
        });
        console.log(`# A client disconnected from server [${ipAddr}]`);
        // StopGame();
    });
});

wsserver.on('listening', () => {
    console.log(`listening on ${process.env.PORT || 3003}`);
});