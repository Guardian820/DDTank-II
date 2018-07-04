using Game.Server.GameObjects;
using System;
namespace Game.Server.Rooms
{
	public class UpdatePlayerStateAction : IAction
	{
		private GamePlayer m_player;
		private BaseRoom m_room;
		private byte m_state;
		public UpdatePlayerStateAction(GamePlayer player, BaseRoom room, byte state)
		{
			this.m_player = player;
			this.m_state = state;
			this.m_room = room;
		}
		public void Execute()
		{
			if (this.m_player.CurrentRoom.RoomId == this.m_room.RoomId)
			{
				this.m_room.UpdatePlayerState(this.m_player, this.m_state, true);
			}
		}
	}
}
