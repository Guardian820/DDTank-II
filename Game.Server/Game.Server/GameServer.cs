using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Events;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Rooms;
using Game.Server.Statics;
using log4net;
using log4net.Config;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Server
{
	public class GameServer : BaseServer
	{
		private const int BUF_SIZE = 30720;
		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static readonly string Edition = "5498628";
		public static bool KeepRunning = false;
		private static GameServer m_instance = null;
		private bool m_isRunning;
		private GameServerConfig m_config;
		private LoginServerConnector _loginServer;
		private Queue m_packetBufPool;
		private bool m_debugMenory;
		private static int m_tryCount = 4;
		private static bool m_compiled = false;
		private Timer _shutdownTimer;
		private int _shutdownCount = 6;
		protected Timer m_saveDbTimer;
		protected Timer m_pingCheckTimer;
		protected Timer m_saveRecordTimer;
		protected Timer m_buffScanTimer;
		public static GameServer Instance
		{
			get
			{
				return GameServer.m_instance;
			}
		}
		public GameServerConfig Configuration
		{
			get
			{
				return this.m_config;
			}
		}
		public LoginServerConnector LoginServer
		{
			get
			{
				return this._loginServer;
			}
		}
		public int PacketPoolSize
		{
			get
			{
				return this.m_packetBufPool.Count;
			}
		}
		public static void CreateInstance(GameServerConfig config)
		{
			if (GameServer.m_instance != null)
			{
				return;
			}
			FileInfo fileInfo = new FileInfo(config.LogConfigFile);
			if (!fileInfo.Exists)
			{
				ResourceUtil.ExtractResource(fileInfo.Name, fileInfo.FullName, Assembly.GetAssembly(typeof(GameServer)));
			}
			XmlConfigurator.ConfigureAndWatch(fileInfo);
			GameServer.m_instance = new GameServer(config);
		}
		protected GameServer(GameServerConfig config)
		{
			this.m_config = config;
			if (GameServer.log.IsDebugEnabled)
			{
				GameServer.log.Debug("Current directory is: " + Directory.GetCurrentDirectory());
				GameServer.log.Debug("Gameserver root directory is: " + this.Configuration.RootDirectory);
				GameServer.log.Debug("Changing directory to root directory");
			}
			Directory.SetCurrentDirectory(this.Configuration.RootDirectory);
		}
		private bool AllocatePacketBuffers()
		{
			int num = this.Configuration.MaxClientCount * 3;
			this.m_packetBufPool = new Queue(num);
			for (int i = 0; i < num; i++)
			{
				this.m_packetBufPool.Enqueue(new byte[30720]);
			}
			if (GameServer.log.IsDebugEnabled)
			{
				GameServer.log.DebugFormat("allocated packet buffers: {0}", num.ToString());
			}
			return true;
		}
		public byte[] AcquirePacketBuffer()
		{
			object syncRoot;
			Monitor.Enter(syncRoot = this.m_packetBufPool.SyncRoot);
			try
			{
				if (this.m_packetBufPool.Count > 0)
				{
					return (byte[])this.m_packetBufPool.Dequeue();
				}
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			GameServer.log.Warn("packet buffer pool is empty!");
			return new byte[30720];
		}
		public void ReleasePacketBuffer(byte[] buf)
		{
			if (buf == null || GC.GetGeneration(buf) < GC.MaxGeneration)
			{
				return;
			}
			object syncRoot;
			Monitor.Enter(syncRoot = this.m_packetBufPool.SyncRoot);
			try
			{
				this.m_packetBufPool.Enqueue(buf);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}
		protected override BaseClient GetNewClient()
		{
			return new GameClient(this, this.AcquirePacketBuffer(), this.AcquirePacketBuffer());
		}
		public new GameClient[] GetAllClients()
		{
			GameClient[] array = null;
			object syncRoot;
			Monitor.Enter(syncRoot = this._clients.SyncRoot);
			try
			{
				array = new GameClient[this._clients.Count];
				this._clients.Keys.CopyTo(array, 0);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			return array;
		}
		public override bool Start()
		{
			if (this.m_isRunning)
			{
				return false;
			}
			bool result;
			try
			{
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
				Thread.CurrentThread.Priority = ThreadPriority.Normal;
				GameProperties.Refresh();
				if (!this.InitComponent(this.RecompileScripts(), "Recompile Scripts"))
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
						if (!this.InitComponent(GameProperties.EDITION == GameServer.Edition, "Edition: " + GameServer.Edition))
						{
							result = false;
						}
						else
						{
							if (!this.InitComponent(this.InitSocket(IPAddress.Parse(this.Configuration.Ip), this.Configuration.Port), "InitSocket Port: " + this.Configuration.Port))
							{
								result = false;
							}
							else
							{
								if (!this.InitComponent(this.AllocatePacketBuffers(), "AllocatePacketBuffers()"))
								{
									result = false;
								}
								else
								{
									if (!this.InitComponent(LogMgr.Setup(this.Configuration.GAME_TYPE, this.Configuration.ServerID, this.Configuration.AreaID), "LogMgr Init"))
									{
										result = false;
									}
									else
									{
										if (!this.InitComponent(WorldMgr.Init(), "WorldMgr Init"))
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
													if (!this.InitComponent(ItemBoxMgr.Init(), "ItemBox Init"))
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
															if (!this.InitComponent(ExerciseMgr.Init(), "ExerciseMgr Init"))
															{
																result = false;
															}
															else
															{
																if (!this.InitComponent(LevelMgr.Init(), "levelMgr Init"))
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
																		if (!this.InitComponent(FusionMgr.Init(), "FusionMgr Init"))
																		{
																			result = false;
																		}
																		else
																		{
																			if (!this.InitComponent(AwardMgr.Init(), "AwardMgr Init"))
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
																					if (!this.InitComponent(MissionInfoMgr.Init(), "MissionInfoMgr Init"))
																					{
																						result = false;
																					}
																					else
																					{
																						if (!this.InitComponent(PveInfoMgr.Init(), "PveInfoMgr Init"))
																						{
																							result = false;
																						}
																						else
																						{
																							if (!this.InitComponent(DropMgr.Init(), "Drop Init"))
																							{
																								result = false;
																							}
																							else
																							{
																								if (!this.InitComponent(FightRateMgr.Init(), "FightRateMgr Init"))
																								{
																									result = false;
																								}
																								else
																								{
																									if (!this.InitComponent(ConsortiaLevelMgr.Init(), "ConsortiaLevelMgr Init"))
																									{
																										result = false;
																									}
																									else
																									{
																										if (!this.InitComponent(RefineryMgr.Init(), "RefineryMgr Init"))
																										{
																											result = false;
																										}
																										else
																										{
																											if (!this.InitComponent(StrengthenMgr.Init(), "StrengthenMgr Init"))
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
																													if (!this.InitComponent(ShopMgr.Init(), "ShopMgr Init"))
																													{
																														result = false;
																													}
																													else
																													{
																														if (!this.InitComponent(QuestMgr.Init(), "QuestMgr Init"))
																														{
																															result = false;
																														}
																														else
																														{
																															if (!this.InitComponent(RoomMgr.Setup(this.Configuration.MaxRoomCount), "RoomMgr.Setup"))
																															{
																																result = false;
																															}
																															else
																															{
																																if (!this.InitComponent(GameMgr.Setup(this.Configuration.ServerID, GameProperties.BOX_APPEAR_CONDITION), "GameMgr.Start()"))
																																{
																																	result = false;
																																}
																																else
																																{
																																	if (!this.InitComponent(ConsortiaMgr.Init(), "ConsortiaMgr Init"))
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
																																			if (!this.InitComponent(RateMgr.Init(this.Configuration), "ExperienceRateMgr Init"))
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
																																					if (!this.InitComponent(CardMgr.Init(), "CardMgr Init"))
																																					{
																																						result = false;
																																					}
																																					else
																																					{
																																						if (!this.InitComponent(PetMgr.Init(), "PetMgr Init"))
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
																																								if (!this.InitComponent(RuneMgr.Init(), "RuneMgr Init"))
																																								{
																																									result = false;
																																								}
																																								else
																																								{
																																									if (!this.InitComponent(TotemMgr.Init(), "TotemMgr Init"))
																																									{
																																										result = false;
																																									}
																																									else
																																									{
																																										if (!this.InitComponent(TotemHonorMgr.Init(), "TotemHonorMgr Init"))
																																										{
																																											result = false;
																																										}
																																										else
																																										{
																																											if (!this.InitComponent(FightSpiritTemplateMgr.Init(), "FightSpiritTemplateMgr Init"))
																																											{
																																												result = false;
																																											}
																																											else
																																											{
																																												if (!this.InitComponent(MacroDropMgr.Init(), "MacroDropMgr Init"))
																																												{
																																													result = false;
																																												}
																																												else
																																												{
																																													if (!this.InitComponent(BattleMgr.Setup(), "BattleMgr Setup"))
																																													{
																																														result = false;
																																													}
																																													else
																																													{
																																														if (!this.InitComponent(this.InitGlobalTimer(), "Init Global Timers"))
																																														{
																																															result = false;
																																														}
																																														else
																																														{
																																															if (!this.InitComponent(MarryRoomMgr.Init(), "MarryRoomMgr Init"))
																																															{
																																																result = false;
																																															}
																																															else
																																															{
																																																if (!this.InitComponent(DiceSystemMgr.Init(), "DiceSystemMgr Init"))
																																																{
																																																	result = false;
																																																}
																																																else
																																																{
																																																	if (!this.InitComponent(LogMgr.Setup(1, 4, 4), "LogMgr Setup"))
																																																	{
																																																		result = false;
																																																	}
																																																	else
																																																	{
																																																		GameEventMgr.Notify(ScriptEvent.Loaded);
																																																		if (!this.InitComponent(this.InitLoginServer(), "Login To CenterServer"))
																																																		{
																																																			result = false;
																																																		}
																																																		else
																																																		{
																																																			RoomMgr.Start();
																																																			GameMgr.Start();
																																																			BattleMgr.Start();
																																																			MacroDropMgr.Start();
																																																			if (!this.InitComponent(base.Start(), "base.Start()"))
																																																			{
																																																				result = false;
																																																			}
																																																			else
																																																			{
																																																				GameEventMgr.Notify(GameServerEvent.Started, this);
																																																				GC.Collect(GC.MaxGeneration);
																																																				if (GameServer.log.IsInfoEnabled)
																																																				{
																																																					GameServer.log.Info("GameServer is now open for connections!");
																																																				}
																																																				this.m_isRunning = true;
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
			}
			catch (Exception exception)
			{
				if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("Failed to start the server", exception);
				}
				result = false;
			}
			return result;
		}
		private bool InitLoginServer()
		{
			this._loginServer = new LoginServerConnector(this.m_config.LoginServerIp, this.m_config.LoginServerPort, this.m_config.ServerID, this.m_config.ServerName, this.AcquirePacketBuffer(), this.AcquirePacketBuffer());
			this._loginServer.Disconnected += new ClientEventHandle(this.loginServer_Disconnected);
			return this._loginServer.Connect();
		}
		private void loginServer_Disconnected(BaseClient client)
		{
			bool isRunning = this.m_isRunning;
			this.Stop();
			if (isRunning && GameServer.m_tryCount > 0)
			{
				GameServer.m_tryCount--;
				GameServer.log.Error("Center Server Disconnect! Stopping Server");
				GameServer.log.ErrorFormat("Start the game server again after 1 second,and left try times:{0}", GameServer.m_tryCount);
				Thread.Sleep(1000);
				if (this.Start())
				{
					GameServer.log.Error("Restart the game server success!");
					return;
				}
			}
			else
			{
				if (GameServer.m_tryCount == 0)
				{
					GameServer.log.ErrorFormat("Restart the game server failed after {0} times.", 4);
					GameServer.log.Error("Server Stopped!");
				}
				LogManager.Shutdown();
			}
		}
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				GameServer.log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
				if (e.IsTerminating)
				{
					this.Stop();
				}
			}
			catch
			{
				try
				{
					using (FileStream fileStream = new FileStream("c:\\testme.log", FileMode.Append, FileAccess.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
						{
							streamWriter.WriteLine(e.ExceptionObject);
						}
					}
				}
				catch
				{
				}
			}
		}
		public bool RecompileScripts()
		{
			if (!GameServer.m_compiled)
			{
				string path = this.Configuration.RootDirectory + Path.DirectorySeparatorChar + "scripts";
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				string[] asm_names = this.Configuration.ScriptAssemblies.Split(new char[]
				{
					','
				});
				GameServer.m_compiled = ScriptMgr.CompileScripts(false, path, this.Configuration.ScriptCompilationTarget, asm_names);
			}
			return GameServer.m_compiled;
		}
		protected bool StartScriptComponents()
		{
			try
			{
				if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Server rules: true");
				}
				ScriptMgr.InsertAssembly(typeof(GameServer).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
				ArrayList arrayList = new ArrayList(ScriptMgr.Scripts);
				foreach (Assembly asm in arrayList)
				{
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
				}
				if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Registering global event handlers: true");
				}
			}
			catch (Exception exception)
			{
				if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("StartScriptComponents", exception);
				}
				return false;
			}
			return true;
		}
		protected bool InitComponent(bool componentInitState, string text)
		{
			if (this.m_debugMenory)
			{
				GameServer.log.Debug(string.Concat(new object[]
				{
					"Start Memory ",
					text,
					": ",
					GC.GetTotalMemory(false) / 1024L / 1024L
				}));
			}
			if (GameServer.log.IsInfoEnabled)
			{
				GameServer.log.Info(text + ": " + componentInitState);
			}
			if (!componentInitState)
			{
				this.Stop();
			}
			if (this.m_debugMenory)
			{
				GameServer.log.Debug(string.Concat(new object[]
				{
					"Finish Memory ",
					text,
					": ",
					GC.GetTotalMemory(false) / 1024L / 1024L
				}));
			}
			return componentInitState;
		}
		public override void Stop()
		{
			if (this.m_isRunning)
			{
				this.m_isRunning = false;
				if (!MarryRoomMgr.UpdateBreakTimeWhereServerStop())
				{
					Console.WriteLine("Update BreakTime failed");
				}
				RoomMgr.Stop();
				GameMgr.Stop();
				if (this._loginServer != null)
				{
					this._loginServer.Disconnected -= new ClientEventHandle(this.loginServer_Disconnected);
					this._loginServer.Disconnect();
				}
				if (this.m_pingCheckTimer != null)
				{
					this.m_pingCheckTimer.Change(-1, -1);
					this.m_pingCheckTimer.Dispose();
					this.m_pingCheckTimer = null;
				}
				if (this.m_saveDbTimer != null)
				{
					this.m_saveDbTimer.Change(-1, -1);
					this.m_saveDbTimer.Dispose();
					this.m_saveDbTimer = null;
				}
				if (this.m_saveRecordTimer != null)
				{
					this.m_saveRecordTimer.Change(-1, -1);
					this.m_saveRecordTimer.Dispose();
					this.m_saveRecordTimer = null;
					this.SaveRecordProc(null);
				}
				if (this.m_buffScanTimer != null)
				{
					this.m_buffScanTimer.Change(-1, -1);
					this.m_buffScanTimer.Dispose();
					this.m_buffScanTimer = null;
				}
				this.SaveTimerProc(null);
				base.Stop();
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				GameServer.log.Info("Server Stopped!");
				Console.WriteLine("Server Stopped!");
			}
		}
		public void Shutdown()
		{
			GameServer.Instance.LoginServer.SendShutdown(true);
			this._shutdownTimer = new Timer(new TimerCallback(this.ShutDownCallBack), null, 0, 60000);
		}
		private void ShutDownCallBack(object state)
		{
			try
			{
				this._shutdownCount--;
				Console.WriteLine(string.Format("Server will shutdown after {0} mins!", this._shutdownCount));
				GameClient[] allClients = GameServer.Instance.GetAllClients();
				GameClient[] array = allClients;
				for (int i = 0; i < array.Length; i++)
				{
					GameClient gameClient = array[i];
					if (gameClient.Out != null)
					{
						gameClient.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1", new object[0]), this._shutdownCount, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2", new object[0])));
					}
				}
				if (this._shutdownCount == 0)
				{
					Console.WriteLine("Server has stopped!");
					GameServer.Instance.LoginServer.SendShutdown(false);
					this._shutdownTimer.Dispose();
					this._shutdownTimer = null;
					GameServer.Instance.Stop();
				}
			}
			catch (Exception message)
			{
				GameServer.log.Error(message);
			}
		}
		public bool InitGlobalTimer()
		{
			int num = this.Configuration.PingCheckInterval * 60 * 1000;
			if (this.m_pingCheckTimer == null)
			{
				this.m_pingCheckTimer = new Timer(new TimerCallback(this.PingCheck), null, num, num);
			}
			else
			{
				this.m_pingCheckTimer.Change(num, num);
			}
			num = this.Configuration.SaveRecordInterval * 60 * 1000;
			if (this.m_saveRecordTimer == null)
			{
				this.m_saveRecordTimer = new Timer(new TimerCallback(this.SaveRecordProc), null, num, num);
			}
			else
			{
				this.m_saveRecordTimer.Change(num, num);
			}
			num = 60000;
			if (this.m_buffScanTimer == null)
			{
				this.m_buffScanTimer = new Timer(new TimerCallback(this.BuffScanTimerProc), null, num, num);
			}
			else
			{
				this.m_buffScanTimer.Change(num, num);
			}
			return true;
		}
		protected void PingCheck(object sender)
		{
			try
			{
				GameServer.log.Info("Begin ping check....");
				long num = (long)this.Configuration.PingCheckInterval * 60L * 1000L * 1000L * 10L;
				GameClient[] allClients = this.GetAllClients();
				if (allClients != null)
				{
					GameClient[] array = allClients;
					for (int i = 0; i < array.Length; i++)
					{
						GameClient gameClient = array[i];
						if (gameClient != null)
						{
							if (gameClient.IsConnected)
							{
								if (gameClient.Player != null)
								{
									gameClient.Out.SendPingTime(gameClient.Player);
									if (AntiAddictionMgr.ISASSon && AntiAddictionMgr.count == 0)
									{
										AntiAddictionMgr.count++;
									}
								}
								else
								{
									if (gameClient.PingTime + num < DateTime.Now.Ticks)
									{
										Console.WriteLine(gameClient.Player.PlayerCharacter.NickName + " PingCheck client.Player Not Exit");
										gameClient.Disconnect();
									}
								}
							}
							else
							{
								Console.WriteLine(gameClient.Player.PlayerCharacter.NickName + " PingCheck client.IsConnected = false");
								gameClient.Disconnect();
							}
						}
					}
				}
				GameServer.log.Info("End ping check....");
			}
			catch (Exception exception)
			{
				if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("PingCheck callback", exception);
				}
			}
			try
			{
				GameServer.log.Info("Begin ping center check....");
				GameServer.Instance.LoginServer.SendPingCenter();
				GameServer.log.Info("End ping center check....");
			}
			catch (Exception exception2)
			{
				if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("PingCheck center callback", exception2);
				}
			}
		}
		protected void SaveTimerProc(object sender)
		{
			try
			{
				int num = Environment.TickCount;
				if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Saving database...");
					GameServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				int num2 = 0;
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer gamePlayer = array[i];
					gamePlayer.SaveIntoDatabase();
					num2++;
				}
				Thread.CurrentThread.Priority = priority;
				num = Environment.TickCount - num;
				if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Saving database complete!");
					GameServer.log.Info(string.Concat(new object[]
					{
						"Saved all databases and ",
						num2,
						" players in ",
						num,
						"ms"
					}));
				}
				if (num > 120000)
				{
					GameServer.log.WarnFormat("Saved all databases and {0} players in {1} ms", num2, num);
				}
			}
			catch (Exception exception)
			{
				if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("SaveTimerProc", exception);
				}
			}
			finally
			{
				GameEventMgr.Notify(GameServerEvent.WorldSave);
			}
		}
		protected void SaveRecordProc(object sender)
		{
			try
			{
				int num = Environment.TickCount;
				if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Saving Record...");
					GameServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				LogMgr.Save();
				Thread.CurrentThread.Priority = priority;
				num = Environment.TickCount - num;
				if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Saving Record complete!");
				}
				if (num > 120000)
				{
					GameServer.log.WarnFormat("Saved all Record  in {0} ms", num);
				}
			}
			catch (Exception exception)
			{
				if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("SaveRecordProc", exception);
				}
			}
		}
		protected void BuffScanTimerProc(object sender)
		{
			try
			{
				int num = Environment.TickCount;
				if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Buff Scaning ...");
					GameServer.log.Debug("BuffScan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				int num2 = 0;
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer gamePlayer = array[i];
					if (gamePlayer.BufferList != null)
					{
						gamePlayer.BufferList.Update();
						num2++;
					}
				}
				Thread.CurrentThread.Priority = priority;
				num = Environment.TickCount - num;
				if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Buff Scan complete!");
					GameServer.log.Info(string.Concat(new object[]
					{
						"Buff all ",
						num2,
						" players in ",
						num,
						"ms"
					}));
				}
				if (num > 120000)
				{
					GameServer.log.WarnFormat("Scan all Buff and {0} players in {1} ms", num2, num);
				}
			}
			catch (Exception exception)
			{
				if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("BuffScanTimerProc", exception);
				}
			}
		}
	}
}
