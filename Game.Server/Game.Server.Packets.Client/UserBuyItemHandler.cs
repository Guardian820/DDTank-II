using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(44, "购买物品")]
	public class UserBuyItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			eMessageType eMessageType = eMessageType.Normal;
			string translateId = "UserBuyItemHandler.Success";
			GSPacketIn gSPacketIn = new GSPacketIn(44);
			List<ItemInfo> list = new List<ItemInfo>();
			List<bool> list2 = new List<bool>();
			List<int> list3 = new List<int>();
			StringBuilder stringBuilder2 = new StringBuilder();
			bool flag = false;
			ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
			int num6 = packet.ReadInt();
			for (int i = 0; i < num6; i++)
			{
				packet.ReadInt();
				int iD = packet.ReadInt();
				int num7 = packet.ReadInt();
				string text = packet.ReadString();
				bool item = packet.ReadBoolean();
				string text2 = packet.ReadString();
				int item2 = packet.ReadInt();
				packet.ReadBoolean();
				ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
				if (shopItemInfoById == null)
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission", new object[0]));
					return 1;
				}
				if (!ShopMgr.CanBuy(shopItemInfoById.ShopID, (consortiaInfo == null) ? 1 : consortiaInfo.ShopLevel, ref flag, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.Riches))
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission", new object[0]));
					return 1;
				}
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
				if (shopItemInfoById.BuyType == 0)
				{
					if (1 == num7)
					{
						itemInfo.ValidDate = shopItemInfoById.AUnit;
					}
					if (2 == num7)
					{
						itemInfo.ValidDate = shopItemInfoById.BUnit;
					}
					if (3 == num7)
					{
						itemInfo.ValidDate = shopItemInfoById.CUnit;
					}
				}
				else
				{
					if (1 == num7)
					{
						itemInfo.Count = shopItemInfoById.AUnit;
					}
					if (2 == num7)
					{
						itemInfo.Count = shopItemInfoById.BUnit;
					}
					if (3 == num7)
					{
						itemInfo.Count = shopItemInfoById.CUnit;
					}
				}
				if (itemInfo != null || shopItemInfoById != null)
				{
					itemInfo.Color = ((text == null) ? "" : text);
					itemInfo.Skin = ((text2 == null) ? "" : text2);
					if (flag)
					{
						itemInfo.IsBinds = true;
					}
					else
					{
						itemInfo.IsBinds = Convert.ToBoolean(shopItemInfoById.IsBind);
					}
					stringBuilder2.Append(num7);
					stringBuilder2.Append(",");
					list.Add(itemInfo);
					list2.Add(item);
					list3.Add(item2);
					ItemInfo.SetItemType(shopItemInfoById, num7, ref num, ref num2, ref num3, ref num4, ref num5);
				}
			}
			int val = packet.ReadInt();
			if (list.Count == 0)
			{
				return 1;
			}
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 1;
			}
			bool flag2 = false;
			int num8 = 0;
			if (num <= client.Player.PlayerCharacter.Gold && num2 <= client.Player.PlayerCharacter.Money && num3 <= client.Player.PlayerCharacter.Offer && num4 <= client.Player.PlayerCharacter.GiftToken)
			{
				num8++;
				client.Player.AddExpVip(num2);
				client.Player.RemoveMoney(num2);
				client.Player.RemoveGold(num);
				client.Player.RemoveOffer(num3);
				client.Player.RemoveGiftToken(num4);
				string text3 = "";
				int num9 = 0;
				MailInfo mailInfo = new MailInfo();
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark", new object[0]));
				for (int j = 0; j < list.Count; j++)
				{
					text3 += ((text3 == "") ? list[j].TemplateID.ToString() : ("," + list[j].TemplateID.ToString()));
					if (!client.Player.AddTemplate(list[j], list[j].Template.BagType, list[j].Count))
					{
						flag2 = true;
						using (PlayerBussiness playerBussiness = new PlayerBussiness())
						{
							list[j].UserID = 0;
							playerBussiness.AddGoods(list[j]);
							num9++;
							stringBuilder3.Append(num9);
							stringBuilder3.Append("、");
							stringBuilder3.Append(list[j].Template.Name);
							stringBuilder3.Append("x");
							stringBuilder3.Append(list[j].Count);
							stringBuilder3.Append(";");
							switch (num9)
							{
							case 1:
								mailInfo.Annex1 = list[j].ItemID.ToString();
								mailInfo.Annex1Name = list[j].Template.Name;
								break;

							case 2:
								mailInfo.Annex2 = list[j].ItemID.ToString();
								mailInfo.Annex2Name = list[j].Template.Name;
								break;

							case 3:
								mailInfo.Annex3 = list[j].ItemID.ToString();
								mailInfo.Annex3Name = list[j].Template.Name;
								break;

							case 4:
								mailInfo.Annex4 = list[j].ItemID.ToString();
								mailInfo.Annex4Name = list[j].Template.Name;
								break;

							case 5:
								mailInfo.Annex5 = list[j].ItemID.ToString();
								mailInfo.Annex5Name = list[j].Template.Name;
								break;
							}
							if (num9 == 5)
							{
								num9 = 0;
								mailInfo.AnnexRemark = stringBuilder3.ToString();
								stringBuilder3.Remove(0, stringBuilder3.Length);
								stringBuilder3.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark", new object[0]));
								mailInfo.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title", new object[0]) + mailInfo.Annex1Name + "]";
								mailInfo.Gold = 0;
								mailInfo.Money = 0;
								mailInfo.Receiver = client.Player.PlayerCharacter.NickName;
								mailInfo.ReceiverID = client.Player.PlayerCharacter.ID;
								mailInfo.Sender = mailInfo.Receiver;
								mailInfo.SenderID = mailInfo.ReceiverID;
								mailInfo.Title = mailInfo.Content;
								mailInfo.Type = 8;
								playerBussiness.SendMail(mailInfo);
								eMessageType = eMessageType.ERROR;
								translateId = "UserBuyItemHandler.Mail";
								mailInfo.Revert();
							}
						}
					}
				}
				if (num9 > 0)
				{
					using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
					{
						mailInfo.AnnexRemark = stringBuilder3.ToString();
						mailInfo.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title", new object[0]) + mailInfo.Annex1Name + "]";
						mailInfo.Gold = 0;
						mailInfo.Money = 0;
						mailInfo.Receiver = client.Player.PlayerCharacter.NickName;
						mailInfo.ReceiverID = client.Player.PlayerCharacter.ID;
						mailInfo.Sender = mailInfo.Receiver;
						mailInfo.SenderID = mailInfo.ReceiverID;
						mailInfo.Title = mailInfo.Content;
						mailInfo.Type = 8;
						playerBussiness2.SendMail(mailInfo);
						eMessageType = eMessageType.ERROR;
						translateId = "UserBuyItemHandler.Mail";
					}
				}
				if (eMessageType == eMessageType.ERROR)
				{
					client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				client.Player.OnPaid(num2, num, num3, num4, num5, stringBuilder.ToString());
				LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Buy, client.Player.PlayerCharacter.ID, num2, client.Player.PlayerCharacter.Money, num, num4, num3, num5, "牌子编号", text3, stringBuilder2.ToString());
			}
			else
			{
				if (num2 > client.Player.PlayerCharacter.Money)
				{
					translateId = "UserBuyItemHandler.NoMoney";
				}
				if (num > client.Player.PlayerCharacter.Gold)
				{
					translateId = "UserBuyItemHandler.NoGold";
				}
				if (num3 > client.Player.PlayerCharacter.Offer)
				{
					translateId = "UserBuyItemHandler.NoOffer";
				}
				if (num4 > client.Player.PlayerCharacter.GiftToken)
				{
					translateId = "UserBuyItemHandler.GiftToken";
				}
				if (num5 > client.Player.PlayerCharacter.medal)
				{
					translateId = "UserBuyItemHandler.Medal";
				}
				eMessageType = eMessageType.ERROR;
			}
			int val2 = 0;
			if (num8 == num6)
			{
				val2 = (flag2 ? 2 : 1);
			}
			client.Player.MainBag.SaveToDatabase();
			client.Player.PropBag.SaveToDatabase();
			gSPacketIn.WriteInt(val2);
			gSPacketIn.WriteInt(val);
			client.Out.SendMessage(eMessageType, LanguageMgr.GetTranslation(translateId, new object[0]));
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
