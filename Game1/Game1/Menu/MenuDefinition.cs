namespace Game1.Menu
{
    public class MenuDefinition
    {
        /// <summary>
        /// the name of this menu
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the menu that brough us to this menu
        /// </summary>
        public string ParentMenu { get; set; }

        /// <summary>
        /// name of the font to render this menu
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// starting x coordinate of menu
        /// </summary>
        public int StartX { get; set; }

        /// <summary>
        /// starting y coordinate of menu
        /// </summary>
        public int StartY { get; set; }

        /// <summary>
        /// list of menu items in this menu
        /// </summary>
        public MenuItemDefinition[] MenuItems;
    }
}
