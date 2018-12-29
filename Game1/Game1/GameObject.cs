using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;
using Box2DX.Collision;

namespace Game1
{
    public class GameObject
    {
        public GameObject(Texture2D texture, Shape shape, Body rigidBody)
        {
            Texture = texture;
            Shape = shape;
            RigidBody = rigidBody;
        }

        public Texture2D Texture { get; private set; } 

        public Body RigidBody { get; private set; }

        public Shape Shape { get; private set; }

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
