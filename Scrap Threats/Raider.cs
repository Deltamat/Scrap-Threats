using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scrap_Threats
{
    public class Raider : Unit
    {
        public bool killedWorker = false;
        public int health;

        public Raider(Vector2 position, string spriteName) : base(position, spriteName)
        {
            speed = 100;
            waypoint = GameWorld.stockpile.Position;
            health = 10;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 direction = waypoint - position;
            direction.Normalize();
            position += direction * speed * (float)GameWorld.globalGameTime;

            if (Vector2.Distance(position, waypoint) < 50)
            {
                int deadWorker = GameWorld.rng.Next(0, GameWorld.activeWorkers.Count); //Picks a random worker
                if (GameWorld.activeWorkers.Count > 0) //Ensures that there are workers to kill
                {
                    GameWorld.activeWorkers[deadWorker].alive = false;
                    GameWorld.activeWorkers.Remove(GameWorld.activeWorkers[deadWorker]);
                    killedWorker = true;
                }
            }

            if (health <= 0)
            {
                killedWorker = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, null, Color.White, rotation, new Vector2(Sprite.Width * 0.5f, Sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
        }
    }
}
