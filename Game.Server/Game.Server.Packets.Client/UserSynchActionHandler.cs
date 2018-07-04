using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(36, "用户同步动作")]
	public class UserSynchActionHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int clientID = packet.ClientID;
			GamePlayer playerById = WorldMgr.GetPlayerById(clientID);
			if (playerById != null)
			{
				packet.Code = 35;
				packet.ClientID = client.Player.PlayerCharacter.ID;
				playerById.Out.SendTCP(packet);
			}
			return 1;
		}
	}
}
