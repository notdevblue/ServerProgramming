const mongoose = require("mongoose");
const { Schema } = mongoose;

const { Types: { ObjectId } } = Schema;

const commentSchema = new Schema({
   commenter: {
      type: ObjectId, // 고유한 ID 값
      required: true,
      ref: "User", // 댓글 작성자와 유저 정보랑 연결됨
   },
   comment: {
      type: String,
      required: true
   },
   createdAt: {
      type: Date,
      default: Date.now
   }
});

module.exports = mongoose.model("Comment", commentSchema);