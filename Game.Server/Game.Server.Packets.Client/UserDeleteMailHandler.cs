using Bussiness;
using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(112, "删除邮件")]
	public class UserDeleteMailHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			int mailID = packet.ReadInt();
			GSPacketIn gSPacketIn = packet.Clone();
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				int playerID;
				if (playerBussiness.DeleteMail(client.Player.PlayerCharacter.ID, mailID, out playerID))
				{
					client.Out.SendMailResponse(playerID, eMailRespose.Receiver);
					gSPacketIn.WriteBoolean(true);
				}
				else
				{
					gSPacketIn.WriteBoolean(false);
				}
			}
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
