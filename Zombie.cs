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
    class Zombie
    {
        // Attributes and Properties
        private int health, damage, tick, maxTick, moveXFinal, moveYFinal, latency;
        private Rectangle pos, hitBox;
        private bool hasHit, isPushing, attack;
        public int soundTick { get; set; }

        //animation vars
        int animationTick, animationState;
        
        public int AnimationState { get { return animationState; } }

        float rotation;

        public float Angle { get { return rotation; } }

        public string Name { get; set; }

        bool moving, attacking;

        public Player[] Players { get; set; }

        public List<Zombie> Zombies { get; set; }

        public int Health { get { return health; } set { health = value; } }

        public Rectangle Pos { get { return pos; } set { pos = value; } }

        public Texture2D Texture { get; set; }

        public Rectangle HitBox { get { return hitBox; } set { hitBox = value; } }

        public World World { get; set; }

        public List<Bullet> bullets { get; set; }

        public List<Crate> CrateList { get; set; }

        public List<Barricade> Barricades;
        int pushTick = 0;
        public int Damage { get { return damage; } set { damage = value; } }

        public int Speed { get; set; }
        
        /// <summary>
        /// 0 - Velocity, 1 - Distance Initial, 2 is Time final
        /// </summary>
        public int[] Physics { get; set; }

        // Constructor
        public Zombie(Player[] play, int X, int Y, int mTick, World w, List<Bullet> b, List<Crate> c, List<Barricade> bar)
        {
            Players = play;
            Pos = new Rectangle(X, Y, 100, 100);
            hasHit = false;
            Speed = 4;
            tick = 0;
            health = 100;
            damage = 5;
            maxTick = mTick;
            HitBox = new Rectangle(Pos.X - 50, Pos.Y - 50, 100, 100);
            World = w;
            bullets = b;
            Physics = new int[3];
            Physics[0] = 5;
            Physics[1] = 0;
            Physics[2] = 10;
            isPushing = false;
            CrateList = c;
            latency = 20;
            Barricades = bar;
        }

        /// <summary>
        /// Set the Attirubtes of the zombie
        /// </summary>
        public void setAttributes(int hp, int sp, int dmg, Rectangle rec)
        {
            Speed = sp;
            Health = hp;
            Damage = dmg;
            Pos = new Rectangle(rec.X, rec.Y, rec.Width, rec.Height);
            HitBox = new Rectangle(rec.X - (rec.Width/2), rec.Y - (rec.Height)/2, rec.Width, rec.Height);
        }

        // update Method nonsense
        public void updateZombie()
        {
            // The closest Player is taken in and bools for horizontal and vertical positions
            Player closest = checkPosition(Players);

            Attack(closest);

            moveXFinal += zombieMovement(closest)[0];
            moveYFinal += zombieMovement(closest)[1];
            Move(closest);

            foreach (Zombie z in Zombies)
            {
                if (z != this)
                {
                    moveXFinal += this.zombieCollide(z, HitBox.Intersects(z.HitBox))[0];
                    moveYFinal += this.zombieCollide(z, HitBox.Intersects(z.HitBox))[1];
                    Move(closest);
                }
            }

            foreach (Crate c in CrateList)
            {
                moveXFinal += crateCollide(c, HitBox.Intersects(c.HitBox))[0];
                moveYFinal += crateCollide(c, HitBox.Intersects(c.HitBox))[1];
                Move(closest);
            }

            List<Barricade> removeList = new List<Barricade>();
            foreach(Barricade b in Barricades)
            {
                moveXFinal += barricadeCollide(b, HitBox.Intersects(b.HitBox))[0];
                moveYFinal += barricadeCollide(b, HitBox.Intersects(b.HitBox))[1];
                Move(closest);
                Attack(b);

                if (b.Health < 0)
                    removeList.Add(b);
            }

            foreach (var item in removeList)
            {
                Barricades.Remove(item);
            }
            Tick(closest, this);
        }

        private void Tick(Player closest, Zombie zombie)
        {
            if (tick > maxTick)
            {
                tick = 0;
                hasHit = false;
            }

            tick++;
        }

        private void Move(Player player)
        {
            playerPush(player, (this.hitBox.Intersects(player.HitBox)));            

            if (isPushing)
            {
                pushTick--;
                if (pushTick <= 0)
                {
                }
            }

            if (attack == true || attacking == true)
            {
                hitBox.X += 0;
                hitBox.Y += 0;
                pos.X += 0;
                pos.Y += 0;
            }
            else
            {
                hitBox.X += moveXFinal;
                hitBox.Y += moveYFinal;
                pos.X += moveXFinal;
                pos.Y += moveYFinal;
            }

            moveXFinal = 0;
            moveYFinal = 0;
        }

        // Checking Positions of players
        private Player checkPosition(Player[] player)
        {
            Player closest;
            closest = player[0];

            // Foreach of the players, check which is closest            
            foreach(Player p in player)
            {
                if (p != player[0])
                {
                    int distance = (Math.Abs(hitBox.X - p.HitBox.X) + Math.Abs(HitBox.Y - p.HitBox.Y));

                    if (distance > closest.HitBox.X + closest.HitBox.Y)
                    {
                        closest = p;
                    }
                }
            }
            

            return closest;
        }

        // Attack method
        private void Attack(Player player)
        {
            attack = false;

            if (this.hitBox.Intersects(player.HitBox))
                attack = true;
            else if (this.hitBox.Contains(player.HitBox))
                attack = true;
            else if (this.HitBox.Intersects(new Rectangle(player.HitBox.X, player.HitBox.Y, player.HitBox.Width + 10, player.HitBox.Height + latency)))
                attack = true;
            else if (this.HitBox.Intersects(new Rectangle(player.HitBox.X, player.HitBox.Y, player.HitBox.Width + 10, player.HitBox.Height - latency)))
                attack = true;
            else if (this.HitBox.Intersects(new Rectangle(player.HitBox.X, player.HitBox.Y, player.HitBox.Width - 10, player.HitBox.Height + latency)))
                attack = true;
            else if (this.HitBox.Intersects(new Rectangle(player.HitBox.X, player.HitBox.Y, player.HitBox.Width - 10, player.HitBox.Height - latency)))
                attack = true;

            if (attack == true)
            {
                if (hasHit == false)
                {
                    animationState = 6;
                    attacking = true;
                    player.Health -= damage;                                        
                    hasHit = true;
                    tick++;
                }

                else
                {
                    if (tick > maxTick)
                    {
                        tick = 0;
                        hasHit = false;
                    }
                }
            }
            else
            {
                attacking = false;
            }
        }

        // Attack Barricade Method
        private void Attack(Barricade player)
        {
            attack = false;

            if (this.hitBox.Intersects(player.HitBox))
                attack = true;
            else if (this.hitBox.Contains(player.HitBox))
                attack = true;
            else if (this.HitBox.Intersects(new Rectangle(player.HitBox.X, player.HitBox.Y, player.HitBox.Width + 10, player.HitBox.Height + latency)))
                attack = true;
            else if (this.HitBox.Intersects(new Rectangle(player.HitBox.X, player.HitBox.Y, player.HitBox.Width + 10, player.HitBox.Height - latency)))
                attack = true;
            else if (this.HitBox.Intersects(new Rectangle(player.HitBox.X, player.HitBox.Y, player.HitBox.Width - 10, player.HitBox.Height + latency)))
                attack = true;
            else if (this.HitBox.Intersects(new Rectangle(player.HitBox.X, player.HitBox.Y, player.HitBox.Width - 10, player.HitBox.Height - latency)))
                attack = true;

            if (attack == true)
            {
                if (hasHit == false)
                {
                    animationState = 6;
                    attacking = true;
                    player.Health -= damage;
                    hasHit = true;
                    tick++;
                }

                else
                {
                    if (tick > maxTick)
                    {
                        tick = 0;
                        hasHit = false;
                    }
                }
            }
            else
            {
                attacking = false;
            }
        }

        private int[] zombieMovement(Player player)
        {
            // X value finding
            int moveX = 0;

            // Right but close
            if (HitBox.X < player.HitBox.X && HitBox.X > player.HitBox.X + 20)
                moveX = 0;

            // Right
            else if (HitBox.X < player.HitBox.X)
                moveX += Speed;

            // Left but close
            else if (HitBox.X > player.HitBox.X && HitBox.X < player.HitBox.X - 20)
                moveX = 0;
            
            // Left
            else if (HitBox.X > player.HitBox.X)
                moveX -= Speed;
                                
            // Dont move
            else
                moveX = 0;                
            
            // Y value finding
            int moveY = 0;

            // Down but close
            if (HitBox.Y < player.HitBox.Y && HitBox.Y > player.HitBox.Y + 20)
                moveY = 0;
            
            // Down
            else if (HitBox.Y < player.HitBox.Y)
                moveY += Speed;
            

            // Up but close
            else if (HitBox.Y > player.HitBox.Y && HitBox.Y < player.HitBox.Y - 20)            
                moveY = 0;
            
            // Up;
            else if (HitBox.Y > player.HitBox.Y)     
                moveY -= Speed;
            
            // Dont move
            else            
                moveY = 0;            

            // If its colliding with player
            if (this.hitBox.Intersects(player.HitBox))
            {
                int X = HitBox.X - player.HitBox.X;
                int Y = HitBox.Y - player.HitBox.Y;
                

                // Stop moving in the direction
                if ((X < 0 && X > -100))
                {
                    // Left
                    moveX = 0;
                }

                else if (X > 0 && X < 100)
                {
                    
                    // Right
                    moveX = 0;
                }

                if ((Y < 0 && Y > -100))
                {
                    // Top
                    moveY = 0;
                }

                else if (Y > 0 && Y < 100)
                {
                    // Bottom
                    moveY = 0;
                }
            }

            updateRotation(player);

            if (moveX != 0 || moveY != 0)
            {
                moving = true;
            }
            else
            {
                moving = false;
            }

            updateAnimation();            

            int[] num = new int[2];
            num[0] = moveX;
            num[1] = moveY;
            return num;
        }

        private void updateAnimation()
        {
            if (attacking)
            {
                animationTick++;
                if (animationTick >= 10)
                {
                    animationState++;
                    animationTick = 0;
                    if (animationState >= 11)
                    {
                        attacking = false;
                        animationState = 0;
                    }

                }
            }
            else if (moving)
            {
                animationTick++;
                if (animationTick >= 10)
                {
                    animationTick = 0;
                    animationState++;
                    if (animationState >= 6)
                    {
                        animationState = 0;
                    }
                }
            }
            else
            {
                animationState = 0;
                animationTick = 0;
            }
        }

        private void updateRotation(Player closest)
        {
            Vector2 closeVec = new Vector2(closest.Pos.X, closest.Pos.Y);
            Vector2 zombiePos = new Vector2(pos.X, pos.Y);
            Vector2 direction = closeVec - zombiePos;
            float angle = (float)Math.Atan2(-direction.Y, -direction.X);
            rotation = angle + (MathHelper.Pi);
        }

        // Zombie collisions
        private int[] zombieCollide(Zombie zombie, bool inside)
        {
            int moveX = 0;
            int moveY = 0;
            if (inside == true)
            {
                // X value finding
                moveX = 0;

                // Right but close
                if (HitBox.X < zombie.HitBox.X && HitBox.X > zombie.HitBox.X + 10)
                    moveX = 0;

                // Right
                else if (HitBox.X < zombie.HitBox.X)
                    moveX -= zombie.Speed;

                // Left but close
                else if (HitBox.X > zombie.HitBox.X && HitBox.X < zombie.HitBox.X - 10)
                    moveX = 0;

                // Left
                else if (HitBox.X > zombie.HitBox.X)
                    moveX += zombie.Speed;

                // Dont move
                else
                    moveX = 0;

                // Y value finding
                moveY = 0;

                // Down but close
                if (HitBox.Y < zombie.HitBox.Y && HitBox.Y > zombie.HitBox.Y + 10)
                    moveY = 0;

                // Down
                else if (HitBox.Y < zombie.HitBox.Y)
                    moveY -= zombie.Speed;

                // Up but close
                else if (HitBox.Y > zombie.HitBox.Y && HitBox.Y < zombie.HitBox.Y - 10)
                    moveY = 0;

                // Up;
                else if (HitBox.Y > zombie.HitBox.Y)
                    moveY += zombie.Speed;

                // Dont move
                else
                    moveY = 0;               
            
            }

            int[] num = new int[2];
            num[0] = moveX;
            num[1] = moveY;
            return num;
        }

        private void playerPush(Player player, bool inside)
        {
            if (inside == true)
            {

                int moveX = 0;
                int moveY = 0;

                // X value finding
                moveX = 0;
                // Right but close
                if (HitBox.X < player.HitBox.X && HitBox.X > player.HitBox.X + 10)
                    moveX = 0;
                // Right
                else if (HitBox.X < player.HitBox.X) 
                    moveX -= player.Speed; 
                // Left but close
                else if (HitBox.X > player.HitBox.X && HitBox.X < player.HitBox.X - 10)
                    moveX = 0;
                // Left
                else if (HitBox.X > player.HitBox.X) 
                    moveX += player.Speed;
                // Dont move
                else
                    moveX = 0;

                // Y value finding
                moveY = 0;
                // Down but close
                if (HitBox.Y < player.HitBox.Y && HitBox.Y > player.HitBox.Y + 10)
                    moveY = 0;
                // Down
                else if (HitBox.Y < player.HitBox.Y)                
                    moveY -= player.Speed;
                
                // Up but close
                else if (HitBox.Y > player.HitBox.Y && HitBox.Y < player.HitBox.Y - 10)
                    moveY = 0;
                // Up;
                else if (HitBox.Y > player.HitBox.Y)                
                    moveY += player.Speed;
                
                // Dont move
                else
                    moveY = 0;

                hitBox.X += moveX;
                hitBox.Y += moveY;
                pos.X += moveX;
                pos.Y += moveY;
            }
        }

        private int[] crateCollide(Crate crate, bool inside)
        {
            int moveX = 0;
            int moveY = 0;

            if (inside == true)
            {
                // X value finding
                moveX = 0;

                // Right but close
                if (HitBox.X < crate.HitBox.X && HitBox.X > crate.HitBox.X + 10)
                    moveX = 0;

                // Right
                else if (HitBox.X < crate.HitBox.X)
                    moveX -= Speed;

                // Left but close
                else if (HitBox.X > crate.HitBox.X && HitBox.X < crate.HitBox.X - 10)
                    moveX = 0;

                // Left
                else if (HitBox.X > crate.HitBox.X)
                    moveX += Speed;

                // Dont move
                else
                    moveX = 0;

                // Y value finding
                moveY = 0;

                // Down but close
                if (HitBox.Y < crate.HitBox.Y && HitBox.Y > crate.HitBox.Y + 10)
                    moveY = 0;

                // Down
                else if (HitBox.Y < crate.HitBox.Y)
                    moveY -= Speed;

                // Up but close
                else if (HitBox.Y > crate.HitBox.Y && HitBox.Y < crate.HitBox.Y - 10)
                    moveY = 0;

                // Up;
                else if (HitBox.Y > crate.HitBox.Y)
                    moveY += Speed;

                // Dont move
                else
                    moveY = 0;

            }

            int[] num = new int[2];
            num[0] = moveX;
            num[1] = moveY;
            return num;
        }

        private int[] barricadeCollide(Barricade barricade, bool inside)
        {
            int moveX = 0;
            int moveY = 0;

            if (inside == true)
            {
                // X value finding
                moveX = 0;

                // Right but close
                if (HitBox.X < barricade.HitBox.X && HitBox.X > barricade.HitBox.X + 10)
                    moveX = 0;

                // Right
                else if (HitBox.X < barricade.HitBox.X)
                    moveX -= Speed;

                // Left but close
                else if (HitBox.X > barricade.HitBox.X && HitBox.X < barricade.HitBox.X - 10)
                    moveX = 0;

                // Left
                else if (HitBox.X > barricade.HitBox.X)
                    moveX += Speed;

                // Dont move
                else
                    moveX = 0;

                // Y value finding
                moveY = 0;

                // Down but close
                if (HitBox.Y < barricade.HitBox.Y && HitBox.Y > barricade.HitBox.Y + 10)
                    moveY = 0;

                // Down
                else if (HitBox.Y < barricade.HitBox.Y)
                    moveY -= Speed;

                // Up but close
                else if (HitBox.Y > barricade.HitBox.Y && HitBox.Y < barricade.HitBox.Y - 10)
                    moveY = 0;

                // Up;
                else if (HitBox.Y > barricade.HitBox.Y)
                    moveY += Speed;

                // Dont move
                else
                    moveY = 0;

            }

            int[] num = new int[2];
            num[0] = moveX;
            num[1] = moveY;
            return num;
        }
    }
}