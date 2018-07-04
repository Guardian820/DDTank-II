using Game.Base;
using System;
namespace GameServerScript.Commands
{
	[Cmd("&changetask", ePrivLevel.Admin, "Test", new string[]
	{
		"changetast...."
	})]
	public class TestTaskCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				string message = args[i];
				this.DisplayMessage(client, message);
			}
			return true;
		}
	}
}
