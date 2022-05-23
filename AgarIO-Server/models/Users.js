const mongoose = require("mongoose");
const { Schema } = mongoose;

const userSchema = new Schema({
   userid: Number,
   username: String,
   score: {
      type: Number,
      require: true, // 필수적인 필드로 지정
   },
   joinAt: { // 게임 가입 시간
      type: Date,
      default: Date.now, // 항목 안 념겨도 알아서 디폴트로 가짐
   }
});

module.exports = mongoose.model("User", userSchema); // model 앞에 들어가는 이름에 s 를 붙혀 컬랙션을 생성함, 그리고 어차피 이름은 소문잘 쇙성이 됨