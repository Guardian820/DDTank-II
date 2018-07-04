using Game.Logic;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Rooms
{
	public class CreateRoomAction : IAction
	{
		private GamePlayer m_player;
		private string m_name;
		private string m_password;
		private eRoomType m_roomType;
		private byte m_timeType;
		public CreateRoomAction(GamePlayer player, string name, string password, eRoomType roomType, byte timeType)
		{
			this.m_player = player;
			this.m_name = name;
			this.m_password = password;
			this.m_roomType = roomType;
			this.m_timeType = timeType;
		}
		public void Execute()
		{
			if (this.m_player.CurrentRoom != null)
			{
				this.m_player.CurrentRoom.RemovePlayerUnsafe(this.m_player);
			}
			if (!this.m_player.IsActive)
			{
				return;
			}
			BaseRoom[] rooms = RoomMgr.Rooms;
			BaseRoom baseRoom = null;
			for (int i = 0; i < rooms.Length; i++)
			{
				if (!rooms[i].IsUsing)
				{
					baseRoom = rooms[i];
					break;
				}
			}
			if (baseRoom != null)
			{
				RoomMgr.WaitingRoom.RemovePlayer(this.m_player);
				baseRoom.Start();
				if (this.m_roomType == eRoomType.Dungeon)
				{
					baseRoom.HardLevel = eHardLevel.Normal;
					baseRoom.LevelLimits = (int)baseRoom.GetLevelLimit(this.m_player);
				}
				baseRoom.UpdateRoom(this.m_name, this.m_password, this.m_roomType, this.m_timeType, 0);
				this.m_player.Out.SendRoomCreate(baseRoom);
				baseRoom.AddPlayerUnsafe(this.m_player);
				RoomMgr.WaitingRoom.SendUpdateRoom(baseRoom);
			}
		}
	}
}
