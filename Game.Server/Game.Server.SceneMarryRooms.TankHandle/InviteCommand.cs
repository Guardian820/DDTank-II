using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(4)]
	public class InviteCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom == null || player.CurrentMarryRoom.RoomState != eRoomState.FREE)
			{
				return false;
			}
			if (!player.CurrentMarryRoom.Info.GuestInvite && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
			{
				return false;
			}
			GSPacketIn gSPacketIn = packet.Clone();
			gSPacketIn.ClearContext();
			int playerId = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (playerById != null && playerById.CurrentRoom == null && playerById.CurrentMarryRoom == null)
			{
				gSPacketIn.WriteByte(4);
				gSPacketIn.WriteInt(player.PlayerCharacter.ID);
				gSPacketIn.WriteString(player.PlayerCharacter.NickName);
				gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.ID);
				gSPacketIn.WriteString(player.CurrentMarryRoom.Info.Name);
				gSPacketIn.WriteString(player.CurrentMarryRoom.Info.Pwd);
				gSPacketIn.WriteInt(player.MarryMap);
				playerById.Out.SendTCP(gSPacketIn);
				return true;
			}
			return false;
		}
	}
}
