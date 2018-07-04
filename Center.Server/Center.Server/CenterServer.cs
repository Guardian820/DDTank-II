using Bussiness;
using Bussiness.Protocol;
using Center.Server.Managers;
using Center.Server.Statics;
using Game.Base;
using Game.Base.Events;
using Game.Base.Packets;
using Game.Server.Managers;
using log4net;
using log4net.Config;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
namespace Center.Server
{
	public class CenterServer : BaseServer
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private CenterServerConfig _config;
		private string Edition = "5498628";
		private bool _aSSState;
		private bool _dailyAwardState;
		private Timer m_loginLapseTimer;
		private Timer m_saveDBTimer;
		private Timer m_saveRecordTimer;
		private Timer m_scanAuction;
		private Timer m_scanMail;
		private Timer m_scanConsortia;
		private static CenterServer _instance;
		public bool ASSState
		{
			get
			{
				return this._aSSState;
			}
			set
			{
				this._aSSState = value;
			}
		}
		public bool DailyAwardState
		{
			get
			{
				return this._dailyAwardState;
			}
			set
			{
				this._dailyAwardState = value;
			}
		}
		public static CenterServer Instance
		{
			get
			{
				return CenterServer._instance;
			}
		}
		protected override BaseClient GetNewClient()
		{
			return new ServerClient(this);
		}
		public override bool Start()
		{
			bool result;
			try
			{
				Thread.CurrentThread.Priority = ThreadPriority.Normal;
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
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
						if (!this.InitComponent(GameProperties.EDITION == this.Edition, "Check Server Edition:" + this.Edition))
						{
							result = false;
						}
						else
						{
							if (!this.InitComponent(this.InitSocket(IPAddress.Parse(this._config.Ip), this._config.Port), "InitSocket Port:" + this._config.Port))
							{
								result = false;
							}
							else
							{
								if (!this.InitComponent(CenterService.Start(), "Center Service"))
								{
									result = false;
								}
								else
								{
									if (!this.InitComponent(ServerMgr.Start(), "Load serverlist"))
									{
										result = false;
									}
									else
									{
										if (!this.InitComponent(ConsortiaLevelMgr.Init(), "Init ConsortiaLevelMgr"))
										{
											result = false;
										}
										else
										{
											if (!this.InitComponent(MacroDropMgr.Init(), "Init MacroDropMgr"))
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
													if (!this.InitComponent(this.InitGlobalTimers(), "Init Global Timers"))
													{
														result = false;
													}
													else
													{
														GameEventMgr.Notify(ScriptEvent.Loaded);
														MacroDropMgr.Start();
														if (!this.InitComponent(base.Start(), "base.Start()"))
														{
															result = false;
														}
														else
														{
															GameEventMgr.Notify(GameServerEvent.Started, this);
															GC.Collect(GC.MaxGeneration);
															CenterServer.log.Info("GameServer is now open for connections!");
															GameProperties.Save();
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
			catch (Exception exception)
			{
				CenterServer.log.Error("Failed to start the server", exception);
				result = false;
			}
			return result;
		}
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			CenterServer.log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
			if (e.IsTerminating)
			{
				LogManager.Shutdown();
			}
		}
		protected bool InitComponent(bool componentInitState, string text)
		{
			CenterServer.log.Info(text + ": " + componentInitState);
			if (!componentInitState)
			{
				this.Stop();
			}
			return componentInitState;
		}
		public bool RecompileScripts()
		{
			string path = this._config.RootDirectory + Path.DirectorySeparatorChar + "scripts";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string[] asm_names = this._config.ScriptAssemblies.Split(new char[]
			{
				','
			});
			return ScriptMgr.CompileScripts(false, path, this._config.ScriptCompilationTarget, asm_names);
		}
		protected bool StartScriptComponents()
		{
			bool result;
			try
			{
				ScriptMgr.InsertAssembly(typeof(CenterServer).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
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
				CenterServer.log.Info("Registering global event handlers: true");
				result = true;
			}
			catch (Exception exception)
			{
				CenterServer.log.Error("StartScriptComponents", exception);
				result = false;
			}
			return result;
		}
		public bool InitGlobalTimers()
		{
			int num = this._config.SaveIntervalInterval * 60 * 1000;
			if (this.m_saveDBTimer == null)
			{
				this.m_saveDBTimer = new Timer(new TimerCallback(this.SaveTimerProc), null, num, num);
			}
			else
			{
				this.m_saveDBTimer.Change(num, num);
			}
			num = this._config.LoginLapseInterval * 60 * 1000;
			if (this.m_loginLapseTimer == null)
			{
				this.m_loginLapseTimer = new Timer(new TimerCallback(this.LoginLapseTimerProc), null, num, num);
			}
			else
			{
				this.m_loginLapseTimer.Change(num, num);
			}
			num = this._config.SaveRecordInterval * 60 * 1000;
			if (this.m_saveRecordTimer == null)
			{
				this.m_saveRecordTimer = new Timer(new TimerCallback(this.SaveRecordProc), null, num, num);
			}
			else
			{
				this.m_saveRecordTimer.Change(num, num);
			}
			num = this._config.ScanAuctionInterval * 60 * 1000;
			if (this.m_scanAuction == null)
			{
				this.m_scanAuction = new Timer(new TimerCallback(this.ScanAuctionProc), null, num, num);
			}
			else
			{
				this.m_scanAuction.Change(num, num);
			}
			num = this._config.ScanMailInterval * 60 * 1000;
			if (this.m_scanMail == null)
			{
				this.m_scanMail = new Timer(new TimerCallback(this.ScanMailProc), null, num, num);
			}
			else
			{
				this.m_scanMail.Change(num, num);
			}
			num = this._config.ScanConsortiaInterval * 60 * 1000;
			if (this.m_scanConsortia == null)
			{
				this.m_scanConsortia = new Timer(new TimerCallback(this.ScanConsortiaProc), null, num, num);
			}
			else
			{
				this.m_scanConsortia.Change(num, num);
			}
			return true;
		}
		public void DisposeGlobalTimers()
		{
			if (this.m_saveDBTimer != null)
			{
				this.m_saveDBTimer.Dispose();
			}
			if (this.m_loginLapseTimer != null)
			{
				this.m_loginLapseTimer.Dispose();
			}
			if (this.m_saveRecordTimer != null)
			{
				this.m_saveRecordTimer.Dispose();
			}
			if (this.m_scanAuction != null)
			{
				this.m_scanAuction.Dispose();
			}
			if (this.m_scanMail != null)
			{
				this.m_scanMail.Dispose();
			}
			if (this.m_scanConsortia != null)
			{
				this.m_scanConsortia.Dispose();
			}
		}
		protected void SaveTimerProc(object state)
		{
			try
			{
				int num = Environment.TickCount;
				if (CenterServer.log.IsInfoEnabled)
				{
					CenterServer.log.Info("Saving database...");
					CenterServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				ServerMgr.SaveToDatabase();
				Thread.CurrentThread.Priority = priority;
				num = Environment.TickCount - num;
				if (CenterServer.log.IsInfoEnabled)
				{
					CenterServer.log.Info("Saving database complete!");
					CenterServer.log.Info("Saved all databases " + num + "ms");
				}
			}
			catch (Exception exception)
			{
				if (CenterServer.log.IsErrorEnabled)
				{
					CenterServer.log.Error("SaveTimerProc", exception);
				}
			}
		}
		protected void LoginLapseTimerProc(object sender)
		{
			try
			{
				Player[] allPlayer = LoginMgr.GetAllPlayer();
				long ticks = DateTime.Now.Ticks;
				long num = (long)this._config.LoginLapseInterval * 10L * 1000L;
				Player[] array = allPlayer;
				for (int i = 0; i < array.Length; i++)
				{
					Player player = array[i];
					if (player.State == ePlayerState.NotLogin)
					{
						if (player.LastTime + num < ticks)
						{
							LoginMgr.RemovePlayer(player.Id);
						}
					}
					else
					{
						player.LastTime = ticks;
					}
				}
			}
			catch (Exception exception)
			{
				if (CenterServer.log.IsErrorEnabled)
				{
					CenterServer.log.Error("LoginLapseTimer callback", exception);
				}
			}
		}
		protected void SaveRecordProc(object sender)
		{
			try
			{
				int num = Environment.TickCount;
				if (CenterServer.log.IsInfoEnabled)
				{
					CenterServer.log.Info("Saving Record...");
					CenterServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				LogMgr.Save();
				Thread.CurrentThread.Priority = priority;
				num = Environment.TickCount - num;
				if (CenterServer.log.IsInfoEnabled)
				{
					CenterServer.log.Info("Saving Record complete!");
				}
				if (num > 120000)
				{
					CenterServer.log.WarnFormat("Saved all Record  in {0} ms!", num);
				}
			}
			catch (Exception exception)
			{
				if (CenterServer.log.IsErrorEnabled)
				{
					CenterServer.log.Error("SaveRecordProc", exception);
				}
			}
		}
		protected void ScanAuctionProc(object sender)
		{
			try
			{
				int num = Environment.TickCount;
				if (CenterServer.log.IsInfoEnabled)
				{
					CenterServer.log.Info("Saving Record...");
					CenterServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				string text = "";
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					playerBussiness.ScanAuction(ref text);
				}
				string[] array = text.Split(new char[]
				{
					','
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i];
					if (!string.IsNullOrEmpty(text2))
					{
						GSPacketIn gSPacketIn = new GSPacketIn(117);
						gSPacketIn.WriteInt(int.Parse(text2));
						gSPacketIn.WriteInt(1);
						this.SendToALL(gSPacketIn);
					}
				}
				Thread.CurrentThread.Priority = priority;
				num = Environment.TickCount - num;
				if (CenterServer.log.IsInfoEnabled)
				{
					CenterServer.log.Info("Scan Auction complete!");
				}
				if (num > 120000)
				{
					CenterServer.log.WarnFormat("Scan all Auction  in {0} ms", num);
				}
			}
			catch (Exception exception)
			{
				if (CenterServer.log.IsErrorEnabled)
				{
					CenterServer.log.Error("ScanAuctionProc", exception);
				}
			}
		}
		protected void ScanMailProc(object sender)
		{
			try
			{
				int num = Environment.TickCount;
				if (CenterServer.log.IsInfoEnabled)
				{
					CenterServer.log.Info("Saving Record...");
					CenterServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				string text = "";
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					playerBussiness.ScanMail(ref text);
				}
				string[] array = text.Split(new char[]
				{
					','
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i];
					if (!string.IsNullOrEmpty(text2))
					{
						GSPacketIn gSPacketIn = new GSPacketIn(117);
						gSPacketIn.WriteInt(int.Parse(text2));
						gSPacketIn.WriteInt(1);
						this.SendToALL(gSPacketIn);
					}
				}
				Thread.CurrentThread.Priority = priority;
				num = Environment.TickCount - num;
				if (CenterServer.log.IsInfoEnabled)
				{
					CenterServer.log.Info("Scan Mail complete!");
				}
				if (num > 120000)
				{
					CenterServer.log.WarnFormat("Scan all Mail in {0} ms", num);
				}
			}
			catch (Exception exception)
			{
				if (CenterServer.log.IsErrorEnabled)
				{
					CenterServer.log.Error("ScanMailProc", exception);
				}
			}
		}
		protected void ScanConsortiaProc(object sender)
		{
			try
			{
				int num = Environment.TickCount;
				if (CenterServer.log.IsInfoEnabled)
				{
					CenterServer.log.Info("Saving Record...");
					CenterServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				string text = "";
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					consortiaBussiness.ScanConsortia(ref text);
				}
				string[] array = text.Split(new char[]
				{
					','
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i];
					if (!string.IsNullOrEmpty(text2))
					{
						GSPacketIn gSPacketIn = new GSPacketIn(128);
						gSPacketIn.WriteByte(2);
						gSPacketIn.WriteInt(int.Parse(text2));
						this.SendToALL(gSPacketIn);
					}
				}
				Thread.CurrentThread.Priority = priority;
				num = Environment.TickCount - num;
				if (CenterServer.log.IsInfoEnabled)
				{
					CenterServer.log.Info("Scan Consortia complete!");
				}
				if (num > 120000)
				{
					CenterServer.log.WarnFormat("Scan all Consortia in {0} ms", num);
				}
			}
			catch (Exception exception)
			{
				if (CenterServer.log.IsErrorEnabled)
				{
					CenterServer.log.Error("ScanConsortiaProc", exception);
				}
			}
		}
		public override void Stop()
		{
			this.DisposeGlobalTimers();
			this.SaveTimerProc(null);
			this.SaveRecordProc(null);
			CenterService.Stop();
			base.Stop();
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
		public void SendConsortiaDelete(int consortiaID)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(5);
			gSPacketIn.WriteInt(consortiaID);
			this.SendToALL(gSPacketIn);
		}
		public void SendSystemNotice(string msg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(10);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteString(msg);
			this.SendToALL(gSPacketIn, null);
		}
		public bool SendAAS(bool state)
		{
			if (StaticFunction.UpdateConfig("Center.Service.exe.config", "AAS", state.ToString()))
			{
				this.ASSState = state;
				GSPacketIn gSPacketIn = new GSPacketIn(7);
				gSPacketIn.WriteBoolean(state);
				this.SendToALL(gSPacketIn);
				return true;
			}
			return false;
		}
		public bool SendConfigState(int type, bool state)
		{
			string name = string.Empty;
			switch (type)
			{
			case 1:
				name = "AAS";
				break;

			case 2:
				name = "DailyAwardState";
				break;

			default:
				return false;
			}
			if (StaticFunction.UpdateConfig("Center.Service.exe.config", name, state.ToString()))
			{
				switch (type)
				{
				case 1:
					this.ASSState = state;
					break;

				case 2:
					this.DailyAwardState = state;
					break;
				}
				this.SendConfigState();
				return true;
			}
			return false;
		}
		public void SendConfigState()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(8);
			gSPacketIn.WriteBoolean(this.ASSState);
			gSPacketIn.WriteBoolean(this.DailyAwardState);
			this.SendToALL(gSPacketIn);
		}
		public int RateUpdate(int serverId)
		{
			ServerClient[] allClients = this.GetAllClients();
			if (allClients != null)
			{
				ServerClient[] array = allClients;
				for (int i = 0; i < array.Length; i++)
				{
					ServerClient serverClient = array[i];
					if (serverClient.Info.ID == serverId)
					{
						GSPacketIn gSPacketIn = new GSPacketIn(177);
						gSPacketIn.WriteInt(serverId);
						serverClient.SendTCP(gSPacketIn);
						return 0;
					}
				}
			}
			return 1;
		}
		public int NoticeServerUpdate(int serverId, int type)
		{
			ServerClient[] allClients = this.GetAllClients();
			if (allClients != null)
			{
				ServerClient[] array = allClients;
				for (int i = 0; i < array.Length; i++)
				{
					ServerClient serverClient = array[i];
					if (serverClient.Info.ID == serverId)
					{
						GSPacketIn gSPacketIn = new GSPacketIn(11);
						gSPacketIn.WriteInt(type);
						serverClient.SendTCP(gSPacketIn);
						return 0;
					}
				}
			}
			return 1;
		}
		public bool SendReload(eReloadType type)
		{
			return this.SendReload(type.ToString());
		}
		public bool SendReload(string str)
		{
			try
			{
				eReloadType eReloadType = (eReloadType)Enum.Parse(typeof(eReloadType), str, true);
				eReloadType eReloadType2 = eReloadType;
				if (eReloadType2 == eReloadType.server)
				{
					this._config.Refresh();
					this.InitGlobalTimers();
					this.LoadConfig();
					ServerMgr.ReLoadServerList();
					this.SendConfigState();
				}
				GSPacketIn gSPacketIn = new GSPacketIn(11);
				gSPacketIn.WriteInt((int)eReloadType);
				this.SendToALL(gSPacketIn, null);
				return true;
			}
			catch (Exception exception)
			{
				CenterServer.log.Error("Order is not Exist!", exception);
			}
			return false;
		}
		public void SendShutdown()
		{
			GSPacketIn pkg = new GSPacketIn(15);
			this.SendToALL(pkg);
		}
		public CenterServer(CenterServerConfig config)
		{
			this._config = config;
			this.LoadConfig();
		}
		public void LoadConfig()
		{
			this._aSSState = bool.Parse(ConfigurationManager.AppSettings["AAS"]);
			this._dailyAwardState = bool.Parse(ConfigurationManager.AppSettings["DailyAwardState"]);
		}
		public static void CreateInstance(CenterServerConfig config)
		{
			if (CenterServer.Instance != null)
			{
				return;
			}
			FileInfo fileInfo = new FileInfo(config.LogConfigFile);
			if (!fileInfo.Exists)
			{
				ResourceUtil.ExtractResource(fileInfo.Name, fileInfo.FullName, Assembly.GetAssembly(typeof(CenterServer)));
			}
			XmlConfigurator.ConfigureAndWatch(fileInfo);
			CenterServer._instance = new CenterServer(config);
		}
	}
}
