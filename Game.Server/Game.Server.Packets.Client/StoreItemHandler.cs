using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(79, "储存物品")]
	public class StoreItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 1;
			}
			int num = (int)packet.ReadByte();
			int num2 = packet.ReadInt();
			packet.ReadInt();
			if (num == 0 && num2 < 31)
			{
				return 1;
			}
			ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
			if (consortiaInfo != null)
			{
				PlayerInventory arg_57_0 = client.Player.ConsortiaBag;
				client.Player.GetInventory((eBageType)num);
			}
			return 0;
		}
	}
}
