let app = require("express")();
let server = require("http").createServer(app); // http 서버를 express 기반으로 만듬

let io = require('socket.io')(server); // http 기반 서버가 socketio 기반으로 업그레이드 됨

io.on("connection", socket => {
   setTimeout(() => {
      socket.emit("connection", { date: new Date().getTime(), data: "Hello Gentoo" });
   }, 1000);

   socket.on("hello", data => {
      console.log("data: " + data);
   });

   socket.on("login", data => {
      socket.name = data.name;
      socket.userid = data.userid;
      socket.password = data.password; // ?

      io.emit("login", data.name);
   });

   socket.on("chat", data => {
      let msg = {
         from: {
            name: socket.name,
            userid: socket.userid
         },
         msg: data.msg
      };

      // 메세지 전송한 클라이언트를 제외하고 모든 클라에게 전송
      socket.broadcast.emit("chat", msg);
      
      // 메세지를 전송한 클라이언트 에게만 전송
      socket.emit("s2c chat", msg);
      
      io.emit("s2c chat", msg);

      // io.to(id).emit("s2c chat", msg);
   });

   socket.on("disconnect", () => { // 자연스러운 종료
      console.log("User disconnected: " + socket.name);
   });

   socket.on("forceDisconnect", () => { // 서버에서 강제 종료
      socket.disconnect();
   });
});

/*
io.emit = 브로드케스트
socket.emit = socket 에만
socket.broadcast.emit = socket 뺴고
io.to(id).emit = id 에게
*/

server.listen(3000, () => {
   console.log("SocketIO server is listening on port: 3000");
});

