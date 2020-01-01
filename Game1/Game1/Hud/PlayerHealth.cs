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
        private Player Player { get; set; }

        private PlayerHealth(HudComponentDefinition hudComponentDefinition,
            Font font,
            string template,
            Player player) :
            base(hudComponentDefinition)
        {
            _font = font;
            _textTemplate = template;
            Player = player;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 viewport)
        {
            var text = _textTemplate.Replace("{health}", Player.Hp.ToString());
            _font.DrawString(spriteBatch, text, Location);
        }

        public static PlayerHealth CreateFromData(dynamic jsonData, 
            ContentManager contentManager, 
            GraphicsDevice graphicsDevice, 
            WeaponInventory weaponInventory, 
            GameWorld gameWorld,
            GameData gameData,
            Player player)
        {
            var hudComponentDefinition = HudComponentDefinition.Create(jsonData);

            var font = FontFactory.Instance.GetFont((string)jsonData["fontName"]);
            var textTemplate = (string)jsonData["textTemplate"];

            return new PlayerHealth(hudComponentDefinition, font, textTemplate, player);
        }
    }
}
