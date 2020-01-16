using System;
using Game1.Fonts;
using Box2DX.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Game1.Menu
{
    public class Menu : Drawable
    {
        private MenuDefinition MenuDefinition { get; set; }

        private IList<MenuItem> MenuItems { get; set; }

        public Menu(MenuDefinition menuDefinition, IList<MenuItem> menuItems) :
            base(null)
        {
            MenuDefinition = menuDefinition;
            MenuItems = menuItems;
        }

        public override Vec2 GetWorldPosition()
        {
            throw new NotImplementedException();
        }

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            var maxWidth = MenuItems.Select(m => m.Texture.Width).Max();
            var totalHeight = MenuItems.Select(m => m.Texture.Height).Sum() + 
                (MenuItems.Count() - 1 * MenuDefinition.SpaceBetweenMenuItems);
            var startY = ((viewport.Y / 2) - (totalHeight / 2));
            foreach (var item in MenuItems)
            {
                // align x to the center of the widest menu item
                var startX = (((viewport.X / 2) - (maxWidth / 2)) + 
                    ((maxWidth / 2) - (item.Texture.Width / 2)));
                var origin = new Vector2(startX, startY);
                item.Draw(spriteBatch, origin);
                startY += item.Texture.Height + MenuDefinition.SpaceBetweenMenuItems;
            }
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
