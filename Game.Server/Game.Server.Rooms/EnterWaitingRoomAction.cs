using Game.Logic;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
namespace Game.Server.Rooms
{
	public class EnterWaitingRoomAction : IAction
	{
		private GamePlayer m_player;
		public EnterWaitingRoomAction(GamePlayer player)
		{
			this.m_player = player;
		}
		public void Execute()
		{
			if (this.m_player.CurrentRoom != null)
			{
				this.m_player.CurrentRoom.RemovePlayerUnsafe(this.m_player);
			}
			BaseWaitingRoom waitingRoom = RoomMgr.WaitingRoom;
			if (waitingRoom.AddPlayer(this.m_player))
			{
				BaseRoom[] rooms = RoomMgr.Rooms;
				List<BaseRoom> list = new List<BaseRoom>();
				for (int i = 0; i < rooms.Length; i++)
				{
					if (!rooms[i].IsEmpty)
					{
						if (this.m_player.ScreenStyle == 1 && (rooms[i].RoomType == eRoomType.Match || rooms[i].RoomType == eRoomType.Freedom))
						{
							list.Add(rooms[i]);
						}
						if (this.m_player.ScreenStyle == 2 && (rooms[i].RoomType == eRoomType.Dungeon || rooms[i].RoomType == eRoomType.Freshman))
						{
							list.Add(rooms[i]);
						}
					}
				}
				this.m_player.Out.SendUpdateRoomList(list);
				GamePlayer[] playersSafe = waitingRoom.GetPlayersSafe();
				GamePlayer[] array = playersSafe;
				for (int j = 0; j < array.Length; j++)
				{
					GamePlayer gamePlayer = array[j];
					if (gamePlayer != this.m_player)
					{
						this.m_player.Out.SendSceneAddPlayer(gamePlayer);
					}
				}
			}
		}
	}
}
