using Bussiness.Protocol;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class DropMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static string[] m_DropTypes = Enum.GetNames(typeof(eDropType));
		private static List<DropCondiction> m_dropcondiction = new List<DropCondiction>();
		private static Dictionary<int, List<DropItem>> m_dropitem = new Dictionary<int, List<DropItem>>();
		public static bool Init()
		{
			return DropMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			try
			{
				List<DropCondiction> value = DropMgr.LoadDropConditionDb();
				Interlocked.Exchange<List<DropCondiction>>(ref DropMgr.m_dropcondiction, value);
				Dictionary<int, List<DropItem>> value2 = DropMgr.LoadDropItemDb();
				Interlocked.Exchange<Dictionary<int, List<DropItem>>>(ref DropMgr.m_dropitem, value2);
				return true;
			}
			catch (Exception exception)
			{
				DropMgr.log.Error("DropMgr", exception);
			}
			return false;
		}
		public static List<DropCondiction> LoadDropConditionDb()
		{
			List<DropCondiction> result;
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				DropCondiction[] allDropCondictions = produceBussiness.GetAllDropCondictions();
				result = ((allDropCondictions != null) ? allDropCondictions.ToList<DropCondiction>() : null);
			}
			return result;
		}
		public static Dictionary<int, List<DropItem>> LoadDropItemDb()
		{
			Dictionary<int, List<DropItem>> dictionary = new Dictionary<int, List<DropItem>>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				DropItem[] allDropItems = produceBussiness.GetAllDropItems();
				foreach (DropCondiction info in DropMgr.m_dropcondiction)
				{
					IEnumerable<DropItem> source = 
						from s in allDropItems
						where s.DropId == info.DropId
						select s;
					dictionary.Add(info.DropId, source.ToList<DropItem>());
				}
			}
			return dictionary;
		}
		public static int FindCondiction(eDropType type, string para1, string para2)
		{
			string value = "," + para1 + ",";
			string value2 = "," + para2 + ",";
			foreach (DropCondiction current in DropMgr.m_dropcondiction)
			{
				if (current.CondictionType == (int)type && current.Para1.IndexOf(value) != -1 && current.Para2.IndexOf(value2) != -1)
				{
					return current.DropId;
				}
			}
			return 0;
		}
		public static List<DropItem> FindDropItem(int dropId)
		{
			if (DropMgr.m_dropitem.ContainsKey(dropId))
			{
				return DropMgr.m_dropitem[dropId];
			}
			return null;
		}
	}
}
