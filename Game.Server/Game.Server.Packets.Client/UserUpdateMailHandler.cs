/*using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(114, "修改邮件的已读未读标志")]
	public class UserUpdateMailHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(114, client.Player.PlayerCharacter.ID);
			int mailID = packet.ReadInt();
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				MailInfo mailSingle = playerBussiness.GetMailSingle(client.Player.PlayerCharacter.ID, mailID);
				if (mailSingle != null && !mailSingle.IsRead)
				{
					mailSingle.IsRead = true;
					if (mailSingle.Type < 100)
					{
						mailSingle.ValidDate = 72;
						mailSingle.SendTime = DateTime.Now;
					}
					playerBussiness.UpdateMail(mailSingle, mailSingle.Money);
					gSPacketIn.WriteBoolean(true);
				}
				else
				{
					gSPacketIn.WriteBoolean(false);
				}
			}
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
	}
}*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using SqlDataProvider.Data;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.UPDATE_MAIL, "修改邮件的已读未读标志")]
    public class UserUpdateMailHandler : IPacketHandler
    {
        //修改:  Xiaov 
        //时间:  2009-11-4
        //描述:  修改邮件的已读未读标志<已测试>    
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            int id = packet.ReadInt();
            using (PlayerBussiness db = new PlayerBussiness())
            {
                MailInfo mes = db.GetMailSingle(client.Player.PlayerCharacter.ID, id);
                if (mes != null && !mes.IsRead)
                {
                    mes.IsRead = true;
                    if (mes.Type < 100)
                    {
                        mes.ValidDate = 3 * 24;
                        mes.SendTime = DateTime.Now;
                    }
                    db.UpdateMail(mes, mes.Money);
                    pkg.WriteBoolean(true);
                }
                else
                {
                    pkg.WriteBoolean(false);
                }
            }

            client.Out.SendTCP(pkg);

            return 0;
        }
    }
}

