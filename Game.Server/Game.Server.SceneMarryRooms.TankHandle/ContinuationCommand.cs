using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Statics;
using log4net;
using System;
using System.Reflection;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(3)]
	public class ContinuationCommand : IMarryCommandHandler
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom == null)
			{
				return false;
			}
			if (player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
			{
				return false;
			}
			int time = packet.ReadInt();
			string[] array = GameProperties.PRICE_MARRY_ROOM.Split(new char[]
			{
				','
			});
			if (array.Length < 3)
			{
				if (ContinuationCommand.log.IsErrorEnabled)
				{
					ContinuationCommand.log.Error("MarryRoomCreateMoney node in configuration file is wrong");
				}
				return false;
			}
			int num;
			switch (time)
			{
			case 2:
				num = int.Parse(array[0]);
				break;

			case 3:
				num = int.Parse(array[1]);
				break;

			case 4:
				num = int.Parse(array[2]);
				break;

			default:
				num = int.Parse(array[2]);
				time = 4;
				break;
			}
			if (player.PlayerCharacter.Money < num)
			{
				player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg1", new object[0]));
				return false;
			}
			player.RemoveMoney(num);
			LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_RoomAdd, player.PlayerCharacter.ID, num, player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
			CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, num, 0, 0, 0);
			player.CurrentMarryRoom.RoomContinuation(time);
			GSPacketIn packet2 = player.Out.SendContinuation(player, player.CurrentMarryRoom.Info);
			int playerId;
			if (player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.GroomID)
			{
				playerId = player.CurrentMarryRoom.Info.BrideID;
			}
			else
			{
				playerId = player.CurrentMarryRoom.Info.GroomID;
			}
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (playerById != null)
			{
				playerById.Out.SendTCP(packet2);
			}
			player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ContinuationCommand.Successed", new object[0]));
			return true;
		}
	}
}
