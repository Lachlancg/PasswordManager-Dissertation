using System;
using System.Net;
using System.Net.Sockets;
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
	public class AddRfidCardDialogViewModel : DialogViewModelBase
	{
		#region RelayCommands
		private readonly RelayCommand _cancelCommand;
		public ICommand CancelCommand => _cancelCommand;
		private readonly RelayCommand _scanRfidCommand;
		public ICommand ScanRfidCommand => _scanRfidCommand;

		#endregion

		public AddRfidCardDialogViewModel()
		{
			_scanRfidCommand = new RelayCommand(ScanRfid);
			_cancelCommand = new RelayCommand(Cancel);

			byte[] tempArray = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

			for (int indx = 0; indx <= tempArray.Length - 1; indx++)
			{
				_rfidOldPin += Convert.ToChar(tempArray[indx]);
			}
			//_rfidOldPin = Encoding.UTF8.GetString(tempArray);
		}



		private void ScanRfid(object param)
		{
			if (_rfidPin != null && _rfidPin.Length == 6)
			{
				if (_rfidOldPin != null && _rfidOldPin.Length == 6)
				{
					CloseDialog(param as Window, DialogResult.Scan);
				}
				else
				{
					MessageBox.Show("Enter valid old Pin");
				}
			}
			else
			{
				MessageBox.Show("Enter valid new Pin");
			}
		}
		private void Cancel(object param)
		{
			CloseDialog(param as Window, DialogResult.Cancel);
		}


		public override (string, string) ReturnPins()
		{
			return (_rfidPin, _rfidOldPin);
		}


		#region INotifyProperties

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
		#endregion
	}
}