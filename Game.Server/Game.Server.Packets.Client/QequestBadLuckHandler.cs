using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(45, "场景用户离开")]
	public class QequestBadLuckHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ReadBoolean();
			return 0;
		}
	}
}
