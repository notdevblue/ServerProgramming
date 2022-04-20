class Player {
   constructor(owner, position, r, g, b) {
      this.owner = owner;
      this.posX = position.x;
      this.posY = position.y;

      this.r = r;
      this.g = g;
      this.b = b;

      this.cellType = -1; // 0 = Player
   }
}

module.exports = Player;