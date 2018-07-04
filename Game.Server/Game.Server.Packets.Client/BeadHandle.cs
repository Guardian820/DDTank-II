using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(121, "物品镶嵌")]
	public class BeadHandle : IPacketHandler
	{
		public static ThreadSafeRandom randomExp = new ThreadSafeRandom();
		private static bool canEquip(int place, int grade, ref int needLv)
		{
			bool result = false;
			switch (place)
			{
			case 6:
				needLv = 15;
				if (grade >= needLv)
				{
					result = true;
				}
				break;

			case 7:
				needLv = 18;
				if (grade >= needLv)
				{
					result = true;
				}
				break;

			case 8:
				needLv = 21;
				if (grade >= needLv)
				{
					result = true;
				}
				break;

			case 9:
				needLv = 24;
				if (grade >= needLv)
				{
					result = true;
				}
				break;

			case 10:
				needLv = 27;
				if (grade >= needLv)
				{
					result = true;
				}
				break;

			case 11:
				needLv = 30;
				if (grade >= needLv)
				{
					result = true;
				}
				break;

			case 12:
				needLv = 33;
				if (grade >= needLv)
				{
					result = true;
				}
				break;
			}
			return result;
		}
		private static bool CanUpLv(int exp, int lv)
		{
			return exp >= GameProperties.RuneExp()[lv];
		}
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte b = packet.ReadByte();
			PlayerInventory inventory = client.Player.GetInventory(eBageType.BeadBag);
			string text = "";
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			if (DateTime.Compare(client.Player.LastDrillUpTime.AddSeconds(2.0), DateTime.Now) > 0)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Quá nhiều thao tác!", new object[0]));
				return 1;
			}
			switch (b)
			{
			case 1:
				{
					int num = packet.ReadInt();
					int num2 = packet.ReadInt();
					int num3 = 10;
					if (num2 == -1)
					{
						num2 = inventory.FindFirstEmptySlot();
					}
					if (num <= 12 && num >= 4 && BeadHandle.canEquip(num2, client.Player.PlayerCharacter.Grade, ref num3))
					{
						client.Out.SendMessage(eMessageType.Normal, string.Format("Cấp {0} mở", num3));
						return 0;
					}
					if (!inventory.MoveItem(num, num2, 1))
					{
						Console.WriteLine("????User move bead error");
					}
					client.Player.MainBag.UpdatePlayerProperties();
					break;
				}

			case 2:
				{
					new List<int>();
					ItemInfo itemAt = inventory.GetItemAt(31);
					if (itemAt == null)
					{
						client.Player.SendMessage("Vật phẩm lạ không tìm thấy!!! Hack chăng???!!!");
						return 0;
					}
					int arg_504_0 = itemAt.Hole1;
					int num4 = packet.ReadInt();
					int num5 = RuneMgr.MaxLv();
					for (int i = 0; i < num4; i++)
					{
						int num6 = packet.ReadInt();
						ItemInfo itemAt2 = inventory.GetItemAt(num6);
						RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneTemplateID(itemAt.TemplateID);
						if (runeTemplateInfo == null)
						{
							inventory.RemoveItem(itemAt2);
						}
						else
						{
							if (itemAt2.Hole1 < itemAt.Hole1 && !itemAt2.IsUsed)
							{
								int hole = itemAt2.Hole2;
								int hole2 = itemAt.Hole2;
								int hole3 = itemAt.Hole1;
								int exp = hole + hole2;
								inventory.RemoveItemAt(num6);
								if (BeadHandle.CanUpLv(exp, hole3))
								{
									itemAt.Hole2 += hole;
									itemAt.Hole1++;
								}
								else
								{
									itemAt.Hole2 += hole;
								}
								int nextTemplateID = runeTemplateInfo.NextTemplateID;
								RuneTemplateInfo runeTemplateInfo2 = RuneMgr.FindRuneTemplateID(nextTemplateID);
								if (itemAt.Hole1 == runeTemplateInfo2.BaseLevel)
								{
									ItemInfo itemInfo = new ItemInfo(ItemMgr.FindItemTemplate(nextTemplateID));
									itemAt.TemplateID = nextTemplateID;
									itemInfo.Copy(itemAt);
									inventory.RemoveItemAt(31);
									inventory.AddItemTo(itemInfo, 31);
								}
								else
								{
									inventory.UpdateItem(itemAt);
								}
								if (itemAt.Hole1 == num5 + 1)
								{
									break;
								}
							}
						}
					}
					break;
				}

			case 3:
				{
					string[] array = GameProperties.OpenRunePackageMoney.Split(new char[]
					{
						'|'
					});
					int num7 = packet.ReadInt();
					packet.ReadBoolean();
					int num8 = Convert.ToInt32(array[num7]);
					if (client.Player.PlayerCharacter.Money < num8)
					{
						client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DivorceApplyHandler.Msg1", new object[0]));
						return 1;
					}
					if (inventory.FindFirstEmptySlot() == -1)
					{
						client.Out.SendMessage(eMessageType.Normal, "Rương đã đầy không thể mở thêm!");
						return 1;
					}
					List<RuneTemplateInfo> list = new List<RuneTemplateInfo>();
					switch (num7)
					{
					case 1:
						list = RuneMgr.OpenPackageLv2();
						break;

					case 2:
						list = RuneMgr.OpenPackageLv3();
						break;

					case 3:
						list = RuneMgr.OpenPackageLv4();
						break;

					default:
						list = RuneMgr.OpenPackageLv1();
						break;
					}
					int index = ThreadSafeRandom.NextStatic(list.Count);
					ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(list[index].TemplateID), 1, 105);
					inventory.AddItem(itemInfo2);
					client.Out.SendMessage(eMessageType.Normal, "Bạn nhận được " + RuneMgr.FindRuneTemplateID(itemInfo2.TemplateID).Name);
					client.Player.RemoveMoney(num8);
					int rand = BeadHandle.NextBeadIndex(client, num7);
					this.BeadIndexUpdate(client, num7);
					client.Out.SendRuneOpenPackage(client.Player, rand);
					break;
				}

			case 4:
				{
					int num = packet.ReadInt();
					ItemInfo itemAt2 = inventory.GetItemAt(num);
					if (itemAt2.IsUsed)
					{
						itemAt2.IsUsed = false;
					}
					else
					{
						itemAt2.IsUsed = true;
					}
					inventory.UpdateItem(itemAt2);
					break;
				}

			case 5:
				{
					int num9 = packet.ReadInt();
					int templateId = packet.ReadInt();
					PlayerInventory inventory2 = client.Player.GetInventory(eBageType.PropBag);
					inventory2.GetItemByTemplateID(0, templateId);
					int itemCount = inventory2.GetItemCount(templateId);
					if (itemCount <= 0)
					{
						client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Mủi khoan không đủ!", new object[0]));
					}
					else
					{
						int num10 = BeadHandle.randomExp.Next(2, 6);
						text = LanguageMgr.GetTranslation("OpenHoleHandler.GetExp", new object[]
						{
							num10
						});
						UserDrillInfo userDrillInfo = client.Player.UserDrills[num9];
						userDrillInfo.HoleExp += num10;
						if ((userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(0) && userDrillInfo.HoleLv == 0) || (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(1) && userDrillInfo.HoleLv == 1) || (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(2) && userDrillInfo.HoleLv == 2) || (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(3) && userDrillInfo.HoleLv == 3) || (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(4) && userDrillInfo.HoleLv == 4))
						{
							userDrillInfo.HoleLv++;
							userDrillInfo.HoleExp = 0;
						}
						client.Player.UpdateDrill(num9, userDrillInfo);
						using (PlayerBussiness playerBussiness = new PlayerBussiness())
						{
							playerBussiness.UpdateUserDrillInfo(userDrillInfo);
						}
					}
					if (text != "")
					{
						client.Out.SendMessage(eMessageType.Normal, text);
					}
					client.Player.Out.SendPlayerDrill(client.Player.PlayerCharacter.ID, client.Player.UserDrills);
					inventory2.RemoveTemplate(templateId, 1);
					client.Player.LastDrillUpTime = DateTime.Now;
					break;
				}
			}
			inventory.SaveToDatabase();
			return 0;
		}
		public static int NextBeadIndex(GameClient client, int index)
		{
			if (client.beadRequestBtn1 == 4 && index == 0)
			{
				return 1;
			}
			if (client.beadRequestBtn2 == 3 && index == 1)
			{
				return 2;
			}
			if (client.beadRequestBtn3 == 30 && index == 2)
			{
				return 4;
			}
			return 0;
		}
		public void BeadIndexUpdate(GameClient client, int index)
		{
			if (index == 0)
			{
				if (client.beadRequestBtn1 > 4)
				{
					client.beadRequestBtn1 = 0;
				}
				client.beadRequestBtn1++;
			}
			if (index == 1)
			{
				client.beadRequestBtn2++;
				client.beadRequestBtn1 = 0;
			}
			if (index == 2)
			{
				client.beadRequestBtn3++;
				client.beadRequestBtn2 = 0;
			}
			if (index == 3)
			{
				client.beadRequestBtn3 = 0;
			}
		}
	}
}
