using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrap_Threats
{
    /// <summary>
    /// The GameObject class that every other class inherits from
    /// </summary>
    public class GameObject
    {
        protected Texture2D sprite;
        protected Vector2 position;
        protected float rotation;
        /// <summary>
        /// Get-set property for the sprite Texture2D
        /// </summary>
        public Texture2D Sprite { get => sprite; set => sprite = value; }
        /// <summary>
        /// Get-set property for the position
        /// </summary>
        public Vector2 Position { get => position; set => position = value; }

        /// <summary>
        /// A vector used for movement 
        /// </summary>
        public Vector2 waypoint;
        /// <summary>
        /// Rectangle that is used to check if the waypoint intersects fx. the scrapyard
        /// </summary>
        public Rectangle waypointRectangle;

        /// <summary>
        /// Constructor for the GameObject
        /// </summary>
        /// <param name="position">The position of the building</param>
        /// <param name="spriteName">The name of the sprite</param>
        public GameObject(Vector2 position, string spriteName)
        {
            this.Position = position;
            Sprite = GameWorld.ContentManager.Load<Texture2D>(spriteName);
        }

        /// <summary>
        /// Get property that returns a collisionbox
        /// </summary>
        public virtual Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)(Position.X - Sprite.Width * 0.5), (int)(Position.Y - Sprite.Height * 0.5), Sprite.Width, Sprite.Height);
            }
        }

        /// <summary>
        /// The override update method
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        public virtual void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// The method that handles drawing
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, null, Color.White, rotation, new Vector2(Sprite.Width * 0.5f, Sprite.Height * 0.5f), 1f, SpriteEffects.None, 0.1f);
        }
    }
}
