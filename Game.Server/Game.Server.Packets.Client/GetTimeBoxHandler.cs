using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(53, "场景用户离开")]
	public class GetTimeBoxHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int receiebox = packet.ReadInt();
			packet.ReadInt();
			packet.ReadInt();
			bool result = false;
			List<ItemInfo> list = new List<ItemInfo>();
			int iD = client.Player.PlayerCharacter.ID;
			int receiebox2 = client.Player.PlayerCharacter.receiebox;
			string message = "Nhận rương thời gian thành công!";
			switch (num)
			{
			case 0:
				client.Player.UpdateTimeBox(receiebox, 20, 0);
				client.Out.SendGetBoxTime(iD, receiebox2, result);
				break;

			case 1:
				result = true;
				list = ItemBoxMgr.GetItemBoxAward(ItemMgr.FindItemBoxTemplate(receiebox2).TemplateID);
				foreach (ItemInfo current in list)
				{
					if (!client.Player.AddTemplate(current, current.Template.BagType, current.Count))
					{
						using (PlayerBussiness playerBussiness = new PlayerBussiness())
						{
							current.UserID = 0;
							playerBussiness.AddGoods(current);
							MailInfo mailInfo = new MailInfo();
							mailInfo.Annex1 = current.ItemID.ToString();
							mailInfo.Content = "Phần thưởng từ rương thời gian.";
							mailInfo.Gold = 0;
							mailInfo.Money = 0;
							mailInfo.Receiver = client.Player.PlayerCharacter.NickName;
							mailInfo.ReceiverID = client.Player.PlayerCharacter.ID;
							mailInfo.Sender = mailInfo.Receiver;
							mailInfo.SenderID = mailInfo.ReceiverID;
							mailInfo.Title = "Mở rương thời gian!";
							mailInfo.Type = 12;
							playerBussiness.SendMail(mailInfo);
							message = "Túi đã đầy, vật phẩm đã được chuyển vào thư!";
						}
						client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
				}
				client.Out.SendGetBoxTime(iD, receiebox2, result);
				client.Out.SendMessage(eMessageType.Normal, message);
				break;
			}
			return 0;
		}
	}
}
