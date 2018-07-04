using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class RefineryMgr
	{
		private static Dictionary<int, RefineryInfo> m_Item_Refinery = new Dictionary<int, RefineryInfo>();
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ThreadSafeRandom rand = new ThreadSafeRandom();
		public static bool Init()
		{
			return RefineryMgr.Reload();
		}
		public static bool Reload()
		{
			try
			{
				Dictionary<int, RefineryInfo> dictionary = new Dictionary<int, RefineryInfo>();
				dictionary = RefineryMgr.LoadFromBD();
				if (dictionary.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, RefineryInfo>>(ref RefineryMgr.m_Item_Refinery, dictionary);
				}
				return true;
			}
			catch (Exception exception)
			{
				RefineryMgr.log.Error("NPCInfoMgr", exception);
			}
			return false;
		}
		public static Dictionary<int, RefineryInfo> LoadFromBD()
		{
			List<RefineryInfo> list = new List<RefineryInfo>();
			Dictionary<int, RefineryInfo> dictionary = new Dictionary<int, RefineryInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				list = produceBussiness.GetAllRefineryInfo();
				foreach (RefineryInfo current in list)
				{
					if (!dictionary.ContainsKey(current.RefineryID))
					{
						dictionary.Add(current.RefineryID, current);
					}
				}
			}
			return dictionary;
		}
		public static ItemTemplateInfo Refinery(GamePlayer player, List<ItemInfo> Items, ItemInfo Item, bool Luck, int OpertionType, ref bool result, ref int defaultprobability, ref bool IsFormula)
		{
			new ItemTemplateInfo();
			foreach (int current in RefineryMgr.m_Item_Refinery.Keys)
			{
				if (RefineryMgr.m_Item_Refinery[current].m_Equip.Contains(Item.TemplateID))
				{
					IsFormula = true;
					int num = 0;
					List<int> list = new List<int>();
					foreach (ItemInfo current2 in Items)
					{
						if (current2.TemplateID == RefineryMgr.m_Item_Refinery[current].Item1 && current2.Count >= RefineryMgr.m_Item_Refinery[current].Item1Count && !list.Contains(current2.TemplateID))
						{
							list.Add(current2.TemplateID);
							if (OpertionType != 0)
							{
								current2.Count -= RefineryMgr.m_Item_Refinery[current].Item1Count;
							}
							num++;
						}
						if (current2.TemplateID == RefineryMgr.m_Item_Refinery[current].Item2 && current2.Count >= RefineryMgr.m_Item_Refinery[current].Item2Count && !list.Contains(current2.TemplateID))
						{
							list.Add(current2.TemplateID);
							if (OpertionType != 0)
							{
								current2.Count -= RefineryMgr.m_Item_Refinery[current].Item2Count;
							}
							num++;
						}
						if (current2.TemplateID == RefineryMgr.m_Item_Refinery[current].Item3 && current2.Count >= RefineryMgr.m_Item_Refinery[current].Item3Count && !list.Contains(current2.TemplateID))
						{
							list.Add(current2.TemplateID);
							if (OpertionType != 0)
							{
								current2.Count -= RefineryMgr.m_Item_Refinery[current].Item3Count;
							}
							num++;
						}
					}
					if (num == 3)
					{
						for (int i = 0; i < RefineryMgr.m_Item_Refinery[current].m_Reward.Count; i++)
						{
							if (Items[Items.Count - 1].TemplateID == RefineryMgr.m_Item_Refinery[current].m_Reward[i])
							{
								if (Luck)
								{
									defaultprobability += 20;
								}
								if (OpertionType == 0)
								{
									int templateId = RefineryMgr.m_Item_Refinery[current].m_Reward[i + 1];
									ItemTemplateInfo result2 = ItemMgr.FindItemTemplate(templateId);
									return result2;
								}
								if (RefineryMgr.rand.Next(100) < defaultprobability)
								{
									int templateId = RefineryMgr.m_Item_Refinery[current].m_Reward[i + 1];
									result = true;
									ItemTemplateInfo result2 = ItemMgr.FindItemTemplate(templateId);
									return result2;
								}
							}
						}
					}
				}
				else
				{
					IsFormula = false;
				}
			}
			return null;
		}
		public static ItemTemplateInfo RefineryTrend(int Operation, ItemInfo Item, ref bool result)
		{
			if (Item != null)
			{
				foreach (int current in RefineryMgr.m_Item_Refinery.Keys)
				{
					if (RefineryMgr.m_Item_Refinery[current].m_Reward.Contains(Item.TemplateID))
					{
						for (int i = 0; i < RefineryMgr.m_Item_Refinery[current].m_Reward.Count; i++)
						{
							if (RefineryMgr.m_Item_Refinery[current].m_Reward[i] == Operation)
							{
								int templateId = RefineryMgr.m_Item_Refinery[current].m_Reward[i + 2];
								result = true;
								return ItemMgr.FindItemTemplate(templateId);
							}
						}
					}
				}
			}
			return null;
		}
	}
}
