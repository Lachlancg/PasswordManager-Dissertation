using ModernWpf.Controls;


namespace PasswordManagerClient.Models
{
    /// <summary>
    /// Base class for a NavigationItem
    /// </summary>
    public class NavigationItemBase 
    {
        public NavigationItemBase(string name)
        {
            Name = name;
        }

        public NavigationItemBase()
        {
            Name = null;
        }

        public string Name { get; set; }

        public override string ToString()
        {
	        return Name;
        }
    }
}

