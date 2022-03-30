const { send } = require("express/lib/response");
const WebSocket = require("ws");

const ws = new WebSocket("ws://ggm-ws-han.herokuapp.com");

var msg = {    // 메세지 타입 구조체
   chat: 4,    // 타입
   text: "Install Gentoo",
   id: "",     // 서버에서 생성
   target: "", // 귓말 타겟 id
   date: ""
};


ws.on("open", () => {
   setInterval(() => {
      ws.send(JSON.stringify(msg));
   }, 1000);
});
