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
        public static int food = 50;
        public static int scrap = 50;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private List<GameObject> userInterfaceObjects;
        Texture2D background;
        public static List<Worker> activeWorkers = new List<Worker>();
        List<Building> buildings = new List<Building>();
        List<Button> UI = new List<Button>();
        Worker worker;
        public static Stockpile stockpile;
        public static Farm farm;
        private static int farmCapacity = 3;
        private static int scrapyardCapacity = 3;
        public static Scrapyard scrapyard;
        public static Random rng = new Random();
        public static Rectangle mouseClickRectangle;
        Vector2 selectionBoxOrigin;
        public static HashSet<GameObject> gameObjects = new HashSet<GameObject>();
        public static List<GameObject> selectedUnit = new List<GameObject>();
        SpriteFont font;
        private Texture2D collisionTexture;
        public static List<Raider> raiders = new List<Raider>();
        Raider killRaider;
        private double raiderAttackTimer;
        int raiderCount = 3;
        public static List<Guard> guards = new List<Guard>();
        public static List<Worker> deadWorkers = new List<Worker>();
        public static int waveCount = 0;


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
            for (int i = 0; i < 3; i++)
            {
                worker = new Worker(new Vector2(rng.Next(100, 1800), rng.Next(100, 900)), "test");
                activeWorkers.Add(worker);
            }

            farm = new Farm(new Vector2(1700, 250), "farm_0");
            stockpile = new Stockpile(new Vector2(960, 540), "stockpile_0");
            scrapyard = new Scrapyard(new Vector2(200, 540), "junkpile");
            buildings.Add(farm);
            buildings.Add(stockpile);
            buildings.Add(scrapyard);

            guards.Add(new Guard(new Vector2(500), "test"));

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

            //Buttons
            var buyWorkerButton = new Button(content.Load<Texture2D>("Button"), content.Load<SpriteFont>("Font"), new Vector2((int)(ScreenSize.Width), (int)(ScreenSize.Height*2)), "Button")
            {
                TextForButton = "Buy Worker",
            };
            var upgradeFarmAmountButton = new Button(content.Load<Texture2D>("Button"), content.Load<SpriteFont>("Font"), new Vector2((int)(ScreenSize.Width * 0.5), (int)(ScreenSize.Height * 2)), "Button")
            {
                TextForButton = "Upgr. Farm Amount",
            };
            var upgradeFarmCapacityButton = new Button(content.Load<Texture2D>("Button"), content.Load<SpriteFont>("Font"), new Vector2((int)(ScreenSize.Width * 0.5), (int)(ScreenSize.Height * 1.9)), "Button")
            {
                TextForButton = "Upgr. Farm Cap.",
            };
            var upgradeScrapyardCapacityButton = new Button(content.Load<Texture2D>("Button"), content.Load<SpriteFont>("Font"), new Vector2((int)(ScreenSize.Width * 1.5), (int)(ScreenSize.Height * 1.9)), "Button")
            {
                TextForButton = "Upgr. Scrapyard Cap.",
            };
            var buyGuardButton = new Button(content.Load<Texture2D>("Button"), content.Load<SpriteFont>("Font"), new Vector2((int)(ScreenSize.Width), (int)(ScreenSize.Height * 1.9)), "Button")
            {
                TextForButton = "Buy Guard",
            };

            //sets the click event for the Button
            buyWorkerButton.Click += BuyWorkerButtonClickEvent;
            upgradeFarmAmountButton.Click += UpgradeFarmAmountButtonClickEvent;
            upgradeFarmCapacityButton.Click += UpgradeFarmCapacityButtonClickEvent;
            upgradeScrapyardCapacityButton.Click += UpgradeScrapyardCapacityButtonClickEvent;
            buyGuardButton.Click += BuyGuardButtonClickEvent;

            userInterfaceObjects = new List<GameObject>()
            {
                buyWorkerButton,
                upgradeFarmAmountButton,
                upgradeFarmCapacityButton,
                upgradeScrapyardCapacityButton,
                buyGuardButton,
            };

            background = Content.Load<Texture2D>("background");
            font = Content.Load<SpriteFont>("font");
            collisionTexture = Content.Load<Texture2D>("CollisionTexture");
        }

        /// <summary>
        /// Looks for the click event on the "resume" button to trigger the "unpaused" state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyWorkerButtonClickEvent(object sender, EventArgs e)
        {
            if (food >= 50)
            {
                activeWorkers.Add(new Worker(new Vector2((int)(ScreenSize.Width * 0.5), (int)(ScreenSize.Height * 0.5)), "test"));
                lock (Worker.lockObject)
                {
                    food -= 50;
                }                
            }
        }

        private void UpgradeFarmAmountButtonClickEvent(object sender, EventArgs e)
        {
            if (scrap >= 100)
            {
                farm.growthAmount += 50;
                lock (Worker.lockObject)
                {
                    scrap -= 100;
                }                
            }
        }

        private void UpgradeFarmCapacityButtonClickEvent(object sender, EventArgs e)
        {
            if (scrap >= 250)
            {
                while (true)
                {
                    try
                    {
                        Farm.FarmingSemaphore.Release();
                    }
                    catch (SemaphoreFullException)
                    {
                        break;
                    }
                }
                farmCapacity++;
                Farm.FarmingSemaphore = new Semaphore(farmCapacity, farmCapacity);
                lock (Worker.lockObject)
                {
                    scrap -= 250;
                }
             
            }
        }

        private void UpgradeScrapyardCapacityButtonClickEvent(object sender, EventArgs e)
        {
            if (scrap >= 250)
            {
                while (true)
                {
                    try
                    {
                        Scrapyard.MiningSemaphore.Release();
                    }
                    catch (SemaphoreFullException)
                    {
                        break;
                    }
                }
                scrapyardCapacity++;
                Scrapyard.MiningSemaphore = new Semaphore(scrapyardCapacity, scrapyardCapacity);
                lock (Worker.lockObject)
                {
                    scrap -= 250;
                }

            }
        }

        private void BuyGuardButtonClickEvent(object sender, EventArgs e)
        {
            if (food >= 10 && scrap >= 25)
            {
                guards.Add(new Guard(new Vector2((int)(ScreenSize.Width * 0.5), (int)(ScreenSize.Height * 0.5)), "test"));
                deadWorkers.Add(activeWorkers[rng.Next(0,activeWorkers.Count)]);
                lock (Worker.lockObject)
                {
                food -= 10;
                scrap -= 25;
                }
            }
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
            
            //updates our click-events for the UI
            foreach (var item in userInterfaceObjects)
            {
                item.Update(gameTime);
            }

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
                        if (item.gatheringScrap is false || item.gatheringFood is false)
                        {
                            selectedUnit.Add(item);
                        }
                    }
                    else
                    {
                        if (item.gatheringScrap is false || item.gatheringFood is false)
                        {
                            selectedUnit.RemoveRange(0, selectedUnit.Count);
                            selectedUnit.Add(item);
                        }
                        //break;
                    }
                }
            }
            foreach (Guard item in guards)
            {
                if (item.CollisionBox.Intersects(mouseClickRectangle))
                {
                    if (mouseClickRectangle.Width > 10 && mouseClickRectangle.Height > 10)
                    {
                        selectedUnit.Add(item);
                    }
                    else
                    {
                        selectedUnit.RemoveRange(0, selectedUnit.Count);
                        selectedUnit.Add(item);
                    }
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
                            deadWorkers.Add(activeWorkers[deadWorker]);
                            //activeWorkers.Remove(activeWorkers[deadWorker]);
                        }                       
                    }
                }
                foodUpkeepTimer = 0;
            }

            foreach (var item in deadWorkers)
            {
                activeWorkers.Remove(item);
            }
            

            if (raiderAttackTimer > 30)
            {
                for (int i = 0; i < raiderCount; i++)
                {
                    raiders.Add(new Raider(new Vector2(1920, rng.Next(0,1080)), "test"));
                }
                raiderCount += 1;
                raiderAttackTimer = 0;
                waveCount++;
            }

            //foreach (var item in raiders)
            //{
            //    //item.Update(gameTime);
            //    if (item.killedWorker == true)
            //    {
            //        killRaider = item;
            //    }
            //}

            //if (killRaider != null && killRaider.killedWorker == true)
            //{
            //    raiders.Remove(killRaider);
            //}

            //foreach (var item in guards)
            //{
            //    item.Update(gameTime);
            //}

            foreach (var item in activeWorkers)
            {
                if (item.gatheringFood is true || item.gatheringScrap is true)
                {
                    item.miningTimer += gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            raiderAttackTimer += globalGameTime;
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
            foreach (var item in userInterfaceObjects)
            {
                item.Draw(spriteBatch);
            }
            foreach (Building building in buildings)
            {
                building.Draw(spriteBatch);
                DrawCollisionBox(building);
            }

            foreach (Worker worker in activeWorkers)
            {
                worker.Draw(spriteBatch);
                DrawCollisionBox(worker);
            }

            foreach (var item in raiders)
            {
                item.Draw(spriteBatch);
            }

            foreach (var item in guards)
            {
                item.Draw(spriteBatch);
            }

            //foreach (Button button in UI)
            //{
            //    button.Draw(spriteBatch);
            //    DrawCollisionBox(button);
            //}
            
            spriteBatch.DrawString(font, $"Scrap: {scrap}", new Vector2(10), Color.White);
            spriteBatch.DrawString(font, $"Food: {food}", new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(font, $"Upkeep timer: {(int)(60-foodUpkeepTimer)}", new Vector2(10, 50), Color.White);
            //spriteBatch.Draw(stockpile.Sprite, mouseClickRectangle, Color.Green);
            int workersInScrap = 0;
            int workersInFood = 0;
            foreach (var item in activeWorkers)
            {

                if (item.gatheringScrap is true)
                {
                    spriteBatch.DrawString(font, $"Worker gathering scrap: Time left: {(int)Math.Abs(item.miningTimer - 5)}", new Vector2(scrapyard.Position.X - 120, scrapyard.Position.Y - 115 - 20 * workersInScrap), Color.White);
                    workersInScrap++;
                }
                if (item.gatheringFood is true)
                {
                    spriteBatch.DrawString(font, $"Worker gathering food: Time left: {(int)Math.Abs(item.miningTimer - 5)}", new Vector2(farm.Position.X - 120, farm.Position.Y - 115 - 20 * workersInFood), Color.White);
                    workersInFood++;
                }
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

        private void DrawCollisionBox(GameObject go)
        {
            Rectangle collisionBox = go.CollisionBox;
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(collisionTexture, topLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(collisionTexture, bottomLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(collisionTexture, rightLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(collisionTexture, leftLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
    }
}
