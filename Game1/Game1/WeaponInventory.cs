using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Pickups;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class WeaponInventory
    {
        public class WeaponInfo 
        {
            public int RemainingAmmo { get; set; }

            public string Name { get; set; }

            public Texture2D Texture { get; set; }
            
            public string ProjectileName { get; set; }
        }

        private List<WeaponInfo> Lasers { get; set; }

        private int LaserIndex { get; set; } = 0;

        public WeaponInventory()
        {
            Lasers = new List<WeaponInfo>();
        }

        public void AddToInventory(Laser laser)
        {
            var existing = Lasers.FirstOrDefault(l => l.Name.ToLower().Equals(laser.Name.ToLower()));
            if(existing != null)
            {
                existing.RemainingAmmo += laser.StartingAmmo;
                LaserIndex = Lasers.IndexOf(existing);
            }
            else
            {
                Lasers.Add(new WeaponInfo
                {
                    Texture = laser.Texture,
                    Name = laser.Name,
                    RemainingAmmo = laser.StartingAmmo,
                    ProjectileName = laser.ProjectileName
                });

                // make this the current laser
                LaserIndex = Lasers.Count - 1;
            }
        }

        private void SelectWeaponWithAmmo()
        {
            var weapon = Lasers.FirstOrDefault(l => l.RemainingAmmo > 0);
            if(weapon != null)
            {
                LaserIndex = Lasers.IndexOf(weapon);
            }
        }

        public void DecreaseAmmo(int amt)
        {
            if (Lasers.Count <= 0) return;

            var info = Lasers[LaserIndex];

            info.RemainingAmmo -= amt;

            if(info.RemainingAmmo <= 0)
            {
                info.RemainingAmmo = 0;
                SelectWeaponWithAmmo();
            }
        }

        public void SelectNextWeapon()
        {
            if (Lasers.Count <= 0) return;

            LaserIndex++;
            if(LaserIndex >= Lasers.Count)
            {
                LaserIndex = 0;
            }
        }

        public void SelectPreviousWeapon()
        {
            if (Lasers.Count <= 0) return;

            LaserIndex--;
            if(LaserIndex < 0)
            {
                LaserIndex = Lasers.Count - 1;
            }
        }

        public WeaponInfo GetSelectedWeapon()
        {
            if (Lasers.Count <= 0) return null;

            return Lasers[LaserIndex];
        }
    }
}
