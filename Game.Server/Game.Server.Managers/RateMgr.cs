using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class RateMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		private static ArrayList m_RateInfos = new ArrayList();
		public static bool Init(GameServerConfig config)
		{
			RateMgr.m_lock.AcquireWriterLock(15000);
			bool result;
			try
			{
				using (ServiceBussiness serviceBussiness = new ServiceBussiness())
				{
					RateMgr.m_RateInfos = serviceBussiness.GetRate(config.ServerID);
				}
				result = true;
			}
			catch (Exception exception)
			{
				if (RateMgr.log.IsErrorEnabled)
				{
					RateMgr.log.Error("RateMgr", exception);
				}
				result = false;
			}
			finally
			{
				RateMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static bool ReLoad()
		{
			return RateMgr.Init(GameServer.Instance.Configuration);
		}
		public static float GetRate(eRateType eType)
		{
			float num = 1f;
			RateMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				RateInfo rateInfoWithType = RateMgr.GetRateInfoWithType((int)eType);
				if (rateInfoWithType == null)
				{
					float result = num;
					return result;
				}
				if (rateInfoWithType.Rate == 0f)
				{
					float result = 1f;
					return result;
				}
				if (RateMgr.IsValid(rateInfoWithType))
				{
					num = rateInfoWithType.Rate;
				}
			}
			catch
			{
			}
			finally
			{
				RateMgr.m_lock.ReleaseReaderLock();
			}
			return num;
		}
		private static RateInfo GetRateInfoWithType(int type)
		{
			foreach (RateInfo rateInfo in RateMgr.m_RateInfos)
			{
				if (rateInfo.Type == type)
				{
					return rateInfo;
				}
			}
			return null;
		}
		private static bool IsValid(RateInfo _RateInfo)
		{
			DateTime arg_06_0 = _RateInfo.BeginDay;
			DateTime arg_0D_0 = _RateInfo.EndDay;
			return _RateInfo.BeginDay.Year <= DateTime.Now.Year && DateTime.Now.Year <= _RateInfo.EndDay.Year && _RateInfo.BeginDay.DayOfYear <= DateTime.Now.DayOfYear && DateTime.Now.DayOfYear <= _RateInfo.EndDay.DayOfYear && !(_RateInfo.BeginTime.TimeOfDay > DateTime.Now.TimeOfDay) && !(DateTime.Now.TimeOfDay > _RateInfo.EndTime.TimeOfDay);
		}
	}
}
