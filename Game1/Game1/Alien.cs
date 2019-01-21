using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using System;

namespace Game1
{
    public class Alien : GameObject
    {
        /// <summary>
        /// alien definition template
        /// </summary>
        private AlienDefinition _definition;

        /// <summary>
        /// the last time a decision was made
        /// </summary>
        private DateTime _lastDecision = DateTime.MinValue;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="def"></param>
        /// <param name="texture"></param>
        /// <param name="shape"></param>
        /// <param name="rigidBody"></param>
        public Alien(AlienDefinition def, Texture2D texture, Shape shape, Body rigidBody) :
            base(texture, shape, rigidBody)
        {
            _definition = def;
            RenderScale = new Vector2(def.Scale, def.Scale);
        }

        /// <summary>
        /// GameObject update method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (DateTime.UtcNow - _lastDecision > TimeSpan.FromSeconds(_definition.DecisionFrequencySec))
            {
                // track the player, just for now.
                var toPlayer = Player.Instance.GetWorldPosition() - RigidBody.GetPosition();

                var rot = GameUtils.Vec2ToRotation(toPlayer);

                RigidBody.SetXForm(RigidBody.GetPosition(), (float)(rot * System.Math.PI / 180.0f));
                Rotation = (float)(rot * System.Math.PI / 180.0f);

                toPlayer.Normalize();

                _lastDecision = DateTime.UtcNow;

                if (Vec2.Distance(Player.Instance.GetWorldPosition(), RigidBody.GetPosition()) > 1 &&
                    Vec2.Distance(Vec2.Zero, RigidBody.GetLinearVelocity()) < _definition.MaxSpeed)
                {
                    RigidBody.ApplyImpulse(toPlayer * _definition.MoveImpulse, RigidBody.GetPosition());
                }
                /*else if (GameUtils.DistanceFrom(Player.Instance.GetWorldPosition(), RigidBody.GetPosition()) > 1)
                {
                    RigidBody.ApplyImpulse(toPlayer * _definition.MoveImpulse, RigidBody.GetPosition());
                }
                */
                else
                {
                    DecreaseLinearVelocity(_definition.AlienTurnVelocityDecrement, 0);
                }
            }

            base.Update(gameTime);
        }
    }
}
