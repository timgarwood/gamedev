using Game1.Fonts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Hud
{
    public class TotalScore : HudComponent
    {
        private Player Player { get; set; }

        private Font Font { get; set; }

        private string TextTemplate { get; set; }

        public TotalScore(HudComponentDefinition definition,
            Player player,
            Font font,
            string textTemplate) : base(definition)
        {
            Player = player;
            Font = font;
            TextTemplate = textTemplate;
        }

        public static TotalScore CreateFromData(dynamic jsonData,
            ContentManager contentManager,
            GraphicsDevice graphicsDevice,
            WeaponInventory weaponInventory,
            GameWorld gameWorld,
            GameData gameData,
            Player player)
        {
            var hudComponentDefinition = HudComponentDefinition.Create(jsonData);

            var textTemplate = (string)jsonData["textTemplate"];
            var fontName = (string)jsonData["fontName"];

            var font = FontFactory.Instance.GetFont(fontName);

            return new TotalScore(hudComponentDefinition, player, font, textTemplate);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 viewport)
        {
            var text = TextTemplate.Replace("{totalScore}", Player.TotalScore.ToString());
            Font.DrawString(spriteBatch, text, Location);
        }
    }
}