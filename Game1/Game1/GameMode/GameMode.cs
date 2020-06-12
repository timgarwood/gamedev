using Game1.Animations;
using Game1.Pickups;
using Microsoft.Xna.Framework;

namespace Game1.GameMode
{
    public enum GameModeStatus
    {
        Continue,
        Success,
        Failed
    }

    public abstract class GameMode
    {
        protected GameWorld GameWorld { get; set; }
        protected AnimationFactory AnimationFactory { get; set; }

        protected AlienFactory AlienFactory { get; set; }

        protected PickupFactory PickupFactory { get; set; }

        protected Player Player { get; set; }

        public GameMode(GameWorld gameWorld, 
            AnimationFactory animationFactory,
            AlienFactory alienFactory, 
            PickupFactory pickupFactory,
            Player player)
        {
            GameWorld = gameWorld;
            AnimationFactory = animationFactory;
            AlienFactory = alienFactory;
            PickupFactory = pickupFactory;
            Player = player;
        }

        public abstract void SetUpForNewGame();

        public abstract void Spawn();

        public abstract GameModeStatus Update(GameTime gameTime);
    }
}
