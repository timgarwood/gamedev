using Box2DX.Dynamics;
using Box2DX.Collision;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Box2DX.Common;
using Microsoft.Xna.Framework;
using System;

namespace Game1.Pickups
{
    public class Health : GameObject
    {
        private TimeSpan RotateTime { get; set; } = TimeSpan.FromMilliseconds(100);

        private TimeSpan LastUpdateTime { get; set; } = TimeSpan.FromSeconds(0);

        public Health(World _world, 
            Texture2D _texture, 
            Shape _shape, 
            Body _rigidBody,
            int hp,
            float scale) : base(_world, _texture, _shape, _rigidBody, 0)
        {
            Hp = hp;
            RenderScale = new Vector2(scale, scale);
        }

        public override void OnCollision(GameObject other, Vec2 position)
        {
            // i can only collide with the player, so just dispose me.
            PendingDispose = true;
        }

        public override void Update(GameTime gameTime)
        {
            if(gameTime.TotalGameTime - LastUpdateTime > RotateTime)
            {
                LastUpdateTime = gameTime.TotalGameTime;
                RotateByDegrees(5);
            }
        }

        public int Hp { get; private set; }
    }
}
