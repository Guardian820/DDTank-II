using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class SealEffect : AbstractEffect
	{
		private int m_count;
		private int m_type;
		public SealEffect(int count, int type) : base(eEffectType.SealEffect)
		{
			this.m_count = count;
			this.m_type = type;
		}
		public override bool Start(Living living)
		{
			SealEffect sealEffect = living.EffectList.GetOfType(eEffectType.SealEffect) as SealEffect;
			if (sealEffect != null)
			{
				sealEffect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
		}
		private void player_BeginFitting(Living living)
		{
			this.m_count--;
			if (this.m_count <= 0)
			{
				this.Stop();
			}
		}
	}
}
