using System;

namespace PasswordManagerClient.Models
{
	public class User
	{

		public User()
		{
			MasterPasswordHash = null;
			Email = null;
			Id = 0;
			SessionId = null;
		}

		public User(byte[] masterPasswordHash, string email)
		{
			MasterPasswordHash = masterPasswordHash;
			Email = email;
			Id = 0;
			SessionId = null;
		}

		public User(byte[] masterPasswordHash, string email, string sessionId)
		{
			MasterPasswordHash = masterPasswordHash;
			Email = email;
			Id = 0;
			SessionId = sessionId;
		}

		public long Id { get; set; }
		public byte[] MasterPasswordHash { get; set; }
		public string Email { get; set; }
		public string SessionId { get; set; }
	}
}
