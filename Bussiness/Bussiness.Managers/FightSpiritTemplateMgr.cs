using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class FightSpiritTemplateMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, FightSpiritTemplateInfo> _fightSpiritTemplate;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom rand;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, FightSpiritTemplateInfo> dictionary = new Dictionary<int, FightSpiritTemplateInfo>();
				if (FightSpiritTemplateMgr.Load(dictionary))
				{
					FightSpiritTemplateMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						FightSpiritTemplateMgr._fightSpiritTemplate = dictionary;
						return true;
					}
					catch
					{
					}
					finally
					{
						FightSpiritTemplateMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (FightSpiritTemplateMgr.log.IsErrorEnabled)
				{
					FightSpiritTemplateMgr.log.Error("ConsortiaLevelMgr", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				FightSpiritTemplateMgr.m_lock = new ReaderWriterLock();
				FightSpiritTemplateMgr._fightSpiritTemplate = new Dictionary<int, FightSpiritTemplateInfo>();
				FightSpiritTemplateMgr.rand = new ThreadSafeRandom();
				result = FightSpiritTemplateMgr.Load(FightSpiritTemplateMgr._fightSpiritTemplate);
			}
			catch (Exception exception)
			{
				if (FightSpiritTemplateMgr.log.IsErrorEnabled)
				{
					FightSpiritTemplateMgr.log.Error("ConsortiaLevelMgr", exception);
				}
				result = false;
			}
			return result;
		}
		private static bool Load(Dictionary<int, FightSpiritTemplateInfo> consortiaLevel)
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				FightSpiritTemplateInfo[] allFightSpiritTemplate = playerBussiness.GetAllFightSpiritTemplate();
				FightSpiritTemplateInfo[] array = allFightSpiritTemplate;
				for (int i = 0; i < array.Length; i++)
				{
					FightSpiritTemplateInfo fightSpiritTemplateInfo = array[i];
					if (!consortiaLevel.ContainsKey(fightSpiritTemplateInfo.ID))
					{
						consortiaLevel.Add(fightSpiritTemplateInfo.ID, fightSpiritTemplateInfo);
					}
				}
			}
			return true;
		}
		public static FightSpiritTemplateInfo FindFightSpiritTemplateInfo(int FigSpiritId, int lv)
		{
			FightSpiritTemplateMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				foreach (FightSpiritTemplateInfo current in FightSpiritTemplateMgr._fightSpiritTemplate.Values)
				{
					if (current.FightSpiritID == FigSpiritId && current.Level == lv)
					{
						return current;
					}
				}
			}
			catch
			{
			}
			finally
			{
				FightSpiritTemplateMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static int getProp(int FigSpiritId, int lv, int place)
		{
			FightSpiritTemplateInfo fightSpiritTemplateInfo = FightSpiritTemplateMgr.FindFightSpiritTemplateInfo(FigSpiritId, lv);
			switch (place)
			{
			case 2:
				return fightSpiritTemplateInfo.Attack;

			case 3:
				return fightSpiritTemplateInfo.Lucky;

			case 4:
				break;

			case 5:
				return fightSpiritTemplateInfo.Agility;

			default:
				switch (place)
				{
				case 11:
					return fightSpiritTemplateInfo.Defence;

				case 13:
					return fightSpiritTemplateInfo.Blood;
				}
				break;
			}
			return 0;
		}
	}
}
