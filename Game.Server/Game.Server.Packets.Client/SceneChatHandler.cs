using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System.Configuration;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(19, "用户场景聊天")]
	public class SceneChatHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ClientID = client.Player.PlayerCharacter.ID;
			byte b = packet.ReadByte();
			bool flag = packet.ReadBoolean();
			packet.ReadString();
			string text = packet.ReadString();
			GSPacketIn gSPacketIn = new GSPacketIn(19, client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteByte(b);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
			gSPacketIn.WriteString(text);
			if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.RoomType == eRoomType.Match && client.Player.CurrentRoom.Game != null)
			{
				client.Player.CurrentRoom.BattleServer.Server.SendChatMessage(text, client.Player, flag);
				return 1;
			}
			if (b == 3)
			{
				if (client.Player.PlayerCharacter.ConsortiaID == 0)
				{
					return 0;
				}
				if (client.Player.PlayerCharacter.IsBanChat)
				{
					client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat", new object[0]));
					return 1;
				}
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.ConsortiaID);
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer gamePlayer = array[i];
					if (gamePlayer.PlayerCharacter.ConsortiaID == client.Player.PlayerCharacter.ConsortiaID && !gamePlayer.IsBlackFriend(client.Player.PlayerCharacter.ID))
					{
						gamePlayer.Out.SendTCP(gSPacketIn);
					}
				}
			}
			else
			{
				if (b == 9)
				{
					if (client.Player.CurrentMarryRoom == null)
					{
						return 1;
					}
					client.Player.CurrentMarryRoom.SendToAllForScene(gSPacketIn, client.Player.MarryMap);
				}
				else
				{
					if (client.Player.CurrentRoom != null)
					{
						if (flag)
						{
							client.Player.CurrentRoom.SendToTeam(gSPacketIn, client.Player.CurrentRoomTeam, client.Player);
						}
						else
						{
							client.Player.CurrentRoom.SendToAll(gSPacketIn);
						}
					}
					else
					{
						if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(1.0), DateTime.Now) > 0 && b == 5)
						{
							return 1;
						}
						if (flag)
						{
							return 1;
						}
						if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(30.0), DateTime.Now) > 0)
						{
							client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("SceneChatHandler.Fast", new object[0]));
							return 1;
						}
                        string NicksGM = ConfigurationManager.AppSettings["GmsPlayerId"];
                        if (bool.Parse(ConfigurationManager.AppSettings["Cmdon"]))
                        {
                            int playerID = client.Player.PlayerId;
                            string PID = client.Player.PlayerId.ToString();
                            if (NicksGM == PID)
                            {
                                switch (text)
                                {
                                    case "mycps":
                                        client.Player.SendMessage("Foi enviado para si![test]");
                                        break;
                                }
                            }
                        }
						client.Player.LastChatTime = DateTime.Now;
						GamePlayer[] allPlayers2 = WorldMgr.GetAllPlayers();
						GamePlayer[] array2 = allPlayers2;
						for (int j = 0; j < array2.Length; j++)
						{
							GamePlayer gamePlayer2 = array2[j];
							if (gamePlayer2.CurrentRoom == null && gamePlayer2.CurrentMarryRoom == null && !gamePlayer2.IsBlackFriend(client.Player.PlayerCharacter.ID))
							{
								gamePlayer2.Out.SendTCP(gSPacketIn);
							}
						}

					}
				}
			}
			return 1;
		}
	}
}
