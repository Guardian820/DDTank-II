using Game.Base.Packets;
using Game.Logic;
using Game.Server.Rooms;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(16, "Player enter scene.")]
	public class UserEnterSceneHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int screenStyle = packet.ReadInt();
			BaseRoom[] rooms = RoomMgr.Rooms;
			List<BaseRoom> list = new List<BaseRoom>();
			client.Player.ScreenStyle = screenStyle;
			switch (screenStyle)
			{
			case 1:
				for (int i = 0; i < rooms.Length; i++)
				{
					if (!rooms[i].IsEmpty && (rooms[i].RoomType == eRoomType.Match || rooms[i].RoomType == eRoomType.Freedom))
					{
						list.Add(rooms[i]);
					}
				}
				break;

			case 2:
				for (int j = 0; j < rooms.Length; j++)
				{
					if (!rooms[j].IsEmpty && rooms[j].RoomType == eRoomType.Dungeon)
					{
						list.Add(rooms[j]);
					}
				}
				break;

			default:
				RoomMgr.EnterWaitingRoom(client.Player);
				break;
			}
			if (list.Count > 0)
			{
				client.Out.SendUpdateRoomList(list);
			}
			return 1;
		}
	}
}
