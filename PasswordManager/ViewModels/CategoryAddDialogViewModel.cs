using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ABI.System.Windows.Input;
using PasswordManagerClient.Enums;

namespace PasswordManagerClient.ViewModels
{
    class CategoryAddDialogViewModel : DialogViewModelBase
    {
        #region RelayCommands
        private readonly RelayCommand _addCategory;
        public ICommand AddCategory => _addCategory;
        private readonly RelayCommand _cancelCategory;
        public ICommand CancelCategory => _cancelCategory;
        #endregion

        /// <summary>
        /// Constructor for Category Add Dialog box view model
        /// </summary>
        /// <param name="dataService"></param>
        public CategoryAddDialogViewModel()
        {
            _addCategory = new RelayCommand(AddCategoryC);
            _cancelCategory = new RelayCommand(CancelCategoryC);
        }
        public override string ReturnCategoryItem()
        {
            return _categoryItem;
        }

        /// <summary>
        /// Relay command function for add category button
        /// </summary>
        /// <param name="param"></param>
        private void AddCategoryC(object param)
        {
	        if ((!string.IsNullOrWhiteSpace(_categoryItem)))
	        {
		        CloseDialog(param as Window, DialogResult.Save);
	        }
	        else
	        {
		        MessageBox.Show("Enter a valid category name");
	        }
        }

        /// <summary>
        /// Relay command function for add category button
        /// </summary>
        /// <param name="param"></param>
        private void CancelCategoryC(object param)
        {
            CloseDialog(param as Window, DialogResult.Cancel);
        }

        private string _categoryItem;

        public string CategoryItem
        {
            get { return _categoryItem; }
            set
            {
                _categoryItem = value;
                this.OnPropertyChanged($"CategoryItem");
            }
        }
    }
}
