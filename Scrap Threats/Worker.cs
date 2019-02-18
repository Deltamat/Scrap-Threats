﻿using Microsoft.Xna.Framework;
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
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (goingLeft == true)
            {
                position.X -= 10;
            }
            else
            {
                position.X += 10;
            }

            if (position.X >= 1920)
            {
                goingLeft = true;
            }
            else if (position.X <= 0)
            {
                goingLeft = false;
            }
            Thread.Sleep(1);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, null, Color.White, rotation, new Vector2(Sprite.Width * 0.5f, Sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
        }
   
    }
}
