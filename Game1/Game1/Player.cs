using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Common;
using NLog;
using Game1.Weapons;
using System;
using Game1.Animations;
using Game1.Pickups;

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

        private Texture2D _positionTexture;
        private Texture2D _upperBoundTexture;
        private Texture2D _lowerBoundTexture;

        private DateTime _lastProjectileTime;

        public int Hp { get; private set; }

        public static Player Instance { get; private set; }

        private static int MaxHp { get; set; } = 100;

        private AnimationFactory AnimationFactory { get; set; }

        private WeaponInventory WeaponInventory { get; set; }

        private FilteredKeyListener FilteredInputListener { get; set; }

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
            Shape shape, Body rigidBody, AnimationFactory animationFactory, WeaponInventory weaponInventory, FilteredKeyListener filteredInputListener) : 
            base(world, texture, shape, rigidBody, 0)
        {
            Active = true;

            _positionTexture = positionTexture;
            _upperBoundTexture = upperBoundTexture;
            _lowerBoundTexture = lowerBoundTexture;

            AnimationFactory = animationFactory;

            Hp = MaxHp;

            Instance = this;
            _lastProjectileTime = DateTime.MinValue;

            WeaponInventory = weaponInventory;

            FilteredInputListener = filteredInputListener;
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
        [Obsolete]
        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraPosition, Vector2 viewport)
        {
            if (Active)
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
                spriteBatch.Draw(_positionTexture, bodyPosition);
                spriteBatch.Draw(_upperBoundTexture, upperBound);
                spriteBatch.Draw(_lowerBoundTexture, lowerBound);
            }
        }

        /// <summary>
        /// update override
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                FilteredInputListener.Update(gameTime);

                //if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                /*if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    var cratePosition = RigidBody.GetPosition();
                    RigidBody.ApplyImpulse(new Vec2(-GameData.Instance.PlayerImpulse, 0),
                        new Vec2(cratePosition.X, cratePosition.Y));
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    var cratePosition = RigidBody.GetPosition();
                    RigidBody.ApplyImpulse(new Vec2(GameData.Instance.PlayerImpulse, 0),
                        new Vec2(cratePosition.X, cratePosition.Y));
                }
                */

                var rotationDegrees = RigidBody.GetAngle() * 180 / System.Math.PI;

                if(Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    //apply impulse to push the player to left
                    var impulseVec = GameUtils.RotationToVec2((float)rotationDegrees - 90);
                    RigidBody.ApplyImpulse(impulseVec * GameData.Instance.PlayerLateralImpulse
                        , RigidBody.GetPosition());
                }

                if(Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    //apply impulse to push the player to right 
                    var impulseVec = GameUtils.RotationToVec2((float)rotationDegrees + 90);

                    RigidBody.ApplyImpulse(impulseVec * GameData.Instance.PlayerLateralImpulse
                        , RigidBody.GetPosition());
                }

                if(FilteredInputListener.WasKeyPressed(Keys.OemOpenBrackets))
                {
                    WeaponInventory.SelectPreviousWeapon();
                    FilteredInputListener.ResetKey(Keys.OemOpenBrackets);
                }

                if(FilteredInputListener.WasKeyPressed(Keys.OemCloseBrackets))
                {
                    WeaponInventory.SelectNextWeapon();
                    FilteredInputListener.ResetKey(Keys.OemCloseBrackets);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    //if (Vec2.Distance(Vec2.Zero, RigidBody.GetLinearVelocity()) < GameData.Instance.PlayerMaxSpeed)
                    {
                        var impulseVec = GameUtils.RotationToVec2((float)rotationDegrees);
                        if(Vec2.Dot(impulseVec, RigidBody.GetLinearVelocity()) == 0)
                        {
                            RigidBody.SetLinearVelocity(Vec2.Zero);
                        }
                        RigidBody.ApplyImpulse(impulseVec * GameData.Instance.PlayerImpulse
                            , RigidBody.GetPosition());
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    //if (Vec2.Distance(Vec2.Zero, RigidBody.GetLinearVelocity()) < GameData.Instance.PlayerMaxSpeed)
                    {
                        var impulseVec = GameUtils.RotationToVec2((float)rotationDegrees);
                        RigidBody.ApplyImpulse(impulseVec * -GameData.Instance.PlayerImpulse
                            , RigidBody.GetPosition());
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    //DecreaseLinearVelocity(GameData.Instance.PlayerTurnVelocityDecrement, 1);
                    RigidBody.ApplyTorque(GameData.Instance.PlayerTurnTorque);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    //DecreaseLinearVelocity(GameData.Instance.PlayerTurnVelocityDecrement, 1);
                    RigidBody.ApplyTorque(-GameData.Instance.PlayerTurnTorque);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    var weapon = WeaponInventory.GetSelectedWeapon();
                    if (weapon != null && weapon.RemainingAmmo > 0)
                    {
                        if (DateTime.Now - _lastProjectileTime > TimeSpan.FromMilliseconds(100))
                        {
                            _lastProjectileTime = DateTime.Now;
                            WeaponInventory.DecreaseAmmo(1);
                            SpawnProjectile(weapon.ProjectileName, ProjectileSource.Player);
                        }
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

        public override void OnCollision(GameObject other, Vec2 position)
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
                        Hp = 0;
                        if (Active)
                        {
                            AnimationFactory.Create(RigidBody.GetPosition(), "AlienExplosion");
                        }

                        Active = false;
                    }
                }
            }
            else if (other is Health)
            {
                var health = other as Health;
                Hp += health.Hp;
                if (Hp >= MaxHp)
                {
                    Hp = MaxHp;
                }
            }
            else if(other is Laser)
            {
                var laser = other as Laser;
                WeaponInventory.AddToInventory(laser);
            }
        }

        public void Reset()
        {
            Active = true;
            Hp = MaxHp;
        }
    }
}
