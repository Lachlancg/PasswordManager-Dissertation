using System;
using System.Buffers.Binary;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Konscious.Security.Cryptography;
using System.Text.RegularExpressions;


namespace PasswordManagerClient.DataService
{
	public class SecurityDataService
	{
		//Special 
		private const string SpecialCharString = "~!£$%^&*_+-@¬#;:<>/?";

		/// <summary>
		/// Returns Hash and salt of plaintext password
		/// </summary>
		/// <param name="plainTextPass"></param>
		/// <returns> byte[] Hash</returns>
		public static byte[] HashAndSaltPassword(string plainTextPass, byte[] salt, int size)
		{
			var argon2 = new Argon2i(Encoding.UTF8.GetBytes(plainTextPass))
			{
				Salt = salt, DegreeOfParallelism = 8, MemorySize = 8192, Iterations = 40
			};

			//number of cores used: 4
			//Amount of memory used when computing hash 
			//Number of iterations to compute hash

			return (argon2.GetBytes(size));
		}

		/// <summary>
		///Generates a random salt value
		/// </summary>
		/// <returns>byte[] Salt</returns>
		public static byte[] GenerateRandomBytes(int size)
		{
			//Generate random 64 bit salt
			RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
			byte[] randomBytes = new byte[size];

			rnd.GetBytes(randomBytes);

			return randomBytes;
		}

		/// <summary>
		/// Generates a random 256bit Aes encryption key
		/// </summary>
		/// <returns></returns>
		public static Aes GenerateAesKey()
		{
			Aes aesKey = new AesCryptoServiceProvider
			{
				KeySize = 256
			};
			aesKey.GenerateKey();
			aesKey.GenerateIV();

			return aesKey;
		}

		/* Begin Reference - Modified Code
         janw. ‘c# - Using the AesGcm class’. Accessed 24 January 2021. https://stackoverflow.com/questions/60889345/using-the-aesgcm-class.
		*/
		/// <summary>
		/// Takes the master encryption key and encrypts it using the hashed password
		/// </summary>
		/// <param name="masterKey"></param>
		/// <param name="hash"></param>
		/// <returns></returns>
		public static string EncryptBytesAES(byte[] plaintext, byte[] key)
		{
			//Get the parameter sizes
			int nonceSize = AesGcm.NonceByteSizes.MaxSize;
			int tagSize = AesGcm.TagByteSizes.MaxSize;
			int cipherSize = plaintext.Length;

			//Write everything to one array to make encoding and storage easier
			int encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
			Span<byte> encryptedData = encryptedDataLength < 1024
				? stackalloc byte[encryptedDataLength]
				: new byte[encryptedDataLength].AsSpan();

			//Copy the parameters into temporary variables
			BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(0, 4), nonceSize);
			BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);

			var nonce = encryptedData.Slice(4, nonceSize);
			var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
			var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

			//Generate random bytes for secure nonce
			//nonce = GenerateRandomBytes(nonceSize);
			RandomNumberGenerator.Fill(nonce);

			//Encrypt the data using the hashed password
			AesGcm aesGCM = new AesGcm(key);
			aesGCM.Encrypt(nonce, plaintext, cipherBytes, tag);

