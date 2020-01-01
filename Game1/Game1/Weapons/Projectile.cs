using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using NLog;
using System;

namespace Game1.Weapons
{
    public enum ProjectileSource
    {
        Player,
        Alien
    }

    /// <summary>
    /// A projectile flying through the game world
    /// </summary>
    public class Projectile : GameObject
    {
        private WeaponDefinition _definition;
        private Vec2 _origin;
        private DateTime _spawnTime = DateTime.MinValue;
        private Logger Logger = LogManager.GetCurrentClassLogger();

        public Projectile(World world
            ,WeaponDefinition definition
            ,Texture2D texture
            ,Shape shape
            ,Body rigidBody
            ,Vec2 origin
            ,float rotation
            ,GameData gameData
            ,GameUtils gameUtils
            ) :
            base(world, texture, shape, rigidBody, rotation, gameData, gameUtils)
        {
            Active = true;
            _origin = origin;
            _definition = definition;
            _spawnTime = DateTime.UtcNow;

            //TODO: move this into json
            RenderScale = new Vector2(1.0f,1.0f);

        }

        public override void Update(GameTime gameTime)
        {
            if(DateTime.UtcNow - _spawnTime >= TimeSpan.FromSeconds(3))
            {
                PendingDispose = true;
            }
            else if(Vec2.Distance(_origin, RigidBody.GetPosition()) >= _definition.MaxDistance)
            {
                PendingDispose = true;
            }
        }

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            if(Active)
            {
                base.OnDraw(spriteBatch, cameraOrigin, viewport);
            }
        }

        public WeaponDefinition Definition
        {
            get
            {
                return _definition;
            }
        }

        public override void OnCollision(GameObject other, Vec2 position)
        {
            PendingDispose = true;
        }
    }
}
