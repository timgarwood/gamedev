using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Box2DX.Common;
using System.Linq;

namespace Game1
{
    /// <summary>
    /// 
    /// </summary>
    public static class Background
    {

        private static Texture2D Texture { get; set; }

        private static List<BackgroundObject> BackgroundObjects { get; set; } = new List<BackgroundObject>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameData"></param>
        public static void GenerateBackground(Texture2D[] backgroundTextures, GameData gameData)
        {
            Texture = backgroundTextures[0];

            var xPixelTotal = gameData.MaxXDimension * gameData.PixelsPerMeter;
            var yPixelTotal = gameData.MaxYDimension * gameData.PixelsPerMeter;

            var cols = xPixelTotal / Texture.Width;
            if(xPixelTotal % Texture.Width != 0)
            {
                cols++;
            }

            var rows = yPixelTotal / Texture.Height;
            if(yPixelTotal % Texture.Height != 0)
            {
                rows++;
            }

            for(var i = 0; i < cols; ++i)
            {
                for(var j = 0; j < rows; ++j)
                {
                    var v = GameUtils.PhysicsVec(new Vector2(i * Texture.Width, j * Texture.Height));
                    BackgroundObjects.Add(new BackgroundObject(Texture, v, GameData.Instance.MaxDistanceFromCamera));
                }
            }


            /*var textureRandom = new Random((int)(DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds);
            var positionRandom = new Random((int)(DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds);
            var distanceRandom = new Random((int)(DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds);
            for(var i = 0; i < gameData.NumBackgroundObjects; ++i)
            {
                var texture = backgroundTextures[textureRandom.Next(0, backgroundTextures.Length - 1)];
                var positionX = positionRandom.Next(0, (int)gameData.MaxXDimension * (int)gameData.MaxXDimension);
                var positionY = positionRandom.Next(0, (int)gameData.MaxYDimension * (int)gameData.MaxYDimension);
                var positionXf = ((float)positionX) / gameData.MaxXDimension;
                var positionYf = ((float)positionY) / gameData.MaxYDimension;
                var position = new Vec2(positionXf, positionYf);
                var distance = distanceRandom.Next(gameData.MaxDistanceFromCamera, gameData.MinDistanceFromCamera);

                _backgroundObjects.Add(new BackgroundObject(texture, position, distance));
            }

            _backgroundObjects.OrderBy(x => x.DistanceFromCamera).Reverse();
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawBackground(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            foreach (var bgo in BackgroundObjects)
            {
                bgo.Draw(spriteBatch, cameraOrigin, viewport);
            }
        }
    }
}
