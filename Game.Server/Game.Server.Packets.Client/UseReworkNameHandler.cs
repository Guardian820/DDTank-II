using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(171, "场景用户离开")]
	public class UseReworkNameHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte bageType = packet.ReadByte();
			int place = packet.ReadInt();
			string newNickName = packet.ReadString();
			string text = "";
			PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
			inventory.RemoveItemAt(place);
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				playerBussiness.RenameNick(client.Player.PlayerCharacter.UserName, client.Player.PlayerCharacter.NickName, newNickName, ref text);
			}
			if (text != "")
			{
				client.Player.SendMessage(text);
			}
			return 0;
		}
	}
}
