using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;

namespace Scrap_Threats
{
    public class Worker : Unit
    {
        //delegate void UpdateDelegate(GameTime gameTime);
        Random rng = new Random();
        bool unemplyed = true;
        public bool readyToMine = true;
        public bool gatheringFood = false;
        public int carryingScrap;
        public int carryingFood;
        bool farming = false;
        public bool waitingForScrap;
        public bool gatheringScrap = false;
        static readonly object lockObject = new object();


        public Worker(Vector2 position, string spriteName) : base(position, spriteName)
        {
            foodUpkeep = 1;
            speed = 30;
            GameWorld.foodUpkeep += this.foodUpkeep;
            alive = true;
            GameTime gameTime = new GameTime();
            waypoint = position;
            Thread t = new Thread(() => Update(gameTime));
            t.IsBackground = true;
            t.Start();
        }

        public override void Update(GameTime gameTime)
        {
            Thread.Sleep(1000);
            while (alive == true)
            {
                // tjekker op på om waypoint er inde i scrapyard, hvis det er bliver workeren jobless = false og begynder at mine.
                // hvis waypoint ikke er inde i en scrapyard bliver workeren jobless igen.
                if (waypointRectangle.Intersects(GameWorld.scrapyard.CollisionBox)) // kan nok laves om til event eventuelt
                {
                    waypoint = GameWorld.scrapyard.Position;
                    unemplyed = false;
                    waypointRectangle = new Rectangle(-1000,-1000,1,1);
                }
                else if (waypointRectangle != new Rectangle(-1000, -1000, 1, 1))
                {
                    unemplyed = true;
                }

                if (waypointRectangle.Intersects(GameWorld.farm.CollisionBox)) // kan nok laves om til event eventuelt
                {
                    waypoint = GameWorld.farm.Position;
                    unemplyed = false;
                    waypointRectangle = new Rectangle(-1000, -1000, 1, 1);
                    farming = true;
                }
                else if (waypointRectangle != new Rectangle(-1000, -1000, 1, 1))
                {
                    unemplyed = true;
                    farming = false;
                }

                // går mod waypoint og teleportere når den kommer tæt nok på for at forhindre den hopper på stedet.
                if (unemplyed == true)
                {
                    if (Vector2.Distance(waypoint, position) < 2)
                    {
                        position = waypoint;
                    }
                    else
                    {
                        Vector2 direction = waypoint - position;
                        direction.Normalize();
                        position += direction * speed * (float)GameWorld.globalGameTime;
                    }
                }

                if (unemplyed == false)
                {
                    if (Vector2.Distance(position, waypoint) > 45)
                    {
                        Vector2 direction = waypoint - position;
                        direction.Normalize();
                        position += direction * speed * (float)GameWorld.globalGameTime;
                    }

                    if (Vector2.Distance(waypoint, position) < 50)
                    {
                        //få tråden ind i minen/scrapyard og vente der til den er færdig
                        if (readyToMine == true)
                        {
                            if (farming is false)
                            {
                                //mining = true;
                                GameWorld.scrapyard.Mining(this);
                                //mining = false;
                            }
                            else if (farming is true)
                            {
                                //mining = true;
                                if (GameWorld.farm.harvestable == true)
                                {
                                    GameWorld.farm.Farming(this);
                                }
                                //mining = false;
                            }

                        }
                        // når den er færdig:
                        if (Vector2.Distance(position, GameWorld.stockpile.Position) < 50)
                        {
                            
                            if (farming is false)
                            {
                                waypoint = GameWorld.scrapyard.Position;
                                readyToMine = true;
                                lock (lockObject)
                                {
                                    GameWorld.scrap += carryingScrap; // OPS måske skal der en lås til
                                }
                                carryingScrap = 0;
                            }
                            else
                            {
                                waypoint = GameWorld.farm.Position;
                                readyToMine = true;
                                lock (lockObject)
                                {
                                    GameWorld.food += carryingFood; // OPS måske skal der en lås til
                                }
                                carryingFood = 0;
                            }

                        }
                        else
                        {
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
