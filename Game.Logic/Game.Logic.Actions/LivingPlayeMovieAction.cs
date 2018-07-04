using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingPlayeMovieAction : BaseAction
	{
		private Living m_living;
		private string m_action;
		public LivingPlayeMovieAction(Living living, string action, int delay, int movieTime) : base(delay, movieTime)
		{
			this.m_living = living;
			this.m_action = action;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			game.SendLivingPlayMovie(this.m_living, this.m_action);
			base.Finish(tick);
		}
	}
}
