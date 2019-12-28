using Box2DX.Dynamics;
using Box2DX.Collision;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Box2DX.Common;
using Microsoft.Xna.Framework;
using System;

namespace Game1.Pickups
{
    public class Health : Pickup
    {
        public Health(World _world, 
            Texture2D _texture, 
            Shape _shape, 
            Body _rigidBody,
            int hp,
            float scale) : base(_world, _texture, _shape, _rigidBody, scale)
        {
            Hp = hp;
        }

        public int Hp { get; private set; }
    }
}
