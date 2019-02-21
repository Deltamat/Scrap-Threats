using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrap_Threats
{
    /// <summary>
    /// Class that Guard, Worker and Raider inherits from
    /// </summary>
    public abstract class Unit : GameObject
    {
        protected int health;
        protected float speed;
        protected int foodUpkeep;
        /// <summary>
        /// Bool that the Threads use to check if they should continiue to run
        /// </summary>
        public bool alive;
        /// <summary>
        /// Vector used to get the direction the Threads move
        /// </summary>
        public Vector2 direction;
        protected double timeElapsed;
        protected int aniIndex;

        /// <summary>
        /// Unit constructor
        /// </summary>
        /// <param name="position">The position of the unit</param>
        /// <param name="spriteName">The name of the sprite</param>
        public Unit(Vector2 position, string spriteName) : base(position, spriteName)
        {
 
        }

        
    }
}
