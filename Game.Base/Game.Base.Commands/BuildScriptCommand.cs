using Game.Server.Managers;
using System;
namespace Game.Base.Commands
{
	[Cmd("&cs", ePrivLevel.Player, "Compile the C# scripts.", new string[]
	{
		"/cs  <source file> <target> <importlib>",
		"eg: /cs ./scripts temp.dll game.base.dll,game.logic.dll"
	})]
	public class BuildScriptCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length >= 4)
			{
				string path = args[1];
				string dllName = args[2];
				string text = args[3];
				ScriptMgr.CompileScripts(false, path, dllName, text.Split(new char[]
				{
					','
				}));
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
	}
}
