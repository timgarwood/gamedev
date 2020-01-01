using Game1.Fonts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Hud
{
    public class SelectedWeapon : HudComponent
    {
        private WeaponInventory WeaponInventory { get; set; }

        private Vector2 Scale { get; set; }

        private SelectedWeapon(HudComponentDefinition definition, WeaponInventory weaponInventory, float scale) :
            base(definition)
        {
            WeaponInventory = weaponInventory;
            Scale = new Vector2(scale, scale);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 viewport)
        {
            var selectedWeapon = WeaponInventory.GetSelectedWeapon();
            if(selectedWeapon != null)
            {
                spriteBatch.Draw(selectedWeapon.Texture, Location, null, null, rotation: 0, scale: Scale);
            }
        }

        public static SelectedWeapon CreateFromData(dynamic jsonData, ContentManager contentManager, GraphicsDevice graphicsDevice, WeaponInventory weaponInventory, GameWorld gameWorld)
        {
            var hudComponentDefinition = HudComponentDefinition.Create(jsonData);

            var scale = float.Parse((string)jsonData["scale"]);

            return new SelectedWeapon(hudComponentDefinition, weaponInventory, scale);
        }

    }
}
