using System.Printing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using ABI.System.Windows.Input;
using PasswordManagerClient.DataService;
using PasswordManagerClient.ViewModels;
using PasswordManagerClient.Enums;

namespace PasswordManagerClient.ViewModels
{
    public class PasswordGeneratorViewModel : DialogViewModelBase
    {
        #region RelayCommands
        private readonly RelayCommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand;
        private readonly RelayCommand _copyCommand;
        public ICommand CopyCommand => _copyCommand;
        #endregion

        public PasswordGeneratorViewModel()
        {
            _cancelCommand = new RelayCommand(Cancel);
            _copyCommand = new RelayCommand(Copy);

            _slider = 12;
            _lowerCase = true;
            _upperCase = true;
            _numbers = true;
            _specialChars = true;

            UpdatePassword();
        }

        public override string ReturnPassword()
        {
            return _generatedPassword;
        }

        private void Cancel(object param)
        {
            CloseDialog(param as Window, DialogResult.Cancel);
        }

        private void Copy(object param)
        {
	        CloseDialog(param as Window, DialogResult.Copy);
        }
        
        /// <summary>
        /// Updates password box using a randomly generated password
        /// </summary>
        private void UpdatePassword()
        {
            GeneratedPassword = SecurityDataService.GeneratePassword(_slider, _lowerCase, _upperCase, _numbers, _specialChars);
            PasswordScore = SecurityDataService.PasswordStrengthCalculator(GeneratedPassword);
            PasswordColour = SecurityDataService.PasswordStrengthColour(PasswordScore);
        }

        #region INotifyProperties

        private Brush _passwordColour;
        public Brush PasswordColour
        {
            get { return _passwordColour; }
            set
            {
                _passwordColour = value;

                this.OnPropertyChanged($"PasswordColour");
            }
        }

        private int _passwordScore;
        public int PasswordScore
        {
            get { return _passwordScore; }
            set
            {
                _passwordScore = value;
                
                this.OnPropertyChanged($"PasswordScore");
            }
        }

        private int _slider;
        public int Slider
        {
            get { return _slider; }
            set
            {
                _slider = value;
                UpdatePassword();
                this.OnPropertyChanged($"Slider");
            }
        }

        private bool _lowerCase;
        public bool LowerCase
        {
            get { return _lowerCase; }
            set
            {
                _lowerCase = value;
                UpdatePassword();
                this.OnPropertyChanged("Letters");
            }
        }

        private bool _upperCase;
        public bool UpperCase
        {
            get { return _upperCase; }
            set
            {
                _upperCase = value;
                UpdatePassword();
                this.OnPropertyChanged("MixedCase");
            }
        }

        private bool _numbers;
        public bool Numbers
        {
            get { return _numbers; }
            set
            {
                _numbers = value;
                UpdatePassword();
                this.OnPropertyChanged($"Numbers");
            }
        }

        private bool _specialChars;
        public bool SpecialChars
        {
            get { return _specialChars; }
            set
            {
                _specialChars = value;
                UpdatePassword();
                this.OnPropertyChanged($"SpecialChars");
            }
        }

        private string _generatedPassword;
        public string GeneratedPassword
        {
            get { return _generatedPassword; }
            set
            {
                _generatedPassword = value;
                this.OnPropertyChanged($"GeneratedPassword");
            }
        }
        #endregion


    }
}