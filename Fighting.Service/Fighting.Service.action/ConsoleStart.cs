using Fighting.Server;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Logic;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
namespace Fighting.Service.action
{
	public class ConsoleStart : Fighting.Service.IAction
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public string HelpStr
		{
			get
			{
				return ConfigurationManager.AppSettings["HelpStr"];
			}
		}
		public string Name
		{
			get
			{
				return "--start";
			}
		}
		public string Syntax
		{
			get
			{
				return "--start [-config=./config/serverconfig.xml]";
			}
		}
		public string Description
		{
			get
			{
				return "Starts the Fighting server in console mode";
			}
		}
		public void OnAction(System.Collections.Hashtable parameters)
		{
			System.Console.WriteLine("This server GunnyII, edit and build by MrPhuong!");
			System.Console.WriteLine("Starting FightingServer ... please wait a moment!");
			FightServerConfig fightServerConfig = new FightServerConfig();
			try
			{
				fightServerConfig.Load();
			}
			catch (System.Exception ex)
			{
				System.Console.WriteLine(ex.Message);
				System.Console.ReadKey();
				return;
			}
			FightServer.CreateInstance(fightServerConfig);
			FightServer.Instance.Start();
			bool flag = true;
			while (flag)
			{
				try
				{
					System.Console.Write("> ");
					string text = System.Console.ReadLine();
					string[] array = text.Split(new char[]
					{
						' '
					});
					string a;
					if ((a = array[0].ToLower()) != null)
					{
						if (!(a == "clear"))
						{
							if (!(a == "list"))
							{
								if (a == "exit")
								{
									flag = false;
								}
							}
							else
							{
								if (array.Length > 1)
								{
									string a2;
									if ((a2 = array[1]) != null)
									{
										if (!(a2 == "-client"))
										{
											if (!(a2 == "-room"))
											{
												if (a2 == "-game")
												{
													System.Console.WriteLine("game list:");
													System.Console.WriteLine("-------------------------------");
													System.Collections.Generic.List<BaseGame> games = GameMgr.GetGames();
													foreach (BaseGame current in games)
													{
														System.Console.WriteLine(current.ToString());
													}
													System.Console.WriteLine("-------------------------------");
												}
											}
											else
											{
												System.Console.WriteLine("room list:");
												System.Console.WriteLine("-------------------------------");
												ProxyRoom[] allRoom = ProxyRoomMgr.GetAllRoom();
												ProxyRoom[] array2 = allRoom;
												for (int i = 0; i < array2.Length; i++)
												{
													ProxyRoom proxyRoom = array2[i];
													System.Console.WriteLine(proxyRoom.ToString());
												}
												System.Console.WriteLine("-------------------------------");
											}
										}
										else
										{
											System.Console.WriteLine("server client list:");
											System.Console.WriteLine("--------------------");
											ServerClient[] allClients = FightServer.Instance.GetAllClients();
											ServerClient[] array3 = allClients;
											for (int j = 0; j < array3.Length; j++)
											{
												ServerClient serverClient = array3[j];
												System.Console.WriteLine(serverClient.ToString());
											}
											System.Console.WriteLine("-------------------");
										}
									}
								}
								else
								{
									System.Console.WriteLine("list [-client][-room][-game]");
									System.Console.WriteLine("     -client:列出所有服务器对象");
									System.Console.WriteLine("     -room:列出所有房间对象");
									System.Console.WriteLine("     -game:列出所有游戏对象");
								}
							}
						}
						else
						{
							System.Console.Clear();
						}
					}
				}
				catch (System.Exception ex2)
				{
					System.Console.WriteLine("Error:" + ex2.ToString());
				}
			}
			if (FightServer.Instance != null)
			{
				FightServer.Instance.Stop();
			}
		}
	}
}
