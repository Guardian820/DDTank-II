using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(60, "隐藏装备")]
	public class UserHideItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			bool hide = packet.ReadBoolean();
			int categoryID = packet.ReadInt();
			switch (categoryID)
			{
			case 13:
				categoryID = 3;
				break;

			case 15:
				categoryID = 4;
				break;
			}
			client.Player.HideEquip(categoryID, hide);
			return 0;
		}
	}
}
