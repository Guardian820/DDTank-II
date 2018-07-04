using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(159, "场景用户离开")]
	public class WonderfulActivityHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			packet.ReadInt();
			packet.ReadInt();
			int[] array = new int[]
			{
				3,
				2,
				4,
				1,
				5,
				7,
				6
			};
			GSPacketIn gSPacketIn = new GSPacketIn(159, client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteByte(0);
			gSPacketIn.WriteInt(7);
			for (int i = 0; i < 7; i++)
			{
				gSPacketIn.WriteInt(array[i]);
				if (i == 0)
				{
					gSPacketIn.WriteInt(1);
				}
				else
				{
					if (i == 1)
					{
						gSPacketIn.WriteInt(1);
					}
					else
					{
						gSPacketIn.WriteInt(1);
					}
				}
				gSPacketIn.WriteInt(-1);
				gSPacketIn.WriteDateTime(DateTime.Now.AddDays(-1.0));
				gSPacketIn.WriteDateTime(DateTime.Now.AddDays(7.0));
			}
			client.Player.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}
