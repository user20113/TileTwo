using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;
using System.Xml;

namespace TileTwo
{
    internal class Main : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch g;
        private Texture2D TileSet;
        private World World = new World();
        private Entity Player = new Entity();
        private Entity SpawnPoint = new Entity();
        private Block Target = new Block();

        private static double PlayerSize = .8;
        private static double UIScale = .8;
        private static double BlockInUIScale = .6;
        private static double StaticSize = 12;
        private static int TileSetWidth = 16;
        private static int InventorySize = 48;
        private static int InventoryRowSize = 8;
        private static double UITopPoint = .9;
        private static double UISidePoint = .1;

        public int BlockSize = 12;
        public int LastScrollValue = 0;
        public int MouseSector = 0;
        public int MouseX = 0;
        public int MouseY = 0;
        public int[,] OnMouse = new int[1, 3];

        public bool ChestOpen = false;
        public bool InventoryOneShot = false;
        public bool CharOneShot = false;
        public bool ChestOneShot = false;
        public bool LeftMouseOneShot = false;
        public bool RightMouseOneShot = false;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.IsMouseVisible = true;
            //INIT
            Player.init();
            World.GenWorld(Player);
            SpawnPoint.yPos = Player.yPos - 1;
            SpawnPoint.ID = 136;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Task.Run(() =>
            {
            });
            //RUN

            KeyRegistry();
            collision(Player);
            Player.Gravity();
            Player.Physics();
            Player.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            //400* (4*100<--)
            base.Draw(gameTime);
            g.Begin(SpriteSortMode.Immediate);
            //Draw

