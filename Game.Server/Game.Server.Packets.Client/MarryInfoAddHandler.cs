using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(236, "添加征婚信息")]
	public class MarryInfoAddHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.MarryInfoID != 0)
			{
				return 1;
			}
			bool isPublishEquip = packet.ReadBoolean();
			string introduction = packet.ReadString();
			int iD = client.Player.PlayerCharacter.ID;
			eMessageType type = eMessageType.Normal;
			string translateId = "MarryInfoAddHandler.Fail";
			int num = 10000;
			if (num > client.Player.PlayerCharacter.Gold)
			{
				type = eMessageType.ERROR;
				translateId = "MarryInfoAddHandler.Msg1";
			}
			else
			{
				client.Player.SaveIntoDatabase();
				MarryInfo marryInfo = new MarryInfo();
				marryInfo.UserID = iD;
				marryInfo.IsPublishEquip = isPublishEquip;
				marryInfo.Introduction = introduction;
				marryInfo.RegistTime = DateTime.Now;
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					if (playerBussiness.AddMarryInfo(marryInfo))
					{
						client.Player.RemoveGold(num);
						translateId = "MarryInfoAddHandler.Msg2";
						client.Player.PlayerCharacter.MarryInfoID = marryInfo.ID;
						client.Out.SendMarryInfoRefresh(marryInfo, marryInfo.ID, true);
					}
				}
			}
			client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId, new object[0]));
			return 0;
		}
	}
}
