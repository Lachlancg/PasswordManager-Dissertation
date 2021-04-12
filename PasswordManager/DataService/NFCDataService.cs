using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PasswordManagerClient.Library;

namespace PasswordManagerClient.DataService
{
	/* Begin Reference - Heavily Modified Code
	  Irantha Jayasekara. ‘ACR122u NFC Card reader application development with C#’. Accessed 07 February 2021. https://iranthajayasekara.com/blog/nfc-card-reading-system-with-acr122u-reader.html.
	  Irantha Jayasekara. ‘Read and Write to NFC Card with ACR122u Reader in C#’. Accessed 07 February 2021. https://iranthajayasekara.com/blog/read-and-write-to-nfc-card-with-acr122u-reader.html.

	 */
	public class NFCDataService
	{
		int retCode;
		IntPtr hCard;
		IntPtr hContext;
		int Protocol;
		public bool connActive = false;
		string readername = "ACS ACR122 0";      // change depending on reader
		public byte[] SendBuff = new byte[263];
		public byte[] RecvBuff = new byte[263];
		public int SendLen, RecvLen, nBytesRet, reqType, Aprotocol, dwProtocol, cbPciLength;
		public Card.SCARD_READERSTATE RdrState;
		public Card.SCARD_IO_REQUEST pioSendRequest;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Block"></param>
		/// <param name="keyNumber"></param>
		/// <returns></returns>
		public string verifyCard(String Block, byte keyNumber)
		{
			string value = "";
			if (connectCard())
			{
				value = readBlock(Block, keyNumber);
			}

			value = value.Split(new char[] { '\0' }, 2, StringSplitOptions.None)[0].ToString();
			Close();
			return value;
		}

		/// <summary>
		/// Read data from card specified block 
		/// </summary>
		/// <param name="Block"></param>
		/// <param name="keyNumber"></param>
		/// <returns></returns>
		public string readBlock(String Block, byte keyNumber)
		{
			string tmpStr = "";
			int indx;

			if (authenticateBlock(Block, keyNumber))
			{
				ClearBuffers();
				SendBuff[0] = 0xFF; // CLA 
				SendBuff[1] = 0xB0;// INS
				SendBuff[2] = 0x00;// P1
				SendBuff[3] = (byte)int.Parse(Block);// P2 : Block No.
				SendBuff[4] = (byte)int.Parse("16");// Le

				SendLen = 5;
				RecvLen = SendBuff[4] + 2;

				retCode = SendAPDUandDisplay(2);

				if (retCode == -200)
				{
					return "outofrangeexception";
				}

				if (retCode == -202)
				{
					return "BytesNotAcceptable";
				}

				if (retCode != Card.SCARD_S_SUCCESS)
				{
					return "FailRead";
				}

				//Display data in text format
				for (indx = 0; indx <= RecvLen - 1; indx++)
				{
					tmpStr = tmpStr + Convert.ToChar(RecvBuff[indx]);
				}

				//tmpStr = System.Text.Encoding.UTF8.GetString(RecvBuff).TrimEnd('\0');

				return (tmpStr);
			}
			else
			{
				return "FailAuthentication";
			}
		}

		/// <summary>
		/// Write data to the card at the specified block
		/// </summary>
		/// <param name="Text"></param>
		/// <param name="Block"></param>
		/// <returns></returns>
		public bool submitText(string Text, string Block)
		{

			//String tmpStr = Text;
			int indx;
			if (authenticateBlock(Block, 0))
			{
				ClearBuffers();
				SendBuff[0] = 0xFF;                             // CLA
				SendBuff[1] = 0xD6;                             // INS
				SendBuff[2] = 0x00;                             // P1
				SendBuff[3] = (byte)int.Parse(Block);           // P2 : Starting Block No.
				SendBuff[4] = (byte)int.Parse("16");            // P3 : Data length
																//Buffer.BlockCopy(Text, 0, SendBuff, 5, Text.Length); //Write new key to start of buffer


				for (indx = 0; indx <= (Text).Length - 1; indx++)
				{
					SendBuff[indx + 5] = (byte)Text[indx];
				}

				SendLen = SendBuff[4] + 5;
				RecvLen = 0x02;

				retCode = SendAPDUandDisplay(2);

				if (retCode != Card.SCARD_S_SUCCESS)
				{
					//MessageBox.Show("fail write");
					return false;
				}
				else
				{
					//MessageBox.Show("write success");
					return true;
				}
			}
			else
			{
				//MessageBox.Show("FailAuthentication");
				return false;
			}
		}

