using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(63, "打开物品")]
	public class OpenUpArkHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int bageType = (int)packet.ReadByte();
			int slot = packet.ReadInt();
			PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
			ItemInfo itemAt = inventory.GetItemAt(slot);
			string text = "";
			List<ItemInfo> list = new List<ItemInfo>();
			if (itemAt != null && itemAt.IsValidItem() && itemAt.Template.CategoryID == 11 && itemAt.Template.Property1 == 6 && client.Player.PlayerCharacter.Grade >= itemAt.Template.NeedLevel)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int[] array = new int[3];
				this.OpenUpItem(itemAt.Template.Data, array, list, ref num2, ref num, ref num3, ref num4);
				array[itemAt.GetBagType()]--;
				if (inventory.RemoveCountFromStack(itemAt, 1))
				{
					StringBuilder stringBuilder = new StringBuilder();
					int num5 = 0;
					StringBuilder stringBuilder2 = new StringBuilder();
					stringBuilder2.Append(LanguageMgr.GetTranslation("OpenUpArkHandler.Start", new object[0]));
					if (num != 0)
					{
						stringBuilder2.Append(num + LanguageMgr.GetTranslation("OpenUpArkHandler.Money", new object[0]));
						client.Player.AddMoney(num);
						LogMgr.LogMoneyAdd(LogMoneyType.Box, LogMoneyType.Box_Open, client.Player.PlayerCharacter.ID, num, client.Player.PlayerCharacter.Money, num2, 0, 0, 0, "", "", "");
					}
					if (num2 != 0)
					{
						stringBuilder2.Append(num2 + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold", new object[0]));
						client.Player.AddGold(num2);
					}
					if (num3 != 0)
					{
						stringBuilder2.Append(num3 + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken", new object[0]));
						client.Player.AddGiftToken(num3);
					}
					if (num4 != 0)
					{
						stringBuilder2.Append(num4 + LanguageMgr.GetTranslation("OpenUpArkHandler.Medal", new object[0]));
						client.Player.AddMedal(num4);
					}
					StringBuilder stringBuilder3 = new StringBuilder();
					foreach (ItemInfo current in list)
					{
						stringBuilder3.Append(current.Template.Name + "x" + current.Count.ToString() + ",");
						if (current.Template.Quality >= itemAt.Template.Property2 & itemAt.Template.Property2 != 0)
						{
							stringBuilder.Append(current.Template.Name + ",");
							num5++;
						}
						if (!client.Player.AddTemplate(current, current.Template.BagType, current.Count))
						{
							using (PlayerBussiness playerBussiness = new PlayerBussiness())
							{
								current.UserID = 0;
								playerBussiness.AddGoods(current);
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
								playerBussiness.SendMail(mailInfo);
								text = LanguageMgr.GetTranslation("OpenUpArkHandler.Mail", new object[0]);
							}
						}
					}
					if (stringBuilder3.Length > 0)
					{
						stringBuilder3.Remove(stringBuilder3.Length - 1, 1);
						string[] array2 = stringBuilder3.ToString().Split(new char[]
						{
							','
						});
						for (int i = 0; i < array2.Length; i++)
						{
							int num6 = 1;
							for (int j = i + 1; j < array2.Length; j++)
							{
								if (array2[i].Contains(array2[j]) && array2[j].Length == array2[i].Length)
								{
									num6++;
									array2[j] = j.ToString();
								}
							}
							if (num6 > 1)
							{
								array2[i] = array2[i].Remove(array2[i].Length - 1, 1);
								array2[i] += num6.ToString();
							}
							if (array2[i] != i.ToString())
							{
								array2[i] += ",";
								stringBuilder2.Append(array2[i]);
							}
						}
					}
					stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
					stringBuilder2.Append(".");
					client.Out.SendMessage(eMessageType.Normal, text + stringBuilder2.ToString());
					if (!string.IsNullOrEmpty(text))
					{
						client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
				}
			}
			return 1;
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
