﻿using System;
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
    /// Unit of type guard
    /// Protects the stockpile against raiders
    /// </summary>
    public class Guard : Unit
    {
        private Raider target;
        private int damage;
        private double cooldown;
        private double cooldownStart;
        Thread t;

        public Guard(Vector2 position, string spriteName) : base(position, spriteName)
        {
            //Stats
            speed = 40f;
            foodUpkeep = 3;
            damage = 5;
            alive = true;

            //Thread
            GameTime gameTime = new GameTime();
            t = new Thread(() => Update(gameTime));
            t.IsBackground = true;
            t.Start();
        }

        public override void Update(GameTime gameTime)
        {
            Thread.Sleep(1000); //Waits for one second when spawned
            while (alive is true)
            {
                //Controls attack cooldown
                if (cooldownStart == 0)
                {
                    cooldownStart = GameWorld.elapsedTime;
                }
                cooldown = GameWorld.elapsedTime - cooldownStart;
                
                //Gives the guard a target
                if (GameWorld.raiders.Count > 0 && target == null) //Ensures that there are raiders and that the guard doesn't have a target
                {
                    try
                    {
                        target = (GameWorld.raiders[GameWorld.rng.Next(0, GameWorld.raiders.Count)]);
                    }
                    catch (Exception)
                    {

                    }
                }

                //Moves the guard towards their target and attacks them
                if (target != null) //If the guard has a target
                {
                    //If the guard is too far away, moves the guard closer to their target
                    if (Vector2.Distance(position, target.Position) > 100)
                    {
                        Vector2 direction = target.Position - position;
                        direction.Normalize();
                        position += direction * speed * (float)GameWorld.globalGameTime;
                    }

                    //If the guard is close enough, attacks their target
                    if (Vector2.Distance(position, target.Position) < 110)
                    {
                        if (cooldown > 1) //If the guard havn't attacked for a second
                        {
                            target.health -= damage;
                            cooldown = 0;
                            cooldownStart = 0;
                        }
                    }

                    //Removes the target if the target is dead
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
            timeElapsed += GameWorld.globalGameTime; //Animation timing
            aniIndex = (int)(timeElapsed * 10); //Animation index
            float radians;
            //Resets aniIndex and timeElapsed
            if (aniIndex > 7)
            {
                aniIndex = 0;
                timeElapsed = 0;
            }

            //Gives direction to the guard
            if (target != null)
            {
                radians = (float)Math.Atan2(position.Y - target.Position.Y, position.X - target.Position.X);
            }
            else
            {
                radians = 0;
            }
            float degrees = MathHelper.ToDegrees(radians);

            //Loads different sprites depending on direction
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

            //Draws the guard
            spriteBatch.Draw(sprite, Position, null, Color.Aquamarine, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
        }
    }
}
