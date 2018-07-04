using System;
namespace Game.Logic.Actions
{
	public class PlaySoundAction : BaseAction
	{
		private string m_action;
		public PlaySoundAction(string action, int delay) : base(delay, 1000)
		{
			this.m_action = action;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			((PVEGame)game).SendPlaySound(this.m_action);
			base.Finish(tick);
		}
	}
}
