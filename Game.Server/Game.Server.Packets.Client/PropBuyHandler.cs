using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(54, "购买道具")]
	public class PropBuyHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int iD = packet.ReadInt();
			int num6 = 1;
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
			if (shopItemInfoById == null)
			{
				return 0;
			}
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
			ItemInfo.SetItemType(shopItemInfoById, num6, ref num, ref num2, ref num3, ref num4, ref num5);
			if (itemTemplateInfo.CategoryID == 10)
			{
				PlayerInfo playerCharacter = client.Player.PlayerCharacter;
				if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked && (num2 > 0 || num3 > 0 || num4 > 0 || num5 > 0))
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
					return 0;
				}
				if (num <= playerCharacter.Gold && num2 <= ((playerCharacter.Money < 0) ? 0 : playerCharacter.Money) && num3 <= playerCharacter.Offer && num4 <= playerCharacter.GiftToken && num5 <= playerCharacter.medal)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
					if (shopItemInfoById.BuyType == 0)
					{
						if (1 == num6)
						{
							itemInfo.ValidDate = shopItemInfoById.AUnit;
						}
						if (2 == num6)
						{
							itemInfo.ValidDate = shopItemInfoById.BUnit;
						}
						if (3 == num6)
						{
							itemInfo.ValidDate = shopItemInfoById.CUnit;
						}
					}
					else
					{
						if (1 == num6)
						{
							itemInfo.Count = shopItemInfoById.AUnit;
						}
						if (2 == num6)
						{
							itemInfo.Count = shopItemInfoById.BUnit;
						}
						if (3 == num6)
						{
							itemInfo.Count = shopItemInfoById.CUnit;
						}
					}
					if (client.Player.FightBag.AddItem(itemInfo, 0))
					{
						client.Player.RemoveGold(num);
						client.Player.RemoveMoney(num2);
						client.Player.RemoveOffer(num3);
						client.Player.RemoveGiftToken(num4);
						client.Player.RemoveMedal(num5);
						LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Buy, client.Player.PlayerCharacter.ID, num2, client.Player.PlayerCharacter.Money, num, 0, 0, 0, "牌子编号", itemTemplateInfo.TemplateID.ToString(), num6.ToString());
					}
				}
				else
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("PropBuyHandler.NoMoney", new object[0]));
				}
			}
			return 0;
		}
	}
}
