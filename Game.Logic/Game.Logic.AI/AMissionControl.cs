using System;
namespace Game.Logic.AI
{
	public abstract class AMissionControl
	{
		private PVEGame m_game;
		public PVEGame Game
		{
			get
			{
				return this.m_game;
			}
			set
			{
				this.m_game = value;
			}
		}
		public virtual void OnPrepareNewSession()
		{
		}
		public virtual void OnStartMovie()
		{
		}
		public virtual void OnStartGame()
		{
		}
		public virtual void OnBeginNewTurn()
		{
		}
		public virtual void OnNewTurnStarted()
		{
		}
		public virtual bool CanGameOver()
		{
			return true;
		}
		public virtual void OnGameOverMovie()
		{
		}
		public virtual void OnGameOver()
		{
		}
		public virtual int CalculateScoreGrade(int score)
		{
			return 0;
		}
		public virtual void GameOverAllSession()
		{
		}
		public virtual int UpdateUIData()
		{
			return 0;
		}
		public virtual void Dispose()
		{
		}
		public virtual void DoOther()
		{
		}
		public virtual void OnShooted()
		{
		}
	}
}
