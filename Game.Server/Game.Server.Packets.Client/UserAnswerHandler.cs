using Game.Base.Packets;
using Game.Logic;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(15, "New User Answer Question")]
	public class UserAnswerHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte b = packet.ReadByte();
			int num = packet.ReadInt();
			bool flag = false;
			if (b == 1)
			{
				flag = packet.ReadBoolean();
			}
			if (b == 1)
			{
				List<ItemInfo> list = null;
				if (DropInventory.AnswerDrop(num, ref list))
				{
					int value = 0;
					int num2 = 0;
					int num3 = 0;
					int num4 = 0;
					foreach (ItemInfo current in list)
					{
						ItemInfo.FindSpecialItemInfo(current, ref value, ref num2, ref num3, ref num4);
						if (current != null && current.Template.BagType == eBageType.PropBag)
						{
							client.Player.MainBag.AddTemplate(current, current.Count);
						}
						client.Player.AddGold(value);
						client.Player.AddMoney(num2);
						client.Player.AddGiftToken(num3);
						LogMgr.LogMoneyAdd(LogMoneyType.Award, LogMoneyType.Award_Answer, client.Player.PlayerCharacter.ID, num3, client.Player.PlayerCharacter.Money, num2, 0, 0, 0, "", "", "");
					}
				}
				if (flag)
				{
					client.Player.PlayerCharacter.openFunction((Step)num);
				}
			}
			if (b == 2)
			{
				client.Player.PlayerCharacter.openFunction((Step)num);
			}
			client.Player.UpdateAnswerSite(num);
			return 1;
		}
	}
}
