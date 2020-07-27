namespace TileTwo
{
    internal class Block
    {
        public int ID = 0;
        public int meta = 0;
        public int durability = 0;
        public int animationTotal = 0;
        public bool animate = false;
        public bool passable = true;

        public bool GetPassable()
        {
            return (ID > 135 || ID == 0);
        }

        public void generate(int ID, bool animate, int animationTotal)
        {
            this.ID = ID;
            this.animate = animate;
            this.animationTotal = animationTotal;
        }
    }
}