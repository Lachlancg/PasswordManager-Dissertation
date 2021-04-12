using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using ABI.System.Windows.Input;
using PasswordManager.DataService;
using PasswordManagerClient.DataService;
using PasswordManagerClient.Enums;
using PasswordManagerClient.Models;

namespace PasswordManagerClient.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{

		//private SecurityDataService _securityDataService;
		private VaultDataService _vaultDataService;
		private SyncDataService _syncDataService;
		public int TimeOut { get; set; }

		//private bool validRfid = false;


		#region RelayCommands
		private readonly RelayCommand _cancelCommand;
		public ICommand CancelCommand => _cancelCommand;
		private readonly RelayCommand _submitCommand;
		public ICommand SubmitCommand => _submitCommand;
		private readonly RelayCommand _loginCommand;
		public ICommand LoginCommand => _loginCommand;
		private readonly RelayCommand _createAccount;
		public ICommand CreateAccount => _createAccount;
		private readonly RelayCommand _logOutCommand;
		public ICommand LogOutCommand => _logOutCommand;
		private readonly RelayCommand _offlineCommand;
		public ICommand OfflineCommand => _offlineCommand;
		private readonly RelayCommand _goOnlineCommand;
		public ICommand GoOnlineCommand => _goOnlineCommand;
		private readonly RelayCommand _deleteAccountCommand;
		public ICommand DeleteAccountCommand => _deleteAccountCommand;

		#endregion

		private readonly NFCDataService NFC = new NFCDataService();


		public MainWindowViewModel()
		{
			_submitCommand = new RelayCommand(Submit);
			_cancelCommand = new RelayCommand(Cancel);
			_loginCommand = new RelayCommand(Login);
			_createAccount = new RelayCommand(CreateAccountC);
			_logOutCommand = new RelayCommand(LogOut);
			_offlineCommand = new RelayCommand(GoOffline);
			_goOnlineCommand = new RelayCommand(GoOnline);
			_deleteAccountCommand = new RelayCommand(DeleteAccount);


			_vaultDataService = new VaultDataService();
			_syncDataService = new SyncDataService();

			//Set the default time for the user inactivity log out 
			TimeOut = 10;

			/* Begin Reference - Copied Code
		     Bianca Kalman. ‘c# - WPF inactivity and activity’. Accessed 25 January 2021. https://stackoverflow.com/questions/4963135/wpf-inactivity-and-activity/4965166#4965166.
		     */
			DispatcherTimer timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
			timer.Tick += Timer_Tick;
			timer.Start();
			/* End Reference */



			NFC.SelectDevice();
			//NFC.establishContext();

			ViewModel = new LoginPageViewModel(NFC);

		}

		/* Begin Reference - Edited Code
		     Bianca Kalman. ‘c# - WPF inactivity and activity’. Accessed 25 January 2021. https://stackoverflow.com/questions/4963135/wpf-inactivity-and-activity/4965166#4965166.
		*/
		void Timer_Tick(object sender, EventArgs e)
		{
			var idleTime = IdleTimeDetector.GetIdleTimeInfo();
			//Check for inactivity.
			if (idleTime.IdleTime.Minutes >= TimeOut)
			{
				//Log the user out if they are inactive for longer than the specified time.
				if (ViewModel.GetType() == typeof(MainPageViewModel))
				{
					_vaultDataService = new VaultDataService();
					ViewModel = new LoginPageViewModel(NFC);
				}
			}
		}
		/* End Reference */

		/// <summary>
		/// Triggered when logout button is pressed
		/// </summary>
		/// <param name="param"></param>
		private void LogOut(object param)
		{
			_vaultDataService = new VaultDataService();
			_syncDataService = new SyncDataService();
			ViewModel = new LoginPageViewModel(NFC);
		}

