let activeLoops = [];
const getLoopId = (
function () { // ???????
   let staticLoopId = 0;

   return function () {
      return staticLoopId++;
   }
}
)();


module.exports.setGameLoop = function (update, tickLength = 1000 / 30) {
   let loopId = getLoopId();
   activeLoops.push(loopId);

   const longWaitMs = Math.floor(tickLength - 1);

   let frame  = 0;
   let prev   = process.hrtime();
   let target = process.hrtime();

   const gameLoop = function () {
      ++frame;
      const now = process.hrtime(); // 고해상도의 시간을 받아올 수 있음

      if (now >= target) {
         const delta = now - prev;
         prev = now;
         target = now + tickLength;

         update(delta);
      }

      if (activeLoops.indexOf(loopId) === -1) { 
         return;
      }

      const remainingTick = target - process.hrtime();

      // TODO: 나중에 한번 해보면 좋을거같은데
      if (remainingTick > longWaitMs) {
         setTimeout(gameLoop, Math.max(longWaitMs, 16));
      } else {
         setImmediate(gameLoop);
      }
   }

   // Begin loop
   gameLoop();

   return loopId;
};

module.exports.clearGameLoop = function (loopId) {
   activeLoops.splice(activeLoops.indexOf(loopId), 1);
};