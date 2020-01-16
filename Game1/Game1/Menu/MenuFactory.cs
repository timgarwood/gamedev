using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Game1.Fonts;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Menu
{
    public class MenuFactory
    {
        private MenuDefinition[] _menuDefinitions;

        private ContentManager ContentManager { get; set; }

        public MenuFactory(ContentManager contentManager)
        {
            ContentManager = contentManager;
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
            var menuItems = new List<MenuItem>();
            foreach(var itemDefinition in menuDefinition.MenuItems)
            {
                menuItems.Add(new MenuItem
                {
                    Definition = itemDefinition,
                    Texture = ContentManager.Load<Texture2D>(itemDefinition.TextureName)
                });
            }
            return new Menu(menuDefinition, menuItems);
        }
    }
}
