using System;

namespace TileTwo
{
    internal class Entity
    {
        private static int InventorySize = 48;
        private static double PlayerSize = .8;

        public int ID = 0;
        public double xPos = 2000;
        public double yPos = 0;
        public double xVel = 0;
        public double yVel = 0;
        public bool CanJump = false;
        public bool[] Nearby = new bool[9];
        public double Speed = 0.03125;
        public bool InventoryOpen = false;
        public bool CharOpen = false;
        private Block Block = new Block();
        public int[,] Inventory = new int[48, 3];
        public int[,] Equip = new int[2, 3];
        public int Health = 0;
        public int Mana = 0;

        public void init()
        {
            for (int x = 0; x < 48; x++)
            {
                Inventory[x, 0] = 0;//ID
                Inventory[x, 1] = 0;//Count
                Inventory[x, 2] = 0;//Meta
            }
            Equip[0, 0] = 0;
            Equip[0, 1] = 0;
            Equip[0, 2] = 0;
            Equip[1, 0] = 0;
            Equip[1, 1] = 0;
            Equip[1, 2] = 0;

            Health = 25;
            Mana = 25;
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
                        xPos = Math.Round(xPos) - (1 - PlayerSize) / 2;
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