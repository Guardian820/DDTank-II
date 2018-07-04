using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	public class TryAgainCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game is PVEGame)
			{
				PVEGame pVEGame = game as PVEGame;
				int num = packet.ReadInt();
				bool flag = packet.ReadBoolean();
				if (flag)
				{
					if (num == 1)
					{
						if (player.PlayerDetail.RemoveMoney(100) > 0)
						{
							pVEGame.WantTryAgain = 1;
							game.SendToAll(packet);
							player.PlayerDetail.LogAddMoney(AddMoneyType.Game, AddMoneyType.Game_TryAgain, player.PlayerDetail.PlayerCharacter.ID, 100, player.PlayerDetail.PlayerCharacter.Money);
						}
						else
						{
							player.PlayerDetail.SendInsufficientMoney(2);
						}
					}
					else
					{
						pVEGame.WantTryAgain = 0;
						game.SendToAll(packet);
					}
					pVEGame.CheckState(0);
				}
			}
		}
	}
}
