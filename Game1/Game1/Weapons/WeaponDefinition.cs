using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Weapons
{
    public class WeaponDefinition
    {
        public string Name { get; set; }

        public string SpriteSheet { get; set; }

        public int XCoordinate { get; set; }

        public int YCoordinate { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public float Velocity { get; set; }
        
        public float Density { get; set; }

        public float Friction { get; set; }

        public int Damage { get; set; }
    }
}
