using Bussiness;
using Bussiness.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Server.Managers
{
	public class FusionMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<string, FusionInfo> _fusions;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom rand;
		public static bool ReLoad()
		{
			try
			{
				Dictionary<string, FusionInfo> dictionary = new Dictionary<string, FusionInfo>();
				if (FusionMgr.LoadFusion(dictionary))
				{
					FusionMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						FusionMgr._fusions = dictionary;
						return true;
					}
					catch
					{
					}
					finally
					{
						FusionMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (FusionMgr.log.IsErrorEnabled)
				{
					FusionMgr.log.Error("FusionMgr", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				FusionMgr.m_lock = new ReaderWriterLock();
				FusionMgr._fusions = new Dictionary<string, FusionInfo>();
				FusionMgr.rand = new ThreadSafeRandom();
				result = FusionMgr.LoadFusion(FusionMgr._fusions);
			}
			catch (Exception exception)
			{
				if (FusionMgr.log.IsErrorEnabled)
				{
					FusionMgr.log.Error("FusionMgr", exception);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadFusion(Dictionary<string, FusionInfo> fusion)
		{
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				FusionInfo[] allFusion = produceBussiness.GetAllFusion();
				FusionInfo[] array = allFusion;
				for (int i = 0; i < array.Length; i++)
				{
					FusionInfo fusionInfo = array[i];
					List<int> list = new List<int>();
					list.Add(fusionInfo.Item1);
					list.Add(fusionInfo.Item2);
					list.Add(fusionInfo.Item3);
					list.Add(fusionInfo.Item4);
					list.Sort();
					StringBuilder stringBuilder = new StringBuilder();
					foreach (int current in list)
					{
						if (current != 0)
						{
							stringBuilder.Append(current);
						}
					}
					string key = stringBuilder.ToString();
					if (!fusion.ContainsKey(key))
					{
						fusion.Add(key, fusionInfo);
					}
				}
			}
			return true;
		}
		public static ItemTemplateInfo Fusion(List<ItemInfo> Items, List<ItemInfo> AppendItems, ref bool isBind, ref bool result)
		{
			List<int> list = new List<int>();
			int num = 0;
			int TotalRate = 0;
			ItemTemplateInfo itemTemplateInfo = null;
			foreach (ItemInfo current in Items)
			{
				list.Add(current.Template.FusionType);
				if (current.Template.Level > num)
				{
					num = current.Template.Level;
				}
				TotalRate += current.Template.FusionRate;
				if (current.IsBinds)
				{
					isBind = true;
				}
			}
			foreach (ItemInfo current2 in AppendItems)
			{
				TotalRate += current2.Template.FusionRate / 2;
				if (current2.IsBinds)
				{
					isBind = true;
				}
			}
			list.Sort();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int current3 in list)
			{
				stringBuilder.Append(current3);
			}
			string key = stringBuilder.ToString();
			FusionMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				if (FusionMgr._fusions.ContainsKey(key))
				{
					FusionInfo fusionInfo = FusionMgr._fusions[key];
					ItemTemplateInfo goodsbyFusionTypeandLevel = ItemMgr.GetGoodsbyFusionTypeandLevel(fusionInfo.Reward, num);
					ItemTemplateInfo goodsbyFusionTypeandLevel2 = ItemMgr.GetGoodsbyFusionTypeandLevel(fusionInfo.Reward, num + 1);
					ItemTemplateInfo goodsbyFusionTypeandLevel3 = ItemMgr.GetGoodsbyFusionTypeandLevel(fusionInfo.Reward, num + 2);
					List<ItemTemplateInfo> list2 = new List<ItemTemplateInfo>();
					if (goodsbyFusionTypeandLevel3 != null)
					{
						list2.Add(goodsbyFusionTypeandLevel3);
					}
					if (goodsbyFusionTypeandLevel2 != null)
					{
						list2.Add(goodsbyFusionTypeandLevel2);
					}
					if (goodsbyFusionTypeandLevel != null)
					{
						list2.Add(goodsbyFusionTypeandLevel);
					}
					ItemTemplateInfo itemTemplateInfo2 = (
						from s in list2
						where (double)TotalRate / (double)s.FusionNeedRate <= 1.1
						orderby (double)TotalRate / (double)s.FusionNeedRate descending
						select s).FirstOrDefault<ItemTemplateInfo>();
					ItemTemplateInfo itemTemplateInfo3 = (
						from s in list2
						where (double)TotalRate / (double)s.FusionNeedRate > 1.1
						orderby (double)TotalRate / (double)s.FusionNeedRate
						select s).FirstOrDefault<ItemTemplateInfo>();
					if (itemTemplateInfo2 != null && itemTemplateInfo3 == null)
					{
						itemTemplateInfo = itemTemplateInfo2;
						if ((double)(100 * TotalRate) / (double)itemTemplateInfo2.FusionNeedRate > (double)FusionMgr.rand.Next(100))
						{
							result = true;
						}
					}
					if (itemTemplateInfo2 != null && itemTemplateInfo3 != null)
					{
						if (itemTemplateInfo2.Level - itemTemplateInfo3.Level == 2)
						{
							double num2 = (double)(100 * TotalRate) * 0.6 / (double)itemTemplateInfo2.FusionNeedRate;
						}
						else
						{
							double num2 = (double)(100 * TotalRate) / (double)itemTemplateInfo2.FusionNeedRate;
						}
						if ((double)(100 * TotalRate) / (double)itemTemplateInfo2.FusionNeedRate > (double)FusionMgr.rand.Next(100))
						{
							itemTemplateInfo = itemTemplateInfo2;
							result = true;
						}
						else
						{
							itemTemplateInfo = itemTemplateInfo3;
							result = true;
						}
					}
					if (itemTemplateInfo2 == null && itemTemplateInfo3 != null)
					{
						itemTemplateInfo = itemTemplateInfo3;
						result = true;
					}
					if (result)
					{
						foreach (ItemInfo current4 in Items)
						{
							if (current4.Template.TemplateID == itemTemplateInfo.TemplateID)
							{
								result = false;
								break;
							}
						}
					}
					return itemTemplateInfo;
				}
			}
			catch
			{
			}
			finally
			{
				FusionMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
		public static Dictionary<int, double> FusionPreview(List<ItemInfo> Items, List<ItemInfo> AppendItems, ref bool isBind)
		{
			List<int> list = new List<int>();
			int num = 0;
			int TotalRate = 0;
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			dictionary.Clear();
			foreach (ItemInfo current in Items)
			{
				list.Add(current.Template.FusionType);
				if (current.Template.Level > num)
				{
					num = current.Template.Level;
				}
				TotalRate += current.Template.FusionRate;
				if (current.IsBinds)
				{
					isBind = true;
				}
			}
			foreach (ItemInfo current2 in AppendItems)
			{
				TotalRate += current2.Template.FusionRate / 2;
				if (current2.IsBinds)
				{
					isBind = true;
				}
			}
			list.Sort();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int current3 in list)
			{
				stringBuilder.Append(current3);
			}
			string key = stringBuilder.ToString();
			FusionMgr.m_lock.AcquireReaderLock(15000);
			try
			{
				Dictionary<int, double> result;
				if (FusionMgr._fusions.ContainsKey(key))
				{
					FusionInfo fusionInfo = FusionMgr._fusions[key];
					ItemTemplateInfo goodsbyFusionTypeandLevel = ItemMgr.GetGoodsbyFusionTypeandLevel(fusionInfo.Reward, num);
					ItemTemplateInfo goodsbyFusionTypeandLevel2 = ItemMgr.GetGoodsbyFusionTypeandLevel(fusionInfo.Reward, num + 1);
					ItemTemplateInfo goodsbyFusionTypeandLevel3 = ItemMgr.GetGoodsbyFusionTypeandLevel(fusionInfo.Reward, num + 2);
					List<ItemTemplateInfo> list2 = new List<ItemTemplateInfo>();
					if (goodsbyFusionTypeandLevel3 != null)
					{
						list2.Add(goodsbyFusionTypeandLevel3);
					}
					if (goodsbyFusionTypeandLevel2 != null)
					{
						list2.Add(goodsbyFusionTypeandLevel2);
					}
					if (goodsbyFusionTypeandLevel != null)
					{
						list2.Add(goodsbyFusionTypeandLevel);
					}
					ItemTemplateInfo itemTemplateInfo = (
						from s in list2
						where (double)TotalRate / (double)s.FusionNeedRate <= 1.1
						orderby (double)TotalRate / (double)s.FusionNeedRate descending
						select s).FirstOrDefault<ItemTemplateInfo>();
					ItemTemplateInfo itemTemplateInfo2 = (
						from s in list2
						where (double)TotalRate / (double)s.FusionNeedRate > 1.1
						orderby (double)TotalRate / (double)s.FusionNeedRate
						select s).FirstOrDefault<ItemTemplateInfo>();
					if (itemTemplateInfo != null && itemTemplateInfo2 == null)
					{
						dictionary.Add(itemTemplateInfo.TemplateID, (double)(100 * TotalRate) / (double)itemTemplateInfo.FusionNeedRate);
					}
					if (itemTemplateInfo != null && itemTemplateInfo2 != null)
					{
						double num2;
						double value;
						if (itemTemplateInfo.Level - itemTemplateInfo2.Level == 2)
						{
							num2 = (double)(100 * TotalRate) * 0.6 / (double)itemTemplateInfo.FusionNeedRate;
							value = 100.0 - num2;
						}
						else
						{
							num2 = (double)(100 * TotalRate) / (double)itemTemplateInfo.FusionNeedRate;
							value = 100.0 - num2;
						}
						dictionary.Add(itemTemplateInfo.TemplateID, num2);
						dictionary.Add(itemTemplateInfo2.TemplateID, value);
					}
					if (itemTemplateInfo == null && itemTemplateInfo2 != null)
					{
						dictionary.Add(itemTemplateInfo2.TemplateID, 100.0);
					}
					int[] array = dictionary.Keys.ToArray<int>();
					int[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						int num3 = array2[i];
						foreach (ItemInfo current4 in Items)
						{
							if (num3 == current4.Template.TemplateID && dictionary.ContainsKey(num3))
							{
								dictionary.Remove(num3);
							}
						}
					}
					result = dictionary;
					return result;
				}
				result = dictionary;
				return result;
			}
			catch
			{
			}
			finally
			{
				FusionMgr.m_lock.ReleaseReaderLock();
			}
			return null;
		}
	}
}
