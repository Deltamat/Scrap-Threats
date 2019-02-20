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
            if (GameWorld.scrap == 0) //No scrap
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("stockpile_0");
            }
            else if (GameWorld.scrap > 0 && GameWorld.scrap <= 250) //Some scrap
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("stockpile_1");
            }
            else if (GameWorld.scrap > 250 && GameWorld.scrap <= 500) //Medium scrap
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("stockpile_2");
            }
            else if (GameWorld.scrap > 500) //Lots of scrap
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("stockpile_3");
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
    }
}
