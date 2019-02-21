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
    /// Scrapyard class, this is where workers harvest scrap
    /// </summary>
    public class Scrapyard : Building
    {
        private float scale = 0.1f;
        /// <summary>
        /// Variable used when creating a new larger Semaphore
        /// </summary>
        public static int scrapyardMax = 3;

        /// <summary>
        /// The semaphore that handles the scrapyard
        /// </summary>
        public static Semaphore MiningSemaphore = new Semaphore(scrapyardMax,scrapyardMax);

        /// <summary>
        /// Scrapyard constructor
        /// </summary>
        /// <param name="position">The position of the scrapyard</param>
        /// <param name="spriteName">The name of the sprite</param>
        public Scrapyard(Vector2 position, string spriteName) : base(position, spriteName)
        {

        }

        /// <summary>
        /// Method that the workerthread calls when it enters the scrapyard
        /// </summary>
        /// <param name="worker">the worker that enters the scrapyard</param>
        public void Mining(Worker worker)
        {
            // The worker waits for access to the semaphore. If there is room the thread enters. 
            // If there is not the thread waits for 1 milisecond and then skips the semaphore and exits.
            // This ensures that the thread does not get stuck waiting for access and the player can move the 
            // worker-thread to other tasks because the thread is not asleep while waiting to get access.
            if (MiningSemaphore.WaitOne(1))
            {
                worker.gatheringScrap = true;
                worker.readyToMine = false;
                worker.carryingScrap = 10;

                Thread.Sleep(5000);
                worker.gatheringScrap = false;
                try
                {
                    MiningSemaphore.Release();
                }
                catch (SemaphoreFullException)
                {
                    
                }
            }
            else
	        {

            }
            
            

        }

        /// <summary>
        /// Method that draws
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Position, null, Color.White, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), scale, SpriteEffects.None, 0.1f);
        }

        /// <summary>
        /// Override property that returns a collisionbox
        /// </summary>
        public override Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)((Position.X - Sprite.Width * scale * 0.5)), (int)((Position.Y - Sprite.Height * scale * 0.5)), (int)(Sprite.Width * scale), (int)(Sprite.Height * scale));
            }
        }
    }
}
