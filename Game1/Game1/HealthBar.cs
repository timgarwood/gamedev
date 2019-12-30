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
        private static int BarWidthPx { get; set; } = 100;
        private static int BarHeightPx { get; set; } = 5;
        private Texture2D Texture { get; set; }

        private static Dictionary<int, Color[]> HealthBars = new Dictionary<int, Color[]>();

        private static void SetHealthBar(Color[] bar, int pct)
        {
        }

        private static Color GetColorForPct(int pct)
        {
            if(pct > 60)
            {
                return Color.LimeGreen;
            }
            else if(pct > 30)
            {
                return Color.Yellow;
            }

            return Color.Red;
        }

        static HealthBar()
        {
            for(var pct = BarWidthPx; pct >= 0; pct-=10)
            {
                var colors = new Color[BarWidthPx * BarHeightPx];
                //figure out what color to make this percentage
                var color = GetColorForPct(pct);
                for(var row = 0; row < BarHeightPx; ++row)
                {
                    //color up to the percentage of the health bar 
                    var col = 0;
                    for(; col < pct; ++col)
                    {
                        colors[col + (row * BarWidthPx)] = color;
                    }

                    for(; col < BarWidthPx; ++col)
                    {
                        colors[col + (row * BarWidthPx)] = Color.Black;
                    }

                }
                
                HealthBars[pct] = colors;
            }
        }

        public HealthBar(GraphicsDevice graphicsDevice)
        {
            Texture = new Texture2D(graphicsDevice, BarWidthPx, BarHeightPx);
            Texture.SetData(HealthBars[100]);
        }

        public void Dispose()
        {
            Texture.Dispose();
            Texture = null;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, int hp, int maxHp)
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
