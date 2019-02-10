﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace Game1.Hud
{
    public class Minimap : HudComponent
    {
        private Texture2D _backgroundTexture;
        private Texture2D _alienTexture;
        private Texture2D _playerTexture;
        private Vector2 _destPoint;
        private static int MinimapBorderThicknessPx = 5;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="definition"></param>
        private Minimap(Texture2D backgroundTexture, Texture2D alienTexture, Texture2D playerTexture, HudComponentDefinition definition) 
            :base(definition)
        {
            _backgroundTexture = backgroundTexture;
            _alienTexture = alienTexture;
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
                var width = (int)jsonData["width"];
                var height = (int)jsonData["height"];
                backgroundTexture = new Texture2D(graphicsDevice, width + (2*MinimapBorderThicknessPx), height + (2*MinimapBorderThicknessPx));
                var textureData = new Color[(width+(2*MinimapBorderThicknessPx)) * (height + (2*MinimapBorderThicknessPx))];
                /*backgroundTexture = new Texture2D(graphicsDevice, width, height);
                var textureData = new Color[width* height];
                for(var i = 0; i < textureData.Length; ++i)
                {
                    textureData[i] = Color.Black;
                }
                */

                //color the top border
                for(var i = 0; i < MinimapBorderThicknessPx * width; ++i)
                {
                    textureData[i] = Color.Red;
                }

                //color the bottom border
                for(var i = (width * height); i < textureData.Length; ++i)
                {
                    textureData[i] = Color.Red;
                }

                for (var i = MinimapBorderThicknessPx * width; i < textureData.Length - (MinimapBorderThicknessPx * width); ++i)
                {
                    var col = i % (width + 2*MinimapBorderThicknessPx);
                    if (col < MinimapBorderThicknessPx)
                    {
                        textureData[i] = Color.Red;
                    }
                    else if (col > width)
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

            Texture2D alienTexture = null;
            try
            {
                alienTexture = contentManager.Load<Texture2D>((string)jsonData["alienTextureAsset"]);
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
            return new Minimap(backgroundTexture, alienTexture, playerTexture, hudComponentDefinition);
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

            var aliens = GameWorld.Instance.GetGameObjects<Alien>();
            foreach(var alien in aliens)
            {
                var alienPosition = alien.GetWorldPosition();
                minimapPosition = new Vector2(_destPoint.X + (alienPosition.X * _backgroundTexture.Width / GameData.Instance.MaxXDimension),
                    _destPoint.Y + (alienPosition.Y * _backgroundTexture.Height / GameData.Instance.MaxYDimension));
                spriteBatch.Draw(_alienTexture, minimapPosition);
            }
        }
    }
}
