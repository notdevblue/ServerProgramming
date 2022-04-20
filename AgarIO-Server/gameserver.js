var Vector = require("./modules/Vector.js");
var Player = require("./entity/Player.js");
const gameLoop = require("./gameloop.js");

// Server op code (서버 이벤트 처리)
const START_COUNTDOWN_OP_CODE = 101;
const MOVE_PLAYER_OP_CODE = 102;

// Client op code (클라 이벤트 처리)
const SCENE_READY_OP_CODE = 200;


const PORT = process.env.PORT || 48000;

const { WebSocketServer } = require('ws');
const wsServer = new WebSocketServer(
   { port: PORT },
   () => {
      console.log(`Server started on port: ${PORT}`);
      StartGame();
   }
);


const players = [];
const serverFramerate = 10.0;

let gameLoopId = null;
let lastPlayerId = 1;

function StartGame() {
   gameLoopId = gameLoop.setGameLoop(function () {
      for (let player = 0; player < players.length; ++player) {
         console.log("GameLoop! " + player);
      }
   }, 1000.0/serverFramerate);
}

function StopGame() {
   if (gameLoopId != null) {
      gameLoop.clearGameLoop(gameLoopId);
      gameLoopId = null;
   }
}

function getRandomPosition() {
   return new Vector(Math.floor(Math.random() * 1920 + 1),
                     Math.floor(Math.random() * 1080 + 1));
}

function getRandomColor() {
   var colorRGB = [255, 10, Math.floor(Math.random() * 256)];
   colorRGB.sort(() => {
      return 0.5 - Math.random(); // 양수, 음수 나누어 정렬됨
   });
   return {
      r: colorRGB[0],
      g: colorRGB[1],
      b: colorRGB[2],
   }
}

function getNewPlayerId() {
   return lastPlayerId++;
}

function spawnPlayer(ws) {
   var ownerId = getNewPlayerId();
   var pos = getRandomPosition();
   var c = getRandomColor();
   var cell = new Player(ownerId, pos, c.r, c.g, c.b);

   players.push(cell);
   ws.send(JSON.stringify(cell));
   console.log("Spawning player");
}

var msg = {
   opCode: 0
}

wsServer.on("listening", () => {
   console.log(`Server listening on port: ${PORT}`);
});

wsServer.on("connection", (client) => {
   client.send("Hello! I am a server that you connected!");
   console.log("Hello new client");

   client.on("message", (data) => {
      if (data.sender != 0) {
         msg = JSON.parse(data.toString());

         switch (msg.opCode)
         {
            case SCENE_READY_OP_CODE:
               spawnPlayer(client);
               break;
            
            default:
               console.log("[Warning] Unrecognized opCode in msg\r\n"
                         + `Code: ${msg.opCode}`);
         }
      }

   });

   client.on("close", () => {
      console.log("Connection closed");
      StopGame();
   });
});