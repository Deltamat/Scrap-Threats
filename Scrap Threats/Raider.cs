using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scrap_Threats
{
    /// <summary>
    /// Unit of the type raider
    /// Attempts to reach the stockpile to pillage some resources and kill some workers/guards
    /// </summary>
    public class Raider : Unit
    {
        public bool killedWorker = false;
        public int health;
        Thread t;

        public Raider(Vector2 position, string spriteName) : base(position, spriteName)
        {
            //Stats
            speed = 10;
            waypoint = GameWorld.stockpile.Position;
            health = 10 + GameWorld.waveCount;
            alive = true;

            //Thread
            GameTime gameTime = new GameTime();
            t = new Thread(() => Update(gameTime));
            t.IsBackground = true;
            t.Start();
        }

        public override void Update(GameTime gameTime)
        {
            Thread.Sleep(10); //Waits for 10 milliseconds when spawned
            while (alive is true)
            {
                //Gives the raider a direction, i.e. the stockpile's location
                Vector2 direction = waypoint - position;
                direction.Normalize();
                position += direction * speed * (float)GameWorld.globalGameTime; //Moves the raider

                //The raider reaches the stockpile
                if (Vector2.Distance(position, waypoint) < 50)
                {
                    int totalUnitCountF = GameWorld.workers.Count + GameWorld.guards.Count; //Gives the total amount of units Friendly to the player, i.e. workers and guards
                    if (GameWorld.rng.Next(0, totalUnitCountF) <= GameWorld.workers.Count && GameWorld.workers.Count > 0)
                    {
                        if (GameWorld.workers.Count > 0) //Ensures that there are workers to kill
                        {
                            int deadWorker = GameWorld.rng.Next(0, GameWorld.workers.Count); //Picks a random worker
                            GameWorld.workers[deadWorker].alive = false;
                            GameWorld.deadWorkers.Add(GameWorld.workers[deadWorker]);
                        }
                    }
                    else
                    {
                        if (GameWorld.guards.Count > 0) //Ensures that there are guards to kill
                        {
                            int deadGuard = GameWorld.rng.Next(0, GameWorld.guards.Count); //Picks a random guard
                            GameWorld.guards[deadGuard].alive = false;
                            GameWorld.deadGuards.Add(GameWorld.guards[deadGuard]);
                        }
                    }

                    health = 0;
                    lock (Worker.lockObject) //Ensures that only one units can access the stockpile's resources, i.e. food and scrap
                    {
                        GameWorld.food -= 10;
                        GameWorld.scrap -= 10;
                    }
                }

                //Removes the raider if they are dead
                if (health <= 0)
                {
                    try
                    {
                        GameWorld.raiders.Remove(this);
                    }
                    catch (Exception)
                    {

                    }
                    break;
                }
                Thread.Sleep(1);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            timeElapsed += GameWorld.globalGameTime; //Animation timing
            aniIndex = (int)(timeElapsed * 5); //Animation index
            //Resets aniIndex and timeElapsed
            if (aniIndex > 7)
            {
                aniIndex = 0;
                timeElapsed = 0;
            }

            //Gives direction to the raider
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

            //Draws the raider
            spriteBatch.Draw(Sprite, Position, null, Color.Crimson, rotation, new Vector2(Sprite.Width * 0.5f, Sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
        }
    }
}
