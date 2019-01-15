using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Common;
using NLog;

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

        /// <summary>
        /// torque to apply when turning
        /// </summary>
        private static float TurnTorque = 0.003f;

        /// <summary>
        /// max impulse to apply when moving forward or backward
        /// </summary>
        private static float MaxImpulse = 0.0045f;

        private Texture2D positionTexture;
        private Texture2D upperBoundTexture;
        private Texture2D lowerBoundTexture;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="positionTexture"></param>
        /// <param name="upperBoundTexture"></param>
        /// <param name="lowerBoundTexture"></param>
        /// <param name="shape"></param>
        /// <param name="rigidBody"></param>
        public Player(Texture2D texture, Texture2D positionTexture, 
            Texture2D upperBoundTexture, Texture2D lowerBoundTexture, 
            Shape shape, Body rigidBody) : 
            base(texture, shape, rigidBody)
        {
            this.positionTexture = positionTexture;
            this.upperBoundTexture = upperBoundTexture;
            this.lowerBoundTexture = lowerBoundTexture;
        }

        /// <summary>
        /// Handles input. Called every frame.
        /// </summary>
        public void HandleInput()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                var cratePosition = RigidBody.GetPosition();
                RigidBody.ApplyImpulse(new Vec2(-.0025f, 0), 
                    new Vec2(cratePosition.X, cratePosition.Y));
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                var cratePosition = RigidBody.GetPosition();
                RigidBody.ApplyImpulse(new Vec2(.0025f, 0), 
                    new Vec2(cratePosition.X, cratePosition.Y));
            }

            var degrees = (RigidBody.GetAngle() * 180 / System.Math.PI) % 360;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                var impulseVec = GameUtils.RotationToVec2((float)(RigidBody.GetAngle() * 180 / System.Math.PI));
                Logger.Info($"impulse = ({impulseVec.X},{impulseVec.Y})");
                RigidBody.ApplyImpulse(new Vec2(impulseVec.X * MaxImpulse, 
                    impulseVec.Y * MaxImpulse), 
                    RigidBody.GetPosition());
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                var impulseVec = GameUtils.RotationToVec2((float)(RigidBody.GetAngle() * 180 / System.Math.PI));
                Logger.Info($"impulse = ({impulseVec.X},{impulseVec.Y})");
                RigidBody.ApplyImpulse(new Vec2(-impulseVec.X * MaxImpulse, 
                    -impulseVec.Y * MaxImpulse), 
                    RigidBody.GetPosition());
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                RigidBody.ApplyTorque(TurnTorque);
                Logger.Info(RigidBody.GetAngle());
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                RigidBody.ApplyTorque(-TurnTorque);
                Logger.Info(RigidBody.GetAngle());
            }
            if (Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.D))
            {
                RigidBody.SetAngularVelocity(0);
            }
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
    }
}
