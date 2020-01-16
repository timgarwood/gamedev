using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game1.Menu
{
    public class MenuItem
    {
        public MenuItemDefinition Definition { get; set; }

        public Texture2D Texture { get; set; }

        public void Draw(SpriteBatch spriteBatch, Vector2 origin)
        {
            spriteBatch.Draw(Texture, origin);
        }
    }
}
