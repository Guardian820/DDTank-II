using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(91, "游戏数据")]
	public class GameDataHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentRoom != null)
			{
				packet.ClientID = client.Player.PlayerId;
				packet.Parameter1 = client.Player.GamePlayerId;
				client.Player.CurrentRoom.ProcessData(packet);
			}
			return 0;
		}
	}
}
