using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(62, "续费")]
	public class UserItemContineueHandler : IPacketHandler
	{
        public int HandlePacket(GameClient client, GSPacketIn packet)//sendGoodsContinue
		{
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			new StringBuilder();
			int num = packet.ReadInt();
			string translateId = "UserItemContineueHandler.Success";
			for (int i = 0; i < num; i++)
			{
				eBageType eBageType = (eBageType)packet.ReadByte();
				int num2 = packet.ReadInt();
				int iD = packet.ReadInt();
				int num3 = (int)packet.ReadByte();
				bool flag = packet.ReadBoolean();
                int themvao = packet.ReadInt();//_loc_3.writeInt(param1[_loc_4][5]);//4.2

				if ((eBageType == eBageType.MainBag && num2 >= 31) || eBageType == eBageType.PropBag || eBageType == eBageType.Consortia)
				{
					ItemInfo itemAt = client.Player.GetItemAt(eBageType, num2);
					int num4 = 0;
					int num5 = 0;
					int num6 = 0;
					int num7 = 0;
					int num8 = 0;
					int validDate = itemAt.ValidDate;
					DateTime arg_C8_0 = itemAt.BeginDate;
					int count = itemAt.Count;
					bool flag2 = itemAt.IsValidItem();
					if (itemAt.ValidDate > 365)
					{
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Thời gian sử dụng đả đạt mức tối đa.", new object[0]));
						break;
					}
					ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
					if (shopItemInfoById.TemplateID != itemAt.TemplateID)
					{
						Console.WriteLine("HACK GIA HAN  " + client.Player.Account);
						return 0;
					}
					ItemInfo.SetItemType(shopItemInfoById, num3, ref num4, ref num5, ref num6, ref num7, ref num8);
					if (num4 <= client.Player.PlayerCharacter.Gold && num5 <= client.Player.PlayerCharacter.Money && num6 <= client.Player.PlayerCharacter.Offer && num7 <= client.Player.PlayerCharacter.GiftToken)
					{
						client.Player.RemoveMoney(num5);
						client.Player.RemoveGold(num4);
						client.Player.RemoveOffer(num6);
						client.Player.RemoveGiftToken(num7);
						if (!flag2 && itemAt.ValidDate != 0)
						{
							itemAt.ValidDate = 0;
						}
						if (1 == num3)
						{
							itemAt.ValidDate += shopItemInfoById.AUnit;
						}
						if (2 == num3)
						{
							itemAt.ValidDate += shopItemInfoById.BUnit;
						}
						if (3 == num3)
						{
							itemAt.ValidDate += shopItemInfoById.CUnit;
						}
						if (!flag2 && itemAt.ValidDate != 0)
						{
							itemAt.BeginDate = DateTime.Now;
							itemAt.IsUsed = false;
						}
						LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Continue, client.Player.PlayerCharacter.ID, num5, client.Player.PlayerCharacter.Money, num4, 0, 0, 0, "牌子编号", itemAt.TemplateID.ToString(), num3.ToString());
					}
					else
					{
						itemAt.ValidDate = validDate;
						itemAt.Count = count;
						translateId = "UserItemContineueHandler.NoMoney";
					}
					if (eBageType == eBageType.MainBag)
					{
						if (flag)
						{
							int toSlot = client.Player.MainBag.FindItemEpuipSlot(itemAt.Template);
							client.Player.MainBag.MoveItem(num2, toSlot, itemAt.Count);
						}
						else
						{
							client.Player.MainBag.UpdateItem(itemAt);
						}
					}
					else
					{
						if (eBageType == eBageType.PropBag)
						{
							client.Player.PropBag.UpdateItem(itemAt);
						}
						else
						{
							client.Player.ConsortiaBag.UpdateItem(itemAt);
						}
					}
				}
			}
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId, new object[0]));
			return 0;
		}
	}
}
