using ModernWpf.Controls;


namespace PasswordManagerClient.Models
{
    /// <summary>
    /// Class which inherits from a navigation item database and overide the symbol string item as a Symbol
    /// </summary>
    public class NavigationItem : NavigationItemBase
    {
        public NavigationItem(Symbol symbol, string name)
        {
            base.Name = name;
            Symbol = symbol;
        }

        public NavigationItem()
        {
            base.Name = null;
        }

        public Symbol Symbol { get; set; }

        //public override string ToString()
        //{
        //    return Name;
        //}
    }
}

