namespace Game1.Fonts
{
    public class FontDefinition
    {
        /// <summary>
        /// name of the font
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// name of the texture asset
        /// </summary>
        public string TextureAsset { get; set; }

        /// <summary>
        /// width of a character in pixels
        /// </summary>
        public uint CharacterWidth { get; set; }

        /// <summary>
        /// height of a character in pixels
        /// </summary>
        public uint CharacterHeight { get; set; }

        /// <summary>
        /// how much to scale each character by
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// whether or not this font is case sensitive
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// list of character definitions
        /// </summary>
        public CharacterDefinition[] Characters { get; set; }
    }
}
