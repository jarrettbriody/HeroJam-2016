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
    class Bullet
    {
        public double Speed { get; set; }
        public int Damage { get; set; }
        
        Rectangle hitbox;
        Vector2 direction;
        public int Penetration { get; set; }
        public Rectangle HitBox { get { return hitbox; } set { hitbox = value; } }
        int lifeTimeTick;
        public bool Alive { get { return alive; } }
        bool alive = true;

        public Bullet(double spd, int dmg, int pen, int x, int y, Vector2 dir, int lifeTime)
        {
            Speed = spd;
            Damage = dmg;
            hitbox = new Rectangle(x - 5, y - 5, 10, 10);
            direction = dir;
            lifeTimeTick = lifeTime;
        }

        public void updateMovement()
        {
            hitbox.X += (int)(direction.X * Speed);
            hitbox.Y += (int)(direction.Y * Speed);

            lifeTimeTick--;
            if (lifeTimeTick <= 0)
            {
                alive = false;
            }
        }

        public void HitDetection(List<Zombie> zombieList)
        {
            for (int i = 0; i < zombieList.Count(); i++)
            {
                if (HitBox.Intersects(zombieList[i].HitBox))
                {
                    zombieList[i].Health -= Damage;
                    Penetration--;
                    Damage /= 2;
                    if(Penetration <= 0)
                    {
                        alive = false;
                    }
                }
            }
        }
    }
}
