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
    class InventoryOverlay
    {
        Texture2D background;
        Texture2D slot;
        SpriteFont typewriter;
        Rectangle backgroundRectangle;
        Rectangle[] slots, slotPic;
        const int SLOT_SIZE = 100;
        string[] itemNames = { "Wood", "Stone", "Metal", "Bandages", "First Aid" };
        Vector2[] textPos, countTextPos;
        int[] itemCount = { 0, 0, 0, 0, 0 };
        Texture2D[] textures;
        MouseState prevMouseState;

        public bool Building { get; set; }

        string[] item =
        {
            "item_Wood",
            "item_Stone",
            "item_Metal",
            "item_Bandage",
            "item_FirstAid"
        };

        //guns


        public void StartUp(GraphicsDevice graphics, Texture2D[] textures, SpriteFont font)
        {
            background = textures[8];
            slot = textures[9];
            this.textures = textures;
            typewriter = font;
            backgroundRectangle = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
            //set up all the arrays
            slots = new Rectangle[itemNames.Length];
            textPos = new Vector2[itemNames.Length];
            slotPic = new Rectangle[itemNames.Length];
            countTextPos = new Vector2[itemNames.Length];
            prevMouseState = Mouse.GetState();
            Building = false;

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new Rectangle(300, 175 +(SLOT_SIZE * i + 20 * i), SLOT_SIZE, SLOT_SIZE);
                slotPic[i] = new Rectangle(300 + (SLOT_SIZE / 2) - 25, slots[i].Y + (SLOT_SIZE / 2) - 25, 50, 50);
                textPos[i] = new Vector2(slots[i].X + 4, slots[i].Y + 4);
                countTextPos[i] = new Vector2(slots[i].X + 4, slots[i].Bottom - 30);
            }
            
        }
        public void Update()
        {

        }
        public void Draw(SpriteBatch sb, Player player)
        {
            sb.Draw(background, backgroundRectangle, Color.White);
            for (int i = 0; i < slots.Length; i++)
            {
                sb.Draw(slot, slots[i], Color.White);
                sb.Draw(textures[i + 3], slotPic[i], Color.White);
                sb.DrawString(typewriter, itemNames[i], textPos[i], Color.Black);
                sb.DrawString(typewriter, "Count: " + itemCount[i], countTextPos[i], Color.Black);
            }
            
            sb.DrawString(typewriter, "Primary Gun", new Vector2(500, 175), Color.Black);
            drawGun(new Rectangle(500, 220, 300, 200), player.PrimaryGun, sb);
            if (player.SecondaryGun != null)
            {
                sb.DrawString(typewriter, "Secondary Gun", new Vector2(500, 475), Color.Black);
                drawGun(new Rectangle(500, 525, 300, 200), player.SecondaryGun, sb);
            }
            string[] ammoText = { "Pistol", "Shotgun", "Rifle", "Sniper Rifle", "Machine Gun" };

            for (int i = 0; i < player.Ammo.Length; i++)
            {
                sb.DrawString(typewriter, ammoText[i] + " Ammo: " + player.Ammo[i], new Vector2(900, 175 + (50 * i)), Color.Black);
            }

            //add button            
            Color buildColor = Color.White;
            Rectangle buildBox = new Rectangle(900, 600, 250, 200);
            sb.DrawString(typewriter, "Build Barricade \nNeeds 75 Wood, 25 Stone and 5 Metal", new Vector2(buildBox.X, buildBox.Y - 25), Color.Black);
            sb.Draw(textures[19], buildBox, buildColor);

            if (buildBox.Contains(Mouse.GetState().Position))
            {
                if (itemCount[0] >= 75 && itemCount[1] >= 25 && itemCount[2] >= 5)
                {
                    buildColor = Color.Green;
                    sb.Draw(textures[19], buildBox, buildColor);

                    if (SingleMouseKeyPress(Mouse.GetState()))
                    {
                        itemCount[0] -= 75;
                        itemCount[1] -= 25;
                        itemCount[2] -= 5;
                        Building = true;
                        player.InventoryOpen = false;
                    }
                }

                else
                {
                    buildColor = Color.Red;
                    sb.Draw(textures[19], buildBox, buildColor);
                }
            }
        }

        private void drawGun(Rectangle rec, Gun gun, SpriteBatch spriteBatch)
        {
            Texture2D gunTexture;
            string gunName = "";
            if (gun.Name == "gun_AssaultRifle")
            {
                gunTexture = textures[13];
                gunName = "SCAR ( Assault Rifle )";
            }
            else if (gun.Name == "gun_Pistol")
            {
                gunTexture = textures[12];
                gunName = "Colt 45 ( Pistol )";
            }
            else if (gun.Name == "gun_Shotgun")
            {
                gunTexture = textures[16];
                gunName = "Shotgun ( Shotgun )";
            }
            else if (gun.Name == "gun_SniperRifle")
            {
                gunTexture = textures[14];
                gunName = "AWP ( Sniper Rifle )";
            }
            else if (gun.Name == "gun_MachineGun")
            {
                gunTexture = textures[15];
                gunName = "Machine Gun ( Machine Gun)";
            }
            else
            {
                gunTexture = textures[0];
            }
            spriteBatch.DrawString(typewriter, "Gun: " + gunName, new Vector2(rec.X, rec.Y - 10), Color.Black);
            spriteBatch.Draw(gunTexture, rec, Color.White);
        }

        public void AddItem(string itemName, int count)
        {
            for (int i = 0; i < item.Length; i++)
            {
                if (itemName == item[i])
                {
                    itemCount[i] += count;
                    return;
                }
            }
        }

        public int getItemCount(int itemNum)
        {
            return itemCount[itemNum];
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
    }
}
