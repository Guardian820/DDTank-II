using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(77, "物品过期")]
	public class ItemOverdueHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.IsPlaying)
			{
				return 0;
			}
			int num = (int)packet.ReadByte();
			int num2 = packet.ReadInt();
			PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
			ItemInfo itemAt = inventory.GetItemAt(num2);
			if (itemAt != null && !itemAt.IsValidItem())
			{
				if (num == 0 && num2 < 11)
				{
					int num3 = inventory.FindFirstEmptySlot(31);
					if (num3 != -1)
					{
						inventory.RemoveItem(itemAt);
						return 0;
					}
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						if (playerBussiness.SendMail(new MailInfo
						{
							Annex1 = itemAt.ItemID.ToString(),
							Content = LanguageMgr.GetTranslation("ItemOverdueHandler.Content", new object[0]),
							Gold = 0,
							IsExist = true,
							Money = 0,
							Receiver = client.Player.PlayerCharacter.NickName,
							ReceiverID = itemAt.UserID,
							Sender = client.Player.PlayerCharacter.NickName,
							SenderID = itemAt.UserID,
							Title = LanguageMgr.GetTranslation("ItemOverdueHandler.Title", new object[0]),
							Type = 9
						}))
						{
							inventory.RemoveItem(itemAt);
						}
						return 0;
					}
				}
				inventory.UpdateItem(itemAt);
			}
			return 0;
		}
	}
}
