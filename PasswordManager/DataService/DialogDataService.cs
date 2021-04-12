using System.Windows;
using PasswordManagerClient.Enums;
using PasswordManagerClient.ViewModels;
using PasswordManagerClient.Views;

namespace PasswordManager.DataService
{
    class DialogDataService
    {

        /* Begin Reference - Modified Code
	     Aleksei Pavlov. ‘Dialogs In WPF (MVVM)’. Accessed 22 January 2021. https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/.
		*/
        public static DialogResult OpenCategoryDialog(ref DialogViewModelBase dialogViewModel)
        {
            DialogWindowView dialog = new DialogWindowView
            {
                DataContext = dialogViewModel,
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
            
            DialogResult result = (dialog.DataContext as DialogViewModelBase).DialogResult;
            return result;
        }
        /* End Reference */

    }
}
