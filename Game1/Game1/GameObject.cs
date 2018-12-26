using Microsoft.Xna.Framework.Graphics;
//using Box2DX.Dynamics;
using ChipmunkSharp;

namespace Game1
{
    public class GameObject
    {
        public GameObject(Texture2D texture, cpShape shape, cpBody body)
        {
            Texture = texture;
            Shape = shape;
            Body = body;
        }

        public Texture2D Texture { get; private set; } 

        public cpShape Shape { get; private set; }

        public cpBody Body { get; private set; }
    }
}
