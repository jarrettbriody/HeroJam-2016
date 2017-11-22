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
    class Sprinter : Zombie
    {
        private int health;
        private int speed;
        private int damage;
        private Rectangle pos;
        private Rectangle hitBox;

        public Sprinter(Player[] play, int X, int Y, int mTick, World w, List<Bullet> b, List<Crate> c, List<Barricade> bar) : base(play, X, Y, mTick, w, b, c, bar)
        {
            Zombie zombie = new ZombieGame.Zombie(play, X, Y, mTick, w, b, c, bar);
            health = zombie.Health / 2;
            speed = zombie.Speed * 2;
            damage = zombie.Damage / 2;
            pos = zombie.Pos;
            hitBox = zombie.HitBox;

            this.setAttributes(health, speed, damage, pos);
        }
    }
}
