using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(5)]
	public class LargessCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom == null)
			{
				return false;
			}
			int num = packet.ReadInt();
			if (num <= 0)
			{
				return false;
			}
			if (player.PlayerCharacter.Money >= num)
			{
				player.RemoveMoney(num);
				LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_Gift, player.PlayerCharacter.ID, num, player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					string translation = LanguageMgr.GetTranslation("LargessCommand.Content", new object[]
					{
						player.PlayerCharacter.NickName,
						num / 2
					});
					string translation2 = LanguageMgr.GetTranslation("LargessCommand.Title", new object[]
					{
						player.PlayerCharacter.NickName
					});
					MailInfo mailInfo = new MailInfo();
					mailInfo.Annex1 = "";
					mailInfo.Content = translation;
					mailInfo.Gold = 0;
					mailInfo.IsExist = true;
					mailInfo.Money = num / 2;
					mailInfo.Receiver = player.CurrentMarryRoom.Info.BrideName;
					mailInfo.ReceiverID = player.CurrentMarryRoom.Info.BrideID;
					mailInfo.Sender = LanguageMgr.GetTranslation("LargessCommand.Sender", new object[0]);
					mailInfo.SenderID = 0;
					mailInfo.Title = translation2;
					mailInfo.Type = 14;
					playerBussiness.SendMail(mailInfo);
					player.Out.SendMailResponse(mailInfo.ReceiverID, eMailRespose.Receiver);
					MailInfo mailInfo2 = new MailInfo();
					mailInfo2.Annex1 = "";
					mailInfo2.Content = translation;
					mailInfo2.Gold = 0;
					mailInfo2.IsExist = true;
					mailInfo2.Money = num / 2;
					mailInfo2.Receiver = player.CurrentMarryRoom.Info.GroomName;
					mailInfo2.ReceiverID = player.CurrentMarryRoom.Info.GroomID;
					mailInfo2.Sender = LanguageMgr.GetTranslation("LargessCommand.Sender", new object[0]);
					mailInfo2.SenderID = 0;
					mailInfo2.Title = translation2;
					mailInfo2.Type = 14;
					playerBussiness.SendMail(mailInfo2);
					player.Out.SendMailResponse(mailInfo2.ReceiverID, eMailRespose.Receiver);
				}
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LargessCommand.Succeed", new object[0]));
				GSPacketIn packet2 = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("LargessCommand.Notice", new object[]
				{
					player.PlayerCharacter.NickName,
					num
				}));
				player.CurrentMarryRoom.SendToPlayerExceptSelf(packet2, player);
				return true;
			}
			player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough", new object[0]));
			return false;
		}
	}
}
