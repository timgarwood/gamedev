using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Box2DX.Common;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Box2DX.Collision;
using Microsoft.Xna.Framework;
using Game1.Physics;
using Box2DX.Dynamics;

namespace Game1.Pickups
{
    public class PickupFactory
    {
        private struct PickupTemplate
        {
            public Texture2D Texture { get; set; }
            public Body RigidBody { get; set;}
            public Shape Shape { get; set; }
            public float Scale { get; set; }
        }

        private PickupDefinition[] Definitions { get; set; }

        private ContentManager ContentManager { get; set; }

        private World PhysicsWorld { get; set; }

        private GameWorld GameWorld { get; set; }

        public static PickupFactory Instance
        {
            get
            {
                return _instance;
            }
        }

        private static PickupFactory _instance { get; set; }

        public PickupFactory(World physicsWorld, ContentManager _manager, GameWorld gameWorld)
        {
            ContentManager = _manager;
            PhysicsWorld = physicsWorld;
            GameWorld = gameWorld;
            _instance = this;
        }

        public void Load(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();

                Definitions = JsonConvert.DeserializeObject<PickupDefinition[]>(json);
            }
        }

        private PickupTemplate CreatePickupTemplate(Vec2 origin, PickupDefinition definition)
        {
            var kvp = definition.Values;

            var texture = ContentManager.Load<Texture2D>(kvp["TextureName"]);
            var scale = float.Parse(kvp["Scale"]);
            var width = texture.Width * scale;
            var height = texture.Height * scale;
            var shapeDef = new PolygonDef();
            var physicsSize = GameUtils.PhysicsVec(new Vector2(width, height));
            shapeDef.Vertices = new Vec2[4];
            shapeDef.Vertices[0] = new Vec2(-(physicsSize.X / 2), -(physicsSize.Y / 2));
            shapeDef.Vertices[1] = new Vec2((physicsSize.X / 2), -(physicsSize.Y / 2));
            shapeDef.Vertices[2] = new Vec2((physicsSize.X / 2), (physicsSize.Y / 2));
            shapeDef.Vertices[3] = new Vec2(-(physicsSize.X / 2), (physicsSize.Y / 2));
            shapeDef.VertexCount = 4;

            shapeDef.Density = float.Parse(kvp["Density"]);
            shapeDef.Friction = float.Parse(kvp["Friction"]);
            shapeDef.Filter.CategoryBits = CollisionCategory.Pickup;
            shapeDef.Filter.MaskBits = CollisionCategory.Player;

            var bodyDef = new BodyDef();
            bodyDef.Position.Set(origin.X, origin.Y);
            var body = PhysicsWorld.CreateBody(bodyDef);
            var shape = body.CreateShape(shapeDef);

            body.SetMass(new MassData
            {
                Mass = 0
            });

            return new PickupTemplate
            {
                Texture = texture,
                RigidBody = body,
                Shape = shape,
                Scale = scale
            };
        }

        public void CreateHealthPickup(Vec2 origin, string definitionName)
        {
            var definition = Definitions.FirstOrDefault(d => d.Name.ToLower().Equals(definitionName.ToLower()));
            if(definition == null)
            {
                throw new Exception($"No such pickup definition {definitionName}");
            }

            var template = CreatePickupTemplate(origin, definition);

            var hp = int.Parse(definition.Values["Hp"]);

            var healthPickup = new Health(PhysicsWorld, template.Texture, template.Shape, template.RigidBody, hp, template.Scale);
            GameWorld.AddGameObject(healthPickup);
        }

        public void CreateLaserPickup(Vec2 origin, string definitionName)
        {
            var definition = Definitions.FirstOrDefault(d => d.Name.ToLower().Equals(definitionName.ToLower()));
            if(definition == null)
            {
                throw new Exception($"No such pickup definition {definitionName}");
            }

            var projectileName = definition.Values["ProjectileName"];
            var startingAmmo = int.Parse(definition.Values["StartingAmmo"]);
            
            var template = CreatePickupTemplate(origin, definition);

            var laserPickup = new Laser(PhysicsWorld, template.Texture, template.Shape, template.RigidBody, projectileName, startingAmmo, definitionName, template.Scale);
            GameWorld.AddGameObject(laserPickup);
        }

    }
}
