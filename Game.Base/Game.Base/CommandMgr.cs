using Game.Base.Events;
using Game.Server.Managers;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace Game.Base
{
	public class CommandMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Hashtable m_cmds = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		private static string[] m_disabledarray = new string[0];
		public static string[] DisableCommands
		{
			get
			{
				return CommandMgr.m_disabledarray;
			}
			set
			{
				CommandMgr.m_disabledarray = ((value == null) ? new string[0] : value);
			}
		}
		public static GameCommand GetCommand(string cmd)
		{
			return CommandMgr.m_cmds[cmd] as GameCommand;
		}
		public static GameCommand GuessCommand(string cmd)
		{
			GameCommand gameCommand = CommandMgr.GetCommand(cmd);
			if (gameCommand != null)
			{
				return gameCommand;
			}
			string value = cmd.ToLower();
			IDictionaryEnumerator enumerator = CommandMgr.m_cmds.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GameCommand gameCommand2 = enumerator.Value as GameCommand;
				string text = enumerator.Key as string;
				if (gameCommand2 != null && text.ToLower().StartsWith(value))
				{
					gameCommand = gameCommand2;
					break;
				}
			}
			return gameCommand;
		}
		public static string[] GetCommandList(ePrivLevel plvl, bool addDesc)
		{
			IDictionaryEnumerator enumerator = CommandMgr.m_cmds.GetEnumerator();
			ArrayList arrayList = new ArrayList();
			while (enumerator.MoveNext())
			{
				GameCommand gameCommand = enumerator.Value as GameCommand;
				string text = enumerator.Key as string;
				if (gameCommand != null && text != null)
				{
					if (text[0] == '&')
					{
						text = '/' + text.Remove(0, 1);
					}
					if (plvl >= (ePrivLevel)gameCommand.m_lvl)
					{
						if (addDesc)
						{
							arrayList.Add(text + " - " + gameCommand.m_desc);
						}
						else
						{
							arrayList.Add(gameCommand.m_cmd);
						}
					}
				}
			}
			return (string[])arrayList.ToArray(typeof(string));
		}
		[ScriptLoadedEvent]
		public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
		{
			CommandMgr.LoadCommands();
		}
		public static bool LoadCommands()
		{
			CommandMgr.m_cmds.Clear();
			ArrayList arrayList = new ArrayList(ScriptMgr.Scripts);
			foreach (Assembly assembly in arrayList)
			{
				if (CommandMgr.log.IsDebugEnabled)
				{
					CommandMgr.log.Debug("ScriptMgr: Searching for commands in " + assembly.GetName());
				}
				Type[] types = assembly.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					Type type = types[i];
					if (type.IsClass && type.GetInterface("Game.Base.ICommandHandler") != null)
					{
						try
						{
							object[] customAttributes = type.GetCustomAttributes(typeof(CmdAttribute), false);
							object[] array = customAttributes;
							for (int j = 0; j < array.Length; j++)
							{
								CmdAttribute cmdAttribute = (CmdAttribute)array[j];
								bool flag = false;
								string[] disabledarray = CommandMgr.m_disabledarray;
								for (int k = 0; k < disabledarray.Length; k++)
								{
									string b = disabledarray[k];
									if (cmdAttribute.Cmd.Replace('&', '/') == b)
									{
										flag = true;
										CommandMgr.log.Info("Will not load command " + cmdAttribute.Cmd + " as it is disabled in game properties");
										break;
									}
								}
								if (!flag)
								{
									if (CommandMgr.m_cmds.ContainsKey(cmdAttribute.Cmd))
									{
										CommandMgr.log.Info(string.Concat(new object[]
										{
											cmdAttribute.Cmd,
											" from ",
											assembly.GetName(),
											" has been suppressed, a command of that type already exists!"
										}));
									}
									else
									{
										if (CommandMgr.log.IsDebugEnabled)
										{
											CommandMgr.log.Debug("Load: " + cmdAttribute.Cmd + "," + cmdAttribute.Description);
										}
										GameCommand gameCommand = new GameCommand();
										gameCommand.m_usage = cmdAttribute.Usage;
										gameCommand.m_cmd = cmdAttribute.Cmd;
										gameCommand.m_lvl = cmdAttribute.Level;
										gameCommand.m_desc = cmdAttribute.Description;
										gameCommand.m_cmdHandler = (ICommandHandler)Activator.CreateInstance(type);
										CommandMgr.m_cmds.Add(cmdAttribute.Cmd, gameCommand);
										if (cmdAttribute.Aliases != null)
										{
											string[] aliases = cmdAttribute.Aliases;
											for (int l = 0; l < aliases.Length; l++)
											{
												string key = aliases[l];
												CommandMgr.m_cmds.Add(key, gameCommand);
											}
										}
									}
								}
							}
						}
						catch (Exception exception)
						{
							if (CommandMgr.log.IsErrorEnabled)
							{
								CommandMgr.log.Error("LoadCommands", exception);
							}
						}
					}
				}
			}
			CommandMgr.log.Info("Loaded " + CommandMgr.m_cmds.Count + " commands!");
			return true;
		}
		public static void DisplaySyntax(BaseClient client)
		{
			client.DisplayMessage("Commands list:");
			string[] commandList = CommandMgr.GetCommandList(ePrivLevel.Admin, true);
			for (int i = 0; i < commandList.Length; i++)
			{
				string str = commandList[i];
				client.DisplayMessage("         " + str);
			}
		}
		public static bool HandleCommandNoPlvl(BaseClient client, string cmdLine)
		{
			try
			{
				string[] array = CommandMgr.ParseCmdLine(cmdLine);
				GameCommand gameCommand = CommandMgr.GuessCommand(array[0]);
				if (gameCommand == null)
				{
					return false;
				}
				CommandMgr.ExecuteCommand(client, gameCommand, array);
			}
			catch (Exception exception)
			{
				if (CommandMgr.log.IsErrorEnabled)
				{
					CommandMgr.log.Error("HandleCommandNoPlvl", exception);
				}
			}
			return true;
		}
		private static bool ExecuteCommand(BaseClient client, GameCommand myCommand, string[] pars)
		{
			pars[0] = myCommand.m_cmd;
			return myCommand.m_cmdHandler.OnCommand(client, pars);
		}
		private static string[] ParseCmdLine(string cmdLine)
		{
			if (cmdLine == null)
			{
				throw new ArgumentNullException("cmdLine");
			}
			List<string> list = new List<string>();
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder(cmdLine.Length >> 1);
			for (int i = 0; i < cmdLine.Length; i++)
			{
				char c = cmdLine[i];
				switch (num)
				{
				case 0:
					if (c != ' ')
					{
						stringBuilder.Length = 0;
						if (c == '"')
						{
							num = 2;
						}
						else
						{
							num = 1;
							i--;
						}
					}
					break;

				case 1:
					if (c == ' ')
					{
						list.Add(stringBuilder.ToString());
						num = 0;
					}
					stringBuilder.Append(c);
					break;

				case 2:
					if (c == '"')
					{
						list.Add(stringBuilder.ToString());
						num = 0;
					}
					stringBuilder.Append(c);
					break;
				}
			}
			if (num != 0)
			{
				list.Add(stringBuilder.ToString());
			}
			string[] array = new string[list.Count];
			list.CopyTo(array);
			return array;
		}
	}
}
