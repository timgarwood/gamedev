using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;
using System;

namespace Game1.Fonts
{
    public class Font
    {
        private Texture2D _texture;
        private FontDefinition _fontDefinition;
        private StringComparison _stringComparison;

        public Font(Texture2D texture, FontDefinition fontDefinition)
        {
            _texture = texture;
            _fontDefinition = fontDefinition;
            if(_fontDefinition.CaseSensitive)
            {
                _stringComparison = StringComparison.Ordinal;
            }
            else
            {
                _stringComparison = StringComparison.OrdinalIgnoreCase;
            }
        }

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 location)
        {
            var destVector = new Vector2(location.X, location.Y);

            foreach(var c in text)
            {
                //TODO:  make this lookup faster
                var charDef = _fontDefinition.Characters.FirstOrDefault(cd => cd.Character.ToString()
                .Equals(c.ToString(), _stringComparison));

                spriteBatch.Draw(_texture, destVector, null, charDef.SourceRectangle);
                destVector.X += _fontDefinition.CharacterWidth;
                //TODO: handle newlines?
                //destVector.Y += _fontDefinition.CharacterHeight;
                //destVector.X = location.X;
            }
        }
    }
}
