using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(194, "撤消拍卖")]
	public class AuctionDeleteHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int auctionID = packet.ReadInt();
			string translation = LanguageMgr.GetTranslation("AuctionDeleteHandler.Fail", new object[0]);
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				if (playerBussiness.DeleteAuction(auctionID, client.Player.PlayerCharacter.ID, ref translation))
				{
					client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
					client.Out.SendAuctionRefresh(null, auctionID, false, null);
				}
				else
				{
					AuctionInfo auctionSingle = playerBussiness.GetAuctionSingle(auctionID);
					client.Out.SendAuctionRefresh(auctionSingle, auctionID, auctionSingle != null, null);
				}
				client.Out.SendMessage(eMessageType.Normal, translation);
			}
			return 0;
		}
	}
}
