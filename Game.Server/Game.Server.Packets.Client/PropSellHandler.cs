using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(55, "出售道具")]
	public class PropSellHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int value = 0;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int type = 1;
			int slot = packet.ReadInt();
			int iD = packet.ReadInt();
			ItemInfo itemAt = client.Player.FightBag.GetItemAt(slot);
			if (itemAt != null)
			{
				client.Player.FightBag.RemoveItem(itemAt);
				ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
				ItemInfo.SetItemType(shopItemInfoById, type, ref value, ref num, ref num2, ref num3, ref num4);
				client.Player.AddGold(value);
			}
			return 0;
		}
	}
}
