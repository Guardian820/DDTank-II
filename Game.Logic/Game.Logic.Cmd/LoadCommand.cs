using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(16, "游戏加载进度")]
	public class LoadCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game.GameState == eGameState.Loading)
			{
				player.LoadingProcess = packet.ReadInt();
				if (player.LoadingProcess >= 100)
				{
					game.CheckState(0);
				}
				GSPacketIn gSPacketIn = new GSPacketIn(91);
				gSPacketIn.WriteByte(16);
				gSPacketIn.WriteInt(player.LoadingProcess);
				gSPacketIn.WriteInt(4);
				gSPacketIn.WriteInt(player.PlayerDetail.PlayerCharacter.ID);
				game.SendToAll(gSPacketIn);
			}
		}
	}
}
