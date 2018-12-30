using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;
using Box2DX.Collision;

namespace Game1
{
    /// <summary>
    /// This class represents an object in the game that can be interacted with and drawn to the screen
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="shape"></param>
        /// <param name="rigidBody"></param>
        public GameObject(Texture2D texture, Shape shape, Body rigidBody)
        {
            Texture = texture;
            Shape = shape;
            RigidBody = rigidBody;
        }

        /// <summary>
        /// texture
        /// </summary>
        public Texture2D Texture { get; private set; } 

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
    }
}
