using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ModernWpf.Controls;
using PasswordManagerClient.Models;

namespace PasswordManagerClient.DataService
{
	public class VaultDataService
	{
		private XDocument xmlDoc;

		//Encrypted master key
		private string _eMasterKey;
		private byte[] SaltVault;
		private byte[] HashVault;
 

		//Stores for vault data
		private List<Site> _siteData;
		private ObservableCollection<NavigationItem> _categoryData;
		private ObservableCollection<NavigationItem> _navData;

		//Event handlers for category data.
		public delegate void PropertyChangedHandler(object obj);
		public static event PropertyChangedHandler CategoriesChanged = delegate { };

		

		/// <summary>
		/// Create default vault elements
		/// </summary>
		/// <param name="salt"></param>
		/// <param name="eMasterKey"></param>
		/// <param name="IV"></param>
		public void CreateVault(string email, byte[] salt, string eMasterKey, byte[] vaultHash)
		{
			//Save the key and vault salt for later
			_eMasterKey = eMasterKey;
			SaltVault = salt;
			HashVault = vaultHash;

			//Create the default XML tree layout for the vault

			xmlDoc = new XDocument();

			XElement root = new XElement("Vault", xmlDoc.Root);
			root.Add(new XAttribute("GUID", Guid.NewGuid().ToString()));

			xmlDoc.Add(root);

			XElement saltNode = new XElement("Salt", Convert.ToBase64String(salt));
			xmlDoc.Element("Vault").Add(saltNode);

			XElement masterKeyNode = new XElement("Key", eMasterKey);
			xmlDoc.Element("Vault").Add(masterKeyNode);

			XElement emailNode = new XElement("Email", email);
			xmlDoc.Element("Vault").Add(emailNode);

			XElement sitesNode = new XElement("Sites");
			xmlDoc.Element("Vault").Add(sitesNode);

			XElement categoriesNode = new XElement("Categories");
			xmlDoc.Element("Vault").Add(categoriesNode);

			EncryptVault(xmlDoc);
			//xmlDoc.Save(xmlPath);
		}

		/// <summary>
		/// Load all data from the vault into memory
		/// </summary>
		public void LoadData()
		{
			LoadSites();
			LoadCategories();
		}

		/// <summary>
		/// Add or edit a site to the XML vault
		/// </summary>
		/// <param name="site"></param>
		/// <param name="key"></param>
		public void SaveSite(Site site)
		{
			xmlDoc = new XDocument();

			//Read in and decrypt the vault
			xmlDoc = DecryptVault();

			//Check if the site already exists in the vault, if so edit 
			if (!(_siteData.Exists(x => x.credGUID == site.credGUID)))
			{
				//Create a new Site and add the element to document
				XElement siteElement = new XElement("Site");
				XElement finalSiteElement = FormatElement(siteElement, site);
				xmlDoc.Descendants().SingleOrDefault(x => x.Name.LocalName == "Sites").Add(finalSiteElement);
			}
			else
			{
				//Find the existing Site and edit its contents
				XElement Site = xmlDoc.Root.Element("Sites");
				foreach (var element in Site.Elements())
				{
					if (element.Attribute("GUID").Value == site.credGUID.ToString())
					{
						XElement tempElement = new XElement("Site");
						element.ReplaceWith(FormatElement(tempElement, site));
					}
				}
			}
			EncryptVault(xmlDoc);
			//xmlDoc.Save(xmlPath);
			LoadData();
		}

		/// <summary>
		/// Format the element to be written to the vault
		/// </summary>
		/// <param name="siteElement"></param>
		/// <param name="site"></param>
		/// <returns></returns>
		private static XElement FormatElement(XElement siteElement, Site site)
		{
			//Create a new XML element and add each Site value to it.
			siteElement.Add(new XAttribute("GUID", site.credGUID.ToString()));
			siteElement.Add(new XElement("Name", site.Name));
			siteElement.Add(new XElement("Domain", site.Domain));
			siteElement.Add(new XElement("Username", site.Username));
			siteElement.Add(new XElement("Password", site.Password));
			siteElement.Add(new XElement("Category", site.Category));
			siteElement.Add(new XElement("Notes", site.Notes));
			siteElement.Add(new XElement("Favourite", site.Favourite));

			return siteElement;
		}

