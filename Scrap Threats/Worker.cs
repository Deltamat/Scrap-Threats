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
    class Worker : Unit
    {
        delegate void UpdateDelegate(GameTime gameTime);
        Random rng = new Random();

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
            while (alive == true)
            {               
                if (Vector2.Distance(waypoint, position) < 2)
                {
                    position = waypoint;
                }
                else
                {
                    Vector2 direction = waypoint - position;
                    direction.Normalize();
                    position += direction * 50f* (float)GameWorld.globalGameTime;
                }
                Thread.Sleep(1);
            }                        
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.selectedUnit == this)
            {
                spriteBatch.Draw(sprite, Position, null, Color.Green, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
            }
            else
            {
                spriteBatch.Draw(sprite, Position, null, Color.White, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
            }
        }
   
        public void WorkerUpdate()
        {
            while (position.X < 1000)
            {
                if (true)
                {
                    Vector2 direction = waypoint - position;
                    direction.Normalize();
                    position += direction;
                    Thread.Sleep(5);
                    if (Vector2.Distance(waypoint, position) < 1)
                    {
                        position = waypoint;
                    }
                }

                if (waypointRectangle.Intersects(GameWorld.stockpile.CollisionBox))
                {
                    position = new Vector2(1000);
                }
            }
        }
    }
}
