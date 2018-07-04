using Bussiness;
using Game.Base.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(248, "离婚")]
	internal class DivorceApplyHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			bool flag = packet.ReadBoolean();
			if (!client.Player.PlayerCharacter.IsMarried)
			{
				return 1;
			}
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			if (client.Player.PlayerCharacter.IsCreatedMarryRoom)
			{
				client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("DivorceApplyHandler.Msg2", new object[0]));
				return 1;
			}
			int num = GameProperties.PRICE_DIVORCED;
			if (flag)
			{
				num = GameProperties.PRICE_DIVORCED_DISCOUNT;
			}
			if (client.Player.PlayerCharacter.Money < num)
			{
				client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DivorceApplyHandler.Msg1", new object[0]));
				return 1;
			}
			client.Player.RemoveMoney(num);
			LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_Unmarry, client.Player.PlayerCharacter.ID, num, client.Player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
			CountBussiness.InsertSystemPayCount(client.Player.PlayerCharacter.ID, num, 0, 0, 3);
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(client.Player.PlayerCharacter.SpouseID);
				if (userSingleByUserID == null || userSingleByUserID.Sex == client.Player.PlayerCharacter.Sex)
				{
					return 1;
				}
				MarryApplyInfo marryApplyInfo = new MarryApplyInfo();
				marryApplyInfo.UserID = client.Player.PlayerCharacter.SpouseID;
				marryApplyInfo.ApplyUserID = client.Player.PlayerCharacter.ID;
				marryApplyInfo.ApplyUserName = client.Player.PlayerCharacter.NickName;
				marryApplyInfo.ApplyType = 3;
				marryApplyInfo.LoveProclamation = "";
				marryApplyInfo.ApplyResult = false;
				int num2 = 0;
				if (playerBussiness.SavePlayerMarryNotice(marryApplyInfo, 0, ref num2))
				{
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(userSingleByUserID.ID);
					client.Player.LoadMarryProp();
				}
			}
			client.Player.QuestInventory.ClearMarryQuest();
			client.Player.Out.SendPlayerDivorceApply(client.Player, true, true);
			client.Player.SendMessage(LanguageMgr.GetTranslation("DivorceApplyHandler.Msg3", new object[0]));
			return 0;
		}
	}
}
