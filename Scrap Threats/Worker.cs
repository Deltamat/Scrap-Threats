using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scrap_Threats
{
    public class Worker : Unit
    {
        //delegate void UpdateDelegate(GameTime gameTime);
        Random rng = new Random();
        bool jobless = true;
        public bool readyToMine = true;
        bool mining = false;
        public int carrying;

        public Worker(Vector2 position, string spriteName) : base(position, spriteName)
        {
            foodUpkeep = 1;
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
            Thread.Sleep(200);
            while (alive == true)
            {
                // tjekker op på om waypoint er inde i scrapyard, hvis det er bliver workeren jobless = false og begynder at mine.
                // hvis waypoint ikke er inde i en scrapyard bliver workeren jobless igen.
                if (waypointRectangle.Intersects(GameWorld.scrapyard.CollisionBox)) // kan nok laves om til event eventuelt
                {
                    waypoint = GameWorld.scrapyard.Position;
                    jobless = false;
                    waypointRectangle = new Rectangle(-1000,-1000,1,1);
                }
                else if (waypointRectangle != new Rectangle(-1000, -1000, 1, 1))
                {
                    jobless = true;
                }

                // går mod waypoint og teleportere når den kommer tæt nok på for at forhindre den hopper på stedet.
                if (jobless == true)
                {
                    if (Vector2.Distance(waypoint, position) < 2)
                    {
                        position = waypoint;
                    }
                    else
                    {
                        Vector2 direction = waypoint - position;
                        direction.Normalize();
                        position += direction * 50f * (float)GameWorld.globalGameTime;
                    }
                }

                if (jobless == false)
                {
                    Vector2 direction = waypoint - position;
                    direction.Normalize();
                    position += direction * 50f * (float)GameWorld.globalGameTime;

                    if (Vector2.Distance(waypoint, position) < 50)
                    {
                        //få tråden ind i minen/scrapyard og vente der til den er færdig
                        if (readyToMine == true)
                        {
                            mining = true;
                            GameWorld.scrapyard.Mining(this);
                            mining = false;
                        }
                        // når den er færdig:
                        if (Vector2.Distance(position, GameWorld.stockpile.Position) < 50)
                        {
                            waypoint = GameWorld.scrapyard.Position;
                            readyToMine = true;
                            GameWorld.scrap += carrying; // OPS måske skal der en lås til
                            carrying = 0;
                        }
                        else
                        {
                            waypoint = GameWorld.stockpile.Position;
                        }

                    }
                }

                Thread.Sleep(1);
            }                        
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.selectedUnit.Contains(this) && mining == false)
            {
                spriteBatch.Draw(sprite, Position, null, Color.Green, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
            }
            else if (mining == true)
            {

            }
            else
            {
                spriteBatch.Draw(sprite, Position, null, Color.White, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);

            }
        }
    }
}
