using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TileTwo
{
    class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch g;
        Texture2D TileSet;
        World World = new World();
        Player Player = new Player();
        public bool Inven = false;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }

        private void OnResize(object sender, EventArgs e)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            //INIT
            World.GenWorld();
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

        protected override void Draw(GameTime gameTime)
        {
            //400* (4*100<--)
            base.Draw(gameTime);
            g.Begin(SpriteSortMode.Immediate);
            //Draw

            for (int x = (int)Player.xpos - 100; x < (int)Player.xpos + 100; x++)
            {
                for (int y = (int)Player.ypos - 100; y < (int)Player.ypos + 100; y++)
                {
                    if (World.world[x, y].animate)                                 //Checkifanimating
                        World.world[x, y].meta++;                                  //Increase animate
                    if (World.world[x, y].animationTotal < World.world[x, y].meta) //CheckIfLoopPoint
                        World.world[x, y].meta = 0;                                //resetAnimation
                    //Draw
                    g.Draw(TileSet,
                        new Rectangle(
                            (int)((x - Player.xpos) * 10 + 400),
                            (int)(GraphicsDevice.Viewport.Height - ((y - Player.ypos) * 10 + 250)),
                            10,
                            10),
                        new Rectangle(
                            (World.world[x, y].ID + World.world[x, y].meta) % 16 * 16,
                            (int)((World.world[x, y].ID + World.world[x, y].meta) / 16) * 16,
                            16,
                            16),
                        Color.White);


                    g.Draw(TileSet,
                        new Rectangle(
                            GraphicsDevice.Viewport.Width / 2 - 5,
                            GraphicsDevice.Viewport.Height / 2 - 5,
                            10,
                            10),
                        new Rectangle(
                            250 % 16 * 16,
                            250 / 16 * 16,
                        16,
                        16),
                        Color.White);


                }

            }











            g.End();
        }

        protected override void Update(GameTime gameTime)
        {

            KeyRegistry();
            base.Update(gameTime);
            Task.Run(() =>
            {
                while (Inven)
                {
                    KeyRegistry();
                    if (!Inven) { }
                    //CloseInven();
                }

            });
            //RUN

            collision(Player);
            Player.PlayerUpdate();
            Player.PlayerGravity();
            Player.PlayerSlow();


        }
        public void KeyRegistry()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Space))
            {
                if (Player.CanJump)
                Player.yvel+=3;
                Player.CanJump = false;
            }
            if (state.IsKeyDown(Keys.A))
                Player.xvel--;
            if (state.IsKeyDown(Keys.D))
                Player.xvel++;
            if (state.IsKeyDown(Keys.E))
                Inven = !Inven;
        }
        public void collision(Player player)
        {

            player.nearby[0] = World.world[(int)Player.xpos - 1, (int)Player.ypos + 1].ID < 250;
            player.nearby[1] = World.world[(int)Player.xpos, (int)Player.ypos + 1].ID < 250;
            player.nearby[2] = World.world[(int)Player.xpos + 1, (int)Player.ypos + 1].ID < 250;


            player.nearby[3] = World.world[(int)Player.xpos - 1, (int)Player.ypos].ID < 250;
            player.nearby[4] = World.world[(int)Player.xpos, (int)Player.ypos].ID < 250;
            player.nearby[5] = World.world[(int)Player.xpos + 1, (int)Player.ypos].ID < 250;


            player.nearby[6] = World.world[(int)Player.xpos - 1, (int)Player.ypos - 1].ID < 250;
            player.nearby[7] = World.world[(int)Player.xpos, (int)Player.ypos - 1].ID < 250;
            player.nearby[8] = World.world[(int)Player.xpos + 1, (int)Player.ypos - 1].ID < 250;
        }

    }
}
