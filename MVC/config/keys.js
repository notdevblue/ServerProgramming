if (process.env.NODE_ENV === "production") { // env.내가_정한_이름
   module.exports = require("./prod.js");
} else {
   module.exports = require("./dev.js");
}