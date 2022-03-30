const mongoose = require("mongoose");
const { Schema } = mongoose;

// 코드 상에서 스키마 까지 관리 가능함
// 관계형은 따로 관리하고, 연결하는 코드 따로
// 관리 포인트 늘어날수록 휴먼 에러 가능성

const userSchema = new Schema({
   name: {
      type: String,     // 타입
      required: true,   // 필수 값 인가
      unique: true      // 중복 여부
   },
   age: {
      type: Number,
      required: true
   },
   createdAt: {
      type: Date,
      default: Date.now
   }
});

module.exports = mongoose.model("User", userSchema);
// userSchema 를 User 라는 내용으로 접근한다는 의미
// 해주면 ref 로 조회 가능함