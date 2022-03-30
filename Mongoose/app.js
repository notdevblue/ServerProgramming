const express = require("express");
const connect = require("./schema/index.js");

const app = express();

app.set("port", process.env.PORT || 3003);
connect();
app.listen(app.get("port"), () => {
   console.log("listening on port: " + app.get("port"));
});