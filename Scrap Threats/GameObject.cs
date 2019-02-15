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
        public Vector2 Position { get => position; set => position = value; }
        protected float rotation;

        public GameObject(Vector2 position, string spriteName)
        {
            this.Position = position;
            sprite = GameWorld.ContentManager.Load<Texture2D>(spriteName);
            //GameWorld.AddGameOject(this);
        }

        public virtual Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)(Position.X - sprite.Width * 0.5), (int)(Position.Y - sprite.Height * 0.5), sprite.Width, sprite.Height);
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
            spriteBatch.Draw(sprite, Position, null, Color.White, rotation, new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
        }

        public void Destroy()
        {
            //GameWorld.RemoveGameObject(this);
        }
    }
}
