using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(119, "物品比较")]
	public class ItemCompareHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int itemID = int.Parse(packet.ReadString());
			int result;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				ItemInfo userItemSingle = playerBussiness.GetUserItemSingle(itemID);
				if (userItemSingle != null)
				{
					GSPacketIn gSPacketIn = new GSPacketIn(119, client.Player.PlayerCharacter.ID);
					gSPacketIn.WriteInt(num);
					if (num == 4)
					{
						gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
					}
					if (num == 5)
					{
						gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
						gSPacketIn.WriteInt(userItemSingle.TemplateID);
					}
					gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
					gSPacketIn.WriteInt(userItemSingle.TemplateID);
					gSPacketIn.WriteInt(userItemSingle.ItemID);
					gSPacketIn.WriteInt(userItemSingle.StrengthenLevel);
					gSPacketIn.WriteInt(userItemSingle.AttackCompose);
					gSPacketIn.WriteInt(userItemSingle.AgilityCompose);
					gSPacketIn.WriteInt(userItemSingle.LuckCompose);
					gSPacketIn.WriteInt(userItemSingle.DefendCompose);
					gSPacketIn.WriteInt(userItemSingle.ValidDate);
					gSPacketIn.WriteBoolean(userItemSingle.IsBinds);
					gSPacketIn.WriteBoolean(userItemSingle.IsJudge);
					gSPacketIn.WriteBoolean(userItemSingle.IsUsed);
					if (userItemSingle.IsUsed)
					{
						gSPacketIn.WriteString(userItemSingle.BeginDate.ToString());
					}
					gSPacketIn.WriteInt(userItemSingle.Hole1);
					gSPacketIn.WriteInt(userItemSingle.Hole2);
					gSPacketIn.WriteInt(userItemSingle.Hole3);
					gSPacketIn.WriteInt(userItemSingle.Hole4);
					gSPacketIn.WriteInt(userItemSingle.Hole5);
					gSPacketIn.WriteInt(userItemSingle.Hole6);
					gSPacketIn.WriteString(userItemSingle.Template.Hole);
					gSPacketIn.WriteString(userItemSingle.Template.Pic);
					gSPacketIn.WriteInt(userItemSingle.Template.RefineryLevel);
					gSPacketIn.WriteDateTime(DateTime.Now);
					gSPacketIn.WriteByte((byte)userItemSingle.Hole5Level);
					gSPacketIn.WriteInt(userItemSingle.Hole5Exp);
					gSPacketIn.WriteByte((byte)userItemSingle.Hole6Level);
					gSPacketIn.WriteInt(userItemSingle.Hole6Exp);
					if (userItemSingle.IsGold)
					{
						gSPacketIn.WriteBoolean(userItemSingle.IsGold);
						gSPacketIn.WriteInt(userItemSingle.goldValidDate);
						gSPacketIn.WriteDateTime(userItemSingle.goldBeginTime);
					}
					else
					{
						gSPacketIn.WriteBoolean(false);
					}
					client.Out.SendTCP(gSPacketIn);
				}
				result = 1;
			}
			return result;
		}
	}
}
