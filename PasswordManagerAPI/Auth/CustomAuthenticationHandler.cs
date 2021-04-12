using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using PasswordManagerAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PasswordManagerAPI.Auth
{
	public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		private Models.UserContext DbContext { get; set; }


		public CustomAuthenticationHandler(
			IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock,
			Models.UserContext dbContext)
			: base(options, logger, encoder, clock)
		{
			DbContext = dbContext;
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if ((!Request.Headers.ContainsKey("SessionId")) && (!Request.Headers.ContainsKey("Email")))
			{
				return Task.FromResult(AuthenticateResult.Fail("Not Authenticated"));
			}

			string sessionId;
			string email;

			try
			{
				sessionId = Request.Headers["SessionId"];
				email = Request.Headers["Email"];
			}
			catch (Exception)
			{
				return Task.FromResult(AuthenticateResult.Fail("Not Authenticated"));
			}

			DateTime timeStamp = DateTime.Parse(sessionId.Substring(0, 19), CultureInfo.InvariantCulture);

			//Search database for user with email and matching password hashed
			User user = UserDataService.FindUser(DbContext, email, sessionId);

			//Check that the returned user is not null and that the sessionId is still valid
			if ((user != null) && (timeStamp > DateTime.UtcNow.AddHours(-24) && timeStamp <= DateTime.UtcNow))
			{
				Claim[] claims = new Claim[2];
				claims[0] = new Claim(ClaimTypes.Name, user.Email);
				ClaimsIdentity identity = new ClaimsIdentity(claims, user.SessionId);
				ClaimsPrincipal principal = new ClaimsPrincipal(identity);

				AuthenticationTicket ticket = new AuthenticationTicket(principal, this.Scheme.Name);

				return Task.FromResult(AuthenticateResult.Success(ticket));
			}

			return Task.FromResult(AuthenticateResult.Fail("Not Authenticated"));
		}

		protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
		{
			byte[] messagebytes = Encoding.ASCII.GetBytes("Unauthorized. Check SessionId and Email in Header are correct.");
			Context.Response.StatusCode = 401;
			Context.Response.ContentType = "application/json";
			await Context.Response.Body.WriteAsync(messagebytes, 0, messagebytes.Length);
		}
	}
}
