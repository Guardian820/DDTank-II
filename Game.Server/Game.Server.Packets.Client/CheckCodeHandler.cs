using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(200, "验证码")]
	public class CheckCodeHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			return 0;
		}
	}
}
