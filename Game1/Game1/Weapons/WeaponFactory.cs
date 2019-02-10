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

namespace Game1.Weapons
{
    public class WeaponFactory
    {
        public static WeaponFactory Instance { get; private set; }

        private List<WeaponDefinition> _weaponDefinitions;

        private ContentManager _contentManager;
        private World _physicsWorld;

        public WeaponFactory(World physicsWorld, ContentManager contentManager)
        {
            Instance = this;
            _physicsWorld = physicsWorld;
            _contentManager = contentManager;
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
        public Projectile CreateProjectile(string name, Vec2 origin, float rotation)
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

            var bodyDef = new BodyDef();
            bodyDef.IsBullet = true;
            bodyDef.Position.Set(origin.X, origin.Y);
            var body = _physicsWorld.CreateBody(bodyDef);
            var shape = body.CreateShape(shapeDef);

            body.SetMassFromShapes();
            var velocityVector = GameUtils.RotationToVec2((float)(rotation * 180.0 / System.Math.PI));
            body.SetLinearVelocity(velocityVector * definition.Velocity);
            //body.SetLinearVelocity(new Vec2(definition.Velocity * (float)System.Math.Cos(rotation), 
            //    definition.Velocity * (float)System.Math.Sin(rotation)));

            var gameObject = new Projectile(_physicsWorld
                ,definition
                ,texture
                ,shape
                ,body
                ,origin
                ,rotation
                //TODO:  TextureSourceRectangle can be the same per-type of projectile, doesn't need to be created each time
                ,new Rectangle(new Point(definition.XCoordinate, definition.YCoordinate),
                    new Point(definition.Width, definition.Height)));

            GameWorld.Instance.AddGameObject(gameObject);
            return gameObject;
        }
    }
}
