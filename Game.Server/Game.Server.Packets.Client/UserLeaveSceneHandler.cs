using Game.Base.Packets;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(21, "场景用户离开")]
	public class UserLeaveSceneHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			RoomMgr.ExitWaitingRoom(client.Player);
			return 0;
		}
	}
}
