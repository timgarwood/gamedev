using Box2DX.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Game1.Animations
{
    public class Animation : GameObject
    {
        private AnimationDefinition _definition;
        private Vec2 _position;
        private int _nextFrame;
        private DateTime _lastFrameTime;

        public Animation(Vec2 position, AnimationDefinition definition) :
            // animations don't participate in the physics world
            // they are GameObjects so that they can be drawn
            base(null, null, null, null, 0, null)
        {
            _position = position;
            _definition = definition;
            RenderScale = new Vector2(_definition.Scale, _definition.Scale);
            _nextFrame = 0;
            _lastFrameTime = DateTime.Now;
        }

        public override Vec2 GetWorldPosition()
        {
            return _position;
        }

        public override void Update(GameTime gameTime)
        {
            if(!_definition.Repeat && _nextFrame >= _definition.NumFrames)
            {
                Remove();
            }
        }

        public override void OnDraw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            var srcRect = _definition.FrameRectangles[_nextFrame];

            var texturePosition = new Vector2((_position.X - cameraOrigin.X) * GameData.Instance.PixelsPerMeter,
                (_position.Y - cameraOrigin.Y) * GameData.Instance.PixelsPerMeter);
            spriteBatch.Draw(_definition.Texture, texturePosition, null, srcRect, rotation: 0, scale: RenderScale);

            if (DateTime.Now - _lastFrameTime > TimeSpan.FromSeconds(_definition.FrameDurationSecs))
            {
                ++_nextFrame;
                _lastFrameTime = DateTime.Now;
            }
        }
    }
}
