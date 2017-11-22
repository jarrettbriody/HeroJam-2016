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
    class GameLoop
    {
        //class calls
        World world = new World();
        InventoryOverlay inventoryOverlay;
        Wave wave = new Wave();
        //variables
        //player
        Player[] player;
        Rectangle staminaRec, staminaBackgroundRec, healthRec, healthRecBackground;
        int staminaFade;
        int healthFade;
        int zombiesKilled;
        bool healthShow = false;
        Random rng;
        bool levelIsSpawned = false;
        public bool Pause { get; set; }
        public bool Quit { get; set; }
        public bool Play { get; set; }
        Random ran = new Random();
        public bool Options { get; set; }
        public bool Menu { get; set; }
        public bool Instructions { get; set; }
        public bool Death { get; set; }
        public bool Credit { get; set; }

        private MouseState prevMouseState;
        KeyboardState prevKey;
        string crateMessage;
        int crateMessageTicks = 120;

        string gunPickupNotification = "Press F to pick up weapon.";
        bool showPickupNotification = false;

        //bullets
        List<Bullet> bulletList = new List<Bullet>();

        // Zombies
        List<Zombie> zombieList;

        //CRATES MAFUGGAS
        List<Crate> crateList = new List<Crate>();

        //guns on ground
        List<Gun> gunList = new List<Gun>();

        List<Barricade> barricadeList = new List<Barricade>();

        bool itemUse = false;
        //other
        GraphicsDevice graphics;
        SpriteFont font;
        SpriteFont bigFont;
        SpriteFont menuFont;
        //textures
        Texture2D[] playerTextures, bulletTextures, mapTextures, zombieTextures, textures;
        Arrow zombieArrow = new Arrow(), crateArrow = new Arrow();
        SoundManager soundManager;

        public void StartUp(GraphicsDevice graphics, InventoryOverlay invOver, Texture2D[] playerPics, Texture2D[] bulletPics, Texture2D[] mapPics, Texture2D[] zombiePics, Texture2D[] otherTextures, SpriteFont font, SpriteFont bigFont, SpriteFont MenuFont, SoundManager soundManager)
        {
            //setup variables
            this.graphics = graphics;
            inventoryOverlay = invOver;
            this.soundManager = soundManager;
            //texture variable set-up
            playerTextures = playerPics;
            bulletTextures = bulletPics;
            mapTextures = mapPics;
            zombieTextures = zombiePics;
            textures = otherTextures;
            this.font = font;
            this.bigFont = bigFont;
            this.menuFont = MenuFont;
            //health bar rectangles
            healthRecBackground = new Rectangle(25, 25, 500, 50);
            healthRec = new Rectangle(healthRecBackground.X + 5, healthRecBackground.Y + 5, 490, 40);
            prevKey = Keyboard.GetState();
            zombiesKilled = 0;

            //stamina bar rectangles
            staminaBackgroundRec = new Rectangle(healthRecBackground.X, healthRecBackground.Y + 60, 306, 26);
            staminaRec = new Rectangle(staminaBackgroundRec.X + 3, staminaBackgroundRec.Y + 3, 290, 20);

            //stamina bar rectangles
            staminaBackgroundRec = new Rectangle(healthRecBackground.X, healthRecBackground.Y + 60, 306, 26);
            staminaRec = new Rectangle(staminaBackgroundRec.X + 3, staminaBackgroundRec.Y + 3, 290, 20);

            //set up world class
            world.createWorld();
            rng = new Random();
            //create the player obj
            player = new Player[1];

            int xPos = graphics.Viewport.Width / 2;
            int yPos = graphics.Viewport.Height / 2;

            Rectangle playerPos = new Rectangle(xPos, yPos, 100, 100);
            Rectangle hitBox = playerPos;
            hitBox.X -= 50;
            hitBox.Y -= 50;
            player[0] = new Player(playerPos, 5, 100, hitBox, soundManager);
            player[0].reload();

            zombieList = new List<Zombie>();

            zombieArrow.setUp(new Vector2(player[0].Pos.X, player[0].Pos.Y), new Vector2(0, 0));
            crateArrow.setUp(new Vector2(player[0].Pos.X, player[0].Pos.Y), new Vector2(0, 0));

            inventoryOverlay.AddItem("item_Bandage", 2);
            prevMouseState = Mouse.GetState();
            Pause = false;
            Quit = false;
        }

        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState key = Keyboard.GetState();
            if(player[0].Health <= 0)
            {
                Death = true;
            }

            if (Pause == false)
            {
                //player movement+rotation update (and shooting)
                showPickupNotification = false;
            foreach (var item in gunList)
            {
                item.HitDetection(player);
                if (item.CanPickup) showPickupNotification = true;
            }
            foreach (var item in player)
            {
                item.updateMovement(world, zombieList, bulletList, crateList, gunList, barricadeList);
                item.UpdateRotation();
                if (item.CurrentStamina < item.TotalStamina)
                {
                    item.replenishStamina();
                }

                    //TODO update this code to accomodate for controllers

                    if (key.IsKeyDown(Keys.R) && !item.Reloading)
                    {
                        if (item.CurrentGunAmmo != item.CurrentGun.MagCapacity)
                        {
                            item.reload();
                        }
                    }

                    if (key.IsKeyDown(Keys.D1) && !itemUse)
                    {
                        itemUse = true;
                        item.changeGun(0);
                    }
                    else if (key.IsKeyDown(Keys.D2) && !itemUse)
                    {
                        itemUse = true;
                        item.changeGun(1);
                    }
                    else if (key.IsKeyDown(Keys.D3) && !itemUse)
                    {
                        itemUse = true;
                        if (inventoryOverlay.getItemCount(3) > 0 && item.Health < 100)
                        {
                            item.HealthUpTick = 200;
                            inventoryOverlay.AddItem("item_Bandage", -1);
                        }
                    }
                    else if (key.IsKeyDown(Keys.D4) && !itemUse)
                    {
                        itemUse = true;
                        if (inventoryOverlay.getItemCount(4) > 0 && item.Health < 100)
                        {
                            item.HealthUpTick = 420;
                            inventoryOverlay.AddItem("item_FirstAid", -1);
                        }
                    }
                    if (key.IsKeyUp(Keys.D1) && key.IsKeyUp(Keys.D2) && key.IsKeyUp(Keys.D3) && key.IsKeyUp(Keys.D4))
                    {
                        itemUse = false;
                    }


                    if ((mouse.LeftButton == ButtonState.Pressed && item.ShootTimeout == 0 && item.CurrentGun.FullAuto == true) ||
                        (item.CurrentGun.FullAuto == false && item.SingleMouseKeyPress() && item.ShootTimeout == 0))
                    {
                        //shoot gun/create bullet[s]
                        Bullet[] bullets = item.shoot();
                        foreach (var bullet in bullets)
                        {
                            bulletList.Add(bullet);
                        }

                    }
                    else if (item.ShootTimeout != 0)
                    {
                        //allow shooting again
                        item.updateShootTimeout();
                    }

                    if (item.InventoryOpen)
                    {
                        inventoryOverlay.Update();
                    }

                }

                //bullet movement update
                for (int i = 0; i < bulletList.Count; i++)
                {
                    if (bulletList[i].Alive == false)
                    {
                        bulletList.Remove(bulletList[i]);
                    }
                    else
                    {
                        bulletList[i].updateMovement();
                        bulletList[i].HitDetection(zombieList);
                    }

                }

                //zombie movement update
                for (int i = 0; i < zombieList.Count; i++)
                {
                    if (zombieList[i].Health <= 0)
                    {
                        zombieList.Remove(zombieList[i]);
                        zombiesKilled++;
                    }
                    else
                    {
                        zombieList[i].updateZombie();

                        if (zombieList[i].soundTick <= 0)
                        {
                            zombieList[i].soundTick = 120;
                            Vector2 vec = new Vector2(zombieList[i].Pos.X, zombieList[i].Pos.Y) - new Vector2(player[0].Pos.X, player[0].Pos.Y);
                            if (vec.Length() <= 500)
                            {
                                playZombieSound();
                            }
                        }
                        else
                        {
                            zombieList[i].soundTick--;
                        }
                    }
                }

                //wave spawn
                if (levelIsSpawned == false)
                {
                    wave.WaveLevel++;
                    crateList.Clear();
                    gunList.Clear();
                    crateList = wave.spawnCrates(world.WorldRec[1, 1].X, world.WorldRec[1, 1].Y, world.WorldRec[world.WorldSize - 2, world.WorldSize - 2].X, world.WorldRec[world.WorldSize - 2, world.WorldSize - 2].Y);
                    

                    zombieList = wave.spawnZombies(player, world, bulletList, crateList, barricadeList);
                    foreach (Zombie zombie in zombieList)
                        zombie.Zombies = zombieList;
                    levelIsSpawned = true;
                }

                //crate updomo remigato mr roboto
                crateMessageTicks--;
                for (int i = 0; i < crateList.Count; i++)
                {
                    for (int j = 0; j < player.Length; j++)
                    {
                        crateList[i].HitDetection(player);
                        if (crateList[i].Alive == false)
                        {
                            if (crateList[i].Weapon != null)
                            {
                                Rectangle temp = crateList[i].HitBox;
                                crateList[i].Weapon.HitBox = temp;
                                gunList.Add(crateList[i].Weapon);
                                crateMessage = "The crate contained a(n)  " + crateList[i].Weapon.ToString() + ".";
                                crateList.RemoveAt(i);
                            }
                            else if (crateList[i].Item != null)
                            {
                                if (crateList[i].Item.Contains("item_"))
                                {
                                    inventoryOverlay.AddItem(crateList[i].Item, crateList[i].Amount);
                                    crateMessage = "The crate contained " + crateList[i].Amount + " " + crateList[i].ToString() + "(s).";
                                }
                                else if (crateList[i].Item.Contains("ammo_"))
                                {
                                    player[j].AddAmmo(crateList[i].Item, crateList[i].Amount);
                                    crateMessage = "The crate contained " + crateList[i].Amount + " " + crateList[i].ToString() + ".";
                                }
                                crateList.RemoveAt(i);
                                crateMessageTicks = 120;
                            }
                        }
                    }
                }
                if (crateMessageTicks == 0)
                {
                    crateMessage = null;
                }

                if (zombieList.Count == 0)
                {
                    levelIsSpawned = false;
                }

                //update zombie arrow
                Vector2 closeZombiePos = new Vector2(0, 0);
                int length = 100000;
                foreach (var item in zombieList)
                {
                    Vector2 vecLength = new Vector2(player[0].Pos.X, player[0].Pos.Y) - new Vector2(item.Pos.X, item.Pos.Y);
                    float vectorLength = Math.Abs(vecLength.Length());
                    if ((int)(vectorLength) < length)
                    {
                        closeZombiePos = new Vector2(item.Pos.X, item.Pos.Y);
                        length = (int)vectorLength;
                    }
                }
                zombieArrow.updateArrow(closeZombiePos);

                //update crate arrow
                Vector2 closeCratePos = new Vector2(0, 0);
                length = 100000;
                foreach (var item in crateList)
                {
                    Vector2 vecLength = new Vector2(player[0].Pos.X, player[0].Pos.Y) - new Vector2(item.HitBox.X, item.HitBox.Y);
                    float vectorLength = Math.Abs(vecLength.Length());
                    if ((int)(vectorLength) < length)
                    {
                        closeCratePos = new Vector2(item.HitBox.X, item.HitBox.Y);
                        length = (int)vectorLength;
                    }
                }

                crateArrow.updateArrow(closeCratePos);
            }
        }

        private void playZombieSound()
        {
            int ranSound = ran.Next(7);
            string sound = ("zombie_" + ranSound);

            soundManager.playSound(sound);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(Death == true)
            {
                spriteBatch.DrawString(menuFont, "You are dead", new Vector2(200, 200), Color.Red);

                spriteBatch.DrawString(bigFont, "You survived to wave " + wave.WaveLevel, new Vector2(200, 400), Color.Red);
                spriteBatch.DrawString(bigFont, "You killed " + zombiesKilled + " zombies", new Vector2(200, 500), Color.Red);

                Color qColor = Color.White;
                Rectangle qBox = new Rectangle(200, 700, 250, 150);
                spriteBatch.Draw(textures[0], qBox, qColor);
                spriteBatch.DrawString(bigFont, "Back to Menu", new Vector2(qBox.X + 20, qBox.Y + 50), qColor);

                if (qBox.Contains(Mouse.GetState().Position))
                {
                    if (SingleMouseKeyPress(Mouse.GetState()))
                    {
                        soundManager.playSound("click");
                        Menu = true;
                        Death = false;
                    }

                    qColor = Color.Yellow;
                    spriteBatch.Draw(textures[0], qBox, qColor);
                    spriteBatch.DrawString(bigFont, "Back to Menu", new Vector2(qBox.X + 20, qBox.Y + 50), qColor);
                }
            }

            else if (Pause == false)
            {
                world.drawWorld(spriteBatch, mapTextures);

            zombieArrow.drawArrow(spriteBatch, textures[17], Color.Red);
            crateArrow.drawArrow(spriteBatch, textures[17], Color.Yellow);

            foreach (var item in crateList)
            {
                if (item.Alive)
                {
                    spriteBatch.Draw(textures[10], item.HitBox, Color.White);
                }
            }

            if (showPickupNotification)
            {
                Vector2 vec = new Vector2(((graphics.Viewport.Width / 2) - (font.MeasureString(gunPickupNotification).X / 2)), graphics.Viewport.Height - 50);
                spriteBatch.DrawString(font, gunPickupNotification, vec, Color.Black);
            }

            foreach (var item in gunList)
            {
                switch (item.Name)
                {
                    case "gun_Pistol":
                        spriteBatch.Draw(textures[12], item.HitBox, Color.White);
                        break;
                    case "gun_Shotgun":
                        spriteBatch.Draw(textures[16], item.HitBox, Color.White);
                        break;
                    case "gun_AssaultRifle":
                        spriteBatch.Draw(textures[13], item.HitBox, Color.White);
                        break;
                    case "gun_SniperRifle":
                        spriteBatch.Draw(textures[14], item.HitBox, Color.White);
                        break;
                    case "gun_MachineGun":
                        spriteBatch.Draw(textures[15], item.HitBox, Color.White);
                        break;
                    default:
                        break;
                }
            }

            if (crateMessage != null && crateMessage != "" && crateMessageTicks > 0)
            {
                Vector2 vec = new Vector2(graphics.Viewport.Width / 2 - font.MeasureString(crateMessage).X / 2, graphics.Viewport.Height / 2 + 100);
                spriteBatch.DrawString(font, crateMessage, vec, Color.Black);
            }

            foreach (var item in bulletList)
            {
                spriteBatch.Draw(bulletTextures[0], item.HitBox, Color.White);
            }

                foreach (var item in player)
                {
                    spriteBatch.Draw(texture: playerTextures[item.AnimationState], destinationRectangle: item.Pos, color: Color.White, rotation: item.Rotation + (float)(Math.PI / 2), origin: new Vector2(50, 50));
                    if (item.Flash != null)
                    {
                        if (item.Flash.TicksLeft <= 0)
                        {
                            item.Flash = null;
                        }
                        else
                        {
                            item.Flash.DrawFlash(spriteBatch, bulletTextures);
                        }
                    }
                }

                foreach (var item in zombieList)
                {
                    Color color = Color.White;

                    if (item is BigBoi)
                        color = Color.Blue;

                    else if (item is Sprinter)
                        color = Color.Red;

                    spriteBatch.Draw(texture: zombieTextures[item.AnimationState], destinationRectangle: item.Pos, color: color, rotation: item.Angle, origin: new Vector2(item.Pos.Width / 2, item.Pos.Height / 2));
                }

                foreach (var item in barricadeList)
                {
                    if (item.HitBox.Width == 300)
                        spriteBatch.Draw(textures[19], item.HitBox, item.Color);
                    else if (item.HitBox.Height == 300)
                        spriteBatch.Draw(textures[20], item.HitBox, item.Color);
                }

                foreach (var item in player)
                {
                    if (item.CurrentStamina < item.TotalStamina || staminaFade > 0)
                    {
                        drawStaminaBar(item, spriteBatch);
                    }
                    if (item.Health < 100 || healthShow)
                    {
                        drawHealthBar(item, spriteBatch);
                    }
                    if (item.InventoryOpen)
                    {
                        inventoryOverlay.Draw(spriteBatch, item);
                    }
                    else
                    {
                        if (item.CurrentGunAmmo == 0 || item.Reloading)
                        {
                            drawReloadBar(item, spriteBatch);
                        }
                        drawSideUI(spriteBatch, item);

                        string text = "Wave: " + wave.WaveLevel + "   Zombies Remaining: " + zombieList.Count;
                        spriteBatch.DrawString(bigFont, text, new Vector2(healthRecBackground.Right + 30, 20), Color.DarkRed);
                    }
                }

                if (inventoryOverlay.Building == true)
                {
                    Rectangle ghostBarricade;
                    Color ghostColor = new Color(Color.White, 0.5f);

                    if (prevKey.IsKeyDown(Keys.W))
                    {
                        ghostBarricade = new Rectangle(player[0].HitBox.Left - 100, player[0].HitBox.Top - 100, player[0].HitBox.Width * 3, player[0].HitBox.Height);
                        spriteBatch.Draw(textures[19], ghostBarricade, ghostColor);

                        if (SingleKeyPress(Keyboard.GetState()))
                        {
                            Barricade barricade = new Barricade(true, ghostBarricade.X, ghostBarricade.Y, Color.White);
                            barricadeList.Add(barricade);
                            inventoryOverlay.Building = false;
                        }
                    }

                    else if (prevKey.IsKeyDown(Keys.A))
                    {
                        ghostBarricade = new Rectangle(player[0].HitBox.Left - 100, player[0].HitBox.Top - 100, player[0].HitBox.Width, player[0].HitBox.Height * 3);
                        spriteBatch.Draw(textures[20], ghostBarricade, ghostColor);

                        if (SingleKeyPress(Keyboard.GetState()))
                        {
                            Barricade barricade = new Barricade(false, ghostBarricade.X, ghostBarricade.Y, Color.White);
                            barricadeList.Add(barricade);
                            inventoryOverlay.Building = false;
                        }
                    }

                    else if (prevKey.IsKeyDown(Keys.S))
                    {
                        ghostBarricade = new Rectangle(player[0].HitBox.Left - 100, player[0].HitBox.Bottom, player[0].HitBox.Width * 3, player[0].HitBox.Height);
                        spriteBatch.Draw(textures[19], ghostBarricade, ghostColor);

                        if (SingleKeyPress(Keyboard.GetState()))
                        {
                            Barricade barricade = new Barricade(true, ghostBarricade.X, ghostBarricade.Y, Color.White);
                            barricadeList.Add(barricade);
                            inventoryOverlay.Building = false;
                        }
                    }

                    else if (prevKey.IsKeyDown(Keys.D))
                    {
                        ghostBarricade = new Rectangle(player[0].HitBox.Right, player[0].HitBox.Y - 100, player[0].HitBox.Width, player[0].HitBox.Height * 3);
                        spriteBatch.Draw(textures[20], ghostBarricade, ghostColor);

                        if (SingleKeyPress(Keyboard.GetState()))
                        {
                            Barricade barricade = new Barricade(false, ghostBarricade.X, ghostBarricade.Y, Color.White);
                            barricadeList.Add(barricade);
                            inventoryOverlay.Building = false;
                        }
                    }
                }

                prevKey = Keyboard.GetState();
            }

            if(Pause == true)
            {
                spriteBatch.DrawString(bigFont, "PAUSE", new Vector2(400, 200), Color.White);

                Color rColor = Color.White;
                Rectangle rBox = new Rectangle(400, 250, 200, 150);
                spriteBatch.Draw(textures[0], rBox, rColor);
                spriteBatch.DrawString(bigFont, "RESUME", new Vector2(rBox.X + 50, rBox.Y + 50), rColor);

                if (rBox.Contains(Mouse.GetState().Position))
                {
                   if (SingleMouseKeyPress(Mouse.GetState()))
                    {
                        soundManager.playSound("click");
                        Pause = false;
                    }

                    rColor = Color.Yellow;
                    spriteBatch.Draw(textures[0], rBox, rColor);
                    spriteBatch.DrawString(bigFont, "RESUME", new Vector2(rBox.X + 50, rBox.Y + 50), rColor);
                }

                Color qColor = Color.White;
                Rectangle qBox = new Rectangle(400, 450, 200, 150);
                spriteBatch.Draw(textures[0], qBox, qColor);
                spriteBatch.DrawString(bigFont, "QUIT", new Vector2(qBox.X + 50, qBox.Y + 50), qColor);

                if (qBox.Contains(Mouse.GetState().Position))
                {
                    if (SingleMouseKeyPress(Mouse.GetState()))
                    {
                        soundManager.playSound("click");
                        Menu = true;
                    }

                    qColor = Color.Yellow;
                    spriteBatch.Draw(textures[0], qBox, qColor);
                    spriteBatch.DrawString(bigFont, "QUIT", new Vector2(qBox.X + 50, qBox.Y + 50), qColor);
                }
            }
        }

        public void DrawMenu(SpriteBatch spriteBatch)
        {
            if (Credit == true)
            {                
                Color qColor = Color.White;
                Rectangle qBox = new Rectangle(200, 700, 240, 100);
                spriteBatch.Draw(textures[0], qBox, qColor);
                spriteBatch.DrawString(bigFont, "Back to Menu", new Vector2(qBox.X + 10, qBox.Y + 20), qColor);

                if (qBox.Contains(Mouse.GetState().Position))
                {
                    if (SingleMouseKeyPress(Mouse.GetState()))
                    {
                        soundManager.playSound("click");
                        Credit = false;
                    }

                    qColor = Color.Yellow;
                    spriteBatch.Draw(textures[0], qBox, qColor);
                    spriteBatch.DrawString(bigFont, "Back to Menu", new Vector2(qBox.X + 10, qBox.Y + 20), qColor);
                }

                spriteBatch.DrawString(menuFont, "Credits", new Vector2(200, 50), Color.Red);
                spriteBatch.DrawString(bigFont, "Coehl Gleckner", new Vector2(200, 250), Color.Green);
                spriteBatch.DrawString(bigFont, "Justin Gourley", new Vector2(200, 350), Color.Blue);
                spriteBatch.DrawString(bigFont, "Jarrett Briody", new Vector2(200, 450), Color.Orange);
                spriteBatch.DrawString(bigFont, "Carlo Guttilla", new Vector2(200, 550), Color.Yellow);

                spriteBatch.DrawString(font, "Player and Zombie Sprites - Opengameart.org \nSound Assets: FreeSounds.org", new Vector2(700, 650), Color.White);
            }
            else if (Instructions == true)
            {
                spriteBatch.Draw(textures[18], new Rectangle(0, 0, 1600, 900), Color.White);

                Color qColor = Color.White;
                Rectangle qBox = new Rectangle(100, 800, 240, 100);
                spriteBatch.Draw(textures[0], qBox, qColor);
                spriteBatch.DrawString(bigFont, "Back to Menu", new Vector2(qBox.X + 10, qBox.Y + 20), qColor);

                if (qBox.Contains(Mouse.GetState().Position))
                {
                    if (SingleMouseKeyPress(Mouse.GetState()))
                    {
                        soundManager.playSound("click");
                        Instructions = false;
                    }

                    qColor = Color.Yellow;
                    spriteBatch.Draw(textures[0], qBox, qColor);
                    spriteBatch.DrawString(bigFont, "Back to Menu", new Vector2(qBox.X + 10, qBox.Y + 20), qColor);
                }
            }
            else
            {
                spriteBatch.DrawString(menuFont, "HORDE", new Vector2(400, 50), Color.Red);

                Color rColor = Color.White;
                Rectangle rBox = new Rectangle(400, 250, 200, 150);
                spriteBatch.Draw(textures[0], rBox, rColor);
                spriteBatch.DrawString(bigFont, "Play", new Vector2(rBox.X + 50, rBox.Y + 50), rColor);

            if (rBox.Contains(Mouse.GetState().Position))
            {
                if (SingleMouseKeyPress(Mouse.GetState()))
                {
                    soundManager.playSound("click");
                    Play = true;
                }

                    rColor = Color.Yellow;
                    spriteBatch.Draw(textures[0], rBox, rColor);
                    spriteBatch.DrawString(bigFont, "Play", new Vector2(rBox.X + 50, rBox.Y + 50), rColor);
                }

                Color qColor = Color.White;
                Rectangle qBox = new Rectangle(400, 450, 200, 150);
                spriteBatch.Draw(textures[0], qBox, qColor);
                spriteBatch.DrawString(bigFont, "Options", new Vector2(qBox.X + 50, qBox.Y + 50), qColor);

            if (qBox.Contains(Mouse.GetState().Position))
            {
                if (SingleMouseKeyPress(Mouse.GetState()))
                {
                    soundManager.playSound("click");
                    Options = true;
                }

                    qColor = Color.Yellow;
                    spriteBatch.Draw(textures[0], qBox, qColor);
                    spriteBatch.DrawString(bigFont, "Options", new Vector2(qBox.X + 50, qBox.Y + 50), qColor);
                }

                Color eColor = Color.White;
                Rectangle eBox = new Rectangle(400, 650, 200, 150);
                spriteBatch.Draw(textures[0], eBox, eColor);
                spriteBatch.DrawString(bigFont, "QUIT", new Vector2(eBox.X + 50, eBox.Y + 50), eColor);

            if (eBox.Contains(Mouse.GetState().Position))
            {
                if (SingleMouseKeyPress(Mouse.GetState()))
                {
                    soundManager.playSound("click");
                    Quit = true;
                }

                    eColor = Color.Yellow;
                    spriteBatch.Draw(textures[0], eBox, eColor);
                    spriteBatch.DrawString(bigFont, "QUIT", new Vector2(eBox.X + 50, eBox.Y + 50), eColor);
                }

                Color iColor = Color.White;
                Rectangle iBox = new Rectangle(700, 450, 250, 150);
                spriteBatch.Draw(textures[0], iBox, iColor);
                spriteBatch.DrawString(bigFont, "Instructions", new Vector2(iBox.X + 25, iBox.Y + 50), iColor);

                if (iBox.Contains(Mouse.GetState().Position))
                {
                    if (SingleMouseKeyPress(Mouse.GetState()))
                    {
                        soundManager.playSound("click");
                        Instructions = true;
                    }

                    iColor = Color.Yellow;
                    spriteBatch.Draw(textures[0], iBox, iColor);
                    spriteBatch.DrawString(bigFont, "Instructions", new Vector2(iBox.X + 25, iBox.Y + 50), iColor);
                }

                Color pColor = Color.White;
                Rectangle pBox = new Rectangle(700, 650, 250, 150);
                spriteBatch.Draw(textures[0], pBox, pColor);
                spriteBatch.DrawString(bigFont, "Credentials", new Vector2(pBox.X + 25, pBox.Y + 50), pColor);

                if (pBox.Contains(Mouse.GetState().Position))
                {
                    if (SingleMouseKeyPress(Mouse.GetState()))
                    {
                        soundManager.playSound("click");
                        Credit = true;
                    }

                    pColor = Color.Yellow;
                    spriteBatch.Draw(textures[0], pBox, pColor);
                    spriteBatch.DrawString(bigFont, "Credentials", new Vector2(pBox.X + 25, pBox.Y + 50), pColor);
                }
            }
        }

        public void DrawOptions(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(bigFont, "Options", new Vector2(400, 200), Color.White);

            Color eColor = Color.White;
            Rectangle eBox = new Rectangle(400, 650, 200, 150);
            spriteBatch.Draw(textures[0], eBox, eColor);
            spriteBatch.DrawString(bigFont, "Back", new Vector2(eBox.X + 50, eBox.Y + 50), eColor);

            if (eBox.Contains(Mouse.GetState().Position))
            {
                if (SingleMouseKeyPress(Mouse.GetState()))
                {
                    soundManager.playSound("click");
                    Menu = true;
                }

                eColor = Color.Yellow;
                spriteBatch.Draw(textures[0], eBox, eColor);
                spriteBatch.DrawString(bigFont, "Back", new Vector2(eBox.X + 50, eBox.Y + 50), eColor);
            }
            //sound active button
            Color sColor = Color.White;
            Rectangle sBox = new Rectangle(400, 300, 300, 150);
            spriteBatch.Draw(textures[0], sBox, sColor);
            spriteBatch.DrawString(bigFont, "Sound [" + soundManager.soundActive + "]", new Vector2(sBox.X + 50, sBox.Y + 50), sColor);

            if (sBox.Contains(Mouse.GetState().Position))
            {
                if (SingleMouseKeyPress(Mouse.GetState()))
                {
                    soundManager.playSound("click");
                    soundManager.soundActive = !soundManager.soundActive;
                }

                sColor = Color.Yellow;
                spriteBatch.Draw(textures[0], sBox, sColor);
                spriteBatch.DrawString(bigFont, "Sound [" + soundManager.soundActive + "]", new Vector2(sBox.X + 50, sBox.Y + 50), sColor);
            }
            //volume button
            Color upColor = Color.White;
            spriteBatch.DrawString(bigFont, "Volume [" + (int)(soundManager.Volume * 100) + "]  ", new Vector2(150, 500), Color.White);

            Rectangle upBox = new Rectangle(550, 450, 150, 150);
            spriteBatch.Draw(textures[0], upBox, upColor);
            spriteBatch.DrawString(bigFont, "[Up]", new Vector2(upBox.X + 50, upBox.Y + 50), upColor);

            if (upBox.Contains(Mouse.GetState().Position))
            {
                if (SingleMouseKeyPress(Mouse.GetState()))
                {
                    soundManager.Volume = soundManager.Volume + 0.05f;
                    if (soundManager.Volume > 1f)
                        soundManager.Volume = 1f;

                    soundManager.playSound("click");
                }

                upColor = Color.Yellow;
                spriteBatch.Draw(textures[0], upBox, upColor);
                spriteBatch.DrawString(bigFont, "[Up]", new Vector2(upBox.X + 50, upBox.Y + 50), upColor);
            }

            Color downColor = Color.White;
            Rectangle downBox = new Rectangle(400, 450, 150, 150);
            spriteBatch.Draw(textures[0], downBox, downColor);
            spriteBatch.DrawString(bigFont, "[Down]", new Vector2(downBox.X + 20, downBox.Y + 50), downColor);

            if (downBox.Contains(Mouse.GetState().Position))
            {
                if (SingleMouseKeyPress(Mouse.GetState()))
                {
                    soundManager.Volume = soundManager.Volume - 0.05f;
                    if (soundManager.Volume < 0f)
                        soundManager.Volume = 0f;
                    
                    soundManager.playSound("click");
                }

                downColor = Color.Yellow;
                spriteBatch.Draw(textures[0], downBox, downColor);
                spriteBatch.DrawString(bigFont, "[Down]", new Vector2(downBox.X + 20, downBox.Y + 50), downColor);
            }

        }

        public void drawReloadBar(Player player, SpriteBatch spriteBatch)
        {
            Rectangle barBack = new Rectangle(player.HitBox.X, player.HitBox.Y - 40, player.CurrentGun.ReloadTime, 30);
            barBack.X -= (barBack.Right - player.HitBox.Right) / 2;
            Rectangle bar = new Rectangle(barBack.X + 1, barBack.Y + 3, 0, 24);

            spriteBatch.Draw(textures[0], barBack, Color.White);

            bar.Width = -(player.ReloadTick - player.CurrentGun.ReloadTime) - 2;

            if (!player.Reloading)
            {
                bar.Width = 0;
            }
            spriteBatch.Draw(textures[11], bar, Color.White);
            
        }

        public void drawSideUI(SpriteBatch spriteBatch, Player player)
        {
            int offSetY = 0, offSetX = 0;
            for (int i = 0; i < player.CurrentGun.MagCapacity; i++)
            {
                Color tint = Color.White;
                if (i > player.CurrentGunAmmo-1)
                {
                    tint = Color.Black;
                }
                if (i == 50)
                {
                    offSetY = 28;
                    offSetX = 50;
                }
                spriteBatch.Draw(bulletTextures[0], new Rectangle(20 + ((i - offSetX) * (18)), 125 + (offSetY), 15, 15), tint);
            }
            string textAdd = "";
            if (player.CurrentGunAmmo == 0)
            {
                textAdd = "   -   Hit R to reload!";
            }
            if (player.Reloading)
            {
                textAdd = "   -   Reloading...";
            }
            spriteBatch.DrawString(bigFont, "" + player.CurrentGunAmmo + " / " + player.Ammo[player.CurrentGun.AmmoType] + textAdd, new Vector2(20, 175), Color.Black);
            Color texttint = Color.Black;
            if (player.CurrentGun == player.PrimaryGun)
            {
                texttint = Color.Red;
            }
            spriteBatch.DrawString(font, "[1]   -   Primary Gun", new Vector2(20, 220), texttint);
            drawGun(player.PrimaryGun, new Rectangle(20, 275, 0, 0), spriteBatch, player);
            texttint = Color.Black;
            if (player.CurrentGun == player.SecondaryGun)
            {
                texttint = Color.Red;
            }
            spriteBatch.DrawString(font, "[2]   -   Secondary Gun", new Vector2(20, 275), texttint);
            if (player.SecondaryGun != null)
            {
                drawGun(player.SecondaryGun, new Rectangle(20, 325, 200, 100), spriteBatch, player);
            }
            else
            {
                spriteBatch.DrawString(font, "     [None]", new Vector2(20, 300), Color.Black);
            }

            spriteBatch.DrawString(font, "[3]   -   Bandages - [" + inventoryOverlay.getItemCount(3) + "]", new Vector2(20, 350), Color.Black);

            spriteBatch.DrawString(font, "[4]   -   First Aid - [" + inventoryOverlay.getItemCount(4) + "]", new Vector2(20, 375), Color.Black);
        }

        private void drawGun(Gun gun, Rectangle rec, SpriteBatch spriteBatch, Player player)
        {
            string gunName = "";
            
            gunName = "     Ammo: " + player.Ammo[gun.AmmoType];

            spriteBatch.DrawString(font, gunName, new Vector2(rec.X, rec.Y-30), Color.Black);
        }

        private void drawStaminaBar(Player item, SpriteBatch spriteBatch)
        {
            Color color = Color.White;
            if (item.CurrentStamina < item.TotalStamina)
            {
                if (staminaFade < 50)
                {
                    color = Color.Transparent;
                }
                staminaFade += 15;
                if (staminaFade >= 255)
                {
                    staminaFade = 255;
                }
            }
            else
            {
                if (staminaFade < 150)
                {
                    staminaFade -= 5;
                    color = Color.Transparent;
                }
                else
                {
                    staminaFade -= 15;
                }
            }
            byte fade = (byte)MathHelper.Clamp(staminaFade, 0, 255);

            spriteBatch.Draw(textures[0], staminaBackgroundRec, new Color(color, fade));
            staminaRec.Width = (300 - (item.TotalStamina - item.CurrentStamina));
            spriteBatch.Draw(textures[1], staminaRec, new Color(color, fade));
        }

        private void drawHealthBar(Player item, SpriteBatch spriteBatch)
        {
            Color color = Color.White;
            if (item.Health < 100 && !healthShow)
            {
                if (healthFade < 50)
                {
                    color = Color.Transparent;
                }
                healthFade += 15;
                if (healthFade >= 255)
                {
                    healthFade = 255;
                    healthShow = true;
                }
            }
            else if (item.Health == 100 && healthShow)
            {
                if (healthFade < 150)
                {
                    healthFade -= 5;
                    color = Color.Transparent;
                    if (healthFade <= 0)
                    {
                        healthFade = 0;
                        healthShow = false;
                    }
                }
                else
                {
                    healthFade -= 15;
                }
            }

            byte fade = (byte)MathHelper.Clamp(healthFade, 0, 255);

            spriteBatch.Draw(textures[0], healthRecBackground, new Color(color, fade));
            healthRec.Width = (500 - ((100 - item.Health) * 5)) - 10;
            spriteBatch.Draw(textures[2], healthRec, new Color(color, fade));
        }

        public bool SingleMouseKeyPress(MouseState currentMouseState)
        {
            bool returning;

            if (prevMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                returning = true;
            }
            else returning = false;
            
            prevMouseState = currentMouseState;
            return returning;
        }

        public bool SingleKeyPress(KeyboardState key)
        {
            bool returning;


            if (prevKey.IsKeyUp(Keys.Space) && key.IsKeyDown(Keys.Space))
            {
                returning = true;
            }
            else returning = false;

            prevKey = key;
            return returning;
        }
    }
}
