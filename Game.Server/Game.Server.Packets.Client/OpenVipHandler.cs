using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(92, "场景用户离开")]
	public class OpenVipHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			string text = packet.ReadString();
			int num = packet.ReadInt();
			int num2 = 569;
			int num3 = 569;
			int num4 = 1707;
			int num5 = 3000;
			string message = "Você acabo de ativar VIP";
			int num6 = num;
			if (num6 != 30)
			{
				if (num6 != 90)
				{
					if (num6 == 180)
					{
						num2 = num5;
					}
				}
				else
				{
					num2 = num4;
				}
			}
			else
			{
				num2 = num3;
			}
			GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(text);
			if (num2 <= client.Player.PlayerCharacter.Money)
			{
				DateTime now = DateTime.Now;
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					playerBussiness.VIPRenewal(text, num, ref now);
					if (clientByPlayerNickName == null)
					{
						message = "Tiếp phí VIP cho " + text + " thàng công!";
					}
					else
					{
						if (client.Player.PlayerCharacter.NickName == text)
						{
							if (client.Player.PlayerCharacter.typeVIP == 0)
							{
								client.Player.OpenVIP(now);
							}
							else
							{
								client.Player.ContinousVIP(now);
                                message = "Renovação VIP bem sucedida!";
							}
							client.Out.SendOpenVIP(client.Player.PlayerCharacter);
						}
						else
						{
							string message2;
							if (clientByPlayerNickName.PlayerCharacter.typeVIP == 0)
							{
								clientByPlayerNickName.OpenVIP(now);
								message = "Kích hoạt VIP cho " + text + " thàng công!";
								message2 = client.Player.PlayerCharacter.NickName + ", tiếp phí VIP cho bạn thàng công!";
							}
							else
							{
								clientByPlayerNickName.ContinousVIP(now);
								message = "Gia hạn VIP cho " + text + " thàng công!";
								message2 = client.Player.PlayerCharacter.NickName + ", gia hạn VIP cho bạn thàng công!";
							}
							clientByPlayerNickName.Out.SendOpenVIP(clientByPlayerNickName.PlayerCharacter);
							clientByPlayerNickName.Out.SendMessage(eMessageType.Normal, message2);
						}
					}
					client.Player.AddExpVip(num2);
					client.Player.RemoveMoney(num2);
					client.Out.SendMessage(eMessageType.Normal, message);
					return 0;
				}
			}
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.Money", new object[0]));
			return 0;
		}
	}
}
