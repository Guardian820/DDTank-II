using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(134, "")]
	public class DiceSystemHandler : IPacketHandler
	{
		private void EnterDice(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(134);
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteBoolean(player.DicePlace.UserFirstCell);
			gSPacketIn.WriteInt(player.DicePlace.CurrentPosition);
			gSPacketIn.WriteInt(player.DicePlace.LuckIntegralLevel);
			gSPacketIn.WriteInt(player.DicePlace.LuckIntegral);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(player.DicePlace.ItemDice.Count);
			foreach (DiceSystemItem current in player.DicePlace.ItemDice)
			{
				gSPacketIn.WriteInt(current.TemplateID);
				gSPacketIn.WriteInt(current.Position);
				gSPacketIn.WriteInt(current.StrengthLevel);
				gSPacketIn.WriteInt(current.Count);
				gSPacketIn.WriteInt(current.Validate);
				gSPacketIn.WriteBoolean(current.IsBind);
			}
			player.Out.SendTCP(gSPacketIn);
		}
		private void receiveResult(GamePlayer player, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			packet.ReadInt();
			ThreadSafeRandom threadSafeRandom = new ThreadSafeRandom();
			int num2;
			int num3;
			switch (num)
			{
			case 1:
				num2 = threadSafeRandom.Next(2, 13);
				num3 = DiceSystemMgr.MoneyXUDoi;
				break;

			case 2:
				num2 = threadSafeRandom.Next(4, 7);
				num3 = DiceSystemMgr.MoneyXULon;
				break;

			case 3:
				num2 = threadSafeRandom.Next(1, 4);
				num3 = DiceSystemMgr.MoneyXUNho;
				break;

			default:
				num2 = threadSafeRandom.Next(1, 7);
				num3 = DiceSystemMgr.MoneyMacDinh;
				break;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(134);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteInt(player.DicePlace.CurrentPosition);
			gSPacketIn.WriteInt(num2);
			player.DicePlace.CurrentPosition += num2;
			if (player.DicePlace.CurrentPosition > 18)
			{
				player.DicePlace.CurrentPosition -= 18;
			}
			int num4 = threadSafeRandom.Next(6, 9);
			player.DicePlace.LuckIntegral += num4;
			if (player.DicePlace.LuckIntegral > 50 && player.DicePlace.LuckIntegralLevel == -1)
			{
				player.DicePlace.LuckIntegralLevel = 0;
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(11036), 1, 116);
				ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(200549), 1, 116);
				itemInfo.Count = 1;
				itemInfo2.Count = 1;
				itemInfo.IsBinds = true;
				itemInfo2.IsBinds = true;
				player.AddItem(itemInfo);
				player.AddItem(itemInfo2);
			}
			if (player.DicePlace.LuckIntegral > 250 && player.DicePlace.LuckIntegralLevel == 0)
			{
				player.DicePlace.LuckIntegralLevel = 1;
				ItemInfo itemInfo3 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(40002), 1, 116);
				ItemInfo itemInfo4 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(200549), 1, 116);
				itemInfo3.Count = 1;
				itemInfo4.Count = 1;
				itemInfo3.IsBinds = true;
				itemInfo4.IsBinds = true;
				player.AddItem(itemInfo3);
				player.AddItem(itemInfo4);
			}
			gSPacketIn.WriteInt(player.DicePlace.LuckIntegral);
			gSPacketIn.WriteInt(player.DicePlace.LuckIntegralLevel);
			gSPacketIn.WriteInt(0);
			DiceSystemItem diceSystemItem = player.DicePlace.ItemDice[player.DicePlace.CurrentPosition];
			ItemInfo itemInfo5 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(diceSystemItem.TemplateID), 1, 116);
			itemInfo5.IsBinds = diceSystemItem.IsBind;
			itemInfo5.StrengthenLevel = diceSystemItem.StrengthLevel;
			itemInfo5.Count = diceSystemItem.Count;
			itemInfo5.ValidDate = diceSystemItem.Validate;
			gSPacketIn.WriteString(string.Concat(new object[]
			{
				itemInfo5.Template.Name,
				" x ",
				itemInfo5.Count,
				"."
			}));
			if (player.RemoveMoney(num3) == num3)
			{
				player.AddItem(itemInfo5);
			}
			player.Out.SendTCP(gSPacketIn);
		}
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			switch (packet.ReadByte())
			{
			case 10:
				this.EnterDice(client.Player, packet);
				break;

			case 11:
				this.receiveResult(client.Player, packet);
				break;

			case 12:
				this.RefreshData(client.Player, packet);
				break;
			}
			return 0;
		}
		private void RefreshData(GamePlayer player, GSPacketIn packet)
		{
			int moneyLamMS = DiceSystemMgr.MoneyLamMS;
			if (player.RemoveMoney(moneyLamMS) == moneyLamMS)
			{
				player.DicePlace.ItemDice = DiceSystemMgr.TaoMoidiem();
				ThreadSafeRandom threadSafeRandom = new ThreadSafeRandom();
				int num = threadSafeRandom.Next(6, 9);
				player.DicePlace.LuckIntegral += num;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(134);
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteBoolean(player.DicePlace.UserFirstCell);
			gSPacketIn.WriteInt(player.DicePlace.CurrentPosition);
			gSPacketIn.WriteInt(player.DicePlace.LuckIntegralLevel);
			gSPacketIn.WriteInt(player.DicePlace.LuckIntegral);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(player.DicePlace.ItemDice.Count);
			foreach (DiceSystemItem current in player.DicePlace.ItemDice)
			{
				gSPacketIn.WriteInt(current.TemplateID);
				gSPacketIn.WriteInt(current.Position);
				gSPacketIn.WriteInt(current.StrengthLevel);
				gSPacketIn.WriteInt(current.Count);
				gSPacketIn.WriteInt(current.Validate);
				gSPacketIn.WriteBoolean(current.IsBind);
			}
			player.Out.SendTCP(gSPacketIn);
		}
	}
}
