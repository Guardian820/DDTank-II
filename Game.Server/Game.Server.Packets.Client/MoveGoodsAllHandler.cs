using Game.Base.Packets;
using Game.Server.GameUtils;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Server.Packets.Client
{
	[PacketHandler(124, "物品比较")]
	public class MoveGoodsAllHandler : IPacketHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			bool flag = packet.ReadBoolean();
			int num = packet.ReadInt();
			int bageType = packet.ReadInt();
			PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
			List<ItemInfo> items = inventory.GetItems(inventory.BeginSlot, inventory.Capalility);
			if (num == items.Count)
			{
				inventory.BeginChanges();
				try
				{
					ItemInfo[] rawSpaces = inventory.GetRawSpaces();
					inventory.ClearBag();
					for (int i = 0; i < num; i++)
					{
						int num2 = packet.ReadInt();
						int num3 = packet.ReadInt();
						ItemInfo item = rawSpaces[num2];
						if (!inventory.AddItemTo(item, num3))
						{
							throw new Exception(string.Format("move item error: old place:{0} new place:{1}", num2, num3));
						}
					}
				}
				catch (Exception ex)
				{
					MoveGoodsAllHandler.log.ErrorFormat("Arrage bag errror,user id:{0}   msg:{1}", client.Player.PlayerId, ex.Message);
				}
				finally
				{
					if (flag)
					{
						items = inventory.GetItems();
						List<int> list = new List<int>();
						for (int j = 0; j < items.Count; j++)
						{
							if (!list.Contains(j))
							{
								for (int k = items.Count - 1; k > j; k--)
								{
									if (!list.Contains(k) && items[j].TemplateID == items[k].TemplateID && items[j].CanStackedTo(items[k]))
									{
										inventory.MoveItem(items[k].Place, items[j].Place, items[k].Count);
										list.Add(k);
									}
								}
							}
						}
						items = inventory.GetItems();
						if (inventory.FindFirstEmptySlot() != -1)
						{
							int num4 = 1;
							while (inventory.FindFirstEmptySlot() < items[items.Count - num4].Place)
							{
								inventory.MoveItem(items[items.Count - num4].Place, inventory.FindFirstEmptySlot(), items[items.Count - num4].Count);
								num4++;
							}
						}
					}
					inventory.CommitChanges();
				}
			}
			return 0;
		}
	}
}
