namespace Game1
{
    public class GameData
    {
        public GameData()
        {
            Instance = this;
        }

        public static GameData Instance { get; private set; }

        /// <summary>
        /// this parameter defines how dense the background should be
        /// </summary>
        public int NumBackgroundObjects { get; set; }

        /// <summary>
        /// max x dimension of the game world (meters)
        /// </summary>
        public float MaxXDimension { get; set; }

        /// <summary>
        /// max y dimension of the game world (meters)
        /// </summary>
        public float MaxYDimension { get; set; }

        /// <summary>
        /// starting position of player
        /// </summary>
        public float PlayerStartX { get; set; }
        public float PlayerStartY { get; set; }

        /// <summary>
        /// translation from meters to pixels
        /// </summary>
        private int pixelsPerMeter;
        public int PixelsPerMeter
        {
            get
            {
                return pixelsPerMeter;
            }
            set
            {
                pixelsPerMeter = value;
                MetersPerPixel = 1.0f / pixelsPerMeter;
            }
        }

        /// <summary>
        /// translation from pixels to meters
        /// </summary>
        public float MetersPerPixel { get; private set; }

        /// <summary>
        /// these parameters define how to relate the background distance
        /// to texture scaling size
        /// </summary>
        public float MinBackgroundDistance { get; set; }
        public float MaxBackgroundDistance { get; set; }
        public float MinBackgroundScale { get; set; }
        public float MaxBackgroundScale { get; set; }
    }
}
