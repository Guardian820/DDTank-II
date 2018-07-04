using System;
namespace Game.Server.Rooms
{
	public class StartGameMissionAction : IAction
	{
		private BaseRoom m_room;
		public StartGameMissionAction(BaseRoom room)
		{
			this.m_room = room;
		}
		public void Execute()
		{
			this.m_room.Game.MissionStart(this.m_room.Host);
		}
	}
}
