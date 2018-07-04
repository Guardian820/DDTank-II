using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class ExerciseMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, ExerciseInfo> _exercises;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom rand;
		public static bool Init()
		{
			bool result;
			try
			{
				ExerciseMgr.m_lock = new ReaderWriterLock();
				ExerciseMgr._exercises = new Dictionary<int, ExerciseInfo>();
				ExerciseMgr.rand = new ThreadSafeRandom();
				result = ExerciseMgr.LoadExercise(ExerciseMgr._exercises);
			}
			catch (Exception exception)
			{
				if (ExerciseMgr.log.IsErrorEnabled)
				{
					ExerciseMgr.log.Error("ExercisesMgr", exception);
				}
				result = false;
			}
			return result;
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, ExerciseInfo> dictionary = new Dictionary<int, ExerciseInfo>();
				if (ExerciseMgr.LoadExercise(dictionary))
				{
					ExerciseMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						ExerciseMgr._exercises = dictionary;
						return true;
					}
					catch
					{
					}
					finally
					{
						ExerciseMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (ExerciseMgr.log.IsErrorEnabled)
				{
					ExerciseMgr.log.Error("ExerciseMgr", exception);
				}
			}
			return false;
		}
		private static bool LoadExercise(Dictionary<int, ExerciseInfo> Exercise)
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				ExerciseInfo[] allExercise = playerBussiness.GetAllExercise();
				ExerciseInfo[] array = allExercise;
				for (int i = 0; i < array.Length; i++)
				{
					ExerciseInfo exerciseInfo = array[i];
					if (!Exercise.ContainsKey(exerciseInfo.Grage))
					{
						Exercise.Add(exerciseInfo.Grage, exerciseInfo);
					}
				}
			}
			return true;
		}
		public static ExerciseInfo FindExercise(int Grage)
		{
			if (Grage == 0)
			{
				Grage = 1;
			}
			ExerciseMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (ExerciseMgr._exercises.ContainsKey(Grage))
				{
					return ExerciseMgr._exercises[Grage];
				}
			}
			catch
			{
			}
			finally
			{
				ExerciseMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static int GetMaxLevel()
		{
			if (ExerciseMgr._exercises == null)
			{
				ExerciseMgr.Init();
			}
			return ExerciseMgr._exercises.Values.Count;
		}
		public static int GetExercise(int GP, string type)
		{
			for (int i = 1; i <= ExerciseMgr.GetMaxLevel(); i++)
			{
				if (GP < ExerciseMgr.FindExercise(i).GP && GP >= 50 && type != null)
				{
					if (type == "A")
					{
						return ExerciseMgr.FindExercise(i - 1).ExerciseA;
					}
					if (type == "AG")
					{
						return ExerciseMgr.FindExercise(i - 1).ExerciseAG;
					}
					if (type == "D")
					{
						return ExerciseMgr.FindExercise(i - 1).ExerciseD;
					}
					if (type == "H")
					{
						return ExerciseMgr.FindExercise(i - 1).ExerciseH;
					}
					if (type == "L")
					{
						return ExerciseMgr.FindExercise(i - 1).ExerciseL;
					}
				}
			}
			return 0;
		}
	}
}
