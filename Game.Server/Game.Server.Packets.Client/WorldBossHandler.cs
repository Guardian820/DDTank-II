using Game.Base.Packets;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(102, "场景用户离开")]
	public class WorldBossHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte b = packet.ReadByte();
            Console.WriteLine("WorldBossHandler: " + (LittleGamePackageInType)b);
			GSPacketIn gSPacketIn = new GSPacketIn(102);
			switch (b)
			{
			case 32:
				gSPacketIn.WriteByte(2);
				gSPacketIn.WriteBoolean(true);
				gSPacketIn.WriteBoolean(false);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				client.Out.SendTCP(gSPacketIn);
				return 0;

			case 33:
				{
					BaseWorldBossRoom worldBossRoom = RoomMgr.WorldBossRoom;
					gSPacketIn.WriteByte(4);
					gSPacketIn.WriteInt(client.Player.PlayerId);
					worldBossRoom.SendToALL(gSPacketIn);
					worldBossRoom.RemovePlayer(client.Player);
					client.Player.IsInWorldBossRoom = false;
					break;
				}

			case 34:
				{
					int worldBossX = packet.ReadInt();
					int worldBossY = packet.ReadInt();
					client.Player.WorldBossX = worldBossX;
					client.Player.WorldBossY = worldBossY;
					if (client.Player.CurrentRoom != null)
					{
						client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
					}
					BaseWorldBossRoom worldBossRoom2 = RoomMgr.WorldBossRoom;
					if (client.Player.IsInWorldBossRoom)
					{
						gSPacketIn.WriteByte(4);
						gSPacketIn.WriteInt(client.Player.PlayerId);
						worldBossRoom2.SendToALL(gSPacketIn);
						worldBossRoom2.RemovePlayer(client.Player);
						client.Player.IsInWorldBossRoom = false;
					}
					else
					{
						if (worldBossRoom2.AddPlayer(client.Player))
						{
							worldBossRoom2.UpdateRoom(client.Player);
						}
					}
					break;
				}

			case 35:
				{
					int num = packet.ReadInt();
					int num2 = packet.ReadInt();
					string str = packet.ReadString();
					client.Player.WorldBossX = num;
					client.Player.WorldBossY = num2;
					gSPacketIn.WriteByte(6);
					gSPacketIn.WriteInt(client.Player.PlayerId);
					gSPacketIn.WriteInt(num);
					gSPacketIn.WriteInt(num2);
					gSPacketIn.WriteString(str);
					RoomMgr.WorldBossRoom.SendToALL(gSPacketIn);
					break;
				}

			case 36:
				{
					byte b2 = packet.ReadByte();
					if (b2 != 3 || client.Player.WorldBossState != 3)
					{
						gSPacketIn.WriteByte(7);
						gSPacketIn.WriteInt(client.Player.PlayerId);
						gSPacketIn.WriteByte(b2);
						gSPacketIn.WriteInt(client.Player.WorldBossX);
						gSPacketIn.WriteInt(client.Player.WorldBossY);
						RoomMgr.WorldBossRoom.SendToALL(gSPacketIn);
						if (b2 == 3 && client.Player.CurrentRoom.Game != null)
						{
							client.Player.CurrentRoom.Game.Stop();
							client.Player.CurrentRoom.Game.RemovePlayer(client.Player, true);
						}
					}
					client.Player.WorldBossState = b2;
					break;
				}

			case 37:
				Console.WriteLine("//REQUEST_REVIVE ");
				break;

			case 38:
				Console.WriteLine("//BUFF_BUY ");
				break;

			default:
				Console.WriteLine("//worldBoss_cmd: " + b);
				break;
			}
			return 0;
		}
	}
}
