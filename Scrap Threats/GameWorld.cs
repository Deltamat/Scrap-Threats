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
        public static List<Worker> workers = new List<Worker>();
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
        private double raiderAttackTimer;
        int raiderCount = 3;
        public static List<Guard> guards = new List<Guard>();
        public static List<Worker> deadWorkers = new List<Worker>();
        public static List<Guard> deadGuards = new List<Guard>();       
        public static int waveCount = 0;



        private static ContentManager content;
        public static ContentManager ContentManager
        {
            get
            {
                return content;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
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
                worker = new Worker(new Vector2(rng.Next(100, 1800), rng.Next(100, 900)), "Spritesheet Walk");
                workers.Add(worker);
            }

            farm = new Farm(new Vector2(1700, 250), "farm_0");
            stockpile = new Stockpile(new Vector2(960, 540), "stockpile_0");
            scrapyard = new Scrapyard(new Vector2(200, 540), "junkpile");
            buildings.Add(farm);
            buildings.Add(stockpile);
            buildings.Add(scrapyard);

            guards.Add(new Guard(new Vector2(500), "Spritesheet Walk"));

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

            //sets a click event for each Button
            buyWorkerButton.Click += BuyWorkerButtonClickEvent;
            upgradeFarmAmountButton.Click += UpgradeFarmAmountButtonClickEvent;
            upgradeFarmCapacityButton.Click += UpgradeFarmCapacityButtonClickEvent;
            upgradeScrapyardCapacityButton.Click += UpgradeScrapyardCapacityButtonClickEvent;
            buyGuardButton.Click += BuyGuardButtonClickEvent;

            //List of our buttons
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
        /// Looks for the click event for the button which this event was added to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyWorkerButtonClickEvent(object sender, EventArgs e)
        {
            if (food >= 50)
            {
                workers.Add(new Worker(new Vector2((int)(ScreenSize.Width * 0.5), (int)(ScreenSize.Height * 0.5)), "test"));
                lock (Worker.lockObject)
                {
                    food -= 50;
                }                
            }
        }

        /// <summary>
        /// Looks for the click event for the button which this event was added to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Looks for the click event for the button which this event was added to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Looks for the click event for the button which this event was added to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Looks for the click event for the button which this event was added to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyGuardButtonClickEvent(object sender, EventArgs e)
        {
            if (food >= 10 && scrap >= 25)
            {
                guards.Add(new Guard(new Vector2((int)(ScreenSize.Width * 0.5), (int)(ScreenSize.Height * 0.5)), "Spritesheet Walk"));
                deadWorkers.Add(workers[rng.Next(0,workers.Count)]);
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

            //Updates the rectangle for selecting units
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

            //selects all the units that intersects with the rectangle generated above.
            foreach (Worker item in workers)
            {
                if (item.CollisionBox.Intersects(mouseClickRectangle))
                {
                    if (mouseClickRectangle.Width > 10 && mouseClickRectangle.Height > 10)
                    {
                        if (item.gatheringScrap is false || item.gatheringFood is false)
                        {
                            selectedUnit.Add(item);
                        }
                    }
                    //Checks if the rectangle is so small that you're only trying to select one unit and selects the top unit, 
                    //which is the last unit in the list that is drawn.
                    else
                    {
                        if (item.gatheringScrap is false || item.gatheringFood is false)
                        {
                            selectedUnit.RemoveRange(0, selectedUnit.Count);
                            selectedUnit.Add(item);
                        }
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
                        int totalUnitCountF = workers.Count + guards.Count;
                        if (rng.Next(0, totalUnitCountF) <= workers.Count) 
                        {                            
                            if (workers.Count > 0) //Ensures that there are workers to kill
                            {
                                int deadWorker = rng.Next(0, workers.Count); //Picks a random worker
                                workers[deadWorker].alive = false;
                                deadWorkers.Add(workers[deadWorker]);
                            }
                        }
                        else
                        {                          
                            if (guards.Count > 0) //Ensures that there are guards to kill
                            {
                                int deadGuard = rng.Next(0, guards.Count); //Picks a random worker
                                guards[deadGuard].alive = false;
                                deadGuards.Add(guards[deadGuard]);
                            }
                        }                     
                    }
                }
                foodUpkeepTimer = 0;
            }

            //removes dead workers
            foreach (var item in deadWorkers)
            {
                workers.Remove(item);
            }
            //removes dead guards
            foreach (var item in deadGuards)
            {
                guards.Remove(item);
            }

            deadWorkers.Clear();
            deadGuards.Clear();

            //spawns raiders every 30 seconds.
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
            
            //counts the time spent working inside the scrapyard or farm for each worker
            foreach (var item in workers)
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
            //Draws all the buttons of the UI
            foreach (var item in userInterfaceObjects)
            {
                item.Draw(spriteBatch);
            }

            //Draws all the buldings
            foreach (Building building in buildings)
            {
                building.Draw(spriteBatch);
#if DEBUG
                DrawCollisionBox(building);
#endif
            }
            //Draws all the workers
            foreach (Worker worker in workers)
            {
                worker.Draw(spriteBatch);
#if DEBUG
                DrawCollisionBox(worker);
#endif
            }
            //Draws the bad guys
            foreach (var item in raiders)
            {
                item.Draw(spriteBatch);
            }
            //Draws the good guys
            foreach (var item in guards)
            {
                item.Draw(spriteBatch);
            }
            //Draws some more UI giving the user some valuable information
            spriteBatch.DrawString(font, $"Scrap: {scrap}", new Vector2(10), Color.White);
            spriteBatch.DrawString(font, $"Food: {food}", new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(font, $"Upkeep timer: {(int)(60 - foodUpkeepTimer)}", new Vector2(10, 50), Color.White);
            spriteBatch.DrawString(font, $"Current upkeep: {foodUpkeep}", new Vector2(10, 70), Color.White);
            spriteBatch.DrawString(font, $"Raider timer: {(int)(30-raiderAttackTimer)}", new Vector2(10, 90), Color.White);
            
            //variables used in below foreach loop
            int workersInScrap = 0;
            int workersInFood = 0;

            //Uses the two variables above to stack the text if more than one worker is
            //inside the scrapyard and/or farm at the same time.
            foreach (var item in workers)
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
        

        /// <summary>
        /// Draws collision boxes
        /// </summary>
        /// <param name="go"></param>
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
