using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Box2DX.Common;

namespace Game1
{
    /// <summary>
    /// 
    /// </summary>
    public static class Background
    {
        static Background()
        {
            _backgroundObjects = new List<BackgroundObject>();
        }

        private static List<BackgroundObject> _backgroundObjects;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameData"></param>
        public static void GenerateBackground(Texture2D[] backgroundTextures, GameData gameData)
        {
            _backgroundObjects.Clear();
            var textureRandom = new Random((int)(DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds);
            var positionRandom = new Random((int)(DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds);
            for(var i = 0; i < gameData.NumBackgroundObjects; ++i)
            {
                var texture = backgroundTextures[textureRandom.Next(0, backgroundTextures.Length - 1)];
                var positionX = positionRandom.Next(0, (int)gameData.MaxXDimension * (int)gameData.MaxXDimension);
                var positionY = positionRandom.Next(0, (int)gameData.MaxYDimension * (int)gameData.MaxYDimension);
                var positionXf = ((float)positionX) / gameData.MaxXDimension;
                var positionYf = ((float)positionY) / gameData.MaxYDimension;
                var position = new Vec2(positionXf, positionYf);

                _backgroundObjects.Add(new BackgroundObject(texture, position, 1));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void DrawBackground(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            foreach (var bgo in _backgroundObjects)
            {
                bgo.Draw(spriteBatch, cameraOrigin, viewport);
            }
        }
    }
}
