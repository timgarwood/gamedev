﻿using Microsoft.Xna.Framework.Graphics;
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

        private DateTime _lastProjectileTime;

        public int Hp { get; private set; }

        public int LivesRemaining { get; private set; }

        public int TotalScore { get; set; }

        private static int MaxHp { get; set; } = 100;

        private static int MaxLives { get; set; } = 3;

        private AnimationFactory AnimationFactory { get; set; }

        private WeaponInventory WeaponInventory { get; set; }

        private FilteredKeyListener FilteredInputListener { get; set; }

        private GameWorld GameWorld { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="positionTexture"></param>
        /// <param name="upperBoundTexture"></param>
        /// <param name="lowerBoundTexture"></param>
        /// <param name="shape"></param>
        /// <param name="rigidBody"></param>
        public Player(World world, Texture2D texture, 
            GameWorld gameWorld,
            Shape shape, 
            Body rigidBody, 
            AnimationFactory animationFactory, 
            WeaponInventory weaponInventory, 
            FilteredKeyListener filteredInputListener,
            GameData gameData,
            GameUtils gameUtils) : 
            base(world, texture, shape, rigidBody, 0, gameData, gameUtils)
        {
            Active = true;

            AnimationFactory = animationFactory;

            GameWorld = gameWorld;

            Hp = MaxHp;

            _lastProjectileTime = DateTime.MinValue;

            WeaponInventory = weaponInventory;

            FilteredInputListener = filteredInputListener;

            RenderScale = new Vector2(1, 1);

            LivesRemaining = MaxLives;
        }

        /// <summary>
        /// calculate camera position from players position
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public Vec2 CalculateCamera(Vector2 viewport)
        {
            var halfViewportHeight = viewport.Y * GameData.MetersPerPixel / 2;
            var halfViewportWidth = viewport.X * GameData.MetersPerPixel / 2;
            var cameraTop = RigidBody.GetPosition().Y - halfViewportHeight;
            var cameraLeft = RigidBody.GetPosition().X - halfViewportWidth;

            if(cameraTop < 0)
            {
                cameraTop = 0;
            }

            if(cameraLeft < 0)
            {
                cameraLeft = 0;
            }

            if(cameraTop > (GameData.MaxYDimension - (halfViewportHeight * 2)))
            {
                cameraTop = GameData.MaxYDimension - (halfViewportHeight * 2);
            }

            if(cameraLeft > (GameData.MaxXDimension - (halfViewportWidth * 2)))
            {
                cameraLeft = GameData.MaxXDimension - (halfViewportWidth * 2);
            }

            return new Vec2(cameraLeft, cameraTop);
        }

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraPosition, Vector2 viewport)
        {
            if (Active)
            {
                Rotation = RigidBody.GetAngle();

                //draw player relative to camera
                var texturePosition = new Vector2((RigidBody.GetPosition().X - cameraPosition.X) * GameData.PixelsPerMeter,
                    (RigidBody.GetPosition().Y - cameraPosition.Y) * GameData.PixelsPerMeter);
                var bodyPosition = new Vector2((RigidBody.GetPosition().X - cameraPosition.X) * GameData.PixelsPerMeter,
                    (RigidBody.GetPosition().Y - cameraPosition.Y) * GameData.PixelsPerMeter);
                var upperBound = new Vector2((BoundingBox.UpperBound.X - cameraPosition.X) * GameData.PixelsPerMeter,
                    (BoundingBox.UpperBound.Y - cameraPosition.Y) * GameData.PixelsPerMeter);
                var lowerBound = new Vector2((BoundingBox.LowerBound.X - cameraPosition.X) * GameData.PixelsPerMeter,
                    (BoundingBox.LowerBound.Y - cameraPosition.Y) * GameData.PixelsPerMeter);

                spriteBatch.Draw(Texture, texturePosition, null, null, rotation: Rotation, origin: new Vector2(Texture.Width / 2, Texture.Height / 2));
                DrawShadow(spriteBatch, texturePosition);
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
                    RigidBody.ApplyImpulse(impulseVec * GameData.PlayerLateralImpulse
                        , RigidBody.GetPosition());
                }

                if(Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    //apply impulse to push the player to right 
                    var impulseVec = GameUtils.RotationToVec2((float)rotationDegrees + 90);

                    RigidBody.ApplyImpulse(impulseVec * GameData.PlayerLateralImpulse
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
                    //if (Vec2.Distance(Vec2.Zero, RigidBody.GetLinearVelocity()) < GameData.PlayerMaxSpeed)
                    {
                        var impulseVec = GameUtils.RotationToVec2((float)rotationDegrees);
                        if(Vec2.Dot(impulseVec, RigidBody.GetLinearVelocity()) == 0)
                        {
                            RigidBody.SetLinearVelocity(Vec2.Zero);
                        }
                        RigidBody.ApplyImpulse(impulseVec * GameData.PlayerImpulse
                            , RigidBody.GetPosition());
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    //if (Vec2.Distance(Vec2.Zero, RigidBody.GetLinearVelocity()) < GameData.PlayerMaxSpeed)
                    {
                        var impulseVec = GameUtils.RotationToVec2((float)rotationDegrees);
                        RigidBody.ApplyImpulse(impulseVec * -GameData.PlayerImpulse
                            , RigidBody.GetPosition());
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    //DecreaseLinearVelocity(GameData.PlayerTurnVelocityDecrement, 1);
                    RigidBody.ApplyTorque(GameData.PlayerTurnTorque);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    //DecreaseLinearVelocity(GameData.PlayerTurnVelocityDecrement, 1);
                    RigidBody.ApplyTorque(-GameData.PlayerTurnTorque);
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
                    DecreaseLinearVelocity(GameData.PlayerTurnVelocityDecrement, 0);
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
                    GameWorld.AddGameObject(AnimationFactory.Create(position, "LaserExplosion"));
                    Hp -= proj.Definition.Damage;
                    if (Hp <= 0)
                    {
                        Hp = 0;

                        LivesRemaining--;
                        if(LivesRemaining < 0)
                        {
                            LivesRemaining = 0;
                        }

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
