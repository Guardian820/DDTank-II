using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(153, "场景用户离开")]
	public class TreasureHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int iD = client.Player.PlayerCharacter.ID;
			GSPacketIn gSPacketIn = new GSPacketIn(153, iD);
			switch (num)
			{
			case 0:
				{
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(3);
					gSPacketIn.WriteInt(3);
					gSPacketIn.WriteInt(3);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteBoolean(false);
					bool val = false;
					if (client.Player.TreasureAdd.Count > 0)
					{
						val = true;
					}
					gSPacketIn.WriteBoolean(val);
					List<TreasureTempInfo> list = new List<TreasureTempInfo>();
					if (client.Player.TreasureTem.Count == 0)
					{
						client.Player.CreatTreasure();
					}
					list = client.Player.TreasureTem;
					gSPacketIn.WriteInt(list.Count);
					for (int i = 0; i < list.Count; i++)
					{
						gSPacketIn.WriteInt(list[i].TemplateID);
						gSPacketIn.WriteInt(list[i].ValidDate);
						gSPacketIn.WriteInt(list[i].Count);
					}
					list = client.Player.TreasureAdd;
					gSPacketIn.WriteInt(list.Count);
					for (int i = 0; i < list.Count; i++)
					{
						gSPacketIn.WriteInt(list[i].TemplateID);
						gSPacketIn.WriteInt(list[i].pos);
						gSPacketIn.WriteInt(list[i].ValidDate);
						gSPacketIn.WriteInt(list[i].Count);
					}
					client.Player.Out.SendTCP(gSPacketIn);
					return 0;
				}

			case 1:
				packet.ReadInt();
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteInt(0);
				client.Player.Out.SendTCP(gSPacketIn);
				return 0;

			case 3:
				{
					int num2 = packet.ReadInt();
					TreasureTempInfo treasureTempInfo = client.Player.TreasureTem[num2 - 1];
					gSPacketIn.WriteInt(3);
					gSPacketIn.WriteInt(treasureTempInfo.TemplateID);
					gSPacketIn.WriteInt(num2);
					gSPacketIn.WriteInt(treasureTempInfo.Count);
					gSPacketIn.WriteInt(2);
					gSPacketIn.WriteInt(2);
					client.Player.Out.SendTCP(gSPacketIn);
					treasureTempInfo.pos = num2;
					client.Player.UpdateTreasureAdd(treasureTempInfo);
					return 0;
				}

			case 6:
				gSPacketIn.WriteInt(6);
				gSPacketIn.WriteBoolean(true);
				client.Player.Out.SendTCP(gSPacketIn);
				return 0;
			}
			Console.WriteLine("//treasure_cmd: " + (TreasurePackageType)num);
			return 0;
		}
	}
}
