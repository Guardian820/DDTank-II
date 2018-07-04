using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ReduceStrengthEffect : AbstractEffect
	{
		private int m_count;
		public ReduceStrengthEffect(int count) : base(eEffectType.ReduceStrengthEffect)
		{
			this.m_count = count;
		}
		public override bool Start(Living living)
		{
			ReduceStrengthEffect reduceStrengthEffect = living.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) as ReduceStrengthEffect;
			if (reduceStrengthEffect != null)
			{
				reduceStrengthEffect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, 1, true);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, 1, false);
		}
		private void player_BeginFitting(Living living)
		{
			this.m_count--;
			if (living is Player)
			{
				(living as Player).Energy -= 50;
			}
			if (this.m_count < 0)
			{
				this.Stop();
			}
		}
	}
}
