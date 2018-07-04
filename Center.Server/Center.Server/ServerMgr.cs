using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Center.Server
{
	public class ServerMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, ServerInfo> _list = new Dictionary<int, ServerInfo>();
		private static object _syncStop = new object();
		public static ServerInfo[] Servers
		{
			get
			{
				return ServerMgr._list.Values.ToArray<ServerInfo>();
			}
		}
		public static bool Start()
		{
			bool result;
			try
			{
				using (ServiceBussiness serviceBussiness = new ServiceBussiness())
				{
					ServerInfo[] serverList = serviceBussiness.GetServerList();
					ServerInfo[] array = serverList;
					for (int i = 0; i < array.Length; i++)
					{
						ServerInfo serverInfo = array[i];
						serverInfo.State = 1;
						serverInfo.Online = 0;
						ServerMgr._list.Add(serverInfo.ID, serverInfo);
					}
				}
				ServerMgr.log.Info("Load server list from db.");
				result = true;
			}
			catch (Exception arg)
			{
				ServerMgr.log.ErrorFormat("Load server list from db failed:{0}", arg);
				result = false;
			}
			return result;
		}
		public static bool ReLoadServerList()
		{
			bool result;
			try
			{
				using (ServiceBussiness serviceBussiness = new ServiceBussiness())
				{
					object syncStop;
					Monitor.Enter(syncStop = ServerMgr._syncStop);
					try
					{
						ServerInfo[] serverList = serviceBussiness.GetServerList();
						ServerInfo[] array = serverList;
						for (int i = 0; i < array.Length; i++)
						{
							ServerInfo serverInfo = array[i];
							if (ServerMgr._list.ContainsKey(serverInfo.ID))
							{
								ServerMgr._list[serverInfo.ID].IP = serverInfo.IP;
								ServerMgr._list[serverInfo.ID].Name = serverInfo.Name;
								ServerMgr._list[serverInfo.ID].Port = serverInfo.Port;
								ServerMgr._list[serverInfo.ID].Room = serverInfo.Room;
								ServerMgr._list[serverInfo.ID].Total = serverInfo.Total;
								ServerMgr._list[serverInfo.ID].MustLevel = serverInfo.MustLevel;
								ServerMgr._list[serverInfo.ID].LowestLevel = serverInfo.LowestLevel;
								ServerMgr._list[serverInfo.ID].Online = serverInfo.Online;
								ServerMgr._list[serverInfo.ID].State = serverInfo.State;
							}
							else
							{
								serverInfo.State = 1;
								serverInfo.Online = 0;
								ServerMgr._list.Add(serverInfo.ID, serverInfo);
							}
						}
					}
					finally
					{
						Monitor.Exit(syncStop);
					}
				}
				ServerMgr.log.Info("ReLoad server list from db.");
				result = true;
			}
			catch (Exception arg)
			{
				ServerMgr.log.ErrorFormat("ReLoad server list from db failed:{0}", arg);
				result = false;
			}
			return result;
		}
		public static ServerInfo GetServerInfo(int id)
		{
			if (ServerMgr._list.ContainsKey(id))
			{
				return ServerMgr._list[id];
			}
			return null;
		}
		public static int GetState(int count, int total)
		{
			if (count >= total)
			{
				return 5;
			}
			if ((double)count > (double)total * 0.5)
			{
				return 4;
			}
			return 2;
		}
		public static void SaveToDatabase()
		{
			try
			{
				using (ServiceBussiness serviceBussiness = new ServiceBussiness())
				{
					foreach (ServerInfo current in ServerMgr._list.Values)
					{
						serviceBussiness.UpdateService(current);
					}
				}
			}
			catch (Exception exception)
			{
				ServerMgr.log.Error("Save server state", exception);
			}
		}
	}
}
