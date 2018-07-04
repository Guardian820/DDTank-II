using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(219, "场景用户离开")]
	public class UserWeeklyClickHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int iD = client.Player.PlayerCharacter.ID;
			GSPacketIn gSPacketIn = new GSPacketIn(219, iD);
			if (DateTime.Now.Date != client.Player.PlayerCharacter.LastGetEgg.Date)
			{
				gSPacketIn.WriteBoolean(true);
			}
			else
			{
				gSPacketIn.WriteBoolean(false);
			}
			client.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
