using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class PropItemMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		private static ReaderWriterLock m_lock;
		private static Dictionary<int, ItemTemplateInfo> _allProp;
		private static int[] PropBag = new int[]
		{
			10001,
			10002,
			10003,
			10004,
			10005,
			10006,
			10007,
			10008
		};
		public static bool Reload()
		{
			try
			{
				Dictionary<int, ItemTemplateInfo> allProp = new Dictionary<int, ItemTemplateInfo>();
				if (PropItemMgr.LoadProps(allProp))
				{
					PropItemMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						PropItemMgr._allProp = allProp;
						return true;
					}
					catch
					{
					}
					finally
					{
						PropItemMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (PropItemMgr.log.IsErrorEnabled)
				{
					PropItemMgr.log.Error("ReloadProps", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				PropItemMgr.m_lock = new ReaderWriterLock();
				PropItemMgr._allProp = new Dictionary<int, ItemTemplateInfo>();
				result = PropItemMgr.LoadProps(PropItemMgr._allProp);
			}
			catch (Exception exception)
			{
				if (PropItemMgr.log.IsErrorEnabled)
				{
					PropItemMgr.log.Error("InitProps", exception);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadProps(Dictionary<int, ItemTemplateInfo> allProp)
		{
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				ItemTemplateInfo[] singleCategory = produceBussiness.GetSingleCategory(10);
				ItemTemplateInfo[] array = singleCategory;
				for (int i = 0; i < array.Length; i++)
				{
					ItemTemplateInfo itemTemplateInfo = array[i];
					allProp.Add(itemTemplateInfo.TemplateID, itemTemplateInfo);
				}
			}
			return true;
		}
		public static ItemTemplateInfo FindAllProp(int id)
		{
			PropItemMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (PropItemMgr._allProp.ContainsKey(id))
				{
					return PropItemMgr._allProp[id];
				}
			}
			catch
			{
			}
			finally
			{
				PropItemMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static ItemTemplateInfo FindFightingProp(int id)
		{
			PropItemMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (!PropItemMgr.PropBag.Contains(id))
				{
					ItemTemplateInfo result = null;
					return result;
				}
				if (PropItemMgr._allProp.ContainsKey(id))
				{
					ItemTemplateInfo result = PropItemMgr._allProp[id];
					return result;
				}
			}
			catch
			{
			}
			finally
			{
				PropItemMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
	}
}
