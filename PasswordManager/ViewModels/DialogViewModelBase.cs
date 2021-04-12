using System;
using System.Windows;
using PasswordManagerClient.DataService;
using PasswordManagerClient.Enums;
using PasswordManagerClient.Models;

namespace PasswordManagerClient.ViewModels
{
    public class DialogViewModelBase : ViewModelBase
    {

	    /* Begin Reference - Modified Code
	    Aleksei Pavlov. ‘Dialogs In WPF (MVVM)’. Accessed 22 January 2021. https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/.
	   */
        /// <summary>
        /// Function which sets the dialog return value and closes the dialog box 
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="result"></param>
        public void CloseDialog(Window dialog, DialogResult result)
        {
            this.DialogResult = result;
            if (dialog != null)
                dialog.DialogResult = true;
        }
        /* End Reference */


        public virtual string ReturnCategoryItem()
        {
            throw new NotImplementedException();
        }

        public virtual string ReturnPassword()
        {
            throw new NotImplementedException();
        }
        public virtual (string, string) ReturnPins()
        {
	        throw new NotImplementedException();
        }
        public virtual string ReturnSecret()
        {
	        throw new NotImplementedException();
        }
        public virtual string ReturnEmail()
        {
	        throw new NotImplementedException();
        }

        /// <summary>
        /// Attribute stores the dialog result value
        /// </summary>
        public DialogResult DialogResult { get; set; }
    }
}