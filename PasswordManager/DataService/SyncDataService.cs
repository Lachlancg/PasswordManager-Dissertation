using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using PasswordManagerClient.Models;

namespace PasswordManagerClient.DataService
{
	public class SyncDataService
	{

		private User _loggedInUser;
		
		public bool OfflineMode { get; set; }


		/// <summary>
		/// Sets the loggedInUser 
		/// </summary>
		/// <param name="user"></param>
		public void AddLoggedInUser(User user)
		{
			_loggedInUser = user;
		}


		/// <summary>
		/// Method attempts to connect to server to authenticate a user
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool AuthenticateUser(User user)
		{
			try
			{
				HttpClient client = new HttpClient();

				var response = client.PostAsJsonAsync("https://localhost:44322/api/UserData/authenticate", user).Result;
				OfflineMode = false;
				if (response.IsSuccessStatusCode)
				{
					_loggedInUser = user;
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception)
			{
				OfflineMode = true;
				return false;
			}
		}

		/// <summary>
		/// Function polls the server to check if the client is connected
		/// </summary>
		/// <returns></returns>
		public bool PollServer()
		{
			try
			{
				HttpClient client = new HttpClient();

				//Create HTTP Headers
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Add("SessionId", _loggedInUser.SessionId);
				client.DefaultRequestHeaders.Add("Email", _loggedInUser.Email);

				var response = client.GetAsync("https://localhost:44322/api/UserData/pollServer").Result;

				if (response.IsSuccessStatusCode)
				{
					return true;
					//We dont put them into online mode as the user needs to authenticate first
				}
				//A response is not a success if a users login details are incorrect or more likely the session id is wrong
				OfflineMode = true;

				return false;

			}
			catch (Exception)
			{
				//If no response from the server then the user is put into offline mode.
				OfflineMode = true;
				return false;
			}
		}

		/// <summary>
		/// Function sends a request to the server to authenticate
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool CreateUser(User user)
		{
			try
			{
				HttpClient client = new HttpClient();

				var response = client.PutAsJsonAsync("https://localhost:44322/api/UserData/createUser", user).Result;

				//A response is not a success if a user with the same details exists on the server
				if (response.IsSuccessStatusCode)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception)
			{
				OfflineMode = true;
				return false;
			}

		}

		/// <summary>
		/// Function sends the local copy of the database to the server
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool SyncDatabase(User user = null)
		{
			try
			{
				var xmlString = File.ReadAllText("Secret.vault");

				User activeUser = user ?? _loggedInUser;

				HttpClient client = new HttpClient();

				//Create HTTP Headers
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Add("SessionId", activeUser.SessionId);
				client.DefaultRequestHeaders.Add("Email", activeUser.Email);

				var response = client.PostAsJsonAsync("https://localhost:44322/api/UserData/syncDatabase", xmlString)
					.Result;

				if (response.IsSuccessStatusCode)
				{
					return true;
				}

				return false;
			}
			catch (Exception)
			{
				OfflineMode = true;
				return false;
			}
		}

		/// <summary>
		/// Function requests the latest copy of the database from the server
		/// </summary>
		public void RequestDatabase()
		{
			try
			{
				//Create HTTP Headers
				HttpClient client = new HttpClient();
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Add("SessionId", _loggedInUser.SessionId);
				client.DefaultRequestHeaders.Add("Email", _loggedInUser.Email);

				var response = client
					.GetStringAsync("https://localhost:44322/api/UserData/requestDatabase").Result;

				if (response != "")
				{
					System.IO.File.WriteAllText("Secret.vault", response);
				}
			}
			catch (Exception)
			{
				OfflineMode = true;
			}

		}

		/// <summary>
		/// Function sends a request to the server to change the users password
		/// </summary>
		/// <param name="newPassword"></param>
		/// <returns></returns>
		public bool ChangePassword(string newPassword)
		{
			try
			{
				byte[] newPasswordHash =
					SecurityDataService.HashAndSaltPassword(newPassword, Encoding.UTF8.GetBytes(_loggedInUser.Email),
						256 / 8);

				HttpClient client = new HttpClient();

				//Create HTTP Headers
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Add("SessionId", _loggedInUser.SessionId);
				client.DefaultRequestHeaders.Add("Email", _loggedInUser.Email);

				var response = client
					.PostAsJsonAsync("https://localhost:44322/api/UserData/changePassword", newPasswordHash).Result;

				if (response.IsSuccessStatusCode)
				{
					_loggedInUser.MasterPasswordHash = newPasswordHash;
					return true;
				}

				return false;
			}
			catch (Exception)
			{
				OfflineMode = true;
				return false;
			}
		}

		/// <summary>
		/// Function sends a request to the server to delete the current users account
		/// </summary>
		/// <returns></returns>
		public bool DeleteAccount()
		{
			try
			{
				HttpClient client = new HttpClient();

				//Create HTTP headers 
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Add("SessionId", _loggedInUser.SessionId);
				client.DefaultRequestHeaders.Add("Email", _loggedInUser.Email);

				var response = client
					.DeleteAsync("https://localhost:44322/api/UserData/deleteAccount").Result;

				if (response.IsSuccessStatusCode)
				{
					return true;
				}

				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}