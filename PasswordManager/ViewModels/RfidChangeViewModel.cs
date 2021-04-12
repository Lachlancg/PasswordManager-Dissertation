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
	public class RfidChangeViewModel : DialogViewModelBase
	{
		#region RelayCommands
		private readonly RelayCommand _cancelCommand;
		public ICommand CancelCommand => _cancelCommand;
		private readonly RelayCommand _saveCommand;
		public ICommand SaveCommand => _saveCommand;
		#endregion

		private readonly NFCDataService NFC;

		public RfidChangeViewModel(NFCDataService nfc)
		{
			_cancelCommand = new RelayCommand(Cancel);
			_saveCommand = new RelayCommand(Save);

			NFC = nfc;
			//NFC.SelectDevice();
			//NFC.establishContext();
		}

		private bool LoadCardCommand()
		{
			if ((!string.IsNullOrWhiteSpace(_rfidPin)) && _rfidPin.Length == 6)
			{
				if ((!string.IsNullOrWhiteSpace(_rfidOldPin)) && _rfidOldPin.Length == 6)
				{
					byte[] newKeyBytes = new byte[6];

					for (int indx = 0; indx <= (_rfidPin).Length - 1; indx++)
					{
						newKeyBytes[indx] = (byte) _rfidPin[indx];
					}

					byte[] oldKeyBytes = new byte[6];

					for (int indx = 0; indx <= (_rfidOldPin).Length - 1; indx++)
					{
						oldKeyBytes[indx] = (byte) _rfidOldPin[indx];
					}

					if (NFC.connectCard())
					{
						NFC.LoadKey(oldKeyBytes, 0); //Change to default key
						NFC.LoadKey(newKeyBytes, 1); //Change to default key

						//Secret = SecurityDataService.GenerateRandomBytes(16);
						Secret = SecurityDataService.GeneratePassword(16, true, true, true, true);

						NFC.submitText(Secret, "4"); // 5 - is the block we are writing data on the card

						NFC.WriteKey(newKeyBytes);
						NFC.Close();

						string text = NFC.verifyCard("4", 1); // 5 - is the block we are reading
						if (text != "FailAuthentication")
						{
							return true;
						}
						return false;
					}
					else
					{
						MessageBox.Show("Cannot find card or reader");
						return false;
					}
				}
				else
				{
					MessageBox.Show("Enter valid RFID Pin");
					return false;
				}
			}
			return false;
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
			if (LoadCardCommand())
			{
				if (!string.IsNullOrWhiteSpace(_password))
				{
					CloseDialog(param as Window, DialogResult.Save);
				}
				else
				{
					MessageBox.Show("Enter a valid Password");
				}
			}
			else
			{
				MessageBox.Show("Failed to save RFID Card");
			}

		}


		public string Secret { get; set; }

		#region INotifyProperties

		private string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				this.OnPropertyChanged($"Password");
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

		private string _rfidOldPin;
		public string RFIDOldPin
		{
			get => _rfidOldPin;
			set
			{
				_rfidOldPin = value;
				this.OnPropertyChanged($"RFIDOldPin");


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