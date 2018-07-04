using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(113, "获取邮件到背包")]
	public class MailGetAttachHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			byte b = packet.ReadByte();
			List<int> list = new List<int>();
			string text = "";
			eMessageType type = eMessageType.Normal;
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(113, client.Player.PlayerCharacter.ID);
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				MailInfo mailSingle = playerBussiness.GetMailSingle(client.Player.PlayerCharacter.ID, num);
				if (mailSingle != null)
				{
					bool flag = true;
					int money = mailSingle.Money;
					GamePlayer playerById = WorldMgr.GetPlayerById(mailSingle.ReceiverID);
					if (mailSingle.Type > 100 && mailSingle.Money > client.Player.PlayerCharacter.Money)
					{
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MailGetAttachHandler.NoMoney", new object[0]));
						return 0;
					}
					if (!mailSingle.IsRead)
					{
						mailSingle.IsRead = true;
						mailSingle.ValidDate = 72;
						mailSingle.SendTime = DateTime.Now;
					}
					if (flag && (b == 0 || b == 1) && !string.IsNullOrEmpty(mailSingle.Annex1) && this.GetAnnex(mailSingle.Annex1, client.Player, ref text, ref flag, ref type))
					{
						list.Add(1);
						mailSingle.Annex1 = null;
					}
					if (flag && (b == 0 || b == 2) && !string.IsNullOrEmpty(mailSingle.Annex2) && this.GetAnnex(mailSingle.Annex2, client.Player, ref text, ref flag, ref type))
					{
						list.Add(2);
						mailSingle.Annex2 = null;
					}
					if (flag && (b == 0 || b == 3) && !string.IsNullOrEmpty(mailSingle.Annex3) && this.GetAnnex(mailSingle.Annex3, client.Player, ref text, ref flag, ref type))
					{
						list.Add(3);
						mailSingle.Annex3 = null;
					}
					if (flag && (b == 0 || b == 4) && !string.IsNullOrEmpty(mailSingle.Annex4) && this.GetAnnex(mailSingle.Annex4, client.Player, ref text, ref flag, ref type))
					{
						list.Add(4);
						mailSingle.Annex4 = null;
					}
					if (flag && (b == 0 || b == 5) && !string.IsNullOrEmpty(mailSingle.Annex5) && this.GetAnnex(mailSingle.Annex5, client.Player, ref text, ref flag, ref type))
					{
						list.Add(5);
						mailSingle.Annex5 = null;
					}
					if ((b == 0 || b == 6) && mailSingle.Gold > 0)
					{
						list.Add(6);
						playerById.AddGold(mailSingle.Gold);
						mailSingle.Gold = 0;
					}
					if ((b == 0 || b == 7) && mailSingle.Type < 100 && mailSingle.Money > 0)
					{
						list.Add(7);
						playerById.AddMoney(mailSingle.Money);
						LogMgr.LogMoneyAdd(LogMoneyType.Mail, LogMoneyType.Mail_Money, playerById.PlayerCharacter.ID, mailSingle.Money, playerById.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
						mailSingle.Money = 0;
					}
					if (mailSingle.Type > 100 && mailSingle.GiftToken > 0)
					{
						list.Add(8);
						playerById.AddGiftToken(mailSingle.GiftToken);
						mailSingle.GiftToken = 0;
					}
					if (mailSingle.Type > 100 && mailSingle.Money > 0)
					{
						mailSingle.Money = 0;
						text = LanguageMgr.GetTranslation("MailGetAttachHandler.Deduct", new object[0]) + (string.IsNullOrEmpty(text) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success", new object[0]) : text);
					}
					if (playerBussiness.UpdateMail(mailSingle, money) && mailSingle.Type > 100 && money > 0)
					{
						playerById.RemoveMoney(money);
						LogMgr.LogMoneyAdd(LogMoneyType.Mail, LogMoneyType.Mail_Pay, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
						client.Out.SendMailResponse(mailSingle.SenderID, eMailRespose.Receiver);
						client.Out.SendMailResponse(mailSingle.ReceiverID, eMailRespose.Send);
					}
					gSPacketIn.WriteInt(num);
					gSPacketIn.WriteInt(list.Count);
					foreach (int current in list)
					{
						gSPacketIn.WriteInt(current);
					}
					client.Out.SendTCP(gSPacketIn);
					client.Out.SendMessage(type, string.IsNullOrEmpty(text) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success", new object[0]) : text);
				}
				else
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("MailGetAttachHandler.Falied", new object[0]));
				}
			}
			return 0;
		}
		public bool GetAnnex(string value, GamePlayer player, ref string msg, ref bool result, ref eMessageType eMsg)
		{
			int itemID = int.Parse(value);
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				ItemInfo userItemSingle = playerBussiness.GetUserItemSingle(itemID);
				if (userItemSingle != null)
				{
					if (userItemSingle.TemplateID == 11408)
					{
						player.AddMedal(userItemSingle.Count);
						bool result2 = true;
						return result2;
					}
					if (player.StackItemToAnother(userItemSingle) || player.AddItem(userItemSingle))
					{
						eMsg = eMessageType.Normal;
						bool result2 = true;
						return result2;
					}
					eMsg = eMessageType.ERROR;
					result = false;
					msg = LanguageMgr.GetTranslation(userItemSingle.GetBagName(), new object[0]) + LanguageMgr.GetTranslation("MailGetAttachHandler.NoPlace", new object[0]);
				}
			}
			return false;
		}
	}
}
