using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace Game1.Weapons
{
    public class Projectile : GameObject
    {
        private WeaponDefinition _definition;

        public Projectile(WeaponDefinition definition, Texture2D texture, Shape shape, Body rigidBody, float rotation):
            base(texture, shape, rigidBody, rotation)
        {
            _definition = definition;
            //TODO: move this into json
            RenderScale = new Vector2(1.0f,1.0f);
        }
    }
}
