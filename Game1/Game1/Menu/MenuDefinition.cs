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
        /// number of pixels between menu items
        /// </summary>
        public int SpaceBetweenMenuItems { get; set; }

        /// <summary>
        /// list of menu items in this menu
        /// </summary>
        public MenuItemDefinition[] MenuItems;
    }
}
