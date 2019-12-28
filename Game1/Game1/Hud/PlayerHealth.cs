using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game1.Fonts;

namespace Game1.Hud
{
    public class PlayerHealth : HudComponent
    {
        private Font _font;
        private string _textTemplate;

        private PlayerHealth(HudComponentDefinition hudComponentDefinition,
            Font font,
            string template) :
            base(hudComponentDefinition)
        {
            _font = font;
            _textTemplate = template;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 viewport)
        {
            var text = _textTemplate.Replace("{health}", Player.Instance.Hp.ToString());
            _font.DrawString(spriteBatch, text, Location);
        }

        public static PlayerHealth CreateFromData(dynamic jsonData, ContentManager contentManager, GraphicsDevice graphicsDevice, WeaponInventory weaponInventory)
        {
            var hudComponentDefinition = HudComponentDefinition.Create(jsonData);

            var font = FontFactory.Instance.GetFont((string)jsonData["fontName"]);
            var textTemplate = (string)jsonData["textTemplate"];

            return new PlayerHealth(hudComponentDefinition, font, textTemplate);
        }
    }
}
