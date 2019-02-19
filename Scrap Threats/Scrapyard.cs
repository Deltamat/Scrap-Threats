using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Scrap_Threats
{
    public class Scrapyard : Building
    {
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
    }
}
