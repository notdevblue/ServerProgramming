
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
   }
);
const gameLoop = require("./gameloop.js");

const players = [];
const serverFramerate = 10.0;
   
let gameLoopId = null;

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

var msg = {
   opCode: 0
}

wsServer.on("listening", () => {
   console.log(`Server listening on port: ${PORT}`);
});

wsServer.on("connection", (client) => {
   client.send("Hello! I am a server that you connected!");
   console.log("Hello new client");

   players.push(1); // FIXME: test

   client.on("message", (data) => {
      if (data.sender != 0) {
         msg = JSON.parse(data.toString());

         switch (msg.opCode)
         {
            case SCENE_READY_OP_CODE:
               StartGame();
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