            for (int x = -800; x < 1600; x += 48)
            {
                for (int y = -800; y < 1600; y += 48)
                {
                    g.Draw(TileSet,
                        new Rectangle(
                            x,
                            y,
                            48,
                            48),
                        new Rectangle(
                            217 % TileSetWidth * 16,
                            (int)(217 / TileSetWidth) * 16,
                            48,
                            48),
                        Color.White);
                }
            }                                   //Background
            for (int x = (int)Player.xPos - 100; x < (int)Player.xPos + 100; x++)
            {
                for (int y = (int)Player.yPos - 100; y < (int)Player.yPos + 100; y++)
                {
                    if (World.world[x, y].animate)
                    {                                                                  //Checkifanimating
                        World.world[x, y].meta++;                                      //Increase animate
                        if (World.world[x, y].animationTotal < World.world[x, y].meta) //CheckIfLoopPoint
                            World.world[x, y].meta = 0;                                //resetAnimation
                    }
                    //Draw
                    g.Draw(TileSet,
                        new Rectangle(
                            (int)((x - Player.xPos) * BlockSize + GraphicsDevice.Viewport.Width / 2),
                            (int)(GraphicsDevice.Viewport.Height / 2 - ((y - Player.yPos) * BlockSize)),
                            BlockSize,
                            BlockSize),
                        new Rectangle(
                            (int)(((World.world[x, y].ID * 4 + World.world[x, y].meta) % TileSetWidth) * 16),
                             (int)((World.world[x, y].ID * 4 + World.world[x, y].meta) / TileSetWidth) * 16,
                            16,
                            16),
                        Color.White);
                }
            }   //blockRender
            for (int EntityLayer = 0; EntityLayer < 4; EntityLayer++)
            {
                g.Draw(TileSet,
                new Rectangle(
                    (int)(GraphicsDevice.Viewport.Width / 2 + (1 - PlayerSize) / 2 * BlockSize),
                    (int)(GraphicsDevice.Viewport.Height / 2 + (1 - PlayerSize) / 2 * BlockSize),
                    (int)(BlockSize * PlayerSize),
                    (int)(BlockSize * PlayerSize)),
                new Rectangle(
                    (128) % TileSetWidth * 16 + EntityLayer * 32,
                    (128) / TileSetWidth * 16,
                32,
                32),
                Color.White);
            }               //PlayerRender
            for (int EntityLayer = 0; EntityLayer < 3; EntityLayer++)
            {
                Entity entity = SpawnPoint;//editwhenlistused
                g.Draw(TileSet,
                        new Rectangle(
                            (int)(((int)entity.xPos - Player.xPos) * BlockSize + GraphicsDevice.Viewport.Width / 2),
                            (int)(GraphicsDevice.Viewport.Height / 2 - (((int)entity.yPos - Player.yPos) * BlockSize)),
                            BlockSize,
                            BlockSize),
                new Rectangle(
                    (entity.ID) % TileSetWidth * 16 + EntityLayer * 32,
                    (entity.ID) / TileSetWidth * 16,
                32,
                32),
                Color.White);
            }               //GeneralEntityCode
            {
                g.Draw(TileSet,
                new Rectangle(
                    (int)(GraphicsDevice.Viewport.Width / 2 + (1 - PlayerSize) / 2 * BlockSize - 2 * PlayerSize * BlockSize + (int)(MouseSector % 3) * PlayerSize * BlockSize * 1.5),
                    (int)(GraphicsDevice.Viewport.Height / 2 + (1 - PlayerSize) / 2 * BlockSize - 2 * PlayerSize * BlockSize + (int)(MouseSector / 3) * PlayerSize * BlockSize * 1.5),
                    (int)(BlockSize * PlayerSize * 2),
                    (int)(BlockSize * PlayerSize * 2)),
                new Rectangle(
                    (208) % TileSetWidth * 16 + (int)(MouseSector % 3) * 48,
                    (208) / TileSetWidth * 16 + (int)(MouseSector / 3) * 48,
                48,
                48),
                Color.White);
            }                                                                        //PlayerMouseRender
            if (Player.InventoryOpen)
            {
                for (int x = 0; x < InventorySize; x++)
                {
                    g.Draw(TileSet,                                                    //backtiles
                        new Rectangle(
                            (int)(GraphicsDevice.Viewport.Width / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)(((x % InventoryRowSize) - 5) * 16 - (InventorySize / InventoryRowSize) * StaticSize) * UIScale),
                            (int)(GraphicsDevice.Viewport.Height / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)((x / InventoryRowSize) * 16 - (InventorySize / InventoryRowSize + 2) * StaticSize) * UIScale),
                            (int)(16 * UIScale),
                            (int)(16 * UIScale)),
                        new Rectangle(
                            223 % TileSetWidth * 16,
                            (int)(223 / TileSetWidth) * 16,
                            16,
                            16),
                        Color.White);
                    g.Draw(TileSet,                                                    //Items
                        new Rectangle(
                            (int)(GraphicsDevice.Viewport.Width / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)(((x % InventoryRowSize) - 5) * 16 - (InventorySize / InventoryRowSize) * StaticSize) * UIScale + StaticSize * UIScale * (1 - BlockInUIScale) / 2),
                            (int)(GraphicsDevice.Viewport.Height / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)((x / InventoryRowSize) * 16 - (InventorySize / InventoryRowSize + 2) * StaticSize) * UIScale + StaticSize * UIScale * (1 - BlockInUIScale) / 2),
                            (int)(16 * UIScale * BlockInUIScale),
                            (int)(16 * UIScale * BlockInUIScale)),
                        new Rectangle(
                            (Player.Inventory[x, 0] * 4 + Player.Inventory[x, 2]) % TileSetWidth * 16,
                            (int)((Player.Inventory[x, 0] * 4 + Player.Inventory[x, 2]) / TileSetWidth) * 16,
                            16,
                            16),
                        Color.White);
                    if (ChestOpen)
                    {
                        g.Draw(TileSet,                                               //backtiles
                            new Rectangle(
                                (int)(GraphicsDevice.Viewport.Width / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)(((x % InventoryRowSize) - 2.5) * 16 + ((InventorySize / InventoryRowSize + InventorySize / InventoryRowSize) / 2.25) * StaticSize) * UIScale),
                                (int)(GraphicsDevice.Viewport.Height / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)((x / InventoryRowSize) * 16 - (InventorySize / InventoryRowSize + 2) * StaticSize) * UIScale),
                                (int)(16 * UIScale),
                                (int)(16 * UIScale)),
                            new Rectangle(
                                223 % TileSetWidth * 16,
                                (int)(223 / TileSetWidth) * 16,
                                16,
                                16),
                            Color.White);
                    }
                }                              //inventory/Chest
                if (Player.CharOpen)
                {                                                                    //backTile
                    g.Draw(TileSet,
                        new Rectangle(
                            (int)(GraphicsDevice.Viewport.Width / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)((InventoryRowSize - 5) * 16 - (InventorySize / InventoryRowSize) * StaticSize) * UIScale),
                            (int)(GraphicsDevice.Viewport.Height / 2 + (1 - PlayerSize) / 2 * StaticSize - (int)((InventorySize / InventoryRowSize + 2) * StaticSize) * UIScale),
                            (int)(4 * StaticSize * UIScale),
                            (int)(4 * StaticSize * UIScale)),
                        new Rectangle(
                            217 % TileSetWidth * 16,
                            (int)(217 / TileSetWidth) * 16,
                            48,
                            48),
                        Color.White);

                    for (int EntityLayer = 0; EntityLayer < 4; EntityLayer++)
                    {
                        g.Draw(TileSet,                                             //User
                        new Rectangle(
                            (int)(GraphicsDevice.Viewport.Width / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)((InventoryRowSize - 5) * 16 - (InventorySize / InventoryRowSize) * StaticSize) * UIScale) + (int)(StaticSize * UIScale),
                            (int)(GraphicsDevice.Viewport.Height / 2 + (1 - PlayerSize) / 2 * StaticSize - (int)((InventorySize / InventoryRowSize + 2) * StaticSize) * UIScale) + (int)(StaticSize * UIScale),
                            (int)(2 * StaticSize * UIScale),
                            (int)(2 * StaticSize * UIScale)),
                        new Rectangle(
                        (128) % TileSetWidth * 16 + EntityLayer * 32,
                        (128) / TileSetWidth * 16,
                        32,
                        32),
                        Color.White);
                        //backTile
                        for (int x = 0; x < 3; x++)
                        {
                            g.Draw(TileSet,                                         //CharTiles
                                new Rectangle(
                                    (int)(GraphicsDevice.Viewport.Width / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)((InventoryRowSize - 4.875 + x) * 16 - (InventorySize / InventoryRowSize) * StaticSize) * UIScale),
                                    (int)(GraphicsDevice.Viewport.Height / 2 + (1 - PlayerSize) / 2 * StaticSize - (int)((InventorySize / InventoryRowSize - 1) * StaticSize) * UIScale),
                                    (int)(StaticSize * UIScale),
                                    (int)(StaticSize * UIScale)),
                                new Rectangle(
                                    (220 + x) % TileSetWidth * 16,
                                    (int)((220 + x) / TileSetWidth) * 16,
                                    16,
                                    16),
                                Color.White);
                        }
                    }
                }                                                                   //CharDisplay
            }                                               //Inventory
            {
                Rectangle SourceRect;
                for (int x = 0; x < 10; x++)
                {
                    SourceRect = new Rectangle(
                           (int)(270 % TileSetWidth) * 16,
                           (int)(270 / TileSetWidth) * 16,
                           16,
                           16);
                    if (x == 0)
                    {
                        SourceRect = new Rectangle(
                               (int)(268 % TileSetWidth) * 16,
                               (int)(268 / TileSetWidth) * 16,
                               16,
                               16);
                    }
                    if (x == 9)
                    {
                        SourceRect = new Rectangle(
                               (int)(269 % TileSetWidth) * 16,
                               (int)(269 / TileSetWidth) * 16,
                               16,
                               16);
                    }

                    g.Draw(TileSet,                                                  //HotBar
                        new Rectangle(
                            (int)(GraphicsDevice.Viewport.Width * UISidePoint + ((x - 1) * (GraphicsDevice.Viewport.Width * (1 - (2 * UISidePoint)) / 25))),
                            (int)(GraphicsDevice.Viewport.Height * UITopPoint + 8 + (GraphicsDevice.Viewport.Height * (1 - 2 * UITopPoint) / 20)) - (int)(GraphicsDevice.Viewport.Height * -1 * (1 - 2 * UITopPoint) / 20),
                            (int)(GraphicsDevice.Viewport.Height * -1 * (1 - 2 * UITopPoint) / 20),
                            (int)(GraphicsDevice.Viewport.Height * -1 * (1 - 2 * UITopPoint) / 20)),
                       SourceRect,
                    Color.White);

                    if (x > 0 && x < 9)
                        SourceRect = new Rectangle(
                               (int)(Player.Inventory[x - 1, 0] % TileSetWidth) * 4 * 16,
                               (int)(Player.Inventory[x - 1, 0] / TileSetWidth) * 4 * 16,
                               16,
                               16);
                    if (x == 0)
                        SourceRect = new Rectangle(
                               (int)(Player.Equip[0, 0] % TileSetWidth) * 4 * 16,
                               (int)(Player.Equip[0, 0] / TileSetWidth) * 4 * 16,
                               16,
                               16); ;
                    if (x == 9)
                        SourceRect = new Rectangle(
                               (int)(Player.Equip[1, 0] % TileSetWidth) * 4 * 16,
                               (int)(Player.Equip[1, 0] / TileSetWidth) * 4 * 16,
                               16,
                               16); ;

                    g.Draw(TileSet,                                                  //HotBarItem
                        new Rectangle(
                            (int)(GraphicsDevice.Viewport.Width * UISidePoint + ((x - 1) * (GraphicsDevice.Viewport.Width * (1 - (2 * UISidePoint)) / 25))),
                            (int)(GraphicsDevice.Viewport.Height * UITopPoint + 8 + (GraphicsDevice.Viewport.Height * (1 - 2 * UITopPoint) / 20)) - (int)(GraphicsDevice.Viewport.Height * -1 * (1 - 2 * UITopPoint) / 20),
                            (int)(GraphicsDevice.Viewport.Height * -1 * (1 - 2 * UITopPoint) / 20),
                            (int)(GraphicsDevice.Viewport.Height * -1 * (1 - 2 * UITopPoint) / 20)),
                       SourceRect,
                    Color.White);
                }
            }                                                                        //HotBar
            {
                {
                    g.Draw(TileSet,                                                  //Health
                        new Rectangle(
                            (int)(GraphicsDevice.Viewport.Width * UISidePoint - GraphicsDevice.Viewport.Width * (1 - (2 * UISidePoint)) / 10),
                            (int)(GraphicsDevice.Viewport.Height * UITopPoint + 8 + (1 - 2 * UITopPoint) / 20 * GraphicsDevice.Viewport.Height),
                            (int)(GraphicsDevice.Viewport.Width * (1 - (UISidePoint * 2)) * Player.Health / 100),
                            (int)(GraphicsDevice.Viewport.Height * (1 - UITopPoint) / 10 * 1)),
                        new Rectangle(
                            239 % TileSetWidth * 16,
                            (int)(239 / TileSetWidth) * 16,
                            16,
                            16),
                    Color.White);
                }

                g.Draw(TileSet,                                                  //Start
                            new Rectangle(
                                (int)(GraphicsDevice.Viewport.Width * UISidePoint - GraphicsDevice.Viewport.Width * (1 - (2 * UISidePoint)) / 10),
                                (int)(GraphicsDevice.Viewport.Height * UITopPoint + 8 + (1 - 2 * UITopPoint) / 20 * GraphicsDevice.Viewport.Height),
                                16,
                                (int)(GraphicsDevice.Viewport.Height * (1 - UITopPoint) / 10 * 1)),
                            new Rectangle(
                                236 % TileSetWidth * 16,
                                (int)(236 / TileSetWidth) * 16,
                                16,
                                16),
                        Color.White);
                g.Draw(TileSet,                                                  //Mid
                    new Rectangle(
                        (int)(GraphicsDevice.Viewport.Width * UISidePoint + 16 - GraphicsDevice.Viewport.Width * (1 - (2 * UISidePoint)) / 10),
                        (int)(GraphicsDevice.Viewport.Height * UITopPoint + 8 + (1 - 2 * UITopPoint) / 20 * GraphicsDevice.Viewport.Height),
                        (int)(GraphicsDevice.Viewport.Width * (1 - 2 * UISidePoint) - 32),
                        (int)(GraphicsDevice.Viewport.Height * (1 - UITopPoint) / 10 * 1)),
                    new Rectangle(
                        237 % TileSetWidth * 16,
                        (int)(237 / TileSetWidth) * 16,
                        16,
                        16),
                Color.White);
                g.Draw(TileSet,                                                  //End
                    new Rectangle(
                        (int)(GraphicsDevice.Viewport.Width * (1 - UISidePoint) - 16 - GraphicsDevice.Viewport.Width * (1 - (2 * UISidePoint)) / 10),
                        (int)(GraphicsDevice.Viewport.Height * UITopPoint + 8 + (1 - 2 * UITopPoint) / 20 * GraphicsDevice.Viewport.Height),
                        16,
                        (int)(GraphicsDevice.Viewport.Height * (1 - UITopPoint) / 10 * 1)),
                    new Rectangle(
                        238 % TileSetWidth * 16,
                        (int)(238 / TileSetWidth) * 16,
                        16,
                        16),
                Color.White);
            }                                                                        //TileHealth
            {
                g.Draw(TileSet,                                                  //Mana
                    new Rectangle(
                        (int)(GraphicsDevice.Viewport.Width * UISidePoint - GraphicsDevice.Viewport.Width * (1 - (2 * UISidePoint)) / 10),
                        (int)(GraphicsDevice.Viewport.Height * UITopPoint + 8 + (1 - 2 * UITopPoint) / 20 * GraphicsDevice.Viewport.Height + 8),
                        (int)(GraphicsDevice.Viewport.Width * (1 - (UISidePoint * 2)) * Player.Mana / 100),
                        (int)(GraphicsDevice.Viewport.Height * (1 - UITopPoint) / 10 * 1)),
                    new Rectangle(
                        255 % TileSetWidth * 16,
                        (int)(255 / TileSetWidth) * 16,
                        16,
                        16),
                Color.White);

                g.Draw(TileSet,                                                  //Start
                        new Rectangle(
                            (int)(GraphicsDevice.Viewport.Width * UISidePoint - GraphicsDevice.Viewport.Width * (1 - (2 * UISidePoint)) / 10),
                            (int)(GraphicsDevice.Viewport.Height * UITopPoint + 8 + (1 - 2 * UITopPoint) / 20 * GraphicsDevice.Viewport.Height + 8),
                            16,
                            (int)(GraphicsDevice.Viewport.Height * (1 - UITopPoint) / 10 * 1)),
                        new Rectangle(
                            252 % TileSetWidth * 16,
                            (int)(252 / TileSetWidth) * 16,
                            16,
                            16),
                        Color.White);
                g.Draw(TileSet,                                                  //Mid
                    new Rectangle(
                        (int)(GraphicsDevice.Viewport.Width * UISidePoint + 16 - GraphicsDevice.Viewport.Width * (1 - (2 * UISidePoint)) / 10),
                        (int)(GraphicsDevice.Viewport.Height * UITopPoint + 8 + (1 - 2 * UITopPoint) / 20 * GraphicsDevice.Viewport.Height + 8),
                        (int)(GraphicsDevice.Viewport.Width * (1 - 2 * UISidePoint) - 32),
                        (int)(GraphicsDevice.Viewport.Height * (1 - UITopPoint) / 10 * 1)),
                    new Rectangle(
                        253 % TileSetWidth * 16,
                        (int)(253 / TileSetWidth) * 16,
                        16,
                        16),
                    Color.White);
                g.Draw(TileSet,                                                  //End
                    new Rectangle(
                        (int)(GraphicsDevice.Viewport.Width * (1 - UISidePoint) - 16 - GraphicsDevice.Viewport.Width * (1 - (2 * UISidePoint)) / 10),
                        (int)(GraphicsDevice.Viewport.Height * UITopPoint + 8 + (1 - 2 * UITopPoint) / 20 * GraphicsDevice.Viewport.Height + 8),
                        16,
                        (int)(GraphicsDevice.Viewport.Height * (1 - UITopPoint) / 10 * 1)),
                    new Rectangle(
                        254 % TileSetWidth * 16,
                        (int)(254 / TileSetWidth) * 16,
                        16,
                        16),
                Color.White);
            }                                                                        //Mana

            g.End();
        }

        public void collision(Entity player)
        {
            double OffSet = .3;
            double LeftX = Player.xPos + (1 - PlayerSize) / 2;
            double RightX = LeftX + PlayerSize;
            double BotY = Player.yPos + (1 - PlayerSize) / 2;
            double TopY = BotY + PlayerSize - (1 - PlayerSize) / 2;
            player.Nearby[1] = !World.world[(int)(LeftX + OffSet), (int)(TopY)].GetPassable();
            player.Nearby[1] |= !World.world[(int)(RightX - OffSet), (int)(TopY)].GetPassable();
            player.Nearby[3] = !World.world[(int)(LeftX), (int)(TopY + -OffSet)].GetPassable();
            player.Nearby[3] |= !World.world[(int)(LeftX), (int)(BotY + OffSet)].GetPassable();
            player.Nearby[5] = !World.world[(int)(RightX), (int)(BotY + OffSet)].GetPassable();
            player.Nearby[5] |= !World.world[(int)(RightX), (int)(TopY - OffSet)].GetPassable();
            player.Nearby[7] = !World.world[(int)(RightX - OffSet), (int)(BotY)].GetPassable();
            player.Nearby[7] |= !World.world[(int)(LeftX + OffSet), (int)(BotY)].GetPassable();
            player.Nearby[4] = !World.world[(int)(LeftX + OffSet), (int)(TopY - OffSet)].GetPassable();
            player.Nearby[4] &= !World.world[(int)(RightX - OffSet), (int)(BotY + OffSet)].GetPassable();
        }

        public void KeyRegistry()
        {
            MouseState MState = Mouse.GetState();
            KeyboardState State = Keyboard.GetState();
            GetMouseState(MState);                                            //init
            {
                if (State.IsKeyDown(Keys.Space))
                    if (Player.Nearby[7])
                        Player.yVel += 10;
                if (State.IsKeyDown(Keys.A))
                    if (!Player.Nearby[3])
                        Player.xVel--;
                if (State.IsKeyDown(Keys.D))
                    if (!Player.Nearby[5])
                        Player.xVel++;
                if (State.IsKeyDown(Keys.W))
                    if (!Player.Nearby[1])
                        Player.yVel += 2;
                if (State.IsKeyDown(Keys.S))
                    if (!Player.Nearby[7])
                        Player.yVel--;
            }                                               //Movement
            {
                if (MState.ScrollWheelValue > LastScrollValue)
                {
                    BlockSize += 2;
                    UIScale += .2;
                }
                if (MState.ScrollWheelValue < LastScrollValue)
                {
                    BlockSize -= 2;
                    UIScale -= .2;
                }
                if (LastScrollValue != MState.ScrollWheelValue)
                {
                    LastScrollValue = MState.ScrollWheelValue;
                }
            }                                               //ScrollWheel
            {
                double LeftX = Player.xPos + (1 - PlayerSize) / 2;
                double RightX = LeftX + PlayerSize;
                double BotY = Player.yPos + (1 - PlayerSize) / 2;
                double TopY = BotY + PlayerSize - (1 - PlayerSize) / 2;
                double Dist = .5;
                if (State.IsKeyDown(Keys.Q))
                {
                    switch (MouseSector)
                    {
                        case 0:
                            Target = World.world[(int)(LeftX - Dist), (int)(TopY + Dist)];
                            if (Target.ID != 0)
                                for (int x = 0; x < InventorySize; x++)
                                {
                                    if (Target.ID != 0)
                                        if (Player.Inventory[x, 0] == 0 || Player.Inventory[x, 0] == Target.ID)
                                        {
                                            Player.Inventory[x, 0] = Target.ID;
                                            Player.Inventory[x, 1]++;
                                            Target.ID = 0;
                                        }
                                }

                            break;

                        case 1:
                            Target = World.world[(int)(LeftX + Dist), (int)(TopY + Dist)];
                            if (Target.ID != 0)
                                for (int x = 0; x < InventorySize; x++)
                                {
                                    if (Target.ID != 0)
                                        if (Player.Inventory[x, 0] == 0 || Player.Inventory[x, 0] == Target.ID)
                                        {
                                            Player.Inventory[x, 0] = Target.ID;
                                            Player.Inventory[x, 1]++;
                                            Target.ID = 0;
                                        }
                                }

                            break;

                        case 2:
                            Target = World.world[(int)(RightX + Dist), (int)(TopY + Dist)];
                            if (Target.ID != 0)
                                for (int x = 0; x < InventorySize; x++)
                                {
                                    if (Target.ID < 4)
                                        if (Player.Inventory[x, 0] == 0 || Player.Inventory[x, 0] == Target.ID)
                                        {
                                            Player.Inventory[x, 0] = Target.ID;
                                            Player.Inventory[x, 1]++;
                                            Target.ID = 0;
                                        }
                                }

                            break;

                        case 3:
                            Target = World.world[(int)(LeftX - Dist), (int)(TopY - Dist)];
                            if (Target.ID != 0)
                                for (int x = 0; x < InventorySize; x++)
                                {
                                    if (Target.ID != 0)
                                        if (Player.Inventory[x, 0] == 0 || Player.Inventory[x, 0] == Target.ID)
                                        {
                                            Player.Inventory[x, 0] = Target.ID;
                                            Player.Inventory[x, 1]++;
                                            Target.ID = 0;
                                        }
                                }

                            break;

                        case 4:
                            ;
                            break;

                        case 5:
                            Target = World.world[(int)(RightX + Dist), (int)(TopY - Dist)];
                            if (Target.ID != 0)
                                for (int x = 0; x < InventorySize; x++)
                                {
                                    if (Target.ID != 0)
                                        if (Player.Inventory[x, 0] == 0 || Player.Inventory[x, 0] == Target.ID)
                                        {
                                            Player.Inventory[x, 0] = Target.ID;
                                            Player.Inventory[x, 1]++;
                                            Target.ID = 0;
                                        }
                                }

                            break;

                        case 6:
                            Target = World.world[(int)(LeftX - Dist), (int)(BotY - Dist)];
                            if (Target.ID != 0)
                                for (int x = 0; x < InventorySize; x++)
                                {
                                    if (Target.ID != 0)
                                        if (Player.Inventory[x, 0] == 0 || Player.Inventory[x, 0] == Target.ID)
                                        {
                                            Player.Inventory[x, 0] = Target.ID;
                                            Player.Inventory[x, 1]++;
                                            Target.ID = 0;
                                        }
                                }

                            break;

                        case 7:
                            Target = World.world[(int)(LeftX + Dist), (int)(BotY - Dist)];
                            if (Target.ID != 0)
                                for (int x = 0; x < InventorySize; x++)
                                {
                                    if (Target.ID != 0)
                                        if (Player.Inventory[x, 0] == 0 || Player.Inventory[x, 0] == Target.ID)
                                        {
                                            Player.Inventory[x, 0] = Target.ID;
                                            Player.Inventory[x, 1]++;
                                            Target.ID = 0;
                                        }
                                }

                            break;

                        case 8:
                            Target = World.world[(int)(RightX + Dist), (int)(BotY - Dist)];
                            if (Target.ID != 0)
                                for (int x = 0; x < InventorySize; x++)
                                {
                                    if (Target.ID != 0)
                                        if (Player.Inventory[x, 0] == 0 || Player.Inventory[x, 0] == Target.ID)
                                        {
                                            Player.Inventory[x, 0] = Target.ID;
                                            Player.Inventory[x, 1]++;
                                            Target.ID = 0;
                                        }
                                }

                            break;

                        default:
                            ;
                            break;
                    }
                }
                if (State.IsKeyDown(Keys.E))
                {
                    switch (MouseSector)
                    {
                        case 0:
                            World.world[(int)(LeftX - Dist), (int)(TopY + Dist)].ID = 0;
                            break;

                        case 1:
                            World.world[(int)(LeftX + Dist), (int)(TopY + Dist)].ID = 0;
                            break;

                        case 2:
                            World.world[(int)(RightX + Dist), (int)(TopY + Dist)].ID = 0;
                            break;

                        case 3:
                            World.world[(int)(LeftX - Dist), (int)(TopY - Dist)].ID = 0;
                            break;

                        case 4:
                            ;
                            break;

                        case 5:
                            World.world[(int)(RightX + Dist), (int)(TopY - Dist)].ID = 0;
                            break;

                        case 6:
                            World.world[(int)(LeftX - Dist), (int)(BotY - Dist)].ID = 0;
                            break;

                        case 7:
                            World.world[(int)(LeftX + Dist), (int)(BotY - Dist)].ID = 0;
                            break;

                        case 8:
                            World.world[(int)(RightX + Dist), (int)(BotY - Dist)].ID = 0;
                            break;

                        default:
                            ;
                            break;
                    }
                }
            }                                               //Place-Break
            {
                if (MState.LeftButton == ButtonState.Pressed && !LeftMouseOneShot)
                {
                    LeftMouseOneShot = true;
                    int Truex = 0;
                    bool Found = false;
                    for (int x = 0; x < InventorySize; x++)
                    {
                        if (!Found)
                        {
                            int LeftX = (int)(GraphicsDevice.Viewport.Width / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)(((x % InventoryRowSize) - 5) * 16 - (InventorySize / InventoryRowSize) * StaticSize) * UIScale);
                            int TopY = (int)(GraphicsDevice.Viewport.Height / 2 + (1 - PlayerSize) / 2 * StaticSize + (int)((x / InventoryRowSize) * 16 - (InventorySize / InventoryRowSize + 2) * StaticSize) * UIScale);
                            if (MouseX > LeftX)
                                if (MouseX < LeftX + StaticSize * UIScale)
                                    if (MouseY > TopY)
                                        if (MouseY < TopY + StaticSize * UIScale)
                                        {
                                            int[,] temp = new int[1, 3];
                                            for (int a = 0; a < 3; a++)
                                            {
                                                temp[0, a] = OnMouse[0, a];
                                                OnMouse[0, a] = Player.Inventory[x, a];
                                                Player.Inventory[x, a] = temp[0, a];
                                            }
                                            Found = true;
                                        }
                        }
                    }
                }
                else
                if (MState.LeftButton == ButtonState.Released)
                    LeftMouseOneShot = false;
                if (Player.InventoryOpen)
                {
                }

                if (State.IsKeyDown(Keys.R) && !InventoryOneShot)
                {
                    Player.InventoryOpen = !Player.InventoryOpen;
                    InventoryOneShot = true;
                }
                else
                    if (!State.IsKeyDown(Keys.R))
                    InventoryOneShot = false;

                if (State.IsKeyDown(Keys.T) && !ChestOneShot)
                {
                    ChestOpen = !ChestOpen;
                    ChestOneShot = true;
                }
                else
                    if (!State.IsKeyDown(Keys.T))
                    ChestOneShot = false;
                if (State.IsKeyDown(Keys.C) && !CharOneShot)
                {
                    Player.CharOpen = !Player.CharOpen;
                    CharOneShot = true;
                }
                else
                    if (!State.IsKeyDown(Keys.C))
                    CharOneShot = false;
            }                                               //UI's
        }

        private void OnResize(object sender, EventArgs e)
        {
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            //LoadGraphics/world
            g = new SpriteBatch(GraphicsDevice);
            TileSet = Content.Load<Texture2D>("TileSet");
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            //disposejunk
        }

        public void GetMouseState(MouseState MState)
        {
            MouseX = MState.Position.X;
            MouseY = MState.Position.Y;
            if (MouseY > GraphicsDevice.Viewport.Height)
                Mouse.SetPosition(MouseX, GraphicsDevice.Viewport.Height);
            if (MouseY < 0)
                Mouse.SetPosition(MouseX, 0);
            if (MouseX > GraphicsDevice.Viewport.Width)
                Mouse.SetPosition(GraphicsDevice.Viewport.Width, MouseY);
            if (MouseX < 0)
                Mouse.SetPosition(0, MouseY);
            if (MouseX < GraphicsDevice.Viewport.Width / 2 - 50)
            {
                if (MouseY < GraphicsDevice.Viewport.Height / 2 - 75)
                {
                    MouseSector = 0;
                }
                else
                    if (MouseY > GraphicsDevice.Viewport.Height / 2 + 75)
                {
                    MouseSector = 6;
                }
                else
                {
                    MouseSector = 3;
                }
            }
            else
        if (MouseX > GraphicsDevice.Viewport.Width / 2 + 50)
            {
                if (MouseY < GraphicsDevice.Viewport.Height / 2 - 75)
                {
                    MouseSector = 2;
                }
                else
                    if (MouseY > GraphicsDevice.Viewport.Height / 2 + 75)
                {
                    MouseSector = 8;
                }
                else
                {
                    MouseSector = 5;
                }
            }
            else
            {
                if (MouseY < GraphicsDevice.Viewport.Height / 2 - 75)
                {
                    MouseSector = 1;
                }
                else
                    if (MouseY > GraphicsDevice.Viewport.Height / 2 + 75)
                {
                    MouseSector = 7;
                }
                else
                {
                    MouseSector = 4;
                }
            }
        }
    }
}