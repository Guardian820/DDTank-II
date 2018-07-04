using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Linq;
namespace Game.Server.Packets.Client
{
	[PacketHandler(247, "求婚")]
	internal class MarryApplyHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.IsMarried)
			{
				return 1;
			}
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 1;
			}
			int num = packet.ReadInt();
			string loveProclamation = packet.ReadString();
			packet.ReadBoolean();
			bool flag = true;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(num);
				if (userSingleByUserID == null || userSingleByUserID.Sex == client.Player.PlayerCharacter.Sex)
				{
					int result = 1;
					return result;
				}
				if (userSingleByUserID.IsMarried)
				{
					client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg2", new object[0]));
					int result = 1;
					return result;
				}
				ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, 11103);
				if (itemByTemplateID == null)
				{
					ShopItemInfo shopItemInfo = ShopMgr.FindShopbyTemplatID(11103).FirstOrDefault<ShopItemInfo>();
					if (shopItemInfo == null)
					{
						client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg6", new object[0]));
						int result = 1;
						return result;
					}
					if (client.Player.PlayerCharacter.Money < shopItemInfo.AValue1)
					{
						client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg1", new object[0]));
						int result = 1;
						return result;
					}
					flag = false;
				}
				MarryApplyInfo marryApplyInfo = new MarryApplyInfo();
				marryApplyInfo.UserID = num;
				marryApplyInfo.ApplyUserID = client.Player.PlayerCharacter.ID;
				marryApplyInfo.ApplyUserName = client.Player.PlayerCharacter.NickName;
				marryApplyInfo.ApplyType = 1;
				marryApplyInfo.LoveProclamation = loveProclamation;
				marryApplyInfo.ApplyResult = false;
				int iD = 0;
				if (playerBussiness.SavePlayerMarryNotice(marryApplyInfo, 0, ref iD))
				{
					if (flag)
					{
						client.Player.RemoveItem(itemByTemplateID);
					}
					else
					{
						ShopItemInfo shopItemInfo2 = ShopMgr.FindShopbyTemplatID(11103).FirstOrDefault<ShopItemInfo>();
						client.Player.RemoveMoney(shopItemInfo2.AValue1);
						LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_Spark, client.Player.PlayerCharacter.ID, shopItemInfo2.AValue1, client.Player.PlayerCharacter.Money, 0, 0, 0, 0, "", shopItemInfo2.TemplateID.ToString(), "1");
					}
					client.Player.Out.SendPlayerMarryApply(client.Player, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, loveProclamation, iD);
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(num);
					string arg_2BA_0 = userSingleByUserID.NickName;
					client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg3", new object[0]));
				}
			}
			return 0;
		}
	}
}
