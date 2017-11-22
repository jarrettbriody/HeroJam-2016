using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame
{
    class World
    {
        const int WORLD_SIZE = 50;
        public int WorldSize { get { return WORLD_SIZE; } }
        Rectangle[,] worldRec = new Rectangle[WORLD_SIZE, WORLD_SIZE];
        int[,] world = new int[WORLD_SIZE, WORLD_SIZE];

        public Rectangle[,] WorldRec { get { return worldRec; } }
        public int[,] WorldMap { get { return world; } }
        Random ran = new Random();

        public void createWorld()
        {
            int finalSize = WORLD_SIZE * 500;
            for (int i = 0; i < WORLD_SIZE; i++)
            {
                for (int j = 0; j < WORLD_SIZE; j++)
                {
                    //TODO update this to make an actual world
                    int random = ran.Next(10);
                    if (random > 7)
                    {
                        world[i, j] = 9;
                    }
                    else
                    {
                        world[i, j] = 0;
                    }

                    if (i == 0)
                    {
                       world[i, j] = 1;
                    }
                    if (i == WORLD_SIZE-1)
                    {
                        world[i, j] = 2;
                    }
                    if (j == 0)
                    {
                        world[i, j] = 3;
                    }
                    if (j == WORLD_SIZE-1)
                    {
                        world[i, j] = 4;
                    }
                    if (j == 0 && i == 0)
                    {
                        world[i, j] = 5;
                    }
                    if (j == 0 && i == WORLD_SIZE - 1)
                    {
                        world[i, j] = 6;
                    }
                    if (j == WORLD_SIZE - 1 && i == 0)
                    {
                        world[i, j] = 7;
                    }
                    if (j == WORLD_SIZE - 1 && i == WORLD_SIZE - 1)
                    {
                        world[i, j] = 8;
                    }
                    worldRec[i, j] = new Rectangle((j * 500) - (finalSize/2), (i * 500) - (finalSize/2), 500, 500);
                }
            }




        }
        
        public void move(int directionX, int directionY, List<Zombie> zombieList, List<Bullet> bulletList, Rectangle playerHitBox, List<Crate> crateList, List<Gun> gunList, List<Barricade> barricadeList)
        {
            //check for allowing of movement

            for (int i = 0; i < WORLD_SIZE; i++)
            {
                for (int j = 0; j < WORLD_SIZE; j++)
                {
                    //hit detection for top
                    if (world[i, j] == 1)
                    {
                        if (playerHitBox.Top <= worldRec[i, j].Bottom)
                        {
                            do
                            {
                                moveEverything(0, -1, zombieList, bulletList, crateList, gunList, barricadeList);
                            } while (playerHitBox.Top < worldRec[i, j].Bottom);
                            if (directionY < 0)
                            {
                                directionY = 0;
                            }
                        }
                    }
                    //hit detection for bottom
                    else if (world[i, j] == 2)
                    {
                        if (playerHitBox.Bottom >= worldRec[i, j].Top)
                        {
                            do
                            {
                                moveEverything(0, 1, zombieList, bulletList, crateList, gunList, barricadeList);
                            } while (playerHitBox.Bottom > worldRec[i, j].Top);
                            if (directionY > 0)
                            {
                                directionY = 0;
                            }
                        }
                    }
                    //hit detection for left side
                    else if (world[i, j] == 3)
                    {
                        if (playerHitBox.Left <= worldRec[i, j].Right)
                        {
                            do
                            {
                                moveEverything(-1, 0, zombieList, bulletList, crateList, gunList, barricadeList);
                            } while (playerHitBox.Left < worldRec[i, j].Right);
                            if (directionX < 0)
                            {
                                directionX = 0;
                            }
                        }
                    }
                    //hit detection for right side
                    else if (world[i, j] == 4)
                    {
                        if (playerHitBox.Right >= worldRec[i, j].Left)
                        {
                            do
                            {
                                moveEverything(1, 0, zombieList, bulletList, crateList, gunList, barricadeList);
                            } while (playerHitBox.Right > worldRec[i, j].Left);
                            if (directionX > 0)
                            {
                                directionX = 0;
                            }
                        }
                    }
                }
            }

            moveEverything(directionX, directionY, zombieList, bulletList, crateList, gunList, barricadeList);

        }

        public void moveEverything(int directionX, int directionY, List<Zombie> zombieList, List<Bullet> bulletList, List<Crate> crateList, List<Gun> gunList, List<Barricade> barricadeList)
        {
            // move everything
            for (int i = 0; i < WORLD_SIZE; i++)
            {
                for (int j = 0; j < WORLD_SIZE; j++)
                {
                    worldRec[i, j].X += directionX;
                    worldRec[i, j].Y += directionY;
                }
            }
            foreach (var item in zombieList)
            {
                int xPos = item.Pos.X + directionX;
                int yPos = item.Pos.Y + directionY;
                item.Pos = new Rectangle(xPos, yPos, item.Pos.Width, item.Pos.Height);
                item.HitBox = new Rectangle(xPos - 50, yPos - 50, item.Pos.Width, item.Pos.Height);
            }

            foreach (var item in bulletList)
            {
                int xPos = item.HitBox.X + directionX;
                int yPos = item.HitBox.Y + directionY;
                item.HitBox = new Rectangle(xPos, yPos, item.HitBox.Width, item.HitBox.Height);
            }

            foreach (var item in crateList)
            {
                int xPos = item.HitBox.X + directionX;
                int yPos = item.HitBox.Y + directionY;
                item.HitBox = new Rectangle(xPos, yPos, item.HitBox.Width, item.HitBox.Height);
            }

            foreach (var item in gunList)
            {
                int xPos = item.HitBox.X + directionX;
                int yPos = item.HitBox.Y + directionY;
                item.HitBox = new Rectangle(xPos, yPos, item.HitBox.Width, item.HitBox.Height);
            }

            foreach (var item in barricadeList)
            {
                int xPos = item.HitBox.X + directionX;
                int yPos = item.HitBox.Y + directionY;
                item.HitBox = new Rectangle(xPos, yPos, item.HitBox.Width, item.HitBox.Height);
            }
        }

        public void drawWorld(SpriteBatch spriteBatch, Texture2D[] mapTextures)
        {
            for (int i = 0; i < WORLD_SIZE; i++)
            {
                for (int j = 0; j < WORLD_SIZE; j++)
                {
                    spriteBatch.Draw(mapTextures[world[i, j]], worldRec[i, j], Color.White);
                }
            }
        }

        public bool worldCollide(Rectangle rec)
        {
            bool collide = false;
            if (rec.Intersects(worldRec[0,0]))
            {
                collide = true;
            }

            return collide;
        }
    }
}
