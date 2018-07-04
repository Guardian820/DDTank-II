using Bussiness;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class ConsortiaMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<string, int> _ally;
		private static ReaderWriterLock m_lock;
		private static Dictionary<int, ConsortiaInfo> _consortia;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<string, int> ally = new Dictionary<string, int>();
				Dictionary<int, ConsortiaInfo> consortia = new Dictionary<int, ConsortiaInfo>();
				if (ConsortiaMgr.Load(ally) && ConsortiaMgr.LoadConsortia(consortia))
				{
					ConsortiaMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						ConsortiaMgr._ally = ally;
						ConsortiaMgr._consortia = consortia;
						return true;
					}
					catch
					{
					}
					finally
					{
						ConsortiaMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (ConsortiaMgr.log.IsErrorEnabled)
				{
					ConsortiaMgr.log.Error("ConsortiaMgr", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				ConsortiaMgr.m_lock = new ReaderWriterLock();
				ConsortiaMgr._ally = new Dictionary<string, int>();
				if (!ConsortiaMgr.Load(ConsortiaMgr._ally))
				{
					result = false;
				}
				else
				{
					ConsortiaMgr._consortia = new Dictionary<int, ConsortiaInfo>();
					if (!ConsortiaMgr.LoadConsortia(ConsortiaMgr._consortia))
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			catch (Exception exception)
			{
				if (ConsortiaMgr.log.IsErrorEnabled)
				{
					ConsortiaMgr.log.Error("ConsortiaMgr", exception);
				}
				result = false;
			}
			return result;
		}
		private static bool Load(Dictionary<string, int> ally)
		{
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				ConsortiaAllyInfo[] consortiaAllyAll = consortiaBussiness.GetConsortiaAllyAll();
				ConsortiaAllyInfo[] array = consortiaAllyAll;
				for (int i = 0; i < array.Length; i++)
				{
					ConsortiaAllyInfo consortiaAllyInfo = array[i];
					if (consortiaAllyInfo.IsExist)
					{
						string key;
						if (consortiaAllyInfo.Consortia1ID < consortiaAllyInfo.Consortia2ID)
						{
							key = consortiaAllyInfo.Consortia1ID + "&" + consortiaAllyInfo.Consortia2ID;
						}
						else
						{
							key = consortiaAllyInfo.Consortia2ID + "&" + consortiaAllyInfo.Consortia1ID;
						}
						if (!ally.ContainsKey(key))
						{
							ally.Add(key, consortiaAllyInfo.State);
						}
					}
				}
			}
			return true;
		}
		private static bool LoadConsortia(Dictionary<int, ConsortiaInfo> consortia)
		{
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				ConsortiaInfo[] consortiaAll = consortiaBussiness.GetConsortiaAll();
				ConsortiaInfo[] array = consortiaAll;
				for (int i = 0; i < array.Length; i++)
				{
					ConsortiaInfo consortiaInfo = array[i];
					if (consortiaInfo.IsExist && !consortia.ContainsKey(consortiaInfo.ConsortiaID))
					{
						consortia.Add(consortiaInfo.ConsortiaID, consortiaInfo);
					}
				}
			}
			return true;
		}
		public static int UpdateConsortiaAlly(int cosortiaID1, int consortiaID2, int state)
		{
			string key;
			if (cosortiaID1 < consortiaID2)
			{
				key = cosortiaID1 + "&" + consortiaID2;
			}
			else
			{
				key = consortiaID2 + "&" + cosortiaID1;
			}
			ConsortiaMgr.m_lock.AcquireWriterLock(15000);
			try
			{
				if (!ConsortiaMgr._ally.ContainsKey(key))
				{
					ConsortiaMgr._ally.Add(key, state);
				}
				else
				{
					ConsortiaMgr._ally[key] = state;
				}
			}
			catch
			{
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return 0;
		}
		public static bool ConsortiaUpGrade(int consortiaID, int consortiaLevel)
		{
			bool result = false;
			ConsortiaMgr.m_lock.AcquireWriterLock(15000);
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID) && ConsortiaMgr._consortia[consortiaID].IsExist)
				{
					ConsortiaMgr._consortia[consortiaID].Level = consortiaLevel;
				}
				else
				{
					ConsortiaInfo consortiaInfo = new ConsortiaInfo();
					consortiaInfo.BuildDate = DateTime.Now;
					consortiaInfo.Level = consortiaLevel;
					consortiaInfo.IsExist = true;
					ConsortiaMgr._consortia.Add(consortiaID, consortiaInfo);
				}
			}
			catch (Exception exception)
			{
				ConsortiaMgr.log.Error("ConsortiaUpGrade", exception);
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static bool ConsortiaStoreUpGrade(int consortiaID, int storeLevel)
		{
			bool result = false;
			ConsortiaMgr.m_lock.AcquireWriterLock(15000);
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID) && ConsortiaMgr._consortia[consortiaID].IsExist)
				{
					ConsortiaMgr._consortia[consortiaID].StoreLevel = storeLevel;
				}
			}
			catch (Exception exception)
			{
				ConsortiaMgr.log.Error("ConsortiaUpGrade", exception);
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static bool ConsortiaShopUpGrade(int consortiaID, int shopLevel)
		{
			bool result = false;
			ConsortiaMgr.m_lock.AcquireWriterLock(15000);
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID) && ConsortiaMgr._consortia[consortiaID].IsExist)
				{
					ConsortiaMgr._consortia[consortiaID].ShopLevel = shopLevel;
				}
			}
			catch (Exception exception)
			{
				ConsortiaMgr.log.Error("ConsortiaUpGrade", exception);
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static bool ConsortiaSmithUpGrade(int consortiaID, int smithLevel)
		{
			bool result = false;
			ConsortiaMgr.m_lock.AcquireWriterLock(15000);
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID) && ConsortiaMgr._consortia[consortiaID].IsExist)
				{
					ConsortiaMgr._consortia[consortiaID].SmithLevel = smithLevel;
				}
			}
			catch (Exception exception)
			{
				ConsortiaMgr.log.Error("ConsortiaUpGrade", exception);
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static bool AddConsortia(int consortiaID)
		{
			bool result = false;
			ConsortiaMgr.m_lock.AcquireWriterLock(15000);
			try
			{
				if (!ConsortiaMgr._consortia.ContainsKey(consortiaID))
				{
					ConsortiaInfo consortiaInfo = new ConsortiaInfo();
					consortiaInfo.BuildDate = DateTime.Now;
					consortiaInfo.Level = 1;
					consortiaInfo.IsExist = true;
					consortiaInfo.ConsortiaName = "";
					consortiaInfo.ConsortiaID = consortiaID;
					ConsortiaMgr._consortia.Add(consortiaID, consortiaInfo);
				}
			}
			catch (Exception exception)
			{
				ConsortiaMgr.log.Error("ConsortiaUpGrade", exception);
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static ConsortiaInfo FindConsortiaInfo(int consortiaID)
		{
			ConsortiaMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID))
				{
					return ConsortiaMgr._consortia[consortiaID];
				}
			}
			catch
			{
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static int CanConsortiaFight(int consortiaID1, int consortiaID2)
		{
			if (consortiaID1 == 0 || consortiaID2 == 0 || consortiaID1 == consortiaID2)
			{
				return -1;
			}
			ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(consortiaID1);
			ConsortiaInfo consortiaInfo2 = ConsortiaMgr.FindConsortiaInfo(consortiaID2);
			if (consortiaInfo == null || consortiaInfo2 == null || consortiaInfo.Level < 3 || consortiaInfo2.Level < 3)
			{
				return -1;
			}
			return ConsortiaMgr.FindConsortiaAlly(consortiaID1, consortiaID2);
		}
		public static int FindConsortiaAlly(int cosortiaID1, int consortiaID2)
		{
			if (cosortiaID1 == 0 || consortiaID2 == 0 || cosortiaID1 == consortiaID2)
			{
				return -1;
			}
			string key;
			if (cosortiaID1 < consortiaID2)
			{
				key = cosortiaID1 + "&" + consortiaID2;
			}
			else
			{
				key = consortiaID2 + "&" + cosortiaID1;
			}
			ConsortiaMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (ConsortiaMgr._ally.ContainsKey(key))
				{
					return ConsortiaMgr._ally[key];
				}
			}
			catch
			{
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseReaderLock();
			}
			return 0;
		}
		public static int GetOffer(int cosortiaID1, int consortiaID2, eGameType gameType)
		{
			return ConsortiaMgr.GetOffer(ConsortiaMgr.FindConsortiaAlly(cosortiaID1, consortiaID2), gameType);
		}
		private static int GetOffer(int state, eGameType gameType)
		{
			switch (gameType)
			{
			case eGameType.Free:
				switch (state)
				{
				case 0:
					return 1;

				case 1:
					return 0;

				case 2:
					return 3;
				}
				break;

			case eGameType.Guild:
				switch (state)
				{
				case 0:
					return 5;

				case 1:
					return 0;

				case 2:
					return 10;
				}
				break;
			}
			return 0;
		}
		public static int KillPlayer(GamePlayer win, GamePlayer lose, Dictionary<GamePlayer, Player> players, eRoomType roomType, eGameType gameClass)
		{
			if (roomType != eRoomType.Match)
			{
				return -1;
			}
			int num = ConsortiaMgr.FindConsortiaAlly(win.PlayerCharacter.ConsortiaID, lose.PlayerCharacter.ConsortiaID);
			if (num == -1)
			{
				return num;
			}
			int offer = ConsortiaMgr.GetOffer(num, gameClass);
			if (lose.PlayerCharacter.Offer < offer)
			{
				offer = lose.PlayerCharacter.Offer;
			}
			if (offer != 0)
			{
				players[win].GainOffer = offer;
				players[lose].GainOffer = -offer;
			}
			return num;
		}
		public static int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int playercount)
		{
			if (roomType != eRoomType.Match)
			{
				return 0;
			}
			int num = playercount / 2;
			int num2 = 0;
			int state = 2;
			int num3 = 1;
			int num4 = 3;
			if (gameClass == eGameType.Guild)
			{
				num4 = 10;
				num3 = (int)RateMgr.GetRate(eRateType.Offer_Rate);
			}
			float rate = RateMgr.GetRate(eRateType.Riches_Rate);
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				if (gameClass == eGameType.Free)
				{
					num = 0;
				}
				else
				{
					consortiaBussiness.ConsortiaFight(consortiaWin, consortiaLose, num, out num2, state, totalKillHealth, rate);
				}
				foreach (KeyValuePair<int, Player> current in players)
				{
					if (current.Value != null)
					{
						if (current.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaWin)
						{
							current.Value.PlayerDetail.AddOffer((num + num4) * num3);
							current.Value.PlayerDetail.PlayerCharacter.RichesRob += num2;
						}
						else
						{
							if (current.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaLose)
							{
								current.Value.PlayerDetail.AddOffer((int)Math.Round((double)num * 0.5) * num3);
								current.Value.PlayerDetail.RemoveOffer(num4);
							}
						}
					}
				}
			}
			return num2;
		}
	}
}
