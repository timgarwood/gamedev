using Box2DX.Collision;
using Box2DX.Dynamics;
using Game1.Animations;
using Game1.Weapons;
using NLog;

namespace Game1.Physics
{
    public class GameContactListener : ContactListener
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public override void Add(ContactPoint point)
        {
            base.Add(point);
        }

        public override void Persist(ContactPoint point)
        {
            base.Persist(point);
        }

        public override void Remove(ContactPoint point)
        {
            base.Remove(point);
        }

        private void ExtractCollisionData(ContactResult point, out Projectile proj, out Alien alien, out Player player)
        {
            proj = null;
            alien = null;
            player = null;
            var shapes = new Shape[] { point.Shape1, point.Shape2 };
            foreach (var shape in shapes)
            {
                if (shape.UserData is Projectile && proj == null)
                {
                    proj = shape.UserData as Projectile;
                }
                else if (shape.UserData is Alien)
                {
                    alien = shape.UserData as Alien;
                }
                else if (shape.UserData is Player)
                {
                    player = shape.UserData as Player;
                }
            }
        }

        public override void Result(ContactResult point)
        {
            Projectile proj = null;
            Alien alien = null;
            Player player = null;
            ExtractCollisionData(point, out proj, out alien, out player);

            if (proj != null)
            {
                if (proj.Active)
                {
                    if (alien != null)
                    {
                        Logger.Info("projectile collided with alien");
                        GameWorld.Instance.RemoveGameObject(proj);
                        GameWorld.Instance.AddGameObject(AnimationFactory.Instance.Create(point.Position, "LaserExplosion"));

                        alien.OnCollision(proj);
                    }
                    else if (player != null)
                    {
                        Logger.Info("projectile collided with player");
                        GameWorld.Instance.RemoveGameObject(proj);
                        GameWorld.Instance.AddGameObject(AnimationFactory.Instance.Create(point.Position, "LaserExplosion"));

                        player.OnCollision(proj);
                    }
                }
            }
            else
            {
                base.Result(point);
            }
        }
    }

}
