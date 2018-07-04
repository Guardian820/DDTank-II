using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(251, "当前场景状态")]
	internal class MarryStateHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			switch (packet.ReadInt())
			{
			case 0:
				if (client.Player.IsInMarryRoom)
				{
					if (client.Player.MarryMap == 1)
					{
						client.Player.X = 646;
						client.Player.Y = 1241;
					}
					else
					{
						if (client.Player.MarryMap == 2)
						{
							client.Player.X = 800;
							client.Player.Y = 763;
						}
					}
					GamePlayer[] allPlayers = client.Player.CurrentMarryRoom.GetAllPlayers();
					GamePlayer[] array = allPlayers;
					for (int i = 0; i < array.Length; i++)
					{
						GamePlayer gamePlayer = array[i];
						if (gamePlayer != client.Player && gamePlayer.MarryMap == client.Player.MarryMap)
						{
							gamePlayer.Out.SendPlayerEnterMarryRoom(client.Player);
							client.Player.Out.SendPlayerEnterMarryRoom(gamePlayer);
						}
					}
				}
				break;

			case 1:
				RoomMgr.EnterWaitingRoom(client.Player);
				break;
			}
			return 0;
		}
	}
}
