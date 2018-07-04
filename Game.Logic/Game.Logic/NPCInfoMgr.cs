using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public static class NPCInfoMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, NpcInfo> m_npcs = new Dictionary<int, NpcInfo>();
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		private static ThreadSafeRandom m_rand = new ThreadSafeRandom();
		public static bool Init()
		{
			return NPCInfoMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, NpcInfo> dictionary = NPCInfoMgr.LoadFromDatabase();
				if (dictionary != null && dictionary.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, NpcInfo>>(ref NPCInfoMgr.m_npcs, dictionary);
				}
				return true;
			}
			catch (Exception exception)
			{
				NPCInfoMgr.log.Error("NPCInfoMgr", exception);
			}
			return false;
		}
		private static Dictionary<int, NpcInfo> LoadFromDatabase()
		{
			Dictionary<int, NpcInfo> dictionary = new Dictionary<int, NpcInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				NpcInfo[] allNPCInfo = produceBussiness.GetAllNPCInfo();
				NpcInfo[] array = allNPCInfo;
				for (int i = 0; i < array.Length; i++)
				{
					NpcInfo npcInfo = array[i];
					if (!dictionary.ContainsKey(npcInfo.ID))
					{
						dictionary.Add(npcInfo.ID, npcInfo);
					}
				}
			}
			return dictionary;
		}
		public static NpcInfo GetNpcInfoById(int id)
		{
			if (NPCInfoMgr.m_npcs.ContainsKey(id))
			{
				return NPCInfoMgr.m_npcs[id];
			}
			return null;
		}
	}
}
