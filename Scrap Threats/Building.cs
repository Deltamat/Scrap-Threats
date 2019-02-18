using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrap_Threats
{
    abstract class Building : GameObject
    {
        public Building(Vector2 position, string spriteName) : base (position, spriteName)
        {

        }
    }
}
