using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class AchievementMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, AchievementInfo> m_achievement = new Dictionary<int, AchievementInfo>();
		private static Dictionary<int, List<AchievementConditionInfo>> m_achievementCondition = new Dictionary<int, List<AchievementConditionInfo>>();
		private static Dictionary<int, List<AchievementRewardInfo>> m_achievementReward = new Dictionary<int, List<AchievementRewardInfo>>();
		private static Hashtable m_distinctCondition = new Hashtable();
		private static Dictionary<int, List<ItemRecordTypeInfo>> m_itemRecordType = new Dictionary<int, List<ItemRecordTypeInfo>>();
		private static Hashtable m_ItemRecordTypeInfo = new Hashtable();
		private static Dictionary<int, List<int>> m_recordLimit = new Dictionary<int, List<int>>();
		public static Dictionary<int, AchievementInfo> Achievement
		{
			get
			{
				return AchievementMgr.m_achievement;
			}
		}
		public static Hashtable ItemRecordType
		{
			get
			{
				return AchievementMgr.m_ItemRecordTypeInfo;
			}
		}
		public static List<AchievementConditionInfo> GetAchievementCondition(AchievementInfo info)
		{
			if (AchievementMgr.m_achievementCondition.ContainsKey(info.ID))
			{
				return AchievementMgr.m_achievementCondition[info.ID];
			}
			return null;
		}
		public static List<AchievementRewardInfo> GetAchievementReward(AchievementInfo info)
		{
			if (AchievementMgr.m_achievementReward.ContainsKey(info.ID))
			{
				return AchievementMgr.m_achievementReward[info.ID];
			}
			return null;
		}
		public static int GetNextLimit(int recordType, int recordValue)
		{
			if (AchievementMgr.m_recordLimit.ContainsKey(recordType))
			{
				foreach (int current in AchievementMgr.m_recordLimit[recordType])
				{
					if (current > recordValue)
					{
						return current;
					}
				}
				return 2147483647;
			}
			return 2147483647;
		}
		public static AchievementInfo GetSingleAchievement(int id)
		{
			if (AchievementMgr.m_achievement.ContainsKey(id))
			{
				return AchievementMgr.m_achievement[id];
			}
			return null;
		}
		public static bool Init()
		{
			return AchievementMgr.Reload();
		}
		public static Dictionary<int, List<AchievementConditionInfo>> LoadAchievementConditionInfoDB(Dictionary<int, AchievementInfo> achievementInfos)
		{
			Dictionary<int, List<AchievementConditionInfo>> dictionary = new Dictionary<int, List<AchievementConditionInfo>>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				AchievementConditionInfo[] aLlAchievementCondition = produceBussiness.GetALlAchievementCondition();
				using (Dictionary<int, AchievementInfo>.ValueCollection.Enumerator enumerator = achievementInfos.Values.GetEnumerator())
				{
					Func<AchievementConditionInfo, bool> func = null;
					AchievementInfo achievementInfo;
					while (enumerator.MoveNext())
					{
						achievementInfo = enumerator.Current;
						if (func == null)
						{
							func = ((AchievementConditionInfo s) => s.AchievementID == achievementInfo.ID);
						}
						IEnumerable<AchievementConditionInfo> enumerable = aLlAchievementCondition.Where(func);
						dictionary.Add(achievementInfo.ID, enumerable.ToList<AchievementConditionInfo>());
						if (enumerable != null)
						{
							foreach (AchievementConditionInfo current in enumerable)
							{
								if (!AchievementMgr.m_distinctCondition.Contains(current.CondictionType))
								{
									AchievementMgr.m_distinctCondition.Add(current.CondictionType, current.CondictionType);
								}
							}
						}
					}
				}
				AchievementConditionInfo[] array = aLlAchievementCondition;
				for (int i = 0; i < array.Length; i++)
				{
					AchievementConditionInfo achievementConditionInfo = array[i];
					int condictionType = achievementConditionInfo.CondictionType;
					int condiction_Para = achievementConditionInfo.Condiction_Para2;
					if (!AchievementMgr.m_recordLimit.ContainsKey(condictionType))
					{
						AchievementMgr.m_recordLimit.Add(condictionType, new List<int>());
					}
					if (!AchievementMgr.m_recordLimit[condictionType].Contains(condiction_Para))
					{
						AchievementMgr.m_recordLimit[condictionType].Add(condiction_Para);
					}
				}
				foreach (int current2 in AchievementMgr.m_recordLimit.Keys)
				{
					AchievementMgr.m_recordLimit[current2].Sort();
				}
			}
			return dictionary;
		}
		public static Dictionary<int, AchievementInfo> LoadAchievementInfoDB()
		{
			Dictionary<int, AchievementInfo> dictionary = new Dictionary<int, AchievementInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				AchievementInfo[] aLlAchievement = produceBussiness.GetALlAchievement();
				AchievementInfo[] array = aLlAchievement;
				for (int i = 0; i < array.Length; i++)
				{
					AchievementInfo achievementInfo = array[i];
					if (!dictionary.ContainsKey(achievementInfo.ID))
					{
						dictionary.Add(achievementInfo.ID, achievementInfo);
					}
				}
			}
			return dictionary;
		}
		public static Dictionary<int, List<AchievementRewardInfo>> LoadAchievementRewardInfoDB(Dictionary<int, AchievementInfo> achievementInfos)
		{
			Dictionary<int, List<AchievementRewardInfo>> dictionary = new Dictionary<int, List<AchievementRewardInfo>>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				AchievementRewardInfo[] aLlAchievementReward = produceBussiness.GetALlAchievementReward();
				using (Dictionary<int, AchievementInfo>.ValueCollection.Enumerator enumerator = achievementInfos.Values.GetEnumerator())
				{
					Func<AchievementRewardInfo, bool> func = null;
					AchievementInfo achievementInfo;
					while (enumerator.MoveNext())
					{
						achievementInfo = enumerator.Current;
						if (func == null)
						{
							func = ((AchievementRewardInfo s) => s.AchievementID == achievementInfo.ID);
						}
						IEnumerable<AchievementRewardInfo> source = aLlAchievementReward.Where(func);
						dictionary.Add(achievementInfo.ID, source.ToList<AchievementRewardInfo>());
					}
				}
			}
			return dictionary;
		}
		public static void LoadItemRecordTypeInfoDB()
		{
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				ItemRecordTypeInfo[] allItemRecordType = produceBussiness.GetAllItemRecordType();
				ItemRecordTypeInfo[] array = allItemRecordType;
				for (int i = 0; i < array.Length; i++)
				{
					ItemRecordTypeInfo itemRecordTypeInfo = array[i];
					if (!AchievementMgr.m_ItemRecordTypeInfo.Contains(itemRecordTypeInfo.RecordID))
					{
						AchievementMgr.m_ItemRecordTypeInfo.Add(itemRecordTypeInfo.RecordID, itemRecordTypeInfo.Name);
					}
				}
			}
		}
		public static bool Reload()
		{
			try
			{
				AchievementMgr.LoadItemRecordTypeInfoDB();
				Dictionary<int, AchievementInfo> dictionary = AchievementMgr.LoadAchievementInfoDB();
				Dictionary<int, List<AchievementConditionInfo>> value = AchievementMgr.LoadAchievementConditionInfoDB(dictionary);
				Dictionary<int, List<AchievementRewardInfo>> value2 = AchievementMgr.LoadAchievementRewardInfoDB(dictionary);
				if (dictionary.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, AchievementInfo>>(ref AchievementMgr.m_achievement, dictionary);
					Interlocked.Exchange<Dictionary<int, List<AchievementConditionInfo>>>(ref AchievementMgr.m_achievementCondition, value);
					Interlocked.Exchange<Dictionary<int, List<AchievementRewardInfo>>>(ref AchievementMgr.m_achievementReward, value2);
				}
				return true;
			}
			catch (Exception exception)
			{
				AchievementMgr.log.Error("AchievementMgr", exception);
			}
			return false;
		}
	}
}
