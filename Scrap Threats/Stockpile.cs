using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scrap_Threats
{
    public class Stockpile : Building
    {
        private float scale = 0.8f;

        public Stockpile(Vector2 position, string spriteName) : base(position, spriteName)
        {
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
