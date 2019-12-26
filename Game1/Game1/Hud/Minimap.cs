using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using Game1.Pickups;

namespace Game1.Hud
{
    public class Minimap : HudComponent
    {
        private Texture2D _backgroundTexture;
        private Texture2D _alienTexture;
        private Texture2D _pickupTexture;
        private Texture2D _playerTexture;
        private Vector2 _destPoint;
        private static int MinimapBorderThicknessPx = 5;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="definition"></param>
        private Minimap(Texture2D backgroundTexture, Texture2D alienTexture, Texture2D pickupTexture, Texture2D playerTexture, HudComponentDefinition definition) 
            :base(definition)
        {
            _backgroundTexture = backgroundTexture;
            _alienTexture = alienTexture;
            _pickupTexture = pickupTexture;
            _playerTexture = playerTexture;
            _destPoint = new Vector2();
        }

        public static Minimap CreateFromData(dynamic jsonData, ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            Texture2D backgroundTexture = null;
            try
            {
                backgroundTexture = contentManager.Load<Texture2D>((string)jsonData["textureAsset"]);
            }
            catch(Exception e)
            {
                var mapWidth = (int)jsonData["width"];
                var mapHeight = (int)jsonData["height"];
                backgroundTexture = new Texture2D(graphicsDevice, mapWidth + (2*MinimapBorderThicknessPx), mapHeight + (2*MinimapBorderThicknessPx));
                var textureData = new Color[(mapWidth+(2*MinimapBorderThicknessPx)) * (mapHeight + (2*MinimapBorderThicknessPx))];
                /*backgroundTexture = new Texture2D(graphicsDevice, width, height);
                var textureData = new Color[width* height];
                for(var i = 0; i < textureData.Length; ++i)
                {
                    textureData[i] = Color.Black;
                }
                */

                //color the top border
                for(var i = 0; i < MinimapBorderThicknessPx * mapWidth; ++i)
                {
                    textureData[i] = Color.Red;
                }

                //color the bottom border
                for(var i = (mapWidth* mapHeight); i < textureData.Length; ++i)
                {
                    textureData[i] = Color.Red;
                }

                for (var i = MinimapBorderThicknessPx * mapWidth; i < textureData.Length - (MinimapBorderThicknessPx * mapWidth); ++i)
                {
                    var col = i % (mapWidth+ 2*MinimapBorderThicknessPx);
                    if (col < MinimapBorderThicknessPx)
                    {
                        textureData[i] = Color.Red;
                    }
                    else if (col > mapWidth)
                    {
                        textureData[i] = Color.Red;
                    }
                    else
                    {
                        textureData[i] = Color.Black;
                    }

                }

                backgroundTexture.SetData(textureData);
            }

            var width = (int)jsonData["alienWidth"];
            var height = (int)jsonData["alienHeight"];

            Texture2D alienTexture = null;
            try
            {
                alienTexture = contentManager.Load<Texture2D>((string)jsonData["alienTextureAsset"]);
            }
            catch(Exception e)
            {
                alienTexture = new Texture2D(graphicsDevice, width, height);
                var textureData = new Color[width * height];
                for (var i = 0; i < textureData.Length; ++i)
                {
                    textureData[i] = Color.Green;
                }

                alienTexture.SetData(textureData);
            }

            var pickupTexture = new Texture2D(graphicsDevice, width, height);
            var pickupTextureData = new Color[width * height];
            for (var i = 0; i < pickupTextureData.Length; ++i)
            {
                pickupTextureData[i] = Color.AliceBlue;
            }

            pickupTexture.SetData(pickupTextureData);

            var playerWidth = (int)jsonData["alienWidth"];
            var playerHeight = (int)jsonData["alienHeight"];
            var playerTexture = new Texture2D(graphicsDevice, playerWidth, playerHeight);
            var playerTextureData = new Color[playerWidth * playerHeight];
            for (var i = 0; i < playerTextureData.Length; ++i)
            {
                playerTextureData[i] = Color.Gray;
            }

            playerTexture.SetData(playerTextureData);

            var hudComponentDefinition = HudComponentDefinition.Create(jsonData);
            return new Minimap(backgroundTexture, alienTexture, pickupTexture, playerTexture, hudComponentDefinition);
        }

        /// <summary>
        /// Draw minimap objects of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        private void Draw<T>(SpriteBatch spriteBatch, Texture2D texture) where T : GameObject
        {
            var objects = GameWorld.Instance.GetGameObjects<T>();
            foreach(var obj in objects)
            {
                var alienPosition = obj.GetWorldPosition();
                var minimapPosition = new Vector2(_destPoint.X + (alienPosition.X * _backgroundTexture.Width / GameData.Instance.MaxXDimension),
                    _destPoint.Y + (alienPosition.Y * _backgroundTexture.Height / GameData.Instance.MaxYDimension));
                spriteBatch.Draw(texture, minimapPosition);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="viewport"></param>
        public override void Draw(SpriteBatch spriteBatch, Vector2 viewport)
        {
            _destPoint.X = (int) Left;
            _destPoint.Y = (int) Top;

            if (_backgroundTexture != null)
            {
                spriteBatch.Draw(_backgroundTexture, _destPoint);
            }

            var playerPosition = Player.Instance.GetWorldPosition();
            var minimapPosition = new Vector2(_destPoint.X + (playerPosition.X * _backgroundTexture.Width / GameData.Instance.MaxXDimension),
                _destPoint.Y + (playerPosition.Y * _backgroundTexture.Height / GameData.Instance.MaxYDimension));
            spriteBatch.Draw(_playerTexture, minimapPosition);

            Draw<Alien>(spriteBatch, _alienTexture);
            Draw<Health>(spriteBatch, _pickupTexture);
        }
    }
}