		/// <summary>
		/// Add a site to the XML vault
		/// </summary>
		/// <param name="site"></param>
		/// <param name="key"></param>
		public void AddSite(Site site, Aes key)
		{
			xmlDoc = new XDocument();
			//xmlDoc = XDocument.Load(xmlPath);

			//Read in and decrypt the vault
			xmlDoc = DecryptVault();

			XElement siteElement = new XElement("Site");
			siteElement.Add(new XAttribute("GUID", site.credGUID.ToString()));
			siteElement.Add(new XElement("Name", site.Name));
			siteElement.Add(new XElement("Domain", site.Domain));
			siteElement.Add(new XElement("Username", site.Username));
			//Encrypt the password using the key parameter before saving to the XML file
			string encryptedPassword =
				SecurityDataService.EncryptBytesAES(Encoding.UTF8.GetBytes(site.Password), key.Key);
			siteElement.Add(new XElement("Password", encryptedPassword));
			siteElement.Add(new XElement("Category", site.Category));
			siteElement.Add(new XElement("Notes", site.Notes));
			siteElement.Add(new XElement("Favourite", site.Favourite));
			xmlDoc.Descendants().SingleOrDefault(x => x.Name.LocalName == "Sites").Add(siteElement);

			
			EncryptVault(xmlDoc);
			//xmlDoc.Save(xmlPath);
			LoadData();
		}


		/// <summary>
		/// Function deletes the site object from the vault and returns a bool to indicate whether it was successful
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		public bool DeleteSite(Site site)
		{
			xmlDoc = new XDocument();
			//xmlDoc = XDocument.Load(xmlPath);

			//Read in and decrypt the vault
			xmlDoc = DecryptVault();
			bool isDeleted = false;

			XElement Site = xmlDoc.Root.Element("Sites");
			foreach (var element in Site.Elements())
			{
				if (element.Attribute("GUID").Value == site.credGUID.ToString())
				{
					element.Remove();
					isDeleted = true;
					break;
				}
			}
			//xmlDoc.Save(xmlPath);
			EncryptVault(xmlDoc);
			LoadData();
			return isDeleted;
		}

		/// <summary>
		/// Function returns a list of sites
		/// </summary>
		/// <returns></returns>
		public List<Site> ReturnSiteData()
		{
			return _siteData;
		}

		/// <summary>
		/// Loads all of the Sites stored in the vault
		/// </summary>
		/// <returns></returns>
		public void LoadSites()
		{
			xmlDoc = new XDocument();
			//xmlDoc = XDocument.Load(xmlPath);

			//Read in and decrypt the vault
			xmlDoc = DecryptVault();

			//Clear the site data read yfor the new values
			_siteData = new List<Site>();

			XElement Sites = xmlDoc.Root.Element("Sites");

			//Loop through each Site stored in the vault and add to the siteData
			foreach (XElement node in Sites.Elements())
			{
				_siteData.Add(new Site(Guid.Parse(node.Attribute("GUID").Value),
					node.Element("Domain").Value,
					node.Element("Name").Value,
					node.Element("Username").Value,
					node.Element("Password").Value,
					node.Element("Category").Value,
					node.Element("Notes").Value,
					Convert.ToBoolean(node.Element("Favourite").Value)));
			}
		}


		/// <summary>
		/// Adds a category to the vault
		/// </summary>
		/// <param name="navItem"></param>
		public void AddCategory(NavigationItemDatabase navItem)
		{
			xmlDoc = new XDocument();
			//xmlDoc = XDocument.Load(xmlPath);

			//Read in and decrypt the vault
			xmlDoc = DecryptVault();

			//Create a new category using the navItem parameter properties and save to the vault
			XElement navElement = new XElement("Category");
			navElement.Add(new XAttribute("Name", navItem.Name));
			navElement.Add(new XElement("Symbol", navItem.Symbol));

			xmlDoc.Descendants().SingleOrDefault(x => x.Name.LocalName == "Categories").Add(navElement);

			//xmlDoc.Save(xmlPath);

			EncryptVault(xmlDoc);
			LoadData();
			CategoriesChanged(_categoryData);

		}

