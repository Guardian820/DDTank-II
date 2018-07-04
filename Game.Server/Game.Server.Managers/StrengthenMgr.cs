using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class StrengthenMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, StrengthenInfo> _strengthens;
		private static Dictionary<int, StrengthenInfo> _Refinery_Strengthens;
		private static Dictionary<int, StrengthenGoodsInfo> _Strengthens_Goods;
		private static Dictionary<int, StrengThenExpInfo> _Strengthens_Exps;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom rand;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, StrengthenInfo> dictionary = new Dictionary<int, StrengthenInfo>();
				Dictionary<int, StrengthenInfo> dictionary2 = new Dictionary<int, StrengthenInfo>();
				Dictionary<int, StrengThenExpInfo> dictionary3 = new Dictionary<int, StrengThenExpInfo>();
				Dictionary<int, StrengthenGoodsInfo> dictionary4 = new Dictionary<int, StrengthenGoodsInfo>();
				if (StrengthenMgr.LoadStrengthen(dictionary, dictionary2, dictionary3, dictionary4))
				{
					StrengthenMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						StrengthenMgr._strengthens = dictionary;
						StrengthenMgr._Refinery_Strengthens = dictionary2;
						StrengthenMgr._Strengthens_Exps = dictionary3;
						StrengthenMgr._Strengthens_Goods = dictionary4;
						return true;
					}
					catch
					{
					}
					finally
					{
						StrengthenMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (StrengthenMgr.log.IsErrorEnabled)
				{
					StrengthenMgr.log.Error("StrengthenMgr", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				StrengthenMgr.m_lock = new ReaderWriterLock();
				StrengthenMgr._strengthens = new Dictionary<int, StrengthenInfo>();
				StrengthenMgr._Refinery_Strengthens = new Dictionary<int, StrengthenInfo>();
				StrengthenMgr._Strengthens_Goods = new Dictionary<int, StrengthenGoodsInfo>();
				StrengthenMgr._Strengthens_Exps = new Dictionary<int, StrengThenExpInfo>();
				StrengthenMgr.rand = new ThreadSafeRandom();
				result = StrengthenMgr.LoadStrengthen(StrengthenMgr._strengthens, StrengthenMgr._Refinery_Strengthens, StrengthenMgr._Strengthens_Exps, StrengthenMgr._Strengthens_Goods);
			}
			catch (Exception exception)
			{
				if (StrengthenMgr.log.IsErrorEnabled)
				{
					StrengthenMgr.log.Error("StrengthenMgr", exception);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadStrengthen(Dictionary<int, StrengthenInfo> strengthen, Dictionary<int, StrengthenInfo> RefineryStrengthen, Dictionary<int, StrengThenExpInfo> StrengthenExp, Dictionary<int, StrengthenGoodsInfo> StrengthensGoods)
		{
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				StrengthenInfo[] allStrengthen = produceBussiness.GetAllStrengthen();
				StrengthenInfo[] allRefineryStrengthen = produceBussiness.GetAllRefineryStrengthen();
				StrengThenExpInfo[] allStrengThenExp = produceBussiness.GetAllStrengThenExp();
				StrengthenGoodsInfo[] allStrengthenGoodsInfo = produceBussiness.GetAllStrengthenGoodsInfo();
				StrengthenInfo[] array = allStrengthen;
				for (int i = 0; i < array.Length; i++)
				{
					StrengthenInfo strengthenInfo = array[i];
					if (!strengthen.ContainsKey(strengthenInfo.StrengthenLevel))
					{
						strengthen.Add(strengthenInfo.StrengthenLevel, strengthenInfo);
					}
				}
				StrengthenInfo[] array2 = allRefineryStrengthen;
				for (int j = 0; j < array2.Length; j++)
				{
					StrengthenInfo strengthenInfo2 = array2[j];
					if (!RefineryStrengthen.ContainsKey(strengthenInfo2.StrengthenLevel))
					{
						RefineryStrengthen.Add(strengthenInfo2.StrengthenLevel, strengthenInfo2);
					}
				}
				StrengThenExpInfo[] array3 = allStrengThenExp;
				for (int k = 0; k < array3.Length; k++)
				{
					StrengThenExpInfo strengThenExpInfo = array3[k];
					if (!StrengthenExp.ContainsKey(strengThenExpInfo.Lv))
					{
						StrengthenExp.Add(strengThenExpInfo.Lv, strengThenExpInfo);
					}
				}
				StrengthenGoodsInfo[] array4 = allStrengthenGoodsInfo;
				for (int l = 0; l < array4.Length; l++)
				{
					StrengthenGoodsInfo strengthenGoodsInfo = array4[l];
					if (!StrengthensGoods.ContainsKey(strengthenGoodsInfo.ID))
					{
						StrengthensGoods.Add(strengthenGoodsInfo.ID, strengthenGoodsInfo);
					}
				}
			}
			return true;
		}
		public static StrengThenExpInfo FindStrengthenExpInfo(int level)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (StrengthenMgr._strengthens.ContainsKey(level))
				{
					return StrengthenMgr._Strengthens_Exps[level];
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static bool canUpLv(int exp, int level)
		{
			StrengThenExpInfo strengThenExpInfo = StrengthenMgr.FindStrengthenExpInfo(level + 1);
			return strengThenExpInfo != null && exp >= strengThenExpInfo.Exp;
		}
		public static int getNeedExp(int Exp, int level)
		{
			StrengThenExpInfo strengThenExpInfo = StrengthenMgr.FindStrengthenExpInfo(level + 1);
			if (strengThenExpInfo == null)
			{
				return 0;
			}
			return strengThenExpInfo.Exp - Exp;
		}
		public static StrengthenInfo FindStrengthenInfo(int level)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (StrengthenMgr._strengthens.ContainsKey(level))
				{
					return StrengthenMgr._strengthens[level];
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static StrengthenInfo FindRefineryStrengthenInfo(int level)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (StrengthenMgr._Refinery_Strengthens.ContainsKey(level))
				{
					return StrengthenMgr._Refinery_Strengthens[level];
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static StrengthenGoodsInfo FindStrengthenGoodsInfo(int level, int TemplateId)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				foreach (int current in StrengthenMgr._Strengthens_Goods.Keys)
				{
					if (StrengthenMgr._Strengthens_Goods[current].Level == level && (TemplateId == StrengthenMgr._Strengthens_Goods[current].CurrentEquip || TemplateId == StrengthenMgr._Strengthens_Goods[current].GainEquip))
					{
						return StrengthenMgr._Strengthens_Goods[current];
					}
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static void InheritProperty(ItemInfo Item, ref ItemInfo item)
		{
			if (Item.Hole1 >= 0)
			{
				item.Hole1 = Item.Hole1;
			}
			if (Item.Hole2 >= 0)
			{
				item.Hole2 = Item.Hole2;
			}
			if (Item.Hole3 >= 0)
			{
				item.Hole3 = Item.Hole3;
			}
			if (Item.Hole4 >= 0)
			{
				item.Hole4 = Item.Hole4;
			}
			if (Item.Hole5 >= 0)
			{
				item.Hole5 = Item.Hole5;
			}
			if (Item.Hole6 >= 0)
			{
				item.Hole6 = Item.Hole6;
			}
			item.AttackCompose = Item.AttackCompose;
			item.DefendCompose = Item.DefendCompose;
			item.LuckCompose = Item.LuckCompose;
			item.AgilityCompose = Item.AgilityCompose;
			item.IsBinds = Item.IsBinds;
			item.ValidDate = Item.ValidDate;
		}
		public static void InheritTransferProperty(ref ItemInfo item0, ref ItemInfo item1, bool tranHole, bool tranHoleFivSix)
		{
			int hole = item0.Hole1;
			int hole2 = item0.Hole2;
			int hole3 = item0.Hole3;
			int hole4 = item0.Hole4;
			int hole5 = item0.Hole5;
			int hole6 = item0.Hole6;
			int hole5Exp = item0.Hole5Exp;
			int hole5Level = item0.Hole5Level;
			int hole6Exp = item0.Hole6Exp;
			int hole6Level = item0.Hole6Level;
			int attackCompose = item0.AttackCompose;
			int defendCompose = item0.DefendCompose;
			int agilityCompose = item0.AgilityCompose;
			int luckCompose = item0.LuckCompose;
			int strengthenLevel = item0.StrengthenLevel;
			int strengthenExp = item0.StrengthenExp;
			bool isGold = item0.IsGold;
			int goldValidDate = item0.goldValidDate;
			DateTime goldBeginTime = item0.goldBeginTime;
			string latentEnergyCurStr = item0.latentEnergyCurStr;
			string latentEnergyNewStr = item0.latentEnergyNewStr;
			DateTime latentEnergyEndTime = item0.latentEnergyEndTime;
			int hole7 = item1.Hole1;
			int hole8 = item1.Hole2;
			int hole9 = item1.Hole3;
			int hole10 = item1.Hole4;
			int hole11 = item1.Hole5;
			int hole12 = item1.Hole6;
			int hole5Exp2 = item1.Hole5Exp;
			int hole5Level2 = item1.Hole5Level;
			int hole6Exp2 = item1.Hole6Exp;
			int hole6Level2 = item1.Hole6Level;
			int attackCompose2 = item1.AttackCompose;
			int defendCompose2 = item1.DefendCompose;
			int agilityCompose2 = item1.AgilityCompose;
			int luckCompose2 = item1.LuckCompose;
			int strengthenLevel2 = item1.StrengthenLevel;
			int strengthenExp2 = item1.StrengthenExp;
			bool isGold2 = item1.IsGold;
			int goldValidDate2 = item1.goldValidDate;
			DateTime goldBeginTime2 = item1.goldBeginTime;
			string latentEnergyCurStr2 = item1.latentEnergyCurStr;
			string latentEnergyNewStr2 = item1.latentEnergyNewStr;
			DateTime latentEnergyEndTime2 = item1.latentEnergyEndTime;
			if (tranHole)
			{
				if (item0.Hole1 >= 0 || item1.Hole1 >= 0)
				{
					item1.Hole1 = hole;
				}
				item0.Hole1 = hole7;
				if (item0.Hole2 >= 0 || item1.Hole2 >= 0)
				{
					item1.Hole2 = hole2;
				}
				item0.Hole2 = hole8;
				if (item0.Hole3 >= 0 || item1.Hole3 >= 0)
				{
					item1.Hole3 = hole3;
				}
				item0.Hole3 = hole9;
				if (item0.Hole4 >= 0 || item1.Hole4 >= 0)
				{
					item1.Hole4 = hole4;
				}
				item0.Hole4 = hole10;
			}
			if (tranHoleFivSix)
			{
				if (item0.Hole5 >= 0 || item1.Hole5 >= 0)
				{
					item1.Hole5 = hole5;
				}
				item0.Hole5 = hole11;
				if (item0.Hole6 >= 0 || item1.Hole6 >= 0)
				{
					item1.Hole6 = hole6;
				}
				item0.Hole6 = hole12;
			}
			item1.Hole5Exp = hole5Exp;
			item0.Hole5Exp = hole5Exp2;
			item1.Hole5Level = hole5Level;
			item0.Hole5Level = hole5Level2;
			item1.Hole6Exp = hole6Exp;
			item0.Hole6Exp = hole6Exp2;
			item1.Hole6Level = hole6Level;
			item0.Hole6Level = hole6Level2;
			item0.StrengthenLevel = strengthenLevel2;
			item1.StrengthenLevel = strengthenLevel;
			item0.StrengthenExp = strengthenExp2;
			item1.StrengthenExp = strengthenExp;
			item0.AttackCompose = attackCompose2;
			item1.AttackCompose = attackCompose;
			item0.DefendCompose = defendCompose2;
			item1.DefendCompose = defendCompose;
			item0.LuckCompose = luckCompose2;
			item1.LuckCompose = luckCompose;
			item0.AgilityCompose = agilityCompose2;
			item1.AgilityCompose = agilityCompose;
			if (item0.IsBinds)
			{
				item1.IsBinds = item0.IsBinds;
			}
			if (item1.IsBinds)
			{
				item0.IsBinds = item1.IsBinds;
			}
			item0.IsGold = isGold2;
			item1.IsGold = isGold;
			item0.goldBeginTime = goldBeginTime2;
			item1.goldBeginTime = goldBeginTime;
			item0.goldValidDate = goldValidDate2;
			item1.goldValidDate = goldValidDate;
			item0.latentEnergyCurStr = latentEnergyCurStr2;
			item1.latentEnergyCurStr = latentEnergyCurStr;
			item0.latentEnergyNewStr = latentEnergyNewStr2;
			item1.latentEnergyNewStr = latentEnergyNewStr;
			item0.latentEnergyEndTime = latentEnergyEndTime2;
			item1.latentEnergyEndTime = latentEnergyEndTime;
		}
	}
}
