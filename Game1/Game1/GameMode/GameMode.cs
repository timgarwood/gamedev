using Game1.Animations;
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
        protected Player Player { get; set; }

        public GameMode(GameWorld gameWorld, 
            AnimationFactory animationFactory,
            AlienFactory alienFactory, 
            Player player)
        {
            GameWorld = gameWorld;
            AnimationFactory = animationFactory;
            AlienFactory = alienFactory;
            Player = player;
        }

        public abstract void Initialize();

        public abstract GameModeStatus Update(GameTime gameTime);
    }
}
