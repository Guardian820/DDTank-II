using Bussiness;
using Bussiness.Managers;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Base.Events;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Managers;
using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
namespace Fighting.Server
{
	public class FightServer : BaseServer
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static bool KeepRunning = false;
		private FightServerConfig m_config;
		private bool m_running;
		private static FightServer m_instance;
		public static FightServer Instance
		{
			get
			{
				return FightServer.m_instance;
			}
		}
		protected override BaseClient GetNewClient()
		{
			return new ServerClient(this);
		}
		public override bool Start()
		{
			if (this.m_running)
			{
				return false;
			}
			bool result;
			try
			{
				this.m_running = true;
				Thread.CurrentThread.Priority = ThreadPriority.Normal;
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
				if (!this.InitComponent(this.InitSocket(this.m_config.Ip, this.m_config.Port), "InitSocket Port:" + this.m_config.Port))
				{
					result = false;
				}
				else
				{
					if (!this.InitComponent(this.StartScriptComponents(), "Script components"))
					{
						result = false;
					}
					else
					{
						if (!this.InitComponent(ProxyRoomMgr.Setup(), "RoomMgr.Setup"))
						{
							result = false;
						}
						else
						{
							if (!this.InitComponent(GameMgr.Setup(0, 4), "GameMgr.Setup"))
							{
								result = false;
							}
							else
							{
								if (!this.InitComponent(MapMgr.Init(), "MapMgr Init"))
								{
									result = false;
								}
								else
								{
									if (!this.InitComponent(ItemMgr.Init(), "ItemMgr Init"))
									{
										result = false;
									}
									else
									{
										if (!this.InitComponent(PropItemMgr.Init(), "PropItemMgr Init"))
										{
											result = false;
										}
										else
										{
											if (!this.InitComponent(BallMgr.Init(), "BallMgr Init"))
											{
												result = false;
											}
											else
											{
												if (!this.InitComponent(BallConfigMgr.Init(), "BallConfigMgr Init"))
												{
													result = false;
												}
												else
												{
													if (!this.InitComponent(DropMgr.Init(), "DropMgr Init"))
													{
														result = false;
													}
													else
													{
														if (!this.InitComponent(NPCInfoMgr.Init(), "NPCInfoMgr Init"))
														{
															result = false;
														}
														else
														{
															if (!this.InitComponent(WindMgr.Init(), "WindMgr Init"))
															{
																result = false;
															}
															else
															{
																if (!this.InitComponent(GoldEquipMgr.Init(), "GoldEquipMgr Init"))
																{
																	result = false;
																}
																else
																{
																	if (!this.InitComponent(LanguageMgr.Setup(""), "LanguageMgr Init"))
																	{
																		result = false;
																	}
																	else
																	{
																		GameEventMgr.Notify(ScriptEvent.Loaded);
																		if (!this.InitComponent(base.Start(), "base.Start()"))
																		{
																			result = false;
																		}
																		else
																		{
																			ProxyRoomMgr.Start();
																			GameMgr.Start();
																			GameEventMgr.Notify(GameServerEvent.Started, this);
																			GC.Collect(GC.MaxGeneration);
																			FightServer.log.Info("GameServer is now open for connections!");
																			result = true;
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				FightServer.log.Error("Failed to start the server", exception);
				result = false;
			}
			return result;
		}
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			FightServer.log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
			if (e.IsTerminating)
			{
				LogManager.Shutdown();
			}
		}
		protected bool InitComponent(bool componentInitState, string text)
		{
			FightServer.log.Info(text + ": " + componentInitState);
			if (!componentInitState)
			{
				this.Stop();
			}
			return componentInitState;
		}
		protected bool StartScriptComponents()
		{
			bool result;
			try
			{
				ScriptMgr.InsertAssembly(typeof(FightServer).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
				Assembly[] scripts = ScriptMgr.Scripts;
				Assembly[] array = scripts;
				for (int i = 0; i < array.Length; i++)
				{
					Assembly asm = array[i];
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
				}
				FightServer.log.Info("Registering global event handlers: true");
				result = true;
			}
			catch (Exception exception)
			{
				FightServer.log.Error("StartScriptComponents", exception);
				result = false;
			}
			return result;
		}
		public override void Stop()
		{
			if (!this.m_running)
			{
				return;
			}
			try
			{
				this.m_running = false;
				GameMgr.Stop();
				ProxyRoomMgr.Stop();
			}
			catch (Exception exception)
			{
				FightServer.log.Error("Server stopp error:", exception);
			}
			finally
			{
				base.Stop();
			}
		}
		public new ServerClient[] GetAllClients()
		{
			ServerClient[] array = null;
			object syncRoot;
			Monitor.Enter(syncRoot = this._clients.SyncRoot);
			try
			{
				array = new ServerClient[this._clients.Count];
				this._clients.Keys.CopyTo(array, 0);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			return array;
		}
		public void SendToALL(GSPacketIn pkg)
		{
			this.SendToALL(pkg, null);
		}
		public void SendToALL(GSPacketIn pkg, ServerClient except)
		{
			ServerClient[] allClients = this.GetAllClients();
			if (allClients != null)
			{
				ServerClient[] array = allClients;
				for (int i = 0; i < array.Length; i++)
				{
					ServerClient serverClient = array[i];
					if (serverClient != except)
					{
						serverClient.SendTCP(pkg);
					}
				}
			}
		}
		private FightServer(FightServerConfig config)
		{
			this.m_config = config;
		}
		public static void CreateInstance(FightServerConfig config)
		{
			if (FightServer.m_instance != null)
			{
				return;
			}
			FileInfo fileInfo = new FileInfo(config.LogConfigFile);
			if (!fileInfo.Exists)
			{
				ResourceUtil.ExtractResource(fileInfo.Name, fileInfo.FullName, Assembly.GetAssembly(typeof(FightServer)));
			}
			XmlConfigurator.ConfigureAndWatch(fileInfo);
			FightServer.m_instance = new FightServer(config);
		}
	}
}
