using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(132, "场景用户离开")]
	public class BattleGroundHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ReadByte();
			return 0;
		}
	}
}
