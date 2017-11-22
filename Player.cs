using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame
{
    class Player
    {

        int health;
        public int Health { get { return health; } set { health = value; } }

        public int Speed { get { return speed; } set { speed = value; } }
        int speed;
        int totalStamina;
        int currentStamina;
        int staminaReplenishTick;
        int replenishTick;
        int soundTick;
        public int CurrentStamina { get { return currentStamina; } }
        public int TotalStamina { get { return totalStamina; } }
        bool stopKey = false;

        int animationState;
        int animationTick;
        int startAnimationState, endAnimationState;
        public int AnimationState { get { return animationState; } }

        MouseState previousMouseState;
        MouseState currentMouseState;
        public int HealthUpTick { get; set; }
        Rectangle pos;
        Vector2 gunPos;
        public Vector2 GunPos { get { return gunPos; } }
        Gun primaryGun;
        Gun secondaryGun;
        Gun currentGun;
        public Gun CurrentGun { get { return currentGun; } }
        public Gun PrimaryGun { get { return primaryGun; } }
        public Gun SecondaryGun { get { return secondaryGun; } }
        //ammo array (1- pistol, 2- shotgun, 3- rifle, 4- sniper, 5 - machine gun)
        int[] ammo = new int[5];
        int currentGunAmmo;
        public int CurrentGunAmmo { get { return currentGunAmmo; } }
        public int[] Ammo { get { return ammo; } }
        bool reloading;
        int reloadTick;
        public int ReloadTick { get { return reloadTick; } }
        int shootTimeout = 0;
        bool sprinting = false;
        public bool Slow { get; set; }
        public bool SlowSmall { get; set; }
        public bool SlowBig { get; set; }
        bool inventoryOpen = false;
        public bool InventoryOpen { get { return inventoryOpen; } set { inventoryOpen = value; } }
        public int ShootTimeout { get { return shootTimeout; } }
        public bool Reloading { get { return reloading; } }
        float rotation;
        public float Rotation { get { return rotation; } }

        public Rectangle Pos { get { return pos; } set { pos = value; } }

        Rectangle hitBox;
        public Rectangle HitBox { get { return hitBox; } }
        Items items = new Items();

        public MuzzleFlash Flash { get; set; }

        SoundManager soundManager;

        public Player(Rectangle position, int speed, int health, Rectangle hit, SoundManager soundManager)
        {
            pos = position;
            hitBox = hit;
            this.speed = speed;
            this.health = health;
            animationState = 0;
            totalStamina = 300;
            currentStamina = totalStamina;
            this.soundManager = soundManager;

            //TODO delete these defaults
            primaryGun = items.GetGun("gun_Pistol");
            secondaryGun = null;
            currentGun = primaryGun;
            ammo[0] = 500;
            currentGunAmmo = currentGun.MagCapacity;
            updateAnimationStates(false);
        }

        private void updateAnimationStates(bool overrideReload)
        {
            if (!reloading || overrideReload)
            {
                if (currentGun.Name == "gun_AssaultRifle" || currentGun.Name == "gun_MachineGun")
                {
                    startAnimationState = 12;
                    endAnimationState = 17;
                }
                if (currentGun.Name == "gun_Pistol")
                {
                    startAnimationState = 0;
                    endAnimationState = 5;
                }
                if (currentGun.Name == "gun_Shotgun" || currentGun.Name == "gun_SniperRifle")
                {
                    startAnimationState = 6;
                    endAnimationState = 11;
                }
            }
            else
            {
                if (currentGun.Name == "gun_AssaultRifle" || currentGun.Name == "gun_MachineGun")
                {
                    startAnimationState = 30;
                    endAnimationState = 35;
                }
                if (currentGun.Name == "gun_Pistol")
                {
                    startAnimationState = 18;
                    endAnimationState = 23;
                }
                if (currentGun.Name == "gun_Shotgun" || currentGun.Name == "gun_SniperRifle")
                {
                    startAnimationState = 24;
                    endAnimationState = 29;
                }
            }

            
            animationState = startAnimationState;
        }

        public void changeGun(int gun)
        {
            bool playSound = false;
            if (gun == 0)
            {
                if (currentGun == primaryGun)
                {
                    return;
                }
                else
                {
                    ammo[currentGun.AmmoType] += currentGunAmmo;
                    currentGun = primaryGun;
                    currentGunAmmo = 0;
                    reload();
                    playSound = true;
                }
            }
            if (gun == 1)
            {
                if (currentGun == secondaryGun)
                {
                    return;
                }
                else
                {
                    if (secondaryGun == null)
                    {
                        return;
                    }
                    ammo[currentGun.AmmoType] += currentGunAmmo;
                    currentGun = secondaryGun;
                    currentGunAmmo = 0;
                    reload();
                    playSound = true;
                }
            }


            if (currentGun.Name == "gun_Pistol")
            {
                soundManager.playSound("reload_pistol");
            }
            else if (currentGun.Name == "gun_Shotgun")
            {
                soundManager.playSound("reload_shotgun");
            }
            else if (currentGun.Name == "gun_AssaultRifle")
            {
                soundManager.playSound("reload_assault");
            }
            else if (currentGun.Name == "gun_SniperRifle" || currentGun.Name == "gun_MachineGun")
            {
                soundManager.playSound("reload_machineGun");
            }
        }

        public void updateMovement(World world, List<Zombie> zombieList, List<Bullet> bulletList, List<Crate> crateList, List<Gun> gunList, List<Barricade> barricadeList)
        {
            KeyboardState key = Keyboard.GetState();

            int moveX = 0;
            int moveY = 0;
            if (key.IsKeyDown(Keys.W))
            {
                moveY = speed;
            }
            if (key.IsKeyDown(Keys.S))
            {
                moveY = 0 - speed;
            }
            if (key.IsKeyDown(Keys.A))
            {
                moveX = speed;
            }
            if (key.IsKeyDown(Keys.D))
            {
                moveX = 0 - speed;
            }

            //slowing
            if (Slow == true)
            {
                moveX /= 2;
                moveY /= 2;
            }

            //sprinting
            if (key.IsKeyDown(Keys.LeftShift) && currentStamina > 0 && (moveX != 0 || moveY != 0))
            {
                replenishTick = 0;
                currentStamina--;
                moveX = (int)(moveX * 2);
                moveY = (int)(moveY * 2);
                sprinting = true;
            }
            else
            {
                sprinting = false;
            }


            if (moveX != 0 || moveY != 0)
            {
                staminaReplenishTick = 3;
            }
            else
            {
                staminaReplenishTick = 1;
            }

            if (key.IsKeyDown(Keys.Tab) && !stopKey)
            {
                inventoryOpen = !inventoryOpen;
                stopKey = true;
            }
            if (key.IsKeyUp(Keys.Tab))
            {
                stopKey = false;
            }
            
            if (reloading)
            {
                moveX /= 2;
                moveY /= 2;
            }

            if (key.IsKeyDown(Keys.F))
            {
                for (int i = 0; i < gunList.Count; i++)
                {
                    if (gunList[i].CanPickup)
                    {
                        if(currentGun == primaryGun)
                        {
                            if (secondaryGun == null)
                            {
                                secondaryGun = gunList[i];
                            }
                            else
                            {
                                primaryGun = gunList[i];
                            }
                        }
                        else if(currentGun == secondaryGun)
                        {
                            secondaryGun = gunList[i];
                        }
                        currentGun = gunList[i];
                        gunList.RemoveAt(i);
                        updateAnimationStates(false);
                        reload();
                        break;
                    }
                } 
            }

            if ((moveX != 0 || moveY != 0) && soundTick <= 0)
            {
                if (sprinting == true)
                {
                    soundManager.playSound("sprinting");
                    soundTick = 25;
                }
                else
                {
                    soundManager.playSound("walking");
                    soundTick = 35;
                }
            }
            else
            {
                soundTick--;
            }

            world.move(moveX, moveY, zombieList, bulletList, hitBox, crateList, gunList, barricadeList);


            if (moveX != 0 || moveY != 0 || reloading)
            {
                animationTick++;
                if ((animationTick >= 5 && !sprinting) || (animationTick >= 3 && sprinting))
                {
                    animationState++;
                    animationTick = 0;

                    if (animationState == endAnimationState + 1)
                    {
                        if (reloading)
                        {
                            updateAnimationStates(true);
                        }
                        animationState = startAnimationState;
                    }
                }
            }
            else
            {
                animationState = startAnimationState;
            }

            if (reloading)
            {
                reloadTick--;
                if (reloadTick <= 0)
                {
                    reloading = false;
                    int refill = currentGunAmmo;
                    if (ammo[CurrentGun.AmmoType] - currentGun.MagCapacity < 0)
                    {
                        currentGunAmmo = refill + ammo[currentGun.AmmoType];
                        ammo[currentGun.AmmoType] = 0;
                    }
                    else
                    {
                        currentGunAmmo = currentGun.MagCapacity;
                        ammo[currentGun.AmmoType] -= currentGun.MagCapacity - refill;
                    }
                }
            }
            if (HealthUpTick == 420)
            {
                health += 50;
                HealthUpTick = 0;
                if (health > 100)
                {
                    health = 100;
                }
            }
            else if (HealthUpTick > 0)
            {
                HealthUpTick--;
                if (HealthUpTick%10 == 0)
                {
                    health++;
                    if (HealthUpTick == 0)
                    {
                        health += 5;
                    }
                    if (health > 100)
                    {
                        health = 100;
                    }
                }
            }
        }

        public void replenishStamina()
        {
            int replenishTime = 60;
            if (replenishTick >= replenishTime)
            {
                if (replenishTick >= (staminaReplenishTick + replenishTime))
                {
                    replenishTick = replenishTime;

                    currentStamina++;
                    if (currentStamina > totalStamina)
                    {
                        currentStamina = totalStamina;
                    }
                }
                else
                {
                    replenishTick++;
                }
            }
            else
            {
                replenishTick++;
            }
        }

        public void UpdateRotation()
        {
            MouseState mouse = Mouse.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = mouse;
            
            Vector2 mouseVec = new Vector2(mouse.X, mouse.Y);
            Vector2 playerPos = new Vector2(Pos.X, Pos.Y);
            Vector2 direction = mouseVec - playerPos;
            float angle = (float)Math.Atan2(-direction.Y, -direction.X);

            angle += (float)(Math.PI * 0.5f);
            rotation = (float)(angle - ((6 * Math.PI) / 180));
            
            float xPos = (float)(pos.X - (30 * Math.Cos(rotation)));
            float yPos = (float)(pos.Y - (30 * Math.Sin(rotation)));
            gunPos = new Vector2(xPos, yPos);
        }

        public Bullet[] shoot()
        {
            
            Bullet[] bullets = new Bullet[0];
            if (inventoryOpen)
            {
                return bullets;
            }
            if (currentGunAmmo > 0 && !reloading)
            {
                if (currentGun.Name == "gun_Pistol")
                {
                    soundManager.playSound("fire_pistol");
                }
                else if (currentGun.Name == "gun_Shotgun")
                {
                    soundManager.playSound("fire_shotgun");
                }
                else if (currentGun.Name == "gun_AssaultRifle")
                {
                    soundManager.playSound("fire_assault");
                }
                else if (currentGun.Name == "gun_SniperRifle")
                {
                    soundManager.playSound("fire_sniper");
                }
                else if (currentGun.Name == "gun_MachineGun")
                {
                    soundManager.playSound("fire_machineGun");
                }
                currentGunAmmo--;

                double X = -(Math.Sin(rotation));
                double Y = Math.Cos(rotation);
                Vector2 direction = new Vector2((float)(X * currentGun.BulletSpeed), (float)(Y * currentGun.BulletSpeed));

                bullets = currentGun.fire((int)gunPos.X, (int)gunPos.Y, direction);
                shootTimeout = (int)Math.Round(currentGun.FireRate);

                Flash = new MuzzleFlash(gunPos, rotation);

                return bullets;
            }
            else
            {
                if (!reloading && currentGunAmmo == 0)
                {
                    reload();
                }
                return bullets;
            }
        }

        public void reload()
        {
            if (currentGun == SecondaryGun && secondaryGun == null)
            {
                return;
            }
            if (ammo[currentGun.AmmoType] <= 0)
            {
                // dont reload 
                return;
            }
            
            reloading = true;
            reloadTick = CurrentGun.ReloadTime;
            
                if (currentGun.Name == "gun_Pistol")
                {
                    soundManager.playSound("reload_pistol");
                }
                else if (currentGun.Name == "gun_Shotgun")
                {
                    soundManager.playSound("reload_shotgun");
                }
                else if (currentGun.Name == "gun_AssaultRifle")
                {
                    soundManager.playSound("reload_assault");
                }
                else if (currentGun.Name == "gun_SniperRifle" || currentGun.Name == "gun_MachineGun")
                {
                    soundManager.playSound("reload_machineGun");
                }
            
                updateAnimationStates(false);
        }

        public void AddAmmo(string name, int amount)
        {
            switch (name)
            {
                case "ammo_AssaultRifleAmmo":
                    ammo[2] += amount;
                    break;
                case "ammo_SniperRifleAmmo":
                    ammo[3] += amount;
                    break;
                case "ammo_MachineGunAmmo":
                    ammo[4] += amount;
                    break;
                case "ammo_PistolAmmo":
                    ammo[0] += amount;
                    break;
                case "ammo_ShotgunAmmo":
                    ammo[1] += amount;
                    break;
                default:
                    break;
            }
        }

        public void updateShootTimeout()
        {
            shootTimeout--;
            if (shootTimeout < 0)
            {
                shootTimeout = 0;
            }
        }

        public void swapGuns()
        {
            if (currentGun == primaryGun)
            {
                currentGun = secondaryGun;
            }
            else if (currentGun == secondaryGun)
            {
                currentGun = primaryGun;
            }
        }

        public bool SingleMouseKeyPress()
        {
            if (previousMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            else return false;
        }
    }
}