			//Encode the data 
			return Convert.ToBase64String(encryptedData);
		}



		/// <summary>
		/// Takes the encrpyted master encryption key and decrypts it using the hashed password
		/// </summary>
		/// <param name="masterKeyCipher"></param>
		/// <param name="hash"></param>
		/// <returns></returns>
		public static byte[] DecryptStringAES(string cipherText, byte[] key)
		{
			//Decode the master key cipher from string 
			Span<byte> encryptedData = Convert.FromBase64String(cipherText).AsSpan();

			//Extract parameter sizes from encryptedData
			int nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(0, 4));
			int tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
			int cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;

			//Extract the parameters from the encryptedData
			var nonce = encryptedData.Slice(4, nonceSize);
			var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
			var cipher = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);


			//Decrypt the cipher data
			byte[] masterKey = new byte[cipherSize];
			AesGcm aesGcm = new AesGcm(key);
			try
			{
				aesGcm.Decrypt(nonce, cipher, tag, masterKey);
			}
			catch (Exception)
			{
				masterKey = null;
			}

			return masterKey;
		}

		/* End Reference */

		/// <summary>
		/// Takes attributes and generates a password
		/// </summary>
		/// <param name="length"></param>
		/// <param name="lowerCase"></param>
		/// <param name="upperCase"></param>
		/// <param name="numbers"></param>
		/// <param name="specialChars"></param>
		/// <returns></returns>
		private static string GeneratePasswordFunction(int length, bool lowerCase, bool upperCase, bool numbers, bool specialChars)
		{
			string upperCaseString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string lowerCaseString = "abcdefghijklmnopqrstuvwxyz";
			string numbersString = "123456789";

			string allCharsString = "";

			var password = new char[length];

			if (lowerCase)
			{
				allCharsString += lowerCaseString;
			}
			if (upperCase)
			{
				allCharsString += upperCaseString;
			}
			if (numbers)
			{
				allCharsString += numbersString;
			}
			if (specialChars)
			{
				allCharsString += SpecialCharString;
			}


			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

			byte[] randomBytes = new byte[length];
			rng.GetBytes(randomBytes);

			if (allCharsString == "")
			{
				return "";
			}

			for (int i = 0; i < length; i++)
			{
				var x = randomBytes[i] % allCharsString.Length;

				password[i] = allCharsString[x];
			}

			return new string(password);
		}

		/// <summary>
		/// This function checks the generated password contains the necessary attributes and generates a new one if not
		/// </summary>
		/// <param name="length"></param>
		/// <param name="lowerCase"></param>
		/// <param name="upperCase"></param>
		/// <param name="numbers"></param>
		/// <param name="specialChars"></param>
		/// <returns></returns>
		public static string GeneratePassword(int length, bool lowerCase, bool upperCase, bool numbers, bool specialChars)
		{
			bool validPass = false;
			string password = "";
			while (!validPass)
			{
				password = GeneratePasswordFunction(length, lowerCase, upperCase, numbers, specialChars);
				validPass = true;
				if (password.Length > 3)
				{
					if (lowerCase && !password.Any(char.IsLower))
					{
						validPass = false;
					}

					if (upperCase && !password.Any(char.IsUpper))
					{
						validPass = false;
					}

					if (numbers && !password.Any(char.IsDigit))
					{
						validPass = false;
					}

					if (specialChars && password.IndexOfAny(SpecialCharString.ToCharArray()) == -1)
					{
						validPass = false;
					}
				}
			}

			return password;
		}

		/// <summary>
		/// Returns a value between 0 - 5 based on the elements of the password string.
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static int PasswordStrengthCalculator(string password)
		{
			int score = 0;
			if (password != null)
			{
				if (password.Length >= 5) //If password less than 5 it always scores 0
				{
					if (password.Any(char.IsUpper) && password.Any(char.IsLower)
					) //Check if password contains upper and lower case value
					{
						score++;
					}

					if (password.Any(char.IsDigit)) //Check if password contains a number
					{
						score++;
					}

					if (password.Length >= 8) //Check if the password is over 8 characters
					{
						score++;
						if (password.Length >= 12) //Check if password is over 12 characters
						{
							score++;
						}

						if (password.IndexOfAny(SpecialCharString.ToCharArray()) != -1
						) //Check if password contains a special character
						{
							score++;
						}
					}
				}
			}

			return score;
		}

		public static Brush PasswordStrengthColour(int score)
		{
			Brush passwordColour = Brushes.AntiqueWhite;

			if (score == 0)
			{
				passwordColour = Brushes.Red;
			}
			else if (score >= 1 && score <= 2)
			{
				passwordColour = Brushes.Orange;
			}
			else if (score == 3)
			{
				passwordColour = Brushes.GreenYellow;
			}
			else if (score >= 4)
			{
				passwordColour = Brushes.Green;
			}

			return passwordColour;
		}

		public static string GenerateAsciiToken()
		{
			/* Begin Reference - Modified Code
			khalilovcmd. ‘randomizer.cs’. Accessed 22 February 2021. https://gist.github.com/khalilovcmd/494988c43b6c205f9d76.
			*/
			int asciiCharacterStart = 65; // from which ascii character code the generation should start
			int asciiCharacterEnd = 122; // to which ascii character code the generation should end
			int characterCount = 32; // count of characters to generate


			Random random = new Random(DateTime.Now.Millisecond);
			StringBuilder builder = new StringBuilder();

			// iterate, get random int between 'asciiCharacterStart' and 'asciiCharacterEnd', then convert it to (char), append to StringBuilder
			for (int i = 0; i < characterCount; i++)
				builder.Append((char)(random.Next(asciiCharacterStart, asciiCharacterEnd + 1) % 255));

			return builder.ToString();
			/* End Reference */

		}


		/* Begin Reference - Copied Code
			adegeo. ‘How to verify that strings are in valid email format’. Accessed 13 March 2021. https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format.
			*/
		public static bool IsValidEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				return false;

			try
			{
				// Normalize the domain
				email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
					RegexOptions.None, TimeSpan.FromMilliseconds(200));

				// Examines the domain part of the email and normalizes it.
				static string DomainMapper(Match match)
				{
					// Use IdnMapping class to convert Unicode domain names.
					var idn = new IdnMapping();

					// Pull out and process domain name (throws ArgumentException on invalid)
					string domainName = idn.GetAscii(match.Groups[2].Value);

					return match.Groups[1].Value + domainName;
				}
			}
			catch (RegexMatchTimeoutException)
			{
				return false;
			}
			catch (ArgumentException)
			{
				return false;
			}

			try
			{
				return Regex.IsMatch(email,
					@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
					RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
			}
			catch (RegexMatchTimeoutException)
			{
				return false;
			}
		}
		/* End Reference */
	}
}