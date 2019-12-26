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
        private PickupDefinition[] Definitions { get; set; }

        private ContentManager ContentManager { get; set; }

        private World PhysicsWorld { get; set; }

        public static PickupFactory Instance
        {
            get
            {
                return _instance;
            }
        }

        private static PickupFactory _instance { get; set; }

        public PickupFactory(World physicsWorld, ContentManager _manager)
        {
            ContentManager = _manager;
            PhysicsWorld = physicsWorld;
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

        public void CreateHealthPickup(Vec2 origin, string definitionName)
        {
            var definition = Definitions.FirstOrDefault(d => d.Name.ToLower().Equals(definitionName.ToLower()));
            if(definition == null)
            {
                throw new Exception($"No such pickup definition {definitionName}");
            }

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
            
            var hp = int.Parse(kvp["Hp"]);

            var healthPickup = new Health(PhysicsWorld, texture, shape, body, hp, scale);
            GameWorld.Instance.AddGameObject(healthPickup);
        }
    }
}
