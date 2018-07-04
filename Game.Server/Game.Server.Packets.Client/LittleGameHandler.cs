using System;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.LITTLEGAME_COMMAND, "SanBoss")]
    public class LittleGameHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            var little = packet.ReadByte();
            Console.WriteLine("LittleGameHandler: " + little.ToString());
            int iD = client.Player.PlayerCharacter.ID;
            GSPacketIn gSPacketIn = new GSPacketIn(166, iD);

            /*int iD = client.Player.PlayerCharacter.ID;
            GSPacketIn gSPacketIn = new GSPacketIn(166, iD);
            byte b2 = b;
            if (b2 == 2)
            {
                gSPacketIn.WriteInt(2);
                gSPacketIn.WriteInt(1);
                gSPacketIn.WriteInt(1);
                gSPacketIn.WriteString("bogu4,bogu5,bogu6,bogu7,bogu8");
                gSPacketIn.WriteString("2001");
                client.Player.Out.SendTCP(gSPacketIn);
            }
            else
            {
                Console.WriteLine("//LittleGame_cmd: " + (LittleGamePackageInType)b);
            }*/

            switch (little)
            {
                case (int)LittleGamePackageInType.WORLD_LIST:
                    {
                        break;
                    }
                case (int)LittleGamePackageInType.START_LOAD:
                    {
                        gSPacketIn.WriteInt(2);
                        gSPacketIn.WriteInt(1);
                        gSPacketIn.WriteInt(1);
                        gSPacketIn.WriteString("bogu4,bogu5,bogu6,bogu7,bogu8");
                        gSPacketIn.WriteString("2001");
                        client.Player.Out.SendTCP(gSPacketIn);
                        Console.WriteLine("Load Started: ");
                        break;
                    }
                case (int)LittleGamePackageInType.ADD_SPRITE:
                    {
                        break;
                    }
                case (int)LittleGamePackageInType.REMOVE_SPRITE:
                    {
                        break;
                    }
                case (int)LittleGamePackageInType.GAME_START:
                    {
                        break;
                    }
                case (int)LittleGamePackageInType.MOVE:
                    {
                        gSPacketIn.WriteInt(1);//_loc_9.writeInt(param3);
                        gSPacketIn.WriteInt(1);//_loc_9.writeInt(param4);
                        gSPacketIn.WriteInt(1);//_loc_9.writeInt(param5);
                        gSPacketIn.WriteInt(1);//_loc_9.writeInt(param6);
                        gSPacketIn.WriteInt(1);//_loc_9.writeInt(param7);
                        break;
                    }
                case (int)LittleGamePackageInType.UPDATE_POS:
                    {
                        break;
                    }
                case (int)LittleGamePackageInType.ADD_OBJECT:
                    {
                        break;
                    }
                case (int)LittleGamePackageInType.UPDATELIVINGSPROPERTY:
                    {
                        break;
                    }
                case (int)LittleGamePackageInType.DoAction:
                    {
                        gSPacketIn.WriteString("SkelletonX");//var _loc_3:* = param2.readUTF();
                        break;
                    }
                case (int)LittleGamePackageInType.DoMovie:
                    {
                        gSPacketIn.WriteInt(0);//var _loc_3:* = param2.readInt();
                        gSPacketIn.WriteString("SkelletonX");//var _loc_5:* = param2.readUTF();
                        break;
                    }
                case (int)LittleGamePackageInType.GETSCORE:
                    {
                        gSPacketIn.WriteInt(0);//var _loc_3:* = param2.readInt();
                        break;
                    }
                case (int)LittleGamePackageInType.INVOKE_OBJECT:
                    {
                        break;
                    }
                case (int)LittleGamePackageInType.SETCLOCK:
                    {
                        break;
                    }
                case (int)LittleGamePackageInType.PONG:
                    {
                        gSPacketIn.WriteInt(0);//var _loc_3:* = param2.readInt();
                        break;
                    }
                case (int)LittleGamePackageInType.NET_DELAY:
                    {
                        gSPacketIn.WriteInt(0);//var _loc_3:* = param2.readInt();
                        break;
                    }
                case (int)LittleGamePackageInType.KICK_PLAYE:
                    {


                        break;
                    }
            }
           // client.Player.SendMessage("Chức năng chưa mở");
            return 0;
        }
        
	}
}
