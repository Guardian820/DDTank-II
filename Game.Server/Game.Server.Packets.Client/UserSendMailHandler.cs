using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Statics;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(116, "发送邮件")]
	public class UserSendMailHandler : IPacketHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.Gold < 100)
			{
				return 1;
			}
			string translateId = "UserSendMailHandler.Success";
			eMessageType type = eMessageType.Normal;
			GSPacketIn gSPacketIn = new GSPacketIn(116, client.Player.PlayerCharacter.ID);
			ItemInfo itemInfo = null;
			string text = packet.ReadString().Trim();
			string title = packet.ReadString();
			string content = packet.ReadString();
			bool flag = packet.ReadBoolean();
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			eBageType eBageType = (eBageType)packet.ReadByte();
			int num3 = packet.ReadInt();
			int num4 = packet.ReadInt();
			if (text == client.Player.PlayerCharacter.NickName)
			{
				UserSendMailHandler.log.Error("Hack ingame: Username: " + client.Player.PlayerCharacter.UserName + " - NickName: " + client.Player.PlayerCharacter.NickName);
				return 0;
			}
			int num5 = 0;
			if ((num2 != 0 || num3 != -1) && client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				gSPacketIn.WriteBoolean(false);
				client.Out.SendTCP(gSPacketIn);
				return 1;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(text);
				PlayerInfo playerInfo;
				if (clientByPlayerNickName == null)
				{
					playerInfo = playerBussiness.GetUserSingleByNickName(text);
				}
				else
				{
					playerInfo = clientByPlayerNickName.PlayerCharacter;
				}
				if (playerInfo != null && !string.IsNullOrEmpty(text))
				{
					if (playerInfo.NickName != client.Player.PlayerCharacter.NickName)
					{
						client.Player.SaveIntoDatabase();
						MailInfo mailInfo = new MailInfo();
						mailInfo.SenderID = client.Player.PlayerCharacter.ID;
						mailInfo.Sender = client.Player.PlayerCharacter.NickName;
						mailInfo.ReceiverID = playerInfo.ID;
						mailInfo.Receiver = playerInfo.NickName;
						mailInfo.IsExist = true;
						mailInfo.Gold = 0;
						mailInfo.Money = 0;
						mailInfo.Title = title;
						mailInfo.Content = content;
						List<ItemInfo> list = new List<ItemInfo>();
						List<eBageType> list2 = new List<eBageType>();
						List<int> list3 = new List<int>();
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(LanguageMgr.GetTranslation("UserSendMailHandler.AnnexRemark", new object[0]));
						int num6 = 0;
						if (num3 != -1)
						{
							itemInfo = client.Player.GetItemAt(eBageType, num3);
							if (itemInfo != null && !itemInfo.IsBinds)
							{
								if (itemInfo.Count < num4 || num4 < 0)
								{
									client.Out.SendMessage(type, LanguageMgr.GetTranslation("Số lượng không có thực, thao tác thất bại!", new object[0]));
									int result = 0;
									return result;
								}
								num5 = itemInfo.Count - num4;
								mailInfo.Annex1Name = itemInfo.Template.Name;
								mailInfo.Annex1 = itemInfo.ItemID.ToString();
								list.Add(itemInfo);
								list2.Add(eBageType);
								list3.Add(num4);
								num6++;
								stringBuilder.Append(num6);
								stringBuilder.Append("、");
								stringBuilder.Append(mailInfo.Annex1Name);
								stringBuilder.Append("x");
								stringBuilder.Append(itemInfo.Count);
								stringBuilder.Append(";");
							}
						}
						if (flag)
						{
							if (num2 <= 0 || (string.IsNullOrEmpty(mailInfo.Annex1) && string.IsNullOrEmpty(mailInfo.Annex2) && string.IsNullOrEmpty(mailInfo.Annex3) && string.IsNullOrEmpty(mailInfo.Annex4)))
							{
								int result = 1;
								return result;
							}
							mailInfo.ValidDate = ((num == 1) ? 1 : 6);
							mailInfo.Type = 101;
							if (num2 > 0)
							{
								mailInfo.Money = num2;
								num6++;
								stringBuilder.Append(num6);
								stringBuilder.Append("、");
								stringBuilder.Append(LanguageMgr.GetTranslation("UserSendMailHandler.PayMoney", new object[0]));
								stringBuilder.Append(num2);
								stringBuilder.Append(";");
							}
						}
						else
						{
							mailInfo.Type = 1;
							if (client.Player.PlayerCharacter.Money >= num2 && num2 > 0)
							{
								mailInfo.Money = num2;
								client.Player.RemoveMoney(num2);
								LogMgr.LogMoneyAdd(LogMoneyType.Mail, LogMoneyType.Mail_Send, client.Player.PlayerCharacter.ID, num2, client.Player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
								num6++;
								stringBuilder.Append(num6);
								stringBuilder.Append("、");
								stringBuilder.Append(LanguageMgr.GetTranslation("UserSendMailHandler.Money", new object[0]));
								stringBuilder.Append(num2);
								stringBuilder.Append(";");
							}
						}
						if (stringBuilder.Length > 1)
						{
							mailInfo.AnnexRemark = stringBuilder.ToString();
						}
						if (playerBussiness.SendMail(mailInfo))
						{
							client.Player.RemoveGold(100);
							if (itemInfo != null)
							{
								itemInfo.UserID = 0;
								client.Player.RemoveItem(itemInfo);
								itemInfo.IsExist = true;
								itemInfo.Count = num4;
								if (num5 > 0)
								{
									client.Player.AddTemplate(itemInfo, eBageType, num5);
								}
							}
						}
						client.Player.SaveIntoDatabase();
						gSPacketIn.WriteBoolean(true);
						if (playerInfo.State != 0)
						{
							client.Player.Out.SendMailResponse(playerInfo.ID, eMailRespose.Receiver);
						}
						client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Send);
					}
					else
					{
						translateId = "UserSendMailHandler.Failed1";
						gSPacketIn.WriteBoolean(false);
					}
				}
				else
				{
					type = eMessageType.ERROR;
					translateId = "UserSendMailHandler.Failed2";
					gSPacketIn.WriteBoolean(false);
				}
			}
			client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId, new object[0]));
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
