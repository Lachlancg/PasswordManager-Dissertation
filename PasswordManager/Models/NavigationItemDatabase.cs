namespace PasswordManagerClient.Models
{
    /// <summary>
    /// Class which describes a navigation item
    /// </summary>
    public class NavigationItemDatabase : NavigationItemBase
    {
        public NavigationItemDatabase(string symbol,string name = null)
        {
            Name = name;
            Symbol = symbol;
        }

        public NavigationItemDatabase(NavigationItem navItem)
        {
            Name = navItem.Name;

            switch (navItem.Symbol)
            {
                case ModernWpf.Controls.Symbol.Home:
                    Symbol = "Home";
                    break;
                case ModernWpf.Controls.Symbol.Setting:
                    Symbol = "Setting";
                    break;
                case ModernWpf.Controls.Symbol.Favorite:
                    Symbol = "Favourite";
                    break;
                default:
                    Symbol = "Tag";
                    break;

            }
        }

        public NavigationItemDatabase()
        {
            Name = null;
            Symbol = null;
        }

        public string Symbol { get; set; }



    }
}

