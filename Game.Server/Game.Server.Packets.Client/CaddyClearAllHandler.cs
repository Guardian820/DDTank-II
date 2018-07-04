using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(232, "打开物品")]
	public class CaddyClearAllHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			PlayerInventory caddyBag = client.Player.CaddyBag;
			int num = 1;
			int num2 = 0;
			int num3 = 0;
			string text = "";
			for (int i = 0; i < caddyBag.Capalility; i++)
			{
				ItemInfo itemAt = caddyBag.GetItemAt(i);
				if (itemAt != null)
				{
					if (itemAt.Template.ReclaimType == 1)
					{
						num2 += num * itemAt.Template.ReclaimValue;
						text += LanguageMgr.GetTranslation("ItemReclaimHandler.Success2", new object[]
						{
							num2
						});
					}
					if (itemAt.Template.ReclaimType == 2)
					{
						num3 += num * itemAt.Template.ReclaimValue;
						text += LanguageMgr.GetTranslation("ItemReclaimHandler.Success1", new object[]
						{
							num3
						});
					}
					caddyBag.RemoveItem(itemAt);
				}
			}
			client.Player.BeginChanges();
			client.Player.AddGold(num2);
			client.Player.AddGiftToken(num3);
			client.Player.CommitChanges();
			client.Out.SendMessage(eMessageType.Normal, text);
			return 1;
		}
	}
}
