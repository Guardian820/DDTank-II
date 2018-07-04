using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(58, "物品合成")]
	public class ItemComposeHandler : IPacketHandler
	{
		public static ThreadSafeRandom random = new ThreadSafeRandom();
		private static readonly double[] composeRate = new double[]
		{
			0.8,
			0.5,
			0.3,
			0.1,
			0.05
		};
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(58, client.Player.PlayerCharacter.ID);
			StringBuilder stringBuilder = new StringBuilder();
			int pRICE_COMPOSE_GOLD = GameProperties.PRICE_COMPOSE_GOLD;
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			if (client.Player.PlayerCharacter.Gold < pRICE_COMPOSE_GOLD)
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemComposeHandler.NoMoney", new object[0]));
				return 0;
			}
			int num = -1;
			int num2 = -1;
			bool flag = false;
			bool flag2 = packet.ReadBoolean();
			ItemInfo itemAt = client.Player.StoreBag.GetItemAt(1);
			ItemInfo itemAt2 = client.Player.StoreBag.GetItemAt(2);
			ItemInfo itemInfo = null;
			ItemInfo itemInfo2 = null;
			string beginProperty = null;
			string text = null;
			using (ItemRecordBussiness itemRecordBussiness = new ItemRecordBussiness())
			{
				itemRecordBussiness.PropertyString(itemAt, ref beginProperty);
			}
			if (itemAt != null && itemAt2 != null && itemAt.Template.CanCompose && (itemAt.Template.CategoryID < 10 || (itemAt2.Template.CategoryID == 11 && itemAt2.Template.Property1 == 1)))
			{
				flag = (flag || itemAt.IsBinds);
				flag = (flag || itemAt2.IsBinds);
				stringBuilder.Append(string.Concat(new object[]
				{
					itemAt.ItemID,
					":",
					itemAt.TemplateID,
					",",
					itemAt2.ItemID,
					":",
					itemAt2.TemplateID,
					","
				}));
				bool flag3 = false;
				byte b = 1;
				double num3 = ItemComposeHandler.composeRate[itemAt2.Template.Quality - 1] * 100.0;
				if (client.Player.StoreBag.GetItemAt(0) != null)
				{
					itemInfo = client.Player.StoreBag.GetItemAt(0);
					if (itemInfo != null && itemInfo.Template.CategoryID == 11 && itemInfo.Template.Property1 == 3)
					{
						flag = (flag || itemInfo.IsBinds);
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"|",
							itemInfo.ItemID,
							":",
							itemInfo.Template.Name,
							"|",
							itemAt2.ItemID,
							":",
							itemAt2.Template.Name
						});
						stringBuilder.Append(string.Concat(new object[]
						{
							itemInfo.ItemID,
							":",
							itemInfo.TemplateID,
							","
						}));
						num3 += num3 * (double)itemInfo.Template.Property2 / 100.0;
					}
				}
				else
				{
					num3 += num3 * 1.0 / 100.0;
				}
				if (num2 != -1)
				{
					itemInfo2 = client.Player.PropBag.GetItemAt(num2);
					if (itemInfo2 != null && itemInfo2.Template.CategoryID == 11 && itemInfo2.Template.Property1 == 7)
					{
						flag = (flag || itemInfo2.IsBinds);
						stringBuilder.Append(string.Concat(new object[]
						{
							itemInfo2.ItemID,
							":",
							itemInfo2.TemplateID,
							","
						}));
						object obj2 = text;
						text = string.Concat(new object[]
						{
							obj2,
							",",
							itemInfo2.ItemID,
							":",
							itemInfo2.Template.Name
						});
					}
					else
					{
						itemInfo2 = null;
					}
				}
				if (flag2)
				{
					ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
					ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
					ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(client.Player.PlayerCharacter.ConsortiaID, 0, 2);
					if (consortiaInfo == null)
					{
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Fail", new object[0]));
					}
					else
					{
						if (client.Player.PlayerCharacter.Riches < consortiaEuqipRiches.Riches)
						{
							client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemStrengthenHandler.FailbyPermission", new object[0]));
							return 1;
						}
						num3 *= 1.0 + 0.1 * (double)consortiaInfo.SmithLevel;
					}
				}
				num3 = Math.Floor(num3 * 10.0) / 10.0;
				int num4 = ItemComposeHandler.random.Next(100);
				switch (itemAt2.Template.Property3)
				{
				case 1:
					if (itemAt2.Template.Property4 > itemAt.AttackCompose)
					{
						flag3 = true;
						if (num3 > (double)num4)
						{
							b = 0;
							itemAt.AttackCompose = itemAt2.Template.Property4;
						}
					}
					break;

				case 2:
					if (itemAt2.Template.Property4 > itemAt.DefendCompose)
					{
						flag3 = true;
						if (num3 > (double)num4)
						{
							b = 0;
							itemAt.DefendCompose = itemAt2.Template.Property4;
						}
					}
					break;

				case 3:
					if (itemAt2.Template.Property4 > itemAt.AgilityCompose)
					{
						flag3 = true;
						if (num3 > (double)num4)
						{
							b = 0;
							itemAt.AgilityCompose = itemAt2.Template.Property4;
						}
					}
					break;

				case 4:
					if (itemAt2.Template.Property4 > itemAt.LuckCompose)
					{
						flag3 = true;
						if (num3 > (double)num4)
						{
							b = 0;
							itemAt.LuckCompose = itemAt2.Template.Property4;
						}
					}
					break;
				}
				if (flag3)
				{
					itemAt.IsBinds = flag;
					if (b != 0)
					{
						stringBuilder.Append("false!");
						flag3 = false;
					}
					else
					{
						stringBuilder.Append("true!");
						flag3 = true;
						client.Player.OnItemCompose(itemAt2.TemplateID);
					}
					LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Compose, beginProperty, itemAt, text, Convert.ToInt32(flag3));
					client.Player.StoreBag.RemoveTemplate(itemAt2.TemplateID, 1);
					if (itemInfo != null)
					{
						client.Player.StoreBag.RemoveTemplate(itemInfo.TemplateID, 1);
					}
					if (itemInfo2 != null)
					{
						client.Player.RemoveItem(itemInfo2);
					}
					client.Player.RemoveGold(pRICE_COMPOSE_GOLD);
					client.Player.StoreBag.UpdateItem(itemAt);
					gSPacketIn.WriteByte(b);
					client.Out.SendTCP(gSPacketIn);
					if (num < 31)
					{
						client.Player.MainBag.UpdatePlayerProperties();
					}
				}
				else
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemComposeHandler.NoLevel", new object[0]));
				}
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemComposeHandler.Fail", new object[0]));
			}
			return 0;
		}
	}
}
