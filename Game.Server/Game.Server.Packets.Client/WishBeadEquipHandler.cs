using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(106, "场景用户离开")]
	public class WishBeadEquipHandler : IPacketHandler
	{
		public static ThreadSafeRandom random = new ThreadSafeRandom();
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int bageType = packet.ReadInt();
			int templateId = packet.ReadInt();
			int place = packet.ReadInt();
			int bagType = packet.ReadInt();
			int templateId2 = packet.ReadInt();
			PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
			ItemInfo itemInfo = inventory.GetItemAt(num);
			client.Player.GetItemAt((eBageType)bagType, place);
			double num2 = 5.0;
			GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = GoldEquipMgr.FindGoldEquipTemplate(templateId);
			GSPacketIn gSPacketIn = new GSPacketIn(106, client.Player.PlayerCharacter.ID);
			if (goldEquipTemplateLoadInfo == null && itemInfo.Template.CategoryID == 7)
			{
				gSPacketIn.WriteInt(5);
			}
			else
			{
				if (!itemInfo.IsGold)
				{
					if (num2 > (double)WishBeadEquipHandler.random.Next(100))
					{
						itemInfo.StrengthenLevel++;
						itemInfo.IsGold = true;
						itemInfo.goldBeginTime = DateTime.Now;
						itemInfo.goldValidDate = 30;
						itemInfo.IsBinds = true;
						if (goldEquipTemplateLoadInfo != null && itemInfo.Template.CategoryID == 7)
						{
							ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(goldEquipTemplateLoadInfo.NewTemplateId);
							if (itemTemplateInfo != null)
							{
								ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 116);
								itemInfo2.StrengthenLevel = itemInfo.StrengthenLevel;
								itemInfo2.IsGold = itemInfo.IsGold;
								itemInfo2.goldBeginTime = itemInfo.goldBeginTime;
								itemInfo2.goldValidDate = itemInfo.goldValidDate;
								itemInfo2.IsBinds = itemInfo.IsBinds;
								ItemInfo.OpenHole(ref itemInfo2);
								StrengthenMgr.InheritProperty(itemInfo, ref itemInfo2);
								inventory.RemoveItemAt(num);
								inventory.AddItemTo(itemInfo2, num);
								itemInfo = itemInfo2;
							}
						}
						inventory.UpdateItem(itemInfo);
						gSPacketIn.WriteInt(0);
						inventory.SaveToDatabase();
					}
					else
					{
						gSPacketIn.WriteInt(1);
					}
					client.Player.RemoveTemplate(templateId2, 1);
				}
				else
				{
					gSPacketIn.WriteInt(6);
				}
			}
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
