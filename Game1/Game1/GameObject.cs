using Microsoft.Xna.Framework.Graphics;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using Microsoft.Xna.Framework;

namespace Game1
{
    /// <summary>
    /// This class represents an object in the game that can be interacted with and drawn to the screen
    /// </summary>
    public class GameObject : Drawable, IUpdateable
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="shape"></param>
        /// <param name="rigidBody"></param>
        public GameObject(Texture2D texture, Shape shape, Body rigidBody) : base(texture)
        {
            Shape = shape;
            RigidBody = rigidBody;
            if (texture != null)
            {
                RenderOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            }
        }

        /// <summary>
        /// physics body
        /// </summary>
        public Body RigidBody { get; private set; }

        /// <summary>
        /// physics shape
        /// </summary>
        public Shape Shape { get; private set; }

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

        protected Vector2 RenderOrigin { get; set; }

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
            var texturePosition = new Vector2((RigidBody.GetPosition().X - cameraOrigin.X) * GameData.Instance.PixelsPerMeter,
                (RigidBody.GetPosition().Y - cameraOrigin.Y) * GameData.Instance.PixelsPerMeter);
            spriteBatch.Draw(Texture, texturePosition, null, null, rotation: Rotation, origin: RenderOrigin, scale: RenderScale);
        }

        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
