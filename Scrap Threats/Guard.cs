using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Guard(Vector2 position, string spriteName) : base(position, spriteName)
        {
            speed = 500f;
            targetDistance = 10000;
            damage = 10;
        }

        public override void Update(GameTime gameTime)
        {
            cooldown += gameTime.ElapsedGameTime.TotalSeconds;
            if (GameWorld.raiders.Count > 0)
            {
                foreach (var item in GameWorld.raiders)
                {
                    distance = Vector2.Distance(position, item.Position);
                    if (distance < targetDistance)
                    {
                        targetDistance = distance;
                        target = item;
                    }
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
                    }

                }
                
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
