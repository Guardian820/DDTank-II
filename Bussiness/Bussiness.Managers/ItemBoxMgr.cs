using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class ItemBoxMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ItemBoxInfo[] m_itemBox;
		private static Dictionary<int, List<ItemBoxInfo>> m_itemBoxs;
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		public static bool ReLoad()
		{
			try
			{
				ItemBoxInfo[] array = ItemBoxMgr.LoadItemBoxDb();
				Dictionary<int, List<ItemBoxInfo>> value = ItemBoxMgr.LoadItemBoxs(array);
				if (array != null)
				{
					Interlocked.Exchange<ItemBoxInfo[]>(ref ItemBoxMgr.m_itemBox, array);
					Interlocked.Exchange<Dictionary<int, List<ItemBoxInfo>>>(ref ItemBoxMgr.m_itemBoxs, value);
				}
			}
			catch (Exception exception)
			{
				if (ItemBoxMgr.log.IsErrorEnabled)
				{
					ItemBoxMgr.log.Error("ReLoad", exception);
				}
				return false;
			}
			return true;
		}
		public static bool Init()
		{
			return ItemBoxMgr.ReLoad();
		}
		public static ItemBoxInfo[] LoadItemBoxDb()
		{
			ItemBoxInfo[] result;
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				ItemBoxInfo[] itemBoxInfos = produceBussiness.GetItemBoxInfos();
				result = itemBoxInfos;
			}
			return result;
		}
		public static Dictionary<int, List<ItemBoxInfo>> LoadItemBoxs(ItemBoxInfo[] itemBoxs)
		{
			Dictionary<int, List<ItemBoxInfo>> dictionary = new Dictionary<int, List<ItemBoxInfo>>();
			ItemBoxInfo info;
			for (int i = 0; i < itemBoxs.Length; i++)
			{
				info = itemBoxs[i];
				if (!dictionary.Keys.Contains(info.DataId))
				{
					IEnumerable<ItemBoxInfo> source = 
						from s in itemBoxs
						where s.DataId == info.DataId
						select s;
					dictionary.Add(info.DataId, source.ToList<ItemBoxInfo>());
				}
			}
			return dictionary;
		}
		public static List<ItemBoxInfo> FindItemBox(int DataId)
		{
			if (ItemBoxMgr.m_itemBoxs.ContainsKey(DataId))
			{
				return ItemBoxMgr.m_itemBoxs[DataId];
			}
			return null;
		}
		public static List<ItemInfo> GetItemBoxAward(int DataId)
		{
			List<ItemBoxInfo> list = ItemBoxMgr.FindItemBox(DataId);
			List<ItemInfo> list2 = new List<ItemInfo>();
			foreach (ItemBoxInfo current in list)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(current.TemplateId), current.ItemCount, 105);
				itemInfo.IsBinds = current.IsBind;
				itemInfo.ValidDate = current.ItemValid;
				list2.Add(itemInfo);
			}
			return list2;
		}
		public static List<ItemBoxInfo> FindItemBoxAward(int DataId)
		{
			List<ItemBoxInfo> list = ItemBoxMgr.FindItemBox(DataId);
			List<ItemBoxInfo> list2 = new List<ItemBoxInfo>();
			foreach (ItemBoxInfo current in list)
			{
				if (current.TemplateId > 0)
				{
					list2.Add(current);
				}
				else
				{
					int templateId = current.TemplateId;
					if (templateId <= -300)
					{
						if (templateId != -1100)
						{
							if (templateId == -300)
							{
								current.TemplateId = 11420;
								current.ItemCount = 1;
							}
						}
						else
						{
							current.TemplateId = 11213;
							current.ItemCount = 1;
						}
					}
					else
					{
						if (templateId != -200)
						{
							if (templateId != -100)
							{
								if (templateId == 11408)
								{
									current.TemplateId = 11420;
									current.ItemCount = 1;
								}
							}
							else
							{
								current.TemplateId = 11233;
								current.ItemCount = 1;
							}
						}
						else
						{
							current.TemplateId = 112244;
							current.ItemCount = 1;
						}
					}
					list2.Add(current);
				}
			}
			return list2;
		}
		public static bool CreateItemBox(int DateId, List<ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int medal)
		{
			List<ItemBoxInfo> list = new List<ItemBoxInfo>();
			List<ItemBoxInfo> list2 = ItemBoxMgr.FindItemBox(DateId);
			if (list2 == null)
			{
				return false;
			}
			list = (
				from s in list2
				where s.IsSelect
				select s).ToList<ItemBoxInfo>();
			int num = 1;
			int maxRound = 0;
			foreach (ItemBoxInfo current in list2)
			{
				if (!current.IsSelect || maxRound >= current.Random)
				{
					maxRound = current.Random;
				}
			}
			maxRound = ItemBoxMgr.random.Next(maxRound);
			List<ItemBoxInfo> list3 = (
				from s in list2
				where !s.IsSelect && s.Random >= maxRound
				select s).ToList<ItemBoxInfo>();
			int num2 = list3.Count<ItemBoxInfo>();
			if (num2 > 0)
			{
				num = ((num > num2) ? num2 : num);
				int[] randomUnrepeatArray = ItemBoxMgr.GetRandomUnrepeatArray(0, num2 - 1, num);
				int[] array = randomUnrepeatArray;
				for (int i = 0; i < array.Length; i++)
				{
					int index = array[i];
					ItemBoxInfo item = list3[index];
					if (list == null)
					{
						list = new List<ItemBoxInfo>();
					}
					list.Add(item);
				}
			}
			foreach (ItemBoxInfo current2 in list)
			{
				if (current2 == null)
				{
					return false;
				}
				int templateId = current2.TemplateId;
				if (templateId <= -800)
				{
					if (templateId <= -1000)
					{
						if (templateId == -1100)
						{
							giftToken += current2.ItemCount;
							continue;
						}
						if (templateId == -1000)
						{
							continue;
						}
					}
					else
					{
						if (templateId == -900 || templateId == -800)
						{
							continue;
						}
					}
				}
				else
				{
					if (templateId <= -200)
					{
						if (templateId == -300)
						{
							medal += current2.ItemCount;
							continue;
						}
						if (templateId == -200)
						{
							point += current2.ItemCount;
							continue;
						}
					}
					else
					{
						if (templateId == -100)
						{
							gold += current2.ItemCount;
							continue;
						}
						if (templateId == 11408)
						{
							medal += current2.ItemCount;
							continue;
						}
					}
				}
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(current2.TemplateId);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, current2.ItemCount, 101);
				if (itemInfo != null)
				{
					itemInfo.Count = current2.ItemCount;
					itemInfo.IsBinds = current2.IsBind;
					itemInfo.ValidDate = current2.ItemValid;
					itemInfo.StrengthenLevel = current2.StrengthenLevel;
					itemInfo.AttackCompose = current2.AttackCompose;
					itemInfo.DefendCompose = current2.DefendCompose;
					itemInfo.AgilityCompose = current2.AgilityCompose;
					itemInfo.LuckCompose = current2.LuckCompose;
					if (itemInfos == null)
					{
						itemInfos = new List<ItemInfo>();
					}
					itemInfos.Add(itemInfo);
				}
			}
			return true;
		}
		public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				int num = ItemBoxMgr.random.Next(minValue, maxValue + 1);
				int num2 = 0;
				for (int j = 0; j < i; j++)
				{
					if (array[j] == num)
					{
						num2++;
					}
				}
				if (num2 == 0)
				{
					array[i] = num;
				}
				else
				{
					i--;
				}
			}
			return array;
		}
	}
}
