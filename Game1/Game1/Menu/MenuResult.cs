namespace Game1.Menu
{
    public class MenuResult
    {
        public MenuResult(MenuAction menuAction, string nextMenuName = null)
        {
            Action = menuAction;
            NextMenuName = nextMenuName;
        }

        public MenuAction Action { get; private set; }

        public string NextMenuName { get; private set; }
    }
}
