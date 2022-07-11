// 테스트 코드 알고리즘 수행 전 적용되어야 하는 로직 미리 적용
// let message;
// beforeEach(() => message = "mmssgg");

// test("message test", () => {
//    expect(message).toBe("mmssgg");
// });
// test("message test1", () => {
//    expect(message + "123").toBe("mmssgg");
// });

// test("object assignment", () => {
//    const data = { one: 1 };
//    data["two"] = 3;

//    expect(data).toEqual({ one: 1, two: 2 });
// });

// test("null", () => {
//    const n = false;
//    // const undef = undefined;
//    // expect(n).toBe(null);
//    // expect(n).toBeNull(); // Null 인지
//    // expect(n).toBeDefined(); // undefined 가 아닌지
//    // expect(undef).toBeDefined(); // undefined 가 아닌지
//    // expect(undef).toBeUndefined(); // undefined 인지

//    // expect(n).not.toBeNull(); // not 붙이면 결과를 뒤집음

//    expect(n).toBeFalsy(); // 거짓인지
//    expect(n).toBeTruthy(); // 참인지
// });

// test("num test", () => {
//    const value = 2 + 2;
//    expect(value).toBeGreaterThan(2);
//    expect(value).toBeGreaterThanOrEqual(4);
//    expect(value).toBeLessThan(5);
//    expect(value).toBeLessThanOrEqual(4.1);
//    expect(value).toBeCloseTo(4.0023); // 임계값
// });

const shoppingList = [
   "keyboard",
   "switches",
   "keycaps",
   // "crytox"
];

test("shopping list has crytox", () => {
   expect(shoppingList).toContain("crytox"); // 요소가 포햄중인지
});

// test("1 is 1", () => {
//    expect(1).toBe(3);
// });

// // 다수의 테스트를 실행할 수 있음
// const str = "Install Gentoo";
// test("test gentoo string", () => {
//    expect(str).toBe("Install LFS");
// });

// 테스트 여러개를 하나의 작업 단위로 만들어줌
// describe("task1", () => {
//    test("2 is 2", () => {
//       expect(2).toBe(2);
//    });

//    test("test lfs string", () => {
//       expect(str).toBe("Install LFS");
//    });
// });

// describe("작업 단위 이름", () => {
//    test();
// });
// test 를 묶어서 indent 를 해줌?

// test("설명", () => {
//    expect((변수)기대값).toBe((상수?)값);
// });