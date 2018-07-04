using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(72, "大喇叭")]
	public class BigBugleHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int templateId = packet.ReadInt();
			ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
			if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(15.0), DateTime.Now) > 0)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Quá nhiều thao tác!", new object[0]));
				return 1;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(72);
			if (itemByTemplateID != null)
			{
				packet.ReadInt();
				packet.ReadString();
				string str = packet.ReadString();
				client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
				gSPacketIn.WriteInt(itemByTemplateID.Template.Property2);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
				gSPacketIn.WriteString(str);
				GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
				client.Player.LastChatTime = DateTime.Now;
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				for (int i = 0; i < allPlayers.Length; i++)
				{
					GamePlayer gamePlayer = allPlayers[i];
					gSPacketIn.ClientID = gamePlayer.PlayerCharacter.ID;
					gamePlayer.Out.SendTCP(gSPacketIn);
				}
			}
			else
			{
				packet.ReadString();
				string str2 = packet.ReadString();
				ItemInfo itemByCategoryID = client.Player.PropBag.GetItemByCategoryID(0, 11, 4);
				client.Player.PropBag.RemoveCountFromStack(itemByCategoryID, 1);
				gSPacketIn.WriteInt(4);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
				gSPacketIn.WriteString(str2);
				gSPacketIn.WriteString("gunnyII");
				GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
				client.Player.LastChatTime = DateTime.Now;
				GamePlayer[] allPlayers2 = WorldMgr.GetAllPlayers();
				for (int j = 0; j < allPlayers2.Length; j++)
				{
					GamePlayer gamePlayer2 = allPlayers2[j];
					gSPacketIn.ClientID = gamePlayer2.PlayerCharacter.ID;
					gamePlayer2.Out.SendTCP(gSPacketIn);
				}
			}
			return 0;
		}
	}
}
