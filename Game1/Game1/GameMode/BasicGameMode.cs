using Game1.Animations;
using Game1.Pickups;
using Microsoft.Xna.Framework;
using System.Threading;
using System;

namespace Game1.GameMode
{
    public class BasicGameMode : GameMode
    {
        private GameData GameData { get; set; }

        private static string[] LaserNames = new string[]
        {
            "RedLaser",
            "YellowLaser",
            "TealLaser",
            "PurpleLaser",
            "BlueLaser"
        };

        private static string[] AlienNames = new string[]
        {
            "Alien1",
            "Alien2",
            "Alien3",
            "Alien4",
            "Alien5",
            "Alien6",
            "Alien8"
        };

        private TimeSpan TimeOfLastWeaponSpawn { get; set; }
        
        /// <summary>
        /// how often to spawn weapons
        /// </summary>
        private TimeSpan WeaponSpawnFrequency { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// hack to set TimeOfLastWeaponSpawn
        /// </summary>
        private bool FirstFrame { get; set; } = true;

        private bool FirstSpawn { get; set; } = true;

        /// <summary>
        /// max number of laser pickups at one time
        /// </summary>
        private static int MaxLasers { get; set; } = 10;
        /// <summary>
        /// max number of healths at one time
        /// </summary>
        private static int MaxHealths { get; set; } = 10;
        /// <summary>
        /// max number of aliens at one time
        /// </summary>
        private static int MaxAliens { get; set; } = 10;

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
            FirstFrame = true;
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

            var remainingWeapons = GameWorld.GetGameObjects<Laser>();
            if(remainingWeapons.Count < MaxLasers)
            {
                if(FirstFrame)
                {
                    FirstFrame = false;
                    TimeOfLastWeaponSpawn = gameTime.TotalGameTime;
                }

                if(gameTime.TotalGameTime - TimeOfLastWeaponSpawn > WeaponSpawnFrequency)
                {
                    var rand = new Random((int)gameTime.TotalGameTime.TotalMilliseconds);
                    TimeOfLastWeaponSpawn = gameTime.TotalGameTime;
                    for(var i = 0; i < MaxLasers - remainingWeapons.Count; ++i)
                    {
                        var laserIndex = rand.Next(0, LaserNames.Length);
                        PickupFactory.CreateLaserPickup(LaserNames[laserIndex]);
                    }
                }
            }
            else
            {
                TimeOfLastWeaponSpawn = gameTime.TotalGameTime;
            }

            Player.Update(gameTime);
            GameWorld.Update(gameTime);

            return GameModeStatus.Continue;
        }

        private void Spawn()
        {
            var aliens = GameWorld.GetGameObjects<Alien>();
            var lasers = GameWorld.GetGameObjects<Laser>();

            var rand = new Random();
            for (var i = 0; i < MaxAliens - aliens.Count; ++i)
            {
                var alienIndex = rand.Next(0, AlienNames.Length);
                AlienFactory.Create(AlienNames[alienIndex]);
                Thread.Sleep(100);
            }

            if (FirstSpawn)
            {
                for (var i = 0; i < MaxHealths; ++i)
                {
                    PickupFactory.CreateHealthPickup("SmallHealth");
                    Thread.Sleep(100);
                }
            }

            for(var i = 0; i < MaxLasers - lasers.Count; ++i)
            {
                var laserIndex = rand.Next(0, LaserNames.Length);
                SpawnLaserPickup(1, LaserNames[laserIndex]);
                Thread.Sleep(100);
            }

            FirstSpawn = false;
        }

        private void SpawnLaserPickup(int numLasers, string definitionName)
        {
            for(var i = 0; i < numLasers; ++i)
            {
                PickupFactory.CreateLaserPickup(definitionName);
                Thread.Sleep(100);
            }
        }
    }
}
