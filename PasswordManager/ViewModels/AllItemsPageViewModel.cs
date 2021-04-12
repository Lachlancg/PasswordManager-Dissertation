using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using PasswordManager.DataService;
using PasswordManagerClient.DataService;
using PasswordManagerClient.Enums;
using PasswordManagerClient.Models;
using ICommand = ABI.System.Windows.Input.ICommand;

namespace PasswordManagerClient.ViewModels
{
	public class AllItemsPageViewModel : ViewModelBase
	{

		private readonly string Filter;
		private readonly VaultDataService _vaultDataService;
		private readonly SyncDataService _syncDataService;


		#region RelayCommands
		private readonly RelayCommand _saveSite;
		public ICommand SaveSite => _saveSite;
		private readonly RelayCommand _addSite;
		public ICommand AddSite => _addSite;
		private readonly RelayCommand _deleteSite;
		public ICommand DeleteSite => _deleteSite;

		#endregion


		#region Constructor
		public AllItemsPageViewModel(ref VaultDataService dataService, ref SyncDataService syncDataService, string filter)
		{
			//Create relay commands
			_saveSite = new RelayCommand(SaveSiteC);
			_addSite = new RelayCommand(AddSiteC);
			_deleteSite = new RelayCommand(DeleteSiteC);

			//Associate function with event handler
			VaultDataService.CategoriesChanged += OnCategoriesChanged;

			//Set vault manager data service instance
			_vaultDataService = dataService;
			_syncDataService = syncDataService;

			//Save filter value
			Filter = filter;
			//Load sites into list view on GUI and set the selected item
			Load();

			if (_siteData != null)
				SiteSelectedItem = _siteData.FirstOrDefault();

		}

		/// <summary>
		/// Method called when categories are changed
		/// </summary>
		/// <param name="obj"></param>
		public void OnCategoriesChanged(object obj)
		{
			List<string> tempList = new List<string> {""};
			tempList.AddRange(_vaultDataService.ReturnCategoryData());
			CategoryData = tempList;
		}

		#endregion

		#region RelayCommandFunctions
		/// <summary>
		/// Save the selected site in the database
		/// </summary>
		/// <param name="param"></param>
		private void SaveSiteC(object param)
		{
			_syncDataService.PollServer();
			if (!_syncDataService.OfflineMode)
			{
				var tempItem = SiteSelectedItem;
				//Check if password value is not all •, if so that means the user has entered a new password
				if (!(_password.All(x => x == '•')))
				{
					_siteSelectedItem.Password = SecurityDataService.EncryptBytesAES(Encoding.UTF8.GetBytes(_password),
						_vaultDataService.ReturnMasterKey());
				}
				//Save the selected site in the database, either update existing or 

				if (!string.IsNullOrWhiteSpace(_siteSelectedItem.Name))
				{
					if (!string.IsNullOrWhiteSpace(_siteSelectedItem.Domain))
					{
						if (!string.IsNullOrWhiteSpace(_siteSelectedItem.Password))
						{
							if (!string.IsNullOrWhiteSpace(_siteSelectedItem.Username))
							{
								_vaultDataService.SaveSite(_siteSelectedItem);
								Load();
								//Sync changes with server
								_syncDataService.SyncDatabase();
								SiteSelectedItem = SiteData.Find(x => x.credGUID == tempItem.credGUID);
							}
							else
							{
								MessageBox.Show("Enter a valid Site UserName");
							}
						}
						else
						{
							MessageBox.Show("Enter a valid Site Password");
						}
					}
					else
					{
						MessageBox.Show("Enter a valid Site Domain");
					}
				}
				else
				{
					MessageBox.Show("Enter a valid Site Name");
				}
			}
			else
			{
				MessageBox.Show("Go online to add Site to Vault");
			}
		}

		/// <summary>
		/// Change GUI to allow new site to be entered
		/// </summary>
		/// <param name="param"></param>
		private void AddSiteC(object param)
		{
			_syncDataService.PollServer();
			if (!_syncDataService.OfflineMode)
			{
				SiteSelectedItem = null;
				//Create a new site when the user presses the add new site button
				SiteSelectedItem = new Site();
				HidePasswordAndReadonly = false;
			}
			else
			{
				MessageBox.Show("Go online to add Site to Vault");
			}
		}

		/// <summary>
		/// Delete site from Vault 
		/// </summary>
		/// <param name="param"></param>
		private void DeleteSiteC(object param)
		{
			_syncDataService.PollServer();
			if (!_syncDataService.OfflineMode)
			{
				DialogViewModelBase SiteDeleteDialogViewModel = new SiteDeleteDialogViewModel(SiteSelectedItem);
				DialogResult categoryResult = DialogDataService.OpenCategoryDialog(ref SiteDeleteDialogViewModel);

				if (categoryResult == DialogResult.Delete)
				{
					//Check if the delete was successful and if not give the user a warning
					if (!_vaultDataService.DeleteSite(SiteSelectedItem))
					{
						MessageBox.Show("Delete of site failed");
					}

					Load();
					//Sync changes with server
					_syncDataService.SyncDatabase();
					SiteSelectedItem = _siteData.FirstOrDefault();
				}

			}
			else
			{
				MessageBox.Show("Go online to delete Site from Vault");
			}
		}
		#endregion

		#region MyRegion


