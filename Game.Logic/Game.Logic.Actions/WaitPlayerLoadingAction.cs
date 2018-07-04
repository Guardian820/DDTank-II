using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
namespace Game.Logic.Actions
{
	public class WaitPlayerLoadingAction : IAction
	{
		private long m_time;
		private bool m_isFinished;
		public WaitPlayerLoadingAction(BaseGame game, int maxTime)
		{
			this.m_time = TickHelper.GetTickCount() + (long)maxTime;
			game.GameStarted += new GameEventHandle(this.game_GameStarted);
		}
		private void game_GameStarted(AbstractGame game)
		{
			game.GameStarted -= new GameEventHandle(this.game_GameStarted);
			this.m_isFinished = true;
		}
		public void Execute(BaseGame game, long tick)
		{
			if (!this.m_isFinished && tick > this.m_time && game.GameState == eGameState.Loading)
			{
				if (game.GameState == eGameState.Loading)
				{
					List<Player> allFightPlayers = game.GetAllFightPlayers();
					foreach (Player current in allFightPlayers)
					{
						if (current.LoadingProcess < 100)
						{
							game.SendPlayerRemove(current);
							game.RemovePlayer(current.PlayerDetail, false);
						}
					}
					game.CheckState(0);
				}
				this.m_isFinished = true;
			}
		}
		public bool IsFinished(long tick)
		{
			return this.m_isFinished;
		}
	}
}
