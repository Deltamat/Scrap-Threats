using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrap_Threats
{
    class GameObject
    {
        protected Texture2D sprite;
        protected Vector2 position;
        protected float rotation;
        public Texture2D Sprite { get => sprite; set => sprite = value; }
        public Vector2 Position { get => position; set => position = value; }

        public GameObject(Vector2 position, string spriteName)
        {
            this.Position = position;
            Sprite = GameWorld.ContentManager.Load<Texture2D>(spriteName);
        }

        public virtual Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)(Position.X - Sprite.Width * 0.5), (int)(Position.Y - Sprite.Height * 0.5), Sprite.Width, Sprite.Height);
            }
        }

        public virtual bool IsColliding(GameObject otherObject)
        {
            return CollisionBox.Intersects(otherObject.CollisionBox);
        }


        public virtual void DoCollision(GameObject otherObject)
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, null, Color.White, rotation, new Vector2(Sprite.Width * 0.5f, Sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
        }

        public void Destroy()
        {

        }
    }
}
