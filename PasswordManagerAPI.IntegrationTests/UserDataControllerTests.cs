﻿using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using PasswordManagerAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;


namespace PasswordManagerAPI.IntegrationTests
{
	public class UserDataControllerTests
	{
		protected readonly HttpClient Client;

		public UserDataControllerTests()
		{
			var appFactory = new WebApplicationFactory<Startup>();
			Client = appFactory.CreateClient();
		}


		[Test, Order(1)]
		public void PutCreateUserTest()
		{
			User user = new User(Convert.FromBase64String("KCVKo2/VwYSi3cLW7GRBjB2txVmKdqO1/CiX1KN5N2o="),
				"pete@gmail.com");

			var response = Client.PutAsJsonAsync("api/UserData/createUser", user).Result;

			Assert.True(response.IsSuccessStatusCode);
		}

		[Test, Order(2)]
		public void PostAuthenticateTest()
		{
			string email = Guid.NewGuid().ToString();

			User user = new User(Convert.FromBase64String("KCVKo2/VwYSi3cLW7GRBjB2txVmKdqO1/CiX1KN5N2o="),
				"pete@gmail.com", "04/12/2021 10:47:17QB[WfCcRbGUwBP[mOF\\AnTCOcQC\\uWV]");

			var response = Client.PostAsJsonAsync("api/UserData/authenticate", user).Result;

			Assert.True(response.IsSuccessStatusCode);
		}

		[Test, Order(3)]
		public void PostSyncDatabaseTest()
		{
			string vault =
				"ilqF+i+tMxw= BREAK DAAAABk3E8WCCGopEomfJBAAAABG7+0s3tz4xz0LwodwYVy6Qe/fFyLaSgIu5fSOh5+V3LtzuWxwwAhqEMf3NyFj0MjXEpWPx5O0wJ7KrElFLGm101F9kB2QNZDLzGoesUMByyC6EnM/rHbYdobNIACTtUhz2yRzGQU6eS/uOUOINPc0kD7jzJX3n0ZlcpAfziGCp9PPxpm883I60OVU+rKlezYsS4b5xreA8exkB29tyJiE8b9tFBuuWqafr1dsTAf9ryFuEJnC47HYFvpO4wFp+vYwae8o7QZa6Qg6UUv63eS6YwtfsLJehS1Gxyxd9kdirmr5AEIg2iskvV3dqdisgds5W3GqBEY0dUppJ/menVbGHL/cTwJVZI+WXkf7zeZYdOhkjOkJwBfdvbmnIG5qVUmvItWrDXMrWcGOK2cTDr4dJ/XPH6MW/+7eOUqN4ZQcVPSxqtLoiU2Gm1sG1XXzyj0+VPjYtgdHIDqH4YKFT4sDOk9XyMK6eYuGqVIel5vC9yUUO+j/huHSDYTAotrpqkQzStaUxYioox2XfAHXeMjNMU2X2DVQHwxkU7eDqDQodhYL3QAkzW/PybQPVFbqRinmaReucKT20G1rDCAGVvgAk9OIRs+V6RPlkv/ZUWpcNRxcGVAXB2Rvq3txVXDNApnln8YZq7wG7A40FMmlpn76RDLD38WHsvyKcW8u6dTdLCu6Z8Nhw5CxmzLrw3Q2QJzOCt7k/21faOfgPn+0vm1ZsRzouWGh9WIgaUoBJ+cFasHgI10Lqo9T0Yzwoz7aheDXpSudnB0tu2slGkdtK/w70uigR9VcxtnfxJ/uW4PovShuh/Qf4nCPVeNdexnsf8Ig7JUZQQLM+YPPU1PwFCaXbIrG5ovImy6TOba6VyCxMQ5+cpGC0rTqI8ZbFzfDEEwVjaYe5RgjPBlrztAENLdjMqE1CISF8HkEJ1m3q2NlQwvvZUwqVgjsByfcOyO4Wj8Ssjbz8oPrrYU/sGlpAdW5AUMNSA0WXGoaj5GqrQWqzoP+XaUvgkD4Bv3EcjZcbHGQKnvdujjGKwlT+FUnDEUiuSDLA7i93z3iU6u1p/wuyfUHKufRT48691ac0FENPDdWEGPKur3cGuo5V+JBisxb/gys0OLUd7V8PJ4b7FKshfGcFsviWGYq1HqoIfP6Vd/QaaktdPJZf5DqMDTkcijnxLJ92jOsFUQj5N2Q5ZQ/yAytLZ/GtSAYNSQh3e0OJMjxbXgwWCrUzwz4yDm8SC0JPzxqYY3TzVsrXB8jMglEAbxI6AeSM0GUGmr61FzSw+rIQihcyWbuQzjVdOOfm1DPALfKE2LOmLWckslnxSpxGFvlHI6EuZHGP9E9AhAqJTuBVFNaddMKhnNNoFHBIt9/VjnoxIQ5yMTollkI9l9lL6INJDAdHGECOdpowcMtua9b2uY1tfhNOrNMUPYiNnZH+sWXED2sm5Jv4e2/3fQ3LCwkpW1R4N6tF5f+WgOfVDggfh22D2LFS6R3aZUI29X79ucgnhP66LAf9yLIzxSamDhQoPGiPCkeXd/d2U+bvqgYHMIOCQEQRq81SyI2pU4j+ZVbJMeIv+FMsxXwfyEbNGln26MRpz94xk+ABJB/UKrZ0/yZi6ZpOkxcS+zRNm08E+lVHBVxr8u9zbO4sQfP+GMrwWeiGAogilssCFs8f/0dBfZzWCNrsVw0e7PuDnc7YT8wgC+x/yjVJ0cgOuWAPCe4TVOU+A==";

			User user = new User(Convert.FromBase64String("KCVKo2/VwYSi3cLW7GRBjB2txVmKdqO1/CiX1KN5N2o="),
				"pete@gmail.com", "04/12/2021 10:47:17QB[WfCcRbGUwBP[mOF\\AnTCOcQC\\uWV]");

			Client.DefaultRequestHeaders.Clear();
			Client.DefaultRequestHeaders.Add("SessionId", user.SessionId);
			Client.DefaultRequestHeaders.Add("Email", user.Email);

			var response = Client.PostAsJsonAsync("api/UserData/syncDatabase", vault).Result;

			Assert.True(response.IsSuccessStatusCode);
		}

		

