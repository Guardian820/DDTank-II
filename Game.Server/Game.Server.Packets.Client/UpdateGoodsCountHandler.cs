using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(168, "场景用户离开")]
	public class UpdateGoodsCountHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ReadInt();
			return 0;
		}
	}
}
