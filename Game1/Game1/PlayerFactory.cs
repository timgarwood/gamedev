using Game1.Animations;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Box2DX.Collision;
using Microsoft.Xna.Framework;
using Box2DX.Common;
using NLog;
using Game1.Physics;

namespace Game1
{
    public class PlayerFactory
    {
        private GameWorld GameWorld { get; set; }

        private AnimationFactory AnimationFactory { get; set; }

        private World PhysicsWorld { get; set; }

        private ContentManager Content { get; set; }

        private GameData GameData { get; set; }

        private GameUtils GameUtils { get; set; }

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        private WeaponInventory WeaponInventory { get; set; }

        public PlayerFactory(GameData gameData, 
            GameUtils gameUtils,
            ContentManager contentManager, 
            World physicsWorld, 
            GameWorld gameWorld, 
            AnimationFactory animationFactory,
            WeaponInventory weaponInventory)
        {
            GameData = gameData;
            GameUtils = gameUtils;
            Content = contentManager;
            PhysicsWorld = physicsWorld;
            GameWorld = gameWorld;
            AnimationFactory = animationFactory;
            WeaponInventory = weaponInventory;
        }

        public Player CreatePlayer(Texture2D crateTexture)
        {
            var crateShapeDef = new PolygonDef();
            var cratePhysicsSize = GameUtils.PhysicsVec(new Vector2(crateTexture.Width, crateTexture.Height));
            crateShapeDef.Vertices = new Vec2[4];
            crateShapeDef.Vertices[0] = new Vec2(-(cratePhysicsSize.X / 2), -(cratePhysicsSize.Y / 2));
            crateShapeDef.Vertices[1] = new Vec2((cratePhysicsSize.X / 2), -(cratePhysicsSize.Y / 2));
            crateShapeDef.Vertices[2] = new Vec2((cratePhysicsSize.X / 2), (cratePhysicsSize.Y / 2));
            crateShapeDef.Vertices[3] = new Vec2(-(cratePhysicsSize.X / 2), (cratePhysicsSize.Y / 2));
            crateShapeDef.VertexCount = 4;

            Logger.Info($"crate size = ({cratePhysicsSize.X},{cratePhysicsSize.Y})");
            crateShapeDef.Density = GameData.PlayerDensity;
            crateShapeDef.Friction = GameData.PlayerFriction;
            crateShapeDef.Filter.CategoryBits = CollisionCategory.Player;
            crateShapeDef.Filter.MaskBits = (ushort)(CollisionCategory.Wall | CollisionCategory.Alien | CollisionCategory.AlienProjectile | CollisionCategory.Pickup);

            var crateBodyDef = new BodyDef();
            crateBodyDef.IsBullet = true;
            var playerPosition = new Vec2(GameData.PlayerStartX, GameData.PlayerStartY);
            crateBodyDef.Position.Set(playerPosition.X, playerPosition.Y);
            var crateBody = PhysicsWorld.CreateBody(crateBodyDef);
            var crateShape = crateBody.CreateShape(crateShapeDef);
            crateBody.SetMassFromShapes();

            var player = new Player(PhysicsWorld, crateTexture, GameWorld, crateShape, crateBody, AnimationFactory, WeaponInventory, new FilteredKeyListener(), GameData, GameUtils);
            GameWorld.AddGameObject(player);
            return player;
        }
    }
}