		#endregion
		/// <summary>
		/// Filter returns favourite value of site item
		/// </summary>
		/// <param name="siteItem"></param>
		/// <returns></returns>
		private static bool FindFavourite(Site siteItem)
		{
			return siteItem.Favourite;
		}
		/// <summary>
		/// Loads the sites stored in the password manage data service to the GUI
		/// </summary>
		/// <param name="filter"></param>
		private void Load()
		{
			if (Filter == "All Items")
			{
				SiteData = _vaultDataService.ReturnSiteData();
			}
			else if (Filter == "Favourites")
			{
				SiteData = _vaultDataService.ReturnSiteData().FindAll(FindFavourite);
			}
			else
			{
				SiteData = _vaultDataService.ReturnSiteData().FindAll(x => x.Category == Filter);
			}

			List<string> tempList = new List<string> {""};
			tempList.AddRange((_vaultDataService.ReturnCategoryData()));
			CategoryData = tempList;


		}

		#region INotifyPropertyChanged Fields

		private List<Site> _siteData;
		/// <summary>
		/// Data which is displayed in the users site area
		/// </summary>
		public List<Site> SiteData
		{
			get { return _siteData; }
			set
			{
				_siteData = value;
				this.OnPropertyChanged($"SiteData");
			}
		}

		private List<string> _categoryData;
		/// <summary>
		/// Data that is displayed in the selected item category drop down.
		/// </summary>
		public List<string> CategoryData
		{
			get { return _categoryData; }
			set
			{
				_categoryData = value;
				this.OnPropertyChanged($"CategoryData");
			}
		}

		//private string _categoryItem;
		/// <summary>
		/// Value that is selected in the selected item category drop down.
		/// </summary>
		public string CategoryItem
		{
			get
			{
				if (_siteSelectedItem != null)
					return _siteSelectedItem.Category;
				else
				{
					return "";
				}
			}
			set
			{
				if (value != null)
				{
					_siteSelectedItem.Category = value;

				}
				this.OnPropertyChanged($"CategoryItem");
			}
		}

		private string _password;
		/// <summary>
		/// Value which is shown in the password text box 
		/// </summary>
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				//Save value if it is not equal to all •, the password is encrypted as soon as it is entered into the system
				if (value != null)
					if (!(value.All(x => x == '•')))
					{
						_siteSelectedItem.Password =
							SecurityDataService.EncryptBytesAES(Encoding.UTF8.GetBytes(value),
								_vaultDataService.ReturnMasterKey());
					}
				this.OnPropertyChanged($"Password");
			}
		}

		private bool _hidePasswordAndReadonly;
		/// <summary>
		/// Triggered by INotifyPropertyChanged when checkbox is ticked the users password is obscured
		/// Password is encrypted up until the point that they wish to view it
		/// </summary>
		public bool HidePasswordAndReadonly
		{
			get { return _hidePasswordAndReadonly; }
			set
			{
				_hidePasswordAndReadonly = value;

				if (_hidePasswordAndReadonly == false && _siteSelectedItem.Password != null)
				{
					Password = Encoding.UTF8.GetString(SecurityDataService.DecryptStringAES(_siteSelectedItem.Password, _vaultDataService.ReturnMasterKey()));
				}
				else
				{
					if (_siteSelectedItem.Password != null)
					{
						//Password = new string('*', _siteSelectedItem.Password.Length);
						Password = "••••••••••••";
					}
					else
					{
						Password = "";
					}
				}
				this.OnPropertyChanged($"HidePasswordAndReadonly");
			}
		}
		

		private Site _siteSelectedItem;

		/// <summary>
		/// Triggered by INotifyPropertyChanged when the SitesView selected item is changed.
		/// </summary>
		public Site SiteSelectedItem
		{
			get { return _siteSelectedItem; }
			set
			{
				_siteSelectedItem = value;
				this.OnPropertyChanged($"SiteSelectedItem");

				if (_siteSelectedItem == null)
				{
					TextBoxVisibility = Visibility.Hidden;
				}
				else
				{
					CategoryItem = _siteSelectedItem.Category;
					HidePasswordAndReadonly = true;
					TextBoxVisibility = Visibility.Visible;
				}
			}
		}

		private Visibility _textBoxVisibility;
		/// <summary>
		/// Used to change the Visibity of the credential text boxes on the all items page
		/// </summary>
		public Visibility TextBoxVisibility
		{
			get { return _textBoxVisibility; }
			set
			{
				_textBoxVisibility = value;
				this.OnPropertyChanged($"TextBoxVisibility");
			}
		}

		private string _searchQuery;
		/// <summary>
		/// Triggered by INotifyPropertyChanged when the searchBox on the all items page is used/
		/// </summary>
		public string SearchQuery
		{
			get { return _searchQuery; }
			set
			{
				_searchQuery = value;
				OnPropertyChanged("");
				SearchSites(_searchQuery);
			}
		}

		/// <summary>
		/// Function used to filter the sites using a query string
		/// </summary>
		/// <param name="searchQuery"></param>
		private void SearchSites(string searchQuery)
		{
			if (!string.IsNullOrEmpty(searchQuery))
			{
				List<Site> searchResults = new List<Site>();
				foreach (var site in _vaultDataService.ReturnSiteData())
				{
					if (site.Name.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase))
					{
						searchResults.Add(site);
					}
				}

				SiteData = searchResults;
			}
			else
			{
				Load();
			}
		}
		#endregion
	}
}
