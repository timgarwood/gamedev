using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Menu
{
    public class MenuItemDefinition
    {
        public enum SelectActions
        {
            Next,
            Back,
            NewGame,
            QuitGame
        };

        private SelectActions _selectAction;
        private string _selectActionString;
        public string Action
        {
            get
            {
                return _selectActionString;
            }
            set
            {
                _selectActionString = value;
                Enum.TryParse(_selectActionString, out _selectAction);
            }
        }

        public SelectActions SelectAction
        {
            get
            {
                return _selectAction;
            }
        }

        /// <summary>
        /// the text of this menu item
        /// </summary>
        public string TextureName { get; set; }

        /// <summary>
        /// the menu to load when this item is selected
        /// </summary>
        public string MenuName { get; set; }
    }
}
