using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using Microsoft.Xna.Framework;

namespace Game1
{
    /// <summary>
    /// This class represents an object in the game that can be interacted with and drawn to the screen
    /// </summary>
    public class GameObject : Drawable
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="shape"></param>
        /// <param name="rigidBody"></param>
        public GameObject(Texture2D texture, Shape shape, Body rigidBody) : base(texture)
        {
            Shape = shape;
            RigidBody = rigidBody;
        }

        /// <summary>
        /// physics body
        /// </summary>
        public Body RigidBody { get; private set; }

        /// <summary>
        /// physics shape
        /// </summary>
        public Shape Shape { get; private set; }

        /// <summary>
        /// physics bounding box
        /// </summary>
        public AABB BoundingBox
        {
            get
            {
                AABB boundingBox;
                Shape.ComputeAABB(out boundingBox, RigidBody.GetXForm());
                return boundingBox;
            }
        }

        public override Vec2 GetWorldPosition()
        {
            return Vec2.Zero;
        }

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {

        }
    }
}
