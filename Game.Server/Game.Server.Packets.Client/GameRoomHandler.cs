using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Rooms;
using System;
using System.Collections.Generic;
using log4net;
using System.Reflection;
namespace Game.Server.Packets.Client
{
	[PacketHandler(94, "游戏创建")]
	public class GameRoomHandler : IPacketHandler
	{
        private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			switch (packet.ReadInt())
			{
			case 0:
				{
					byte roomType = packet.ReadByte();
					byte timeType = packet.ReadByte();
					string name = packet.ReadString();
					string password = packet.ReadString();
					RoomMgr.CreateRoom(client.Player, name, password, (eRoomType)roomType, timeType);
					break;
				}

			case 1:
				{
					packet.ReadBoolean();
					int num = packet.ReadInt();
					int num2 = packet.ReadInt();
					int roomId = -1;
					string pwd = null;
					if (num2 == -1)
					{
						roomId = packet.ReadInt();
						pwd = packet.ReadString();
					}
					if (num == 1)
					{
						num = 0;
					}
					else
					{
						if (num == 2)
						{
							num = 4;
						}
					}
					RoomMgr.EnterRoom(client.Player, roomId, pwd, num);
					break;
				}

			case 2:
				if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host && !client.Player.CurrentRoom.IsPlaying)
				{
					int num3 = packet.ReadInt();
					eRoomType roomType2 = (eRoomType)packet.ReadByte();
					bool isOpenBoss = packet.ReadBoolean();
					string password2 = packet.ReadString();
					string roomname = packet.ReadString();
					byte timeMode = packet.ReadByte();
					byte hardLevel = packet.ReadByte();
					int levelLimits = packet.ReadInt();
					bool isCrosszone = packet.ReadBoolean();
					packet.ReadInt();
                    Console.WriteLine("=====>MapID: " + num3.ToString() + " |roomType: " + roomType2.ToString());
					if (num3 == 0)
					{
						num3 = 401;//ME CUNG
					}
					RoomMgr.UpdateRoomGameType(client.Player.CurrentRoom, roomType2, timeMode, (eHardLevel)hardLevel, levelLimits, num3, password2, roomname, isCrosszone, isOpenBoss);
                    //log.Error("=====>1: " + client.Player.CurrentRoom + " |2: " + roomType2 + " |3: " + timeMode + " |4: " + (eHardLevel)hardLevel + " |5: " + levelLimits + " |6: " + num3 + " |7: " + password2 + " |8: " + roomname + " |9: " + isCrosszone + " |10: " + isOpenBoss);
				}
				break;

			case 3:
				if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host)
				{
					RoomMgr.KickPlayer(client.Player.CurrentRoom, packet.ReadByte());
				}
				break;

			case 5:
				if (client.Player.CurrentRoom != null)
				{
					RoomMgr.ExitRoom(client.Player.CurrentRoom, client.Player);
				}
				break;

			case 6:
				if (client.Player.CurrentRoom == null || client.Player.CurrentRoom.RoomType == eRoomType.Match)
				{
					return 0;
				}
				RoomMgr.SwitchTeam(client.Player);
				break;

			case 7:
				{
					BaseRoom currentRoom = client.Player.CurrentRoom;
					if (currentRoom != null && currentRoom.Host == client.Player)
					{
						if (client.Player.MainWeapon == null)
						{
							client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
							return 0;
						}
						if (currentRoom.RoomType == eRoomType.Dungeon && !client.Player.IsPvePermission(currentRoom.MapId, currentRoom.HardLevel))
						{
							client.Player.SendMessage("Do not PvePermission enter this map!");
							return 0;
						}
						RoomMgr.StartGame(client.Player.CurrentRoom);
					}
					break;
				}

			case 9:
				{
                    packet.ReadInt();
					int num4 = packet.ReadInt();
					int num5 = 1011;
					if (num4 == -2)
					{
						packet.ReadInt();
						num5 = packet.ReadInt();
					}
					BaseRoom[] rooms = RoomMgr.Rooms;
					List<BaseRoom> list = new List<BaseRoom>();
					for (int i = 0; i < rooms.Length; i++)
					{
						if (!rooms[i].IsEmpty)
						{
							switch (num4)
							{
							case 3:
								if (rooms[i].RoomType == eRoomType.Match || rooms[i].RoomType == eRoomType.Freedom)
								{
									list.Add(rooms[i]);
								}
								break;

							case 4:
								if (rooms[i].RoomType == eRoomType.Match)
								{
									list.Add(rooms[i]);
								}
								break;

							case 5:
								if (rooms[i].RoomType == eRoomType.Freedom)
								{
									list.Add(rooms[i]);
								}
								break;

							default:
								if (rooms[i].RoomType == eRoomType.Dungeon)
								{
									switch (num5)
									{
									case 1007:
										if (rooms[i].HardLevel == eHardLevel.Simple)
										{
											list.Add(rooms[i]);
										}
										break;

									case 1008:
										if (rooms[i].HardLevel == eHardLevel.Normal)
										{
											list.Add(rooms[i]);
										}
										break;

									case 1009:
										if (rooms[i].HardLevel == eHardLevel.Hard)
										{
											list.Add(rooms[i]);
										}
										break;

									case 1010:
										if (rooms[i].HardLevel == eHardLevel.Terror)
										{
											list.Add(rooms[i]);
										}
										break;

									default:
										list.Add(rooms[i]);
										break;
									}
								}
								break;
							}
						}
					}
					if (list.Count > 0)
					{
						client.Out.SendUpdateRoomList(list);
					}
					break;
				}

			case 10:
				if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host)
				{
					byte pos = packet.ReadByte();
					int place = packet.ReadInt();
					bool isOpened = packet.ReadBoolean();
					int placeView = packet.ReadInt();
					RoomMgr.UpdateRoomPos(client.Player.CurrentRoom, (int)pos, isOpened, place, placeView);
				}
				break;

			case 11:
				if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.BattleServer != null)
				{
					client.Player.CurrentRoom.BattleServer.RemoveRoom(client.Player.CurrentRoom);
					if (client.Player != client.Player.CurrentRoom.Host)
					{
						client.Player.CurrentRoom.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed", new object[0]));
						RoomMgr.UpdatePlayerState(client.Player, 0);
					}
					else
					{
						RoomMgr.UpdatePlayerState(client.Player, 2);
					}
				}
				break;

			case 12:
				packet.ReadInt();
				if (client.Player.CurrentRoom != null)
				{
					int num6 = packet.ReadInt();
					int num7 = num6;
					if (num7 == 0)
					{
						client.Player.CurrentRoom.GameType = eGameType.Free;
					}
					else
					{
						client.Player.CurrentRoom.GameType = eGameType.Guild;
					}
					GSPacketIn pkg = client.Player.Out.SendRoomType(client.Player, client.Player.CurrentRoom);
					client.Player.CurrentRoom.SendToAll(pkg, client.Player);
				}
				break;

			case 15:
				if (client.Player.MainWeapon == null)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
					return 0;
				}
				if (client.Player.CurrentRoom != null)
				{
					RoomMgr.UpdatePlayerState(client.Player, packet.ReadByte());
				}
				break;
			}
			return 0;
		}
	}
}
