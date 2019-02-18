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
        bool goingLeft = false;
        double elapsedTime;
        Random rng = new Random();

        public Worker(Vector2 position, string spriteName, GameTime gameTime) : base(position, spriteName)
        {

        }

        public Worker(Vector2 position, string spriteName) : base(position, spriteName)
        {
            
        }

        public override void Update(GameTime gameTime)
        {           
            //elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
            //if (goingLeft == true)
            //{
            //    position.X -= 10;
            //}
            //else
            //{
            //    position.X += 10;
            //}

            //if (position.X >= 1920)
            //{
            //    goingLeft = true;
            //}
            //else if (position.X <= 0)
            //{
            //    goingLeft = false;
            //}
            //Thread.Sleep(1);
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
                Vector2 direction = waypoint - position;
                direction.Normalize();
                position += direction;
                Thread.Sleep(5);
            }
        }
    }
}
