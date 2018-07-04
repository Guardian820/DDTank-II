using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ContinueReduceDamageEffect : AbstractEffect
	{
		private int m_count;
		public ContinueReduceDamageEffect(int count) : base(eEffectType.ContinueReduceDamageEffect)
		{
			this.m_count = count;
		}
		public override bool Start(Living living)
		{
			ContinueReduceDamageEffect continueReduceDamageEffect = living.EffectList.GetOfType(eEffectType.ContinueDamageEffect) as ContinueReduceDamageEffect;
			if (continueReduceDamageEffect != null)
			{
				continueReduceDamageEffect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			if (living is Player)
			{
				(living as Player).BaseDamage = (living as Player).BaseDamage * 5.0 / 100.0;
			}
			living.Game.SendPlayerPicture(living, 4, true);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			(living as Player).BaseDamage = (living as Player).BaseDamage * 100.0 / 5.0;
			living.Game.SendPlayerPicture(living, 4, false);
		}
		private void player_BeginFitting(Living living)
		{
			this.m_count--;
			if (this.m_count < 0)
			{
				this.Stop();
			}
		}
	}
}
