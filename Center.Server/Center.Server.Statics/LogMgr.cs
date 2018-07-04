using Bussiness;
using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Threading;
namespace Center.Server.Statics
{
	public class LogMgr
	{
		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static object _syncStop = new object();
		private static int _gameType;
		private static int _serverId;
		private static int _areaId;
		public static DataTable m_LogServer;
		private static int regCount;
		public static object _sysObj = new object();
		public static int GameType
		{
			get
			{
				return int.Parse(ConfigurationManager.AppSettings["GameType"]);
			}
		}
		public static int ServerID
		{
			get
			{
				return int.Parse(ConfigurationManager.AppSettings["ServerID"]);
			}
		}
		public static int AreaID
		{
			get
			{
				return int.Parse(ConfigurationManager.AppSettings["AreaID"]);
			}
		}
		public static int SaveRecordSecond
		{
			get
			{
				return int.Parse(ConfigurationManager.AppSettings["SaveRecordInterval"]) * 60;
			}
		}
		public static int RegCount
		{
			get
			{
				object sysObj;
				Monitor.Enter(sysObj = LogMgr._sysObj);
				int result;
				try
				{
					result = LogMgr.regCount;
				}
				finally
				{
					Monitor.Exit(sysObj);
				}
				return result;
			}
			set
			{
				object sysObj;
				Monitor.Enter(sysObj = LogMgr._sysObj);
				try
				{
					LogMgr.regCount = value;
				}
				finally
				{
					Monitor.Exit(sysObj);
				}
			}
		}
		public static bool Setup()
		{
			return LogMgr.Setup(LogMgr.GameType, LogMgr.ServerID, LogMgr.AreaID);
		}
		public static bool Setup(int gametype, int serverid, int areaid)
		{
			LogMgr._gameType = gametype;
			LogMgr._serverId = serverid;
			LogMgr._areaId = areaid;
			LogMgr.m_LogServer = new DataTable("Log_Server");
			LogMgr.m_LogServer.Columns.Add("ApplicationId", typeof(int));
			LogMgr.m_LogServer.Columns.Add("SubId", typeof(int));
			LogMgr.m_LogServer.Columns.Add("EnterTime", typeof(DateTime));
			LogMgr.m_LogServer.Columns.Add("Online", typeof(int));
			LogMgr.m_LogServer.Columns.Add("Reg", typeof(int));
			return true;
		}
		public static void Reset()
		{
			DataTable logServer;
			Monitor.Enter(logServer = LogMgr.m_LogServer);
			try
			{
				LogMgr.m_LogServer.Clear();
			}
			finally
			{
				Monitor.Exit(logServer);
			}
		}
		public static void Save()
		{
			int onlineCount = LoginMgr.GetOnlineCount();
			int arg_0B_0 = LogMgr._gameType;
			int arg_11_0 = LogMgr._serverId;
			DateTime arg_17_0 = DateTime.Now;
			int arg_1D_0 = LogMgr.RegCount;
			LogMgr.RegCount = 0;
			int arg_29_0 = LogMgr.SaveRecordSecond;
			using (ItemRecordBussiness itemRecordBussiness = new ItemRecordBussiness())
			{
				itemRecordBussiness.LogServerDb(LogMgr.m_LogServer);
			}
		}
		public static void AddRegCount()
		{
			object sysObj;
			Monitor.Enter(sysObj = LogMgr._sysObj);
			try
			{
				LogMgr.regCount++;
			}
			finally
			{
				Monitor.Exit(sysObj);
			}
		}
	}
}
