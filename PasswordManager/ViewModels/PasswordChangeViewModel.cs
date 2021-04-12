using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ABI.System.Windows.Input;
using PasswordManagerClient.DataService;
using PasswordManagerClient.ViewModels;
using PasswordManagerClient.Enums;

namespace PasswordManagerClient.ViewModels
{
    public class PasswordChangeViewModel : DialogViewModelBase
    {
        #region RelayCommands
        private readonly RelayCommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand;
        private readonly RelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;
        private readonly RelayCommand _loadRfidCommand;
        public ICommand LoadRfidCommand => _loadRfidCommand;
        #endregion

        private readonly NFCDataService NFC;

        public PasswordChangeViewModel(NFCDataService nfc)
        {
            _cancelCommand = new RelayCommand(Cancel);
            _saveCommand = new RelayCommand(Save);
            _loadRfidCommand = new RelayCommand(LoadCard);


            _slider = 12;
            _lowerCase = true;
            _upperCase = true;
            _numbers = true;
            _specialChars = true;


            //NFC.SelectDevice();
            //NFC.establishContext();

            NFC = nfc;

            UpdatePassword();
        }
        private void LoadCard(object param)
        {
	        if ((!string.IsNullOrWhiteSpace(_rfidPin)) && _rfidPin.Length == 6)
	        {
		        byte[] key = Encoding.UTF8.GetBytes(_rfidPin);

				if (NFC.connectCard())
				{
					NFC.LoadKey(key, 0);
					NFC.Close();

					string text = NFC.verifyCard("4", 0); // 5 - is the block we are reading
					if (text != "FailAuthentication")
					{
						ValidRfid = true;
						Secret = text.Substring(0, 16);
					}
				}
				else
				{
					MessageBox.Show("Cannot find card or reader");
				}
            }
	        else
	        {
		        MessageBox.Show("Enter valid RFID Pin");
	        }
        }

        public override string ReturnPassword()
        {
            return _password;
        }

        public override string ReturnSecret()
        {
	        return Secret;
        }

        private void Cancel(object param)
        {
            CloseDialog(param as Window, DialogResult.Cancel);
        }

        private void Save(object param)
        {
	        if (_validRfid)
	        {
		        if ((!string.IsNullOrWhiteSpace(_password)) && (!string.IsNullOrWhiteSpace(_confirmPassword)))
		        {
			        if (_password != _confirmPassword)
			        {
				        MessageBox.Show("Passwords do not match");
			        }
			        else
			        {
				        CloseDialog(param as Window, DialogResult.Save);

			        }
		        }
		        else
		        {
			        MessageBox.Show("Enter a valid Password");
		        }
	        }
	        else
	        {
		        MessageBox.Show("Load RFID Card");
	        }

        }

        private void UpdatePassword()
        {
            string genPass = SecurityDataService.GeneratePassword(_slider, _lowerCase, _upperCase, _numbers, _specialChars);
            Password = genPass;
            ConfirmPassword = genPass;
            PasswordScore = SecurityDataService.PasswordStrengthCalculator(Password);
            PasswordColour = SecurityDataService.PasswordStrengthColour(PasswordScore);
        }

        public string Secret { get; set; }

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

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
	            _password = value;
	            PasswordScore = SecurityDataService.PasswordStrengthCalculator(Password);
	            PasswordColour = SecurityDataService.PasswordStrengthColour(PasswordScore);
                this.OnPropertyChanged($"Password");
            }
        }
        private string _confirmPassword;
        public string ConfirmPassword
        {
	        get { return _confirmPassword; }
	        set
	        {
		        _confirmPassword = value;

		        this.OnPropertyChanged($"ConfirmPassword");
	        }
        }

        private string _rfidPin;
        public string RFIDPin
        {
	        get => _rfidPin;
	        set
	        {
		        _rfidPin = value;
		        this.OnPropertyChanged($"RFIDPin");

	        }
        }
        private bool _validRfid;
        public bool ValidRfid
        {
	        get { return _validRfid; }
	        set
	        {
		        _validRfid = value;
		        OnPropertyChanged("Background");
		        OnPropertyChanged($"RfidColour");
	        }
        }

        public Brush RfidColour =>
	        ValidRfid ? Brushes.LightGreen : Brushes.Red;

        #endregion


    }
}