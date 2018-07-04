using Game.Server.Rooms;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
namespace Game.Server.Battle
{
	public class BattleMgr
	{
		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static List<BattleServer> m_list = new List<BattleServer>();
		public static bool Setup()
		{
			if (File.Exists("battle.xml"))
			{
				try
				{
					XDocument xDocument = XDocument.Load("battle.xml");
					using (IEnumerator<XNode> enumerator = xDocument.Root.Nodes().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							XElement xElement = (XElement)enumerator.Current;
							try
							{
								int serverId = int.Parse(xElement.Attribute("id").Value);
								string value = xElement.Attribute("ip").Value;
								int num = int.Parse(xElement.Attribute("port").Value);
								string value2 = xElement.Attribute("key").Value;
								BattleMgr.m_list.Add(new BattleServer(serverId, value, num, value2));
								BattleMgr.log.InfoFormat("Battle server {0}:{1} loaded...", value, num);
							}
							catch (Exception exception)
							{
								BattleMgr.log.Error("BattleMgr setup error:", exception);
							}
						}
					}
				}
				catch (Exception exception2)
				{
					BattleMgr.log.Error("BattleMgr setup error:", exception2);
				}
			}
			BattleMgr.log.InfoFormat("Total {0} battle server loaded.", BattleMgr.m_list.Count);
			return true;
		}
		public static void Start()
		{
			foreach (BattleServer current in BattleMgr.m_list)
			{
				try
				{
					current.Start();
				}
				catch (Exception ex)
				{
					BattleMgr.log.ErrorFormat("Batter server {0}:{1} can't connected!", current.Ip, current.Port);
					BattleMgr.log.Error(ex.Message);
				}
			}
		}
		public static BattleServer FindActiveServer()
		{
			List<BattleServer> list;
			Monitor.Enter(list = BattleMgr.m_list);
			try
			{
				foreach (BattleServer current in BattleMgr.m_list)
				{
					if (current.IsActive)
					{
						return current;
					}
				}
			}
			finally
			{
				Monitor.Exit(list);
			}
			return null;
		}
		public static BattleServer AddRoom(BaseRoom room)
		{
			BattleServer battleServer = BattleMgr.FindActiveServer();
			if (battleServer != null && battleServer.AddRoom(room))
			{
				return battleServer;
			}
			return null;
		}
		public static List<BattleServer> GetAllBattles()
		{
			return BattleMgr.m_list;
		}
	}
}
