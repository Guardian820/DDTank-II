using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(170, "场景用户离开")]
	public class CardSlotHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			string text = "";
			List<UsersCardInfo> items = client.Player.CardBag.GetItems(0, 5);
			switch (num)
			{
			case 0:
				if (num3 <= client.Player.PlayerCharacter.CardSoul && num3 > 0)
				{
					int type = items[num2].Type;
					int gP = items[num2].CardGP + num3;
					int level = CardMgr.GetLevel(gP, type);
					int num4 = CardMgr.GetGP(level, type) - items[num2].CardGP;
					if (level == 40)
					{
						num3 = num4;
					}
					client.Player.CardBag.UpGraceSlot(num3, level, num2);
					client.Player.RemoveCardSoul(num3);
					client.Player.Out.SendPlayerCardSlot(client.Player.PlayerCharacter, items[num2]);
					client.Player.MainBag.UpdatePlayerProperties();
				}
				else
				{
					text = "Thẻ hồn không đủ!";
				}
				break;

			case 1:
				if (client.Player.PlayerCharacter.Money >= 300)
				{
					int num5 = 0;
					for (int i = 0; i < items.Count; i++)
					{
						num5 += items[i].CardGP;
					}
					client.Player.CardBag.ResetCardSoul();
					client.Player.AddCardSoul(num5);
					text = LanguageMgr.GetTranslation("UpdateSLOT.ResetComplete", new object[]
					{
						num5
					});
					client.Player.RemoveMoney(300);
					client.Player.Out.SendPlayerCardSlot(client.Player.PlayerCharacter, items);
					client.Player.MainBag.UpdatePlayerProperties();
				}
				else
				{
					text = "Xu không đủ!";
				}
				break;
			}
			if (text != "")
			{
				client.Out.SendMessage(eMessageType.Normal, text);
			}
			return 0;
		}
	}
}
