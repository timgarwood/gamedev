using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using Microsoft.Xna.Framework;
using NLog;
using Game1.Weapons;
using Color = Microsoft.Xna.Framework.Color;

namespace Game1
{
    /// <summary>
    /// This class represents an object in the game that can be interacted with and drawn to the screen
    /// </summary>
    public class GameObject : Drawable, IUpdateable
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        private Vector2 ShadowOffset { get; set; } = new Vector2(0, 0);
        private static Vector2 ShadowScale { get; set; } = new Vector2(.4f, .4f);

        private Vector2 TextureOffset { get; set; } = Vector2.Zero;

        /// <summary>
        /// does this game object need to be removed on the next frame?
        /// </summary>
        public bool PendingDispose { get; protected set; }

        protected GameData GameData { get; set; }

        protected GameUtils GameUtils { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="shape"></param>
        /// <param name="rigidBody"></param>
        public GameObject(World world, 
            Texture2D texture, 
            Shape shape, 
            Body rigidBody, 
            float rotation, 
            GameData gameData, 
            GameUtils gameUtils) : base(texture)
        {
            World = world;
            Shape = shape;
            RigidBody = rigidBody;
            Rotation = rotation;
            if (Shape != null)
            {
                Shape.UserData = this;
            }

            /*if(TextureSourceRectangle.HasValue)
            {
                CenterOfRotation = new Vector2(
                    TextureSourceRectangle.Value.Left + TextureSourceRectangle.Value.Width/2
                    , TextureSourceRectangle.Value.Top + TextureSourceRectangle.Value.Height/2);
            }
            */
            if (texture != null)
            {
                CenterOfRotation = new Vector2(texture.Width / 2, texture.Height / 2);
            }

            if (Texture != null)
            {
                ShadowOffset = new Vector2(-20, 20);
            }

            GameData = gameData;
            GameUtils = gameUtils;
        }

        /// <summary>
        /// dispose
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if(RigidBody != null && Shape != null)
            {
                RigidBody.DestroyShape(Shape);
            }
            if(World != null && RigidBody != null)
            {
               World.DestroyBody(RigidBody);
            }

            RigidBody = null;
            Shape = null;
        }

        /// <summary>
        /// the physics world
        /// </summary>
        protected World World { get; private set; }

        /// <summary>
        /// physics body
        /// </summary>
        public Body RigidBody { get; private set; }

        /// <summary>
        /// physics shape
        /// </summary>
        public Shape Shape { get; private set; }

        /// <summary>
        /// whether or not the game object is available to interact
        /// with the game
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        /// physics bounding box
        /// </summary>
        public AABB BoundingBox
        {
            get
            {
                AABB boundingBox;
                Shape.ComputeAABB(out boundingBox, RigidBody.GetXForm());
                return boundingBox;
            }
        }

        public override Vec2 GetWorldPosition()
        {
            return RigidBody.GetPosition();
        }

        protected Vector2 CenterOfRotation { get; set; }

        protected float Rotation { get; set; } = 0.0f;

        protected void DecreaseLinearVelocity(float step, float min)
        {
            var lv = RigidBody.GetLinearVelocity();
            if (System.Math.Abs(lv.X) > min)
            {
                if (lv.X > 0)
                {
                    lv.X -= step;
                }
                else
                {
                    lv.X += step;
                }

                if(System.Math.Abs(lv.X) < min)
                {
                    lv.X = min;
                }
            }

            if (System.Math.Abs(lv.Y) > min)
            {
                if (lv.Y > 0)
                {
                    lv.Y -= step;
                }
                else
                {
                    lv.Y += step;
                }

                if (System.Math.Abs(lv.Y) < min)
                {
                    lv.Y = min;
                }
            }

            RigidBody.SetLinearVelocity(lv);
        }

        protected void DrawShadow(SpriteBatch spriteBatch, Vector2 texturePosition)
        {
            var shadowLocation = new Vector2(texturePosition.X + ShadowOffset.X, texturePosition.Y + (Texture.Height * RenderScale.Y) + ShadowOffset.Y);
            spriteBatch.Draw(Texture, shadowLocation, null, null, rotation: Rotation, color: Color.Black, origin: CenterOfRotation, scale: ShadowScale * RenderScale);
        }

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            if (Texture != null)
            {
                var rigidBodyPosition = RigidBody.GetPosition();
                var texturePosition = new Vector2((RigidBody.GetPosition().X - cameraOrigin.X) * GameData.PixelsPerMeter,
                    (RigidBody.GetPosition().Y - cameraOrigin.Y) * GameData.PixelsPerMeter);
                //Logger.Info($"body position @ ({rigidBodyPosition.X},{rigidBodyPosition.Y})");
                //Logger.Info($"texture @ ({texturePosition.X},{texturePosition.Y})");
                spriteBatch.Draw(Texture, texturePosition, null, null, rotation: Rotation, origin: CenterOfRotation, scale: RenderScale);
                DrawShadow(spriteBatch, texturePosition);
            }
        }

        private void ClipRotation()
        {
            if (Rotation >= 2 * System.Math.PI)
            {
                Rotation = (Rotation - (float)(2 * System.Math.PI));
            }
        }

        public virtual void RotateByRadians(float radians)
        {
            Rotation += radians;
            ClipRotation();
        }

        public virtual void RotateByDegrees(int degrees)
        {
            Rotation += (float)(degrees * System.Math.PI / 180);
            ClipRotation();
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void OnCollision(GameObject other, Vec2 position)
        {
        }

        protected void SpawnProjectile(string projectileName, ProjectileSource source)
        {
            var slop = 20;
            //spawn the projectile just outside the players bounding box in the direction the player is facing.
            var offset = GameUtils.RotationToVec2((float)(RigidBody.GetAngle() * 180 / System.Math.PI));
            var offsetLength = GameUtils.PhysicsVec(new Vector2(0, (Texture.Height + slop) / 2));
            offset = offset * offsetLength.Length();

            WeaponFactory.Instance.CreateProjectile(projectileName, RigidBody.GetPosition() + offset, RigidBody.GetAngle(), source);
        }
    }
}
