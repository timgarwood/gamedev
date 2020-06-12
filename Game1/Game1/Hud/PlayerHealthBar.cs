using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Hud
{
    public class PlayerHealthBar : HudComponent
    {
        private HealthBar HealthBar { get; set; }
        private Player Player { get; set; }

        public PlayerHealthBar(HudComponentDefinition hudComponentDefinition, 
            Player player,
            HealthBar healthBar) :
            base(hudComponentDefinition)
        {
            Player = player;
            HealthBar = healthBar;
        }

        public static PlayerHealthBar CreateFromData(dynamic jsonData, 
            ContentManager contentManager, 
            GraphicsDevice graphicsDevice, 
            WeaponInventory weaponInventory, 
            GameWorld gameWorld,
            GameData gameData,
            HealthBarFactory healthBarFactory,
            Player player)
        {
            var hudComponentDefinition = HudComponentDefinition.Create(jsonData);
            var healthBar = healthBarFactory.Create("PlayerHealthBar", hudComponentDefinition.Width, hudComponentDefinition.Height);
            return new PlayerHealthBar(hudComponentDefinition, player, healthBar);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 viewport)
        {
            HealthBar.Draw(spriteBatch, Location, Player.Hp);
        }
    }
}
