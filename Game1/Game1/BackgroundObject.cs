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
        private GameData GameData { get; set; }

        private GameUtils GameUtils { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="scale"></param>
        public BackgroundObject(GameData gameData,
            GameUtils gameUtils,
            Texture2D texture, 
            Vec2 worldPosition, 
            int distanceFromCamera) : base(texture)
        {
            GameData = gameData;
            GameUtils = gameUtils;
            WorldPosition = worldPosition;
            DistanceFromCamera = distanceFromCamera;

            var scale = (((float)GameData.MaxDistanceFromCamera / (float)distanceFromCamera) * GameData.MaxBackgroundScale) + 
                GameData.MinBackgroundScale;
            Scale = new Vector2(scale, scale);
        }

        /// <summary>
        /// the world position of this background object
        /// </summary>
        public Vec2 WorldPosition { get; private set; }

        /// <summary>
        /// how far are we from the camera
        /// </summary>
        public int DistanceFromCamera { get; private set; }

        /// <summary>
        /// how much to scale the background object
        /// based on distance-from-camera
        /// </summary>
        public Vector2 Scale { get; private set; }

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
        /// <param name="cameraOrigin"></param>
        /// <param name="viewport"></param>
        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            //parallax calculation
            //we can use Scale.X and Scale.Y here because they are derived from distance-from-camera
            /*var diffx = (WorldPosition.X + ((1 - Scale.X) * cameraOrigin.X)) - cameraOrigin.X;
            var diffy = (WorldPosition.Y + ((1 - Scale.Y) * cameraOrigin.Y)) - cameraOrigin.Y;

            var location = new Vector2(diffx * GameData.PixelsPerMeter, diffy * GameData.Instance.PixelsPerMeter);
            spriteBatch.Draw(Texture, location, null, null, null, 0, Scale);
            */
            var location = GameUtils.GraphicsVec(WorldPosition - cameraOrigin);
            spriteBatch.Draw(Texture, location);
        }
    }
}
