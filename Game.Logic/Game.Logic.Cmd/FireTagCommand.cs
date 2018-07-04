using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(96, "准备开炮")]
	public class FireTagCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (player.IsAttacking)
			{
				packet.Parameter1 = player.Id;
				game.SendToAll(packet);
				game.SendSyncLifeTime();
				bool flag = packet.ReadBoolean();
				if (flag)
				{
					byte speedTime = packet.ReadByte();
					player.PrepareShoot(speedTime);
				}
			}
		}
	}
}
