using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;

namespace Game1
{
    public class GameObject
    {
        public GameObject(Texture2D texture, Body rigidBody)
        {
            Texture = texture;
            RigidBody = rigidBody;
        }

        public Texture2D Texture { get; private set; } 

        public Body RigidBody { get; private set; }
    }
}
