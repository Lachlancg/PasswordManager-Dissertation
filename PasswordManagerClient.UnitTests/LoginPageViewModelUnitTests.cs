using System.Threading;
using NUnit.Framework;
using PasswordManagerClient.DataService;
using PasswordManagerClient.ViewModels;

namespace PasswordManager
{
	public class LoginPageViewModelUnitTests
	{
		[Test, Apartment(ApartmentState.STA)]
		public void EmailTest()
		{
			/* Begin Reference - Modified Code
			BFree. ‘Unit testing the Viewmodel’. Accessed 03 March 2021. https://stackoverflow.com/questions/4845332/unit-testing-the-viewmodel.
			*/
			LoginPageViewModel loginPageViewModel = new LoginPageViewModel(new NFCDataService());
			bool propertyChanged = false;

			
			loginPageViewModel.PropertyChanged += (s, e) =>
				{
					//Check that the property firing is correct 
					Assert.AreEqual("NewUserEmail", e.PropertyName);
					//Check the value it has been changed to is correct
					Assert.AreEqual("test@gmail.com", loginPageViewModel.UserEmail);
					propertyChanged = true;
				};

			loginPageViewModel.UserEmail = "test@gmail.com";
			//Check the property changed event fired
			Assert.IsTrue(propertyChanged);
			/* End Reference */
		}

		[Test, Apartment(ApartmentState.STA)]
		public void PasswordTest()
		{
			/* Begin Reference - Modified Code
			BFree. ‘Unit testing the Viewmodel’. Accessed 03 March 2021. https://stackoverflow.com/questions/4845332/unit-testing-the-viewmodel.
			*/
			LoginPageViewModel loginPageViewModel = new LoginPageViewModel(new NFCDataService());
			bool propertyChanged = false;


			loginPageViewModel.PropertyChanged += (s, e) =>
			{
				//Check that the property firing is correct 
				Assert.AreEqual("NewUserPassword", e.PropertyName);
				//Check the value it has been changed to is correct
				Assert.AreEqual("password", loginPageViewModel.UserPassword);
				propertyChanged = true;
			};

			loginPageViewModel.UserPassword = "password";
			//Check the property changed event fired
			Assert.IsTrue(propertyChanged);
			/* End Reference */
		}

		[Test, Apartment(ApartmentState.STA)]
		public void RfidPinTest()
		{
			/* Begin Reference - Modified Code
			BFree. ‘Unit testing the Viewmodel’. Accessed 03 March 2021. https://stackoverflow.com/questions/4845332/unit-testing-the-viewmodel.
			*/
			LoginPageViewModel loginPageViewModel = new LoginPageViewModel(new NFCDataService());
			bool propertyChanged = false;


			loginPageViewModel.PropertyChanged += (s, e) =>
			{
				//Check that the property firing is correct 
				Assert.AreEqual("RFIDPin", e.PropertyName);
				//Check the value it has been changed to is correct
				Assert.AreEqual("123456", loginPageViewModel.RFIDPin);
				propertyChanged = true;
			};

			loginPageViewModel.RFIDPin = "123456";
			//Check the property changed event fired
			Assert.IsTrue(propertyChanged);
			/* End Reference */
		}
	}
}