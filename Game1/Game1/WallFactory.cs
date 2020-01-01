using Box2DX.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Box2DX.Common;
using Box2DX.Collision;
using Math = System.Math;
using Game1.Physics;

namespace Game1
{
    public class WallFactory
    {
        private GameData GameData { get; set; }

        private World PhysicsWorld { get; set; }
        private GameWorld GameWorld { get; set; }

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public WallFactory(GameData gameData, World physicsWorld, GameWorld gameWorld)
        {
            GameData = gameData;
            PhysicsWorld = physicsWorld;
            GameWorld = gameWorld;
        }

        /// <summary>
        /// Creates a wall
        /// </summary>
        /// <param name="x">x position in pixels</param>
        /// <param name="y">y position in pixels</param>
        /// <param name="w">width of wall in pixels</param>
        /// <param name="h">height of wall in pixels</param>
        /// <returns></returns>
        private GameObject Wall(Vec2 topLeft, Vec2 bottomRight)
        {
            // Define the ground body.
            var wallBodyDef = new BodyDef();
            wallBodyDef.Position.Set(topLeft.X, topLeft.Y);

            // Call the body factory which creates the wall box shape.
            // The body is also added to the world.
            var wallBody = PhysicsWorld.CreateBody(wallBodyDef);

            // Define the wall box shape.
            var wallShapeDef = new PolygonDef();
            wallShapeDef.Friction = 0.3f;
            wallShapeDef.Density = 1.0f;

            // The extents are the half-widths of the box.
            var wallPhysicsSize = new Vec2(Math.Abs(bottomRight.X - topLeft.X), Math.Abs(bottomRight.Y - topLeft.Y));
            if (wallPhysicsSize.X <= 0)
            {
                wallPhysicsSize.X = 1 * GameData.MetersPerPixel;
            }
            if (wallPhysicsSize.Y <= 0)
            {
                wallPhysicsSize.Y = 1 * GameData.MetersPerPixel;
            }

            wallShapeDef.Filter.CategoryBits = (ushort)CollisionCategory.Wall;
            wallShapeDef.Filter.MaskBits = (ushort)(CollisionCategory.Player | CollisionCategory.Alien | CollisionCategory.PlayerProjectile | CollisionCategory.AlienProjectile);
            wallShapeDef.SetAsBox(wallPhysicsSize.X, wallPhysicsSize.Y);

            // Add the ground shape to the ground body.
            var shape = wallBody.CreateShape(wallShapeDef);
            var vTex = GameUtils.GraphicsVec(wallPhysicsSize);

            if (vTex.X <= 0)
            {
                vTex.X = 1;
            }
            if (vTex.Y <= 0)
            {
                vTex.Y = 1;
            }

            Logger.Info($"Wall created at ({wallBody.GetPosition().X},{wallBody.GetPosition().Y}) " +
                $"extends to ({wallBody.GetPosition().X + wallPhysicsSize.X},{wallBody.GetPosition().Y + wallPhysicsSize.Y})");
            return new GameObject(PhysicsWorld, null, shape, wallBody, 0);
        }

        public void CreateWalls()
        {
            //top wall
            GameWorld.AddGameObject(Wall(new Vec2(0.1f, 0.1f), new Vec2(GameData.MaxXDimension - 1, 0.1f)));
            GameWorld.AddGameObject(Wall(new Vec2(0.1f, GameData.MaxYDimension - 1), new Vec2(GameData.MaxXDimension - 1, GameData.MaxYDimension - 1)));
            GameWorld.AddGameObject(Wall(new Vec2(0.1f, 0.1f), new Vec2(0.1f, GameData.MaxYDimension - 1)));
            GameWorld.AddGameObject(Wall(new Vec2(GameData.MaxXDimension - 1, 0.1f), new Vec2(GameData.MaxXDimension - 1, GameData.MaxYDimension - 1)));
        }
    }
}
