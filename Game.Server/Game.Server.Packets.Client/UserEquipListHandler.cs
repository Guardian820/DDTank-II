using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(74, "获取用户装备")]
	public class UserEquipListHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			bool flag = packet.ReadBoolean();
			int num = 0;
			GamePlayer gamePlayer;
			if (flag)
			{
				num = packet.ReadInt();
				gamePlayer = WorldMgr.GetPlayerById(num);
			}
			else
			{
				string nickName = packet.ReadString();
				gamePlayer = WorldMgr.GetClientByPlayerNickName(nickName);
			}
			PlayerInfo playerInfo;
			List<ItemInfo> list;
			List<ItemInfo> list2;
			List<UserGemStone> list3;
			if (gamePlayer != null)
			{
				playerInfo = gamePlayer.PlayerCharacter;
				list = gamePlayer.MainBag.GetItems(0, 31);
				list2 = gamePlayer.BeadBag.GetItems(0, 31);
				list3 = gamePlayer.GemStone;
			}
			else
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					playerInfo = playerBussiness.GetUserSingleByUserID(num);
					if (playerInfo == null)
					{
						return 0;
					}
					list = playerBussiness.GetUserEuqip(num);
					list2 = playerBussiness.GetUserBeadEuqip(num);
					playerInfo.Texp = playerBussiness.GetUserTexpInfoSingle(num);
					list3 = playerBussiness.GetSingleGemStones(num);
				}
			}
			if (playerInfo != null && list != null && list2 != null && list3 != null)
			{
				client.Player.Out.SendUserEquip(playerInfo, list, list3, list2);
			}
			return 0;
		}
	}
}
