using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Logic;
using Game.Server;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Rooms;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;



namespace Game.Service.actions
{
	public class ConsoleStart : Game.Service.IAction
	{
		private delegate int ConsoleCtrlDelegate(ConsoleStart.ConsoleEvent ctrlType);
		private enum ConsoleEvent
		{
			Ctrl_C,
			Ctrl_Break,
			Close,
			Logoff,
			Shutdown
		}
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Timer _timer;
		private static int _count;
		private static ConsoleStart.ConsoleCtrlDelegate handler;
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
				return "Starts the DOL server in console mode";
			}
		}
		public void OnAction(Hashtable parameters)
		{
			Console.WriteLine("This server GunnyII, edit and build by SkelletonX!");
			Console.WriteLine("Starting GameServer ... please wait a moment!");
			GameServer.CreateInstance(new GameServerConfig());
			GameServer.Instance.Start();
			GameServer.KeepRunning = true;
			Console.WriteLine("Server started!");
			ConsoleClient client = new ConsoleClient();
			while (GameServer.KeepRunning)
			{
				try
				{
					ConsoleStart.handler = new ConsoleStart.ConsoleCtrlDelegate(ConsoleStart.ConsoleCtrHandler);
					ConsoleStart.SetConsoleCtrlHandler(ConsoleStart.handler, true);
					Console.Write("> ");
					string text = Console.ReadLine();
					string[] array = text.Split(new char[]
					{
						' '
					});
					string key;
					switch (key = array[0])
					{
					case "exit":
						GameServer.KeepRunning = false;
						continue;
                    //dragonares
                    case "lock":
                        Console.Clear();
                        Console.WriteLine("Ten tai khoan: ");
                        string bnickname = Console.ReadLine();
                        Console.WriteLine("Ly do band: ");
                        string breason = Console.ReadLine();
                        DateTime dt2 = new DateTime(2014, 07, 02); //Tempo de banimento
                        using (ManageBussiness mg = new ManageBussiness())
                        {
                            mg.ForbidPlayerByNickName(bnickname, dt2, false);
                        }
                        Console.WriteLine("Nguoi dung " + bnickname + " da bi khoa.");

                        break;
                    case "unlock":
                        Console.Clear();
                        Console.WriteLine("Ten tai khoan: ");
                        string bnickname2 = Console.ReadLine();
                        DateTime dt22 = new DateTime(2014, 07, 02); //Tempo de banimento
                        using (ManageBussiness mg = new ManageBussiness())
                        {
                            mg.ForbidPlayerByNickName(bnickname2, dt22, true);
                        }
                        Console.WriteLine("Nguoi dung " + bnickname2 + " da mo khoa.");

                        break;
                    case "thongbao":
                        {
                            Console.WriteLine("Thong bao: ");
                            string value = Console.ReadLine();
                            Console.WriteLine(string.Format(value));
                            Console.WriteLine("Thong bao thanh cong .");
                            continue;
                        }
					case "cp":
						{
							GameClient[] allClients = GameServer.Instance.GetAllClients();
							int num2 = (allClients == null) ? 0 : allClients.Length;
							GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
							int num3 = (allPlayers == null) ? 0 : allPlayers.Length;
							List<BaseRoom> allUsingRoom = RoomMgr.GetAllUsingRoom();
							int num4 = 0;
							int num5 = 0;
							foreach (BaseRoom current in allUsingRoom)
							{
								if (!current.IsEmpty)
								{
									num4++;
									if (current.IsPlaying)
									{
										num5++;
									}
								}
							}
							double num6 = (double)GC.GetTotalMemory(false);
							Console.WriteLine(string.Format("Total Clients/Players:{0}/{1}", num2, num3));
							Console.WriteLine(string.Format("Total Rooms/Games:{0}/{1}", num4, num5));
							Console.WriteLine(string.Format("Total Momey Used:{0} MB", num6 / 1024.0 / 1024.0));
							continue;
						}

					case "shutdown":
						ConsoleStart._count = 6;
						ConsoleStart._timer = new Timer(new TimerCallback(ConsoleStart.ShutDownCallBack), null, 0, 60000);
						continue;

					case "savemap":
						continue;

					case "clear":
						Console.Clear();
						continue;

					case "ball&reload":
						if (BallMgr.ReLoad())
						{
							Console.WriteLine("Ball info is Reload!");
							continue;
						}
						Console.WriteLine("Ball info is Error!");
						continue;

					case "map&reload":
						if (MapMgr.ReLoadMap())
						{
							Console.WriteLine("Map info is Reload!");
							continue;
						}
						Console.WriteLine("Map info is Error!");
						continue;

					case "mapserver&reload":
						if (MapMgr.ReLoadMapServer())
						{
							Console.WriteLine("mapserver info is Reload!");
							continue;
						}
						Console.WriteLine("mapserver info is Error!");
						continue;

					case "prop&reload":
						if (PropItemMgr.Reload())
						{
							Console.WriteLine("prop info is Reload!");
							continue;
						}
						Console.WriteLine("prop info is Error!");
						continue;

					case "item&reload":
						if (ItemMgr.ReLoad())
						{
							Console.WriteLine("item info is Reload!");
							continue;
						}
						Console.WriteLine("item info is Error!");
						continue;

					case "shop&reload":
						if (ShopMgr.ReLoad())
						{
							Console.WriteLine("shop info is Reload!");
							continue;
						}
						Console.WriteLine("shop info is Error!");
						continue;

					case "quest&reload":
						if (QuestMgr.ReLoad())
						{
							Console.WriteLine("quest info is Reload!");
							continue;
						}
						Console.WriteLine("quest info is Error!");
						continue;

					case "fusion&reload":
						if (FusionMgr.ReLoad())
						{
							Console.WriteLine("fusion info is Reload!");
							continue;
						}
						Console.WriteLine("fusion info is Error!");
						continue;

					case "consortia&reload":
						if (ConsortiaMgr.ReLoad())
						{
							Console.WriteLine("consortiaMgr info is Reload!");
							continue;
						}
						Console.WriteLine("consortiaMgr info is Error!");
						continue;

					case "rate&reload":
						if (RateMgr.ReLoad())
						{
							Console.WriteLine("Rate Rate is Reload!");
							continue;
						}
						Console.WriteLine("Rate Rate is Error!");
						continue;

					case "fight&reload":
						if (FightRateMgr.ReLoad())
						{
							Console.WriteLine("FightRateMgr is Reload!");
							continue;
						}
						Console.WriteLine("FightRateMgr is Error!");
						continue;

					case "dailyaward&reload":
						if (AwardMgr.ReLoad())
						{
							Console.WriteLine("dailyaward is Reload!");
							continue;
						}
						Console.WriteLine("dailyaward is Error!");
						continue;

					case "language&reload":
						if (LanguageMgr.Reload(""))
						{
							Console.WriteLine("language is Reload!");
							continue;
						}
						Console.WriteLine("language is Error!");
						continue;

					case "nickname":
						{
							Console.WriteLine("Please enter the nickname");
							string nickName = Console.ReadLine();
							string playerStringByPlayerNickName = WorldMgr.GetPlayerStringByPlayerNickName(nickName);
							Console.WriteLine(playerStringByPlayerNickName);
							continue;
						}
					}
					if (text.Length > 0)
					{
						if (text[0] == '/')
						{
							text = text.Remove(0, 1);
							text = text.Insert(0, "&");
						}
						try
						{
							if (!CommandMgr.HandleCommandNoPlvl(client, text))
							{
								Console.WriteLine("Unknown command: " + text);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
						}
					}
				}
				catch (Exception value)
				{
					Console.WriteLine(value);
				}
			}
			if (GameServer.Instance != null)
			{
				GameServer.Instance.Stop();
			}
			LogManager.Shutdown();
		}
		private static void ShutDownCallBack(object state)
		{
			ConsoleStart._count--;
			Console.WriteLine(string.Format("Server will shutdown after {0} mins!", ConsoleStart._count));
			GameClient[] allClients = GameServer.Instance.GetAllClients();
			GameClient[] array = allClients;
			for (int i = 0; i < array.Length; i++)
			{
				GameClient gameClient = array[i];
				if (gameClient.Out != null)
				{
					gameClient.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1", new object[0]), ConsoleStart._count, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2", new object[0])));
				}
                
			}
			if (ConsoleStart._count == 0)
			{
				ConsoleStart._timer.Dispose();
				ConsoleStart._timer = null;
				GameServer.Instance.Stop();
				Console.WriteLine("Server has stopped!");
			}
		}
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern int SetConsoleCtrlHandler(ConsoleStart.ConsoleCtrlDelegate HandlerRoutine, bool add);
		private static int ConsoleCtrHandler(ConsoleStart.ConsoleEvent e)
		{
			ConsoleStart.SetConsoleCtrlHandler(ConsoleStart.handler, false);
			if (GameServer.Instance != null)
			{
				GameServer.Instance.Stop();
			}
			return 0;
		}
	}
}
