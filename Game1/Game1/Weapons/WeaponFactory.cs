using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Newtonsoft.Json;
using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Game1.Physics;

namespace Game1.Weapons
{
    public class WeaponFactory
    {
        public static WeaponFactory Instance { get; private set; }

        private List<WeaponDefinition> _weaponDefinitions;

        private ContentManager _contentManager;
        private World _physicsWorld;
        private GameWorld GameWorld { get; set; }

        private GameUtils GameUtils { get; set; }

        private GameData GameData { get; set; }

        public WeaponFactory(World physicsWorld, 
            ContentManager contentManager, 
            GameWorld gameWorld, 
            GameData gameData,
            GameUtils gameUtils)
        {
            Instance = this;
            _physicsWorld = physicsWorld;
            _contentManager = contentManager;
            GameWorld = gameWorld;
            GameUtils = gameUtils;
            GameData = gameData;
        }

        public void Load(Stream stream)
        {
            using(stream)
            {
                using(var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    _weaponDefinitions = JsonConvert.DeserializeObject<WeaponDefinition[]>(json).ToList();
                }
            }
        }

        /// <summary>
        /// Creates a projectile starting at the given location with the given rotation in radians
        /// </summary>
        /// <param name="name"></param>
        /// <param name="origin"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public Projectile CreateProjectile(string name, Vec2 origin, float rotation, ProjectileSource source)
        {
            var definition = _weaponDefinitions.FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
            if(definition == null)
            {
                throw new Exception($"No WeaponDefinition found for {name}");
            }

            var texture = _contentManager.Load<Texture2D>(definition.SpriteSheet);
            var shapeDef = new PolygonDef();
            var physicsSize = GameUtils.PhysicsVec(new Vector2(definition.Width, definition.Height));
            shapeDef.Vertices = new Vec2[4];
            shapeDef.Vertices[0] = new Vec2(-(physicsSize.X / 2), -(physicsSize.Y / 2));
            shapeDef.Vertices[1] = new Vec2((physicsSize.X / 2), -(physicsSize.Y / 2));
            shapeDef.Vertices[2] = new Vec2((physicsSize.X / 2), (physicsSize.Y / 2));
            shapeDef.Vertices[3] = new Vec2(-(physicsSize.X / 2), (physicsSize.Y / 2));
            shapeDef.VertexCount = 4;

            shapeDef.Density = definition.Density;
            shapeDef.Friction = definition.Friction;
            //projectiles cannot collide with eachother
            if (source == ProjectileSource.Player)
            {
                shapeDef.Filter.CategoryBits = CollisionCategory.PlayerProjectile;
                shapeDef.Filter.MaskBits = (ushort)(CollisionCategory.Alien | CollisionCategory.AlienProjectile | CollisionCategory.Wall);
            }
            else
            {
                shapeDef.Filter.CategoryBits = CollisionCategory.AlienProjectile;
                shapeDef.Filter.MaskBits = (ushort)(CollisionCategory.Player | CollisionCategory.PlayerProjectile | CollisionCategory.Wall);
            }

            var bodyDef = new BodyDef();
            bodyDef.IsBullet = true; bodyDef.Position.Set(origin.X, origin.Y);
            var body = _physicsWorld.CreateBody(bodyDef);
            var shape = body.CreateShape(shapeDef);

            body.SetMassFromShapes();

            var velocityVector = GameUtils.RotationToVec2((float)(rotation * 180.0f / System.Math.PI));
            body.SetLinearVelocity(velocityVector * definition.Velocity);

            var gameObject = new Projectile(_physicsWorld
                , definition
                , texture
                , shape
                , body
                , origin
                , rotation
                , GameData
                , GameUtils);

            GameWorld.AddGameObject(gameObject);
            return gameObject;
        }
    }
}
