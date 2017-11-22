using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame
{
    class Wave
    {
        ItemGeneration iGen = new ItemGeneration();
        Random rng = new Random();
        public int WaveLevel { get; set; }

        public Wave()
        {
            WaveLevel = 0;
        }

        public List<Crate> spawnCrates(int minX, int minY, int maxX, int maxY)
        {
            int crateNum = (WaveLevel * 10) + 20;
            if (crateNum > 400)
                crateNum = 400;
            return iGen.generateItems(crateNum, minX, minY, maxX, maxY);
        }

        public List<Zombie> spawnZombies(Player[] player, World world, List<Bullet> bulletList, List<Crate> crateList, List<Barricade> barList)
        {
            int minX = world.WorldRec[1, 1].X;
            int minY = world.WorldRec[1, 1].Y;
            int maxX = world.WorldRec[world.WorldSize - 2, world.WorldSize - 2].X;
            int maxY = world.WorldRec[world.WorldSize - 2, world.WorldSize - 2].Y;

            List<Zombie> zombieList = new List<Zombie>();
            int numOfZombies = rng.Next(WaveLevel * 10, (WaveLevel * 30) + 1);
            if (numOfZombies > 125) numOfZombies = 125;
            int sideSpawn;
            int xZomb = 0;
            int yZomb = 0;
            
            for (int i = 0; i < numOfZombies; i++)
            {
                sideSpawn = rng.Next(1, 5);
                switch (sideSpawn)
                {
                    //left
                    case (1):
                        xZomb = rng.Next(minX, minX + 300);
                        yZomb = rng.Next(minY, maxY - 300);
                        break;
                    //right
                    case (2):
                        xZomb = rng.Next(maxX - 600, maxX - 300);
                        yZomb = rng.Next(minY, maxY - 300);
                        break;
                    //up
                    case (3):
                        xZomb = rng.Next(minX, maxX - 300);
                        yZomb = rng.Next(minY, minY + 300);
                        break;
                    //down
                    case (4):
                        xZomb = rng.Next(minX, maxX - 300);
                        yZomb = rng.Next(minY - 600, minY - 300);
                        break;
                    default:
                        break;
                }
                switch (i % 10)
                {
                    case 1:
                        Sprinter runner = new Sprinter(player, xZomb, yZomb, 20, world, bulletList, crateList, barList);
                        runner.Name = "Zombie " + i;

                        xZomb += (runner.Pos.Width + rng.Next(-50, 50));
                        yZomb += (runner.Pos.Height + rng.Next(-50, 50));
                        runner.Health = 50 + (WaveLevel * 2);
                        runner.Damage = 2;
                        zombieList.Add(runner);
                        break;

                    case 2:
                        BigBoi big = new BigBoi(player, xZomb, yZomb, 120, world, bulletList, crateList, barList);
                        big.Name = "Zombie " + i;

                        xZomb += (big.Pos.Width + rng.Next(-50, 50));
                        yZomb += (big.Pos.Height + rng.Next(-50, 50));

                        big.Health = 400 + (WaveLevel * 5);
                        big.Damage = 15;
                        zombieList.Add(big);
                        break;

                    default:
                        Zombie zombie = new Zombie(player, xZomb, yZomb, 60, world, bulletList, crateList, barList);
                        zombie.Name = "Zombie " + i;

                        xZomb += (zombie.Pos.Width + rng.Next(-50, 50));
                        yZomb += (zombie.Pos.Height + rng.Next(-50, 50));

                        zombie.Health = 100 + (WaveLevel * 2);
                        zombie.Damage = 5;
                        zombieList.Add(zombie);
                        break;
                }
            }
            return zombieList;
        }
    }
}
