const express = require("express");
const User = require("../schema/user");
const Comment = require("../schema/comment");

const router = express.Router();

router.route("/")
   .get(async (req, res, next) => {
      // user find
   }) // 읽기
   .post(async (req, res, next) => {
      try {
         const user = await User.create({ // 데이터 생성 명령
            name: req.body.name, // body 에 post 들어가 있음
            age: req.body.age
         });
      } catch (ex) {
         //error
         next(err); // 다음 과정으로 진행하는데 문제 없게
      }
   }); // 쓰기