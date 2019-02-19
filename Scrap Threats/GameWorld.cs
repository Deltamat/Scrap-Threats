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
        public static double globalGameTime;
        public static double elapsedTime;
        private double foodUpkeepTimer;
        public static int foodUpkeep;
        public static int food = 1000;
        public static int scrap;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background;
        List<Worker> activeWorkers = new List<Worker>();
        List<Building> buildings = new List<Building>();
        Worker worker;
        public static Stockpile stockpile;
        public static Farm farm;
        Random rng = new Random();
        public static Rectangle mouseClickRectangle;
        public static HashSet<GameObject> gameObjects = new HashSet<GameObject>();
        public static GameObject selectedUnit;
        

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

            farm = new Farm(new Vector2(1700, 250), "farm_0");
            stockpile = new Stockpile(new Vector2(960, 540), "stockpile_0");
            buildings.Add(farm);
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
                mouseClickRectangle = new Rectangle(Mouse.GetState().Position.X, Mouse.GetState().Position.Y, 1, 1);
                selectedUnit = null;
            }

            if (Mouse.GetState().RightButton is ButtonState.Pressed)
            {
                if (selectedUnit != null)
                {
                    selectedUnit.waypoint = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
                    selectedUnit.waypointRectangle = new Rectangle((int)selectedUnit.waypoint.X, (int)selectedUnit.waypoint.Y, 1, 1);
                }
            }

            foreach (GameObject go in gameObjects)
            {
                if (go.CollisionBox.Intersects(mouseClickRectangle) && go is Worker)
                {
                    selectedUnit = go;
                }

                foreach (GameObject other in gameObjects)
                {
                    if (go != other && go.IsColliding(other))
                    {
                        go.DoCollision(other);
                    }
                }
            }

            foreach (Worker item in activeWorkers)
            {
                if (item.CollisionBox.Intersects(mouseClickRectangle))
                {
                    selectedUnit = item;
                }
            }
                        
            //Food upkeep, 60 sec timer
            if (foodUpkeepTimer >= 60)
            {                
                if (food > foodUpkeep) //'Pays' upkeep
                {
                    food -= foodUpkeep;
                }
                else //Workers 'starve' to death
                {
                    int missingFood = foodUpkeep - food;
                    food = 0;
                    for (int i = 0; i < missingFood; i++) //For every piece of missing food, kill one worker
                    {
                        int deadWorker = rng.Next(0, activeWorkers.Count); //Picks a random worker
                        if (activeWorkers.Count > 0) //Ensures that there are workers to kill
                        {
                            activeWorkers[deadWorker].alive = false;
                            activeWorkers.Remove(activeWorkers[deadWorker]);
                        }                       
                    }
                }
                foodUpkeepTimer = 0;
            }

            globalGameTime = gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += globalGameTime;
            foodUpkeepTimer += globalGameTime;
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
