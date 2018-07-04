using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(233, "结婚场景切换")]
	internal class MarrySceneChangeHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentMarryRoom == null || client.Player.MarryMap == 0)
			{
				return 1;
			}
			int num = packet.ReadInt();
			if (num == client.Player.MarryMap)
			{
				return 1;
			}
			GSPacketIn packet2 = new GSPacketIn(244, client.Player.PlayerCharacter.ID);
			client.Player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(packet2, client.Player);
			client.Player.MarryMap = num;
			if (num == 1)
			{
				client.Player.X = 514;
				client.Player.Y = 637;
			}
			else
			{
				if (num == 2)
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
			return 0;
		}
	}
}
