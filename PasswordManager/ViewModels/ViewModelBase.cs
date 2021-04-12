using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerClient.ViewModels
{
    public abstract  class ViewModelBase : INotifyPropertyChanged
    {
        /* Begin Reference - Copied Code
        kexugit. ‘Patterns - WPF Apps With The Model-View-ViewModel Design Pattern’. Accessed 11 January 2021. https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern.
        */
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        /* End Reference */
        
    }
}
