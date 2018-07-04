using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(215, "场景用户离开")]
	public class CaddyConvertedHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ReadBoolean();
			packet.ReadInt();
			packet.ReadInt();
			return 0;
		}
	}
}
