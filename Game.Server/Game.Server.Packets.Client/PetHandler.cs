using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Game.Server.Packets.Client
{
	[PacketHandler(68, "添加好友")]
	public class PetHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte b = packet.ReadByte();
			string text = "Xu không đủ!";
			int num = 1;
			switch (b)
			{
			case 1:
				this.UpdatePetHandle(client, packet.ReadInt());
				break;

			case 2:
				{
                    Console.WriteLine("//ADD_PET ");
                    int place = packet.ReadInt();
					num = packet.ReadInt();
					int iD = client.Player.PlayerCharacter.ID;
					int num2 = client.Player.PetBag.FindFirstEmptySlot();
					if (num2 == -1)
					{
						client.Player.SendMessage("Số lượng pet đã đạt giới hạn!");
					}
					else
					{
						ItemInfo itemAt = client.Player.GetItemAt((eBageType)num, place);
						PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(itemAt.Template.Property5);
						UsersPetinfo usersPetinfo = PetMgr.CreatePet(petTemplateInfo, iD, num2);
						using (PlayerBussiness playerBussiness = new PlayerBussiness())
						{
							usersPetinfo.IsExit = false;
							playerBussiness.AddUserAdoptPet(usersPetinfo, true);
						}
						usersPetinfo.IsExit = true;
						PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
						if (inventory.RemoveCountFromStack(itemAt, 1))
						{
							client.Player.PetBag.AddPetTo(usersPetinfo, num2);
							client.Player.SendMessage("Bạn nhận được 1 " + petTemplateInfo.Name);
						}
						client.Player.SaveIntoDatabase();

					}
					break;
				}

			case 3:
				{
					int num3 = packet.ReadInt();
					break;
				}

			case 4:
				{
					int place = packet.ReadInt();
					num = packet.ReadInt();
					int num3 = packet.ReadInt();
					bool flag = false;
					ItemInfo itemAt2 = client.Player.GetItemAt((eBageType)num, place);
					if (itemAt2 == null)
					{
						client.Player.SendMessage("Vật phẩm lạ không tìm thấy!!! Hack chăng???!!!");
						return 0;
					}
					int num4 = Convert.ToInt32(PetMgr.FindConfig("MaxHunger").Value);
					int num5 = Convert.ToInt32(PetMgr.FindConfig("MaxLevel").Value);
					UsersPetinfo petAt = client.Player.PetBag.GetPetAt(num3);
					int num6 = itemAt2.Count;
					int property = itemAt2.Template.Property2;
					int property2 = itemAt2.Template.Property3;
					int num7 = num6 * property2;
					int num8 = num7 + petAt.Hunger;
					int num9 = num6 * property;
					text = "";
					if (itemAt2.TemplateID == 334100)
					{
						num9 = itemAt2.DefendCompose;
					}
					if (petAt.Level > 11 && itemAt2.TemplateID == 334100)
					{
						text = "Pet level 10 trở xuống mới dùng đuợc " + itemAt2.Template.Name;
					}
					else
					{
						if (petAt.Level < num5)
						{
							num9 += petAt.GP;
							int level = petAt.Level;
							int level2 = PetMgr.GetLevel(num9);
							int gP = PetMgr.GetGP(level2 + 1);
							int gP2 = PetMgr.GetGP(num5);
							int num10 = num9;
							if (num9 > gP2)
							{
								num9 -= gP2;
								if (num9 >= property && property != 0)
								{
									num6 = num9 / property;
								}
							}
							petAt.GP = ((num10 >= gP2) ? gP2 : num10);
							petAt.Level = level2;
							petAt.MaxGP = ((gP == 0) ? gP2 : gP);
							petAt.Hunger = ((num8 > num4) ? num4 : num8);
							flag = client.Player.PetBag.UpGracePet(petAt, num3, true, level, level2, ref text);
							if (itemAt2.TemplateID == 334100)
							{
								client.Player.StoreBag.RemoveItem(itemAt2);
							}
							else
							{
								client.Player.StoreBag.RemoveCountFromStack(itemAt2, num6);
								client.Player.OnUsingItem(itemAt2.TemplateID);
							}
						}
						else
						{
							int hunger = petAt.Hunger;
							int num11 = num4 - hunger;
							if (num8 >= num4 && num8 >= property2)
							{
								num6 = num8 / property2;
							}
							num8 = hunger + num11;
							petAt.Hunger = num8;
							if (hunger < num4)
							{
								client.Player.StoreBag.RemoveCountFromStack(itemAt2, num6);
								flag = client.Player.PetBag.UpGracePet(petAt, num3, false, 0, 0, ref text);
								text = "Ðộ vui vẻ tang thêm " + num11;
							}
							else
							{
								text = "Ðộ vui vui đã đạt mức tối da";
							}
						}
					}
					if (flag)
					{
						UsersPetinfo[] pets = client.Player.PetBag.GetPets();
						client.Player.Out.SendUpdatePetInfo(client.Player.PlayerCharacter, pets);
					}
					if (!string.IsNullOrEmpty(text))
					{
						client.Player.SendMessage(text);
					}
					break;
				}

			case 5:
				{
					bool refreshBtn = packet.ReadBoolean();
					this.RefreshPetHandle(client, refreshBtn, text);
					break;
				}

			case 6:
				{
					int num3 = packet.ReadInt();
					int num12 = client.Player.PetBag.FindFirstEmptySlot();
					if (num12 == -1)
					{
						client.Player.Out.SendRefreshPet(client.Player, client.Player.PetBag.GetAdoptPet(), null, false);
						client.Player.SendMessage("Số lượng pet đã đạt giới hạn!");
					}
					else
					{
						UsersPetinfo adoptPetAt = client.Player.PetBag.GetAdoptPetAt(num3);
						if (client.Player.PetBag.AddPetTo(adoptPetAt, num12))
						{
							client.Player.PetBag.RemoveAdoptPet(adoptPetAt);
							client.Player.OnAdoptPetEvent();
						}
					}
					break;
				}

			case 7:
				{
					int num3 = packet.ReadInt();
					int killId = packet.ReadInt();
					int killindex = packet.ReadInt();
					if (client.Player.PetBag.EquipSkillPet(num3, killId, killindex))
					{
						UsersPetinfo[] pets = client.Player.PetBag.GetPets();
						client.Player.Out.SendUpdatePetInfo(client.Player.PlayerCharacter, pets);
					}
					else
					{
						client.Player.SendMessage("Skill này đã trang bị!");
					}
					break;
				}

			case 8:
				{
					int num3 = packet.ReadInt();
					UsersPetinfo petAt2 = client.Player.PetBag.GetPetAt(num3);
					if (client.Player.PetBag.RemovePet(petAt2))
					{
						using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
						{
							playerBussiness2.UpdateUserAdoptPet(petAt2.ID);
						}
					}
					client.Player.SendMessage("Thả pet thành công!");
					break;
				}

			case 9:
				{
					int num3 = packet.ReadInt();
					string name = packet.ReadString();
					int num13 = Convert.ToInt32(PetMgr.FindConfig("ChangeNameCost").Value);
					if (client.Player.PlayerCharacter.Money >= num13)
					{
						if (client.Player.PetBag.RenamePet(num3, name))
						{
							UsersPetinfo[] pets = client.Player.PetBag.GetPets();
							client.Out.SendUpdatePetInfo(client.Player.PlayerCharacter, pets);
							text = "Đổi tên thành công!";
						}
						client.Player.RemoveMoney(num13);
					}
					client.Player.SendMessage(text);
					break;
				}

			case 16:
				{
					Console.WriteLine("//PAY_SKILL ");
					int num3 = packet.ReadInt();
					break;
				}

			case 17:
				{
					int num3 = packet.ReadInt();
					bool isEquip = packet.ReadBoolean();
					if (client.Player.PetBag.SetIsEquip(num3, isEquip))
					{
						UsersPetinfo[] pets = client.Player.PetBag.GetPets();
						client.Player.MainBag.UpdatePlayerProperties();
						client.Player.Out.SendUpdatePetInfo(client.Player.PlayerCharacter, pets);
					}
					break;
				}

			case 18:
				{
					int num3 = packet.ReadInt();
					this.RevertPetHandle(client, num3, text);
					break;
				}

            case 19:
                {
                    Console.WriteLine("//BUY_PET_EXP_ITEM ");
                    bool buyPetExpItem = packet.ReadBoolean();
                    int GoodsID = packet.ReadInt();
                    int count = 20 * 80;
                    ShopItemInfo shopItem = ShopMgr.GetShopItemInfoById(GoodsID);
                    SqlDataProvider.Data.ItemInfo item = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(334103), 20, 0x66);
                    client.Player.RemoveMoney(count);
                    client.Player.AddItem(item);
                    text = "Mua thức ăn thành công.";
                    client.Player.SendMessage(text);
                    break;
                }
            }
			client.Player.PetBag.SaveToDatabase();
			return 0;
		}
		public void RevertPetHandle(GameClient client, int place, string msg)
		{
			int num = Convert.ToInt32(PetMgr.FindConfig("RecycleCost").Value);
			if (client.Player.PlayerCharacter.Money >= num)
			{
				UsersPetinfo petAt = client.Player.PetBag.GetPetAt(place);
				UsersPetinfo usersPetinfo = new UsersPetinfo();
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(334100);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 0);
				itemInfo.IsBinds = true;
				itemInfo.DefendCompose = petAt.GP;
				itemInfo.AgilityCompose = petAt.MaxGP;
				if (!client.Player.PropBag.AddTemplate(itemInfo, 1))
				{
					client.Player.SendItemToMail(itemInfo, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), eMailType.ItemOverdue);
					client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				int iD = petAt.ID;
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					usersPetinfo = playerBussiness.GetAdoptPetSingle(iD);
				}
				petAt.Blood = usersPetinfo.Blood;
				petAt.Attack = usersPetinfo.Attack;
				petAt.Defence = usersPetinfo.Defence;
				petAt.Agility = usersPetinfo.Agility;
				petAt.Luck = usersPetinfo.Luck;
				int arg_158_0 = client.Player.PlayerCharacter.ID;
				int templateID = usersPetinfo.TemplateID;
				petAt.TemplateID = templateID;
				petAt.Skill = usersPetinfo.Skill;
				petAt.SkillEquip = usersPetinfo.SkillEquip;
				petAt.GP = 0;
				petAt.Level = 1;
				petAt.MaxGP = 55;
				bool flag = client.Player.PetBag.UpGracePet(petAt, place, false, 0, 0, ref msg);
				client.Player.SendMessage("Phục hồi thành công!");
				client.Player.RemoveMoney(num);
				if (flag)
				{
					client.Player.Out.SendUpdatePetInfo(client.Player.PlayerCharacter, client.Player.PetBag.GetPets());
					return;
				}
			}
			else
			{
				client.Player.SendMessage(msg);
			}
		}
		public void RefreshPetHandle(GameClient client, bool refreshBtn, string msg)
		{
			int num = Convert.ToInt32(PetMgr.FindConfig("AdoptRefereshCost").Value);
			int templateId = Convert.ToInt32(PetMgr.FindConfig("FreeRefereshID").Value);
			ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
			if (refreshBtn)
			{
				if (itemByTemplateID != null)
				{
					client.Player.PropBag.RemoveTemplate(templateId, 1);
				}
				else
				{
					if (client.Player.PlayerCharacter.Money >= num)
					{
						client.Player.RemoveMoney(num);
						client.Player.AddPetScore(num / 10);
					}
					else
					{
						client.Player.SendMessage(msg);
					}
				}
				List<UsersPetinfo> list = PetMgr.CreateAdoptList(client.Player.PlayerCharacter.ID);
				client.Player.PetBag.ClearAdoptPets();
				foreach (UsersPetinfo current in list)
				{
					client.Player.PetBag.AddAdoptPetTo(current, current.Place);
				}
			}
			client.Player.Out.SendRefreshPet(client.Player, client.Player.PetBag.GetAdoptPet(), null, refreshBtn);
		}
		public void UpdatePetHandle(GameClient client, int ID)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(ID);
			PlayerInfo playerInfo;
			UsersPetinfo[] array;
			if (playerById != null)
			{
				playerInfo = playerById.PlayerCharacter;
				array = playerById.PetBag.GetPets().ToArray<UsersPetinfo>();
			}
			else
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					playerInfo = playerBussiness.GetUserSingleByUserID(ID);
					array = playerBussiness.GetUserPetSingles(ID);
				}
			}
			if (array != null && playerInfo != null)
			{
				client.Out.SendUpdatePetInfo(playerInfo, array);
			}
		}
	}
}
