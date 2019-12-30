using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using System;
using Game1.Weapons;
using Game1.Animations;

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

        private float _lastDistanceToTarget;

        private enum MoveStates
        {
            Moving,
            Stopping,
            Stopped
        }

        private MoveStates MoveState { get; set; } = MoveStates.Stopped;

        /// <summary>
        /// my remaining health
        /// </summary>
        private int Hp { get; set; }

        /// <summary>
        /// animation factory
        /// </summary>
        private AnimationFactory AnimationFactory { get; set; }

        /// <summary>
        /// where i died
        /// </summary>
        private Vec2 DeathLocation { get; set; }

        /// <summary>
        /// the last time i attacked the player
        /// </summary>
        private DateTime LastAttackTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="def"></param>
        /// <param name="texture"></param>
        /// <param name="shape"></param>
        /// <param name="rigidBody"></param>
        public Alien(World world, AlienDefinition def, AnimationFactory animationFactory, Texture2D texture, Shape shape, Body rigidBody) :
            base(world, texture, shape, rigidBody, 0)
        {
            Hp = def.Hp;

            AnimationFactory = animationFactory;

            _definition = def;
            RenderScale = new Vector2(def.Scale, def.Scale);

            //set initial distance to as far from the player as possible
            _lastDistanceToTarget = Vec2.Distance(Vec2.Zero, new Vec2(GameData.Instance.MaxXDimension, GameData.Instance.MaxYDimension));
        }

        public override void Dispose()
        {
            base.Dispose();
            AnimationFactory.Create(DeathLocation, "AlienExplosion");
        }

        private void GoToStopping()
        {
            MoveState = MoveStates.Stopping;
            DecreaseLinearVelocity(RigidBody.GetLinearVelocity().Length() / 5, 0);
            if(RigidBody.GetLinearVelocity().Length() <= 0.05)
            {
                MoveState = MoveStates.Stopped;
            }
        }

        private void GoToMoving(float distToTarget, Vec2 toTarget)
        {
            if (distToTarget > 1)
            {
                MoveState = MoveStates.Moving;
                if (RigidBody.GetLinearVelocity().Length() < _definition.MaxSpeed)
                {
                    RigidBody.ApplyImpulse(toTarget * _definition.MoveImpulse, RigidBody.GetPosition());
                }
            }
        }

        public override void OnCollision(GameObject other, Vec2 position)
        {
            if (!PendingDispose)
            {
                if (other is Projectile)
                {
                    var proj = other as Projectile;
                    if (!proj.PendingDispose)
                    {
                        GameWorld.Instance.AddGameObject(AnimationFactory.Instance.Create(position, "LaserExplosion"));

                        Hp -= proj.Definition.Damage;
                        if (Hp <= 0)
                        {

                            PendingDispose = true;
                            DeathLocation = RigidBody.GetPosition();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// GameObject update method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // track the player, just for now.
            var toTarget = Player.Instance.GetWorldPosition() - RigidBody.GetPosition();
            var distToTarget = Vec2.Distance(Player.Instance.GetWorldPosition(), RigidBody.GetPosition());

            if(distToTarget > 10 && !Active)
            {
                return;
            }

            Active = true;

            var rot = GameUtils.Vec2ToRotation(toTarget);

            RigidBody.SetXForm(RigidBody.GetPosition(), (float)(rot * System.Math.PI / 180.0f));
            Rotation = (float)(rot * System.Math.PI / 180.0f);

            toTarget.Normalize();

            _lastDecision = DateTime.UtcNow;

            var targetVelocity = Player.Instance.RigidBody.GetLinearVelocity();
            var myVelocity = RigidBody.GetLinearVelocity();

            //velocityAngle will tell us if the alien and player are moving in different directions
            //180 degrees means they are moving opposite
            //0 means they are moving in the same direction
            //90, etc..

            //if(velocityAngle > 0)
            //{
            //}

            //getting further away?
            if(_lastDistanceToTarget < distToTarget)
            {
                if (myVelocity.Length() > 1)
                {
                    var velocityAngle = System.Math.Abs(System.Math.Acos(Vec2.Dot(targetVelocity, myVelocity) / (targetVelocity.Length() * myVelocity.Length())));
                    //if we're not going in the same direction, start slowing down
                    if (velocityAngle >= System.Math.PI / (2*60))
                    {
                        //start slowing down so we can pursue the player
                        GoToStopping();
                    }
                    else
                    {
                        GoToMoving(distToTarget, toTarget);
                    }
                }
                else
                {
                    GoToMoving(distToTarget, toTarget);
                }
            }
            else
            {
                //distance hasn't changed
                if(distToTarget > 1)
                {
                    GoToMoving(distToTarget, toTarget);
                }
                else
                {
                    GoToStopping();
                }
            }

            if(distToTarget <= 5)
            {
                var attackDiff = DateTime.UtcNow - LastAttackTime;
                if(attackDiff.TotalMilliseconds >= 500)
                {
                    LastAttackTime = DateTime.UtcNow;
                    SpawnProjectile("GreenLaser-small", ProjectileSource.Alien);
                }
            }

            /*if (distToTarget > 1 && distToTarget <= 3)
            {
                // if the target is stopped
                if (Player.Instance.RigidBody.GetLinearVelocity().Length() < 0.05)
                {
                    if (_moveState == MoveState.Stopping)
                    {
                        GoToStopping();
                    }
                    else
                    {
                        GoToMoving(toTarget);
                    }
                }
                else if (Player.Instance.RigidBody.GetLinearVelocity().Length() > RigidBody.GetLinearVelocity().Length())
                {
                    GoToMoving(toTarget);
                }
            }
            else if (distToTarget > 3 && targetVelocity <= 0.05)
            {
                if (myVelocity >= 0.05)
                {
                    GoToStopping();
                }
                else if(_moveState == MoveState.Stopped)
                {
                    RigidBody.SetLinearVelocity(Vec2.Zero);
                    GoToMoving(toTarget);
                }
            }
            */



            //if we're getting close enough to the player, decrease our impulse
            //if (_lastDistanceToPlayer.Length() > distToPlayer && distToPlayer)
            //{
            //}

            //if (Vec2.Distance(Player.Instance.GetWorldPosition(), RigidBody.GetPosition()) > 1 &&
            //    Vec2.Distance(Vec2.Zero, RigidBody.GetLinearVelocity()) < _definition.MaxSpeed)
            //{
            //    RigidBody.ApplyImpulse(toPlayer * _definition.MoveImpulse, RigidBody.GetPosition());
            // }
            /*else if (GameUtils.DistanceFrom(Player.Instance.GetWorldPosition(), RigidBody.GetPosition()) > 1)
            {
                RigidBody.ApplyImpulse(toPlayer * _definition.MoveImpulse, RigidBody.GetPosition());
            }
            */
            //else
            //{
            //    DecreaseLinearVelocity(_definition.AlienTurnVelocityDecrement, 0);
            //    RigidBody.SetAngularVelocity(0);
            //}

            _lastDistanceToTarget = distToTarget;
            base.Update(gameTime);
        }

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraPosition, Vector2 viewport)
        {
            var angle = RigidBody.GetAngle();

            //draw player relative to camera
            var texturePosition = new Vector2((RigidBody.GetPosition().X - cameraPosition.X) * GameData.Instance.PixelsPerMeter,
                (RigidBody.GetPosition().Y - cameraPosition.Y) * GameData.Instance.PixelsPerMeter);
            var bodyPosition = new Vector2((RigidBody.GetPosition().X - cameraPosition.X) * GameData.Instance.PixelsPerMeter,
                (RigidBody.GetPosition().Y - cameraPosition.Y) * GameData.Instance.PixelsPerMeter);
            var upperBound = new Vector2((BoundingBox.UpperBound.X - cameraPosition.X) * GameData.Instance.PixelsPerMeter,
                (BoundingBox.UpperBound.Y - cameraPosition.Y) * GameData.Instance.PixelsPerMeter);
            var lowerBound = new Vector2((BoundingBox.LowerBound.X - cameraPosition.X) * GameData.Instance.PixelsPerMeter,
                (BoundingBox.LowerBound.Y - cameraPosition.Y) * GameData.Instance.PixelsPerMeter);

            spriteBatch.Draw(Texture, texturePosition, null, null, rotation: angle, scale: RenderScale, origin: RenderScale * new Vector2(Texture.Width / 2, Texture.Height / 2));
            DrawShadow(spriteBatch, texturePosition);
        }
    }
}
