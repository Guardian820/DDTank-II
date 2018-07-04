using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(136, "场景用户离开")]
	public class OpenOneTotemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.Grade < 20)
			{
				client.Out.SendMessage(eMessageType.Normal, "Hack level sao bạn!");
				return 0;
			}
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 1;
			}
			int totemId = client.Player.PlayerCharacter.totemId;
			int consumeExp = TotemMgr.FindTotemInfo(totemId).ConsumeExp;
			int consumeHonor = TotemMgr.FindTotemInfo(totemId).ConsumeHonor;
			if (client.Player.PlayerCharacter.Money >= consumeExp && client.Player.PlayerCharacter.myHonor >= consumeHonor)
			{
				if (totemId == 0)
				{
					client.Player.AddTotem(10001);
				}
				else
				{
					client.Player.AddTotem(1);
				}
				client.Player.AddExpVip(consumeExp);
				client.Player.RemoveMoney(consumeExp);
				client.Player.RemovemyHonor(consumeHonor);
				client.Player.Out.SendPlayerRefreshTotem(client.Player.PlayerCharacter);
				client.Player.MainBag.UpdatePlayerProperties();
				client.Player.OnUserToemGemstoneEvent();
			}
			return 0;
		}
	}
}
