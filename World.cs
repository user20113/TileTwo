using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TileTwo
{
    class World
    {

        Block Block = new Block();
        public Block[,] world = new Block[4000, 4000];

        public Random Rand = new Random(DateTime.Now.Millisecond);
        public int random = 0;
        public void GenWorld()
        {
            int z = 0;
            int HeightOffset = 0;
            for (int x = 0; x < 4000; x++)
            {
                random = (int)(Rand.NextDouble() * 4);
                if ((int)random == 0) HeightOffset -= 1;
                if ((int)random == 1) HeightOffset -= 2;
                if ((int)random == 2) HeightOffset += 1;
                if ((int)random == 3) HeightOffset += 2;
                for (int y = 0; y < 4000; y++)
                {
                    z = 252;

                    if (y < 3500 + HeightOffset)
                        z = 4;
                    if (y < 3490 + HeightOffset)
                        z = 8;
                    if (y < 3485 + HeightOffset)
                        z = 0;
                    random = (int)(Rand.NextDouble() * 10);
                    if (random > 5) z++;
                    if (random > 7) z++;
                    if (random > 8) z++;
                    world[x, y] = new Block();
                    world[x, y].generate(z, false, 5);

                }

            }



        }
    }
}
