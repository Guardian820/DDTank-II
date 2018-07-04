using Game.Logic;
using System;
namespace Game.Server.Rooms
{
	internal class RoomSetupChangeAction : IAction
	{
		private BaseRoom m_room;
		private eRoomType m_roomType;
		private byte m_timeMode;
		private eHardLevel m_hardLevel;
		private int m_mapId;
		private int m_levelLimits;
		private string m_password;
		private string m_roomName;
		private bool m_isCrosszone;
		private bool m_isOpenBoss;
		public RoomSetupChangeAction(BaseRoom room, eRoomType roomType, byte timeMode, eHardLevel hardLevel, int levelLimits, int mapId, string password, string roomname, bool isCrosszone, bool isOpenBoss)
		{
			this.m_room = room;
			this.m_roomType = roomType;
			this.m_timeMode = timeMode;
			this.m_hardLevel = hardLevel;
			this.m_levelLimits = levelLimits;
			this.m_mapId = mapId;
			this.m_password = password;
			this.m_roomName = roomname;
			this.m_isCrosszone = isCrosszone;
			this.m_isOpenBoss = isOpenBoss;
		}
		public void Execute()
		{
			this.m_room.RoomType = this.m_roomType;
			this.m_room.TimeMode = this.m_timeMode;
			this.m_room.HardLevel = this.m_hardLevel;
			this.m_room.LevelLimits = this.m_levelLimits;
			this.m_room.MapId = this.m_mapId;
			this.m_room.Name = this.m_roomName;
			this.m_room.Password = this.m_password;
			this.m_room.isCrosszone = this.m_isCrosszone;
			this.m_room.isOpenBoss = this.m_isOpenBoss;
			this.m_room.UpdateRoomGameType();
			this.m_room.SendRoomSetupChange(this.m_room);
			RoomMgr.WaitingRoom.SendUpdateRoom(this.m_room);
		}
	}
}
