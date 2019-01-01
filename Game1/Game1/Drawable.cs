using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DX.Common;

namespace Game1
{
    /// <summary>
    /// Base class for things that can be drawn in the game
    /// Determines whether or not the object is visible through the camera
    /// </summary>
    public abstract class Drawable : IDrawable
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="origin"></param>
        public Drawable(Texture2D texture)
        {
            Texture = texture;
        }

        /// <summary>
        /// my texture
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraOrigin">Top left corner of camera (world position)</param>
        /// <param name="viewport">Size of graphics viewport</param>
        public void Draw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            //FIXME:  add filtering for objects that are not visible
            OnDraw(spriteBatch, cameraOrigin);
        }

        /// <summary>
        /// performs the type-specific drawing operation
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraOrigin"></param>
        public abstract void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin);

        /// <summary>
        /// retrieves the world position of this object
        /// </summary>
        /// <returns></returns>
        public abstract Vec2 GetWorldPosition();
    }
}
