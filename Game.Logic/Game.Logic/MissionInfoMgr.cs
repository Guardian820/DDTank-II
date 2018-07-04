using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public static class MissionInfoMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, MissionInfo> m_missionInfos = new Dictionary<int, MissionInfo>();
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		private static ThreadSafeRandom m_rand = new ThreadSafeRandom();
		public static bool Init()
		{
			return MissionInfoMgr.Reload();
		}
		public static bool Reload()
		{
			try
			{
				Dictionary<int, MissionInfo> dictionary = MissionInfoMgr.LoadFromDatabase();
				if (dictionary.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, MissionInfo>>(ref MissionInfoMgr.m_missionInfos, dictionary);
				}
				return true;
			}
			catch (Exception exception)
			{
				MissionInfoMgr.log.Error("MissionInfoMgr", exception);
			}
			return false;
		}
		private static Dictionary<int, MissionInfo> LoadFromDatabase()
		{
			Dictionary<int, MissionInfo> dictionary = new Dictionary<int, MissionInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				MissionInfo[] allMissionInfo = produceBussiness.GetAllMissionInfo();
				MissionInfo[] array = allMissionInfo;
				for (int i = 0; i < array.Length; i++)
				{
					MissionInfo missionInfo = array[i];
					if (!dictionary.ContainsKey(missionInfo.Id))
					{
						dictionary.Add(missionInfo.Id, missionInfo);
					}
				}
			}
			return dictionary;
		}
		public static MissionInfo GetMissionInfo(int id)
		{
			if (MissionInfoMgr.m_missionInfos.ContainsKey(id))
			{
				return MissionInfoMgr.m_missionInfos[id];
			}
			return null;
		}
	}
}
