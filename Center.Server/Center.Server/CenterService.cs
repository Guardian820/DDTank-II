using Center.Server.Statics;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
namespace Center.Server
{
	public class CenterService : ICenterService
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ServiceHost host;
		public List<ServerData> GetServerList()
		{
			ServerInfo[] servers = ServerMgr.Servers;
			List<ServerData> list = new List<ServerData>();
			ServerInfo[] array = servers;
			for (int i = 0; i < array.Length; i++)
			{
				ServerInfo serverInfo = array[i];
				list.Add(new ServerData
				{
					Id = serverInfo.ID,
					Name = serverInfo.Name,
					Ip = serverInfo.IP,
					Port = serverInfo.Port,
					State = serverInfo.State,
					MustLevel = serverInfo.MustLevel,
					LowestLevel = serverInfo.LowestLevel,
					Online = serverInfo.Online
				});
			}
			return list;
		}
		public bool ChargeMoney(int userID, string chargeID)
		{
			ServerClient serverClient = LoginMgr.GetServerClient(userID);
			if (serverClient != null)
			{
				serverClient.SendChargeMoney(userID, chargeID);
				return true;
			}
			return false;
		}
		public bool SystemNotice(string msg)
		{
			bool result;
			try
			{
				CenterServer.Instance.SendSystemNotice(msg);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}
		public bool KitoffUser(int playerID, string msg)
		{
			try
			{
				ServerClient serverClient = LoginMgr.GetServerClient(playerID);
				if (serverClient != null)
				{
					msg = (string.IsNullOrEmpty(msg) ? "You are kicking out by GM!" : msg);
					serverClient.SendKitoffUser(playerID, msg);
					LoginMgr.RemovePlayer(playerID);
					return true;
				}
			}
			catch
			{
			}
			return false;
		}
		public bool ReLoadServerList()
		{
			return ServerMgr.ReLoadServerList();
		}
		public bool MailNotice(int playerID)
		{
			try
			{
				ServerClient serverClient = LoginMgr.GetServerClient(playerID);
				if (serverClient != null)
				{
					GSPacketIn gSPacketIn = new GSPacketIn(117);
					gSPacketIn.WriteInt(playerID);
					gSPacketIn.WriteInt(1);
					serverClient.SendTCP(gSPacketIn);
					return true;
				}
			}
			catch
			{
			}
			return false;
		}
		public bool AASUpdateState(bool state)
		{
			try
			{
				return CenterServer.Instance.SendAAS(state);
			}
			catch
			{
			}
			return false;
		}
		public int AASGetState()
		{
			try
			{
				return CenterServer.Instance.ASSState ? 1 : 0;
			}
			catch
			{
			}
			return 2;
		}
		public int ExperienceRateUpdate(int serverId)
		{
			try
			{
				return CenterServer.Instance.RateUpdate(serverId);
			}
			catch
			{
			}
			return 2;
		}
		public int NoticeServerUpdate(int serverId, int type)
		{
			try
			{
				return CenterServer.Instance.NoticeServerUpdate(serverId, type);
			}
			catch
			{
			}
			return 2;
		}
		public bool UpdateConfigState(int type, bool state)
		{
			try
			{
				return CenterServer.Instance.SendConfigState(type, state);
			}
			catch
			{
			}
			return false;
		}
		public int GetConfigState(int type)
		{
			try
			{
				switch (type)
				{
				case 1:
					{
						int result = CenterServer.Instance.ASSState ? 1 : 0;
						return result;
					}

				case 2:
					{
						int result = CenterServer.Instance.DailyAwardState ? 1 : 0;
						return result;
					}
				}
			}
			catch
			{
			}
			return 2;
		}
		public bool Reload(string type)
		{
			try
			{
				return CenterServer.Instance.SendReload(type);
			}
			catch
			{
			}
			return false;
		}
		public bool ActivePlayer(bool isActive)
		{
			try
			{
				if (isActive)
				{
					LogMgr.AddRegCount();
					return true;
				}
			}
			catch
			{
			}
			return false;
		}
		public bool CreatePlayer(int id, string name, string password, bool isFirst)
		{
			try
			{
				LoginMgr.CreatePlayer(new Player
				{
					Id = id,
					Name = name,
					Password = password,
					IsFirst = isFirst
				});
				return true;
			}
			catch
			{
			}
			return false;
		}
		public bool ValidateLoginAndGetID(string name, string password, ref int userID, ref bool isFirst)
		{
			try
			{
				Player[] allPlayer = LoginMgr.GetAllPlayer();
				if (allPlayer != null)
				{
					Player[] array = allPlayer;
					for (int i = 0; i < array.Length; i++)
					{
						Player player = array[i];
						if (player.Name == name && player.Password == password)
						{
							userID = player.Id;
							isFirst = player.IsFirst;
							return true;
						}
					}
				}
			}
			catch
			{
			}
			return false;
		}
		public static bool Start()
		{
			bool result;
			try
			{
				CenterService.host = new ServiceHost(typeof(CenterService), new Uri[0]);
				CenterService.host.Open();
				CenterService.log.Info("Center Service started!");
				result = true;
			}
			catch (Exception arg)
			{
				CenterService.log.ErrorFormat("Start center server failed:{0}", arg);
				result = false;
			}
			return result;
		}
		public static void Stop()
		{
			try
			{
				if (CenterService.host != null)
				{
					CenterService.host.Close();
					CenterService.host = null;
				}
			}
			catch
			{
			}
		}
	}
}
