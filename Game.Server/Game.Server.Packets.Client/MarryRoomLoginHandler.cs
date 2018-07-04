using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(242, "进入礼堂")]
	public class MarryRoomLoginHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			MarryRoom marryRoom = null;
			string text = "";
			int num = packet.ReadInt();
			string text2 = packet.ReadString();
			int marryMap = packet.ReadInt();
			if (num != 0)
			{
				marryRoom = MarryRoomMgr.GetMarryRoombyID(num, (text2 == null) ? "" : text2, ref text);
			}
			else
			{
				if (client.Player.PlayerCharacter.IsCreatedMarryRoom)
				{
					MarryRoom[] allMarryRoom = MarryRoomMgr.GetAllMarryRoom();
					MarryRoom[] array = allMarryRoom;
					for (int i = 0; i < array.Length; i++)
					{
						MarryRoom marryRoom2 = array[i];
						if (marryRoom2.Info.GroomID == client.Player.PlayerCharacter.ID || marryRoom2.Info.BrideID == client.Player.PlayerCharacter.ID)
						{
							marryRoom = marryRoom2;
							break;
						}
					}
				}
				if (marryRoom == null && client.Player.PlayerCharacter.SelfMarryRoomID != 0)
				{
					client.Player.Out.SendMarryRoomLogin(client.Player, false);
					MarryRoomInfo marryRoomInfo = null;
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						marryRoomInfo = playerBussiness.GetMarryRoomInfoSingle(client.Player.PlayerCharacter.SelfMarryRoomID);
					}
					if (marryRoomInfo != null)
					{
						client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoomLoginHandler.RoomExist", new object[]
						{
							marryRoomInfo.ServerID,
							client.Player.PlayerCharacter.SelfMarryRoomID
						}));
						return 0;
					}
				}
			}
			if (marryRoom != null)
			{
				if (marryRoom.CheckUserForbid(client.Player.PlayerCharacter.ID))
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("MarryRoomLoginHandler.Forbid", new object[0]));
					client.Player.Out.SendMarryRoomLogin(client.Player, false);
					return 1;
				}
				if (marryRoom.RoomState == eRoomState.FREE)
				{
					if (marryRoom.AddPlayer(client.Player))
					{
						client.Player.MarryMap = marryMap;
						client.Player.Out.SendMarryRoomLogin(client.Player, true);
						marryRoom.SendMarryRoomInfoUpdateToScenePlayers(marryRoom);
						return 0;
					}
				}
				else
				{
					client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoomLoginHandler.AlreadyBegin", new object[0]));
				}
				client.Player.Out.SendMarryRoomLogin(client.Player, false);
			}
			else
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation(string.IsNullOrEmpty(text) ? "MarryRoomLoginHandler.Failed" : text, new object[0]));
				client.Player.Out.SendMarryRoomLogin(client.Player, false);
			}
			return 1;
		}
	}
}
