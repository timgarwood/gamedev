using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DX.Common;
using System;

namespace Game1
{
    /// <summary>
    /// Base class for things that can be drawn in the game
    /// Determines whether or not the object is visible through the camera
    /// </summary>
    public abstract class Drawable : IDrawable, IDisposable
    {
        /// <summary>
        /// the rendering scale
        /// </summary>
        protected Vector2 RenderScale { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="origin"></param>
        public Drawable(Texture2D texture)
        {
            Texture = texture;
            RenderScale = new Vector2(1, 1);
        }

        /// <summary>
        /// my texture
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// dispose
        /// </summary>
        public virtual void Dispose()
        {
            Texture = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraOrigin">Top left corner of camera (world position)</param>
        /// <param name="viewport">Size of graphics viewport</param>
        public void Draw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            OnDraw(spriteBatch, cameraOrigin, viewport);
        }

        /// <summary>
        /// performs the type-specific drawing operation
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraOrigin"></param>
        /// <param name="viewport"></param>
        public abstract void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport);

        /// <summary>
        /// retrieves the world position of this object
        /// </summary>
        /// <returns></returns>
        public abstract Vec2 GetWorldPosition();
    }
}
