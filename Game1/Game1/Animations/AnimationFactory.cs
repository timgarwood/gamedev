using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Newtonsoft.Json;
using Box2DX.Common;

namespace Game1.Animations
{
    public class AnimationFactory
    {
        private IList<AnimationDefinition> _definitions;
        private ContentManager _contentManager;
        private static AnimationFactory _instance = null;

        public AnimationFactory(ContentManager contentManager)
        {
            _contentManager = contentManager;
            Instance = this;
        }

        public static AnimationFactory Instance
        {
            get
            {
                return _instance;
            }

            private set { _instance = value; }
        }


        public void Load(Stream stream)
        {
            using (stream)
            {
                using(var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    _definitions = JsonConvert.DeserializeObject<AnimationDefinition[]>(json).ToList();
                    foreach(var def in _definitions)
                    {
                        def.Texture = _contentManager.Load<Texture2D>(def.SpriteSheet);
                        def.FrameRectangles = new Microsoft.Xna.Framework.Rectangle[def.NumFrames];
                        var x = 0;
                        var y = 0;
                        for (var i = 0; i < def.FrameRectangles.Length; ++i)
                        {
                            def.FrameRectangles[i] = new Microsoft.Xna.Framework.Rectangle
                            {
                                X = x,
                                Y = y,
                                Width = def.FrameWidth,
                                Height = def.FrameHeight
                            };

                            x += def.FrameWidth;
                            y += def.FrameHeight;

                            if (x >= def.Texture.Width)
                            {
                                x = 0;
                            }

                            if (y >= def.Texture.Height)
                            {
                                y = 0;
                            }
                        }
                    }
                }
            }
        }

        public Animation Create(Vec2 position, string name)
        {
            var def = _definitions.FirstOrDefault(x => x.Name.Equals(name));
            if(def == null)
            {
                throw new System.Exception($"No such definition for animation {name}");
            }

            var animation = new Animation(position, def);
            GameWorld.Instance.AddGameObject(animation);
            return animation;
        }
    }
}
