using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Text;
namespace Game.Server.Packets.Client
{
	[PacketHandler(61, "物品转移")]
	public class ItemTransferHandler : IPacketHandler
	{
		public void GetWeaponID(ref int id0, ref int id1)
		{
			string text = id0.ToString().Substring(0, 4);
			string text2 = id1.ToString().Substring(0, 4);
			text += id1.ToString().Substring(4);
			text2 += id0.ToString().Substring(4);
			id0 = int.Parse(text);
			id1 = int.Parse(text2);
		}
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(61, client.Player.PlayerCharacter.ID);
			new StringBuilder();
			int num = 40000;
			bool tranHole = packet.ReadBoolean();
			bool tranHoleFivSix = packet.ReadBoolean();
			ItemInfo itemInfo = client.Player.StoreBag.GetItemAt(0);
			ItemInfo itemInfo2 = client.Player.StoreBag.GetItemAt(1);
			if (itemInfo != null && itemInfo2 != null && itemInfo.Template.CategoryID == itemInfo2.Template.CategoryID && itemInfo2.Count == 1 && itemInfo.Count == 1 && itemInfo.IsValidItem() && itemInfo2.IsValidItem())
			{
				if (client.Player.PlayerCharacter.Gold < num)
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nogold", new object[0]));
					return 1;
				}
				client.Player.RemoveGold(num);
				if (itemInfo.Template.CategoryID == 7 || itemInfo2.Template.CategoryID == 7)
				{
					ItemInfo itemInfo3 = null;
					ItemInfo itemInfo4 = null;
					int templateID = itemInfo.TemplateID;
					int templateID2 = itemInfo2.TemplateID;
					this.GetWeaponID(ref templateID, ref templateID2);
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateID);
					ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(templateID2);
					if (itemTemplateInfo != null)
					{
						itemInfo3 = ItemInfo.CreateWeapon(itemTemplateInfo, itemInfo, 116);
					}
					itemInfo = itemInfo3;
					if (itemTemplateInfo2 != null)
					{
						itemInfo4 = ItemInfo.CreateWeapon(itemTemplateInfo2, itemInfo2, 116);
					}
					itemInfo2 = itemInfo4;
				}
				StrengthenMgr.InheritTransferProperty(ref itemInfo, ref itemInfo2, tranHole, tranHoleFivSix);
				client.Player.StoreBag.ClearBag();
				client.Player.StoreBag.AddItemTo(itemInfo, 0);
				client.Player.StoreBag.AddItemTo(itemInfo2, 1);
				gSPacketIn.WriteByte(0);
				client.Out.SendTCP(gSPacketIn);
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nocondition", new object[0]));
			}
			return 0;
		}
	}
}
