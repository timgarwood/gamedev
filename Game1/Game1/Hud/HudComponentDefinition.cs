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

        /// <summary>
        /// creates a HudComponentDefinition from the given dynamic object
        /// </summary>
        /// <param name="jsonData"></param>
        public static HudComponentDefinition Create(dynamic d)
        {
            var def = new HudComponentDefinition(); 
            def.HorizontalPercentage = (float)d["horizontalPercentage"]; 
            def.VerticalPercentage = (float)d["verticalPercentage"];
            def.HAlign = (string)d["hAlignment"];
            def.VAlign = (string)d["vAlignment"];
            return def;
        }
    }
}
