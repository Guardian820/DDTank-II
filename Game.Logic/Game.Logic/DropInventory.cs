using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Game.Logic
{
	public class DropInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static int roundDate = 0;
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		public static List<ItemInfo> CopySystemDrop(int copyId, int OpenCount)
		{
			int num = Convert.ToInt32((double)OpenCount * 0.1);
			int num2 = Convert.ToInt32((double)OpenCount * 0.3);
			int num3 = OpenCount - num - num2;
			List<ItemInfo> list = new List<ItemInfo>();
			List<ItemInfo> list2 = null;
			int dropCondiction = DropInventory.GetDropCondiction(eDropType.Copy, copyId.ToString(), "2");
			if (dropCondiction > 0)
			{
				for (int i = 0; i < num; i++)
				{
					if (DropInventory.GetDropItems(eDropType.Copy, dropCondiction, ref list2))
					{
						list.Add(list2[0]);
						list2 = null;
					}
				}
			}
			int dropCondiction2 = DropInventory.GetDropCondiction(eDropType.Copy, copyId.ToString(), "3");
			if (dropCondiction2 > 0)
			{
				for (int i = 0; i < num2; i++)
				{
					if (DropInventory.GetDropItems(eDropType.Copy, dropCondiction2, ref list2))
					{
						list.Add(list2[0]);
						list2 = null;
					}
				}
			}
			int dropCondiction3 = DropInventory.GetDropCondiction(eDropType.Copy, copyId.ToString(), "4");
			if (dropCondiction3 > 0)
			{
				for (int i = 0; i < num3; i++)
				{
					if (DropInventory.GetDropItems(eDropType.Copy, dropCondiction3, ref list2))
					{
						list.Add(list2[0]);
						list2 = null;
					}
				}
			}
			return DropInventory.RandomSortList(list);
		}
		public static List<ItemInfo> RandomSortList(List<ItemInfo> list)
		{
			return (
				from key in list
				orderby DropInventory.random.Next()
				select key).ToList<ItemInfo>();
		}
		public static bool CardDrop(eRoomType e, ref List<ItemInfo> info)
		{
			eDropType arg_0F_0 = eDropType.Cards;
			int num = (int)e;
			int dropCondiction = DropInventory.GetDropCondiction(arg_0F_0, num.ToString(), "0");
			if (dropCondiction > 0)
			{
				List<ItemInfo> list = null;
				if (DropInventory.GetDropItems(eDropType.Cards, dropCondiction, ref list))
				{
					info = ((list != null) ? list : null);
					return true;
				}
			}
			return false;
		}
		public static bool BoxDrop(eRoomType e, ref List<ItemInfo> info)
		{
			eDropType arg_0F_0 = eDropType.Box;
			int num = (int)e;
			int dropCondiction = DropInventory.GetDropCondiction(arg_0F_0, num.ToString(), "0");
			if (dropCondiction > 0)
			{
				List<ItemInfo> list = null;
				if (DropInventory.GetDropItems(eDropType.Box, dropCondiction, ref list))
				{
					info = ((list != null) ? list : null);
					return true;
				}
			}
			return false;
		}
		public static bool NPCDrop(int dropId, ref List<ItemInfo> info)
		{
			if (dropId > 0)
			{
				List<ItemInfo> list = null;
				if (DropInventory.GetDropItems(eDropType.NPC, dropId, ref list))
				{
					info = ((list != null) ? list : null);
					return true;
				}
			}
			return false;
		}
		public static bool BossDrop(int missionId, ref List<ItemInfo> info)
		{
			int dropCondiction = DropInventory.GetDropCondiction(eDropType.Boss, missionId.ToString(), "0");
			if (dropCondiction > 0)
			{
				List<ItemInfo> list = null;
				if (DropInventory.GetDropItems(eDropType.Boss, dropCondiction, ref list))
				{
					info = ((list != null) ? list : null);
					return true;
				}
			}
			return false;
		}
		public static bool CopyDrop(int copyId, int user, ref List<ItemInfo> info)
		{
			int dropCondiction = DropInventory.GetDropCondiction(eDropType.Copy, copyId.ToString(), user.ToString());
			if (dropCondiction > 0)
			{
				List<ItemInfo> list = null;
				if (DropInventory.GetDropItems(eDropType.Copy, dropCondiction, ref list))
				{
					info = ((list != null) ? list : null);
					return true;
				}
			}
			return false;
		}
		public static bool SpecialDrop(int missionId, int boxType, ref List<ItemInfo> info)
		{
			int dropCondiction = DropInventory.GetDropCondiction(eDropType.Special, missionId.ToString(), boxType.ToString());
			if (dropCondiction > 0)
			{
				List<ItemInfo> list = null;
				if (DropInventory.GetDropItems(eDropType.Special, dropCondiction, ref list))
				{
					info = ((list != null) ? list : null);
					return true;
				}
			}
			return false;
		}
		public static bool PvPQuestsDrop(eRoomType e, bool playResult, ref List<ItemInfo> info)
		{
			eDropType arg_18_0 = eDropType.PvpQuests;
			int num = (int)e;
			int dropCondiction = DropInventory.GetDropCondiction(arg_18_0, num.ToString(), Convert.ToInt16(playResult).ToString());
			if (dropCondiction > 0)
			{
				List<ItemInfo> list = null;
				if (DropInventory.GetDropItems(eDropType.PvpQuests, dropCondiction, ref list))
				{
					info = ((list != null) ? list : null);
					return true;
				}
			}
			return false;
		}
		public static bool FireDrop(eRoomType e, ref List<ItemInfo> info)
		{
			eDropType arg_0F_0 = eDropType.Fire;
			int num = (int)e;
			int dropCondiction = DropInventory.GetDropCondiction(arg_0F_0, num.ToString(), "0");
			if (dropCondiction > 0)
			{
				List<ItemInfo> list = null;
				if (DropInventory.GetDropItems(eDropType.Fire, dropCondiction, ref list))
				{
					info = ((list != null) ? list : null);
					return true;
				}
			}
			return false;
		}
		public static bool PvEQuestsDrop(int npcId, ref List<ItemInfo> info)
		{
			int dropCondiction = DropInventory.GetDropCondiction(eDropType.PveQuests, npcId.ToString(), "0");
			if (dropCondiction > 0)
			{
				List<ItemInfo> list = null;
				if (DropInventory.GetDropItems(eDropType.PveQuests, dropCondiction, ref list))
				{
					info = ((list != null) ? list : null);
					return true;
				}
			}
			return false;
		}
		public static bool AnswerDrop(int answerId, ref List<ItemInfo> info)
		{
			int dropCondiction = DropInventory.GetDropCondiction(eDropType.Answer, answerId.ToString(), "0");
			if (dropCondiction > 0 && dropCondiction > 0)
			{
				List<ItemInfo> list = null;
				if (DropInventory.GetDropItems(eDropType.Answer, dropCondiction, ref list))
				{
					info = ((list != null) ? list : null);
					return true;
				}
			}
			return false;
		}
		private static int GetDropCondiction(eDropType type, string para1, string para2)
		{
			try
			{
				return DropMgr.FindCondiction(type, para1, para2);
			}
			catch (Exception ex)
			{
				if (DropInventory.log.IsErrorEnabled)
				{
					DropInventory.log.Error(string.Concat(new object[]
					{
						"Drop Error：",
						type,
						" @ ",
						ex
					}));
				}
			}
			return 0;
		}
		private static bool GetDropItems(eDropType type, int dropId, ref List<ItemInfo> itemInfos)
		{
			if (dropId == 0)
			{
				return false;
			}
			try
			{
				int num = 1;
				List<DropItem> source = DropMgr.FindDropItem(dropId);
				int maxRound = ThreadSafeRandom.NextStatic((
					from s in source
					select s.Random).Max());
				List<DropItem> list = (
					from s in source
					where s.Random >= maxRound
					select s).ToList<DropItem>();
				int num2 = list.Count<DropItem>();
				bool result;
				if (num2 == 0)
				{
					result = false;
					return result;
				}
				num = ((num > num2) ? num2 : num);
				int[] randomUnrepeatArray = DropInventory.GetRandomUnrepeatArray(0, num2 - 1, num);
				int[] array = randomUnrepeatArray;
				for (int i = 0; i < array.Length; i++)
				{
					int index = array[i];
					int count = ThreadSafeRandom.NextStatic(list[index].BeginData, list[index].EndData);
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(list[index].ItemId);
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, count, 101);
					if (itemInfo != null)
					{
						itemInfo.IsBinds = list[index].IsBind;
						itemInfo.ValidDate = list[index].ValueDate;
						if (itemInfos == null)
						{
							itemInfos = new List<ItemInfo>();
						}
						if (DropInfoMgr.CanDrop(itemTemplateInfo.TemplateID))
						{
							itemInfos.Add(itemInfo);
						}
					}
				}
				result = true;
				return result;
			}
			catch (Exception ex)
			{
				if (DropInventory.log.IsErrorEnabled)
				{
					DropInventory.log.Error(string.Concat(new object[]
					{
						"Drop Error：",
						type,
						" @ ",
						ex
					}));
				}
			}
			return false;
		}
		public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				int num = ThreadSafeRandom.NextStatic(minValue, maxValue + 1);
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
