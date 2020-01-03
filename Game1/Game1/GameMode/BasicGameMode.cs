﻿using Box2DX.Common;
using Game1.Animations;
using Game1.Pickups;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Game1.GameMode
{
    public class BasicGameMode : GameMode
    {
        private GameData GameData { get; set; }

        private int MaxNumWeapons { get; set; } = 10;

        public BasicGameMode(GameWorld gameWorld,
            AnimationFactory animationFactory,
            AlienFactory alienFactory, 
            PickupFactory pickupFactory, 
            Player player,
            GameData gameData) : 
            base(gameWorld, animationFactory, alienFactory, pickupFactory, player)
        {
            GameData = gameData;
        }

        public override void Initialize()
        {
            Spawn();
        }

        public override GameModeStatus Update(GameTime gameTime)
        {
            var disposed = GameWorld.GetAll(x => x.PendingDispose);
            foreach(var d in disposed)
            {
                GameWorld.RemoveGameObject(d);
            }

            var remainingAliens = GameWorld.GetGameObjects<Alien>();
            if(remainingAliens.Count <= 0)
            {
                return GameModeStatus.Success;
            }

            if(Player.Hp <= 0)
            {
                return GameModeStatus.Failed;
            }

            Player.Update(gameTime);
            GameWorld.Update(gameTime);

            return GameModeStatus.Continue;
        }

        private void Spawn()
        {
            var numAliens = 10;
            for (var i = 0; i < numAliens; ++i)
            {
                AlienFactory.Create("Alien1");
                Thread.Sleep(100);
            }

            var numHealths = 10;

            for(var i = 0; i < numHealths; ++i)
            {
                var rand = new System.Random((int)(System.DateTime.UtcNow - System.DateTime.MinValue).Ticks);
                PickupFactory.CreateHealthPickup(new Vec2(rand.Next(0, (int)GameData.MaxXDimension), rand.Next(0, (int)GameData.MaxYDimension)), "SmallHealth");
                Thread.Sleep(100);
            }

            SpawnLaserPickup(2, "RedLaser");
            SpawnLaserPickup(2, "YellowLaser");
            SpawnLaserPickup(2, "TealLaser");
            SpawnLaserPickup(2, "PurpleLaser");
            SpawnLaserPickup(2, "BlueLaser");
        }

        private void SpawnLaserPickup(int numLasers, string definitionName)
        {
            for(var i = 0; i < numLasers; ++i)
            {
                var rand = new System.Random((int)(System.DateTime.UtcNow - System.DateTime.MinValue).Ticks);
                PickupFactory.CreateLaserPickup(new Vec2(rand.Next(0, (int)GameData.MaxXDimension), rand.Next(0, (int)GameData.MaxYDimension)), definitionName);
                Thread.Sleep(100);
            }
        }
    }
}
