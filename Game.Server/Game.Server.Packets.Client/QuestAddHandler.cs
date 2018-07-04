using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(176, "添加任务")]
	public class QuestAddHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			for (int i = 0; i < num; i++)
			{
				int id = packet.ReadInt();
				QuestInfo singleQuest = QuestMgr.GetSingleQuest(id);
				string text;
				client.Player.QuestInventory.AddQuest(singleQuest, out text);
			}
			return 0;
		}
	}
}
