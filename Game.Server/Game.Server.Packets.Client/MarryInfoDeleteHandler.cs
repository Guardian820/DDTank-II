using Bussiness;
using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(238, "撤消征婚信息")]
	public class MarryInfoDeleteHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			string translation = LanguageMgr.GetTranslation("MarryInfoDeleteHandler.Fail", new object[0]);
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				if (playerBussiness.DeleteMarryInfo(num, client.Player.PlayerCharacter.ID, ref translation))
				{
					client.Out.SendAuctionRefresh(null, num, false, null);
				}
				client.Out.SendMessage(eMessageType.Normal, translation);
			}
			return 0;
		}
	}
}
