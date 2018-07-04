using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class RuneMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, RuneTemplateInfo> _items;
		private static ReaderWriterLock m_lock;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, RuneTemplateInfo> dictionary = new Dictionary<int, RuneTemplateInfo>();
				if (RuneMgr.LoadItem(dictionary))
				{
					RuneMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						RuneMgr._items = dictionary;
						return true;
					}
					catch
					{
					}
					finally
					{
						RuneMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (RuneMgr.log.IsErrorEnabled)
				{
					RuneMgr.log.Error("ReLoad", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				RuneMgr.m_lock = new ReaderWriterLock();
				RuneMgr._items = new Dictionary<int, RuneTemplateInfo>();
				result = RuneMgr.LoadItem(RuneMgr._items);
			}
			catch (Exception exception)
			{
				if (RuneMgr.log.IsErrorEnabled)
				{
					RuneMgr.log.Error("Init", exception);
				}
				result = false;
			}
			return result;
		}
		public static bool LoadItem(Dictionary<int, RuneTemplateInfo> infos)
		{
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				RuneTemplateInfo[] allRuneTemplate = produceBussiness.GetAllRuneTemplate();
				RuneTemplateInfo[] array = allRuneTemplate;
				for (int i = 0; i < array.Length; i++)
				{
					RuneTemplateInfo runeTemplateInfo = array[i];
					if (!infos.Keys.Contains(runeTemplateInfo.TemplateID))
					{
						infos.Add(runeTemplateInfo.TemplateID, runeTemplateInfo);
					}
				}
			}
			return true;
		}
		public static RuneTemplateInfo FindRuneTemplateID(int templateID)
		{
			if (RuneMgr._items == null)
			{
				RuneMgr.Init();
			}
			RuneMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				foreach (RuneTemplateInfo current in RuneMgr._items.Values)
				{
					if (current.TemplateID == templateID)
					{
						return current;
					}
				}
			}
			finally
			{
				RuneMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static int FindRuneExp(int lv)
		{
			if (lv < 0)
			{
				return 1;
			}
			return GameProperties.RuneExp()[lv];
		}
		public static int MaxLv()
		{
			return GameProperties.RuneExp().Count - 1;
		}
		public static bool CanUpLv(int exp, int lv)
		{
			return exp >= GameProperties.RuneExp()[lv];
		}
		public static List<RuneTemplateInfo> OpenPackageLv1()
		{
			return RuneMgr.OpenPackage(0, 3);
		}
		public static List<RuneTemplateInfo> OpenPackageLv2()
		{
			return RuneMgr.OpenPackage(3, 9);
		}
		public static List<RuneTemplateInfo> OpenPackageLv3()
		{
			return RuneMgr.OpenPackage(9, 11);
		}
		public static List<RuneTemplateInfo> OpenPackageLv4()
		{
			return RuneMgr.OpenPackage(13, 15);
		}
		public static List<RuneTemplateInfo> OpenPackage(int Minlv, int Maxlv)
		{
			List<RuneTemplateInfo> list = new List<RuneTemplateInfo>();
			foreach (RuneTemplateInfo current in RuneMgr._items.Values)
			{
				if (current.BaseLevel > Minlv && current.BaseLevel <= Maxlv)
				{
					list.Add(current);
				}
			}
			return list;
		}
	}
}
