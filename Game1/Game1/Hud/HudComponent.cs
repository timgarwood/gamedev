using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Hud
{
    public abstract class HudComponent
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
            if(definition.Width <= 0 || definition.Height <= 0)
            {
                throw new System.Exception("HudComponent needs a width and height");
            }

        }

        public virtual void OnWindowResized(Vector2 viewport)
        {
            _viewport = viewport;
        }

        /// <summary>
        /// calculates the leftmost pixel of this hud component
        /// </summary>
        protected float Left
        {
            get
            {
                if (_hudComponentDefinition.HorizontalAlignment == HorizontalAlignment.Left)
                {
                    return _viewport.X * _hudComponentDefinition.HorizontalPercentage;
                }
                else if (_hudComponentDefinition.HorizontalAlignment == HorizontalAlignment.Right)
                {
                    return (_viewport.X - _hudComponentDefinition.Width) * (1 - _hudComponentDefinition.HorizontalPercentage);
                }
                else
                {
                    return _viewport.X * .5f - (_hudComponentDefinition.Width / 2);
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
                if (_hudComponentDefinition.VerticalAlignment == VerticalAlignment.Top)
                {
                    return _viewport.Y * _hudComponentDefinition.VerticalPercentage;
                }
                else if (_hudComponentDefinition.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return (_viewport.Y - _hudComponentDefinition.Height) * (1 - _hudComponentDefinition.VerticalPercentage);
                }
                else
                {
                    return _viewport.Y * .5f - (_hudComponentDefinition.Height / 2);
                }
            }
        }

        /// <summary>
        /// type of this component (from json data)
        /// </summary>
        public string ComponentType { get; set; }

        /// <summary>
        /// abstract draw method
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="viewport"></param>
        public abstract void Draw(SpriteBatch spriteBatch, Vector2 viewport);
    }
}
