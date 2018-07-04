using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(108, "选取")]
	public class GameTakeTempItemsHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			string empty = string.Empty;
			int num = packet.ReadInt();
			if (num != -1)
			{
				ItemInfo itemAt = client.Player.TempBag.GetItemAt(num);
				this.GetItem(client.Player, itemAt, ref empty);
			}
			else
			{
				List<ItemInfo> items = client.Player.TempBag.GetItems();
				foreach (ItemInfo current in items)
				{
					if (!this.GetItem(client.Player, current, ref empty))
					{
						break;
					}
				}
			}
			client.Player.SaveIntoDatabase();
			if (!string.IsNullOrEmpty(empty))
			{
				client.Out.SendMessage(eMessageType.ERROR, empty);
			}
			return 0;
		}
		private bool GetItem(GamePlayer player, ItemInfo item, ref string message)
		{
			if (item == null)
			{
				return false;
			}
			PlayerInventory itemInventory = player.GetItemInventory(item.Template);
			if (itemInventory.AddItem(item))
			{
				player.TempBag.RemoveItem(item);
				item.IsExist = true;
				return true;
			}
			itemInventory.UpdateChangedPlaces();
			message = LanguageMgr.GetTranslation(item.GetBagName(), new object[0]) + LanguageMgr.GetTranslation("GameTakeTempItemsHandler.Msg", new object[0]);
			return false;
		}
	}
}
