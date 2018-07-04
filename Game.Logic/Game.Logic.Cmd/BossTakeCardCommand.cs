using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(130, "战胜关卡中Boss翻牌")]
	public class BossTakeCardCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game is PVEGame)
			{
				PVEGame pVEGame = game as PVEGame;
				if (pVEGame.BossCardCount + 1 > 0)
				{
					int num = (int)packet.ReadByte();
					if (num < 0 || num > pVEGame.BossCards.Length)
					{
						if (pVEGame.IsBossWar != "")
						{
							pVEGame.TakeBossCard(player);
							return;
						}
						pVEGame.TakeCard(player);
						return;
					}
					else
					{
						if (pVEGame.IsBossWar != "")
						{
							pVEGame.TakeBossCard(player, num);
							return;
						}
						pVEGame.TakeCard(player, num);
					}
				}
			}
		}
	}
}
