using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using Microsoft.Xna.Framework;
using NLog;

namespace Game1
{
    /// <summary>
    /// This class represents an object in the game that can be interacted with and drawn to the screen
    /// </summary>
    public class GameObject : Drawable, IUpdateable
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        private Vector2 TextureOffset { get; set; } = Vector2.Zero;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="shape"></param>
        /// <param name="rigidBody"></param>
        public GameObject(World world, Texture2D texture, Shape shape, Body rigidBody, float rotation, Rectangle? textureSourceRectangle) : base(texture)
        {
            Active = true;
            World = world;
            Shape = shape;
            RigidBody = rigidBody;
            Rotation = rotation;
            TextureSourceRectangle = textureSourceRectangle;
            if(TextureSourceRectangle.HasValue)
            {
                TextureOffset = new Vector2(
                    Texture.Width/2 - TextureSourceRectangle.Value.Left
                    ,Texture.Height/2 - TextureSourceRectangle.Value.Top);
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
        }

        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            base.Dispose();
            if(Shape != null)
            {
                RigidBody.DestroyShape(Shape);
            }
            World.DestroyBody(RigidBody);
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
        /// source rectangle in the given texture
        /// </summary>
        public Rectangle? TextureSourceRectangle { get; set; }

        /// <summary>
        /// whether or not the game object is available to interact
        /// with the game
        /// </summary>
        public bool Active { get; set; }

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

        protected Vector2 RenderScale { get; set; }

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

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            var rigidBodyPosition = RigidBody.GetPosition();
            var texturePosition = new Vector2((RigidBody.GetPosition().X - cameraOrigin.X) * GameData.Instance.PixelsPerMeter,
                (RigidBody.GetPosition().Y - cameraOrigin.Y) * GameData.Instance.PixelsPerMeter);
            Logger.Info($"body position @ ({rigidBodyPosition.X},{rigidBodyPosition.Y})");
            Logger.Info($"texture @ ({texturePosition.X},{texturePosition.Y})");
            spriteBatch.Draw(Texture, texturePosition + TextureOffset, null, TextureSourceRectangle, rotation: 0, origin: CenterOfRotation, scale: RenderScale);
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// removes this game object from the game world
        /// </summary>
        protected void Remove()
        {
            Active = false;
            GameWorld.Instance.RemoveGameObject(this);
        }
    }
}
