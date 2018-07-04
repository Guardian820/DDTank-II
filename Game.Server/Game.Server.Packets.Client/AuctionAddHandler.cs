using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(192, "添加拍卖")]
	public class AuctionAddHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			eBageType bagType = (eBageType)packet.ReadByte();
			int place = packet.ReadInt();
			int num = (int)packet.ReadByte();
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			int num4 = packet.ReadInt();
			int num5 = packet.ReadInt();
			string translateId = "AuctionAddHandler.Fail";
			num = 1;
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			if (num2 < 0 || (num3 != 0 && num3 < num2))
			{
				return 0;
			}
			int num6 = 1;
			if (num != 0)
			{
				num6 = 1;
				num = 1;
			}
			int num7 = (int)((double)(num6 * num2) * 0.03 * (double)((num4 == 0) ? 1 : ((num4 == 1) ? 3 : 6)));
			num7 = ((num7 < 1) ? 1 : num7);
			ItemInfo itemAt = client.Player.GetItemAt(bagType, place);
			if (itemAt == null)
			{
				client.Player.SendMessage("Vật phẩm lạ không tìm thấy!!! Hack chăng???!!!");
				return 0;
			}
			int num8 = itemAt.Count - num5;
			if (itemAt.Count < num5 || num5 < 0)
			{
				num5 = itemAt.Count;
			}
			if (num2 < 0)
			{
				translateId = "AuctionAddHandler.Msg1";
			}
			else
			{
				if (num3 != 0 && num3 < num2)
				{
					translateId = "AuctionAddHandler.Msg2";
				}
				else
				{
					if (num7 > client.Player.PlayerCharacter.Gold)
					{
						translateId = "AuctionAddHandler.Msg3";
					}
					else
					{
						if (itemAt == null)
						{
							translateId = "AuctionAddHandler.Msg4";
						}
						else
						{
							if (itemAt.IsBinds)
							{
								translateId = "AuctionAddHandler.Msg5";
							}
							else
							{
								client.Player.SaveIntoDatabase();
								AuctionInfo auctionInfo = new AuctionInfo();
								auctionInfo.AuctioneerID = client.Player.PlayerCharacter.ID;
								auctionInfo.AuctioneerName = client.Player.PlayerCharacter.NickName;
								auctionInfo.BeginDate = DateTime.Now;
								auctionInfo.BuyerID = 0;
								auctionInfo.BuyerName = "";
								auctionInfo.IsExist = true;
								auctionInfo.ItemID = itemAt.ItemID;
								auctionInfo.Mouthful = num3;
								auctionInfo.PayType = num;
								auctionInfo.Price = num2;
								auctionInfo.Rise = num2 / 10;
								auctionInfo.Rise = ((auctionInfo.Rise < 1) ? 1 : auctionInfo.Rise);
								auctionInfo.Name = itemAt.Template.Name;
								auctionInfo.Category = itemAt.Template.CategoryID;
								auctionInfo.ValidDate = ((num4 == 0) ? 8 : ((num4 == 1) ? 24 : 48));
								auctionInfo.TemplateID = itemAt.TemplateID;
								auctionInfo.goodsCount = num5;
								auctionInfo.Random = ThreadSafeRandom.NextStatic(GameProperties.BeginAuction, GameProperties.EndAuction);
								using (PlayerBussiness playerBussiness = new PlayerBussiness())
								{
									if (playerBussiness.AddAuction(auctionInfo))
									{
										itemAt.Count = num5;
										client.Player.TakeOutItem(itemAt);
										if (num8 > 0)
										{
											client.Player.AddTemplate(itemAt, bagType, num8);
										}
										client.Player.SaveIntoDatabase();
										client.Player.RemoveGold(num7);
										translateId = "AuctionAddHandler.Msg6";
										client.Out.SendAuctionRefresh(auctionInfo, auctionInfo.AuctionID, true, itemAt);
									}
								}
							}
						}
					}
				}
			}
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId, new object[0]));
			return 0;
		}
	}
}
