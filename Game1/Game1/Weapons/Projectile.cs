﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using NLog;

namespace Game1.Weapons
{
    /// <summary>
    /// A projectile flying through the game world
    /// </summary>
    public class Projectile : GameObject
    {
        private WeaponDefinition _definition;
        private Vec2 _origin;
        private Logger Logger = LogManager.GetCurrentClassLogger();

        public Projectile(World world
            ,WeaponDefinition definition
            ,Texture2D texture
            ,Shape shape
            ,Body rigidBody
            ,Vec2 origin
            ,float rotation
            ,Rectangle textureSourceRectangle):
            base(world, texture, shape, rigidBody, rotation, textureSourceRectangle)
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

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            base.OnDraw(spriteBatch, cameraOrigin, viewport);
        }
    }
}
