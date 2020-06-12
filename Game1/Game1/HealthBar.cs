using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game1
{
    public class HealthBar : IDisposable
    {
        private Texture2D Texture { get; set; }

        private Dictionary<int, Color[]> HealthBars { get; set; }
        public HealthBar(GraphicsDevice graphicsDevice, Texture2D texture, Dictionary<int, Color[]> healthBars)
        {
            Texture = texture;
            HealthBars = healthBars;
            Texture.SetData(HealthBars[100]);
        }

        public void Dispose()
        {
            Texture.Dispose();
            Texture = null;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, int hp)
        {
            var pct = hp - (hp % 10);
            if(pct < 0)
            {
                pct = 0;
            }
            Texture.SetData(HealthBars[pct]);
            spriteBatch.Draw(Texture, position);
        }
    }
}
