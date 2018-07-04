using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Linq;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(6)]
	public class UserFirecrackersCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom != null)
			{
				packet.ReadInt();
				int templatID = packet.ReadInt();
				ShopItemInfo shopItemInfo = ShopMgr.FindShopbyTemplatID(templatID).FirstOrDefault<ShopItemInfo>();
				if (shopItemInfo != null)
				{
					if (shopItemInfo.APrice1 == -2)
					{
						if (player.PlayerCharacter.Gold >= shopItemInfo.AValue1)
						{
							player.RemoveGold(shopItemInfo.AValue1);
							player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.Successed1", new object[]
							{
								shopItemInfo.AValue1
							}));
							return true;
						}
						player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserFirecrackersCommand.GoldNotEnough", new object[0]));
					}
					if (shopItemInfo.APrice1 == -1)
					{
						if (player.PlayerCharacter.Money >= shopItemInfo.AValue1)
						{
							player.RemoveMoney(shopItemInfo.AValue1);
							LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_Flower, player.PlayerCharacter.ID, shopItemInfo.AValue1, player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
							player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.Successed2", new object[]
							{
								shopItemInfo.AValue1
							}));
							return true;
						}
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough", new object[0]));
					}
				}
			}
			return false;
		}
	}
}
