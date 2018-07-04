using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(7)]
	public class KickCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom != null && player.CurrentMarryRoom.RoomState == eRoomState.FREE && (player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.GroomID || player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.BrideID))
			{
				int userID = packet.ReadInt();
				player.CurrentMarryRoom.KickPlayerByUserID(player, userID);
				return true;
			}
			return false;
		}
	}
}
