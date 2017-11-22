using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame
{
    class Barricade
    {
        public Rectangle HitBox { get; set; }
        public Color Color { get; set; }
        public int Health { get; set; }

        /// <summary>
        /// True for 300x100, False for 100x300
        /// </summary>
        /// <param name="facing"> True for 300x100, False for 100x300</param>
        public Barricade(bool facing, int X, int Y, Color c)
        {
            if(facing == true)
                HitBox = new Rectangle(X, Y, 300, 100);

            else if(facing == false)
                HitBox = new Rectangle(X, Y, 100, 300);

            Color = c;
            Health = 50;
        }
    }
}
