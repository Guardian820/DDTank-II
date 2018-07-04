using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class CardMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static CardGrooveUpdateInfo[] m_grooveUpdate;
		private static Dictionary<int, List<CardGrooveUpdateInfo>> m_grooveUpdates;
		private static CardTemplateInfo[] m_cardBox;
		private static Dictionary<int, List<CardTemplateInfo>> m_cardBoxs;
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		public static bool ReLoad()
		{
			try
			{
				CardGrooveUpdateInfo[] array = CardMgr.LoadGrooveUpdateDb();
				Dictionary<int, List<CardGrooveUpdateInfo>> value = CardMgr.LoadGrooveUpdates(array);
				if (array != null)
				{
					Interlocked.Exchange<CardGrooveUpdateInfo[]>(ref CardMgr.m_grooveUpdate, array);
					Interlocked.Exchange<Dictionary<int, List<CardGrooveUpdateInfo>>>(ref CardMgr.m_grooveUpdates, value);
				}
				CardTemplateInfo[] array2 = CardMgr.LoadCardBoxDb();
				Dictionary<int, List<CardTemplateInfo>> value2 = CardMgr.LoadCardBoxs(array2);
				if (array2 != null)
				{
					Interlocked.Exchange<CardTemplateInfo[]>(ref CardMgr.m_cardBox, array2);
					Interlocked.Exchange<Dictionary<int, List<CardTemplateInfo>>>(ref CardMgr.m_cardBoxs, value2);
				}
			}
			catch (Exception exception)
			{
				if (CardMgr.log.IsErrorEnabled)
				{
					CardMgr.log.Error("ReLoad", exception);
				}
				return false;
			}
			return true;
		}
		public static bool Init()
		{
			return CardMgr.ReLoad();
		}
		public static CardGrooveUpdateInfo[] LoadGrooveUpdateDb()
		{
			CardGrooveUpdateInfo[] result;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				CardGrooveUpdateInfo[] allCardGrooveUpdate = playerBussiness.GetAllCardGrooveUpdate();
				result = allCardGrooveUpdate;
			}
			return result;
		}
		public static Dictionary<int, List<CardGrooveUpdateInfo>> LoadGrooveUpdates(CardGrooveUpdateInfo[] GrooveUpdates)
		{
			Dictionary<int, List<CardGrooveUpdateInfo>> dictionary = new Dictionary<int, List<CardGrooveUpdateInfo>>();
			CardGrooveUpdateInfo info;
			for (int i = 0; i < GrooveUpdates.Length; i++)
			{
				info = GrooveUpdates[i];
				if (!dictionary.Keys.Contains(info.Type))
				{
					IEnumerable<CardGrooveUpdateInfo> source = 
						from s in GrooveUpdates
						where s.Type == info.Type
						select s;
					dictionary.Add(info.Type, source.ToList<CardGrooveUpdateInfo>());
				}
			}
			return dictionary;
		}
		public static CardTemplateInfo[] LoadCardBoxDb()
		{
			CardTemplateInfo[] result;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				CardTemplateInfo[] allCardTemplate = playerBussiness.GetAllCardTemplate();
				result = allCardTemplate;
			}
			return result;
		}
		public static Dictionary<int, List<CardTemplateInfo>> LoadCardBoxs(CardTemplateInfo[] CardBoxs)
		{
			Dictionary<int, List<CardTemplateInfo>> dictionary = new Dictionary<int, List<CardTemplateInfo>>();
			CardTemplateInfo info;
			for (int i = 0; i < CardBoxs.Length; i++)
			{
				info = CardBoxs[i];
				if (!dictionary.Keys.Contains(info.CardID))
				{
					IEnumerable<CardTemplateInfo> source = 
						from s in CardBoxs
						where s.CardID == info.CardID
						select s;
					dictionary.Add(info.CardID, source.ToList<CardTemplateInfo>());
				}
			}
			return dictionary;
		}
		public static CardTemplateInfo GetCard(int cardId)
		{
			CardTemplateInfo cardTemplateInfo = new CardTemplateInfo();
			List<CardTemplateInfo> list = CardMgr.FindCardBox(cardId);
			int num = 1;
			int maxRound = 0;
			foreach (CardTemplateInfo current in list)
			{
				if (maxRound >= current.probability)
				{
					maxRound = current.probability;
				}
			}
			maxRound = CardMgr.random.Next(maxRound);
			List<CardTemplateInfo> list2 = (
				from s in list
				where s.probability >= maxRound
				select s).ToList<CardTemplateInfo>();
			int num2 = list2.Count<CardTemplateInfo>();
			if (num2 > 0)
			{
				num = ((num > num2) ? num2 : num);
				int[] randomUnrepeatArray = CardMgr.GetRandomUnrepeatArray(0, num2 - 1, num);
				int[] array = randomUnrepeatArray;
				for (int i = 0; i < array.Length; i++)
				{
					int index = array[i];
					cardTemplateInfo = list2[index];
				}
			}
			if (cardTemplateInfo.CardType == 0)
			{
				return null;
			}
			return cardTemplateInfo;
		}
		public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				int num = CardMgr.random.Next(minValue, maxValue + 1);
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
		public static List<CardTemplateInfo> FindCardBox(int cardId)
		{
			if (CardMgr.m_cardBoxs == null)
			{
				CardMgr.Init();
			}
			if (CardMgr.m_cardBoxs.ContainsKey(cardId))
			{
				return CardMgr.m_cardBoxs[cardId];
			}
			return null;
		}
		public static CardTemplateInfo GetSingleCard(int id)
		{
			List<CardTemplateInfo> allCard = CardMgr.GetAllCard();
			foreach (CardTemplateInfo current in allCard)
			{
				if (current.ID == id)
				{
					return current;
				}
			}
			return null;
		}
		public static List<CardTemplateInfo> GetAllCard()
		{
			if (CardMgr.m_cardBox == null)
			{
				CardMgr.Init();
			}
			List<CardTemplateInfo> list = new List<CardTemplateInfo>();
			Dictionary<int, CardTemplateInfo> dictionary = new Dictionary<int, CardTemplateInfo>();
			CardTemplateInfo[] cardBox = CardMgr.m_cardBox;
			for (int i = 0; i < cardBox.Length; i++)
			{
				CardTemplateInfo cardTemplateInfo = cardBox[i];
				if (!dictionary.Keys.Contains(cardTemplateInfo.CardID))
				{
					if (cardTemplateInfo.CardID != 314150)
					{
						list.Add(cardTemplateInfo);
					}
					dictionary.Add(cardTemplateInfo.CardID, cardTemplateInfo);
				}
			}
			return list;
		}
		public static List<CardGrooveUpdateInfo> FindCardGrooveUpdate(int type)
		{
			if (CardMgr.m_grooveUpdates == null)
			{
				CardMgr.Init();
			}
			if (CardMgr.m_grooveUpdates.ContainsKey(type))
			{
				return CardMgr.m_grooveUpdates[type];
			}
			return null;
		}
		public static int CardCount()
		{
			return CardMgr.m_cardBox.Count<CardTemplateInfo>();
		}
		public static int MaxLv(int type)
		{
			return CardMgr.FindCardGrooveUpdate(type).Count - 1;
		}
		public static int GetLevel(int GP, int type)
		{
			if (GP >= CardMgr.FindCardGrooveUpdate(type)[CardMgr.MaxLv(type)].Exp)
			{
				return CardMgr.FindCardGrooveUpdate(type)[CardMgr.MaxLv(type)].Level;
			}
			for (int i = 1; i <= CardMgr.MaxLv(type); i++)
			{
				if (GP < CardMgr.FindCardGrooveUpdate(type)[i].Exp)
				{
					int index = (i - 1 == -1) ? 0 : (i - 1);
					return CardMgr.FindCardGrooveUpdate(type)[index].Level;
				}
			}
			return 0;
		}
		public static int GetProp(UsersCardInfo slot, int type)
		{
			int num = 0;
			for (int i = 0; i < slot.Level; i++)
			{
				num += CardMgr.GetGrooveSlot(slot.Type, i, type);
			}
			if (slot.CardID != 0)
			{
				num += CardMgr.GetPropCard(slot.CardType, slot.CardID, type);
			}
			return num;
		}
		public static int GetGrooveSlot(int type, int lv, int typeProp)
		{
			CardGrooveUpdateInfo[] grooveUpdate = CardMgr.m_grooveUpdate;
			for (int i = 0; i < grooveUpdate.Length; i++)
			{
				CardGrooveUpdateInfo cardGrooveUpdateInfo = grooveUpdate[i];
				if (cardGrooveUpdateInfo.Type == type && cardGrooveUpdateInfo.Level == lv)
				{
					int result;
					switch (typeProp)
					{
					case 0:
						result = cardGrooveUpdateInfo.Attack;
						break;

					case 1:
						result = cardGrooveUpdateInfo.Defend;
						break;

					case 2:
						result = cardGrooveUpdateInfo.Agility;
						break;

					case 3:
						result = cardGrooveUpdateInfo.Lucky;
						break;

					case 4:
						result = cardGrooveUpdateInfo.Damage;
						break;

					case 5:
						result = cardGrooveUpdateInfo.Guard;
						break;

					default:
						goto IL_7A;
					}
					return result;
				}
				IL_7A:;
			}
			return 0;
		}
		public static int GetPropCard(int cardtype, int cardID, int type)
		{
			CardTemplateInfo[] cardBox = CardMgr.m_cardBox;
			for (int i = 0; i < cardBox.Length; i++)
			{
				CardTemplateInfo cardTemplateInfo = cardBox[i];
				if (cardTemplateInfo.CardType == cardtype && cardTemplateInfo.CardID == cardID)
				{
					int result;
					switch (type)
					{
					case 0:
						result = cardTemplateInfo.AddAttack;
						break;

					case 1:
						result = cardTemplateInfo.AddDefend;
						break;

					case 2:
						result = cardTemplateInfo.AddAgility;
						break;

					case 3:
						result = cardTemplateInfo.AddLucky;
						break;

					case 4:
						result = cardTemplateInfo.AddDamage;
						break;

					case 5:
						result = cardTemplateInfo.AddGuard;
						break;

					default:
						goto IL_7A;
					}
					return result;
				}
				IL_7A:;
			}
			return 0;
		}
		public static int GetGP(int level, int type)
		{
			for (int i = 1; i <= CardMgr.MaxLv(type); i++)
			{
				if (level == CardMgr.FindCardGrooveUpdate(type)[i].Level)
				{
					return CardMgr.FindCardGrooveUpdate(type)[i].Exp;
				}
			}
			return 0;
		}
	}
}
