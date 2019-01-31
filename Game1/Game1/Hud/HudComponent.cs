using Box2DX.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Hud
{
    public abstract class HudComponent : IDrawable
    {
        /// <summary>
        /// the base hub component definition
        /// </summary>
        private HudComponentDefinition _hudComponentDefinition;

        /// <summary>
        /// the viewport dimensions
        /// </summary>
        public Vector2 _viewport;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="definition"></param>
        public HudComponent(HudComponentDefinition definition)
        {
            _hudComponentDefinition = definition;
        }

        public void OnWindowResized(Vector2 viewport)
        {
            _viewport = viewport;
        }

        public void Draw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
        }

        /// <summary>
        /// calculates the leftmost pixel of this hud component
        /// </summary>
        protected float Left
        {
            get
            {
                if(_hudComponentDefinition.HorizontalAlignment == HorizontalAlignment.Left)
                {
                    return _viewport.X * _hudComponentDefinition.HorizontalPercentage;
                }
                else if(_hudComponentDefinition.HorizontalAlignment == HorizontalAlignment.Right)
                {
                    return _viewport.X * (1 - _hudComponentDefinition.HorizontalPercentage);
                }
                else
                {
                    return _viewport.X * .5f;
                }
            }
        }

        /// <summary>
        /// calculates the top-most pixel of this hud component
        /// </summary>
        protected float Top 
        {
            get
            {
                if(_hudComponentDefinition.VerticalAlignment == VerticalAlignment.Top)
                {
                    return _viewport.Y * _hudComponentDefinition.VerticalPercentage;
                }
                else if (_hudComponentDefinition.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return _viewport.Y * (1 - _hudComponentDefinition.VerticalPercentage);
                }
                else
                {
                    return _viewport.Y * .5f;
                }
            }
        }

        /// <summary>
        /// type of this component (from json data)
        /// </summary>
        public string ComponentType { get; set; }
    }
}
