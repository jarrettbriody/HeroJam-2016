using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame
{
    class Crate
    {
        private Gun weapon;

        public Gun Weapon
        {
            get { return weapon; }
            set
            {
                weapon = value;
                item = null;
            }
        }

        private string item;

        public string Item
        {
            get { return item; }
            set
            {
                item = value;
                weapon = null;
            }
        }


        public int Amount { get; set; }
        private Rectangle hitBox;
        public Rectangle HitBox { get { return hitBox; } set { hitBox = value; } }
        public bool Alive { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Crate(int x, int y)
        {
            Amount = 1;
            Alive = true;
            X = x;
            Y = y;
            hitBox = new Rectangle(X, Y, 100, 100);
        }

        public void HitDetection(Player[] player)
        {
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].HitBox.Intersects(HitBox))
                {
                    Alive = false;
                }
            }
        }

        public override string ToString()
        {
            string temp = Item;
            temp = temp.Replace("ammo_", "");
            temp = temp.Replace("item_", "");
            return temp;
        }
    }
}
