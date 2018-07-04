using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class TotemHonorMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, TotemHonorTemplateInfo> _totemHonorTemplate;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom rand;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, TotemHonorTemplateInfo> totemHonorTemplate = new Dictionary<int, TotemHonorTemplateInfo>();
				if (TotemHonorMgr.Load(totemHonorTemplate))
				{
					TotemHonorMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						TotemHonorMgr._totemHonorTemplate = totemHonorTemplate;
						return true;
					}
					catch
					{
					}
					finally
					{
						TotemHonorMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (TotemHonorMgr.log.IsErrorEnabled)
				{
					TotemHonorMgr.log.Error("ConsortiaLevelMgr", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				TotemHonorMgr.m_lock = new ReaderWriterLock();
				TotemHonorMgr._totemHonorTemplate = new Dictionary<int, TotemHonorTemplateInfo>();
				TotemHonorMgr.rand = new ThreadSafeRandom();
				result = TotemHonorMgr.Load(TotemHonorMgr._totemHonorTemplate);
			}
			catch (Exception exception)
			{
				if (TotemHonorMgr.log.IsErrorEnabled)
				{
					TotemHonorMgr.log.Error("ConsortiaLevelMgr", exception);
				}
				result = false;
			}
			return result;
		}
		private static bool Load(Dictionary<int, TotemHonorTemplateInfo> TotemHonorTemplate)
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				TotemHonorTemplateInfo[] allTotemHonorTemplate = playerBussiness.GetAllTotemHonorTemplate();
				TotemHonorTemplateInfo[] array = allTotemHonorTemplate;
				for (int i = 0; i < array.Length; i++)
				{
					TotemHonorTemplateInfo totemHonorTemplateInfo = array[i];
					if (!TotemHonorTemplate.ContainsKey(totemHonorTemplateInfo.ID))
					{
						TotemHonorTemplate.Add(totemHonorTemplateInfo.ID, totemHonorTemplateInfo);
					}
				}
			}
			return true;
		}
		public static TotemHonorTemplateInfo FindTotemHonorTemplateInfo(int ID)
		{
			TotemHonorMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (TotemHonorMgr._totemHonorTemplate.ContainsKey(ID))
				{
					return TotemHonorMgr._totemHonorTemplate[ID];
				}
			}
			catch
			{
			}
			finally
			{
				TotemHonorMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
	}
}