		private void DeleteAccount(object param)
		{
			DialogViewModelBase accountDialogViewModel = new AccountDeleteDialogViewModel();
			DialogResult categoryResult = DialogDataService.OpenCategoryDialog(ref accountDialogViewModel);

			if (categoryResult == DialogResult.Delete)
			{
				_syncDataService.DeleteAccount();
				_vaultDataService = new VaultDataService();
				_syncDataService = new SyncDataService();
				ViewModel = new LoginPageViewModel(NFC);
			}
		}

		private void GoOnline(object param)
		{
			if (_syncDataService.OfflineMode)
			{
				//Get the login details off of the user
				DialogViewModelBase goOnlineDialogViewModel = new GoOnlineViewModel(NFC);
				DialogResult categoryResult = DialogDataService.OpenCategoryDialog(ref goOnlineDialogViewModel);

				if (categoryResult == DialogResult.Login)
				{
					string passwordAndSecret = goOnlineDialogViewModel.ReturnPassword() +
											   goOnlineDialogViewModel.ReturnSecret();

					//Check the user is attempting to log into the current local vault account
					if (_vaultDataService.AuthenticateUser(goOnlineDialogViewModel.ReturnEmail(),
						passwordAndSecret))
					{
						//Generate a random sessionId used to create a unique user session and ensure only one user is logged in at one time.
						string sessionId = DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture) + SecurityDataService.GenerateAsciiToken();

						//Authenticate the user with the server
						if (_syncDataService.AuthenticateUser(new User(
							SecurityDataService.HashAndSaltPassword(passwordAndSecret,
								Encoding.UTF8.GetBytes(goOnlineDialogViewModel.ReturnEmail()), 256 / 8),
							goOnlineDialogViewModel.ReturnEmail(), sessionId)))
						{
							_syncDataService.RequestDatabase();
							//Load all data from the vault into memory
							_vaultDataService.LoadData();
							ViewModel = new MainPageViewModel(ref _vaultDataService, ref _syncDataService, NFC);
							MessageBox.Show("User online");
						}
						else
						{
							if (_syncDataService.OfflineMode)
							{
								MessageBox.Show("Could not connect to server");
							}
							else
							{
								MessageBox.Show("Password Incorrect");
							}
						}
					}
					else
					{
						MessageBox.Show("Login details incorrect or do not match current logged in account, log out and try again");
					}

				}
			}
			else
			{
				MessageBox.Show("Already online");
			}

		}

		/// <summary>
		/// Triggered when logout button is pressed
		/// </summary>
		/// <param name="param"></param>
		private void GoOffline(object param)
		{
			_syncDataService.OfflineMode = true;
		}

		/// <summary>
		/// Triggered when create account button is pressed
		/// </summary>
		/// <param name="param"></param>
		private void CreateAccountC(object param)
		{
			ViewModel = new CreateAccountViewModel(NFC);
		}

		/// <summary>
		/// Triggered when login button is pressed
		/// </summary>
		/// <param name="param"></param>
		private void Login(object param)
		{
			var loginPageViewModel = (LoginPageViewModel)param;
			//Check if the secret has been written to the RFID card
			if (loginPageViewModel != null)
			{
				if (!string.IsNullOrWhiteSpace(loginPageViewModel.Secret))
				{
					//Check the email address field is not null and is a valid email
					if (SecurityDataService.IsValidEmail(loginPageViewModel.UserEmail))
					{
						//Check the password field is not null
						if (!string.IsNullOrWhiteSpace(loginPageViewModel.UserPassword))
						{
							//Generate a random sessionId used to create a unique user session and ensure only one user is logged in at one time.
							//string sessionId = SecurityDataService.GeneratePassword(10, true, true, true, true);
							string sessionId = DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture) +
											   SecurityDataService.GenerateAsciiToken();
							string passwordAndSecret = loginPageViewModel.UserPassword +
													   loginPageViewModel.Secret;

							//User user = new User(
							//	SecurityDataService.HashAndSaltPassword(loginPageViewModel.UserPassword,
							//		Encoding.UTF8.GetBytes(loginPageViewModel.UserEmail), 256 / 8),
							//	loginPageViewModel.UserEmail, sessionId);
							User user = new User(
								SecurityDataService.HashAndSaltPassword(passwordAndSecret,
									Encoding.UTF8.GetBytes(loginPageViewModel.UserEmail), 256 / 8),
								loginPageViewModel.UserEmail, sessionId);

							//Attempt to authenticate the entered details with the server
							if (_syncDataService.AuthenticateUser(user))
							{
								if (File.Exists("Secret.Vault"))
								{
									//If the user fails to authenticate with the current local vault,
									//their backup is loaded from the vault server
									if (_vaultDataService.AuthenticateUser(loginPageViewModel.UserEmail,
										passwordAndSecret))
									{
										//Load the corresponding users most up to date vault from the server
										_syncDataService.RequestDatabase();
										//Load all data from the vault into memory
										_vaultDataService.LoadData();
										ViewModel = new MainPageViewModel(ref _vaultDataService, ref _syncDataService,
											NFC);
									}
									else
									{
										//Load the corresponding users vault from the server
										_syncDataService.RequestDatabase();
										//Attempt to decrypt the vault using the supplied information
										if (_vaultDataService.AuthenticateUser(loginPageViewModel.UserEmail,
											passwordAndSecret))
										{
											//Load all data from the vault into memory
											_vaultDataService.LoadData();
											ViewModel = new MainPageViewModel(ref _vaultDataService,
												ref _syncDataService, NFC);
										}
										else
										{
											MessageBox.Show("Login details failed to decrypt database");
										}
									}

								}
								else
								{
									//Load the corresponding users vault from the server
									_syncDataService.RequestDatabase();

									//Attempt to decrypt the vault using the supplied information
									if (_vaultDataService.AuthenticateUser(loginPageViewModel.UserEmail,
										passwordAndSecret))
									{
										//Load all data from the vault into memory
										_vaultDataService.LoadData();
										ViewModel = new MainPageViewModel(ref _vaultDataService, ref _syncDataService,
											NFC);
									}
									else
									{
										MessageBox.Show("Password failed to decrypt database");
									}
								}
							}
							//If authentication fails, check if it was due to a wrong password or no connection
							else
							{
								if (_syncDataService.OfflineMode)
								{
									//Check that a vault file exists 
									if (File.Exists("Secret.Vault"))
									{
										//Attempt to decrypt the vault using the supplied information, the user logs in to the local vault in read only mode
										if (_vaultDataService.AuthenticateUser(loginPageViewModel.UserEmail,
											passwordAndSecret))
										{
											_syncDataService.AddLoggedInUser(user);
											MessageBox.Show("Logging in offline mode");
											//Load all data from the vault into memory
											_vaultDataService.LoadData();
											ViewModel = new MainPageViewModel(ref _vaultDataService,
												ref _syncDataService, NFC);
										}
										else
										{
											MessageBox.Show("Password failed to decrypt database");
										}
									}
								}
								else
								{
									MessageBox.Show("Incorrect login details");
								}
							}
						}
						else
						{
							MessageBox.Show("Input a valid Password");
						}
					}
					else
					{
						MessageBox.Show("Input a valid Email Address");
					}
				}
				else
				{
					MessageBox.Show("Scan a valid RFID card");
				}
			}
		}

		/// <summary>
		/// Triggered when create account button is pressed
		/// </summary>
		/// <param name="param"></param>
		private void Submit(object param)
		{
			var createAccountViewModel = (CreateAccountViewModel)param;

			//Check if the secret has been written to the RFID card
			if (!string.IsNullOrWhiteSpace(createAccountViewModel.Secret))
			{
				//Check the email address field is not null and is a valid email
				if (SecurityDataService.IsValidEmail(createAccountViewModel.NewUserEmail))
				{
					//Check the password fields are not null 
					if ((!string.IsNullOrWhiteSpace(createAccountViewModel.NewUserPassword)) &&
						(!string.IsNullOrWhiteSpace(createAccountViewModel.NewUserConfirmPassword)))
					{
						//Check the password fields match
						if (createAccountViewModel.NewUserConfirmPassword == createAccountViewModel.NewUserPassword)
						{
							//Generate a random sessionId used to create a unique user session and ensure only one user is logged in at one time.
							string sessionId = DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture) + SecurityDataService.GenerateAsciiToken();
							string passwordAndSecret = createAccountViewModel.NewUserPassword +
											createAccountViewModel.Secret;

							User user = new User(
								SecurityDataService.HashAndSaltPassword(passwordAndSecret,
									Encoding.UTF8.GetBytes(createAccountViewModel.NewUserEmail), 256 / 8),
								createAccountViewModel.NewUserEmail, sessionId);

							//Attempt to create new user with the supplied email and password, if the email has been used before this will fail
							if (_syncDataService.CreateUser(user))
							{
								//Generate hash and salt of new users password
								byte[] Salt = SecurityDataService.GenerateRandomBytes(64 / 8);
								byte[] Hash =
									SecurityDataService.HashAndSaltPassword(passwordAndSecret,
										Salt,
										256 / 8);

								//Generate the master database encryption key
								var masterAesKey = SecurityDataService.GenerateAesKey();

								//Encrypt master key
								var eMasterKey = SecurityDataService.EncryptBytesAES(masterAesKey.Key, Hash);

								var sanityCheck = SecurityDataService.DecryptStringAES(eMasterKey, Hash);

								if (!masterAesKey.Key.SequenceEqual(sanityCheck))
								{
									MessageBox.Show("Failed encryption sanity check");
								}

								//Create XML file to store users data
								_vaultDataService.CreateVault(user.Email, Salt, eMasterKey, Hash);

								//Fill the default data //ToDO: remove or make this more general
								_vaultDataService.AddSite(
									new Site(Guid.NewGuid(), "google.com", "Google", user.Email, "pass", "Social", "", true),
									masterAesKey);
								_vaultDataService.AddSite(
									new Site(Guid.NewGuid(), "facebook.com", "Facebook", user.Email, "pass123", "Bank", "",
										false), masterAesKey);
								_vaultDataService.AddSite(
									new Site(Guid.NewGuid(), "github.com", "Github", user.Email, "pass123!", "Social", "",
										true),
									masterAesKey);

								_vaultDataService.AddCategory(new NavigationItemDatabase("Social", "Social"));
								_vaultDataService.AddCategory(new NavigationItemDatabase("Bank", "Bank"));

								_syncDataService.AuthenticateUser(user);

								//Sync the vault with the server
								_syncDataService.SyncDatabase(user);

								ViewModel = new LoginPageViewModel(NFC);
							}
							else
							{
								if (!_syncDataService.OfflineMode)
								{
									MessageBox.Show("Email address has already been used before");
								}
								else
								{
									MessageBox.Show("Cannot connect to the server");
								}
							}
						}
						else
						{
							MessageBox.Show("Passwords do not match");
						}
					}
					else
					{
						MessageBox.Show("Input a valid Password");
					}
				}
				else
				{
					MessageBox.Show("Input a valid Email Address");
				}
			}
			else
			{
				MessageBox.Show("Scan a valid RFID card");
			}
		}

		/// <summary>
		/// Triggered when cancel button is pressed on create account page
		/// </summary>
		/// <param name="param"></param>
		private void Cancel(object param)
		{
			ViewModel = new LoginPageViewModel(NFC);
		}

		private ViewModelBase _viewModel;
		public ViewModelBase ViewModel
		{
			get { return _viewModel; }
			set
			{
				_viewModel = value;
				OnPropertyChanged($"ViewModel");
			}
		}

	}
}