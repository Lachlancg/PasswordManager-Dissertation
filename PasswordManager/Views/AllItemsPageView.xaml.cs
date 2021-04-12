using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PasswordManagerClient.DataService;
using PasswordManagerClient.Models;
using PasswordManagerClient.ViewModels;
using Page = ModernWpf.Controls.Page;

namespace PasswordManagerClient.Views
{
    /// <summary>
    /// Interaction logic for AllItemsPage.xaml
    /// </summary>
    public partial class AllItemsPageView : UserControl
    {
        public AllItemsPageView(ref VaultDataService _dataService, ref SyncDataService _syncDataService, string filter)
        {
            InitializeComponent();
            DataContext = new AllItemsPageViewModel(ref _dataService, ref _syncDataService, filter);
        }

    }


}