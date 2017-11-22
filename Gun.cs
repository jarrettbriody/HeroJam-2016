using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ZombieGame
{
    class Gun
    {
        public double BulletSpeed { get; set; }
        public int Damage { get; set; }
        public double FireRate { get; set; }
        Texture2D texture;
        public int MagCapacity { get; set; }
        public bool FullAuto { get; set; }
        public int Spray { get; set; }
        public int Penetration { get; set; }
        public int Range { get; set; }
        public string Name { get; set; }
        public int ReloadTime { get; set; }
        public int AmmoType { get; set; }
        public float Accuracy { get; set; }
        private bool canPickup = false;

        public bool CanPickup
        {
            get { return canPickup; }
            set { canPickup = value; }
        }

        private Rectangle hitBox;

        public Rectangle HitBox
        {
            get { return hitBox; }
            set { hitBox = value; }
        }


        Random ran = new Random();

        public Bullet[] fire(int x, int y, Vector2 direction)
        {
            Bullet[] bullets;
            
            

            bullets = new Bullet[Spray];
            for (int i = 0; i < Spray; i++)
            {
                Vector2 newDirection = direction;
                double changeX = ran.NextDouble();
                double changeY = ran.NextDouble();
                int chance = ran.Next(1);
                if (chance == 0)
                {
                    newDirection.X += (float)(changeX / Accuracy);
                }
                else
                {
                    newDirection.X -= (float)(changeX / Accuracy);
                }

                chance = ran.Next(1);
                if (chance == 0)
                {
                    newDirection.Y += (float)(changeY / Accuracy);
                }
                else
                {
                    newDirection.Y -= (float)(changeY / Accuracy);
                }

                bullets[i] = new Bullet(BulletSpeed, Damage, Penetration, x, y, newDirection, Range);
            }
            return bullets;
        }

        public void HitDetection(Player[] player)
        {
            for (int i = 0; i < player.Length; i++)
            {
                if (HitBox.Intersects(player[i].HitBox))
                {
                    CanPickup = true;
                }
                else
                {
                    CanPickup = false;
                }
            }
        }

        public override string ToString()
        {
            string temp = Name;
            temp = temp.Replace("gun_", "");
            return temp;
        }
    }
}
