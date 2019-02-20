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
    public class Scrapyard : Building
    {
        private float scale = 0.1f;
        public static Semaphore MiningSemaphore = new Semaphore(3,3);

        public Scrapyard(Vector2 position, string spriteName) : base(position, spriteName)
        {

        }

        public void Mining(Worker worker)
        {
            MiningSemaphore.WaitOne();
            worker.readyToMine = false;
            worker.carrying = 10;
            
            Thread.Sleep(5000);
            MiningSemaphore.Release();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Position, null, Color.White, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), scale, SpriteEffects.None, 0.1f);
        }

        public override Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)((Position.X - Sprite.Width * scale * 0.5)), (int)((Position.Y - Sprite.Height * scale * 0.5)), (int)(Sprite.Width * scale), (int)(Sprite.Height * scale));
            }
        }
    }
}
