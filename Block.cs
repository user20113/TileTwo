using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTwo
{
    class Block
    {

        public int ID = 0;
        public int meta = 0;
        public int durability = 0;
        public int animationTotal = 0;
        public bool animate = false;
        public void generate(int ID, bool animate, int animationTotal)
        {
            this.ID = ID;
            this.animate = animate;
            this.animationTotal = animationTotal;

        }
    }
}
