using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Game1.Fonts
{
    public class Font
    {
        private Texture2D _texture;
        private FontDefinition _fontDefinition;

        public Font(Texture2D texture, FontDefinition fontDefinition)
        {
            _texture = texture;
            _fontDefinition = fontDefinition;
        }

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 location)
        {
            var destVector = new Vector2(location.X, location.Y);

            foreach(var c in text)
            {
                //TODO:  make this lookup faster
                var charDef = _fontDefinition.Characters.FirstOrDefault(cd => cd.Character.ToString()
                .Equals(c.ToString(), System.StringComparison.OrdinalIgnoreCase));

                spriteBatch.Draw(_texture, destVector, null, charDef.SourceRectangle);
                destVector.X += _fontDefinition.CharacterWidth;
                //TODO: handle newlines?
                //destVector.Y += _fontDefinition.CharacterHeight;
                //destVector.X = location.X;
            }
        }
    }
}
