using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileTwo
{
    class Entity
    {
        public int ID = 0;
        public double xPos = 2000;
        public double yPos = 0;
        public double xVel = 0;
        public double yVel = 0;
        public bool CanJump = false;
        public bool[] Nearby = new bool[9];
        public double Speed = 0.03125;
        static double PlayerSize = .8;
        public void init()
        {
        }
        public void Gravity()
        {
            if (!Nearby[7])
                yVel -= .5;
            if (yVel < 0)
                if (!Nearby[7])
                    yVel *= 1.05;
        }
        public void Physics()
        {

            {
                if (Nearby[1])//up
                    if (yVel > 0)
                    {
                        yVel = 0;
                        yPos = (int)yPos + (1 - PlayerSize) / 2;
                    }
                if (Nearby[3])//left
                    if (xVel < 0)
                    {
                        xVel = 0;
                        xPos= Math.Round(xPos) - (1 - PlayerSize) / 2;

                    }
                if (Nearby[5])//right
                    if (xVel > 0)
                    {
                        xVel = 0;
                        xPos = (int)xPos + (1 - PlayerSize) / 2;

                    }
                if (Nearby[7])//Down
                    if (yVel < 0)
                    {
                        yVel = 0;
                        yPos = Math.Round(yPos) - (1 - PlayerSize) / 2;

                    }
                if (Nearby[4])//center
                    yPos++;

            }//collisionPhysics
            //SlowPhysics
            yVel /= 1.10;
            xVel /= 1.10;
            if (Math.Abs(yVel) < .125)
                yVel = 0;
            if (Math.Abs(xVel) < .125)
                xVel = 0;
            return;

        }
        public void Update()
        {
            yPos += yVel * Speed;
            xPos += xVel * Speed;
        }
    }
}
