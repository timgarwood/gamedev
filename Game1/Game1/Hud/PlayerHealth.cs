﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game1.Fonts;

namespace Game1.Hud
{
    public class PlayerHealth : HudComponent
    {
        private Font _font;
        private string _textTemplate;
        private Vector2 _location;

        private PlayerHealth(HudComponentDefinition hudComponentDefinition,
            Font font,
            string template) :
            base(hudComponentDefinition)
        {
            _font = font;
            _textTemplate = template;
            _location = new Vector2(Left, Top);
        }

        public override void OnWindowResized(Vector2 viewport)
        {
            base.OnWindowResized(viewport);
            _location = new Vector2(Left, Top);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 viewport)
        {
            var text = _textTemplate.Replace("{health}", "100");
            _font.DrawString(spriteBatch, text, _location);
        }

        public static PlayerHealth CreateFromData(dynamic jsonData, ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            var hudComponentDefinition = new HudComponentDefinition();

            var font = FontFactory.Instance.GetFont((string)jsonData["fontName"]);
            var textTemplate = (string)jsonData["textTemplate"];

            hudComponentDefinition.Width = (int)jsonData["width"];
            hudComponentDefinition.Height = (int)jsonData["width"];
            hudComponentDefinition.HAlign = (string)jsonData["hAlignment"];
            hudComponentDefinition.VAlign = (string)jsonData["vAlignment"];
            hudComponentDefinition.HorizontalPercentage = (float)jsonData["horizontalPercentage"];
            hudComponentDefinition.VerticalPercentage = (float)jsonData["verticalPercentage"];

            return new PlayerHealth(hudComponentDefinition, font, textTemplate);
        }
    }
}
