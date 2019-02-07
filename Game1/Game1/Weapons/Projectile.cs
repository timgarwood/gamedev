using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace Game1.Weapons
{
    /// <summary>
    /// A projectile flying through the game world
    /// </summary>
    public class Projectile : GameObject
    {
        private WeaponDefinition _definition;
        private Vec2 _origin;

        public Projectile(World world, WeaponDefinition definition, Texture2D texture, Shape shape, Body rigidBody, Vec2 origin, float rotation):
            base(world, texture, shape, rigidBody, rotation)
        {
            _origin = origin;
            _definition = definition;

            //TODO: move this into json
            RenderScale = new Vector2(1.0f,1.0f);
        }

        public override void Update(GameTime gameTime)
        {
            if(Vec2.Distance(_origin, RigidBody.GetPosition()) >= _definition.MaxDistance)
            {
                Remove();
            }
        }
    }
}
