using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading.Tasks;

namespace TileTwo
{
    class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch g;
        Texture2D TileSet;
        World World = new World();
        Entity Player = new Entity();
        Entity SpawnPoint = new Entity();
        public int BlockSize = 12;
        static double PlayerSize = .8;
        public int LastScrollValue = 0;

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
                            195 % 16 * 16,
                            (int)(195) * 16,
                            48,
                            48),
                        Color.White);
                }
            }                                   //background
            for (int x = (int)Player.xPos - 100; x < (int)Player.xPos + 100; x++)
            {
                for (int y = (int)Player.yPos - 100; y < (int)Player.yPos + 100; y++)
                {
                    if (World.world[x, y].animate)                                 //Checkifanimating
                        World.world[x, y].meta++;                                  //Increase animate
                    if (World.world[x, y].animationTotal < World.world[x, y].meta) //CheckIfLoopPoint
                        World.world[x, y].meta = 0;                                //resetAnimation
                    //Draw
                    g.Draw(TileSet,
                        new Rectangle(
                            (int)((x - Player.xPos) * BlockSize + GraphicsDevice.Viewport.Width / 2),
                            (int)(GraphicsDevice.Viewport.Height / 2 - ((y - Player.yPos) * BlockSize)),
                            BlockSize,
                            BlockSize),
                        new Rectangle(
                            (World.world[x, y].ID + World.world[x, y].meta) % 16 * 16,
                            (int)((World.world[x, y].ID + World.world[x, y].meta) / 16) * 16,
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
                    (128) % 16 * 16 + EntityLayer * 32,
                    (128) / 16 * 16,
                32,
                32),
                Color.White);
            }               //playerRender
            for (int EntityLayer = 0; EntityLayer < 2; EntityLayer++)
            {
                Entity entity = SpawnPoint;
                g.Draw(TileSet,
                        new Rectangle(
                            (int)(((int)entity.xPos - Player.xPos) * BlockSize + GraphicsDevice.Viewport.Width / 2),
                            (int)(GraphicsDevice.Viewport.Height / 2 - (((int)entity.yPos - Player.yPos) * BlockSize)),
                            BlockSize,
                            BlockSize),
                new Rectangle(
                    (entity.ID) % 16 * 16 + EntityLayer * 32,
                    (entity.ID) / 16 * 16,
                32,
                32),
                Color.White);
            }               //generalEntityCode                                                    //HitBox Debug
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
                    Player.yVel+=2;
            if (State.IsKeyDown(Keys.S))
                if (!Player.Nearby[7])
                    Player.yVel--;
            if (MState.ScrollWheelValue > LastScrollValue)
                BlockSize++;
            if (MState.ScrollWheelValue < LastScrollValue)
                BlockSize--;

            if (LastScrollValue != MState.ScrollWheelValue)
            {
                LastScrollValue = MState.ScrollWheelValue;
            }
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
    }
}
