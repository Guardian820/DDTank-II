using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(81, "游戏创建")]
	public class EnterFarmHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte b = packet.ReadByte();
            if (client.Player.PlayerCharacter.GP < 1437053)
            {
                client.Out.SendMessage(eMessageType.Normal, "Hack level sao bạn!");
                return 0;
            }
			if (client.Player.PlayerCharacter.Grade < 25)
			{
                client.Out.SendMessage(eMessageType.Normal, "Hack level sao bạn!");
                return 0;
			}
			switch (b)
			{
			case 0:
				Console.WriteLine("//FRUSH_FIELD ");
				break;

			case 1:
				{
					int num = packet.ReadInt();
					if (num == client.Player.PlayerCharacter.ID)
					{
						client.Player.Farm.EnterFarm();
						if (client.Player.PlayerCharacter.IsFistGetPet)
						{
							List<UsersPetinfo> list = PetMgr.CreateFirstAdoptList(num);
							client.Player.PetBag.ClearAdoptPets();
							foreach (UsersPetinfo current in list)
							{
								client.Player.PetBag.AddAdoptPetTo(current, current.Place);
							}
							client.Player.RemoveFistGetPet();
						}
						else
						{
							if (client.Player.PlayerCharacter.LastRefreshPet.Date < DateTime.Now.Date)
							{
								List<UsersPetinfo> list2 = PetMgr.CreateAdoptList(num);
								client.Player.PetBag.ClearAdoptPets();
								foreach (UsersPetinfo current2 in list2)
								{
									client.Player.PetBag.AddAdoptPetTo(current2, current2.Place);
								}
								client.Player.RemoveLastRefreshPet();
							}
						}
						client.Player.Farm.CropHelperSwitchField(false);
					}
					else
					{
						client.Player.Farm.EnterFriendFarm(num);
					}
					break;
				}

			case 2:
				{
					packet.ReadByte();
					int num2 = packet.ReadInt();
					int fieldId = packet.ReadInt();
					if (client.Player.Farm.GrowField(fieldId, num2))
					{
						client.Player.FarmBag.RemoveTemplate(num2, 1);
						client.Player.OnSeedFoodPetEvent();
					}
					break;
				}

			case 3:
				packet.ReadByte();
				packet.ReadInt();
				packet.ReadInt();
				packet.ReadInt();
				Console.WriteLine("//ACCELERATE_FIELD ");
				break;

			case 4:
				{
					int num3 = packet.ReadInt();
					int fieldId2 = packet.ReadInt();
					string message = "Giờ bạn khác giờ hệ thống. Vui lòng chờ một lúc sau rồi vào thu hoạch lại nhé!";
					if (num3 == client.Player.PlayerCharacter.ID)
					{
						if (client.Player.Farm.GainField(fieldId2))
						{
							message = "Thu hoạch thành công!";
						}
					}
					else
					{
						if (client.Player.Farm.GainFriendFields(num3, fieldId2))
						{
							message = "Thao tác thành công.";
						}
						else
						{
							message = "Không thể chộm nửa.";
						}
					}
					client.Player.Out.SendMessage(eMessageType.Normal, message);
					break;
				}

			case 5:
				packet.ReadInt();
				packet.ReadInt();
				Console.WriteLine("//COMPOSE_FOOD ");
				break;

			case 6:
				{
					string message2 = "Mở rộng thành công!";
					List<int> list3 = new List<int>();
					int num4 = packet.ReadInt();
					for (int i = 0; i < num4; i++)
					{
						int item = packet.ReadInt();
						list3.Add(item);
					}
					int num5 = packet.ReadInt();
					PlayerFarm farm = client.Player.Farm;
					int num6;
					if (farm.payFieldTimeToMonth() == num5)
					{
						num6 = num4 * farm.payFieldMoneyToMonth();
					}
					else
					{
						num6 = num4 * farm.payFieldMoneyToWeek();
					}
					if (client.Player.PlayerCharacter.Money >= num6)
					{
						farm.PayField(list3, num5);
						client.Player.RemoveMoney(num6);
					}
					else
					{
						message2 = "Xu không đủ!";
					}
					client.Out.SendMessage(eMessageType.Normal, message2);
					break;
				}

			case 7:
				{
					int fieldId3 = packet.ReadInt();
					client.Player.Farm.killCropField(fieldId3);
					break;
				}

			case 8:
				Console.WriteLine("//HELPER_PAY_FIELD ");
				break;

			case 9:
				{
					string message3 = "Chức năng này hiện đang bảo trì.";
					bool flag = packet.ReadBoolean();
					int seedID = packet.ReadInt();
					int seedTime = packet.ReadInt();
					int haveCount = packet.ReadInt();
					int getCount = packet.ReadInt();
					int num7 = packet.ReadInt();
					int num8 = packet.ReadInt();
					bool flag2 = false;
					if (flag)
					{
						if (client.Player.PlayerCharacter.Money >= num8 && num7 == -1)
						{
							client.Player.RemoveMoney(num8);
							flag2 = true;
						}
						else
						{
							if (client.Player.PlayerCharacter.GiftToken >= num8 && num7 == -2)
							{
								client.Player.RemoveGiftToken(num8);
								flag2 = true;
							}
							else
							{
								if (num7 == -1)
								{
									message3 = "Xu không đủ!";
								}
								else
								{
									message3 = "Xu khóa không đủ!";
								}
							}
						}
					}
					else
					{
						message3 = "Hủy trợ thủ thành công!";
						client.Player.Farm.CropHelperSwitchField(true);
					}
					if (flag2)
					{
						client.Player.Farm.HelperSwitchField(flag, seedID, seedTime, haveCount, getCount);
						message3 = "Kích hoạt trợ thủ thành công!";
					}
					client.Out.SendMessage(eMessageType.Normal, message3);
					break;
				}

			case 16:
				client.Player.Farm.ExitFarm();
				break;
			}
			client.Player.Farm.SaveToDatabase();
			return 0;
		}
	}
}