		/// <summary>
		/// Writes a new key to the card
		/// </summary>
		/// <param name="newKey"></param>
		public void WriteKey(byte[] newKey)
		{
			ClearBuffers();
			SendBuff[0] = 0xFF;                             // CLA
			SendBuff[1] = 0xD6;                             // INS
			SendBuff[2] = 0x00;                             // P1
			SendBuff[3] = (byte)int.Parse("7");           // P2 : Starting Block No.
			SendBuff[4] = (byte)int.Parse("16");            // P3 : Data length
			Buffer.BlockCopy(newKey, 0, SendBuff, 5, newKey.Length); //Write new key to start of buffer
			SendBuff[11] = 0xFF;                             // C1
			SendBuff[12] = 0x07;                             // C2
			SendBuff[13] = 0x80;                             // C3
			SendBuff[14] = 0x00;                             // C4
			Buffer.BlockCopy(newKey, 0, SendBuff, 15, newKey.Length); //Write new key to start of buffer

			SendLen = SendBuff[4] + 5;
			RecvLen = 0x02;

			retCode = SendAPDUandDisplay(2);

		}

		/// <summary>
		/// Load keys to RFID reader memory.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="keyNumber"></param>
		public void LoadKey(byte[] key, byte keyNumber)
		{
			ClearBuffers();

			//Create buffer
			SendBuff[0] = 0xFF;
			SendBuff[1] = 0x82;
			SendBuff[2] = 0x00;
			SendBuff[3] = keyNumber;
			SendBuff[4] = 0x06;
			//Copy key to buffer 
			Buffer.BlockCopy(key, 0, SendBuff, 5, key.Length);

			SendLen = 0x0B;
			RecvLen = 0x02;

			//Write key to RFID reader
			retCode = SendAPDUandDisplay(0);
		}

		/// <summary>
		/// Authenticate with the selected block used the key loaded into card reader
		/// </summary>
		/// <param name="block"></param>
		/// <param name="keyNumber"></param>
		/// <returns></returns>
		public bool authenticateBlock(String block, byte keyNumber)
		{
			ClearBuffers();
			
			SendBuff[0] = 0xFF;                         // CLA
			SendBuff[1] = 0x86;                         // INS: for stored key input
			SendBuff[2] = 0x00;                         // P1: same for all source types 
			SendBuff[3] = 0x00;                         // P2 : Memory location;  P2: for stored key input
			SendBuff[4] = 0x05;                         // P3: for stored key input

			SendBuff[5] = 0x01;                         // Byte 1: version number
			SendBuff[6] = 0x00;                         // Byte 2
			SendBuff[7] = (byte)int.Parse(block);       // Byte 3: sectore no. for stored key input
			SendBuff[8] = 0x60;                         // Byte 4 : Key A for stored key input
														//SendBuff[9] = (byte)int.Parse("1");         // Byte 5 : Session key for non-volatile memory
			SendBuff[9] = 0x00;         // Byte 5 : Session key for non-volatile memory
			SendBuff[9] = keyNumber;         // Key number

			SendLen = 0x0A;
			RecvLen = 0x02;


			retCode = SendAPDUandDisplay(0);

			if (retCode != Card.SCARD_S_SUCCESS)
			{
				//MessageBox.Show("FAIL Authentication!");
				return false;
			}

			return true;
		}

		
		/// <summary>
		/// Clear memory buffers
		/// </summary>
		private void ClearBuffers()
		{
			long indx;

			for (indx = 0; indx <= 262; indx++)
			{
				RecvBuff[indx] = 0;
				SendBuff[indx] = 0;
			}
		}

		/// <summary>
		/// Send application protocol data unit commands between reader and card
		/// </summary>
		/// <param name="reqType"></param>
		/// <returns></returns>
		private int SendAPDUandDisplay(int reqType)
		{
			int indx;
			string tmpStr = "";

			pioSendRequest.dwProtocol = Protocol;
			pioSendRequest.cbPciLength = 8;

			//Display Apdu In
			for (indx = 0; indx <= SendLen - 1; indx++)
			{
				tmpStr = tmpStr + " " + string.Format("{0:X2}", SendBuff[indx]);
			}

			retCode = Card.SCardTransmit(hCard, ref pioSendRequest, ref SendBuff[0],
								 SendLen, ref pioSendRequest, ref RecvBuff[0], ref RecvLen);

			if (retCode != Card.SCARD_S_SUCCESS)
			{
				return retCode;
			}

			else
			{
				try
				{
					tmpStr = "";
					switch (reqType)
					{
						case 0:
							for (indx = (RecvLen - 2); indx <= (RecvLen - 1); indx++)
							{
								tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
							}

							if ((tmpStr).Trim() != "90 00")
							{
								//MessageBox.Show("Return bytes are not acceptable.");
								return -202;
							}

							break;

						case 1:

							for (indx = (RecvLen - 2); indx <= (RecvLen - 1); indx++)
							{
								tmpStr = tmpStr + string.Format("{0:X2}", RecvBuff[indx]);
							}

							if (tmpStr.Trim() != "90 00")
							{
								tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
							}

							else
							{
								tmpStr = "ATR : ";
								for (indx = 0; indx <= (RecvLen - 3); indx++)
								{
									tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
								}
							}

							break;

						case 2:

							for (indx = 0; indx <= (RecvLen - 1); indx++)
							{
								tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
							}

							break;
					}
				}
				catch (IndexOutOfRangeException)
				{
					return -200;
				}
			}
			return retCode;
		}

