using Bussiness;
using Bussiness.Interface;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(1, "User Login handler")]
	public class UserLoginHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			try
			{
				if (client.Player == null)
				{
					int version = packet.ReadInt();
					packet.ReadInt();
					byte[] array = new byte[8];
					byte[] array2 = packet.ReadBytes();
					try
					{
						array2 = WorldMgr.RsaCryptor.Decrypt(array2, false);
					}
					catch (ExecutionEngineException exception)
					{
						client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError", new object[0]));
						client.Disconnect();
						GameServer.log.Error("ExecutionEngineException", exception);
						int result = 0;
						return result;
					}
					catch (Exception exception2)
					{
						client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError", new object[0]));
						client.Disconnect();
						GameServer.log.Error("RsaCryptor", exception2);
						int result = 0;
						return result;
					}
					string arg_B1_0 = GameServer.Edition;
					for (int i = 0; i < 8; i++)
					{
						array[i] = array2[i + 7];
					}
					client.setKey(array);
					string[] array3 = Encoding.UTF8.GetString(array2, 15, array2.Length - 15).Split(new char[]
					{
						','
					});
					if (array3.Length == 2)
					{
						string text = array3[0];
						string pass = array3[1];
						if (!LoginMgr.ContainsUser(text))
						{
							bool flag = false;
							BaseInterface baseInterface = BaseInterface.CreateInterface();
							PlayerInfo playerInfo = baseInterface.LoginGame(text, pass, ref flag);
							if (playerInfo != null && playerInfo.ID != 0)
							{
								if (playerInfo.ID == -2)
								{
									client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid", new object[0]));
									client.Disconnect();
									int result = 0;
									return result;
								}
								if (!flag)
								{
									client.Player = new GamePlayer(playerInfo.ID, text, client, playerInfo);
									LoginMgr.Add(playerInfo.ID, client);
									client.Server.LoginServer.SendAllowUserLogin(playerInfo.ID);
									client.Version = version;
								}
								else
								{
									client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Register", new object[0]));
									client.Disconnect();
								}
							}
							else
							{
								client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.OverTime", new object[0]));
								client.Disconnect();
							}
						}
						else
						{
							client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LoginError", new object[0]));
							client.Disconnect();
						}
					}
					else
					{
						client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LengthError", new object[0]));
						client.Disconnect();
					}
				}
			}
			catch (Exception exception3)
			{
				client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.ServerError", new object[0]));
				client.Disconnect();
				GameServer.log.Error(LanguageMgr.GetTranslation("UserLoginHandler.ServerError", new object[0]), exception3);
			}
			return 1;
		}
	}
}
