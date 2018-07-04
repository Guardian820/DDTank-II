using Bussiness;
using Game.Base.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(193, "更新拍卖")]
	public class AuctionUpdateHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			bool val = false;
			GSPacketIn gSPacketIn = new GSPacketIn(193, client.Player.PlayerCharacter.ID);
			string translateId = "AuctionUpdateHandler.Fail";
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				AuctionInfo auctionSingle = playerBussiness.GetAuctionSingle(num);
				if (auctionSingle == null)
				{
					translateId = "AuctionUpdateHandler.Msg1";
				}
				else
				{
					if (auctionSingle.PayType == 0 && num2 > client.Player.PlayerCharacter.Gold)
					{
						translateId = "AuctionUpdateHandler.Msg2";
					}
					else
					{
						if (auctionSingle.PayType == 1 && num2 > client.Player.PlayerCharacter.Money)
						{
							translateId = "AuctionUpdateHandler.Msg3";
						}
						else
						{
							if (auctionSingle.BuyerID == 0 && auctionSingle.Price > num2)
							{
								translateId = "AuctionUpdateHandler.Msg4";
							}
							else
							{
								if (auctionSingle.BuyerID != 0 && auctionSingle.Price + auctionSingle.Rise > num2 && (auctionSingle.Mouthful == 0 || auctionSingle.Mouthful > num2))
								{
									translateId = "AuctionUpdateHandler.Msg5";
								}
								else
								{
									int buyerID = auctionSingle.BuyerID;
									auctionSingle.BuyerID = client.Player.PlayerCharacter.ID;
									auctionSingle.BuyerName = client.Player.PlayerCharacter.NickName;
									auctionSingle.Price = num2;
									if (auctionSingle.Mouthful != 0 && num2 >= auctionSingle.Mouthful)
									{
										auctionSingle.Price = auctionSingle.Mouthful;
										auctionSingle.IsExist = false;
									}
									if (playerBussiness.UpdateAuction(auctionSingle))
									{
										if (auctionSingle.PayType == 0)
										{
											client.Player.RemoveGold(auctionSingle.Price);
										}
										else
										{
											client.Player.RemoveMoney(auctionSingle.Price);
											LogMgr.LogMoneyAdd(LogMoneyType.Auction, LogMoneyType.Auction_Update, client.Player.PlayerCharacter.ID, auctionSingle.Price, client.Player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
										}
										if (auctionSingle.IsExist)
										{
											translateId = "AuctionUpdateHandler.Msg6";
										}
										else
										{
											translateId = "AuctionUpdateHandler.Msg7";
											client.Out.SendMailResponse(auctionSingle.AuctioneerID, eMailRespose.Receiver);
											client.Out.SendMailResponse(auctionSingle.BuyerID, eMailRespose.Receiver);
										}
										if (buyerID != 0)
										{
											client.Out.SendMailResponse(buyerID, eMailRespose.Receiver);
										}
										val = true;
									}
								}
							}
						}
					}
				}
				client.Out.SendAuctionRefresh(auctionSingle, num, auctionSingle != null && auctionSingle.IsExist, null);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId, new object[0]));
			}
			gSPacketIn.WriteBoolean(val);
			gSPacketIn.WriteInt(num);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
