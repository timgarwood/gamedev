using Box2DX.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Hud
{
    public class Minimap : HudComponent, IDrawable
    {
        private MinimapDefinition _definition;
        private Texture2D _backgroundTexture;
        private Texture2D _alienTexture;
        private Vector2 _destPoint;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="definition"></param>
        public Minimap(Texture2D backgroundTexture, Texture2D alienTexture, MinimapDefinition definition) :
            base(definition)
        {
            _backgroundTexture = backgroundTexture;
            _alienTexture = alienTexture;
            _definition = definition;
            _destPoint = new Vector2();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraOrigin"></param>
        /// <param name="viewport"></param>
        public void Draw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            _destPoint.X = (int) Left;
            _destPoint.Y = (int) Top;

            if (_backgroundTexture != null)
            {
                spriteBatch.Draw(_backgroundTexture, _destPoint);
            }

            var aliens = GameWorld.Instance.GetGameObjects<Alien>();
            foreach(var alien in aliens)
            {
                var alienPosition = alien.GetWorldPosition();
                var minimapPosition = new Vector2(_destPoint.X + (alienPosition.X * _backgroundTexture.Width / GameData.Instance.MaxXDimension),
                    _destPoint.Y + (alienPosition.Y * _backgroundTexture.Height / GameData.Instance.MaxYDimension));
                spriteBatch.Draw(_alienTexture, minimapPosition);
            }
        }
    }
}
