using Box2DX.Common;
using Game1.Animations;
using Microsoft.Xna.Framework;

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
            var rand = new System.Random((int)(System.DateTime.UtcNow - System.DateTime.MinValue).TotalMilliseconds);
            var alien = AlienFactory.Create("Alien1", new Vec2(rand.Next(40, 45), rand.Next(40, 45)));
            alien.Death += OnAlienDeath;
        }

        public override GameModeStatus Update(GameTime gameTime)
        {
            Player.Update(gameTime);
            GameWorld.Update(gameTime);

            return GameModeStatus.Continue;
        }

        public void OnAlienDeath(object sender, AlienDeathEventArgs args)
        {
            var alien = sender as Alien;
            alien.Dispose();
            GameWorld.RemoveGameObject(alien);
            AnimationFactory.Create(args.Location, "AlienExplosion");
        }
    }
}