		/// <summary>
		/// Deletes a category from the system and returns a bool to indicate if it was successful
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool DeleteCategory(NavigationItemDatabase item)
		{
			xmlDoc = new XDocument();
			//xmlDoc = XDocument.Load(xmlPath);

			//Read in and decrypt the vault
			xmlDoc = DecryptVault();

			bool isDeleted = false;

			XElement Category = xmlDoc.Root.Element("Categories");
			foreach (var element in Category.Elements())
			{
				if (element.Attribute("Name").Value == item.Name)
				{
					element.Remove();
					isDeleted = true;
					break;
				}
			}
			//xmlDoc.Save(xmlPath);

			EncryptVault(xmlDoc);

			LoadData();
			return isDeleted;
		}

		/// <summary>
		/// Loads all of the Sites stored in the vault
		/// </summary>
		/// <returns></returns>
		public void LoadCategories()
		{
			xmlDoc = new XDocument();
			//xmlDoc = XDocument.Load("Secret.vault");
			
			//Read in and decrypt the vault
			xmlDoc = DecryptVault();

			//Clear the category data before loading the items from the vault
			_categoryData = new ObservableCollection<NavigationItem>();

			XElement Categories = xmlDoc.Root.Element("Categories");

			if (Categories != null)
			{
				foreach (XElement node in Categories.Elements())
				{
					NavigationItem navItem = new NavigationItem
					{
						Symbol = node.Element("Symbol").Value switch
						{
							"Home" => Symbol.Home,
							"Setting" => Symbol.Setting,
							"Favourite" => Symbol.Favorite,
							_ => Symbol.Tag
						},
						Name = node.Attribute("Name").Value
					};

					//Convert the saved Symbol text into a Symbol object

					_categoryData.Add(navItem);
				}
			}
			
			//Add the category data to the Navigation items used for the root navigation menu
			_navData = CreateNavDataFormatted();
		}

		/// <summary>
		/// Return the formatted navigation data for the root navigation menu
		/// </summary>
		/// <returns></returns>
		private ObservableCollection<NavigationItem> CreateNavDataFormatted()
		{
			var tempCustomNavData = _categoryData.ToList();

			ObservableCollection<NavigationItem> FinalNavData = new ObservableCollection<NavigationItem>
			{
				new NavigationItem(Symbol.Home, "All Items"), new NavigationItem(Symbol.Favorite, "Favourites")
			};

			tempCustomNavData.ForEach(FinalNavData.Add);
			return FinalNavData;
		}

		/// <summary>
		/// Return the category data
		/// </summary>
		/// <returns></returns>
		public List<string> ReturnCategoryData()
		{
			return new List<string>(_categoryData.ToList().Select(x => x.Name));
		}

		/// <summary>
		/// Return the navigation item data
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<NavigationItem> ReturnNavData()
		{
			return _navData;
		}

		/// <summary>
		/// Clears a deleted category from the Sites which contain it.
		/// </summary>
		/// <param name="navItem"></param>
		public void ClearSitesDeletedCategory(NavigationItemDatabase navItem)
		{
			foreach (Site site in _siteData)
			{
				if (site.Category == navItem.Name)
				{
					site.Category = "";
					SaveSite(site);
				}
			}
		}

