using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.NEWCHICKENBOX_SYS, "")]
    public class NewChickenBoxHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            byte num = packet.ReadByte();
            int iD = client.Player.PlayerCharacter.ID;
            GSPacketIn gSPacketIn = new GSPacketIn(87, iD);
            switch (num)
            {
                case 0:
                    {
                        gSPacketIn.WriteInt(0);//this._model.canOpenCounts = _local2.readInt();
                        gSPacketIn.WriteInt(3);//this._model.openCardPrice.push(_local2.readInt());
                        gSPacketIn.WriteInt(3);//this._model.canEagleEyeCounts = _local2.readInt();
                        gSPacketIn.WriteInt(3);// this._model.eagleEyePrice.push(_local2.readInt());
                        gSPacketIn.WriteInt(0);//this._model.flushPrice = _local2.readInt();
                        return 0;
                    }
            }
            Console.WriteLine("NewChickenBoxHandler: " + num);
            return 0;
        }
	}
}
