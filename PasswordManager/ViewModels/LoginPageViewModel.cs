using System.Text;
using System.Windows;
using System.Windows.Media;
using ABI.System.Windows.Input;
using PasswordManagerClient.DataService;

namespace PasswordManagerClient.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {

	    private readonly RelayCommand _loadRfidCommand;
	    public ICommand LoadRfidCommand => _loadRfidCommand;
	    private readonly NFCDataService NFC;

		public LoginPageViewModel(NFCDataService nfc)
        {
	        _loadRfidCommand = new RelayCommand(LoadCard);

	        NFC = nfc;
        }

		/// <summary>
		/// Method used to load the RFID card secret into the system
		/// </summary>
		/// <param name="param"></param>
		private void LoadCard(object param)
		{
			ValidRfid = false;

			if (!string.IsNullOrWhiteSpace(_rfidPin) && _rfidPin.Length == 6)
			{
				byte[] key = Encoding.UTF8.GetBytes(_rfidPin);

				if (NFC.connectCard())
				{
					NFC.LoadKey(key, 0);
					NFC.Close();

					string text = NFC.verifyCard("4", 0); // 4 - is the block we are reading
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

		public string Secret { get; set; }


		private string _userEmail;
		public string UserEmail
		{
			get { return _userEmail; }
			set
			{
				_userEmail = value;
				this.OnPropertyChanged("NewUserEmail");
			}
		}

		private string _userPassword;
        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                _userPassword = value;
                this.OnPropertyChanged("NewUserPassword");
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
	}
}