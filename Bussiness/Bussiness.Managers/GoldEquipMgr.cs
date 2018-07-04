using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class GoldEquipMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, GoldEquipTemplateLoadInfo> _items;
		private static ReaderWriterLock m_lock;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, GoldEquipTemplateLoadInfo> dictionary = new Dictionary<int, GoldEquipTemplateLoadInfo>();
				if (GoldEquipMgr.LoadItem(dictionary))
				{
					GoldEquipMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						GoldEquipMgr._items = dictionary;
						return true;
					}
					catch
					{
					}
					finally
					{
						GoldEquipMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (GoldEquipMgr.log.IsErrorEnabled)
				{
					GoldEquipMgr.log.Error("ReLoad", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				GoldEquipMgr.m_lock = new ReaderWriterLock();
				GoldEquipMgr._items = new Dictionary<int, GoldEquipTemplateLoadInfo>();
				result = GoldEquipMgr.LoadItem(GoldEquipMgr._items);
			}
			catch (Exception exception)
			{
				if (GoldEquipMgr.log.IsErrorEnabled)
				{
					GoldEquipMgr.log.Error("Init", exception);
				}
				result = false;
			}
			return result;
		}
		public static bool LoadItem(Dictionary<int, GoldEquipTemplateLoadInfo> infos)
		{
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				GoldEquipTemplateLoadInfo[] allGoldEquipTemplateLoad = produceBussiness.GetAllGoldEquipTemplateLoad();
				GoldEquipTemplateLoadInfo[] array = allGoldEquipTemplateLoad;
				for (int i = 0; i < array.Length; i++)
				{
					GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = array[i];
					if (!infos.Keys.Contains(goldEquipTemplateLoadInfo.ID))
					{
						infos.Add(goldEquipTemplateLoadInfo.ID, goldEquipTemplateLoadInfo);
					}
				}
			}
			return true;
		}
		public static GoldEquipTemplateLoadInfo FindGoldEquipCategoryID(int CategoryID)
		{
			if (GoldEquipMgr._items == null)
			{
				GoldEquipMgr.Init();
			}
			GoldEquipMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				foreach (GoldEquipTemplateLoadInfo current in GoldEquipMgr._items.Values)
				{
					if (current.CategoryID == CategoryID)
					{
						return current;
					}
				}
			}
			finally
			{
				GoldEquipMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static GoldEquipTemplateLoadInfo FindGoldEquipTemplate(int TemplateId)
		{
			if (GoldEquipMgr._items == null)
			{
				GoldEquipMgr.Init();
			}
			GoldEquipMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				foreach (GoldEquipTemplateLoadInfo current in GoldEquipMgr._items.Values)
				{
					if (current.OldTemplateId == TemplateId || current.NewTemplateId == TemplateId)
					{
						return current;
					}
				}
			}
			finally
			{
				GoldEquipMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
	}
}
