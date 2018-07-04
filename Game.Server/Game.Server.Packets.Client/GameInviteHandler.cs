using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(70, "邀请")]
	public class GameInviteHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentRoom == null)
			{
				return 0;
			}
			int playerId = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (playerById == client.Player)
			{
				return 0;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(70, client.Player.PlayerCharacter.ID);
			List<GamePlayer> players = client.Player.CurrentRoom.GetPlayers();
			foreach (GamePlayer current in players)
			{
				if (current == playerById)
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Sameroom", new object[0]));
					int result = 0;
					return result;
				}
			}
            if (playerById != null && playerById.CurrentRoom == null)//__receiveInvite
			{
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
				gSPacketIn.WriteInt(client.Player.CurrentRoom.RoomId);
				gSPacketIn.WriteInt(client.Player.CurrentRoom.MapId);
				gSPacketIn.WriteByte(client.Player.CurrentRoom.TimeMode);
				gSPacketIn.WriteByte((byte)client.Player.CurrentRoom.RoomType);
				gSPacketIn.WriteByte((byte)client.Player.CurrentRoom.HardLevel);
				gSPacketIn.WriteByte((byte)client.Player.CurrentRoom.LevelLimits);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
				gSPacketIn.WriteByte(client.Player.PlayerCharacter.typeVIP);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.VIPLevel);
				gSPacketIn.WriteString(client.Player.CurrentRoom.Name);
				gSPacketIn.WriteString(client.Player.CurrentRoom.Password);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteBoolean(client.Player.CurrentRoom.isOpenBoss);
				playerById.Out.SendTCP(gSPacketIn);
				return 0;
			}
			if (playerById != null && playerById.CurrentRoom != null && playerById.CurrentRoom != client.Player.CurrentRoom)
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Room", new object[0]));
				return 0;
			}
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Fail", new object[0]));
			return 0;
		}
	}
}
