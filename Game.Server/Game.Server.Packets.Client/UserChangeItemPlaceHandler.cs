using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(49, "改变物品位置")]
	public class UserChangeItemPlaceHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			eBageType eBageType = (eBageType)packet.ReadByte();
			int num = packet.ReadInt();
			eBageType eBageType2 = (eBageType)packet.ReadByte();
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			packet.ReadBoolean();
			PlayerInventory inventory = client.Player.GetInventory(eBageType);
			PlayerInventory inventory2 = client.Player.GetInventory(eBageType2);
			ItemInfo itemAt = inventory.GetItemAt(num);
			if (eBageType2 == eBageType.TempBag)
			{
				GameServer.log.Error("User want to put item into temp bag!");
				return 0;
			}
			if (inventory == null || itemAt == null)
			{
				return 0;
			}
			ItemInfo itemAt2 = inventory2.GetItemAt(num2);
			if (itemAt2 != null)
			{
				if (itemAt2.ItemID == itemAt.ItemID)
				{
					GameServer.log.Error(string.Concat(new string[]
					{
						"Hack Trung ItemID ",
						client.Player.PlayerCharacter.UserName,
						"-[",
						client.Player.PlayerCharacter.NickName,
						"]"
					}));
                    client.Disconnect();//kick user hack
					return 0;
				}
				if (inventory == inventory2 && itemAt2.Place == itemAt.Place)
				{
					GameServer.log.Error(string.Concat(new string[]
					{
						"Hack Trung PLace ",
						client.Player.PlayerCharacter.UserName,
						"-[",
						client.Player.PlayerCharacter.NickName,
						"]"
					}));
					return 0;
				}
			}
			if (num3 < 0 || num3 > itemAt.Count)
			{
				num3 = itemAt.Count;
			}
			inventory.BeginChanges();
			inventory2.BeginChanges();
			try
			{
				if (eBageType2 == eBageType.Consortia)
				{
					ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
					if (consortiaInfo != null)
					{
						inventory2.Capalility = consortiaInfo.StoreLevel * 10;
					}
				}
				if (num2 == -1)
				{
					bool flag = false;
					if (eBageType == eBageType.CaddyBag && eBageType2 == eBageType.BeadBag)
					{
						num2 = inventory2.FindFirstEmptySlot();
						if (inventory2.AddItemTo(itemAt, num2))
						{
							inventory.TakeOutItem(itemAt);
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						if (eBageType == eBageType2 && eBageType2 == eBageType.MainBag)
						{
							num2 = inventory2.FindFirstEmptySlot();
							if (!inventory.MoveItem(num, num2, num3))
							{
								flag = true;
							}
						}
						else
						{
							if (inventory2.StackItemToAnother(itemAt) || inventory2.AddItem(itemAt))
							{
								inventory.TakeOutItem(itemAt);
							}
							else
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]));
					}
				}
				else
				{
					if (eBageType == eBageType2)
					{
						inventory.MoveItem(num, num2, num3);
						client.Player.OnNewGearEvent(itemAt.Template.CategoryID);
					}
					else
					{
						if (eBageType == eBageType.Store)
						{
							this.MoveFromStore(client, inventory, itemAt, num2, inventory2, num3);
						}
						else
						{
							if (eBageType == eBageType.Consortia)
							{
								UserChangeItemPlaceHandler.MoveFromBank(client, num, num2, inventory, inventory2, itemAt);
							}
							else
							{
								if (eBageType2 == eBageType.Store)
								{
									this.MoveToStore(client, inventory, itemAt, num2, inventory2, num3);
								}
								else
								{
									if (eBageType2 == eBageType.Consortia)
									{
										UserChangeItemPlaceHandler.MoveToBank(num, num2, inventory, inventory2, itemAt);
									}
									else
									{
										if (inventory2.AddItemTo(itemAt, num2))
										{
											inventory.TakeOutItem(itemAt);
										}
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				inventory.CommitChanges();
				inventory2.CommitChanges();
			}
			return 0;
		}
		public void MoveFromStore(GameClient client, PlayerInventory storeBag, ItemInfo item, int toSlot, PlayerInventory bag, int count)
		{
			if (client.Player != null && item != null && storeBag != null && bag != null && item.Template.BagType == (eBageType)bag.BagType)
			{
				if (toSlot < bag.BeginSlot || toSlot > bag.Capalility)
				{
					if (bag.StackItemToAnother(item))
					{
						storeBag.RemoveItem(item, eItemRemoveType.Stack);
						return;
					}
					string key = string.Format("temp_place_{0}", item.ItemID);
					if (client.Player.TempProperties.ContainsKey(key))
					{
						toSlot = (int)storeBag.Player.TempProperties[key];
						storeBag.Player.TempProperties.Remove(key);
					}
					else
					{
						toSlot = bag.FindFirstEmptySlot();
					}
				}
				if (bag.StackItemToAnother(item) || bag.AddItemTo(item, toSlot))
				{
					storeBag.TakeOutItem(item);
					return;
				}
				toSlot = bag.FindFirstEmptySlot();
				if (bag.AddItemTo(item, toSlot))
				{
					storeBag.TakeOutItem(item);
					return;
				}
				storeBag.TakeOutItem(item);
				client.Player.SendItemToMail(item, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), eMailType.ItemOverdue);
				client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
		public void MoveToStore(GameClient client, PlayerInventory bag, ItemInfo item, int toSlot, PlayerInventory storeBag, int count)
		{
			if (client.Player != null && bag != null && item != null && storeBag != null)
			{
				int place = item.Place;
				ItemInfo itemAt = storeBag.GetItemAt(toSlot);
				if (itemAt != null)
				{
					if (itemAt.CanStackedTo(item))
					{
						return;
					}
					if (item.Count == 1 && item.BagType == itemAt.BagType)
					{
						bag.TakeOutItem(item);
						storeBag.TakeOutItem(itemAt);
						bag.AddItemTo(itemAt, place);
						storeBag.AddItemTo(item, toSlot);
						return;
					}
					string key = string.Format("temp_place_{0}", itemAt.ItemID);
					PlayerInventory itemInventory = client.Player.GetItemInventory(itemAt.Template);
					if (client.Player.TempProperties.ContainsKey(key) && itemInventory.BagType == 0)
					{
						int place2 = (int)client.Player.TempProperties[key];
						client.Player.TempProperties.Remove(key);
						if (itemInventory.AddItemTo(itemAt, place2))
						{
							storeBag.TakeOutItem(itemAt);
						}
					}
					else
					{
						if (itemInventory.StackItemToAnother(itemAt))
						{
							storeBag.RemoveItem(itemAt, eItemRemoveType.Stack);
						}
						else
						{
							if (itemInventory.AddItem(itemAt))
							{
								storeBag.TakeOutItem(itemAt);
							}
							else
							{
								client.Player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]));
							}
						}
					}
				}
				if (storeBag.IsEmpty(toSlot))
				{
					if (item.Count == 1)
					{
						if (storeBag.AddItemTo(item, toSlot))
						{
							bag.TakeOutItem(item);
							if (item.Template.BagType == eBageType.MainBag && place < 31)
							{
								string key = string.Format("temp_place_{0}", item.ItemID);
								if (client.Player.TempProperties.ContainsKey(key))
								{
									client.Player.TempProperties[key] = place;
									return;
								}
								client.Player.TempProperties.Add(key, place);
								return;
							}
						}
					}
					else
					{
						ItemInfo itemInfo = item.Clone();
						itemInfo.Count = count;
						if (bag.RemoveCountFromStack(item, count, eItemRemoveType.Stack) && !storeBag.AddItemTo(itemInfo, toSlot))
						{
							bag.AddCountToStack(item, count);
						}
					}
				}
			}
		}
		private static void MoveToBank(int place, int toplace, PlayerInventory bag, PlayerInventory bank, ItemInfo item)
		{
			if (bag != null && item != null && bag != null)
			{
				ItemInfo itemAt = bank.GetItemAt(toplace);
				if (itemAt != null)
				{
					if (item.CanStackedTo(itemAt) && item.Count + itemAt.Count <= item.Template.MaxCount)
					{
						if (bank.AddCountToStack(itemAt, item.Count))
						{
							bag.RemoveItem(item, eItemRemoveType.Stack);
							return;
						}
					}
					else
					{
						if (itemAt.Template.BagType == (eBageType)bag.BagType)
						{
							bag.TakeOutItem(item);
							bank.TakeOutItem(itemAt);
							bag.AddItemTo(itemAt, place);
							bank.AddItemTo(item, toplace);
							return;
						}
					}
				}
				else
				{
					if (bank.AddItemTo(item, toplace))
					{
						bag.TakeOutItem(item);
					}
				}
			}
		}
		private static void MoveFromBank(GameClient client, int place, int toplace, PlayerInventory bag, PlayerInventory tobag, ItemInfo item)
		{
			if (item != null)
			{
				PlayerInventory itemInventory = client.Player.GetItemInventory(item.Template);
				if (itemInventory == tobag)
				{
					ItemInfo itemAt = itemInventory.GetItemAt(toplace);
					if (itemAt == null)
					{
						if (itemInventory.AddItemTo(item, toplace))
						{
							bag.TakeOutItem(item);
							return;
						}
					}
					else
					{
						if (!item.CanStackedTo(itemAt) || item.Count + itemAt.Count > item.Template.MaxCount)
						{
							itemInventory.TakeOutItem(itemAt);
							bag.TakeOutItem(item);
							itemInventory.AddItemTo(item, toplace);
							bag.AddItemTo(itemAt, place);
							return;
						}
						if (itemInventory.AddCountToStack(itemAt, item.Count))
						{
							bag.RemoveItem(item, eItemRemoveType.Stack);
							return;
						}
					}
				}
				else
				{
					if (itemInventory.AddItem(item))
					{
						bag.TakeOutItem(item);
					}
				}
			}
		}
	}
}
