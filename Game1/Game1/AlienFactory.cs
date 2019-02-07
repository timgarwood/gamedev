using System;
using System.IO;
using Newtonsoft.Json;
using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NLog;

namespace Game1
{
    public class AlienFactory
    {
        /// <summary>
        /// logger 
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        private IList<AlienDefinition> _alienDefinitions = null;

        private World _physicsWorld;

        private ContentManager _contentManager;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="physicsWorld"></param>
        public AlienFactory(World physicsWorld, ContentManager contentManager)
        {
            _physicsWorld = physicsWorld;
            _contentManager = contentManager;
        }

        /// <summary>
        /// loads alien definitions from the given json stream
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            using(stream)
            {
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var defs = JsonConvert.DeserializeObject<AlienDefinition[]>(json);
                    _alienDefinitions = new List<AlienDefinition>(defs);
                }
            }
        }

        /// <summary>
        /// creates an instance of the given alien definition name
        /// </summary>
        /// <param name="name"></param>
        public GameObject Create(string name, Vec2 location)
        {
            if(_alienDefinitions != null)
            {
                var definition = _alienDefinitions.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if(definition == null)
                {
                    throw new Exception($"No Alien definition found for name {name}");
                }
                var texture = _contentManager.Load<Texture2D>(definition.TextureName);

                var physicsSize = GameUtils.PhysicsVec(new Vector2(texture.Width * definition.Scale, texture.Height * definition.Scale));

                var shapeDef = new PolygonDef();
                shapeDef.Vertices = new Vec2[4];
                shapeDef.Vertices[0] = new Vec2(-(physicsSize.X / 2), -(physicsSize.Y / 2));
                shapeDef.Vertices[1] = new Vec2((physicsSize.X / 2), -(physicsSize.Y / 2));
                shapeDef.Vertices[2] = new Vec2((physicsSize.X / 2), (physicsSize.Y / 2));
                shapeDef.Vertices[3] = new Vec2(-(physicsSize.X / 2), (physicsSize.Y / 2));
                shapeDef.VertexCount = 4;

                Logger.Info($"crate size = ({physicsSize.X},{physicsSize.Y})");
                shapeDef.Density = definition.Density;
                shapeDef.Friction = definition.Friction;

                var bodyDef = new BodyDef();
                bodyDef.IsBullet = true;
                bodyDef.Position.Set(location.X, location.Y);
                var body = _physicsWorld.CreateBody(bodyDef);
                var shape = body.CreateShape(shapeDef);

                body.SetMassFromShapes();

                var gameObject = new Alien(_physicsWorld, definition, texture, shape, body);
                GameWorld.Instance.AddGameObject(gameObject);
                return gameObject;
            }

            throw new Exception("Uninitialized definitions");
        }
    }
}
