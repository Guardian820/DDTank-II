using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class LevelMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, LevelInfo> _levels;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom rand;
		public static bool Init()
		{
			bool result;
			try
			{
				LevelMgr.m_lock = new ReaderWriterLock();
				LevelMgr._levels = new Dictionary<int, LevelInfo>();
				LevelMgr.rand = new ThreadSafeRandom();
				result = LevelMgr.LoadLevel(LevelMgr._levels);
			}
			catch (Exception exception)
			{
				if (LevelMgr.log.IsErrorEnabled)
				{
					LevelMgr.log.Error("LevelMgr", exception);
				}
				result = false;
			}
			return result;
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, LevelInfo> dictionary = new Dictionary<int, LevelInfo>();
				if (LevelMgr.LoadLevel(dictionary))
				{
					LevelMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						LevelMgr._levels = dictionary;
						return true;
					}
					catch
					{
					}
					finally
					{
						LevelMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (LevelMgr.log.IsErrorEnabled)
				{
					LevelMgr.log.Error("LevelMgr", exception);
				}
			}
			return false;
		}
		private static bool LoadLevel(Dictionary<int, LevelInfo> Level)
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				LevelInfo[] allLevel = playerBussiness.GetAllLevel();
				LevelInfo[] array = allLevel;
				for (int i = 0; i < array.Length; i++)
				{
					LevelInfo levelInfo = array[i];
					if (!Level.ContainsKey(levelInfo.Grade))
					{
						Level.Add(levelInfo.Grade, levelInfo);
					}
				}
			}
			return true;
		}
		public static LevelInfo FindLevel(int Grade)
		{
			LevelMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (LevelMgr._levels.ContainsKey(Grade))
				{
					return LevelMgr._levels[Grade];
				}
			}
			catch
			{
			}
			finally
			{
				LevelMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static int GetMaxLevel()
		{
			if (LevelMgr._levels == null)
			{
				LevelMgr.Init();
			}
			return LevelMgr._levels.Values.Count;
		}
		public static int GetLevel(int GP)
		{
			if (GP >= LevelMgr.FindLevel(LevelMgr.GetMaxLevel()).GP)
			{
				return LevelMgr.GetMaxLevel();
			}
			int i = 1;
			while (i <= LevelMgr.GetMaxLevel())
			{
				if (GP < LevelMgr.FindLevel(i).GP)
				{
					if (i - 1 != 0)
					{
						return i - 1;
					}
					return 1;
				}
				else
				{
					i++;
				}
			}
			return 1;
		}
		public static int GetGP(int level)
		{
			if (LevelMgr.GetMaxLevel() > level && level > 0)
			{
				return LevelMgr.FindLevel(level - 1).GP;
			}
			return 0;
		}
		public static int ReduceGP(int level, int totalGP)
		{
			if (LevelMgr.GetMaxLevel() <= level || level <= 0)
			{
				return 0;
			}
			totalGP -= LevelMgr.FindLevel(level - 1).GP;
			if (totalGP >= level * 12)
			{
				return level * 12;
			}
			if (totalGP >= 0)
			{
				return totalGP;
			}
			return 0;
		}
		public static int IncreaseGP(int level, int totalGP)
		{
			if (LevelMgr.GetMaxLevel() > level && level > 0)
			{
				return level * 12;
			}
			return 0;
		}
	}
}
