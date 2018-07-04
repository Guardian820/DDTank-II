using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(78, "熔化")]
	public class ItemFusionHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				return 0;
			}
			int num = (int)packet.ReadByte();
			bool flag = false;
			int num2 = 0;
			bool flag2 = false;
			ItemTemplateInfo itemTemplateInfo = null;
			StringBuilder stringBuilder = new StringBuilder();
			int num3 = 2147483647;
			List<ItemInfo> list = new List<ItemInfo>();
			List<ItemInfo> appendItems = new List<ItemInfo>();
			PlayerInventory storeBag = client.Player.StoreBag;
			ItemInfo item = null;
			string beginProperty = null;
			string addItem = "";
			int num4 = 0;
			for (int i = 1; i < 5; i++)
			{
				ItemInfo itemAt = storeBag.GetItemAt(i);
				if (itemAt != null)
				{
					if (list.Contains(itemAt))
					{
						client.Out.SendMessage(eMessageType.Normal, "Bad Input 1");
						return 1;
					}
					stringBuilder.Append(string.Concat(new object[]
					{
						itemAt.ItemID,
						":",
						itemAt.TemplateID,
						","
					}));
					list.Add(itemAt);
					if (itemAt.ValidDate != 0 && itemAt.ValidDate >= num4)
					{
						num3 = itemAt.ValidDate;
					}
					num4 = itemAt.ValidDate;
				}
			}
			switch (num)
			{
			case 0:
				{
					flag = false;
					Dictionary<int, double> dictionary = FusionMgr.FusionPreview(list, appendItems, ref flag);
					if (dictionary != null && dictionary.Count > 0)
					{
						if (dictionary.Count != 0)
						{
							client.Out.SendFusionPreview(client.Player, dictionary, flag, num3);
						}
					}
					else
					{
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.ItemNotEnough", new object[0]));
					}
					return 0;
				}

			case 1:
				num2 = 1600;
				if (client.Player.PlayerCharacter.Gold < num2)
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.NoMoney", new object[0]));
					return 0;
				}
				flag = false;
				flag2 = false;
				itemTemplateInfo = FusionMgr.Fusion(list, appendItems, ref flag, ref flag2);
				break;
			}
			int num5 = client.Player.MainBag.FindFirstEmptySlot();
			int num6 = client.Player.PropBag.FindFirstEmptySlot();
			if ((num5 == -1 && itemTemplateInfo.BagType == eBageType.MainBag) || (num6 == -1 && itemTemplateInfo.BagType == eBageType.PropBag))
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]));
				return 0;
			}
			if (itemTemplateInfo != null)
			{
				client.Player.RemoveGold(num2);
				for (int j = 1; j < 5; j++)
				{
					ItemInfo itemAt2 = storeBag.GetItemAt(j);
					if (itemAt2.Count > 1)
					{
						itemAt2.Count--;
						storeBag.UpdateItem(itemAt2);
					}
					else
					{
						storeBag.RemoveItem(itemAt2);
					}
				}
				if (flag2)
				{
					stringBuilder.Append(itemTemplateInfo.TemplateID + ",");
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
					if (itemInfo == null)
					{
						return 0;
					}
					item = itemInfo;
					itemInfo.IsBinds = flag;
					itemInfo.ValidDate = num3;
					client.Player.OnItemFusion(itemInfo.Template.FusionType);
					client.Out.SendFusionResult(client.Player, flag2);
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Succeed1", new object[0]) + " " + itemInfo.Template.Name);
					if ((itemInfo.TemplateID >= 8300 && itemInfo.TemplateID <= 8999) || (itemInfo.TemplateID >= 9300 && itemInfo.TemplateID <= 9999) || (itemInfo.TemplateID >= 14300 && itemInfo.TemplateID <= 14999) || (itemInfo.TemplateID >= 7024 && itemInfo.TemplateID <= 7028) || (itemInfo.TemplateID >= 14006 && itemInfo.TemplateID <= 14010) || (itemInfo.TemplateID >= 17000 && itemInfo.TemplateID <= 17010))
					{
						string translation = LanguageMgr.GetTranslation("ItemFusionHandler.Notice", new object[]
						{
							client.Player.PlayerCharacter.NickName,
							itemInfo.Template.Name
						});
						client.Player.SendAllMessage(translation);
					}
					if (!storeBag.AddItemTo(itemInfo, 0))
					{
						stringBuilder.Append("NoPlace");
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(itemInfo.GetBagName(), new object[0]) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace", new object[0]));
					}
				}
				else
				{
					stringBuilder.Append("false");
					client.Out.SendFusionResult(client.Player, flag2);
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Failed", new object[0]));
				}
				LogMgr.LogItemAdd(client.Player.PlayerCharacter.ID, LogItemType.Fusion, beginProperty, item, addItem, Convert.ToInt32(flag2));
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition", new object[0]));
			}
			return 0;
		}
	}
}
