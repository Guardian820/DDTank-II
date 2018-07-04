using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class ItemMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, ItemTemplateInfo> _items;
		private static Dictionary<int, LoadUserBoxInfo> _timeBoxs;
		private static List<ItemTemplateInfo> Lists;
		private static ReaderWriterLock m_lock;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, ItemTemplateInfo> dictionary = new Dictionary<int, ItemTemplateInfo>();
				Dictionary<int, LoadUserBoxInfo> dictionary2 = new Dictionary<int, LoadUserBoxInfo>();
				if (ItemMgr.LoadItem(dictionary, dictionary2))
				{
					ItemMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						ItemMgr._items = dictionary;
						ItemMgr._timeBoxs = dictionary2;
						return true;
					}
					catch
					{
					}
					finally
					{
						ItemMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (ItemMgr.log.IsErrorEnabled)
				{
					ItemMgr.log.Error("ReLoad", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				ItemMgr.m_lock = new ReaderWriterLock();
				ItemMgr._items = new Dictionary<int, ItemTemplateInfo>();
				ItemMgr._timeBoxs = new Dictionary<int, LoadUserBoxInfo>();
				ItemMgr.Lists = new List<ItemTemplateInfo>();
				result = ItemMgr.LoadItem(ItemMgr._items, ItemMgr._timeBoxs);
			}
			catch (Exception exception)
			{
				if (ItemMgr.log.IsErrorEnabled)
				{
					ItemMgr.log.Error("Init", exception);
				}
				result = false;
			}
			return result;
		}
		public static bool LoadItem(Dictionary<int, ItemTemplateInfo> infos, Dictionary<int, LoadUserBoxInfo> userBoxs)
		{
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				ItemTemplateInfo[] allGoods = produceBussiness.GetAllGoods();
				ItemTemplateInfo[] array = allGoods;
				for (int i = 0; i < array.Length; i++)
				{
					ItemTemplateInfo itemTemplateInfo = array[i];
					if (!infos.Keys.Contains(itemTemplateInfo.TemplateID))
					{
						infos.Add(itemTemplateInfo.TemplateID, itemTemplateInfo);
					}
				}
				LoadUserBoxInfo[] allTimeBoxAward = produceBussiness.GetAllTimeBoxAward();
				LoadUserBoxInfo[] array2 = allTimeBoxAward;
				for (int j = 0; j < array2.Length; j++)
				{
					LoadUserBoxInfo loadUserBoxInfo = array2[j];
					if (!infos.Keys.Contains(loadUserBoxInfo.ID))
					{
						userBoxs.Add(loadUserBoxInfo.ID, loadUserBoxInfo);
					}
				}
			}
			return true;
		}
		public static LoadUserBoxInfo FindItemBoxTypeAndLv(int type, int lv)
		{
			if (ItemMgr._timeBoxs == null)
			{
				ItemMgr.Init();
			}
			ItemMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				foreach (LoadUserBoxInfo current in ItemMgr._timeBoxs.Values)
				{
					if (current.Type == type && current.Level == lv)
					{
						return current;
					}
				}
			}
			finally
			{
				ItemMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static LoadUserBoxInfo FindItemBoxTemplate(int Id)
		{
			if (ItemMgr._timeBoxs == null)
			{
				ItemMgr.Init();
			}
			ItemMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (ItemMgr._timeBoxs.Keys.Contains(Id))
				{
					return ItemMgr._timeBoxs[Id];
				}
			}
			finally
			{
				ItemMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static void LoadRecycleItemTemplate()
		{
			if (ItemMgr._items == null)
			{
				ItemMgr.Init();
			}
			foreach (ItemTemplateInfo current in ItemMgr._items.Values)
			{
				if (current.Quality > 1 && current.Quality < 5 && current.CanRecycle > 0 && current.TemplateID > 0 && current.TemplateID != 11107)
				{
					switch (current.CategoryID)
					{
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 8:
					case 9:
					case 11:
					case 13:
					case 14:
					case 15:
					case 16:
					case 17:
					case 18:
					case 19:
						ItemMgr.Lists.Add(current);
						break;
					}
				}
			}
		}
		public static List<ItemTemplateInfo> FindRecycleItemTemplate(int qmax)
		{
			if (ItemMgr.Lists.Count == 0)
			{
				ItemMgr.LoadRecycleItemTemplate();
			}
			List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
			ItemMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				foreach (ItemTemplateInfo current in ItemMgr.Lists)
				{
					if (current.Quality < qmax)
					{
						list.Add(current);
					}
				}
			}
			finally
			{
				ItemMgr.m_lock.ReleaseReaderLock();
			}
			return list;
		}
		public static ItemTemplateInfo FindItemTemplate(int templateId)
		{
			if (ItemMgr._items == null)
			{
				ItemMgr.Init();
			}
			ItemMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (ItemMgr._items.Keys.Contains(templateId))
				{
					return ItemMgr._items[templateId];
				}
			}
			finally
			{
				ItemMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static ItemTemplateInfo GetGoodsbyFusionTypeandQuality(int fusionType, int quality)
		{
			if (ItemMgr._items == null)
			{
				ItemMgr.Init();
			}
			ItemMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				foreach (ItemTemplateInfo current in ItemMgr._items.Values)
				{
					if (current.FusionType == fusionType && current.Quality == quality)
					{
						return current;
					}
				}
			}
			finally
			{
				ItemMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static ItemTemplateInfo GetGoodsbyFusionTypeandLevel(int fusionType, int level)
		{
			if (ItemMgr._items == null)
			{
				ItemMgr.Init();
			}
			ItemMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				foreach (ItemTemplateInfo current in ItemMgr._items.Values)
				{
					if (current.FusionType == fusionType && current.Level == level)
					{
						return current;
					}
				}
			}
			finally
			{
				ItemMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
	}
}
