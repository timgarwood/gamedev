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
using Game1.Animations;
using Game1.Physics;

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

        private AnimationFactory _animationFactory;

        private GameWorld GameWorld { get; set; }

        private GraphicsDevice GraphicsDevice { get; set; }

        private GameData GameData { get; set; }

        private GameUtils GameUtils { get; set; }

        private Player Player { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="physicsWorld"></param>
        public AlienFactory(World physicsWorld, 
            GameData gameData,
            GameUtils gameUtils,
            ContentManager contentManager, 
            GameWorld gameWorld,
            AnimationFactory animationFactory, 
            GraphicsDevice graphicsDevice,
            Player player)
        {
            _physicsWorld = physicsWorld;
            GameData = gameData;
            GameUtils = gameUtils;
            _contentManager = contentManager;
            _animationFactory = animationFactory;
            GraphicsDevice = graphicsDevice;
            GameWorld = gameWorld;
            Player = player;
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
        public Alien Create(string name, Vec2 location)
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
                shapeDef.Vertices = new Vec2[definition.Vertices.Length];
                for(var i = 0; i < definition.Vertices.Length; ++i)
                {
                    var x = definition.Vertices[i].X;
                    var y = definition.Vertices[i].Y;
                    shapeDef.Vertices[i] = GameUtils.PhysicsVec(new Vector2(x, y));
                }

                shapeDef.VertexCount = shapeDef.Vertices.Length;

                shapeDef.Density = definition.Density;
                shapeDef.Friction = definition.Friction;
                shapeDef.Filter.CategoryBits = CollisionCategory.Alien;
                shapeDef.Filter.MaskBits = (ushort)(CollisionCategory.Wall | CollisionCategory.Player | CollisionCategory.PlayerProjectile | CollisionCategory.Alien);

                var bodyDef = new BodyDef();
                bodyDef.IsBullet = true;
                bodyDef.Position.Set(location.X, location.Y);
                var body = _physicsWorld.CreateBody(bodyDef);
                var shape = body.CreateShape(shapeDef);

                body.SetMassFromShapes();

                var gameObject = new Alien(_physicsWorld, GameData, GameUtils, definition, _animationFactory, GameWorld, texture, shape, body, GraphicsDevice, Player);
                GameWorld.AddGameObject(gameObject);
                return gameObject;
            }

            throw new Exception("Uninitialized definitions");
        }
    }
}
