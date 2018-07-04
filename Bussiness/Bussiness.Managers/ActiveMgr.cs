using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class ActiveMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static Dictionary<int, ActiveAwardInfo> m_ActiveAwardInfo = new Dictionary<int, ActiveAwardInfo>();
		public static Dictionary<int, List<ActiveConditionInfo>> m_ActiveConditionInfo = new Dictionary<int, List<ActiveConditionInfo>>();
		public static List<ActiveAwardInfo> GetAwardInfo(DateTime lastDate, int playerGrade)
		{
			string text = null;
			int num = (DateTime.Now - lastDate).Days;
			if (DateTime.Now.DayOfYear > lastDate.DayOfYear)
			{
				num++;
			}
			List<ActiveAwardInfo> list = new List<ActiveAwardInfo>();
			foreach (List<ActiveConditionInfo> current in ActiveMgr.m_ActiveConditionInfo.Values)
			{
				foreach (ActiveConditionInfo current2 in current)
				{
					if (ActiveMgr.IsValid(current2) && ActiveMgr.IsInGrade(current2.LimitGrade, playerGrade) && current2.Condition <= num)
					{
						text = current2.AwardId;
						int arg_98_0 = current2.ActiveID;
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					','
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string value = array2[i];
					if (!string.IsNullOrEmpty(value) && ActiveMgr.m_ActiveAwardInfo.ContainsKey(Convert.ToInt32(value)))
					{
						list.Add(ActiveMgr.m_ActiveAwardInfo[Convert.ToInt32(value)]);
					}
				}
			}
			return list;
		}
		public static bool Init()
		{
			return ActiveMgr.ReLoad();
		}
		private static bool IsInGrade(string limitGrade, int playerGrade)
		{
			bool result = false;
			int num = 0;
			int num2 = 0;
			if (limitGrade != null)
			{
				string[] array = limitGrade.Split(new char[]
				{
					'-'
				});
				if (array.Length == 2)
				{
					num = Convert.ToInt32(array[0]);
					num2 = Convert.ToInt32(array[1]);
				}
				if (num <= playerGrade && num2 >= playerGrade)
				{
					result = true;
				}
			}
			return result;
		}
		public static bool IsValid(ActiveConditionInfo info)
		{
			DateTime arg_06_0 = info.StartTime;
			DateTime arg_0D_0 = info.EndTime;
			return info.StartTime.Ticks <= DateTime.Now.Ticks && info.EndTime.Ticks >= DateTime.Now.Ticks;
		}
		public static Dictionary<int, ActiveAwardInfo> LoadActiveAwardDb(Dictionary<int, List<ActiveConditionInfo>> conditions)
		{
			Dictionary<int, ActiveAwardInfo> dictionary = new Dictionary<int, ActiveAwardInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				ActiveAwardInfo[] allActiveAwardInfo = produceBussiness.GetAllActiveAwardInfo();
				foreach (int current in conditions.Keys)
				{
					ActiveAwardInfo[] array = allActiveAwardInfo;
					for (int i = 0; i < array.Length; i++)
					{
						ActiveAwardInfo activeAwardInfo = array[i];
						if (current == activeAwardInfo.ActiveID && !dictionary.ContainsKey(activeAwardInfo.ID))
						{
							dictionary.Add(activeAwardInfo.ID, activeAwardInfo);
						}
					}
				}
			}
			return dictionary;
		}
		public static Dictionary<int, List<ActiveConditionInfo>> LoadActiveConditionDb()
		{
			Dictionary<int, List<ActiveConditionInfo>> dictionary = new Dictionary<int, List<ActiveConditionInfo>>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				ActiveConditionInfo[] allActiveConditionInfo = produceBussiness.GetAllActiveConditionInfo();
				ActiveConditionInfo[] array = allActiveConditionInfo;
				for (int i = 0; i < array.Length; i++)
				{
					ActiveConditionInfo activeConditionInfo = array[i];
					List<ActiveConditionInfo> list = new List<ActiveConditionInfo>();
					if (!dictionary.ContainsKey(activeConditionInfo.ActiveID))
					{
						list.Add(activeConditionInfo);
						dictionary.Add(activeConditionInfo.ActiveID, list);
					}
					else
					{
						dictionary[activeConditionInfo.ActiveID].Add(activeConditionInfo);
					}
				}
			}
			return dictionary;
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, List<ActiveConditionInfo>> dictionary = ActiveMgr.LoadActiveConditionDb();
				Dictionary<int, ActiveAwardInfo> value = ActiveMgr.LoadActiveAwardDb(dictionary);
				if (dictionary.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, List<ActiveConditionInfo>>>(ref ActiveMgr.m_ActiveConditionInfo, dictionary);
					Interlocked.Exchange<Dictionary<int, ActiveAwardInfo>>(ref ActiveMgr.m_ActiveAwardInfo, value);
				}
				return true;
			}
			catch (Exception exception)
			{
				ActiveMgr.log.Error("QuestMgr", exception);
			}
			return false;
		}
	}
}
