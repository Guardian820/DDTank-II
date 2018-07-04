using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(218, "场景用户离开")]
	public class PlayerGiftHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ReadInt();
			return 0;
		}
	}
}
