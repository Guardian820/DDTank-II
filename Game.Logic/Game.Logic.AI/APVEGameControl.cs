using System;
namespace Game.Logic.AI
{
	public abstract class APVEGameControl
	{
		protected PVEGame m_game;
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
		public virtual void OnCreated()
		{
		}
		public virtual void OnPrepated()
		{
		}
		public virtual void OnGameOverAllSession()
		{
		}
		public virtual int CalculateScoreGrade(int score)
		{
			return 0;
		}
		public virtual void Dispose()
		{
		}
	}
}
