using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DX.Common;

namespace Game1
{
    /// <summary>
    /// This class represents an object in the background of the game
    /// </summary>
    public class BackgroundObject : Drawable
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="scale"></param>
        public BackgroundObject(Texture2D texture, Vec2 worldPosition, float scale) : base(texture)
        {
            WorldPosition = worldPosition;
        }

        /// <summary>
        /// the world position of this background object
        /// </summary>
        public Vec2 WorldPosition { get; private set; }

        /// <summary>
        /// returns the world position of this background object
        /// </summary>
        /// <returns></returns>
        public override Vec2 GetWorldPosition()
        {
            return WorldPosition;
        }

        /// <summary>
        /// Draws this background object using the given SpriteBatch
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin)
        {
            var diffx = WorldPosition.X - cameraOrigin.X;
            var diffy = WorldPosition.Y - cameraOrigin.Y;

            var location = new Vector2(diffx * GameData.Instance.PixelsPerMeter, diffy * GameData.Instance.PixelsPerMeter);
            spriteBatch.Draw(Texture, location, null, null, null, 0);
        }
    }
}
