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
    public class Guard : Unit
    {
        private float distance;
        private float targetDistance;
        private Raider target;
        int damage;
        double cooldown;
        double cooldownStart;
        Thread t;

        public Guard(Vector2 position, string spriteName) : base(position, spriteName)
        {
            speed = 40f;
            targetDistance = 10000;
            damage = 5;
            alive = true;
            GameTime gameTime = new GameTime();
            t = new Thread(() => Update(gameTime));
            t.IsBackground = true;
            t.Start();
        }

        public override void Update(GameTime gameTime)
        {
            Thread.Sleep(1000);
            while (alive is true)
            {
                if (cooldownStart == 0)
                {
                    cooldownStart = GameWorld.elapsedTime;
                }
                cooldown = GameWorld.elapsedTime - cooldownStart;
                if (GameWorld.raiders.Count > 0 && target == null)
                {
                    //try
                    //{
                        target = (GameWorld.raiders[GameWorld.rng.Next(0, GameWorld.raiders.Count)]);

                        //foreach (var item in GameWorld.raiders)
                        //{
                            
                        //    //distance = Vector2.Distance(GameWorld.stockpile.Position, item.Position);
                        //    //if (distance < targetDistance)
                        //    //{
                        //    //    targetDistance = distance;
                        //    //    target = item;
                        //    //}
                        //}
                    //}
                    //catch (Exception)
                    //{
                        
                    //}
                    
                }

                if (target != null)
                {
                    if (Vector2.Distance(position, target.Position) > 100)
                    {
                        Vector2 direction = target.Position - position;
                        direction.Normalize();
                        position += direction * speed * (float)GameWorld.globalGameTime;
                    }

                    if (Vector2.Distance(position, target.Position) < 110)
                    {
                        if (cooldown > 1)
                        {
                            target.health -= damage;
                            targetDistance = 10000;
                            cooldown = 0;
                            cooldownStart = 0;
                        }
                    }

                    if (target.health <= 0)
                    {
                        target = null;
                    }
                }
                Thread.Sleep(1);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameWorld.selectedUnit.Contains(this))
            {
                spriteBatch.Draw(sprite, Position, null, Color.CornflowerBlue, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
            }
            else
            {
                spriteBatch.Draw(sprite, Position, null, Color.Aquamarine, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);

            }
        }
    }
}
