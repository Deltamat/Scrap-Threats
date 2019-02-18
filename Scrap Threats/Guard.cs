using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scrap_Threats
{
    class Guard : Unit
    {
        public Guard(Vector2 position, string spriteName) : base(position, spriteName)
        {

        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, null, Color.White, rotation, new Vector2(Sprite.Width * 0.5f, Sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
        }
    }
}
