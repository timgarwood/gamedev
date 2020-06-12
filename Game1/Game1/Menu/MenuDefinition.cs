namespace Game1.Menu
{
    public class MenuDefinition
    {
        /// <summary>
        /// the name of this menu
        /// </summary>
        public string Name { get; set; }

        public string[] MessageTextures { get; set; }

        /// <summary>
        /// number of pixels between menu items
        /// </summary>
        public int SpaceBetweenMenuItems { get; set; }

        public int SpaceBetweenMessages { get; set; }

        /// <summary>
        /// list of menu items in this menu
        /// </summary>
        public MenuItemDefinition[] MenuItems;
    }
}
