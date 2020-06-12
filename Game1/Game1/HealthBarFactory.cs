using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class HealthBarFactory
    {
        private Dictionary<string, Dictionary<int, Color[]> > HealthTextures { get; set; }

        private GraphicsDevice GraphicsDevice { get; set; }

        public HealthBarFactory(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            HealthTextures = new Dictionary<string, Dictionary<int, Color[]> >();
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

        public HealthBar Create(string name, int width, int height)
        {
            if (!HealthTextures.ContainsKey(name))
            {
                var healthTextures = new Dictionary<int, Color[]>();

                for (var pct = 100; pct >= 0; pct -= 10)
                {
                    var colors = new Color[width * height];
                    //figure out what color to make this percentage
                    var color = GetColorForPct(pct);
                    for (var row = 0; row < height; ++row)
                    {
                        //color up to the percentage of the health bar 
                        var col = 0;
                        for (; col < pct * (width / 100); ++col)
                        {
                            colors[col + (row * width)] = color;
                        }

                        for (; col < width; ++col)
                        {
                            colors[col + (row * width)] = Color.Black;
                        }

                    }

                    healthTextures[pct] = colors;
                }
                
                HealthTextures[name] = healthTextures;
            }

            var texture = new Texture2D(GraphicsDevice, width, height);

            var healthBar = new HealthBar(GraphicsDevice, texture, HealthTextures[name]);
            return healthBar;
        }
    }
}
