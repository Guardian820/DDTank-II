using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	public class MissionStartCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game.GameState == eGameState.SessionPrepared || game.GameState == eGameState.GameOver)
			{
				bool flag = packet.ReadBoolean();
				if (flag)
				{
					player.Ready = true;
					game.CheckState(0);
				}
			}
		}
	}
}
