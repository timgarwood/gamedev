using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Game1.Fonts;

namespace Game1.Menu
{
    public class MenuFactory
    {
        private MenuDefinition[] _menuDefinitions;
        private FontFactory _fontFactory;

        public MenuFactory(FontFactory fontFactory)
        {
            _fontFactory = fontFactory;
        }

        public void Load(Stream stream)
        {
            using(stream)
            {
                using(var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    _menuDefinitions = JsonConvert.DeserializeObject<MenuDefinition[]>(json);
                }
            }
        }

        public Menu Create(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var menuDefinition = _menuDefinitions.FirstOrDefault(m => m.Name.ToLower().Equals(name.ToLower()));
            var font = _fontFactory.GetFont(menuDefinition.FontName);
            return new Menu(menuDefinition, font);
        }
    }
}
