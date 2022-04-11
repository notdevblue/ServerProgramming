const mongoose = require("mongoose");
const Account = mongoose.model("accounts");

const argon2i = require("argon2-ffi").argon2i;
const crypto = require("crypto");

const passwordRegex = new RegExp("(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.{8,})");


// 라우팅
module.exports = (app) => {
   app.post("/account/login", async (req, res) => {
      console.log(req.body);
      var response = { code: 0, msg: "success" };

      const { rUsername, rPassword } = req.body;
      if (rUsername == null || !passwordRegex.test(rPassword)) {
         response.code = 1;
         response.code = "Invalid credentials";
         console.log("Invalid Cred null");
         res.send(response);
         return;
      }

      // username 이 rUsername 인 계정을 찾고, username 만 가져옴
      // 여러개 가져올때는 "username password" 처럼 띄어쓰기로 구분함
      var userAccount = await Account.findOne({ username: rUsername }, "username password"); // IO => await

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
      else {
         response.code = 1;
         console.log("Cannot find account");
         response.msg = "Invalid credentials";
         res.send(response);
         return;
      }
   });

   app.post("/account/create", async (req, res) => {
      console.log(req.body);
      var response = {};

      const { rUsername, rPassword } = req.body;
      if (rUsername == null || rUsername.length < 3 || rUsername.length > 24) {
         response.code = 1;
         response.msg = "Invalid Credentials";
         res.send(response);
         return;
      }

      if (!passwordRegex.test(rPassword)) {
         response.code = 3;
         response.msg = "Unsafe password";
         res.send(response);
         return;
      }

      var userAccount = await Account.findOne({ username: rUsername }, "username"); // IO => await

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