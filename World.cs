using System;

namespace TileTwo
{
    internal class World
    {
        private Block Block = new Block();
        public Block[,] world = new Block[4000, 4000];

        public Random Rand = new Random(DateTime.Now.Millisecond);
        public int random = 0;

        public void GenWorld(Entity player)
        {
            int z = 0;
            int HeightOffset = 0;
            for (int x = 0; x < 4000; x++)
            {
                random = (int)(Rand.NextDouble() * 10);
                if ((int)random == 0) HeightOffset -= 1;
                if ((int)random == 1) HeightOffset -= 2;
                if ((int)random == 2) HeightOffset += 1;
                if ((int)random == 3) HeightOffset += 2;
                if (x == 2000)
                    player.yPos = 3501 + HeightOffset;
                for (int y = 0; y < 4000; y++)
                {
                    z = 0;
                    if (y < 3500 + HeightOffset)
                        z = 3;
                    if (y < 3499 + HeightOffset)
                        z = 7;
                    if (y < 3485 + HeightOffset)
                        z = 1;
                    random = (int)(Rand.NextDouble() * 10);
                    int meta = 0;
                    if (random > 5) meta++;
                    if (random > 7) meta++;
                    if (random > 8) meta++;
                    world[x, y] = new Block();
                    world[x, y].generate(z, false, 0);
                    if (z != 0)
                        world[x, y].meta = meta;
                }
            }
            world[2000, (int)player.yPos - 1].generate(127, false, 0);
        }
    }
}