using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(30, "打开物品")]
	public class LotteryGetItem : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ReadByte();
			packet.ReadInt();
			PlayerInventory caddyBag = client.Player.CaddyBag;
			PlayerInventory propBag = client.Player.PropBag;
			for (int i = 0; i < caddyBag.Capalility; i++)
			{
				ItemInfo itemAt = caddyBag.GetItemAt(i);
				if (itemAt != null)
				{
					Console.WriteLine("caddyBag.MoveItem: " + propBag.BagType);
				}
			}
			return 1;
		}
	}
}
