using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Game1.Fonts
{
    public class FontFactory
    {
        private ContentManager _contentManager;

        private Dictionary<string, Font> _fontMap;

        public static FontFactory Instance { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="contentManager"></param>
        public FontFactory(ContentManager contentManager)
        {
            _contentManager = contentManager;
            _fontMap = new Dictionary<string, Font>();
            Instance = this;
        }

        /// <summary>
        /// load fonts from the given json stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            using(stream)
            {
                using(var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var fontDefinitions = JsonConvert.DeserializeObject<FontDefinition[]>(json);
                    foreach(var fd in fontDefinitions)
                    {
                        var texture = _contentManager.Load<Texture2D>(fd.TextureAsset);
                        uint pixelX, pixelY = 0;
                        if(!fd.Characters[0].TopLeftX.HasValue || !fd.Characters[0].TopLeftY.HasValue)
                        {
                            throw new Exception($"First font character must define a pixel location");
                        }

                        pixelX = fd.Characters[0].TopLeftX.Value;
                        pixelY = fd.Characters[0].TopLeftY.Value;

                        fd.Characters[0].SourceRectangle = new Rectangle(new Point((int)pixelX, (int)pixelY),
                            new Point((int)fd.CharacterWidth, (int)fd.CharacterHeight));

                        for(var i = 1; i < fd.Characters.Length; ++i)
                        {
                            var cd = fd.Characters[i];

                            if(cd.TopLeftX.HasValue && cd.TopLeftY.HasValue)
                            {
                                pixelX = cd.TopLeftX.Value;
                                pixelY = cd.TopLeftY.Value;
                            }
                            else
                            {
                                pixelX = fd.Characters[i - 1].TopLeftX.Value + fd.CharacterWidth;
                                cd.TopLeftX = pixelX+1;
                                cd.TopLeftY = pixelY;
                            }

                            cd.SourceRectangle = new Rectangle(new Point((int)cd.TopLeftX.Value, (int)cd.TopLeftY.Value),
                                new Point((int)fd.CharacterWidth, (int)fd.CharacterHeight));
                        }

                        _fontMap.Add(fd.Name, new Font(texture, fd));
                    }
                }
            }
        }

        public Font GetFont(string name)
        {
            return _fontMap[name];
        }
    }
}
