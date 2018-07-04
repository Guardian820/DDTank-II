using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(18, "场景用户离开")]
	public class CardInfoHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(num);
			PlayerInfo playerInfo;
			List<UsersCardInfo> list;
			if (playerById != null)
			{
				playerInfo = playerById.PlayerCharacter;
				list = playerById.CardBag.GetItems(0, 5);
			}
			else
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					playerInfo = playerBussiness.GetUserSingleByUserID(num);
					list = playerBussiness.GetUserCardEuqip(num);
				}
			}
			if (list != null && playerInfo != null)
			{
				client.Player.Out.SendPlayerCardSlot(playerInfo, list);
			}
			return 0;
		}
	}
}
