using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using ModernWpf.Controls;
using System.Windows;
using PasswordManager.DataService;
using PasswordManagerClient.DataService;
using PasswordManagerClient.Enums;
using PasswordManagerClient.Models;
using PasswordManagerClient.Views;

namespace PasswordManagerClient.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		private VaultDataService _vaultDataService;
		private SyncDataService _syncDataService;
		private readonly NFCDataService NFC;

		#region RelayCommands
		private readonly RelayCommand _addCategory;
		public ICommand AddCategory => _addCategory;
		private readonly RelayCommand _deleteCategory;
		public ICommand DeleteCategory => _deleteCategory;
		private readonly RelayCommand _generatePassword;
		public ICommand GeneratePassword => _generatePassword;
		private readonly RelayCommand _changePassword;
		public ICommand ChangePassword => _changePassword;
		private readonly RelayCommand _syncCommand;
		public ICommand SyncCommand => _syncCommand;
		private readonly RelayCommand _changeRfid;
		public ICommand ChangeRfid => _changeRfid;
		#endregion

		#region Constructor
		public MainPageViewModel(ref VaultDataService vaultDataService, ref SyncDataService syncDataService, NFCDataService nfc)
		{
			_addCategory = new RelayCommand(AddCategoryC);
			_deleteCategory = new RelayCommand(DeleteCategoryC);
			_generatePassword = new RelayCommand(GeneratePasswordC);
			_changePassword = new RelayCommand(ChangePasswordC);
			_syncCommand = new RelayCommand(Sync);
			_changeRfid = new RelayCommand(ChangeRfidC);

			_vaultDataService = vaultDataService;
			_syncDataService = syncDataService;
			NFC = nfc;

			Load();
			
			NavItem = new AllItemsPageView(ref _vaultDataService, ref _syncDataService, "All Items");
			NavSelectedItem = _vaultDataService.ReturnNavData().FirstOrDefault();

		}

		#endregion


	
		#region Relaycommand Functions
		/// <summary>
		/// Relay command function triggered when sync button is pressed
		/// </summary>
		/// <param name="param"></param>
		private void Sync(object param)
		{
			_syncDataService.PollServer();
			if (!_syncDataService.OfflineMode)
			{
				_syncDataService.SyncDatabase();
				_syncDataService.RequestDatabase();
			}
			else
			{
				MessageBox.Show("Go online to sync Vault");
			}
		}



		/// <summary>
		/// Relay command function triggered by addCategory menu button.
		/// </summary>
		/// <param name="param"></param>
		private void AddCategoryC(object param)
		{
			_syncDataService.PollServer();
			if (!_syncDataService.OfflineMode)
			{
				DialogViewModelBase categoryAddDialogViewModel = new CategoryAddDialogViewModel();
				DialogResult categoryResult = DialogDataService.OpenCategoryDialog(ref categoryAddDialogViewModel);

				if (categoryResult == DialogResult.Save)
				{
					_vaultDataService.AddCategory(new NavigationItemDatabase("Tag", categoryAddDialogViewModel.ReturnCategoryItem()));
				}
				Load();
				//Sync changes with server
				_syncDataService.SyncDatabase();
			}
			else
			{
				MessageBox.Show("Go online to add category to the Vault");
			}

		}

		/// <summary>
		/// Relay command function triggered by DeleteCategory menu button
		/// </summary>
		/// <param name="param"></param>
		private void DeleteCategoryC(object param)
		{
			_syncDataService.PollServer();
			if (!_syncDataService.OfflineMode)
			{
				if (NavSelectedItem.Symbol == Symbol.Tag)
				{
					NavigationItemDatabase _itemToDelete = new NavigationItemDatabase(_navSelectedItem);
					DialogViewModelBase categoryDeleteDialogViewModel =
						new CategoryDeleteDialogViewModel(_itemToDelete);
					DialogResult categoryResult =
						DialogDataService.OpenCategoryDialog(ref categoryDeleteDialogViewModel);


					if (categoryResult == DialogResult.Delete)
					{
						_vaultDataService.DeleteCategory(_itemToDelete);
						_vaultDataService.ClearSitesDeletedCategory(_itemToDelete);
						NavSelectedItem = _navigationData.FirstOrDefault();
					}

					Load();
					//Sync changes with server
					_syncDataService.SyncDatabase();

				}
				else
				{
					DialogViewModelBase categoryDeleteDialogViewModel = new CategoryDeleteWarningViewModel();
					DialogDataService.OpenCategoryDialog(ref categoryDeleteDialogViewModel);
				}
			}
			else
			{
				MessageBox.Show("Go online to delete category from the Vault");
			}
		}

		private void ChangeRfidC(object param)
		{
			_syncDataService.PollServer();
			if (!_syncDataService.OfflineMode)
			{
				DialogViewModelBase RfidChangeDialogViewModel = new RfidChangeViewModel(NFC);
				DialogResult dialogResult = DialogDataService.OpenCategoryDialog(ref RfidChangeDialogViewModel);

				if (dialogResult == DialogResult.Save)
				{
					string passwordAndSecret = RfidChangeDialogViewModel.ReturnPassword() +
					                           RfidChangeDialogViewModel.ReturnSecret();

					if (_syncDataService.ChangePassword(passwordAndSecret))
					{
						_vaultDataService.ChangeMasterPassword(passwordAndSecret);
						_syncDataService.SyncDatabase();
					}
					else
					{
						if (!_syncDataService.OfflineMode)
						{
							MessageBox.Show("Could not change users password due to authentication issue");
						}
						else
						{
							MessageBox.Show("Cannot connect to server to change password");
						}

					}
				}
			}
		}

		private void ChangePasswordC(object param)
		{
			_syncDataService.PollServer();
			if (!_syncDataService.OfflineMode)
			{
				DialogViewModelBase PassChangeDialogViewModel = new PasswordChangeViewModel(NFC);
				DialogResult dialogResult = DialogDataService.OpenCategoryDialog(ref PassChangeDialogViewModel);

				if (dialogResult == DialogResult.Save)
				{
					string passwordAndSecret = PassChangeDialogViewModel.ReturnPassword() +
					                           PassChangeDialogViewModel.ReturnSecret();

					if (_syncDataService.ChangePassword(passwordAndSecret))
					{
						_vaultDataService.ChangeMasterPassword(passwordAndSecret);
						_syncDataService.SyncDatabase();
					}
					else
					{
						if (!_syncDataService.OfflineMode)
						{
							MessageBox.Show("Could not change users password due to authentication issue");
						}
						else
						{
							MessageBox.Show("Cannot connect to server to change password");
						}
						
					}
				}
			}
			else
			{
				MessageBox.Show("Go online to change Vault password");
			}
		}

		private void GeneratePasswordC(object param)
		{
			DialogViewModelBase PassGenDialogViewModel = new PasswordGeneratorViewModel();
			DialogResult dialogResult = DialogDataService.OpenCategoryDialog(ref PassGenDialogViewModel);

			if (dialogResult == DialogResult.Copy)
			{
				if (!string.IsNullOrWhiteSpace(PassGenDialogViewModel.ReturnPassword()))
				{
					Clipboard.SetText(PassGenDialogViewModel.ReturnPassword());
				}
			}

		}


		/// <summary>
		/// Loads the navigation items into the NavigationData
		/// </summary>
		private void Load()
		{
			NavigationData = _vaultDataService.ReturnNavData();
		}

		#endregion

		/// <summary>
		/// ObservableCollection associated with the navigation items
		/// </summary>
		private ObservableCollection<NavigationItem> _navigationData;
		public ObservableCollection<NavigationItem> NavigationData
		{
			get { return _navigationData; }
			set
			{
				_navigationData = value;
				this.OnPropertyChanged($"NavigationData");
			}
		}

		#region INotifyPropertyChanged Fields

		private bool _offlineVisibility;

		public bool OfflineVisibility
		{
			get
			{
				return _offlineVisibility;
			}
			set
			{
				_offlineVisibility = value;

			}
		}




		private UserControl _navItem;
		/// <summary>
		/// Triggers a property changed event when set which changes the page displayed on the main page.
		/// </summary>
		public UserControl NavItem
		{
			get
			{
				return _navItem;
			}
			set
			{
				_navItem = value;
				this.OnPropertyChanged($"NavItem");
			}
		}

		private NavigationItem _navSelectedItem;
		/// <summary>
		/// Triggered by INotifyPropertyChanged when the NavigationView on the main page view is changed.
		/// </summary>
		public NavigationItem NavSelectedItem
		{
			get
			{
				return _navSelectedItem;
			}
			set
			{
				_navSelectedItem = value;
				if (_navSelectedItem != null)
				{
					NavItem = new AllItemsPageView(ref _vaultDataService, ref _syncDataService, _navSelectedItem.Name);
				}
				else
				{
					NavItem = null;
				}

				this.OnPropertyChanged($"NavSelectedItem");
			}
		}

		#endregion
	}

}
