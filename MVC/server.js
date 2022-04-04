const express  = require("express");
const keys     = require("./config/keys.js");
const mongoose = require("mongoose");
const bodyParser = require("body-parser");

const app   = express();
const port  = 3003;

mongoose.connect(keys.mongoURI);
app.use(bodyParser.urlencoded({ extended: false }));


// setup database model
require("./model/Account.js");

// setup the routes.
require("./routes/authRoutes.js")(app);

app.listen(port, () => {
   console.log(`Listening on ${port}`);
});