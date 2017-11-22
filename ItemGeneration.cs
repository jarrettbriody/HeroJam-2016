using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame
{
    class ItemGeneration
    {
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        Random rng = new Random();
        Dictionary<string, int[]> rarity = new Dictionary<string, int[]>();
        Items _items = new Items();
        public ItemGeneration()
        {
            rarity.Add("gun_AssaultRifle", new int[] { 1, 3 });
            rarity.Add("gun_SniperRifle", new int[] { 4, 6 });
            rarity.Add("gun_MachineGun", new int[] { 7, 7 });
            rarity.Add("gun_Pistol", new int[] { 8, 12 });
            rarity.Add("gun_Shotgun", new int[] { 13, 15 });
            rarity.Add("ammo_AssaultRifleAmmo", new int[] { 16, 20 });
            rarity.Add("ammo_SniperRifleAmmo", new int[] { 21, 25 });
            rarity.Add("ammo_MachineGunAmmo", new int[] { 26, 30 });
            rarity.Add("ammo_PistolAmmo", new int[] { 31, 40 });
            rarity.Add("ammo_ShotgunAmmo", new int[] {41, 50 });
            rarity.Add("item_Wood", new int[] { 51, 70 });
            rarity.Add("item_Stone", new int[] { 71, 80 });
            rarity.Add("item_Metal", new int[] { 81, 85 });
            rarity.Add("item_Medkit", new int[] { 86, 89 });
            rarity.Add("item_Bandage", new int[] { 90, 99 });
        }
        public List<Crate> generateItems(int amountOfCrates, int minX, int minY, int maxX, int maxY)
        {
            List<Crate> spawnedItems = new List<Crate>();
            string item = null;
            for (int i = 0; i < amountOfCrates; i++)
            {
                int num = rng.Next(1, 100);
                foreach (var entry in rarity)
                {
                    if(num >= rarity[entry.Key][0] && num <= rarity[entry.Key][1])
                    {
                        item = entry.Key;
                        break;
                    }
                }
                if(item.Contains("gun_")) spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Weapon = _items.GetGun(item) });
                else if (item.Contains("ammo_") || item.Contains("item_"))
                {
                    switch (item)
                    {
                        case "ammo_AssaultRifleAmmo":
                            spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Item = _items.GetItem(item), Amount = rng.Next(10, 51) });
                            break;
                        case "ammo_SniperRifleAmmo":
                            spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Item = _items.GetItem(item), Amount = rng.Next(5, 16)});
                            break;
                        case "ammo_MachineGunAmmo":
                            spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Item = _items.GetItem(item), Amount = rng.Next(50, 200)});
                            break;
                        case "ammo_PistolAmmo":
                            spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Item = _items.GetItem(item), Amount = rng.Next(10, 31)});
                            break;
                        case "ammo_ShotgunAmmo":
                            spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Item = _items.GetItem(item), Amount = rng.Next(5, 21)});
                            break;
                        case "item_Wood":
                            spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Item = _items.GetItem(item), Amount = rng.Next(5, 21)});
                            break;
                        case "item_Stone":
                            spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Item = _items.GetItem(item), Amount = rng.Next(3, 10)});
                            break;
                        case "item_Metal":
                            spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Item = _items.GetItem(item), Amount = rng.Next(2, 6)});
                            break;
                        case "item_Bandage":
                            spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Item = _items.GetItem(item), Amount = rng.Next(1, 4)});
                            break;
                        case "item_Medkit":
                            spawnedItems.Add(new Crate(rng.Next(minX + 60, maxX - 60), rng.Next(minY + 60, maxY - 60)) { Item = _items.GetItem(item), Amount = rng.Next(1, 3)});
                            break;
                        default:
                            break;
                    }
                }
            }
            return spawnedItems;
        }
    }
}
