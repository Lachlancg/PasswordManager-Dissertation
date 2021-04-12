using System.Collections.Generic;
using ABI.System.Windows.Input;
using System.Windows;
using PasswordManagerClient.Models;
using PasswordManagerClient.Enums;

namespace PasswordManagerClient.ViewModels
{
    public class CategoryDeleteDialogViewModel : DialogViewModelBase
    {
        private NavigationItemDatabase _itemToDelete;
        #region RelayCommands
        private readonly RelayCommand _deleteCategory;
        public ICommand DeleteCategory => _deleteCategory;
        private readonly RelayCommand _cancelCategory;
        public ICommand CancelCategory => _cancelCategory;
        #endregion

        public CategoryDeleteDialogViewModel(NavigationItemDatabase itemToDelete)
        {
            _deleteCategory = new RelayCommand(deleteCategory);
            _cancelCategory = new RelayCommand(cancelCategory);

            _itemToDelete = itemToDelete;
            Description = "Delete Selected Category?";
            //ItemName = _itemToDelete.Name;

        }

        /// <summary>
        /// Relay command function for add category button
        /// </summary>
        /// <param name="param"></param>
        private void deleteCategory(object param)
        {
            CloseDialog(param as Window, DialogResult.Delete);
        }

        /// <summary>
        /// Relay command function for cancel button
        /// </summary>
        /// <param name="param"></param>
        private void cancelCategory(object param)
        {
            CloseDialog(param as Window, DialogResult.Cancel);
        }


        //public string ItemName { get; set; }

        public string Description { get; set; }
    }
}