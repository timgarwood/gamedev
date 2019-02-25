using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game1.Animations
{
    public class AnimationDefinition
    {
        public string Name { get; set; }

        public string SpriteSheet { get; set; }

        public Texture2D Texture { get; set; }

        public Rectangle[] FrameRectangles { get; set; }

        public int FrameWidth { get; set; }

        public int FrameHeight { get; set; }

        public int NumFrames { get; set; }

        public bool Repeat { get; set; }

        public float FrameDurationSecs { get; set; }

        public float Scale { get; set; }
    }
}
