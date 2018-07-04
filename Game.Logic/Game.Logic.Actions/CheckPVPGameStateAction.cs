using System;
namespace Game.Logic.Actions
{
	public class CheckPVPGameStateAction : IAction
	{
		private long m_tick;
		private bool m_isFinished;
		public CheckPVPGameStateAction(int delay)
		{
			this.m_isFinished = false;
			this.m_tick += TickHelper.GetTickCount() + (long)delay;
		}
		public void Execute(BaseGame game, long tick)
		{
			if (this.m_tick <= tick)
			{
				PVPGame pVPGame = game as PVPGame;
				if (pVPGame != null)
				{
					switch (game.GameState)
					{
					case eGameState.Inited:
						pVPGame.Prepare();
						break;

					case eGameState.Prepared:
						pVPGame.StartLoading();
						break;

					case eGameState.Loading:
						if (pVPGame.IsAllComplete())
						{
							pVPGame.StartGame();
						}
						break;

					case eGameState.Playing:
						if (pVPGame.CurrentPlayer == null || !pVPGame.CurrentPlayer.IsAttacking)
						{
							if (pVPGame.CanGameOver())
							{
								pVPGame.GameOver();
							}
							else
							{
								pVPGame.NextTurn();
							}
						}
						break;

					case eGameState.GameOver:
						pVPGame.Stop();
						break;
					}
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
