using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PasswordManagerClient.DataService;

namespace PasswordManager
{
	public class SecurityTests
	{

		[SetUp]
		public void Setup()
		{
		}

		[TestCase(" ", 0)]
		[TestCase("helloworld", 1)]
		[TestCase("HelloWorld", 2)]
		[TestCase("helloworld12", 3)]
		[TestCase("HelloWorld12", 4)]
		[TestCase("HelloWorld1!", 5)]
		public void TestPassStrengthCalc(string plainTextPass, int expected)
		{
			int actual = SecurityDataService.PasswordStrengthCalculator(plainTextPass);
			Assert.AreEqual(expected, actual);
		}

		/* Begin Reference - Copied Code
		JaredPar. ‘How can I convert a hex string to a byte array?’. Accessed 03 March 2021. https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array.
	   */
		public static byte[] StringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
				.ToArray();
		}
		/* End Reference */


		[Test]
		public void TestHashAndSalt()
		{
			byte[] expected = StringToByteArray("FF2CE626544A11FA707CE8F28CA3516C6381B25877AC5825BA7ADF051102C45C");

			byte[] salt = Encoding.UTF8.GetBytes("ThisIsATestSalt");
			byte[] actual = SecurityDataService.HashAndSaltPassword("HelloWorld1!", salt, 32);

			Assert.AreEqual(expected, actual);
		}

		[TestCase(0, true, true, true, true, 0)]
		[TestCase(2, true, true, true, true, 2)]
		[TestCase(8, true, true, true, true, 8)]
		[TestCase(12, true, true, true, true, 12)]
		[TestCase(64, true, true, true, true, 64)]
		[TestCase(128, true, true, true, true, 128)]
		public void TestPasswordGenLength(int length, bool lowerCase, bool upperCase, bool numbers, bool specialChars, int expected)
		{
			string actual = SecurityDataService.GeneratePassword(length, lowerCase, upperCase, numbers, specialChars);

			Assert.AreEqual(expected, actual.Length);
		}

		[TestCase(12, true, true, true, true, true)]
		[TestCase(12, false, true, true, true, false)]
		public void TestPasswordGenLowerCase(int length, bool lowerCase, bool upperCase, bool numbers, bool specialChars, bool expected)
		{
			bool actual = (SecurityDataService.GeneratePassword(length, lowerCase, upperCase, numbers, specialChars)).Any(char.IsLower);

			Assert.AreEqual(expected, actual);
		}

		[TestCase(12, true, true, true, true, true)]
		[TestCase(12, true, false, true, true, false)]
		public void TestPasswordGenUpperCase(int length, bool lowerCase, bool upperCase, bool numbers, bool specialChars, bool expected)
		{
			bool actual = (SecurityDataService.GeneratePassword(length, lowerCase, upperCase, numbers, specialChars)).Any(char.IsUpper);

			Assert.AreEqual(expected, actual);
		}

		[TestCase(12, true, true, true, true, true)]
		[TestCase(12, true, true, false, true, false)]
		public void TestPasswordGenDigit(int length, bool lowerCase, bool upperCase, bool numbers, bool specialChars, bool expected)
		{
			bool actual = (SecurityDataService.GeneratePassword(length, lowerCase, upperCase, numbers, specialChars)).Any(char.IsDigit);

			Assert.AreEqual(expected, actual);
		}

		[TestCase(12, true, true, true, true, true)]
		[TestCase(12, true, true, true, false, false)]
		public void TestPasswordGenSpecial(int length, bool lowerCase, bool upperCase, bool numbers, bool specialChars, bool expected)
		{
			string SpecialCharString = "~!£$%^&*_+-@¬#;:<>/?";
			bool actual = (SecurityDataService.GeneratePassword(length, lowerCase, upperCase, numbers, specialChars)).IndexOfAny(SpecialCharString.ToCharArray()) != -1;

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestEncryptionSanityCheck()
		{
			byte[] Salt = SecurityDataService.GenerateRandomBytes(64 / 8);
			byte[] plaintext = SecurityDataService.GenerateRandomBytes(10);
			byte[] hash = SecurityDataService.HashAndSaltPassword(Encoding.UTF8.GetString(plaintext), Salt,256 / 8);

			var encryptedString = SecurityDataService.EncryptBytesAES(plaintext, hash);

			byte[] plaintextResult = SecurityDataService.DecryptStringAES(encryptedString, hash);

			Assert.AreEqual(plaintext, plaintextResult);

		}
	}
}