		[Test, Order(4)]
		public void GetRequestVaultTest()
		{
			string email = Guid.NewGuid().ToString();

			User user = new User(Convert.FromBase64String("KCVKo2/VwYSi3cLW7GRBjB2txVmKdqO1/CiX1KN5N2o="),
				"pete@gmail.com", "04/12/2021 10:47:17QB[WfCcRbGUwBP[mOF\\AnTCOcQC\\uWV]");

			Client.DefaultRequestHeaders.Clear();
			Client.DefaultRequestHeaders.Add("SessionId", user.SessionId);
			Client.DefaultRequestHeaders.Add("Email", user.Email);


			var response = Client.GetStringAsync("api/UserData/requestDatabase").Result;

			Assert.True(response != null);
		}

		
		[Test, Order(5)]
		public void PostChangePasswordTest()
		{
			byte[] newPassword = Encoding.UTF8.GetBytes("thisisapassword");
			User user = new User(Convert.FromBase64String("KCVKo2/VwYSi3cLW7GRBjB2txVmKdqO1/CiX1KN5N2o="),
				"pete@gmail.com", "04/12/2021 10:47:17QB[WfCcRbGUwBP[mOF\\AnTCOcQC\\uWV]");

			Client.DefaultRequestHeaders.Clear();
			Client.DefaultRequestHeaders.Add("SessionId", user.SessionId);
			Client.DefaultRequestHeaders.Add("Email", user.Email);


			var response = Client.PostAsJsonAsync("api/UserData/changePassword", newPassword).Result;

			Assert.True(response.IsSuccessStatusCode);
		}
		
		[Test, Order(6)]
		public void DeleteAccountTest()
		{
			User user = new User(Convert.FromBase64String("KCVKo2/VwYSi3cLW7GRBjB2txVmKdqO1/CiX1KN5N2o="),
				"pete@gmail.com", "04/12/2021 10:47:17QB[WfCcRbGUwBP[mOF\\AnTCOcQC\\uWV]");

			Client.DefaultRequestHeaders.Clear();
			Client.DefaultRequestHeaders.Add("SessionId", user.SessionId);
			Client.DefaultRequestHeaders.Add("Email", user.Email);


			var response = Client.DeleteAsync("api/UserData/deleteAccount").Result;

			Assert.True(response.IsSuccessStatusCode);
		}

	}
}