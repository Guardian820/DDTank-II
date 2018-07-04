using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingBeatAction : BaseAction
	{
		private Living m_living;
		private Living m_target;
		private int m_demageAmount;
		private int m_criticalAmount;
		private string m_action;
		private int m_livingCount;
		private int m_attackEffect;
		public LivingBeatAction(Living living, Living target, int demageAmount, int criticalAmount, string action, int delay, int livingCount, int attackEffect) : base(delay)
		{
			this.m_living = living;
			this.m_target = target;
			this.m_demageAmount = demageAmount;
			this.m_criticalAmount = criticalAmount;
			this.m_action = action;
			this.m_livingCount = livingCount;
			this.m_attackEffect = attackEffect;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			this.m_target.SyncAtTime = false;
			try
			{
				if (this.m_target.TakeDamage(this.m_living, ref this.m_demageAmount, ref this.m_criticalAmount, "LivingFire"))
				{
					int totalDemageAmount = this.m_demageAmount + this.m_criticalAmount;
					game.SendLivingBeat(this.m_living, this.m_target, totalDemageAmount, this.m_action, this.m_livingCount, this.m_attackEffect);
				}
				this.m_target.IsFrost = false;
				base.Finish(tick);
			}
			finally
			{
				this.m_target.SyncAtTime = true;
			}
		}
	}
}
