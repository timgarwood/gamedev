using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game1
{
    /// <summary>
    /// This class represents an object in the background of the game
    /// </summary>
    public class BackgroundObject
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="distanceFromCamera"></param>
        public BackgroundObject(Texture2D texture, float distanceFromCamera)
        {
        }

        /// <summary>
        /// the texture of the background
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// the world position of this background object.  This uses the XNA vector
        /// type since we don't really need a physics type for backgrounds
        /// </summary>
        public Vector2 WorldPosition { get; private set; }

        /// <summary>
        /// Draws this background object using the given SpriteBatch
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 cameraOrigin, Vector2 viewport)
        {
        }
    }
}
