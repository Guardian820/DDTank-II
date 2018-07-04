using Game.Base;
using Game.Server;
using System;
namespace GameServerScript.Commands
{
	[Cmd("&version", ePrivLevel.Player, "Get the version of the GameServer", new string[]
	{
		"/version"
	})]
	public class VersionCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			this.DisplayMessage(client, "Servion Id:{0},Version:{1}", new object[]
			{
				GameServer.Instance.Configuration.ServerID,
				GameServer.Edition
			});
			return true;
		}
	}
}
