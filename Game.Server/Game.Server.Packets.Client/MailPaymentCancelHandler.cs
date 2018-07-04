using Bussiness;
using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(118, "取消付款邮件")]
	public class MailPaymentCancelHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int mailID = packet.ReadInt();
			int playerID = 0;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				if (playerBussiness.CancelPaymentMail(client.Player.PlayerCharacter.ID, mailID, ref playerID))
				{
					client.Out.SendMailResponse(playerID, eMailRespose.Receiver);
					packet.WriteBoolean(true);
				}
				else
				{
					packet.WriteBoolean(false);
				}
			}
			client.Out.SendTCP(packet);
			return 1;
		}
	}
}
