using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(183, "卡片使用")]
	public class CardUseHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int bagType = packet.ReadInt();
			int num = packet.ReadInt();
			string translateId = null;
			List<ShopItemInfo> list = new List<ShopItemInfo>();
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			ItemInfo itemInfo;
			if (num == -1)
			{
				int num2 = packet.ReadInt();
				int num3 = packet.ReadInt();
				int num4 = 0;
				int num5 = 0;
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(num2);
				itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
				list = ShopMgr.FindShopbyTemplatID(num2);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].APrice1 == -1 && list[i].AValue1 != 0)
					{
						num5 = list[i].AValue1;
						itemInfo.ValidDate = list[i].AUnit;
					}
				}
				if (itemInfo != null)
				{
					if (num4 <= client.Player.PlayerCharacter.Gold && num5 <= client.Player.PlayerCharacter.Money)
					{
						client.Player.RemoveMoney(num5);
						client.Player.RemoveGold(num4);
						LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Card, client.Player.PlayerCharacter.ID, num5, client.Player.PlayerCharacter.Money, num4, 0, 0, 0, "牌子编号", itemInfo.TemplateID.ToString(), num3.ToString());
						translateId = "CardUseHandler.Success";
					}
					else
					{
						itemInfo = null;
					}
				}
			}
			else
			{
				itemInfo = client.Player.PropBag.GetItemAt(num);
				translateId = "CardUseHandler.Success";
			}
			if (itemInfo != null)
			{
				string translateId2 = string.Empty;
				if (itemInfo.Template.Property1 != 21)
				{
					AbstractBuffer abstractBuffer = BufferList.CreateBuffer(itemInfo.Template, itemInfo.ValidDate);
					if (abstractBuffer != null)
					{
						abstractBuffer.Start(client.Player);
						if (num != -1)
						{
							client.Player.PropBag.RemoveCountFromStack(itemInfo, 1);
						}
					}
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId, new object[0]));
				}
				else
				{
					if (itemInfo.IsValidItem())
					{
                        client.Player.PlayerCharacter.GP += client.Player.AddGP(itemInfo.Template.Property2 * itemInfo.Count);//fix add nuoc kinh nghiem
						if (itemInfo.Template.CanDelete)
						{
							client.Player.RemoveAt((eBageType)bagType, num);
							translateId2 = "GPDanUser.Success";
						}
					}
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId2, new object[]
					{
						itemInfo.Template.Property2 * itemInfo.Count
					}));
				}
			}
			return 0;
		}
	}
}
