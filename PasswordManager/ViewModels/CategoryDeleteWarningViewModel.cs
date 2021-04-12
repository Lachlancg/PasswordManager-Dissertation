using System.Windows;
using ABI.System.Windows.Input;
using PasswordManagerClient.Enums;

namespace PasswordManagerClient.ViewModels
{
    public class CategoryDeleteWarningViewModel : DialogViewModelBase
    {
        #region RelayCommands
        private readonly RelayCommand _okayCategory;
        public ICommand OkayCategory => _okayCategory;
        #endregion

        public CategoryDeleteWarningViewModel ()
        {
            _okayCategory = new RelayCommand(okayCategory);
        }

        private void okayCategory(object param)
        {
            CloseDialog(param as Window, DialogResult.Okay);
        }
    }
}