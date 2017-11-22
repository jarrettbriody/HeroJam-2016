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
    class BigBoi : Zombie
    {
        private int health;
        private int speed;
        private int damage;
        private Rectangle pos;
        private Rectangle hitBox;

        public BigBoi(Player[] play, int X, int Y, int mTick, World w, List<Bullet> b, List<Crate> c, List<Barricade> bar) : base(play, X, Y, mTick, w, b, c, bar)
        {
            Zombie zombie = new Zombie(play, X, Y, mTick, w, b, c, bar);
            health = zombie.Health * 4;
            speed = 1;
            damage = zombie.Damage * 2;
            pos = new Rectangle(zombie.Pos.X, zombie.Pos.X, zombie.Pos.Width + (zombie.Pos.Width / 2), zombie.Pos.Height + (zombie.Pos.Height / 2));
            hitBox = zombie.HitBox;

            this.setAttributes(health, speed, damage, pos);
        }
    }
}
