using System;

namespace Game1.Hud
{
    public class HudComponentDefinition
    {
        public HorizontalAlignment HorizontalAlignment;

        public VerticalAlignment VerticalAlignment;

        public string HAlign
        {
            set
            {
                Enum.TryParse<HorizontalAlignment>(value, out HorizontalAlignment);
            }
        }

        public string VAlign
        {
            set
            {
                Enum.TryParse<VerticalAlignment>(value, out VerticalAlignment);
            }
        }

        /// <summary>
        /// amount to shift the component by from where it is horizontally justified
        /// </summary>
        public float HorizontalPercentage { get; set; }

        /// <summary>
        /// amount to shift the component by from where it is vertically justified
        /// </summary>
        public float VerticalPercentage { get; set; }
    }
}
