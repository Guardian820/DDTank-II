using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(240, "Player enter marry scene.")]
	public class UserEnterMarrySceneHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(240, client.Player.PlayerCharacter.ID);
			if (WorldMgr.MarryScene.AddPlayer(client.Player))
			{
				gSPacketIn.WriteBoolean(true);
			}
			else
			{
				gSPacketIn.WriteBoolean(false);
			}
			client.Out.SendTCP(gSPacketIn);
			if (client.Player.CurrentMarryRoom == null)
			{
				MarryRoom[] allMarryRoom = MarryRoomMgr.GetAllMarryRoom();
				MarryRoom[] array = allMarryRoom;
				for (int i = 0; i < array.Length; i++)
				{
					MarryRoom room = array[i];
					client.Player.Out.SendMarryRoomInfo(client.Player, room);
				}
			}
			return 0;
		}
	}
}
