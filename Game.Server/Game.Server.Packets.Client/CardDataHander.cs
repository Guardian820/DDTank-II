using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(216, "防沉迷系统开关")]
	internal class CardDataHander : IPacketHandler
	{
		public static ThreadSafeRandom random = new ThreadSafeRandom();
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			PlayerInfo arg_12_0 = client.Player.PlayerCharacter;
			CardInventory cardBag = client.Player.CardBag;
			string text = "";
			switch (num)
			{
			case 0:
				{
					int num2 = packet.ReadInt();
					int num3 = packet.ReadInt();
					int templateID = cardBag.GetItemAt(num2).TemplateID;
					if (cardBag.FindEquipCard(templateID) && num2 != num3)
					{
						text = "Thẻ này đã trang bị!";
					}
					else
					{
						if (num2 != num3)
						{
							text = "Trang bị thành công!";
						}
						cardBag.MoveItem(num2, num3);
						client.Player.MainBag.UpdatePlayerProperties();
					}
					if (text != "")
					{
						client.Out.SendMessage(eMessageType.Normal, text);
					}
					break;
				}

			case 1:
				{
					int slot = packet.ReadInt();
					int count = packet.ReadInt();
					ItemInfo itemAt = client.Player.MainBag.GetItemAt(slot);
					int property = itemAt.Template.Property5;
					int place = client.Player.CardBag.FindFirstEmptySlot(5);
					UsersCardInfo usersCardInfo = new UsersCardInfo();
					CardTemplateInfo card = CardMgr.GetCard(itemAt.Template.Property5);
					bool flag = false;
					if (card != null)
					{
						if (client.Player.CardBag.FindPlaceByTamplateId(5, property) == -1)
						{
							usersCardInfo.CardType = card.CardType;
							usersCardInfo.UserID = client.Player.PlayerCharacter.ID;
							usersCardInfo.Place = place;
							usersCardInfo.TemplateID = card.CardID;
							usersCardInfo.isFirstGet = true;
							usersCardInfo.Attack = 0;
							usersCardInfo.Agility = 0;
							usersCardInfo.Defence = 0;
							usersCardInfo.Luck = 0;
							usersCardInfo.Damage = 0;
							usersCardInfo.Guard = 0;
							client.Player.CardBag.AddCardTo(usersCardInfo, place);
							client.Out.SendGetCard(client.Player.PlayerCharacter, usersCardInfo);
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						int num4 = CardDataHander.random.Next(5, 50);
						client.Player.AddCardSoul(num4);
						client.Player.Out.SendPlayerCardSoul(client.Player.PlayerCharacter, true, num4);
					}
					client.Player.MainBag.RemoveCountFromStack(itemAt, count);
					break;
				}

			case 4:
				{
					int slot2 = packet.ReadInt();
					int count = packet.ReadInt();
					ItemInfo itemAt = client.Player.PropBag.GetItemAt(slot2);
					int id = CardDataHander.random.Next(CardMgr.CardCount());
					CardTemplateInfo singleCard = CardMgr.GetSingleCard(id);
					bool flag2 = false;
					if (singleCard == null)
					{
						flag2 = true;
					}
					else
					{
						int place2 = client.Player.CardBag.FindFirstEmptySlot(5);
						int cardID = singleCard.CardID;
						CardTemplateInfo card2 = CardMgr.GetCard(cardID);
						UsersCardInfo usersCardInfo2 = new UsersCardInfo();
						if (card2 == null)
						{
							flag2 = true;
						}
						else
						{
							if (client.Player.CardBag.FindPlaceByTamplateId(5, cardID) == -1)
							{
								usersCardInfo2.CardType = card2.CardType;
								usersCardInfo2.UserID = client.Player.PlayerCharacter.ID;
								usersCardInfo2.Place = place2;
								usersCardInfo2.TemplateID = card2.CardID;
								usersCardInfo2.isFirstGet = true;
								usersCardInfo2.Attack = 0;
								usersCardInfo2.Agility = 0;
								usersCardInfo2.Defence = 0;
								usersCardInfo2.Luck = 0;
								usersCardInfo2.Damage = 0;
								usersCardInfo2.Guard = 0;
								client.Player.CardBag.AddCardTo(usersCardInfo2, place2);
								client.Out.SendGetCard(client.Player.PlayerCharacter, usersCardInfo2);
							}
							else
							{
								flag2 = true;
							}
						}
					}
					if (flag2)
					{
						int num5 = CardDataHander.random.Next(5, 50);
						client.Player.AddCardSoul(num5);
						client.Player.Out.SendPlayerCardSoul(client.Player.PlayerCharacter, true, num5);
					}
					client.Player.PropBag.RemoveCountFromStack(itemAt, count);
					break;
				}
			}
			cardBag.SaveToDatabase();
			return 0;
		}
	}
}
