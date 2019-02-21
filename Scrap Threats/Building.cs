using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scrap_Threats
{
    /// <summary>
    /// Building class that Stockpile, Farm and Scrapyard inherits from 
    /// </summary>
    public abstract class Building : GameObject
    {
        /// <summary>
        /// Building constructor
        /// </summary>
        /// <param name="position">The position of the building</param>
        /// <param name="spriteName">The name of the sprite that the building shall have</param>
        public Building(Vector2 position, string spriteName) : base(position, spriteName)
        {
        }
    }
}
