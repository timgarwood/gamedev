using Microsoft.Xna.Framework;

namespace Game1.Fonts
{
    public class CharacterDefinition
    {
        public char Character { get; set; }

        public uint? TopLeftX { get; set; }

        public uint? TopLeftY { get; set; }

        /// <summary>
        /// rectangle within the texture
        /// </summary>
        public Rectangle SourceRectangle { get; set; }
    }
}
