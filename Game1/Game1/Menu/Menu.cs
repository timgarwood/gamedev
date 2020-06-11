using System;
using Game1.Fonts;
using Box2DX.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Game1.Menu
{
    public class Menu : Drawable
    {
        private MenuDefinition MenuDefinition { get; set; }

        private IList<MenuItem> MenuItems { get; set; }

        private int SelectedIndex { get; set; }

        private FilteredKeyListener KeyListener { get; set; }

        private Texture2D SelectTexture { get; set; }

        public Menu(MenuDefinition menuDefinition, 
            IList<MenuItem> menuItems, 
            FilteredKeyListener keyListener,
            Texture2D selectTexture) :
            base(null)
        {
            MenuDefinition = menuDefinition;
            MenuItems = menuItems;
            SelectedIndex = 0;
            KeyListener = keyListener;
            SelectTexture = selectTexture;
        }

        public override Vec2 GetWorldPosition()
        {
            throw new NotImplementedException();
        }

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            var maxWidth = MenuItems.Select(m => m.Texture.Width).Max();
            var totalHeight = MenuItems.Select(m => m.Texture.Height).Sum() + 
                ((MenuItems.Count() - 1) * MenuDefinition.SpaceBetweenMenuItems);
            var startY = ((viewport.Y / 2) - (totalHeight / 2));
            for(var i = 0; i < MenuItems.Count(); ++i)
            {
                var item = MenuItems[i];
                // align x to the center of the widest menu item
                var startX = (((viewport.X / 2) - (maxWidth / 2)) + 
                    ((maxWidth / 2) - (item.Texture.Width / 2)));
                var origin = new Vector2(startX, startY);
                item.Draw(spriteBatch, origin);
                startY += item.Texture.Height + MenuDefinition.SpaceBetweenMenuItems;
            }

            var selectedItem = MenuItems[SelectedIndex];

            var position = selectedItem.Position;
            position.X -= (SelectTexture.Width + 100);

            spriteBatch.Draw(SelectTexture, position);
        }

        public MenuResult Update(GameTime gameTime)
        {
            KeyListener.Update(gameTime);

            if(KeyListener.WasKeyPressed(Keys.W) || 
                KeyListener.WasKeyPressed(Keys.Up))
            {
                SelectedIndex--;
                if(SelectedIndex < 0)
                {
                    SelectedIndex = MenuItems.Count() - 1;
                }
                
                KeyListener.ResetKey(Keys.W);
                KeyListener.ResetKey(Keys.Up);
            }
            else if(KeyListener.WasKeyPressed(Keys.S) || 
                KeyListener.WasKeyPressed(Keys.Down))
            {
                SelectedIndex++;
                if(SelectedIndex >= MenuItems.Count())
                {
                    SelectedIndex = 0;
                }
                
                KeyListener.ResetKey(Keys.S);
                KeyListener.ResetKey(Keys.Down);
            }
            else if(KeyListener.WasKeyPressed(Keys.Enter))
            {
                KeyListener.ResetKey(Keys.Enter);

                var menuItem = MenuItems[SelectedIndex];
                if(menuItem.Definition.SelectAction == MenuAction.Navigate)
                {
                    var result = new MenuResult(menuItem.Definition.SelectAction, menuItem.Definition.MenuName);
                    return result;
                }
                else if(menuItem.Definition.SelectAction == MenuAction.NewGame ||
                    menuItem.Definition.SelectAction == MenuAction.QuitGame)
                {
                    var result = new MenuResult(menuItem.Definition.SelectAction);
                    return result;
                }

            }

            return null;
        }
    }
}
