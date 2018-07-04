using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class QuestMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, QuestInfo> m_questinfo = new Dictionary<int, QuestInfo>();
		private static Dictionary<int, List<QuestConditionInfo>> m_questcondiction = new Dictionary<int, List<QuestConditionInfo>>();
		private static Dictionary<int, List<QuestAwardInfo>> m_questgoods = new Dictionary<int, List<QuestAwardInfo>>();
		public static bool Init()
		{
			return QuestMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, QuestInfo> dictionary = QuestMgr.LoadQuestInfoDb();
				Dictionary<int, List<QuestConditionInfo>> value = QuestMgr.LoadQuestCondictionDb(dictionary);
				Dictionary<int, List<QuestAwardInfo>> value2 = QuestMgr.LoadQuestGoodDb(dictionary);
				if (dictionary.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, QuestInfo>>(ref QuestMgr.m_questinfo, dictionary);
					Interlocked.Exchange<Dictionary<int, List<QuestConditionInfo>>>(ref QuestMgr.m_questcondiction, value);
					Interlocked.Exchange<Dictionary<int, List<QuestAwardInfo>>>(ref QuestMgr.m_questgoods, value2);
				}
				return true;
			}
			catch (Exception exception)
			{
				QuestMgr.log.Error("QuestMgr", exception);
			}
			return false;
		}
		public static Dictionary<int, QuestInfo> LoadQuestInfoDb()
		{
			Dictionary<int, QuestInfo> dictionary = new Dictionary<int, QuestInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				QuestInfo[] aLlQuest = produceBussiness.GetALlQuest();
				QuestInfo[] array = aLlQuest;
				for (int i = 0; i < array.Length; i++)
				{
					QuestInfo questInfo = array[i];
					if (!dictionary.ContainsKey(questInfo.ID))
					{
						dictionary.Add(questInfo.ID, questInfo);
					}
				}
			}
			return dictionary;
		}
		public static Dictionary<int, List<QuestConditionInfo>> LoadQuestCondictionDb(Dictionary<int, QuestInfo> quests)
		{
			Dictionary<int, List<QuestConditionInfo>> dictionary = new Dictionary<int, List<QuestConditionInfo>>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				QuestConditionInfo[] allQuestCondiction = produceBussiness.GetAllQuestCondiction();
				foreach (QuestInfo quest in quests.Values)
				{
					IEnumerable<QuestConditionInfo> source = 
						from s in allQuestCondiction
						where s.QuestID == quest.ID
						select s;
					dictionary.Add(quest.ID, source.ToList<QuestConditionInfo>());
				}
			}
			return dictionary;
		}
		public static Dictionary<int, List<QuestAwardInfo>> LoadQuestGoodDb(Dictionary<int, QuestInfo> quests)
		{
			Dictionary<int, List<QuestAwardInfo>> dictionary = new Dictionary<int, List<QuestAwardInfo>>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				QuestAwardInfo[] allQuestGoods = produceBussiness.GetAllQuestGoods();
				foreach (QuestInfo quest in quests.Values)
				{
					IEnumerable<QuestAwardInfo> source = 
						from s in allQuestGoods
						where s.QuestID == quest.ID
						select s;
					dictionary.Add(quest.ID, source.ToList<QuestAwardInfo>());
				}
			}
			return dictionary;
		}
		public static QuestInfo GetSingleQuest(int id)
		{
			if (QuestMgr.m_questinfo.ContainsKey(id))
			{
				return QuestMgr.m_questinfo[id];
			}
			return null;
		}
		public static List<QuestAwardInfo> GetQuestGoods(QuestInfo info)
		{
			if (QuestMgr.m_questgoods.ContainsKey(info.ID))
			{
				return QuestMgr.m_questgoods[info.ID];
			}
			return null;
		}
		public static List<QuestConditionInfo> GetQuestCondiction(QuestInfo info)
		{
			if (QuestMgr.m_questcondiction.ContainsKey(info.ID))
			{
				return QuestMgr.m_questcondiction[info.ID];
			}
			return null;
		}
	}
}
