using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(182, "改变物品颜色")]
	public class UserChangeItemColorHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			eMessageType type = eMessageType.Normal;
			string translateId = "UserChangeItemColorHandler.Success";
			packet.ReadInt();
			int slot = packet.ReadInt();
			packet.ReadInt();
			int slot2 = packet.ReadInt();
			string text = packet.ReadString();
			string text2 = packet.ReadString();
			int num = packet.ReadInt();
			ItemInfo itemAt = client.Player.MainBag.GetItemAt(slot2);
			ItemInfo itemAt2 = client.Player.PropBag.GetItemAt(slot);
			if (itemAt != null)
			{
				client.Player.BeginChanges();
				try
				{
					bool flag = false;
					if (itemAt2 != null && itemAt2.IsValidItem())
					{
						client.Player.PropBag.RemoveItem(itemAt2);
						flag = true;
					}
					else
					{
						ItemMgr.FindItemTemplate(num);
						List<ShopItemInfo> list = ShopMgr.FindShopbyTemplatID(num);
						int num2 = 0;
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i].APrice1 == -1 && list[i].AValue1 != 0)
							{
								num2 = list[i].AValue1;
							}
						}
						if (num2 <= client.Player.PlayerCharacter.Money)
						{
							client.Player.RemoveMoney(num2);
							LogMgr.LogMoneyAdd(LogMoneyType.Item, LogMoneyType.Item_Color, client.Player.PlayerCharacter.ID, num2, client.Player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
							flag = true;
						}
					}
					if (flag)
					{
						itemAt.Color = ((text == null) ? "" : text);
						itemAt.Skin = ((text2 == null) ? "" : text2);
						client.Player.MainBag.UpdateItem(itemAt);
					}
				}
				finally
				{
					client.Player.CommitChanges();
				}
			}
			client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId, new object[0]));
			return 0;
		}
	}
}
