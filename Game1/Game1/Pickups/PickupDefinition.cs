using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Pickups
{
    public class PickupDefinition
    {
        public string Name { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public PickupDefinition(string name, Dictionary<string, string> kvp)
        {
            Name = name;
            Values = kvp;
        }
    }
}
