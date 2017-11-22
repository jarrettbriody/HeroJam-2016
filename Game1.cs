using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ZombieGame
{
    enum GAMESTATE
    {
        Menu, Options, Game
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //class calls
        GameLoop gameLoop = new GameLoop();
        InventoryOverlay invOverlay = new InventoryOverlay();

        //texture loads
        Texture2D[] playerTextures, bulletTextures, mapTextures, zombieTextures, textures;
        Texture2D pointer;
        //variables
        bool stopKey = false;
        KeyboardState prevKey;
        GAMESTATE gs;
        //fonts
        SpriteFont typewriter, bigType, menuFont;
        
        SoundManager soundManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1600;
        }
        
        protected override void Initialize()
        {
            prevKey = Keyboard.GetState();
            gs = 0;
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //player texture loading
            playerTextures = new Texture2D[36];
            //handgun
            playerTextures[0] = Content.Load<Texture2D>("Player/Pistol/survivor-move_handgun_0");
            playerTextures[1] = Content.Load<Texture2D>("Player/Pistol/survivor-move_handgun_1");
            playerTextures[2] = Content.Load<Texture2D>("Player/Pistol/survivor-move_handgun_2");
            playerTextures[3] = Content.Load<Texture2D>("Player/Pistol/survivor-move_handgun_3");
            playerTextures[4] = Content.Load<Texture2D>("Player/Pistol/survivor-move_handgun_4");
            playerTextures[5] = Content.Load<Texture2D>("Player/Pistol/survivor-move_handgun_5");
            //shotgun
            playerTextures[6] = Content.Load<Texture2D>("Player/Shotgun/Move/survivor-move_shotgun_0");
            playerTextures[7] = Content.Load<Texture2D>("Player/Shotgun/Move/survivor-move_shotgun_1");
            playerTextures[8] = Content.Load<Texture2D>("Player/Shotgun/Move/survivor-move_shotgun_2");
            playerTextures[9] = Content.Load<Texture2D>("Player/Shotgun/Move/survivor-move_shotgun_3");
            playerTextures[10] = Content.Load<Texture2D>("Player/Shotgun/Move/survivor-move_shotgun_4");
            playerTextures[11] = Content.Load<Texture2D>("Player/Shotgun/Move/survivor-move_shotgun_5");
            //rifle
            playerTextures[12] = Content.Load<Texture2D>("Player/Rifle/survivor-move_rifle_0");
            playerTextures[13] = Content.Load<Texture2D>("Player/Rifle/survivor-move_rifle_1");
            playerTextures[14] = Content.Load<Texture2D>("Player/Rifle/survivor-move_rifle_2");
            playerTextures[15] = Content.Load<Texture2D>("Player/Rifle/survivor-move_rifle_3");
            playerTextures[16] = Content.Load<Texture2D>("Player/Rifle/survivor-move_rifle_4");
            playerTextures[17] = Content.Load<Texture2D>("Player/Rifle/survivor-move_rifle_5");
            //reloading
            //handgun
            playerTextures[18] = Content.Load<Texture2D>("Player/Pistol/survivor-reload_handgun_0");
            playerTextures[19] = Content.Load<Texture2D>("Player/Pistol/survivor-reload_handgun_1");
            playerTextures[20] = Content.Load<Texture2D>("Player/Pistol/survivor-reload_handgun_2");
            playerTextures[21] = Content.Load<Texture2D>("Player/Pistol/survivor-reload_handgun_3");
            playerTextures[22] = Content.Load<Texture2D>("Player/Pistol/survivor-reload_handgun_4");
            playerTextures[23] = Content.Load<Texture2D>("Player/Pistol/survivor-reload_handgun_5");
            //shotgun
            playerTextures[24] = Content.Load<Texture2D>("Player/Shotgun/survivor-reload_shotgun_0");
            playerTextures[25] = Content.Load<Texture2D>("Player/Shotgun/survivor-reload_shotgun_1");
            playerTextures[26] = Content.Load<Texture2D>("Player/Shotgun/survivor-reload_shotgun_2");
            playerTextures[27] = Content.Load<Texture2D>("Player/Shotgun/survivor-reload_shotgun_3");
            playerTextures[28] = Content.Load<Texture2D>("Player/Shotgun/survivor-reload_shotgun_4");
            playerTextures[29] = Content.Load<Texture2D>("Player/Shotgun/survivor-reload_shotgun_5");
            //rifle
            playerTextures[30] = Content.Load<Texture2D>("Player/Rifle/survivor-reload_rifle_0");
            playerTextures[31] = Content.Load<Texture2D>("Player/Rifle/survivor-reload_rifle_1");
            playerTextures[32] = Content.Load<Texture2D>("Player/Rifle/survivor-reload_rifle_2");
            playerTextures[33] = Content.Load<Texture2D>("Player/Rifle/survivor-reload_rifle_3");
            playerTextures[34] = Content.Load<Texture2D>("Player/Rifle/survivor-reload_rifle_4");
            playerTextures[35] = Content.Load<Texture2D>("Player/Rifle/survivor-reload_rifle_5");

            //zombie texture loading
            zombieTextures = new Texture2D[11];
            //moving
            zombieTextures[0] = Content.Load<Texture2D>("Zombie/skeleton-move_0");
            zombieTextures[1] = Content.Load<Texture2D>("Zombie/skeleton-move_1");
            zombieTextures[2] = Content.Load<Texture2D>("Zombie/skeleton-move_2");
            zombieTextures[3] = Content.Load<Texture2D>("Zombie/skeleton-move_3");
            zombieTextures[4] = Content.Load<Texture2D>("Zombie/skeleton-move_4");
            zombieTextures[5] = Content.Load<Texture2D>("Zombie/skeleton-move_5");
            //attacking
            zombieTextures[6] = Content.Load<Texture2D>("Zombie/skeleton-attack_0");
            zombieTextures[7] = Content.Load<Texture2D>("Zombie/skeleton-attack_1");
            zombieTextures[8] = Content.Load<Texture2D>("Zombie/skeleton-attack_2");
            zombieTextures[9] = Content.Load<Texture2D>("Zombie/skeleton-attack_3");
            zombieTextures[10] = Content.Load<Texture2D>("Zombie/skeleton-attack_4");

            //map texture loading
            mapTextures = new Texture2D[10];
            mapTextures[0] = Content.Load<Texture2D>("Tiles/sandy Background");
            mapTextures[1] = Content.Load<Texture2D>("Tiles/Bottom");
            mapTextures[2] = Content.Load<Texture2D>("Tiles/Top");
            mapTextures[3] = Content.Load<Texture2D>("Tiles/Right");
            mapTextures[4] = Content.Load<Texture2D>("Tiles/Left");
            mapTextures[5] = Content.Load<Texture2D>("Tiles/TL corner");
            mapTextures[6] = Content.Load<Texture2D>("Tiles/TR corner");
            mapTextures[7] = Content.Load<Texture2D>("Tiles/BL corner");
            mapTextures[8] = Content.Load<Texture2D>("Tiles/BR corner");
            mapTextures[9] = Content.Load<Texture2D>("Tiles/Grassy one");

            //bullet texture loading
            bulletTextures = new Texture2D[6];
            bulletTextures[0] = Content.Load<Texture2D>("Bullet/BasicBullet");
            bulletTextures[1] = Content.Load<Texture2D>("Muzzle Flash/muzzle_flash_0");
            bulletTextures[2] = Content.Load<Texture2D>("Muzzle Flash/muzzle_flash_1");
            bulletTextures[3] = Content.Load<Texture2D>("Muzzle Flash/muzzle_flash_2");
            bulletTextures[4] = Content.Load<Texture2D>("Muzzle Flash/muzzle_flash_3");
            bulletTextures[5] = Content.Load<Texture2D>("Muzzle Flash/muzzle_flash_4");

            //other textures
            textures = new Texture2D[21];
            textures[0] = Content.Load<Texture2D>("Other/BarBackground");
            textures[1] = Content.Load<Texture2D>("Other/StaminaBar");
            textures[2] = Content.Load<Texture2D>("Other/HealthBar");
            textures[3] = Content.Load<Texture2D>("Other/Resources/wood");
            textures[4] = Content.Load<Texture2D>("Other/Resources/stone");
            textures[5] = Content.Load<Texture2D>("Other/Resources/metal");
            textures[6] = Content.Load<Texture2D>("Other/Resources/bandage");
            textures[7] = Content.Load<Texture2D>("Other/Resources/firstAid");
            textures[8] = Content.Load<Texture2D>("Inventory_Overlay/background");
            textures[9] = Content.Load<Texture2D>("Inventory_Overlay/slot");
            textures[10] = Content.Load<Texture2D>("Other/Crate");
            textures[11] = Content.Load<Texture2D>("Other/ReloadBar");
            textures[12] = Content.Load<Texture2D>("Other/gun-pistol");
            textures[13] = Content.Load<Texture2D>("Other/gun-scar");
            textures[14] = Content.Load<Texture2D>("Other/gun-awp");
            textures[15] = Content.Load<Texture2D>("Other/Machine gun");
            textures[16] = Content.Load<Texture2D>("Other/shotgun");
            textures[17] = Content.Load<Texture2D>("Other/arrow");
            textures[18] = Content.Load<Texture2D>("Other/title screen");
            textures[19] = Content.Load<Texture2D>("Other/Horizontal");
            textures[20] = Content.Load<Texture2D>("Other/Vertical");

            //inventory textures / font
            typewriter = Content.Load<SpriteFont>("Inventory_Overlay/typewriter");
            bigType = Content.Load<SpriteFont>("Inventory_Overlay/bigType");
            menuFont = Content.Load<SpriteFont>("Inventory_Overlay/menuFont");

            pointer = Content.Load<Texture2D>("pointer");

            string[] soundNames =
            {
                "fire_pistol",
                "reload_pistol",
                "fire_shotgun",
                "reload_shotgun",
                "fire_assault",
                "reload_assault",
                "fire_machineGun",
                "reload_machineGun",
                "fire_sniper",
                "click",
                "zombie_1",
                "zombie_2",
                "zombie_3",
                "zombie_4",
                "zombe_5",
                "zombie_6",
                "zombie_7",
                "walking"
            };

            SoundEffect[] sounds = new SoundEffect[18];
            sounds[0] = Content.Load<SoundEffect>("Sounds/pistol_fire");
            sounds[1] = Content.Load<SoundEffect>("Sounds/pistol_reload");
            sounds[2] = Content.Load<SoundEffect>("Sounds/shotgun_fire");
            sounds[3] = Content.Load<SoundEffect>("Sounds/shotgun_reload");
            sounds[4] = Content.Load<SoundEffect>("Sounds/assault_fire");
            sounds[5] = Content.Load<SoundEffect>("Sounds/assault_reload");
            sounds[6] = Content.Load<SoundEffect>("Sounds/machineGun_fire");
            sounds[7] = Content.Load<SoundEffect>("Sounds/machineGun_reload");
            sounds[8] = Content.Load<SoundEffect>("Sounds/sniper_fire");
            sounds[9] = Content.Load<SoundEffect>("Sounds/buttonClick");
            sounds[10] = Content.Load<SoundEffect>("Sounds/Pool_1");
            sounds[11] = Content.Load<SoundEffect>("Sounds/Pool_10");
            sounds[12] = Content.Load<SoundEffect>("Sounds/Pool_3");
            sounds[13] = Content.Load<SoundEffect>("Sounds/Pool_5");
            sounds[14] = Content.Load<SoundEffect>("Sounds/Pool_4");
            sounds[15] = Content.Load<SoundEffect>("Sounds/Pool_8");
            sounds[16] = Content.Load<SoundEffect>("Sounds/Pool_9");
            sounds[17] = Content.Load<SoundEffect>("Sounds/Walking");

            soundManager = new SoundManager(sounds, soundNames);

            invOverlay.StartUp(GraphicsDevice, textures, typewriter);
            gameLoop.StartUp(GraphicsDevice, invOverlay, playerTextures, bulletTextures, mapTextures, zombieTextures, textures, typewriter, bigType, menuFont, soundManager);
        }
        
        protected override void UnloadContent()
        {
        }
        
        protected override void Update(GameTime gameTime)
        {
            KeyboardState key = Keyboard.GetState();

            switch (gs)
            {
                case GAMESTATE.Menu:
                    if (gameLoop.Quit == true)
                        Exit();

                    if (gameLoop.Play == true)
                    {
                        gameLoop.Play = false;
                        gameLoop = new GameLoop();
                        invOverlay = new InventoryOverlay();

                        invOverlay.StartUp(GraphicsDevice, textures, typewriter);
                        gameLoop.StartUp(GraphicsDevice, invOverlay, playerTextures, bulletTextures, mapTextures, zombieTextures, textures, typewriter, bigType, menuFont, soundManager);
                        gs = GAMESTATE.Game;
                    }
                    if(gameLoop.Options == true)
                    {
                        gameLoop.Options = false;
                        gs = GAMESTATE.Options;
                    }

                    break;
                case GAMESTATE.Options:
                    if(gameLoop.Menu == true)
                    {
                        gameLoop.Menu = false;
                        gs = GAMESTATE.Menu;
                    }
                    break;
                case GAMESTATE.Game:
                    if(gameLoop.Menu == true)
                    {
                        gameLoop.Menu = false;
                        gs = GAMESTATE.Menu;
                    }
                    
                    //Pause
                    if (SingleKeyPress(Keyboard.GetState()) == true)
                    {
                        gameLoop.Pause = true;
                        graphics.ApplyChanges();
                    }

                    //make game full-screen
                    if (key.IsKeyDown(Keys.F11) && !stopKey)
                    {
                        stopKey = true;
                        graphics.IsFullScreen = !graphics.IsFullScreen;
                        graphics.ApplyChanges();
                    }
                    if (key.IsKeyUp(Keys.F11))
                    {
                        stopKey = false;
                    }

                    gameLoop.Update();

                    base.Update(gameTime);
                    break;
                default:
                    break;                    
            }            
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            MouseState mouse = Mouse.GetState();            

            switch (gs)
            {
                case GAMESTATE.Menu:
                    GraphicsDevice.Clear(Color.Black);
                    gameLoop.DrawMenu(spriteBatch);                    
                    break;
                case GAMESTATE.Options:
                    GraphicsDevice.Clear(Color.Black);
                    gameLoop.DrawOptions(spriteBatch);
                    break;
                case GAMESTATE.Game:
                    if (gameLoop.Pause == true || gameLoop.Death == true)
                        GraphicsDevice.Clear(Color.Black); 
                    else
                        GraphicsDevice.Clear(Color.DarkGoldenrod);

                    gameLoop.Draw(spriteBatch);
                    base.Draw(gameTime);
                    break;
            }

            spriteBatch.Draw(pointer, new Vector2(mouse.X, mouse.Y), Color.White);
            spriteBatch.End();
        }

        public bool SingleKeyPress(KeyboardState ks)
        {
            bool check;

            if (ks.IsKeyDown(Keys.Escape) && prevKey.IsKeyUp(Keys.Escape))
            {
                check = true;
            }
            else
            {
                check = false;
            }
            prevKey = ks;
            return check;
        }
    }
}
