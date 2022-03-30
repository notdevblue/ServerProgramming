const express = require("express");
const User = require("../schema/user.js");

const router = express.Router();

// 통신은 비동기가 좋음
router.get("/", async (req, res, next) => {
   try { // 프로미스 오류 사전에 예방
      const users = await User.find({}); // 아무런 조건 없이 읽어음
      res.render("mongoose", { users });
   } catch (ex) {
      console.error(ex);
      next(err);
   }
});

module.exports = router;