using System;
using System.Net;
using System.Net.Sockets;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ABI.System.Windows.Input;
using ABI.Windows.Web.Http;
using PasswordManagerClient.DataService;
using PasswordManagerClient.ViewModels;
using PasswordManagerClient.Enums;

namespace PasswordManagerClient.ViewModels
{
    public class GoOnlineViewModel : DialogViewModelBase
    {
        #region RelayCommands
        private readonly RelayCommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand;
        private readonly RelayCommand _copyCommand;
        public ICommand CopyCommand => _copyCommand;
        private readonly RelayCommand _loadRfidCommand;
        public ICommand LoadRfidCommand => _loadRfidCommand;
        #endregion

        private readonly NFCDataService NFC;

        public GoOnlineViewModel(NFCDataService nfc)
        {
            _cancelCommand = new RelayCommand(Cancel);
            _copyCommand = new RelayCommand(Copy);
            _loadRfidCommand = new RelayCommand(LoadCard);

            NFC = nfc;
            //NFC.SelectDevice();
            //NFC.establishContext();
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


        private void Cancel(object param)
        {
            CloseDialog(param as Window, DialogResult.Cancel);
        }

        private void Copy(object param)
        {
            if(!string.IsNullOrWhiteSpace(_emailAddress))
			{
				if (!string.IsNullOrWhiteSpace(_password))
				{
					CloseDialog(param as Window, DialogResult.Login);
				}
				else
				{
                    MessageBox.Show("Enter a valid Password");
				}
			}
            else
            {
                MessageBox.Show("Enter a valid Email Address");
            }
        }

        public override string ReturnPassword()
        {
	        return _password;
        }

        public override string ReturnEmail()
        {
	        return _emailAddress;
        }
        public override string ReturnSecret()
        {
	        return Secret;
        }

        public string Secret { get; set; }

        #region INotifyProperties

        private string _emailAddress;
        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
	            _emailAddress = value;
            }
        }

        private string _password;
        public string Password
        {
	        get { return _password; }
	        set
	        {
		        _password = value;
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