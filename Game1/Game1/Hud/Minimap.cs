using Box2DX.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace Game1.Hud
{
    public class Minimap : HudComponent
    {
        private Texture2D _backgroundTexture;
        private Texture2D _alienTexture;
        private Vector2 _destPoint;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="definition"></param>
        private Minimap(Texture2D backgroundTexture, Texture2D alienTexture, HudComponentDefinition definition) 
            :base(definition)
        {
            _backgroundTexture = backgroundTexture;
            _alienTexture = alienTexture;
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
                var width = (int)jsonData["width"];
                var height = (int)jsonData["height"];
                backgroundTexture = new Texture2D(graphicsDevice, width, height);
                var textureData = new Color[width * height];
                for (var i = 0; i < textureData.Length; ++i)
                {
                    textureData[i] = Color.Black;
                }

                backgroundTexture.SetData(textureData);
            }

            Texture2D alienTexture = null;
            try
            {
                alienTexture = contentManager.Load<Texture2D>(jsonData["alienTextureAsset"]);
            }
            catch(Exception e)
            {
                var width = (int)jsonData["alienWidth"];
                var height = (int)jsonData["alienHeight"];
                alienTexture = new Texture2D(graphicsDevice, width, height);
                var textureData = new Color[width * height];
                for (var i = 0; i < textureData.Length; ++i)
                {
                    textureData[i] = Color.Green;
                }

                alienTexture.SetData(textureData);
            }

            var hudComponentDefinition = HudComponentDefinition.Create(jsonData);
            return new Minimap(backgroundTexture, alienTexture, hudComponentDefinition);
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

            var aliens = GameWorld.Instance.GetGameObjects<Alien>();
            foreach(var alien in aliens)
            {
                var alienPosition = alien.GetWorldPosition();
                var minimapPosition = new Vector2(_destPoint.X + (alienPosition.X * _backgroundTexture.Width / GameData.Instance.MaxXDimension),
                    _destPoint.Y + (alienPosition.Y * _backgroundTexture.Height / GameData.Instance.MaxYDimension));
                spriteBatch.Draw(_alienTexture, minimapPosition);
            }
        }
    }
}