		/// <summary>
		/// Method returns a boolean value which indicates whether a user has entered the correct master password
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public bool AuthenticateUser(string userEmail, string password)
		{
			//Open the Vault XML file
			xmlDoc = new XDocument();
			//xmlDoc = XDocument.Load("Secret.vault");

			//Read in and decrypt the vault
			xmlDoc = DecryptVault(password);

			//Check if vault was decrypted successfully
			if (xmlDoc.Root == null)
			{
				return false;
			}

			var email = xmlDoc.Descendants().SingleOrDefault(x => x.Name.LocalName == "Email").Value;

			if (userEmail != email)
			{
				return false;
			}

			//Copy the salt value from the vault
			var salt = xmlDoc.Descendants().SingleOrDefault(x => x.Name.LocalName == "Salt").Value;

			//Hash the plain text password entered by the user
			byte[] hash = SecurityDataService.HashAndSaltPassword(password, Convert.FromBase64String(salt), 256 / 8);

			//Copy the encrypted master key from the vault header
			var eMasterKey = xmlDoc.Descendants().SingleOrDefault(x => x.Name.LocalName == "Key").Value;

			byte[] masterKey = SecurityDataService.DecryptStringAES(eMasterKey, hash);

			//if (!SecurityDataService.EncryptionSanityCheck(hash))
			//{
			//	MessageBox.Show("Failed sanity check");
			//}

			if (masterKey == null)
			{
				return false;
			}

			//Store the values to be used later
			_eMasterKey = eMasterKey;
			SaltVault = Convert.FromBase64String(salt);
			HashVault = hash;
			return true;
		}

		/// <summary>
		/// Function encrypts the local vault using the supplied password
		/// </summary>
		/// <param name="xml"></param>
		public void EncryptVault(XDocument xml)
		{
			//Convert XML into a string
			StringWriter sw = new StringWriter();
			XmlTextWriter xtw = new XmlTextWriter(sw);
			xml.WriteTo(xtw);
			string str = sw.ToString();

			File.WriteAllText("Secret.vault", Convert.ToBase64String(SaltVault) + " BREAK " + SecurityDataService.EncryptBytesAES(Encoding.UTF8.GetBytes(str), HashVault));
		}
		/// <summary>
		/// Function decrypts the local vault using the supplied password
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public XDocument DecryptVault(string password = null)
		{
			var xmlString = File.ReadAllText("Secret.vault");

			var result = xmlString.Split(" BREAK ");

			 //xmlString.Substring(8, xmlString.Length - 8);
			 SaltVault = Convert.FromBase64String(result[0]);
			 var cipherString = result[1];

			//If function parameter is null then saved hash is used
			if (password != null)
			{
				HashVault = SecurityDataService.HashAndSaltPassword(password, SaltVault, 256 / 8);
			}

			var xmlByteString = SecurityDataService.DecryptStringAES(cipherString, HashVault);

			//Password entered is wrong
			if (xmlByteString == null)
			{
				return new XDocument();
			}

			var xmlDocument = Encoding.UTF8.GetString(xmlByteString);
			return XDocument.Parse(xmlDocument);
		}

		/// <summary>
		/// Returns the master encryption key used for decrypting the credential passwords
		/// </summary>
		/// <returns></returns>
		public byte[] ReturnMasterKey()
		{
			return SecurityDataService.DecryptStringAES(_eMasterKey, HashVault);
		}

		public void ChangeMasterPassword(string newPassword)
		{
			//Generate hash and salt of new users password
			byte[] Salt = SecurityDataService.GenerateRandomBytes(64 / 8);
			byte[] Hash = SecurityDataService.HashAndSaltPassword(newPassword, Salt, 256 / 8);

			//Decrypt the old master encryption key using the old password hash
			var decryptedMasterEncryptionKey = SecurityDataService.DecryptStringAES(_eMasterKey, HashVault);
			//Encrypt the new master encryption key using the new password hash
			var newMasterEncryptionKey = SecurityDataService.EncryptBytesAES(decryptedMasterEncryptionKey, Hash);

			xmlDoc = DecryptVault();

			XElement salt = xmlDoc.Root.Element("Salt");
			XElement newSalt = new XElement("Salt", Convert.ToBase64String(Salt));
			salt.ReplaceWith(newSalt);

			XElement Key = xmlDoc.Root.Element("Key");
			XElement newKey = new XElement("Key", newMasterEncryptionKey);
			Key.ReplaceWith(newKey);


			_eMasterKey = newMasterEncryptionKey;
			HashVault = Hash;
			SaltVault = Salt;

			EncryptVault(xmlDoc);
		}

	}
}