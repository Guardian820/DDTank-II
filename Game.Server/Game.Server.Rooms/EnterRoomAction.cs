using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using System;
namespace Game.Server.Rooms
{
	internal class EnterRoomAction : IAction
	{
		private GamePlayer m_player;
		private int m_roomId;
		private string m_pwd;
		private int m_type;
		public EnterRoomAction(GamePlayer player, int roomId, string pwd, int type)
		{
			this.m_player = player;
			this.m_roomId = roomId;
			this.m_pwd = pwd;
			this.m_type = type;
		}
		public void Execute()
		{
			if (!this.m_player.IsActive)
			{
				return;
			}
			if (this.m_player.CurrentRoom != null)
			{
				this.m_player.CurrentRoom.RemovePlayerUnsafe(this.m_player);
			}
			BaseRoom[] rooms = RoomMgr.Rooms;
			BaseRoom baseRoom;
			if (this.m_roomId == -1)
			{
				baseRoom = this.FindRandomRoom(rooms);
				if (baseRoom == null)
				{
					this.m_player.Out.SendMessage(eMessageType.ERROR, "Không có phòng game nào!");
					this.m_player.Out.SendRoomLoginResult(false);
					return;
				}
			}
			else
			{
				baseRoom = rooms[this.m_roomId - 1];
			}
			if (!baseRoom.IsUsing)
			{
				this.m_player.Out.SendMessage(eMessageType.Normal, "Phòng đang sử dụng!");
				return;
			}
			if (baseRoom.PlayerCount == baseRoom.PlacesCount)
			{
				this.m_player.Out.SendMessage(eMessageType.ERROR, "Phòng đã đấy！");
				return;
			}
			if (!baseRoom.NeedPassword || baseRoom.Password == this.m_pwd)
			{
				if (baseRoom.Game == null || baseRoom.Game.CanAddPlayer())
				{
					if (baseRoom.LevelLimits > (int)baseRoom.GetLevelLimit(this.m_player))
					{
						this.m_player.Out.SendMessage(eMessageType.ERROR, "Level chưa đủ！");
						return;
					}
					RoomMgr.WaitingRoom.RemovePlayer(this.m_player);
					this.m_player.Out.SendRoomLoginResult(true);
					this.m_player.Out.SendRoomCreate(baseRoom);
					if (baseRoom.AddPlayerUnsafe(this.m_player) && baseRoom.Game != null)
					{
						baseRoom.Game.AddPlayer(this.m_player);
					}
					RoomMgr.WaitingRoom.SendUpdateRoom(baseRoom);
					this.m_player.Out.SendRoomChange(baseRoom);
					return;
				}
			}
			else
			{
				this.m_player.Out.SendMessage(eMessageType.ERROR, "Password không đúng!");
				this.m_player.Out.SendRoomLoginResult(false);
			}
		}
		private BaseRoom FindRandomRoom(BaseRoom[] rooms)
		{
			for (int i = 0; i < rooms.Length; i++)
			{
				if (rooms[i].PlayerCount > 0 && rooms[i].CanAddPlayer() && !rooms[i].NeedPassword && !rooms[i].IsPlaying)
				{
					if (10 != this.m_type)
					{
						if (rooms[i].RoomType == (eRoomType)this.m_type)
						{
							return rooms[i];
						}
					}
					else
					{
						if (rooms[i].RoomType == (eRoomType)this.m_type && rooms[i].LevelLimits < (int)rooms[i].GetLevelLimit(this.m_player))
						{
							return rooms[i];
						}
					}
				}
			}
			return null;
		}
	}
}
