using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace PasswordManagerAPI.Models
{
    public class User
    {

        public User()
        {
            MasterPasswordHash = null;
            Email = null;
            Id = 0;
        }

        public User(byte[] masterPasswordHash, string email)
        {
            MasterPasswordHash = masterPasswordHash;
            Email = email;
        }
        public User(byte[] masterPasswordHash, string email, string sessionId)
        {
	        MasterPasswordHash = masterPasswordHash;
	        Email = email;
	        SessionId = sessionId;
        }

        [Key]
        public long Id { get; set; }
        public byte[] MasterPasswordHash { get; set; }
        public string Email { get; set; }
        public string SessionId { get; set; }

    }

    public static class UserDataService
    {
	    public static User FindUser(UserContext context, string email, string sessionId)
	    {
		    User user = context.Users.SingleOrDefault(x => x.SessionId == sessionId && x.Email == email);

		    return user;
	    }

	    public static bool AuthenticateUser(UserContext context, User userParam)
	    {
		    //Search database for user with email and matching password 
		    User user = context.Users.SingleOrDefault(x => x.MasterPasswordHash.SequenceEqual(userParam.MasterPasswordHash) && x.Email == userParam.Email);

		    if (user != null)
		    {
			    //Set the users session ID
			    user.SessionId = userParam.SessionId;
			    context.SaveChanges();
			    return true;
		    }

		    return false;
	    }

		public static bool UserExists(UserContext context, string email)
	    {
		    User user = context.Users.SingleOrDefault(x => x.Email == email);

		    return user != null;
	    }

		

		public static bool AddUser(UserContext context, string email, byte[] passwordHash)
	    {
		    try
		    {
			    context.Users.Add(new User(passwordHash, email));
			    context.SaveChanges();
			    return true;
		    }
		    catch (Exception)
		    {
			    return false;
		    }
	    }
    }
}