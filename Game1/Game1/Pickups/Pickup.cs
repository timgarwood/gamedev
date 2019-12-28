using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Pickups
{
    public abstract class Pickup : GameObject
    {
        private TimeSpan RotateTime { get; set; } = TimeSpan.FromMilliseconds(10);

        private TimeSpan LastUpdateTime { get; set; } = TimeSpan.FromSeconds(0);

        public Pickup(World world, Texture2D texture, Shape shape, Body rigidBody, float scale) :
            base(world, texture, shape, rigidBody, 0)
        {
            RenderScale = new Vector2(scale, scale);
        }

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            base.OnDraw(spriteBatch, cameraOrigin, viewport);
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - LastUpdateTime > RotateTime)
            {
                LastUpdateTime = gameTime.TotalGameTime;
                RotateByDegrees(1);
            }
        }

        public override void OnCollision(GameObject other, Vec2 position)
        {
            // i can only collide with the player, so just dispose me.
            PendingDispose = true;
        }
    }
}
