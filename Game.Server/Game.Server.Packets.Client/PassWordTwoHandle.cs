using Bussiness;
using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(25, "二级密码")]
	public class PassWordTwoHandle : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			string translateId = "";
			bool val = false;
			int val2 = 0;
			bool val3 = false;
			int num = 0;
			string text = packet.ReadString();
			string passwordTwo = packet.ReadString();
			int num2 = packet.ReadInt();
			string text2 = packet.ReadString();
			string text3 = packet.ReadString();
			string text4 = packet.ReadString();
			string text5 = packet.ReadString();
			switch (num2)
			{
			case 1:
				val2 = 1;
				if (string.IsNullOrEmpty(client.Player.PlayerCharacter.PasswordTwo))
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						if (text != "" && playerBussiness.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, text))
						{
							client.Player.PlayerCharacter.PasswordTwo = text;
							client.Player.PlayerCharacter.IsLocked = false;
							translateId = "SetPassword.success";
						}
						if (text2 != "" && text3 != "" && text4 != "" && text5 != "")
						{
							if (playerBussiness.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, text2, text3, text4, text5, 5))
							{
								val = true;
								val3 = false;
								translateId = "UpdatePasswordInfo.Success";
							}
							else
							{
								val = false;
							}
						}
						else
						{
							val = true;
							val3 = true;
						}
						goto IL_48E;
					}
				}
				translateId = "SetPassword.Fail";
				val = false;
				val3 = false;
				goto IL_48E;

			case 2:
				val2 = 2;
				if (text == client.Player.PlayerCharacter.PasswordTwo)
				{
					client.Player.PlayerCharacter.IsLocked = false;
					translateId = "BagUnlock.success";
					val = true;
					goto IL_48E;
				}
				translateId = "PasswordTwo.error";
				val = false;
				val3 = false;
				goto IL_48E;

			case 3:
				val2 = 3;
				using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
				{
					playerBussiness2.GetPasswordInfo(client.Player.PlayerCharacter.ID, ref text2, ref text3, ref text4, ref text5, ref num);
					num--;
					playerBussiness2.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, text2, text3, text4, text5, num);
					if (text == client.Player.PlayerCharacter.PasswordTwo)
					{
						if (playerBussiness2.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, passwordTwo))
						{
							client.Player.PlayerCharacter.IsLocked = false;
							client.Player.PlayerCharacter.PasswordTwo = passwordTwo;
							translateId = "UpdatePasswordTwo.Success";
							val = true;
							val3 = false;
						}
						else
						{
							translateId = "UpdatePasswordTwo.Fail";
							val = false;
							val3 = false;
						}
					}
					else
					{
						translateId = "PasswordTwo.error";
						val = false;
						val3 = false;
					}
					goto IL_48E;
				}
				break;

			case 4:
				break;

			case 5:
				goto IL_3FD;

			default:
				goto IL_48E;
			}
			val2 = 4;
			string a = "";
			string passwordTwo2 = "";
			string a2 = "";
			using (PlayerBussiness playerBussiness3 = new PlayerBussiness())
			{
				playerBussiness3.GetPasswordInfo(client.Player.PlayerCharacter.ID, ref text2, ref a, ref text4, ref a2, ref num);
				num--;
				playerBussiness3.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, text2, text3, text4, text5, num);
				if (a == text3 && a2 == text5 && a != "" && a2 != "")
				{
					if (playerBussiness3.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, passwordTwo2))
					{
						client.Player.PlayerCharacter.PasswordTwo = passwordTwo2;
						client.Player.PlayerCharacter.IsLocked = false;
						translateId = "DeletePassword.success";
						val = true;
						val3 = false;
					}
					else
					{
						translateId = "DeletePassword.Fail";
						val = false;
					}
				}
				else
				{
					if (text == client.Player.PlayerCharacter.PasswordTwo)
					{
						if (playerBussiness3.UpdatePasswordTwo(client.Player.PlayerCharacter.ID, passwordTwo2))
						{
							client.Player.PlayerCharacter.PasswordTwo = passwordTwo2;
							client.Player.PlayerCharacter.IsLocked = false;
							translateId = "DeletePassword.success";
							val = true;
							val3 = false;
						}
					}
					else
					{
						translateId = "DeletePassword.Fail";
						val = false;
					}
				}
				goto IL_48E;
			}
			IL_3FD:
			val2 = 5;
			if (client.Player.PlayerCharacter.PasswordTwo != null && text2 != "" && text3 != "" && text4 != "" && text5 != "")
			{
				using (PlayerBussiness playerBussiness4 = new PlayerBussiness())
				{
					if (playerBussiness4.UpdatePasswordInfo(client.Player.PlayerCharacter.ID, text2, text3, text4, text5, 5))
					{
						val = true;
						val3 = false;
						translateId = "UpdatePasswordInfo.Success";
					}
					else
					{
						val = false;
					}
				}
			}
			IL_48E:
			GSPacketIn gSPacketIn = new GSPacketIn(25, client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteInt(val2);
			gSPacketIn.WriteBoolean(val);
			gSPacketIn.WriteBoolean(val3);
			gSPacketIn.WriteString(LanguageMgr.GetTranslation(translateId, new object[0]));
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteString(text2);
			gSPacketIn.WriteString(text4);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
