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
        //private float distance;
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
            foodUpkeep = 3;
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
                    try
                    {
                        target = (GameWorld.raiders[GameWorld.rng.Next(0, GameWorld.raiders.Count)]);
                    }
                    catch (Exception)
                    {

                    }

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
            timeElapsed += GameWorld.globalGameTime;
            aniIndex = (int)(timeElapsed * 10);
            float radians;
            if (aniIndex > 7)
            {
                aniIndex = 0;
                timeElapsed = 0;
            }
            if (target != null)
            {
                radians = (float)Math.Atan2(position.Y - target.Position.Y, position.X - target.Position.X);
            }
            else
            {
                radians = 0;
            }
            float degrees = MathHelper.ToDegrees(radians);

            if (degrees == 0)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("standing");
            }
            else if (degrees > -22.5f && degrees < 22.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"w_p{aniIndex}");
            }
            else if (degrees > 22.5f && degrees < 67.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"nw_p{aniIndex}");
            }
            else if (degrees > 67.5f && degrees < 112.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"n_p{aniIndex}");
            }
            else if (degrees > 112.5f && degrees < 157.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"ne_p{aniIndex}");
            }
            else if (degrees > 157.5f || degrees < -157.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"e_p{aniIndex}");
            }
            else if (degrees < -22.5f && degrees > -67.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"sw_p{aniIndex}");
            }
            else if (degrees < -67.5f && degrees > -112.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"s_p{aniIndex}");
            }
            else if (degrees < -112.5f && degrees > -157.5f)
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>($"se_p{aniIndex}");
            }

            spriteBatch.Draw(sprite, Position, null, Color.Aquamarine, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);

        }
    }
}
