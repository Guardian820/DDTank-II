using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(2)]
	public class HymenealCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom == null || player.CurrentMarryRoom.RoomState != eRoomState.FREE)
			{
				return false;
			}
			if (player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
			{
				return false;
			}
			int pRICE_PROPOSE = GameProperties.PRICE_PROPOSE;
			if (player.CurrentMarryRoom.Info.IsHymeneal && player.PlayerCharacter.Money < pRICE_PROPOSE)
			{
				player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough", new object[0]));
				return false;
			}
			GamePlayer playerByUserID = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.GroomID);
			if (playerByUserID == null)
			{
				player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoGroom", new object[0]));
				return false;
			}
			GamePlayer playerByUserID2 = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.BrideID);
			if (playerByUserID2 == null)
			{
				player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoBride", new object[0]));
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			GSPacketIn gSPacketIn = packet.Clone();
			int num = packet.ReadInt();
			if (1 == num)
			{
				player.CurrentMarryRoom.RoomState = eRoomState.FREE;
			}
			else
			{
				player.CurrentMarryRoom.RoomState = eRoomState.Hymeneal;
				player.CurrentMarryRoom.BeginTimerForHymeneal(170000);
				if (!player.PlayerCharacter.IsGotRing)
				{
					flag2 = true;
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(9022);
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 112);
					itemInfo.IsBinds = true;
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						itemInfo.UserID = 0;
						playerBussiness.AddGoods(itemInfo);
						string translation = LanguageMgr.GetTranslation("HymenealCommand.Content", new object[]
						{
							playerByUserID2.PlayerCharacter.NickName
						});
						MailInfo mailInfo = new MailInfo();
						mailInfo.Annex1 = itemInfo.ItemID.ToString();
						mailInfo.Content = translation;
						mailInfo.Gold = 0;
						mailInfo.IsExist = true;
						mailInfo.Money = 0;
						mailInfo.Receiver = playerByUserID.PlayerCharacter.NickName;
						mailInfo.ReceiverID = playerByUserID.PlayerCharacter.ID;
						mailInfo.Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender", new object[0]);
						mailInfo.SenderID = 0;
						mailInfo.Title = LanguageMgr.GetTranslation("HymenealCommand.Title", new object[0]);
						mailInfo.Type = 14;
						if (playerBussiness.SendMail(mailInfo))
						{
							flag = true;
						}
						player.Out.SendMailResponse(mailInfo.ReceiverID, eMailRespose.Receiver);
					}
					ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(goods, 1, 112);
					itemInfo2.IsBinds = true;
					using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
					{
						itemInfo2.UserID = 0;
						playerBussiness2.AddGoods(itemInfo2);
						string translation2 = LanguageMgr.GetTranslation("HymenealCommand.Content", new object[]
						{
							playerByUserID.PlayerCharacter.NickName
						});
						MailInfo mailInfo2 = new MailInfo();
						mailInfo2.Annex1 = itemInfo2.ItemID.ToString();
						mailInfo2.Content = translation2;
						mailInfo2.Gold = 0;
						mailInfo2.IsExist = true;
						mailInfo2.Money = 0;
						mailInfo2.Receiver = playerByUserID2.PlayerCharacter.NickName;
						mailInfo2.ReceiverID = playerByUserID2.PlayerCharacter.ID;
						mailInfo2.Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender", new object[0]);
						mailInfo2.SenderID = 0;
						mailInfo2.Title = LanguageMgr.GetTranslation("HymenealCommand.Title", new object[0]);
						mailInfo2.Type = 14;
						if (playerBussiness2.SendMail(mailInfo2))
						{
							flag = true;
						}
						player.Out.SendMailResponse(mailInfo2.ReceiverID, eMailRespose.Receiver);
					}
					player.CurrentMarryRoom.Info.IsHymeneal = true;
					using (PlayerBussiness playerBussiness3 = new PlayerBussiness())
					{
						playerBussiness3.UpdateMarryRoomInfo(player.CurrentMarryRoom.Info);
						playerBussiness3.UpdatePlayerGotRingProp(playerByUserID.PlayerCharacter.ID, playerByUserID2.PlayerCharacter.ID);
						playerByUserID.LoadMarryProp();
						playerByUserID2.LoadMarryProp();
						goto IL_429;
					}
				}
				flag2 = false;
				flag = true;
				IL_429:
				if (!flag2)
				{
					player.RemoveMoney(pRICE_PROPOSE);
					LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_Hymeneal, player.PlayerCharacter.ID, pRICE_PROPOSE, player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
					CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, pRICE_PROPOSE, 0, 0, 1);
				}
				gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.ID);
				gSPacketIn.WriteBoolean(flag);
				player.CurrentMarryRoom.SendToAll(gSPacketIn);
				if (flag)
				{
					string translation3 = LanguageMgr.GetTranslation("HymenealCommand.Succeed", new object[]
					{
						playerByUserID.PlayerCharacter.NickName,
						playerByUserID2.PlayerCharacter.NickName
					});
					GSPacketIn packet2 = player.Out.SendMessage(eMessageType.ChatNormal, translation3);
					player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(packet2, player);
				}
			}
			return true;
		}
	}
}
