using log4net;
using System;
using System.Reflection;
namespace Game.Logic.Actions
{
	public class CheckPVEGameStateAction : IAction
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private long m_time;
		private bool m_isFinished;
		public CheckPVEGameStateAction(int delay)
		{
			this.m_time = TickHelper.GetTickCount() + (long)delay;
			this.m_isFinished = false;
		}
		public void Execute(BaseGame game, long tick)
		{
			if (this.m_time <= tick && game.GetWaitTimer() < tick)
			{
				PVEGame pVEGame = game as PVEGame;
				if (pVEGame != null)
				{
					switch (pVEGame.GameState)
					{
					case eGameState.Inited:
						pVEGame.Prepare();
						break;

					case eGameState.Prepared:
						pVEGame.PrepareNewSession();
						break;

					case eGameState.Loading:
						if (!pVEGame.IsAllComplete())
						{
							game.WaitTime(1000);
						}
						else
						{
							pVEGame.StartGame();
						}
						break;

					case eGameState.GameStartMovie:
						if (game.CurrentActionCount > 1)
						{
							pVEGame.StartGameMovie();
						}
						else
						{
							pVEGame.StartGame();
						}
						break;

					case eGameState.GameStart:
						pVEGame.PrepareNewGame();
						break;

					case eGameState.Playing:
						if ((pVEGame.CurrentLiving == null || !pVEGame.CurrentLiving.IsAttacking) && game.CurrentActionCount <= 1)
						{
							if (pVEGame.CanGameOver())
							{
								pVEGame.GameOver();
							}
							else
							{
								pVEGame.NextTurn();
							}
						}
						break;

					case eGameState.GameOver:
						if (!pVEGame.HasNextSession())
						{
							pVEGame.GameOverAllSession();
						}
						else
						{
							pVEGame.PrepareNewSession();
						}
						break;

					case eGameState.SessionPrepared:
						if (!pVEGame.CanStartNewSession())
						{
							game.WaitTime(1000);
						}
						else
						{
							pVEGame.StartLoading();
						}
						break;

					case eGameState.ALLSessionStopped:
						if (pVEGame.PlayerCount == 0 || pVEGame.WantTryAgain == 0)
						{
							pVEGame.Stop();
						}
						else
						{
							if (pVEGame.WantTryAgain == 1)
							{
								pVEGame.SessionId--;
								pVEGame.PrepareNewSession();
							}
							else
							{
								game.WaitTime(1000);
							}
						}
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
