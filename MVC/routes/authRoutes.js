const mongoose = require("mongoose");
const Account = mongoose.model("accounts");

const argon2i = require("argon2-ffi").argon2i;
const crypto = require("crypto");


// 라우팅
module.exports = (app) => {
   app.post("/account/login", async (req, res) => {
      console.log("Recived: " + req.body);

      const { rUsername, rPassword } = req.body;
      if (rUsername == null || rPassword == null) {
         res.send("Invalid credentials");
         return;
      }

      var userAccount = await Account.findOne({ username: rUsername }); // IO => await

      if (userAccount != null) {
         if (userAccount.password == rPassword) {
            userAccount.lastAuthentication = Date.now();
            await userAccount.save();
            console.log("Login scueess");
            res.send(userAccount);
            return;
         }
      }

      res.send("Invalid credentials");
      return;
   });

   app.post("/account/create", async (req, res) => {
      console.log(req.body);

      var accountSalt = null;
      var hashedPassword = null;

      crypto.randomBytes(32,async function (err, salt) {
         accountSalt = salt;

         argon2i.hash(rPassword, salt).then(hash => {
            hashedPassword = hash;
            console.log("Hashed: " + hashedPassword);
         });

         console.log("salt: " + accountSalt);
      }); // 유저 비번 + 서버 생성(salt) = 저장되는 비번

      const { rUsername, rPassword } = req.body;
      if (rUsername == null || rPassword == null) {
         res.send("Invalid credentials");
         return;
      }

      var userAccount = await Account.findOne({ username: rUsername }); // IO => await

      if (userAccount == null) {
         console.log("Created new account");
         var newAccount = new Account({
            username: rUsername,
            password: hashedPassword,
            lastAuthentication: Date.now()
         });
         
         await newAccount.save();
         res.send(newAccount);
         return;
      } else {
         res.send("Username is already taken.");
      }
      return;
   });
}