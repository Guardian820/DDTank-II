using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(13, "场景用户离开")]
	public class DailyAwardHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int type = packet.ReadInt();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int[] bag = new int[3];
			StringBuilder stringBuilder = new StringBuilder();
			List<ItemInfo> list = new List<ItemInfo>();
			string text = "";
			switch (type)
			{
			case 0:
				if (AwardMgr.AddDailyAward(client.Player))
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						if (playerBussiness.UpdatePlayerLastAward(client.Player.PlayerCharacter.ID, type))
						{
							stringBuilder.Append(LanguageMgr.GetTranslation("Nhận được Thẻ x2 kinh nghiệm 60 phút", new object[0]));
						}
						else
						{
							stringBuilder.Append(LanguageMgr.GetTranslation("GameUserDailyAward.Fail", new object[0]));
						}
						goto IL_29A;
					}
				}
				stringBuilder.Append(LanguageMgr.GetTranslation("GameUserDailyAward.Fail1", new object[0]));
				goto IL_29A;

			case 1:
			case 4:
				goto IL_29A;

			case 2:
				{
					if (DateTime.Now.Date == client.Player.PlayerCharacter.LastGetEgg.Date)
					{
						stringBuilder.Append("Bạn đã nhận 1 lần hôm nay!");
						goto IL_29A;
					}
					using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
					{
						playerBussiness2.UpdatePlayerLastAward(client.Player.PlayerCharacter.ID, type);
					}
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(112059);
					this.OpenUpItem(itemTemplateInfo.Data, bag, list, ref num2, ref num, ref num3, ref num4);
					goto IL_29A;
				}

			case 3:
				{
					int vIPLevel = client.Player.PlayerCharacter.VIPLevel;
					client.Player.LastVIPPackTime();
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(ItemMgr.FindItemBoxTypeAndLv(2, vIPLevel).TemplateID);
					this.OpenUpItem(itemTemplateInfo.Data, bag, list, ref num2, ref num, ref num3, ref num4);
					using (PlayerBussiness playerBussiness3 = new PlayerBussiness())
					{
						playerBussiness3.UpdateLastVIPPackTime(client.Player.PlayerCharacter.ID);
						goto IL_29A;
					}
					break;
				}

			case 5:
				break;

			default:
				goto IL_29A;
			}
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				DailyLogListInfo dailyLogListSingle = produceBussiness.GetDailyLogListSingle(client.Player.PlayerCharacter.ID);
				string text2 = dailyLogListSingle.DayLog;
				text2.Split(new char[]
				{
					','
				});
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "True";
					dailyLogListSingle.UserAwardLog = 0;
				}
				else
				{
					text2 += ",True";
				}
				dailyLogListSingle.DayLog = text2;
				dailyLogListSingle.UserAwardLog++;
				produceBussiness.UpdateDailyLogList(dailyLogListSingle);
			}
			stringBuilder.Append("Điểm danh thành công!");
			IL_29A:
			if (num != 0)
			{
				stringBuilder.Append(num + LanguageMgr.GetTranslation("OpenUpArkHandler.Money", new object[0]));
				client.Player.AddMoney(num);
			}
			if (num2 != 0)
			{
				stringBuilder.Append(num2 + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold", new object[0]));
				client.Player.AddGold(num2);
			}
			if (num3 != 0)
			{
				stringBuilder.Append(num3 + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken", new object[0]));
				client.Player.AddGiftToken(num3);
			}
			if (num4 != 0)
			{
				stringBuilder.Append(num4 + LanguageMgr.GetTranslation("OpenUpArkHandler.Medal", new object[0]));
				client.Player.AddMedal(num4);
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (ItemInfo current in list)
			{
				stringBuilder2.Append(current.Template.Name + "x" + current.Count.ToString() + ",");
				if (!client.Player.AddTemplate(current, current.Template.BagType, current.Count))
				{
					using (PlayerBussiness playerBussiness4 = new PlayerBussiness())
					{
						current.UserID = 0;
						playerBussiness4.AddGoods(current);
						MailInfo mailInfo = new MailInfo();
						mailInfo.Annex1 = current.ItemID.ToString();
						mailInfo.Content = LanguageMgr.GetTranslation("OpenUpArkHandler.Content1", new object[0]) + current.Template.Name + LanguageMgr.GetTranslation("OpenUpArkHandler.Content2", new object[0]);
						mailInfo.Gold = 0;
						mailInfo.Money = 0;
						mailInfo.Receiver = client.Player.PlayerCharacter.NickName;
						mailInfo.ReceiverID = client.Player.PlayerCharacter.ID;
						mailInfo.Sender = mailInfo.Receiver;
						mailInfo.SenderID = mailInfo.ReceiverID;
						mailInfo.Title = LanguageMgr.GetTranslation("OpenUpArkHandler.Title", new object[0]) + current.Template.Name + "]";
						mailInfo.Type = 12;
						playerBussiness4.SendMail(mailInfo);
						text = LanguageMgr.GetTranslation("OpenUpArkHandler.Mail", new object[0]);
					}
				}
			}
			if (stringBuilder2.Length > 0)
			{
				stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
				string[] array = stringBuilder2.ToString().Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					int num5 = 1;
					for (int j = i + 1; j < array.Length; j++)
					{
						if (array[i].Contains(array[j]) && array[j].Length == array[i].Length)
						{
							num5++;
							array[j] = j.ToString();
						}
					}
					if (num5 > 1)
					{
						array[i] = array[i].Remove(array[i].Length - 1, 1);
						array[i] += num5.ToString();
					}
					if (array[i] != i.ToString())
					{
						array[i] += ",";
						stringBuilder.Append(array[i]);
					}
				}
			}
			if (stringBuilder.Length - 1 > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.Append(".");
			}
			client.Out.SendMessage(eMessageType.Normal, text + stringBuilder.ToString());
			if (!string.IsNullOrEmpty(text))
			{
				client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			return 2;
		}
		public void OpenUpItem(string data, int[] bag, List<ItemInfo> infos, ref int gold, ref int money, ref int giftToken, ref int medal)
		{
			if (!string.IsNullOrEmpty(data))
			{
				ItemBoxMgr.CreateItemBox(Convert.ToInt32(data), infos, ref gold, ref money, ref giftToken, ref medal);
			}
		}
	}
}
