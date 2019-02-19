using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Scrap_Threats
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameWorld : Game
    {
        public static double elapsedTime;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background;
        List<Worker> activeWorkers = new List<Worker>();
        List<Building> buildings = new List<Building>();
        Thread t;
        Worker worker;
        public static Building stockpile;
        public static Scrapyard scrapyard;
        Random rng = new Random();
        public static Rectangle mouseClickRectangle;
        Vector2 selectionBoxOrigin;
        public static HashSet<GameObject> gameObjects = new HashSet<GameObject>();
        public static List<GameObject> selectedUnit = new List<GameObject>();
        public static int scrap;
        SpriteFont font;
        

        private static ContentManager content;
        public static ContentManager ContentManager
        {
            get
            {
                return content;
            }
        }

        public GameWorld()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = Content;
            //Maximises
            var form = (Form)Form.FromHandle(Window.Handle);
            form.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Creates a rectangle whithin the bounds of the window
        /// </summary>
        public Rectangle ScreenSize
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Bounds;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GameTime gameTime = new GameTime();
            IsMouseVisible = true;
            for (int i = 0; i < 10; i++)
            {
                worker = new Worker(new Vector2(rng.Next(100, 1800), rng.Next(100, 900)), "test");
                activeWorkers.Add(worker);
            }
           
            stockpile = new Building(new Vector2(960, 540), "stockpile_empty");
            scrapyard = new Scrapyard(new Vector2(200, 540), "stockpile_empty");
            buildings.Add(scrapyard);
            buildings.Add(stockpile);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background");
            font = Content.Load<SpriteFont>("font");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if (Mouse.GetState().LeftButton is ButtonState.Pressed)
            {

                if (selectionBoxOrigin == new Vector2(-100))
                {
                    selectionBoxOrigin = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
                    selectedUnit.RemoveRange(0, selectedUnit.Count);
                }

                mouseClickRectangle = new Rectangle((int)selectionBoxOrigin.X, (int)selectionBoxOrigin.Y, (int)(Mouse.GetState().Position.X - selectionBoxOrigin.X), (int)(Mouse.GetState().Position.Y - selectionBoxOrigin.Y));
            }
            else
            {
                selectionBoxOrigin = new Vector2(-100);
            }
            if (Mouse.GetState().RightButton is ButtonState.Pressed)
            {
                if (selectedUnit.Count > 0)
                {
                    foreach (var item in selectedUnit)
                    {
                        item.waypoint = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
                        item.waypointRectangle = new Rectangle((int)item.waypoint.X, (int)item.waypoint.Y, 1, 1);
                    }
                }
            }

            //foreach (GameObject go in gameObjects)
            //{
            //    //go.Update(gameTime);
            //    if (go.CollisionBox.Intersects(mouseClickRectangle) && go is Worker)
            //    {
            //        if (mouseClickRectangle.Width > 10 && mouseClickRectangle.Height > 10)
            //        {
            //            selectedUnit.Add(go);
            //        }
            //        else
            //        {
            //            selectedUnit.Add(go);
            //            break;
            //        }
            //    }

            //    foreach (GameObject other in gameObjects)
            //    {
            //        if (go != other && go.IsColliding(other))
            //        {
            //            go.DoCollision(other);
            //        }
            //    }
            //}

            foreach (Worker item in activeWorkers)
            {
                if (item.CollisionBox.Intersects(mouseClickRectangle))
                {
                    //selectedUnit.Add(item);

                    if (mouseClickRectangle.Width > 10 && mouseClickRectangle.Height > 10)
                    {
                        selectedUnit.Add(item);
                    }
                    else
                    {
                        selectedUnit.Add(item);
                        break;
                    }
                }
            }

            elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;
            mouseClickRectangle = new Rectangle(-8888, -9999, 1, 1);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(background, ScreenSize, Color.White);

            foreach (Building building in buildings)
            {
                building.Draw(spriteBatch);
            }

            foreach (Worker worker in activeWorkers)
            {
                worker.Draw(spriteBatch);
            }
            foreach (GameObject item in gameObjects)
            {
                item.Draw(spriteBatch);
            }

            spriteBatch.DrawString(font, $"{scrap}", new Vector2(10), Color.White);
            spriteBatch.Draw(stockpile.Sprite, mouseClickRectangle, Color.Green);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void UpdateWorkers(GameTime gameTime, Worker worker)
        {
            while (true)
            {
                worker.Update(gameTime);
            }
        }
    }
}
