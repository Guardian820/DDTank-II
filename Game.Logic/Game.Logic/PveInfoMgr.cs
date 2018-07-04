using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public static class PveInfoMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, PveInfo> m_pveInfos = new Dictionary<int, PveInfo>();
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		private static ThreadSafeRandom m_rand = new ThreadSafeRandom();
		public static bool Init()
		{
			return PveInfoMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, PveInfo> dictionary = PveInfoMgr.LoadFromDatabase();
				if (dictionary.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, PveInfo>>(ref PveInfoMgr.m_pveInfos, dictionary);
				}
				return true;
			}
			catch (Exception exception)
			{
				PveInfoMgr.log.Error("PveInfoMgr", exception);
			}
			return false;
		}
		public static Dictionary<int, PveInfo> LoadFromDatabase()
		{
			Dictionary<int, PveInfo> dictionary = new Dictionary<int, PveInfo>();
			using (PveBussiness pveBussiness = new PveBussiness())
			{
				PveInfo[] allPveInfos = pveBussiness.GetAllPveInfos();
				PveInfo[] array = allPveInfos;
				for (int i = 0; i < array.Length; i++)
				{
					PveInfo pveInfo = array[i];
					if (!dictionary.ContainsKey(pveInfo.ID))
					{
						dictionary.Add(pveInfo.ID, pveInfo);
					}
				}
			}
			return dictionary;
		}
		public static PveInfo GetPveInfoById(int id)
		{
			if (PveInfoMgr.m_pveInfos.ContainsKey(id))
			{
				return PveInfoMgr.m_pveInfos[id];
			}
			return null;
		}
		public static PveInfo GetPveInfoByType(eRoomType roomType, int levelLimits)
		{
			if (roomType == eRoomType.Dungeon || roomType == eRoomType.Freshman || roomType == eRoomType.Lanbyrinth || roomType == eRoomType.ConsortiaBoss || roomType == eRoomType.AcademyDungeon || roomType == eRoomType.FightLib)
			{
				using (Dictionary<int, PveInfo>.ValueCollection.Enumerator enumerator = PveInfoMgr.m_pveInfos.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PveInfo current = enumerator.Current;
						if (current.Type == (int)roomType)
						{
							PveInfo result = current;
							return result;
						}
					}
					goto IL_AA;
				}
			}
			if (roomType == eRoomType.Exploration)
			{
				foreach (PveInfo current2 in PveInfoMgr.m_pveInfos.Values)
				{
					if (current2.Type == (int)roomType && current2.LevelLimits == levelLimits)
					{
						PveInfo result = current2;
						return result;
					}
				}
			}
			IL_AA:
			return null;
		}
	}
}
