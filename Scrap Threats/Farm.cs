using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scrap_Threats
{
    public class Farm : Building
    {
        private float scale = 2f;
        private double growTimer;
        private double growthStart;
        public bool harvestable;
        private bool growing;
        public int harvestableFood;
        public int growthAmount = 50;
        public static Semaphore FarmingSemaphore = new Semaphore(3, 3);
        static Mutex m = new Mutex();

        public Farm(Vector2 position, string spriteName) : base(position, spriteName)
        {
            GameTime gameTime = new GameTime();
            Thread t = new Thread(() => Update(gameTime));
            t.IsBackground = true;
            t.Start();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (growTimer < 2.5 && harvestable == false) //Harvested
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("farm_0");
            }
            else if (growTimer >= 2.5 && growTimer < 5) //Tiny
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("farm_1");
            }
            else if (growTimer >= 5 && growTimer < 7.5) //Small
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("farm_2");
            }
            else if (growTimer >= 7.5 && growTimer < 10) //Medium
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("farm_3");
            }
            else //Ripe
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("farm_4");
            }
            spriteBatch.Draw(sprite, Position, null, Color.White, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), scale, SpriteEffects.None, 0.1f);
        }

        public override Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)((Position.X - Sprite.Width * scale * 0.5)), (int)((Position.Y - Sprite.Height * scale * 0.5)), (int)(Sprite.Width * scale), (int)(Sprite.Height * scale));
            }
        }

        public override void Update(GameTime gameTime)
        {
            while (true)
            {
                //If the field has been harvested, starts the growth process
                if (harvestableFood == 0 && harvestable == false && growthStart == 0)
                {
                    growthStart = GameWorld.elapsedTime;
                    growing = true;
                }

                //If there are no more harvestable food and the field ins't growing
                if (harvestableFood == 0 && growing == false)
                {
                    harvestable = false;
                    growthStart = 0;
                }

                //If the field has been growing for 10 seconds
                if ((GameWorld.elapsedTime - growthStart) > 10 && harvestable == false && growing == true)
                {
                    harvestableFood = growthAmount;
                    harvestable = true;
                    growing = false;
                }
                
                growTimer = GameWorld.elapsedTime - growthStart;

                Thread.Sleep(1);
            }
        }

        public void Farming(Worker worker)
        {
            if (FarmingSemaphore.WaitOne(1))
            {
                worker.gatheringFood = true;
                m.WaitOne();
                worker.readyToMine = false;
                if (harvestableFood > 0)
                {
                    worker.carryingFood = 10;
                    harvestableFood -= 10;
                }
                m.ReleaseMutex();
                Thread.Sleep(5000);
                worker.gatheringFood = false;
                FarmingSemaphore.Release();
            }
            else
            {

            }
        }
    }
}
