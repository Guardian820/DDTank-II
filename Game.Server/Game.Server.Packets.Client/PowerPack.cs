using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(46, "购买物品")]
	public class PowerPack : IPacketHandler
	{
		private ItemInfo getitem(GameClient client, int type, string color, string skin, int id)
		{
			ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(id);
			bool flag = false;
			ItemInfo itemInfo = null;
			if (shopItemInfoById != null)
			{
				itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID), 1, 102);
				if (shopItemInfoById.BuyType == 0)
				{
					if (1 == type)
					{
						itemInfo.ValidDate = shopItemInfoById.AUnit;
					}
					if (2 == type)
					{
						itemInfo.ValidDate = shopItemInfoById.BUnit;
					}
					if (3 == type)
					{
						itemInfo.ValidDate = shopItemInfoById.CUnit;
					}
				}
				else
				{
					if (1 == type)
					{
						itemInfo.Count = shopItemInfoById.AUnit;
					}
					if (2 == type)
					{
						itemInfo.Count = shopItemInfoById.BUnit;
					}
					if (3 == type)
					{
						itemInfo.Count = shopItemInfoById.CUnit;
					}
				}
				if (itemInfo != null || shopItemInfoById != null)
				{
					itemInfo.Color = ((color == null) ? "" : color);
					itemInfo.Skin = ((skin == null) ? "" : skin);
					if (flag)
					{
						itemInfo.IsBinds = true;
						return itemInfo;
					}
					itemInfo.IsBinds = Convert.ToBoolean(shopItemInfoById.IsBind);
				}
			}
			return itemInfo;
		}
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (998 <= client.Player.PlayerCharacter.Money)
			{
				ItemInfo item = this.getitem(client, 1, "", "", 1101801);
				ItemInfo item2 = this.getitem(client, 1, "", "", 1102001);
				ItemInfo item3 = this.getitem(client, 1, "", "", 1102401);
				ItemInfo item4 = this.getitem(client, 1, "", "", 1102401);
				ItemInfo item5 = this.getitem(client, 1, "", "", 1102401);
				PlayerInventory inventory = client.Player.GetInventory(eBageType.Store);
				client.Player.GetInventory(eBageType.PropBag);
				List<ItemInfo> items = inventory.GetItems();
				foreach (ItemInfo arg_CA_0 in items)
				{
				}
				inventory.AddItemTo(item, 4);
				inventory.AddItemTo(item2, 3);
				inventory.AddItemTo(item3, 2);
				inventory.AddItemTo(item4, 1);
				inventory.AddItemTo(item5, 0);
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.Success", new object[0]));
				client.Player.RemoveMoney(998);
			}
			else
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney", new object[0]));
			}
			return 0;
		}
	}
}
