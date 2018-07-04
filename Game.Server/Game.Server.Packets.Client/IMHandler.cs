using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(160, "添加好友")]
	public class IMHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte b = packet.ReadByte();
			byte b2 = b;
			if (b2 <= 51)
			{
				if (b2 != 45)
				{
					if (b2 == 51)
					{
						int num = packet.ReadInt();
						string msg = packet.ReadString();
						packet.ReadBoolean();
						GamePlayer playerById = WorldMgr.GetPlayerById(num);
						if (playerById != null)
						{
							client.Player.Out.sendOneOnOneTalk(num, false, client.Player.PlayerCharacter.NickName, msg, client.Player.PlayerCharacter.ID);
							playerById.Out.sendOneOnOneTalk(client.Player.PlayerCharacter.ID, false, client.Player.PlayerCharacter.NickName, msg, num);
						}
						else
						{
							client.Player.Out.SendMessage(eMessageType.Normal, "Người chơi không online!");
						}
					}
				}
			}
			else
			{
				switch (b2)
				{
				case 160:
					{
						string text = packet.ReadString();
						int num2 = packet.ReadInt();
						if (num2 < 0 || num2 > 1)
						{
							return 1;
						}
						using (PlayerBussiness playerBussiness = new PlayerBussiness())
						{
							GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(text);
							PlayerInfo playerInfo;
							if (clientByPlayerNickName != null)
							{
								playerInfo = clientByPlayerNickName.PlayerCharacter;
							}
							else
							{
								playerInfo = playerBussiness.GetUserSingleByNickName(text);
							}
							if (!string.IsNullOrEmpty(text) && playerInfo != null)
							{
								if (!client.Player.Friends.ContainsKey(playerInfo.ID) || client.Player.Friends[playerInfo.ID] != num2)
								{
									if (playerBussiness.AddFriends(new FriendInfo
									{
										FriendID = playerInfo.ID,
										IsExist = true,
										Remark = "",
										UserID = client.Player.PlayerCharacter.ID,
										Relation = num2
									}))
									{
										client.Player.FriendsAdd(playerInfo.ID, num2);
										if (num2 != 1 && playerInfo.State != 0)
										{
											GSPacketIn gSPacketIn = new GSPacketIn(160, client.Player.PlayerCharacter.ID);
											gSPacketIn.WriteByte(166);
											gSPacketIn.WriteInt(playerInfo.ID);
											gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
											gSPacketIn.WriteBoolean(false);
											if (clientByPlayerNickName != null)
											{
												clientByPlayerNickName.Out.SendTCP(gSPacketIn);
											}
											else
											{
												GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
											}
										}
										client.Out.SendAddFriend(playerInfo, num2, true);
									}
								}
								else
								{
									client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FriendAddHandler.Falied", new object[0]));
								}
							}
							else
							{
								client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("FriendAddHandler.Success", new object[0]));
							}
							return 1;
						}
						break;
					}

				case 161:
					break;

				case 162:
					goto IL_28F;

				case 163:
					return 1;

				case 164:
					Console.WriteLine("//SAME_CITY_FRIEND ");
					return 1;

				case 165:
					{
						int num3 = packet.ReadInt();
						GSPacketIn gSPacketIn2 = new GSPacketIn(160, client.Player.PlayerCharacter.ID);
						gSPacketIn2.WriteByte(165);
						gSPacketIn2.WriteInt(num3);
						gSPacketIn2.WriteInt((int)client.Player.PlayerCharacter.typeVIP);
						gSPacketIn2.WriteInt(client.Player.PlayerCharacter.VIPLevel);
						gSPacketIn2.WriteBoolean(false);
						GameServer.Instance.LoginServer.SendPacket(gSPacketIn2);
						WorldMgr.ChangePlayerState(client.Player.PlayerCharacter.ID, num3, client.Player.PlayerCharacter.ConsortiaID);
						return 1;
					}

				case 166:
					Console.WriteLine("//FRIEND_RESPONSE ");
					return 1;

				default:
					if (b2 != 208)
					{
						return 1;
					}
					Console.WriteLine("//ADD_CUSTOM_FRIENDS ");
					return 1;
				}
				int num4 = packet.ReadInt();
				using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
				{
					if (playerBussiness2.DeleteFriends(client.Player.PlayerCharacter.ID, num4))
					{
						client.Player.FriendsRemove(num4);
						client.Out.SendFriendRemove(num4);
					}
					return 1;
				}
				IL_28F:
				Console.WriteLine("//FRIEND_UPDATE ");
			}
			return 1;
		}
	}
}
