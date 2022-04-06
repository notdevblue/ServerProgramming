const mongoose = require("mongoose");
const Account = mongoose.model("accounts");

const argon2i = require("argon2-ffi").argon2i;
const crypto = require("crypto");


// 라우팅
module.exports = (app) => {
   app.post("/account/login", async (req, res) => {
      console.log(req.body);
      var response = { code: 0, msg: "success" };

      const { rUsername, rPassword } = req.body;
      if (rUsername == null || rPassword == null) {
         response.code = 1;
         response.code = "Invalid credentials";
         console.log("Invalid Cred null");
         res.send(response);
         return;
      }

      var userAccount = await Account.findOne({ username: rUsername }); // IO => await

      if (userAccount != null) {

         // 암호화 된 문자열, 평문 문자열을 비교함
         argon2i.verify(userAccount.password, rPassword).then(async (success) => {
            if (success) {
               userAccount.lastAuthentication = Date.now();
               await userAccount.save();
               response.code = 0;
               response.msg = "Account found: " + userAccount.username;

               res.send(response);
               return;

            } else {
               console.log("Invalid Cred wrong password");

               response.code = 1;
               response.msg = "Invalid credentials"; // 둘중 뭐가 잘못됬는지 안 알려주는게 보안적인 이슈 때문이었음
               res.send(response);
               return;
            }
               
         });


         // if (userAccount.password == rPassword) {
         //    userAccount.lastAuthentication = Date.now();
         //    await userAccount.save();
         //    console.log("Login scueess");
         //    res.send(userAccount);
         //    return;
         // }
      }
      response.code = 1;
      console.log("Cannot find account");
      response.msg = "Invalid credentials";
      res.send(response);
      return;
   });

   app.post("/account/create", async (req, res) => {
      console.log(req.body);

      const { rUsername, rPassword } = req.body;
      if (rUsername == null || rPassword == null) {
         res.send("Invalid credentials");
         return;
      }

      var userAccount = await Account.findOne({ username: rUsername }); // IO => await

      if (userAccount == null) {
         console.log("Created new account");


         crypto.randomBytes(32, function (err, salt) {

            argon2i.hash(rPassword, salt).then(async (hash) => {
               console.log("Hashed: " + hash);

               var newAccount = new Account({
                  username: rUsername,
                  password: hash,
                  salt: salt,
                  lastAuthentication: Date.now()
               });

               await newAccount.save();
               res.send(newAccount);
               return;
            });

         }); // 유저 비번 + 서버 생성(salt) = 저장되는 비번         

      } else {
         res.send("Username is already taken.");
      }
      return;
   });
}