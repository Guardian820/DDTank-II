using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.GameUtils
{
	public class PlayerBeadInventory : PlayerInventory
	{
		private const int BAG_START = 32;
		public PlayerBeadInventory(GamePlayer player) : base(player, true, 179, 21, 32, false)
		{
		}
		public override void LoadFromDatabase()
		{
			base.BeginChanges();
			try
			{
				base.LoadFromDatabase();
				List<ItemInfo> list = new List<ItemInfo>();
				for (int i = 1; i < 32; i++)
				{
					ItemInfo itemInfo = this.m_items[i];
					if (this.m_items[i] != null && !this.m_items[i].IsValidItem())
					{
						int num = base.FindFirstEmptySlot(32);
						if (num >= 0)
						{
							this.MoveItem(itemInfo.Place, num, itemInfo.Count);
						}
						else
						{
							list.Add(itemInfo);
						}
					}
				}
				if (list.Count > 0)
				{
					this.m_player.SendItemsToMail(list, null, null, eMailType.ItemOverdue);
					this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
			}
			finally
			{
				base.CommitChanges();
			}
		}
		public override bool MoveItem(int fromSlot, int toSlot, int count)
		{
			return this.m_items[fromSlot] != null && base.MoveItem(fromSlot, toSlot, count);
		}
		public override void UpdateChangedPlaces()
		{
			int[] array = this.m_changedPlaces.ToArray();
			int[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				int slot = array2[i];
				if (this.IsEquipSlot(slot))
				{
					ItemInfo itemAt = this.GetItemAt(slot);
					if (itemAt != null)
					{
						itemAt.IsBinds = true;
						if (!itemAt.IsUsed)
						{
							itemAt.BeginDate = DateTime.Now;
						}
					}
					break;
				}
			}
			base.UpdateChangedPlaces();
		}
		public int FindItemEpuipSlot(ItemTemplateInfo item)
		{
			switch (item.CategoryID)
			{
			case 8:
				if (this.m_items[7] == null)
				{
					return 7;
				}
				return 8;

			case 9:
				if (this.m_items[9] == null)
				{
					return 9;
				}
				return 10;

			case 13:
				return 11;

			case 14:
				return 12;

			case 15:
				return 13;

			case 16:
				return 14;
			}
			return item.CategoryID - 1;
		}
		public bool CanEquipSlotContains(int slot, ItemTemplateInfo temp)
		{
			if (temp.CategoryID == 8)
			{
				return slot == 7 || slot == 8;
			}
			if (temp.CategoryID == 9)
			{
				if (temp.TemplateID == 9022 || temp.TemplateID == 9122 || temp.TemplateID == 9222 || temp.TemplateID == 9322 || temp.TemplateID == 9422 || temp.TemplateID == 9522)
				{
					return slot == 9 || slot == 10 || slot == 16;
				}
				return slot == 9 || slot == 10;
			}
			else
			{
				if (temp.CategoryID == 13)
				{
					return slot == 11;
				}
				if (temp.CategoryID == 14)
				{
					return slot == 12;
				}
				if (temp.CategoryID == 15)
				{
					return slot == 13;
				}
				if (temp.CategoryID == 16)
				{
					return slot == 14;
				}
				if (temp.CategoryID == 17)
				{
					return slot == 15;
				}
				return temp.CategoryID - 1 == slot;
			}
		}
		public bool IsEquipSlot(int slot)
		{
			return slot >= 0 && slot < 32;
		}
	}
}
