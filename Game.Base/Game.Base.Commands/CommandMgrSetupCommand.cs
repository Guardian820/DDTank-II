using System;
namespace Game.Base.Commands
{
	[Cmd("&cmd", ePrivLevel.Admin, "Config the command system.", new string[]
	{
		"/cmd [option] <para1> <para2>      ",
		"eg: /cmd -reload           :Reload the command system.",
		"    /cmd -list             :Display all commands."
	})]
	public class CommandMgrSetupCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				string a;
				if ((a = args[1]) != null)
				{
					if (a == "-reload")
					{
						CommandMgr.LoadCommands();
						return true;
					}
					if (a == "-list")
					{
						CommandMgr.DisplaySyntax(client);
						return true;
					}
				}
				this.DisplaySyntax(client);
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
	}
}
