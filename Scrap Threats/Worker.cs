using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;
using System.Windows;

namespace Scrap_Threats
{
    public class Worker : Unit
    {
        //delegate void UpdateDelegate(GameTime gameTime);
        Random rng = new Random();
        bool unemployed = true;
        public bool readyToMine = true;
        public bool gatheringFood = false;
        public int carryingScrap;
        public int carryingFood;
        bool farming = false;
        public Thread t;
        public bool waitingForScrap;
        public bool gatheringScrap = false;
        public static readonly object lockObject = new object();
        public double miningTimer;




        public Worker(Vector2 position, string spriteName) : base(position, spriteName)
        {
            foodUpkeep = 2;
            speed = 30;
            GameWorld.foodUpkeep += this.foodUpkeep;
            alive = true;
            GameTime gameTime = new GameTime();
            waypoint = position;
            t = new Thread(() => Update(gameTime));
            t.IsBackground = true;
            t.Start();
        }

        public override void Update(GameTime gameTime)
        {
            Thread.Sleep(1000);
            while (alive == true)
            {
                // Tjekker op på om waypoint er inde i scrapyarden, hvis det er bliver workeren "unemployed = false" og
                // begynder at samle "scraps".
                // Hvis waypoint ikke er inde i en scrapyarden bliver workeren "unemployed = true" igen.
                if (waypointRectangle.Intersects(GameWorld.scrapyard.CollisionBox)) 
                {
                    waypoint = GameWorld.scrapyard.Position;
                    unemployed = false;
                    waypointRectangle = new Rectangle(-1000,-1000,1,1);
                }
                else if (waypointRectangle != new Rectangle(-1000, -1000, 1, 1))
                {
                    unemployed = true;
                }

                // Tjekker op på om waypoint er inde i farm'en, hvis det er bliver workeren "unemployed = false" og
                // begynder at samle "food".
                // Hvis waypoint ikke er inde i en farm'en bliver workeren "unemployed = true" igen.
                if (waypointRectangle.Intersects(GameWorld.farm.CollisionBox)) 
                {
                    waypoint = GameWorld.farm.Position;
                    unemployed = false;
                    waypointRectangle = new Rectangle(-1000, -1000, 1, 1);
                    farming = true;
                }
                else if (waypointRectangle != new Rectangle(-1000, -1000, 1, 1))
                {
                    unemployed = true;
                    farming = false;
                }

                // går mod waypoint og teleportere når den kommer tæt nok på for at forhindre den hopper på stedet.
                if (unemployed == true)
                {
                    if (Vector2.Distance(waypoint, position) < 2)
                    {
                        position = waypoint;
                    }
                    else
                    {
                        direction = waypoint - position;
                        direction.Normalize();
                        position += direction * speed * (float)GameWorld.globalGameTime;
                       
                    }


                }

                
                if (unemployed == false)
                {
                    //går hen mod waypoint så længe den er over 45 fra.
                    if (Vector2.Distance(position, waypoint) > 45)
                    {
                        Vector2 direction = waypoint - position;
                        direction.Normalize();
                        position += direction * speed * (float)GameWorld.globalGameTime;
                    }

                    if (Vector2.Distance(waypoint, position) < 50)
                    {
                        //får tråden ind i farmen/scrapyarden og venter der til den er færdig med at samle ressourcer.
                        if (readyToMine == true)
                        {
                            if (farming is false)
                            {
                                GameWorld.scrapyard.Mining(this);
                            }
                            else if (farming is true)
                            {
                                if (GameWorld.farm.harvestable == true)
                                {
                                    GameWorld.farm.Farming(this);
                                }
                            }
                            //resetter timeren til teksten som vises oven over scrapyarden/farmen.
                            miningTimer = 0;

                        }
                        // når den er færdig:
                        if (Vector2.Distance(position, GameWorld.stockpile.Position) < 50)
                        {
                            //Læsser scraps af i stockpilen
                            if (farming is false)
                            {
                                waypoint = GameWorld.scrapyard.Position;
                                readyToMine = true;
                                lock (lockObject)
                                {
                                    GameWorld.scrap += carryingScrap;
                                }
                                carryingScrap = 0;
                            }
                            //Læsser mad af i stockpilen
                            else
                            {
                                waypoint = GameWorld.farm.Position;
                                readyToMine = true;
                                lock (lockObject)
                                {
                                    GameWorld.food += carryingFood;
                                }
                                carryingFood = 0;
                            }

                        }
                        else
                        {
                            //returnerer til stockpilen med ressourcer 
                            if (readyToMine == false)
                            {
                                waypoint = GameWorld.stockpile.Position;

                            }
                        }

                    }
                }

                Thread.Sleep(1);
            }                        
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            timeElapsed += GameWorld.globalGameTime; //Animation timing
            aniIndex = (int)(timeElapsed * 10); //Animation index
            //Resets aniIndex and timeElapsed
            if (aniIndex > 7)
            {
                aniIndex = 0;
                timeElapsed = 0;
            }

            //Gives direction to the worker
            float radians = (float)Math.Atan2(position.Y - waypoint.Y, position.X - waypoint.X);
            float degrees = MathHelper.ToDegrees(radians);

            //Loads different sprites depending on direction
            if (degrees == 0)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("standing");
            }
            else if (degrees > -22.5f && degrees < 22.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"w_p{aniIndex}");
            }
            else if (degrees > 22.5f && degrees < 67.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"nw_p{aniIndex}");
            }
            else if (degrees > 67.5f && degrees < 112.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"n_p{aniIndex}");
            }
            else if (degrees > 112.5f && degrees < 157.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"ne_p{aniIndex}");
            }
            else if (degrees > 157.5f || degrees < -157.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"e_p{aniIndex}");
            }
            else if (degrees < -22.5f && degrees > -67.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"sw_p{aniIndex}");
            }
            else if (degrees < -67.5f && degrees > -112.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"s_p{aniIndex}");
            }
            else if (degrees < -112.5f && degrees > -157.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"se_p{aniIndex}");
            }


            //Draws the worker
            if (GameWorld.selectedUnit.Contains(this) && (gatheringFood == false && gatheringScrap == false))
            {
                spriteBatch.Draw(sprite, Position, null, Color.Green, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
            }
            else if (gatheringFood == true || gatheringScrap == true)
            {

            }
            else
            {
                spriteBatch.Draw(sprite, Position, null, Color.White, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);

            }
        }

       
    }
}