		/// <summary>
		/// Clock card reader connection
		/// </summary>
		public void Close()
		{
			if (connActive)
			{
				retCode = Card.SCardDisconnect(hCard, Card.SCARD_UNPOWER_CARD);
			}
			retCode = Card.SCardReleaseContext(hCard);
		}

		
		/// <summary>
		/// Connect card 
		/// </summary>
		/// <returns></returns>
		public bool connectCard()
		{
			connActive = true;
			establishContext();

			retCode = Card.SCardConnect(hContext, readername, Card.SCARD_SHARE_SHARED,
				Card.SCARD_PROTOCOL_T0 | Card.SCARD_PROTOCOL_T1, ref hCard, ref Protocol);

			if (retCode != Card.SCARD_S_SUCCESS)
			{
				//MessageBox.Show("Card not available", "Card not available", MessageBoxButton.OK, MessageBoxImage.Error);
				connActive = false;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Returns the UUID stored in the first block of card
		/// </summary>
		/// <returns></returns>
		public string getcardUID()//only for mifare 1k cards
		{
			string cardUID = "";
			byte[] receivedUID = new byte[256];
			Card.SCARD_IO_REQUEST request = new Card.SCARD_IO_REQUEST();
			request.dwProtocol = Card.SCARD_PROTOCOL_T1;
			request.cbPciLength = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Card.SCARD_IO_REQUEST));
			byte[] sendBytes = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 }; //get UID command      for Mifare cards
			int outBytes = receivedUID.Length;
			int status = Card.SCardTransmit(hCard, ref request, ref sendBytes[0], sendBytes.Length, ref request, ref receivedUID[0], ref outBytes);

			if (status != Card.SCARD_S_SUCCESS)
			{
				cardUID = "Error";
			}
			else
			{
				cardUID = BitConverter.ToString(receivedUID.Take(4).ToArray()).Replace("-", string.Empty).ToLower();
			}

			return cardUID;
		}

		/// <summary>
		/// Select the card reader
		/// </summary>
		public void SelectDevice()
		{
			List<string> availableReaders = this.ListReaders();
			this.RdrState = new Card.SCARD_READERSTATE();
			readername = availableReaders[0].ToString();//selecting first device
			this.RdrState.RdrName = readername;
		}

		/// <summary>
		/// Find all available card readers
		/// </summary>
		/// <returns></returns>
		public List<string> ListReaders()
		{
			int ReaderCount = 0;
			List<string> AvailableReaderList = new List<string>();

			//Make sure a context has been established before 
			//retrieving the list of smartcard readers.
			retCode = Card.SCardListReaders(hContext, null, null, ref ReaderCount);
			if (retCode != Card.SCARD_S_SUCCESS)
			{
				MessageBox.Show(Card.GetScardErrMsg(retCode));
				//connActive = false;
			}

			byte[] ReadersList = new byte[ReaderCount];

			//Get the list of reader present again but this time add sReaderGroup, retData as 2rd & 3rd parameter respectively.
			retCode = Card.SCardListReaders(hContext, null, ReadersList, ref ReaderCount);
			if (retCode != Card.SCARD_S_SUCCESS)
			{
				MessageBox.Show(Card.GetScardErrMsg(retCode));
			}

			string rName = "";
			int indx = 0;
			if (ReaderCount > 0)
			{
				// Convert reader buffer to string
				while (ReadersList[indx] != 0)
				{

					while (ReadersList[indx] != 0)
					{
						rName = rName + (char)ReadersList[indx];
						indx = indx + 1;
					}

					//Add reader name to list
					AvailableReaderList.Add(rName);
					rName = "";
					indx = indx + 1;

				}
			}
			return AvailableReaderList;

		}

		/// <summary>
		/// Connect card reader 
		/// </summary>
		internal void establishContext()
		{
			retCode = Card.SCardEstablishContext(Card.SCARD_SCOPE_SYSTEM, 0, 0, ref hContext);
			if (retCode != Card.SCARD_S_SUCCESS)
			{
				MessageBox.Show("Check your device and please restart again", "Reader not connected", MessageBoxButton.OK, MessageBoxImage.Warning);
				connActive = false;
				return;
			}
		}
	}
	/* End Reference */

}