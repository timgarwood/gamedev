using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.Menu
{
    public class MenuFactory
    {
        private MenuData MenuData { get; set; }

        private ContentManager ContentManager { get; set; }

        private FilteredKeyListener KeyListener { get; set; }

        private Dictionary<string, Menu> Menus { get; set; }

        public MenuFactory(ContentManager contentManager)
        {
            ContentManager = contentManager;


            Menus = new Dictionary<string, Menu>();
        }

        public void Load(Stream stream)
        {
            using(stream)
            {
                using(var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    MenuData = JsonConvert.DeserializeObject<MenuData>(json);
                }
            }

            var menuSelectTexture = ContentManager.Load<Texture2D>(MenuData.SelectTextureName);

            foreach (var menuDefinition in MenuData.MenuDefinitions)
            {
                var menuItems = new List<MenuItem>();

                var messageTextures = new Texture2D[menuDefinition.MessageTextures.Length];

                for(var i = 0; i < messageTextures.Length; ++i)
                {
                    messageTextures[i] = ContentManager.Load<Texture2D>(menuDefinition.MessageTextures[i]);
                }

                foreach (var itemDefinition in menuDefinition.MenuItems)
                {
                    menuItems.Add(new MenuItem
                    {
                        Definition = itemDefinition,
                        Texture = ContentManager.Load<Texture2D>(itemDefinition.TextureName)
                    });

                }

            
                //create a FilteredKeyListener to track the keys for the menus
                var trackKeys = new Keys[] { Keys.W, Keys.S, Keys.Down, Keys.Up, Keys.Enter};
                var keyListener = new FilteredKeyListener(trackKeys);

                var menu = new Menu(menuDefinition, menuItems, keyListener, messageTextures, menuSelectTexture);
                Menus.Add(menuDefinition.Name, menu);
            }
        }

        public Menu Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            if (Menus.ContainsKey(name))
            {
                return Menus[name];
            }

            return null;
        }
    }
}
