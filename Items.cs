using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame
{
    class Items
    {
        List<Gun> guns = new List<Gun>();
        List<String> items = new List<String>();

        Gun assaultRifle = new Gun() { Name = "gun_AssaultRifle", BulletSpeed = 7, Damage = 30, FireRate = (1.0 /(500.0 / 3600.0)), FullAuto = true, MagCapacity = 30, Penetration = 1, Range = 2000, Spray = 1, ReloadTime = 60, AmmoType = 2, Accuracy = 2 };
        Gun sniperRifle = new Gun() { Name = "gun_SniperRifle", BulletSpeed = 10, Damage = 100, FireRate = (1.0 / (60.0 / 3600.0)), FullAuto = false, MagCapacity = 10, Penetration = 3, Range = 2000, Spray = 1, ReloadTime = 120, AmmoType = 3, Accuracy = 50 };
        Gun machineGun = new Gun() { Name = "gun_MachineGun", BulletSpeed = 8, Damage = 40, FireRate = (1.0 / (725.0 / 3600.0)), FullAuto = true, MagCapacity = 100, Penetration = 2, Range = 2000, Spray = 1, ReloadTime = 300, AmmoType = 4, Accuracy = 0.9f };
        Gun pistol = new Gun() { Name = "gun_Pistol", BulletSpeed = 6, Damage = 40, FireRate = (1.0 / (150.0 / 3600.0)), FullAuto = false, MagCapacity = 17, Penetration = 1, Range = 2000, Spray = 1, ReloadTime = 30, AmmoType = 0, Accuracy = 1f };
        Gun shotgun = new Gun() { Name = "gun_Shotgun", BulletSpeed = 7, Damage = 40, FireRate = (1.0 / (60.0 / 3600.0)), FullAuto = false, MagCapacity = 8, Penetration = 1, Range = 2000, Spray = 5, ReloadTime = 180, AmmoType = 1, Accuracy = 0.8f };

        string assaultRifleAmmo = "ammo_AssaultRifleAmmo";
        string sniperRifleAmmo = "ammo_SniperRifleAmmo";
        string machineGunAmmo = "ammo_MachineGunAmmo";
        string pistolAmmo = "ammo_PistolAmmo";
        string shotgunAmmo = "ammo_ShotgunAmmo";

        string wood = "item_Wood";
        string stones = "item_Stone";
        string metal = "item_Metal";
        string medkit = "item_FirstAid";
        string bandage = "item_Bandage";
        
        public Items()
        {
            guns.Add(assaultRifle);
            guns.Add(sniperRifle);
            guns.Add(machineGun);
            guns.Add(pistol);
            guns.Add(shotgun);
            items.Add(assaultRifleAmmo);
            items.Add(sniperRifleAmmo);
            items.Add(machineGunAmmo);
            items.Add(pistolAmmo);
            items.Add(shotgunAmmo);
            items.Add(wood);
            items.Add(stones);
            items.Add(metal);
            items.Add(medkit);
            items.Add(bandage);
        }

        public Gun GetGun(string name)
        {
            Gun newGun = new Gun();
            foreach (var entry in guns)
            {
                if (entry.Name == name)
                {
                    newGun = entry;
                    return newGun;
                }
            }
            return null;
        }

        public string GetItem(string name)
        {
            foreach (var entry in items)
            {
                if (entry == name) return entry;
            }
            return null;
        }
        
    }
}
