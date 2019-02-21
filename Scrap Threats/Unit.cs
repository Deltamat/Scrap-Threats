﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrap_Threats
{
    public abstract class Unit : AnimatedGameObject
    {
        protected int health;
        protected float speed;
        protected int foodUpkeep;
        public bool alive;
        public Vector2 direction;

        public Unit(Vector2 position, string spriteName) : base(position, spriteName)
        {
 
        }

        
    }
}
