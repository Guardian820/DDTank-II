using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public static class ShopMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, ShopItemInfo> m_shop = new Dictionary<int, ShopItemInfo>();
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		public static bool Init()
		{
			return ShopMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, ShopItemInfo> dictionary = ShopMgr.LoadFromDatabase();
				if (dictionary.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, ShopItemInfo>>(ref ShopMgr.m_shop, dictionary);
				}
				return true;
			}
			catch (Exception exception)
			{
				ShopMgr.log.Error("ShopInfoMgr", exception);
			}
			return false;
		}
		private static Dictionary<int, ShopItemInfo> LoadFromDatabase()
		{
			Dictionary<int, ShopItemInfo> dictionary = new Dictionary<int, ShopItemInfo>();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				ShopItemInfo[] aLllShop = produceBussiness.GetALllShop();
				ShopItemInfo[] array = aLllShop;
				for (int i = 0; i < array.Length; i++)
				{
					ShopItemInfo shopItemInfo = array[i];
					if (!dictionary.ContainsKey(shopItemInfo.ID))
					{
						dictionary.Add(shopItemInfo.ID, shopItemInfo);
					}
				}
			}
			return dictionary;
		}
		public static ShopItemInfo GetShopItemInfoById(int ID)
		{
			if (ShopMgr.m_shop.ContainsKey(ID))
			{
				return ShopMgr.m_shop[ID];
			}
			return null;
		}
		public static bool CanBuy(int shopID, int consortiaShopLevel, ref bool isBinds, int cousortiaID, int playerRiches)
		{
			bool result = false;
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				switch (shopID)
				{
				case 1:
					result = true;
					isBinds = false;
					break;

				case 2:
					result = true;
					isBinds = false;
					break;

				case 3:
					result = true;
					isBinds = false;
					break;

				case 4:
					result = true;
					isBinds = false;
					break;

				case 11:
					{
						ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 1, 1);
						if (consortiaShopLevel >= consortiaEuqipRiches.Level && playerRiches >= consortiaEuqipRiches.Riches)
						{
							result = true;
							isBinds = true;
						}
						break;
					}

				case 12:
					{
						ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 2, 1);
						if (consortiaShopLevel >= consortiaEuqipRiches.Level && playerRiches >= consortiaEuqipRiches.Riches)
						{
							result = true;
							isBinds = true;
						}
						break;
					}

				case 13:
					{
						ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 3, 1);
						if (consortiaShopLevel >= consortiaEuqipRiches.Level && playerRiches >= consortiaEuqipRiches.Riches)
						{
							result = true;
							isBinds = true;
						}
						break;
					}

				case 14:
					{
						ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 4, 1);
						if (consortiaShopLevel >= consortiaEuqipRiches.Level && playerRiches >= consortiaEuqipRiches.Riches)
						{
							result = true;
							isBinds = true;
						}
						break;
					}

				case 15:
					{
						ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 5, 1);
						if (consortiaShopLevel >= consortiaEuqipRiches.Level && playerRiches >= consortiaEuqipRiches.Riches)
						{
							result = true;
							isBinds = true;
						}
						break;
					}
				}
			}
			return result;
		}
		public static int FindItemTemplateID(int id)
		{
			if (ShopMgr.m_shop.ContainsKey(id))
			{
				return ShopMgr.m_shop[id].TemplateID;
			}
			return 0;
		}
		public static List<ShopItemInfo> FindShopbyTemplatID(int TemplatID)
		{
			List<ShopItemInfo> list = new List<ShopItemInfo>();
			foreach (ShopItemInfo current in ShopMgr.m_shop.Values)
			{
				if (current.TemplateID == TemplatID)
				{
					list.Add(current);
				}
			}
			return list;
		}
		public static ShopItemInfo FindShopbyTemplateID(int TemplatID)
		{
			foreach (ShopItemInfo current in ShopMgr.m_shop.Values)
			{
				if (current.TemplateID == TemplatID)
				{
					return current;
				}
			}
			return null;
		}
	}
}
