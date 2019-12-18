using Box2DX.Common;
using Game1.Animations;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Game1.GameMode
{
    public class BasicGameMode : GameMode
    {
        public BasicGameMode(GameWorld gameWorld,
            AnimationFactory animationFactory,
            AlienFactory alienFactory, 
            Player player) : 
            base(gameWorld, animationFactory, alienFactory, player)
        {

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
            var numAliens = GameWorld.GetGameObjects<Alien>().Count;
            var diff = 75 - numAliens;
            for (var i = 0; i < diff; ++i)
            {
                var rand = new System.Random((int)(System.DateTime.UtcNow - System.DateTime.MinValue).Ticks);
                AlienFactory.Create("Alien1", new Vec2(rand.Next(0, (int)GameData.Instance.MaxXDimension), rand.Next(0, (int)GameData.Instance.MaxYDimension)));
                Thread.Sleep(100);
            }
        }
    }
}
