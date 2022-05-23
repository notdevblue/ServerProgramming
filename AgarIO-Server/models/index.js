require("dotenv").config();
const mongoose = require("mongoose");
const { MONGO_ID, MONGO_PASSWORD, NODE_ENV } = process.env;
const MONGO_URL = `mongodb+srv://${MONGO_ID}:${MONGO_PASSWORD}@han-atlas.pcesj.mongodb.net`;


const connect = () => {
   mongoose.connect(
      MONGO_URL, {
         dbName: "Agario-Test",
         retryWrites: true,
      }, error => {
         if (error) {
            console.log("Mongodb conenction error. ", error);
         }
         else {
            console.log("Mongodb connection success");
         }
      }
   );
};

mongoose.connection.on("error", error => {
   console.error("Mongodb error. ", error);
});

mongoose.connection.on("disconnected", () => {
   console.log("Mongodb disconnected. trying to reconnect...");
   console.log(MONGO_URL);
   // connect();
});

module.exports = connect;