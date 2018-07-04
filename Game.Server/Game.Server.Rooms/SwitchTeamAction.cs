using Game.Server.GameObjects;
using System;
namespace Game.Server.Rooms
{
	public class SwitchTeamAction : IAction
	{
		private GamePlayer m_player;
		public SwitchTeamAction(GamePlayer player)
		{
			this.m_player = player;
		}
		public void Execute()
		{
			BaseRoom currentRoom = this.m_player.CurrentRoom;
			if (currentRoom != null)
			{
				currentRoom.SwitchTeamUnsafe(this.m_player);
			}
		}
	}
}
