// // 파일 이름에 .test 들어가면 vscode? 가 인식한다고 함
const sum = require("./sum.js");

test("1 + 2 is equal to 3", () => {
   expect(sum(1, 2)).toBe(3);
});