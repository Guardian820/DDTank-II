using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(57, "购买物品")]
	public class UserPresentGoodsHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = 0;
			int num2 = 0;
			int offer = 0;
			int num3 = 0;
			int medal = 0;
			StringBuilder stringBuilder = new StringBuilder();
			eMessageType eMessageType = eMessageType.Normal;
			string translateId = "UserPresentGoodsHandler.Success";
			string str = packet.ReadString();
			string nickName = packet.ReadString();
			int num4 = packet.ReadInt();
			List<ItemInfo> list = new List<ItemInfo>();
			StringBuilder stringBuilder2 = new StringBuilder();
			GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(nickName);
			PlayerInfo playerInfo;
			if (clientByPlayerNickName == null)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					playerInfo = playerBussiness.GetUserSingleByNickName(nickName);
					goto IL_78;
				}
			}
			playerInfo = clientByPlayerNickName.PlayerCharacter;
			IL_78:
			bool flag = false;
			for (int i = 0; i < num4; i++)
			{
				int iD = packet.ReadInt();
				int num5 = packet.ReadInt();
				string text = packet.ReadString();
				string text2 = packet.ReadString();
				packet.ReadInt();
				ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
				if (shopItemInfoById.BuyType == 0)
				{
					if (1 == num5)
					{
						itemInfo.ValidDate = shopItemInfoById.AUnit;
					}
					if (2 == num5)
					{
						itemInfo.ValidDate = shopItemInfoById.BUnit;
					}
					if (3 == num5)
					{
						itemInfo.ValidDate = shopItemInfoById.CUnit;
					}
				}
				else
				{
					if (1 == num5)
					{
						itemInfo.Count = shopItemInfoById.AUnit;
					}
					if (2 == num5)
					{
						itemInfo.Count = shopItemInfoById.BUnit;
					}
					if (3 == num5)
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
					stringBuilder2.Append(num5);
					stringBuilder2.Append(",");
					list.Add(itemInfo);
					ItemInfo.SetItemType(shopItemInfoById, num5, ref num, ref num2, ref offer, ref num3, ref medal);
				}
			}
			if (list.Count == 0)
			{
				return 1;
			}
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 1;
			}
			if (num <= client.Player.PlayerCharacter.Gold && num2 <= client.Player.PlayerCharacter.Money && num3 <= client.Player.PlayerCharacter.GiftToken)
			{
				client.Player.RemoveMoney(num2);
				client.Player.RemoveGold(num);
				client.Player.RemoveGiftToken(num3);
				string text3 = "";
				int num6 = 0;
				MailInfo mailInfo = new MailInfo();
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark", new object[0]));
				for (int j = 0; j < list.Count; j++)
				{
					text3 += ((text3 == "") ? list[j].TemplateID.ToString() : ("," + list[j].TemplateID.ToString()));
					using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
					{
						list[j].UserID = 0;
						playerBussiness2.AddGoods(list[j]);
						num6++;
						stringBuilder3.Append(num6);
						stringBuilder3.Append("、");
						stringBuilder3.Append(list[j].Template.Name);
						stringBuilder3.Append("x");
						stringBuilder3.Append(list[j].Count);
						stringBuilder3.Append(";");
						switch (num6)
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
						if (num6 == 5)
						{
							num6 = 0;
							mailInfo.AnnexRemark = stringBuilder3.ToString();
							stringBuilder3.Remove(0, stringBuilder3.Length);
							stringBuilder3.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark", new object[0]));
							mailInfo.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title", new object[0]) + mailInfo.Annex1Name + "] " + str;
							mailInfo.Gold = 0;
							mailInfo.Money = 0;
							mailInfo.Receiver = playerInfo.NickName;
							mailInfo.ReceiverID = playerInfo.ID;
							mailInfo.Sender = client.Player.PlayerCharacter.NickName;
							mailInfo.SenderID = client.Player.PlayerCharacter.ID;
							mailInfo.Title = mailInfo.Content;
							mailInfo.Type = 8;
							playerBussiness2.SendMail(mailInfo);
							eMessageType = eMessageType.ERROR;
							mailInfo.Revert();
						}
					}
				}
				if (num6 > 0)
				{
					using (PlayerBussiness playerBussiness3 = new PlayerBussiness())
					{
						mailInfo.AnnexRemark = stringBuilder3.ToString();
						mailInfo.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title", new object[0]) + mailInfo.Annex1Name + "] " + str;
						mailInfo.Gold = 0;
						mailInfo.Money = 0;
						mailInfo.Receiver = playerInfo.NickName;
						mailInfo.ReceiverID = playerInfo.ID;
						mailInfo.Sender = client.Player.PlayerCharacter.NickName;
						mailInfo.SenderID = client.Player.PlayerCharacter.ID;
						mailInfo.Title = mailInfo.Content;
						mailInfo.Type = 8;
						playerBussiness3.SendMail(mailInfo);
						eMessageType = eMessageType.ERROR;
					}
				}
				if (eMessageType == eMessageType.ERROR && clientByPlayerNickName != null)
				{
					clientByPlayerNickName.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				client.Player.OnPaid(num2, num, offer, num3, medal, stringBuilder.ToString());
				LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Buy, client.Player.PlayerCharacter.ID, num2, client.Player.PlayerCharacter.Money, num, num3, offer, medal, "牌子编号", text3, stringBuilder2.ToString());
			}
			else
			{
				if (num > client.Player.PlayerCharacter.Gold)
				{
					translateId = "UserBuyItemHandler.NoGold";
				}
				if (num2 > client.Player.PlayerCharacter.Money)
				{
					translateId = "UserBuyItemHandler.NoMoney";
				}
				if (num3 > client.Player.PlayerCharacter.GiftToken)
				{
					translateId = "UserBuyItemHandler.GiftToken";
				}
				eMessageType = eMessageType.ERROR;
			}
			client.Out.SendMessage(eMessageType, LanguageMgr.GetTranslation(translateId, new object[0]));
			return 0;
		}
	}
}
