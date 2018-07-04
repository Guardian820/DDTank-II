using Game.Server.Managers;
using System;
using System.IO;
using System.Reflection;
namespace Game.Base.Commands
{
	[Cmd("&sm", ePrivLevel.Player, "Script Manager console commands.", new string[]
	{
		"   /sm  <option>  [para1][para2]...",
		"eg: /sm -list              : List all assemblies in scripts array.",
		"    /sm -add <assembly>    : Add assembly into the scripts array.",
		"    /sm -remove <assembly> : Remove assembly from the scripts array."
	})]
	public class ScriptManagerCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				string a;
				if ((a = args[1]) != null)
				{
					if (!(a == "-list"))
					{
						if (a == "-add")
						{
							if (args.Length > 2 && args[2] != null && File.Exists(args[2]))
							{
								try
								{
									Assembly ass = Assembly.LoadFile(args[2]);
									bool result;
									if (ScriptMgr.InsertAssembly(ass))
									{
										this.DisplayMessage(client, "Add assembly success!");
										result = true;
										return result;
									}
									this.DisplayMessage(client, "Assembly already exists in the scripts array!");
									result = false;
									return result;
								}
								catch (Exception ex)
								{
									this.DisplayMessage(client, "Add assembly error:", new object[]
									{
										ex.Message
									});
									bool result = false;
									return result;
								}
							}
							this.DisplayMessage(client, "Can't find add assembly!");
							return false;
						}
						if (!(a == "-remove"))
						{
							goto IL_178;
						}
						if (args.Length > 2 && args[2] != null && File.Exists(args[2]))
						{
							try
							{
								Assembly ass2 = Assembly.LoadFile(args[2]);
								bool result;
								if (ScriptMgr.RemoveAssembly(ass2))
								{
									this.DisplayMessage(client, "Remove assembly success!");
									result = true;
									return result;
								}
								this.DisplayMessage(client, "Assembly didn't exist in the scripts array!");
								result = false;
								return result;
							}
							catch (Exception ex2)
							{
								this.DisplayMessage(client, "Remove assembly error:", new object[]
								{
									ex2.Message
								});
								bool result = false;
								return result;
							}
						}
						this.DisplayMessage(client, "Can't find remove assembly!");
						return false;
					}
					Assembly[] scripts = ScriptMgr.Scripts;
					for (int i = 0; i < scripts.Length; i++)
					{
						Assembly assembly = scripts[i];
						this.DisplayMessage(client, assembly.FullName);
					}
					return true;
				}
				IL_178:
				this.DisplayMessage(client, "Can't fine option:{0}", new object[]
				{
					args[1]
				});
				return true;
			}
			this.DisplaySyntax(client);
			return true;
		}
	}
}
