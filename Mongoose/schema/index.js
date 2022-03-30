const mongoose = require('mongoose');

const connect = () => {

   mongoose.connect("mongodb://han:0225@127.0.0.1:27017/admin", {
      dbName: "nodejs",
      useNewUrlParser: true,
      // useCreateIndex: true,

   }, (error) => {
      if (error) {
         console.log("MongoDB connection error: " + error);
      }
   }); // mongodb://이름:비번@주소:포트/권환
};

mongoose.connection.on("error", (error) => {
   console.log("MongoDB connection error");
}); // 에러 이벤트 날아왔을 시

mongoose.connection.on("disconnection", (error) => {
   console.log("MongoDB disconnected");
   connect();
}); // 연결이 끊긴 경우

module.exports = connect;