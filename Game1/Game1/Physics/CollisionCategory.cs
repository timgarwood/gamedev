using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Physics
{
    public struct CollisionCategory
    {
        public static ushort Player { get; private set; } = 0x0001;
        public static ushort Alien { get; private set; } = 0x0002;
        public static ushort PlayerProjectile { get; private set; } = 0x0004;
        public static ushort AlienProjectile { get; private set; } = 0x0008;
        public static ushort Pickup { get; private set; } = 0x0010;
        public static ushort Wall { get; private set; } = 0x0020;
    }
}
