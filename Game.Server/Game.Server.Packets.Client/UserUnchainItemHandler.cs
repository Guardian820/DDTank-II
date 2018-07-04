using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(47, "解除物品")]
	public class UserUnchainItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.IsPlaying)
			{
				return 0;
			}
			int fromSlot = packet.ReadInt();
			int toSlot = client.Player.MainBag.FindFirstEmptySlot(31);
			client.Player.MainBag.MoveItem(fromSlot, toSlot, 0);
			return 0;
		}
	}
}
