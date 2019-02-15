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
        Thread t;
        public Worker(Vector2 position, string spriteName, GameTime gameTime) : base(position, spriteName)
        {
            //UpdateDelegate tmp = Update;
            t = new Thread(Test);
            t.Start(gameTime);
            t.IsBackground = true;
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Position, null, Color.White, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
        }

        public void Test(object o)
        {
            o = (GameTime)o;
            while (true)
            {
                //position.X + 100 * ;
            }
        }
        
    }
}
