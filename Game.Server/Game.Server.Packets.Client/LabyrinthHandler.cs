using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(131, "场景用户离开")]
	public class LabyrinthHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int num2 = num;
			if (num2 == 2)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(131, client.Player.PlayerCharacter.ID);
				gSPacketIn.WriteByte(2);
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteBoolean(false);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteBoolean(true);
				gSPacketIn.WriteBoolean(false);
				gSPacketIn.WriteBoolean(false);
				gSPacketIn.WriteBoolean(false);
				client.Player.Out.SendTCP(gSPacketIn);
			}
			else
			{
				Console.WriteLine("???labyrinth_cmd: " + (LabyrinthPackageType)num);
			}
			return 0;
		}
	}
}
