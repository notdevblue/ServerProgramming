const Vector = require("../modules/Vector");
class Food {
   constructor(postiion) {
      this.postiion = new Vector(postiion.x, postiion.y);
   }
}

module.exports = Food;