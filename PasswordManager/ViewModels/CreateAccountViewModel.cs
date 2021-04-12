using System.Windows;
using System.Windows.Media;
using ABI.System.Windows.Input;
using PasswordManager.DataService;
using PasswordManagerClient.DataService;
using PasswordManagerClient.Enums;

namespace PasswordManagerClient.ViewModels
{
	public class CreateAccountViewModel: ViewModelBase
    {
	    private readonly RelayCommand _addRfidCommand;
	    public ICommand AddRfidCommand => _addRfidCommand;
	    private readonly NFCDataService NFC;



		public CreateAccountViewModel(NFCDataService nfc)
        {
	        _addRfidCommand = new RelayCommand(AddCard);

	        NFC = nfc;
        }

		/// <summary>
		/// Function opens RFID card dialog and reads RFID secret.
		/// </summary>
		/// <param name="param"></param>
        private void AddCard(object param)
        {
			ValidRfid = false;
			DialogViewModelBase addRfiDialogViewModel = new AddRfidCardDialogViewModel();
	        DialogResult categoryResult = DialogDataService.OpenCategoryDialog(ref addRfiDialogViewModel);

	        if (categoryResult == DialogResult.Scan)
	        {
		        string newKey, oldKey;
		        (newKey, oldKey) = addRfiDialogViewModel.ReturnPins();
		        //byte[] newKeyBytes = Encoding.UTF8.GetBytes(newKey);

		        byte[] newKeyBytes = new byte[6];

		        for (int indx = 0; indx <= (newKey).Length - 1; indx++)
		        {
			        newKeyBytes[indx] = (byte)newKey[indx];
		        }

				byte[] oldKeyBytes = new byte[6];

				for (int indx = 0; indx <= (oldKey).Length - 1; indx++)
		        {
			        oldKeyBytes[indx] = (byte)oldKey[indx];
		        }
				//byte[] oldKeyBytes = Encoding.UTF8.GetBytes(oldKey);

				if (NFC.connectCard())
				{
					//MessageBox.Show(cardUid);
					NFC.LoadKey(oldKeyBytes, 0); //Change to default key
					NFC.LoadKey(newKeyBytes, 1); //Change to default key

					//Secret = SecurityDataService.GenerateRandomBytes(16);
					Secret = SecurityDataService.GeneratePassword(16, true, true, true, true);

					if (NFC.submitText(Secret, "4")) // 5 - is the block we are writing data on the card
					{
						NFC.WriteKey(newKeyBytes);
						NFC.Close();

						string text = NFC.verifyCard("4", 1); // 5 - is the block we are reading
						if (text != "FailAuthentication")
						{
							ValidRfid = true;
						}
						else
						{
							MessageBox.Show("New pin authentication failed");
						}
					}
					else
					{
						MessageBox.Show("Old pin incorrect, try again");
					}
				}
				else
				{
					MessageBox.Show("Cannot find card or reader");
				}

	        }
        }

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

		public string Secret { get; set; }

		private string _newUserEmail;
		public string NewUserEmail
		{
			get => _newUserEmail;
			set
			{
				_newUserEmail = value;
				this.OnPropertyChanged($"NewUserEmail");
			}
		}

		private string _newUserPassword;
        public string NewUserPassword
        {
            get => _newUserPassword;
            set
            {
                _newUserPassword = value;

                PasswordScore = SecurityDataService.PasswordStrengthCalculator(_newUserPassword);
                PasswordColour = SecurityDataService.PasswordStrengthColour(PasswordScore);
				this.OnPropertyChanged($"NewUserPassword");
            }
        }

        private string _newUserConfirmPassword;
        public string NewUserConfirmPassword
        {
            get => _newUserConfirmPassword;
            set
            {
                _newUserConfirmPassword = value;
                this.OnPropertyChanged($"NewUserConfirmPassword");
            }
        }

        private bool validRfid;
        public bool ValidRfid
		{
			get { return validRfid; }
			set
			{
				validRfid = value;
				OnPropertyChanged($"RfidColour");
				OnPropertyChanged($"ValidRfid");
			}
		}

		public Brush RfidColour =>
			ValidRfid ? Brushes.LightGreen : Brushes.Red;

	}
}