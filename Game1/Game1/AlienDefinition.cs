using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class AlienDefinition
    {
        public AlienDefinition()
        {
        }

        public string Name { get; set; }

        public string TextureName { get; set; }

        public int Hp { get; set; }

        public float Scale { get; set; }

        public float Friction { get; set; }

        public float Density { get; set; }
    }
}
