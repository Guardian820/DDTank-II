using Bussiness.Managers;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.GameUtils
{
	public class PlayerEquipInventory : PlayerInventory
	{
		private const int BAG_START = 31;
		private static readonly int[] StyleIndex = new int[]
		{
			1,
			2,
			3,
			4,
			5,
			6,
			11,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20
		};
		public PlayerEquipInventory(GamePlayer player) : base(player, true, 80, 0, 31, true)
		{
		}
		public override void LoadFromDatabase()
		{
			base.BeginChanges();
			try
			{
				base.LoadFromDatabase();
				List<ItemInfo> list = new List<ItemInfo>();
				for (int i = 0; i < 31; i++)
				{
					ItemInfo itemInfo = this.m_items[i];
					if (this.m_items[i] != null && !this.m_items[i].IsValidItem())
					{
						int num = base.FindFirstEmptySlot(31);
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
			if (this.m_items[fromSlot] == null || toSlot == -1)
			{
				return false;
			}
			if (this.IsEquipSlot(fromSlot) && !this.IsEquipSlot(toSlot) && this.m_items[toSlot] != null && this.m_items[toSlot].Template.CategoryID != this.m_items[fromSlot].Template.CategoryID)
			{
				if (!this.CanEquipSlotContains(fromSlot, this.m_items[toSlot].Template))
				{
					toSlot = base.FindFirstEmptySlot(31);
				}
			}
			else
			{
				if (this.IsEquipSlot(toSlot))
				{
					if (!this.CanEquipSlotContains(toSlot, this.m_items[fromSlot].Template))
					{
						this.UpdateItem(this.m_items[fromSlot]);
						return false;
					}
					if (!this.m_player.CanEquip(this.m_items[fromSlot].Template) || !this.m_items[fromSlot].IsValidItem())
					{
						this.UpdateItem(this.m_items[fromSlot]);
						return false;
					}
				}
				if (this.IsEquipSlot(fromSlot) && this.m_items[toSlot] != null && !this.CanEquipSlotContains(fromSlot, this.m_items[toSlot].Template))
				{
					this.UpdateItem(this.m_items[toSlot]);
					return false;
				}
			}
			return base.MoveItem(fromSlot, toSlot, count);
		}
		public override void UpdateChangedPlaces()
		{
			int[] array = this.m_changedPlaces.ToArray();
			bool flag = false;
			int[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				int slot = array2[i];
				if (this.IsEquipSlot(slot))
				{
					ItemInfo itemAt = this.GetItemAt(slot);
					if (itemAt != null)
					{
						this.m_player.OnUsingItem(this.GetItemAt(slot).TemplateID);
						itemAt.IsBinds = true;
						if (!itemAt.IsUsed)
						{
							itemAt.IsUsed = true;
							itemAt.BeginDate = DateTime.Now;
						}
					}
					flag = true;
					break;
				}
			}
			base.UpdateChangedPlaces();
			if (flag)
			{
				this.UpdatePlayerProperties();
			}
		}
		public void UpdatePlayerProperties()
		{
			this.m_player.BeginChanges();
			try
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				string text = "";
				string text2 = "";
				string skin = "";
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				int num13 = 0;
				int num14 = 0;
				int num15 = 0;
				int num16 = 0;
				int num17 = 0;
				int num18 = 0;
				int num19 = 0;
				int num20 = 0;
				int num21 = 0;
				int num22 = 0;
				int num23 = 0;
				int num24 = 0;
				int num25 = 0;
				this.m_player.UpdatePet(this.m_player.PetBag.GetPetIsEquip());
				List<UsersCardInfo> items = this.m_player.CardBag.GetItems(0, 5);
				object @lock;
				Monitor.Enter(@lock = this.m_lock);
				try
				{
					text = ((this.m_items[0] == null) ? "" : (this.m_items[0].TemplateID.ToString() + "|" + this.m_items[0].Template.Pic));
					text2 = ((this.m_items[0] == null) ? "" : this.m_items[0].Color);
					skin = ((this.m_items[5] == null) ? "" : this.m_items[5].Skin);
					for (int i = 0; i < 31; i++)
					{
						ItemInfo itemInfo = this.m_items[i];
						if (itemInfo != null)
						{
							num += itemInfo.Attack;
							num2 += itemInfo.Defence;
							num3 += itemInfo.Agility;
							num4 += itemInfo.Luck;
							num6 = ((num6 > itemInfo.StrengthenLevel) ? num6 : itemInfo.StrengthenLevel);
							this.AddBaseLatentProperty(itemInfo, ref num, ref num2, ref num3, ref num4);
							this.AddBaseGemstoneProperty(itemInfo, ref num7, ref num8, ref num9, ref num10, ref num11);
						}
						this.AddBeadProperty(i, ref num, ref num2, ref num3, ref num4, ref num5);
					}
					this.AddBaseTotemProperty(this.m_player.PlayerCharacter, ref num, ref num2, ref num3, ref num4, ref num5);
					if (this.m_player.Pet != null)
					{
						num16 += this.m_player.Pet.Attack;
						num17 += this.m_player.Pet.Defence;
						num18 += this.m_player.Pet.Agility;
						num19 += this.m_player.Pet.Luck;
						num20 += this.m_player.Pet.Blood;
					}
					foreach (UsersCardInfo current in items)
					{
						num12 += CardMgr.GetProp(current, 0);
						num13 += CardMgr.GetProp(current, 1);
						num14 += CardMgr.GetProp(current, 2);
						num15 += CardMgr.GetProp(current, 3);
						if (current.CardID != 0)
						{
							num12 += current.Attack;
							num13 += current.Defence;
							num14 += current.Agility;
							num15 += current.Luck;
						}
					}
					num21 += ExerciseMgr.GetExercise(this.m_player.PlayerCharacter.Texp.attTexpExp, "A");
					num22 += ExerciseMgr.GetExercise(this.m_player.PlayerCharacter.Texp.defTexpExp, "D");
					num23 += ExerciseMgr.GetExercise(this.m_player.PlayerCharacter.Texp.spdTexpExp, "AG");
					num24 += ExerciseMgr.GetExercise(this.m_player.PlayerCharacter.Texp.lukTexpExp, "L");
					num25 += ExerciseMgr.GetExercise(this.m_player.PlayerCharacter.Texp.hpTexpExp, "H");
					for (int j = 0; j < PlayerEquipInventory.StyleIndex.Length; j++)
					{
						text += ",";
						text2 += ",";
						if (this.m_items[PlayerEquipInventory.StyleIndex[j]] != null)
						{
							object obj = text;
							text = string.Concat(new object[]
							{
								obj,
								this.m_items[PlayerEquipInventory.StyleIndex[j]].TemplateID,
								"|",
								this.m_items[PlayerEquipInventory.StyleIndex[j]].Template.Pic
							});
							text2 += this.m_items[PlayerEquipInventory.StyleIndex[j]].Color;
						}
					}
					this.EquipBuffer();
				}
				finally
				{
					Monitor.Exit(@lock);
				}
				num += num7 + num12 + num16 + num21;
				num2 += num8 + num13 + num17 + num22;
				num3 += num9 + num14 + num18 + num23;
				num4 += num10 + num15 + num19 + num24;
				num5 += num11 + num20 + num25;
				this.m_player.UpdateBaseProperties(num, num2, num3, num4, num5);
				this.m_player.UpdateStyle(text, text2, skin);
				this.m_player.ApertureEquip(num6);
				this.m_player.UpdateWeapon(this.m_items[6]);
				this.m_player.UpdateSecondWeapon(this.m_items[15]);
				this.m_player.UpdatePlayerProperty("Texp", this.GetProp(num21, num22, num23, num24, num25));
				this.m_player.UpdatePlayerProperty("Card", this.GetProp(num12, num13, num14, num15, 0));
				this.m_player.UpdatePlayerProperty("Pet", this.GetProp(num16, num17, num18, num19, num20));
				this.m_player.UpdatePlayerProperty("Gem", this.GetProp(num7, num8, num9, num10, num11));
				this.m_player.UpdateFightPower();
				this.GetUserNimbus();
			}
			finally
			{
				this.m_player.CommitChanges();
			}
		}
		public Dictionary<string, int> GetProp(int attack, int defence, int agility, int lucky, int hp)
		{
			return new Dictionary<string, int>
			{

				{
					"Attack",
					attack
				},

				{
					"Defence",
					defence
				},

				{
					"Agility",
					agility
				},

				{
					"Luck",
					lucky
				},

				{
					"HP",
					hp
				}
			};
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
			return slot >= 0 && slot < 31;
		}
		public void GetUserNimbus()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 31; i++)
			{
				ItemInfo itemAt = this.GetItemAt(i);
				if (itemAt != null)
				{
					if (itemAt.StrengthenLevel >= 5 && itemAt.StrengthenLevel <= 8)
					{
						if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
						{
							num = ((num > 1) ? num : 1);
						}
						if (itemAt.Template.CategoryID == 7)
						{
							num2 = ((num2 > 1) ? num2 : 1);
						}
					}
					if (itemAt.StrengthenLevel >= 9 && itemAt.StrengthenLevel <= 11)
					{
						if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
						{
							num = ((num > 1) ? num : 2);
						}
						if (itemAt.Template.CategoryID == 7)
						{
							num2 = ((num2 > 1) ? num2 : 2);
						}
					}
					if (itemAt.StrengthenLevel == 12)
					{
						if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
						{
							num = ((num > 1) ? num : 3);
						}
						if (itemAt.Template.CategoryID == 7)
						{
							num2 = ((num2 > 1) ? num2 : 3);
						}
					}
					if (itemAt.IsGold || itemAt.StrengthenLevel == 15)
					{
						if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
						{
							num = ((num > 1) ? num : 5);
						}
						if (itemAt.Template.CategoryID == 7)
						{
							num2 = ((num2 > 1) ? num2 : 5);
						}
					}
				}
			}
			this.m_player.PlayerCharacter.Nimbus = num * 100 + num2;
			this.m_player.Out.SendUpdatePublicPlayer(this.m_player.PlayerCharacter);
		}
		public void EquipBuffer()
		{
			this.m_player.EquipEffect.Clear();
			for (int i = 0; i < 31; i++)
			{
				ItemInfo itemAt = this.m_player.BeadBag.GetItemAt(i);
				if (itemAt != null)
				{
					RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneTemplateID(itemAt.TemplateID);
					if (runeTemplateInfo != null && (runeTemplateInfo.Type1 == 37 || runeTemplateInfo.Type1 == 39 || runeTemplateInfo.Type1 < 31))
					{
						this.m_player.AddBeadEffect(itemAt);
					}
				}
			}
		}
		public void AddBeadProperty(int place, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
		{
			ItemInfo itemAt = this.m_player.BeadBag.GetItemAt(place);
			if (itemAt != null)
			{
				this.AddRuneProperty(itemAt, ref attack, ref defence, ref agility, ref lucky, ref hp);
			}
		}
		public void AddRuneProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
		{
			RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneTemplateID(item.TemplateID);
			if (runeTemplateInfo != null)
			{
				string[] array = runeTemplateInfo.Attribute1.Split(new char[]
				{
					'|'
				});
				string[] array2 = runeTemplateInfo.Attribute2.Split(new char[]
				{
					'|'
				});
				int num = 0;
				int num2 = 0;
				if (item.Hole1 > runeTemplateInfo.BaseLevel)
				{
					if (array.Length > 1)
					{
						num = 1;
					}
					if (array2.Length > 1)
					{
						num2 = 1;
					}
				}
				int num3 = Convert.ToInt32(array[num]);
				int num4 = Convert.ToInt32(array2[num2]);
				switch (runeTemplateInfo.Type1)
				{
				case 31:
					attack += num3;
					hp += num4;
					return;

				case 32:
					defence += num3;
					hp += num4;
					return;

				case 33:
					agility += num3;
					hp += num4;
					return;

				case 34:
					lucky += num3;
					hp += num4;
					return;

				case 35:
					hp += num4;
					return;

				case 36:
					hp += num4;
					return;

				case 37:
					hp += num3;
					break;

				default:
					return;
				}
			}
		}
		public void AddBaseTotemProperty(PlayerInfo p, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
		{
			attack += TotemMgr.getProp(p.totemId, "att");
			defence += TotemMgr.getProp(p.totemId, "def");
			agility += TotemMgr.getProp(p.totemId, "agi");
			lucky += TotemMgr.getProp(p.totemId, "luc");
			hp += TotemMgr.getProp(p.totemId, "blo");
		}
		public void AddBaseLatentProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky)
		{
			if (item != null && item.latentEnergyEndTime.Date >= DateTime.Now.Date)
			{
				attack += Convert.ToInt32(item.latentEnergyCurStr.Split(new char[]
				{
					','
				})[0]);
				defence += Convert.ToInt32(item.latentEnergyCurStr.Split(new char[]
				{
					','
				})[1]);
				agility += Convert.ToInt32(item.latentEnergyCurStr.Split(new char[]
				{
					','
				})[2]);
				lucky += Convert.ToInt32(item.latentEnergyCurStr.Split(new char[]
				{
					','
				})[3]);
			}
		}
		public void AddBaseGemstoneProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
		{
			List<UserGemStone> gemStone = this.m_player.GemStone;
			foreach (UserGemStone current in gemStone)
			{
				int figSpiritId = current.FigSpiritId;
				int lv = Convert.ToInt32(current.FigSpiritIdValue.Split(new char[]
				{
					'|'
				})[0].Split(new char[]
				{
					','
				})[0]);
				int num = current.FigSpiritIdValue.Split(new char[]
				{
					'|'
				}).Length;
				int place = item.Place;
				int place2 = item.Place;
				switch (place2)
				{
				case 2:
					attack += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * num;
					break;

				case 3:
					lucky += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * num;
					break;

				case 4:
					break;

				case 5:
					agility += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * num;
					break;

				default:
					switch (place2)
					{
					case 11:
						defence += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * num;
						break;

					case 13:
						hp += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * num;
						break;
					}
					break;
				}
			}
		}
	}
}
