using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTwo
{
    class Player
    {
        public double xpos = 2000;
        public double ypos = 3550;
        public double xvel = 0;
        public double yvel = 0;
        public bool CanJump = false;
        public bool[] nearby = new bool[9];
        public double speed = 0.0625;


        public void PlayerGravity()
        {
            if (!nearby[7])
                yvel -= .5;
            else
            {
                yvel = 0;
                CanJump = true;
            }
        }
        public void PlayerSlow()
        {
            if (xvel > 0)
            {
                xvel -= speed;
                if (xvel < speed)
                    xvel = 0;
                if (nearby[5])
                {
                    xvel = 0;
                    xpos = (int)xpos + 1;
                }
            }
            if (xvel < 0)
            {
                xvel += speed;
                if (xvel > -speed)
                    xvel = 0;
                if (nearby[3])
                {
                    xvel = 0;
                    xpos = (int)xpos;
                }
            }

            if (yvel > 0)
                if (nearby[1])
                {
                    yvel = 0;
                    ypos = (int)ypos;
                }
            if (yvel < 0)
                if (nearby[7])
                {
                    yvel = 0;
                    ypos = (int)ypos + 1;
                }
            if (nearby[4])
                yvel += .5;
        }
        public void PlayerUpdate()
        {
            if (xvel > 4)
                xvel = 4;
            if (xvel < -4)
                xvel = -4;

            xpos += speed * xvel;
            ypos += speed * yvel;
        }
    }
}
