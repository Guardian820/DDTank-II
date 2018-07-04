using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class TotemMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, TotemInfo> _consortiaLevel;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom rand;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, TotemInfo> consortiaLevel = new Dictionary<int, TotemInfo>();
				if (TotemMgr.Load(consortiaLevel))
				{
					TotemMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						TotemMgr._consortiaLevel = consortiaLevel;
						return true;
					}
					catch
					{
					}
					finally
					{
						TotemMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (TotemMgr.log.IsErrorEnabled)
				{
					TotemMgr.log.Error("ConsortiaLevelMgr", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				TotemMgr.m_lock = new ReaderWriterLock();
				TotemMgr._consortiaLevel = new Dictionary<int, TotemInfo>();
				TotemMgr.rand = new ThreadSafeRandom();
				result = TotemMgr.Load(TotemMgr._consortiaLevel);
			}
			catch (Exception exception)
			{
				if (TotemMgr.log.IsErrorEnabled)
				{
					TotemMgr.log.Error("ConsortiaLevelMgr", exception);
				}
				result = false;
			}
			return result;
		}
		private static bool Load(Dictionary<int, TotemInfo> consortiaLevel)
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				TotemInfo[] allTotem = playerBussiness.GetAllTotem();
				TotemInfo[] array = allTotem;
				for (int i = 0; i < array.Length; i++)
				{
					TotemInfo totemInfo = array[i];
					if (!consortiaLevel.ContainsKey(totemInfo.ID))
					{
						consortiaLevel.Add(totemInfo.ID, totemInfo);
					}
				}
			}
			return true;
		}
		public static TotemInfo FindTotemInfo(int ID)
		{
			if (ID < 10000)
			{
				ID = 10001;
			}
			TotemMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (TotemMgr._consortiaLevel.ContainsKey(ID))
				{
					return TotemMgr._consortiaLevel[ID];
				}
			}
			catch
			{
			}
			finally
			{
				TotemMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static int getProp(int id, string typeOf)
		{
			int num = 0;
			for (int i = 10001; i <= id; i++)
			{
				TotemInfo totemInfo = TotemMgr.FindTotemInfo(i);
				switch (typeOf)
				{
				case "att":
					num += totemInfo.AddAttack;
					break;

				case "agi":
					num += totemInfo.AddAgility;
					break;

				case "def":
					num += totemInfo.AddDefence;
					break;

				case "luc":
					num += totemInfo.AddLuck;
					break;

				case "blo":
					num += totemInfo.AddBlood;
					break;

				case "dam":
					num += totemInfo.AddDamage;
					break;

				case "gua":
					num += totemInfo.AddGuard;
					break;
				}
			}
			return num;
		}
	}
}
