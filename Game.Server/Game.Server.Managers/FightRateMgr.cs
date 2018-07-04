using Bussiness;
using Game.Base.Packets;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class FightRateMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ReaderWriterLock m_lock;
		protected static Dictionary<int, FightRateInfo> _fightRate;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, FightRateInfo> dictionary = new Dictionary<int, FightRateInfo>();
				if (FightRateMgr.LoadFightRate(dictionary))
				{
					FightRateMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						FightRateMgr._fightRate = dictionary;
						return true;
					}
					catch
					{
					}
					finally
					{
						FightRateMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (FightRateMgr.log.IsErrorEnabled)
				{
					FightRateMgr.log.Error("AwardMgr", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				FightRateMgr.m_lock = new ReaderWriterLock();
				FightRateMgr._fightRate = new Dictionary<int, FightRateInfo>();
				result = FightRateMgr.LoadFightRate(FightRateMgr._fightRate);
			}
			catch (Exception exception)
			{
				if (FightRateMgr.log.IsErrorEnabled)
				{
					FightRateMgr.log.Error("AwardMgr", exception);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadFightRate(Dictionary<int, FightRateInfo> fighRate)
		{
			using (ServiceBussiness serviceBussiness = new ServiceBussiness())
			{
				FightRateInfo[] fightRate = serviceBussiness.GetFightRate(GameServer.Instance.Configuration.ServerID);
				FightRateInfo[] array = fightRate;
				for (int i = 0; i < array.Length; i++)
				{
					FightRateInfo fightRateInfo = array[i];
					if (!fighRate.ContainsKey(fightRateInfo.ID))
					{
						fighRate.Add(fightRateInfo.ID, fightRateInfo);
					}
				}
			}
			return true;
		}
		public static FightRateInfo[] GetAllFightRateInfo()
		{
			FightRateInfo[] array = null;
			FightRateMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				array = FightRateMgr._fightRate.Values.ToArray<FightRateInfo>();
			}
			catch
			{
			}
			finally
			{
				FightRateMgr.m_lock.ReleaseReaderLock();
			}
			if (array != null)
			{
				return array;
			}
			return new FightRateInfo[0];
		}
		public static bool CanChangeStyle(BaseRoom game, GSPacketIn pkg)
		{
			FightRateInfo[] allFightRateInfo = FightRateMgr.GetAllFightRateInfo();
			try
			{
				FightRateInfo[] array = allFightRateInfo;
				for (int i = 0; i < array.Length; i++)
				{
					FightRateInfo fightRateInfo = array[i];
					if (fightRateInfo.BeginDay.Year <= DateTime.Now.Year && DateTime.Now.Year <= fightRateInfo.EndDay.Year && fightRateInfo.BeginDay.DayOfYear <= DateTime.Now.DayOfYear && DateTime.Now.DayOfYear <= fightRateInfo.EndDay.DayOfYear && fightRateInfo.BeginTime.TimeOfDay <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= fightRateInfo.EndTime.TimeOfDay && ThreadSafeRandom.NextStatic(1000000) < fightRateInfo.Rate)
					{
						return true;
					}
				}
			}
			catch
			{
			}
			pkg.WriteBoolean(false);
			return false;
		}
	}
}
