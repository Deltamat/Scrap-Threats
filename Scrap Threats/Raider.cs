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
    public class Raider : Unit
    {
        public bool killedWorker = false;
        public int health;
        Thread t;

        public Raider(Vector2 position, string spriteName) : base(position, spriteName)
        {
            speed = 10;
            waypoint = GameWorld.stockpile.Position;
            health = 10 + GameWorld.waveCount;
            alive = true;
            GameTime gameTime = new GameTime();
            t = new Thread(() => Update(gameTime));
            t.IsBackground = true;
            t.Start();
        }

        public override void Update(GameTime gameTime)
        {
            Thread.Sleep(10);
            while (alive is true)
            {
                Vector2 direction = waypoint - position;
                direction.Normalize();
                position += direction * speed * (float)GameWorld.globalGameTime;

                if (Vector2.Distance(position, waypoint) < 50)
                {
                    int totalUnitCountF = GameWorld.workers.Count + GameWorld.guards.Count;
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
                            int deadGuard = GameWorld.rng.Next(0, GameWorld.guards.Count); //Picks a random worker
                            GameWorld.guards[deadGuard].alive = false;
                            GameWorld.deadGuards.Add(GameWorld.guards[deadGuard]);
                        }
                    }
                    //int deadWorker = GameWorld.rng.Next(0, GameWorld.workers.Count); //Picks a random worker
                    //if (GameWorld.workers.Count > 0) //Ensures that there are workers to kill
                    //{
                    //    GameWorld.workers[deadWorker].alive = false;
                    //    GameWorld.deadWorkers.Add(GameWorld.workers[deadWorker]);
                    health = 0;
                    lock (Worker.lockObject)
                    {
                        GameWorld.food -= 10;
                        GameWorld.scrap -= 10;
                    }
                    //}
                }

                if (health <= 0)
                {
                    GameWorld.raiders.Remove(this);                   
                    break;
                }
                Thread.Sleep(1);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, null, Color.Crimson, rotation, new Vector2(Sprite.Width * 0.5f, Sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
        }
    }
}
