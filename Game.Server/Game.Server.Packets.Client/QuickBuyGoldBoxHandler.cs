using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(126, "场景用户离开")]
	public class QuickBuyGoldBoxHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			packet.ReadBoolean();
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 1;
			}
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(1123301);
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
			int num2 = num * shopItemInfoById.AValue1;
			if (client.Player.PlayerCharacter.Money > num2)
			{
				client.Player.RemoveMoney(num2);
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				List<ItemInfo> infos = new List<ItemInfo>();
				this.OpenUpItem(itemTemplateInfo.Data, infos, ref num4, ref num3, ref num5, ref num6);
				int num7 = num * num4;
				client.Player.AddGold(num7);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bạn nhận được " + num7 + " vàng.", new object[0]));
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney", new object[0]));
			}
			return 0;
		}
		public void OpenUpItem(string data, List<ItemInfo> infos, ref int gold, ref int money, ref int giftToken, ref int medal)
		{
			if (!string.IsNullOrEmpty(data))
			{
				ItemBoxMgr.CreateItemBox(Convert.ToInt32(data), infos, ref gold, ref money, ref giftToken, ref medal);
			}
		}
	}
}
