using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
namespace Game.Server.Rooms
{
	public class ExitRoomAction : IAction
	{
		private BaseRoom m_room;
		private GamePlayer m_player;
		public ExitRoomAction(BaseRoom room, GamePlayer player)
		{
			this.m_room = room;
			this.m_player = player;
		}
		public void Execute()
		{
			this.m_room.RemovePlayerUnsafe(this.m_player);
			List<BaseRoom> list = new List<BaseRoom>();
			if (this.m_room.PlayerCount > 0)
			{
				list.Add(this.m_room);
				GSPacketIn packet = this.m_player.Out.SendUpdateRoomList(list);
				RoomMgr.WaitingRoom.SendToALL(packet, this.m_player);
			}
			if (this.m_room.PlayerCount == 0)
			{
				this.m_room.Stop();
			}
		}
	}
}
