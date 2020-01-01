using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Hud
{
    public class Hud
    {
        /// <summary>
        /// the hud components
        /// </summary>
        private List<HudComponent> _hudComponents;

        private ContentManager _contentManager;
        private GraphicsDevice _graphicsDevice;
        private WeaponInventory _weaponInventory;
        private GameWorld GameWorld { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public Hud(ContentManager contentManager, 
            GraphicsDevice graphicsDevice, 
            WeaponInventory weaponInventory,
            GameWorld gameWorld)
        {
            _contentManager = contentManager;
            _graphicsDevice = graphicsDevice;
            _weaponInventory = weaponInventory;
            _hudComponents = new List<HudComponent>();
            GameWorld = gameWorld;
        }

        /// <summary>
        /// window resized method
        /// </summary>
        /// <param name="viewport"></param>
        public void OnWindowResized(Vector2 viewport)
        {
            foreach(var hudComponent in _hudComponents)
            {
                hudComponent.OnWindowResized(viewport);
            }
        }

        /// <summary>
        /// initializes hud components from the given json stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            using(stream)
            {
                using(var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var objects = JsonConvert.DeserializeObject<dynamic[]>(json);
                    foreach (var o in objects)
                    {
                        Type type = Type.GetType((string)o["componentTypeName"], true, true);
                        var createMethod = type.GetMethod("CreateFromData", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                        if(createMethod == null)
                        {
                            throw new Exception($"No create method found for type {type}");
                        }

                        var hudComponent = createMethod.Invoke(null, new object[] { o, _contentManager, _graphicsDevice, _weaponInventory, GameWorld});
                        _hudComponents.Add((HudComponent)hudComponent);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 viewport)
        {
            foreach(var component in _hudComponents)
            {
                component.Draw(spriteBatch, viewport);
            }
        }
    }
}
