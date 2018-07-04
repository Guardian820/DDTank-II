using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(222, "场景用户离开")]
	public class EquipRetrieveHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			PlayerInventory inventory = client.Player.GetInventory(eBageType.Store);
			PlayerInventory arg_19_0 = client.Player.PropBag;
			int num = 0;
			bool isBinds = true;
			for (int i = 1; i < 5; i++)
			{
				ItemInfo itemAt = inventory.GetItemAt(i);
				if (itemAt != null)
				{
					inventory.RemoveItemAt(i);
				}
				if (itemAt.IsBinds)
				{
					isBinds = true;
				}
				num += itemAt.Template.Quality;
			}
			int[] array = new int[]
			{
				7015,
				7016,
				7017,
				7018,
				7019,
				7021,
				7022,
				7023,
				7048,
				11041,
				15019,
				16015
			};
			Random random = new Random();
			int num2 = random.Next(0, array.Length - 1);
			int templateId = array[num2];
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 105);
			itemInfo.IsBinds = isBinds;
			itemInfo.BeginDate = DateTime.Now;
			if (itemInfo.Template.CategoryID != 11)
			{
				itemInfo.ValidDate = 7;
			}
			itemInfo.RemoveDate = DateTime.Now.AddDays(7.0);
			inventory.AddItemTo(itemInfo, 0);
			return 1;
		}
	}
}
