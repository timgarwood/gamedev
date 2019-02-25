using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Common;
using NLog;
using Game1.Weapons;
using System;

namespace Game1
{
    /// <summary>
    /// Handles player logic
    /// </summary>
    public class Player : GameObject
    {
        /// <summary>
        /// logger
        /// </summary>
        private Logger Logger = LogManager.GetCurrentClassLogger();

        private Texture2D positionTexture;
        private Texture2D upperBoundTexture;
        private Texture2D lowerBoundTexture;

        private DateTime _lastProjectileTime;

        public static Player Instance { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="positionTexture"></param>
        /// <param name="upperBoundTexture"></param>
        /// <param name="lowerBoundTexture"></param>
        /// <param name="shape"></param>
        /// <param name="rigidBody"></param>
        public Player(World world, Texture2D texture, Texture2D positionTexture, 
            Texture2D upperBoundTexture, Texture2D lowerBoundTexture, 
            Shape shape, Body rigidBody) : 
            base(world, texture, shape, rigidBody, 0, null)
        {
            this.positionTexture = positionTexture;
            this.upperBoundTexture = upperBoundTexture;
            this.lowerBoundTexture = lowerBoundTexture;

            Instance = this;
            _lastProjectileTime = DateTime.MinValue;
        }

        /// <summary>
        /// calculate camera position from players position
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public Vec2 CalculateCamera(Vector2 viewport)
        {
            //figure out where the camera should be using the player position
            return RigidBody.GetPosition() - new Vec2((viewport.X / 2) * GameData.Instance.MetersPerPixel,
                (viewport.Y / 2) * GameData.Instance.MetersPerPixel);
        }

        /// <summary>
        /// player draw routine
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraPosition"></param>
        /// <param name="viewport"></param>
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

            spriteBatch.Draw(Texture, texturePosition, null, null, rotation: angle, origin: new Vector2(Texture.Width / 2, Texture.Height / 2));
            spriteBatch.Draw(positionTexture, bodyPosition);
            spriteBatch.Draw(upperBoundTexture, upperBound);
            spriteBatch.Draw(lowerBoundTexture, lowerBound);
        }

        /// <summary>
        /// update override
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                var cratePosition = RigidBody.GetPosition();
                RigidBody.ApplyImpulse(new Vec2(-GameData.Instance.PlayerImpulse, 0),
                    new Vec2(cratePosition.X, cratePosition.Y));
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                var cratePosition = RigidBody.GetPosition();
                RigidBody.ApplyImpulse(new Vec2(GameData.Instance.PlayerImpulse, 0),
                    new Vec2(cratePosition.X, cratePosition.Y));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                //if (Vec2.Distance(Vec2.Zero, RigidBody.GetLinearVelocity()) < GameData.Instance.PlayerMaxSpeed)
                {
                    var impulseVec = GameUtils.RotationToVec2((float)(RigidBody.GetAngle() * 180 / System.Math.PI));
                    RigidBody.ApplyImpulse(impulseVec * GameData.Instance.PlayerImpulse
                        ,RigidBody.GetPosition());
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                //if (Vec2.Distance(Vec2.Zero, RigidBody.GetLinearVelocity()) < GameData.Instance.PlayerMaxSpeed)
                {
                    var impulseVec = GameUtils.RotationToVec2((float)(RigidBody.GetAngle() * 180 / System.Math.PI));
                    RigidBody.ApplyImpulse(impulseVec * -GameData.Instance.PlayerImpulse
                        ,RigidBody.GetPosition());
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                DecreaseLinearVelocity(GameData.Instance.PlayerTurnVelocityDecrement, 1);
                RigidBody.ApplyTorque(GameData.Instance.PlayerTurnTorque);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                DecreaseLinearVelocity(GameData.Instance.PlayerTurnVelocityDecrement, 1);
                RigidBody.ApplyTorque(-GameData.Instance.PlayerTurnTorque);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (DateTime.Now - _lastProjectileTime > TimeSpan.FromMilliseconds(100))
                {
                    _lastProjectileTime = DateTime.Now;

                    //spawn the projectile just outside the players bounding box in the direction the player is facing.
                    var offset = GameUtils.RotationToVec2((float)(RigidBody.GetAngle() * 180 / System.Math.PI));
                    var offsetLength = GameUtils.PhysicsVec(new Vector2(Texture.Width / 2, Texture.Height / 2));
                    offset = offset * offsetLength.Length();

                    WeaponFactory.Instance.CreateProjectile("GreenLaser-small", RigidBody.GetPosition() + offset, RigidBody.GetAngle());
                }
            }

            if (Keyboard.GetState().IsKeyUp(Keys.W) &&
               Keyboard.GetState().IsKeyUp(Keys.A) &&
               Keyboard.GetState().IsKeyUp(Keys.S) &&
               Keyboard.GetState().IsKeyUp(Keys.D))
            {
                DecreaseLinearVelocity(GameData.Instance.PlayerTurnVelocityDecrement, 0);
            }

            if (Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.D))
            {
                RigidBody.SetAngularVelocity(0);
            }
        }
    }
}
