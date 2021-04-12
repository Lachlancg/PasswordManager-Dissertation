using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using PasswordManagerAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PasswordManagerAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserDataController : ControllerBase
	{
		private readonly UserContext _context;

		public UserDataController(UserContext context)
		{
			_context = context;
		}


		// PUT api/<UserDataController>/createUser
		[HttpPut("createUser")]
		public IActionResult CreateUser([FromBody] User param)
		{
			//Search database Users for existing user using email from param
			if (UserDataService.UserExists(_context, param.Email))
			{
				return BadRequest();
			}

			//If does not exist add new user to database.
			if (UserDataService.AddUser(_context, param.Email, param.MasterPasswordHash))
			{
				return Ok();
			}
			return BadRequest();
		}

		// POST api/<UserDataController>/authenticate
		[HttpPost("authenticate")]
		public IActionResult Authenticate([FromBody] User param)
		{
			//Search database for user with email and matching password 
			if (UserDataService.AuthenticateUser(_context, param))
			{
				return Ok();
			}

			return BadRequest();

		}


		// POST api/<UserDataController>/syncDatabase
		[HttpPost("syncDatabase")]
		[Authorize]
		public IActionResult SyncDatabase([FromBody] string param)
		{
			//Retrieve email and session id from the header
			string sessionId = Request.Headers["SessionId"];
			string email = Request.Headers["Email"];


			//Search database for user with email and matching sessionId
			User user = UserDataService.FindUser(_context, email, sessionId);
			
			if (user != null)
			{
				//Save vault file to the server
				System.IO.Directory.CreateDirectory("vaults");
				System.IO.File.WriteAllText("vaults/" + email + ".vault", param);
				return Ok();
			}

			return BadRequest();
		}

		// GET api/<UserDataController>/requestDatabase
		[HttpGet("requestDatabase")]
		[Authorize]
		public IActionResult RequestDatabase()
		{
			//Retrieve email and session id from the header
			string sessionId = Request.Headers["SessionId"];
			string email = Request.Headers["Email"];

			//Search database for user with email and matching sessionId
			User user = UserDataService.FindUser(_context, email, sessionId);

			if (user != null)
			{
				//Read vault from file and send as a string
				var xmlString = System.IO.File.ReadAllText("vaults/" + email + ".vault");

				return Ok(xmlString);
			}

			return BadRequest();
		}

		// POST api/<UserDataController>/changePassword
		[HttpPost("changePassword")]
		[Authorize]
		public IActionResult ChangePassword([FromBody] byte[] param)
		{
			//Retrieve email and session id from the header
			string sessionId = Request.Headers["SessionId"];
			string email = Request.Headers["Email"];

			//Search database for user with email and matching sessionId
			User user = UserDataService.FindUser(_context, email, sessionId);

			if (user != null)
			{
				//Save new password hash in dB
				user.MasterPasswordHash = param;
				_context.SaveChanges();
				return Ok();
			}

			return BadRequest();
		}

		// DELETE api/<UserDataController>/deleteAccount
		[HttpDelete("deleteAccount")]
		[Authorize]
		public IActionResult DeleteAccount()
		{
			//Retrieve email and session id from the header
			string sessionId = Request.Headers["SessionId"];
			string email = Request.Headers["Email"];

			//Search database for user with email and matching sessionId
			User user = _context.Users.SingleOrDefault(x => x.Email == email && x.SessionId == sessionId);
			if (user != null)
			{
				//Remove user from dB and delete vault file
				_context.Users.Remove(user);
				System.IO.File.Delete("vaults/" + email + ".vault");
				_context.SaveChanges();
				return Ok();
			}
			return BadRequest();
		}

		//This is a health check for the server, it returns an ok response if everything is okay.
		// POST api/<UserDataController>/pollServer
		[HttpGet("pollServer")]
		[Authorize]
		public IActionResult PollServer()
		{
			//Retrieve email and session id from the header
			string sessionId = Request.Headers["SessionId"];
			string email = Request.Headers["Email"];

			//Search database for user with email and matching sessionId
			User user = _context.Users.SingleOrDefault(x => x.Email == email &&  x.SessionId == sessionId);
			if (user != null)
			{
				return Ok();
			}

			return BadRequest();
		}

	}
}
