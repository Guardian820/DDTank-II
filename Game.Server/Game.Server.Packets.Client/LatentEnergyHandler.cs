using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(133, "场景用户离开")]
	public class LatentEnergyHandler : IPacketHandler
	{
		public static ThreadSafeRandom random = new ThreadSafeRandom();
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = (int)packet.ReadByte();
			int num2 = packet.ReadInt();
			int place = packet.ReadInt();
			ItemInfo itemAt = client.Player.GetItemAt((eBageType)num2, place);
			PlayerInventory inventory = client.Player.GetInventory((eBageType)num2);
			string msg = "Kích hoạt tiềm năng thành công!";
			GSPacketIn gSPacketIn = new GSPacketIn(133, client.Player.PlayerCharacter.ID);
			if (num == 1)
			{
				int num3 = packet.ReadInt();
				int place2 = packet.ReadInt();
				ItemInfo itemAt2 = client.Player.GetItemAt((eBageType)num3, place2);
				PlayerInventory inventory2 = client.Player.GetInventory((eBageType)num3);
				inventory2.RemoveCountFromStack(itemAt2, 1);
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(itemAt2.TemplateID);
				int property = itemTemplateInfo.Property3;
				string text = property.ToString();
				for (int i = 1; i < 4; i++)
				{
					text = text + "," + property.ToString();
				}
				if (itemAt.latentEnergyCurStr.Split(new char[]
				{
					','
				})[0] == "0")
				{
					itemAt.latentEnergyCurStr = text;
				}
				itemAt.latentEnergyNewStr = text;
				itemAt.latentEnergyEndTime = DateTime.Now.AddDays((double)itemTemplateInfo.Property4);
			}
			else
			{
				client.Player.MainBag.UpdatePlayerProperties();
				itemAt.latentEnergyCurStr = itemAt.latentEnergyNewStr;
				msg = "Cập nhật tiềm năng thành công!";
			}
			itemAt.IsBinds = true;
			inventory.UpdateItem(itemAt);
			inventory.SaveToDatabase();
			gSPacketIn.WriteInt(itemAt.Place);
			gSPacketIn.WriteString(itemAt.latentEnergyCurStr);
			gSPacketIn.WriteString(itemAt.latentEnergyNewStr);
			gSPacketIn.WriteDateTime(itemAt.latentEnergyEndTime);
			client.Out.SendTCP(gSPacketIn);
			client.Player.SendMessage(msg);
			return 0;
		}
	}
}
