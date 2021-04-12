using System.Threading;
using NUnit.Framework;
using PasswordManagerClient.DataService;
using PasswordManagerClient.ViewModels;

namespace PasswordManager
{
	public class CreateAccountPageViewModelUnitTests
	{

		[Test, Apartment(ApartmentState.STA)]
		public void EmailTest()
		{
			/* Begin Reference - Modified Code
			BFree. ‘Unit testing the Viewmodel’. Accessed 03 March 2021. https://stackoverflow.com/questions/4845332/unit-testing-the-viewmodel.
			*/
			CreateAccountViewModel createAccountViewModel = new CreateAccountViewModel(new NFCDataService());
			bool propertyChanged = false;


			createAccountViewModel.PropertyChanged += (s, e) =>
			{
				//Check that the property firing is correct 
				Assert.AreEqual("NewUserEmail", e.PropertyName);
				//Check the value it has been changed to is correct
				Assert.AreEqual("test@gmail.com", createAccountViewModel.NewUserEmail);
				propertyChanged = true;
			};

			createAccountViewModel.NewUserEmail = "test@gmail.com";
			//Check the property changed event fired
			Assert.IsTrue(propertyChanged);
			/* End Reference */
		}

		[Test, Apartment(ApartmentState.STA)]
		public void ConfirmPasswordTest()
		{
			/* Begin Reference - Modified Code
			BFree. ‘Unit testing the Viewmodel’. Accessed 03 March 2021. https://stackoverflow.com/questions/4845332/unit-testing-the-viewmodel.
			*/
			CreateAccountViewModel createAccountViewModel = new CreateAccountViewModel(new NFCDataService());
			bool propertyChanged = false;


			createAccountViewModel.PropertyChanged += (s, e) =>
			{
				//Check that the property firing is correct 
				Assert.AreEqual("NewUserConfirmPassword", e.PropertyName);
				//Check the value it has been changed to is correct
				Assert.AreEqual("password", createAccountViewModel.NewUserConfirmPassword);
				propertyChanged = true;
			};

			createAccountViewModel.NewUserConfirmPassword = "password";
			//Check the property changed event fired
			Assert.IsTrue(propertyChanged);
			/* End Reference */
		}
	}
}