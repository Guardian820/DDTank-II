using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(165, "场景用户离开")]
	public class LuckStoneEnableHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(165, client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteDateTime(DateTime.Now);
			gSPacketIn.WriteDateTime(DateTime.Now.AddDays(7.0));
			gSPacketIn.WriteBoolean(true);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
