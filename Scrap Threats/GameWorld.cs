using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Scrap_Threats
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameWorld : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background;
        List<Worker> ActiveWorkers = new List<Worker>();
        Worker a;
        Thread t;
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
            //Sets the window size
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
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
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            for (int i = 0; i < 10; i++)
            {
                ActiveWorkers.Add(new Worker(new Vector2(rng.Next(100,1800), rng.Next(100,900)), "test"));
            }

            GameTime gameTime = new GameTime();
            t = new Thread(() => UpdateWorkers(gameTime));
            t.IsBackground = true;
            t.Start();

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
            a = new Worker(new Vector2(200), "test");

            background = Content.Load<Texture2D>("background");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
                }
            }

            foreach (GameObject go in gameObjects)
            {
                //go.Update(gameTime);
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

            if (a == null)
            {
                gameObjects.Add(a = new Worker(new Vector2(200), "test"));
                gameObjects.Add(new Worker(new Vector2(400), "test"));
                gameObjects.Add(new Worker(new Vector2(300), "test"));
            }
            //a.Update(gameTime);



            // TODO: Add your update logic here

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

            foreach (GameObject item in gameObjects)
            {
                item.Draw(spriteBatch);
            }
            foreach (Worker worker in ActiveWorkers)
            {
                worker.Draw(spriteBatch);
            }
            // TODO: Add your drawing code here

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void UpdateWorkers(GameTime gameTime)
        {
            while (true)
            {
                foreach (Worker worker in ActiveWorkers)
                {
                    worker.Update(gameTime);
                }
            }
        }
    }
}
