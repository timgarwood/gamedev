using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Fonts;
using Box2DX.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Menu
{
    public class Menu : Drawable
    {
        private MenuDefinition _menuDefinition;
        private Font _font;

        public Menu(MenuDefinition menuDefinition, Font font) :
            base(null)
        {
            _menuDefinition = menuDefinition;
            _font = font;
        }

        public override Vec2 GetWorldPosition()
        {
            throw new NotImplementedException();
        }

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            var startX = _menuDefinition.StartX;
            var startY = _menuDefinition.StartY;
            foreach (var item in _menuDefinition.MenuItems)
            {
                _font.DrawString(spriteBatch, item.Text, new Vector2(startX, startY));
                startY += item.Height;
            }
        }
    }
}
