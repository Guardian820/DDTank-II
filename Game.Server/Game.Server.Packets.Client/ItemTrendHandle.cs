using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(120, "物品倾向转移")]
	public class ItemTrendHandle : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			eBageType bagType = (eBageType)packet.ReadInt();
			int place = packet.ReadInt();
			eBageType bagType2 = (eBageType)packet.ReadInt();
			List<ShopItemInfo> list = new List<ShopItemInfo>();
			int num = packet.ReadInt();
			int operation = packet.ReadInt();
			ItemInfo itemInfo;
			if (num == -1)
			{
				packet.ReadInt();
				int num2 = packet.ReadInt();
				int num3 = 0;
				int num4 = 0;
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(34101);
				itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
				list = ShopMgr.FindShopbyTemplatID(34101);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].APrice1 == -1 && list[i].AValue1 != 0)
					{
						num4 = list[i].AValue1;
						itemInfo.ValidDate = list[i].AUnit;
					}
				}
				if (itemInfo != null)
				{
					if (num3 <= client.Player.PlayerCharacter.Gold && num4 <= client.Player.PlayerCharacter.Money)
					{
						client.Player.RemoveMoney(num4);
						client.Player.RemoveGold(num3);
						LogMgr.LogMoneyAdd(LogMoneyType.Item, LogMoneyType.Item_Move, client.Player.PlayerCharacter.ID, num4, client.Player.PlayerCharacter.Money, num3, 0, 0, 0, "牌子编号", itemInfo.TemplateID.ToString(), num2.ToString());
					}
					else
					{
						itemInfo = null;
					}
				}
			}
			else
			{
				itemInfo = client.Player.GetItemAt(bagType2, num);
			}
			ItemInfo itemAt = client.Player.GetItemAt(bagType, place);
			StringBuilder stringBuilder = new StringBuilder();
			if (itemInfo == null || itemAt == null)
			{
				return 1;
			}
			bool flag = false;
			ItemTemplateInfo itemTemplateInfo = RefineryMgr.RefineryTrend(operation, itemAt, ref flag);
			if (flag && itemTemplateInfo != null)
			{
				ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 115);
				AbstractInventory itemInventory = client.Player.GetItemInventory(itemTemplateInfo);
				if (itemInventory.AddItem(itemInfo2, itemInventory.BeginSlot))
				{
					client.Player.UpdateItem(itemInfo2);
					client.Player.RemoveItem(itemAt);
					itemInfo.Count--;
					client.Player.UpdateItem(itemInfo);
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemTrendHandle.Success", new object[0]));
				}
				else
				{
					stringBuilder.Append("NoPlace");
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(itemInfo2.GetBagName(), new object[0]) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace", new object[0]));
				}
				return 1;
			}
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemTrendHandle.Fail", new object[0]));
			return 1;
		}
	}
}
