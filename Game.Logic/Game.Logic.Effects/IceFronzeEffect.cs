using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class IceFronzeEffect : AbstractEffect
	{
		private int m_count;
		public IceFronzeEffect(int count) : base(eEffectType.IceFronzeEffect)
		{
			this.m_count = count;
		}
		public override bool Start(Living living)
		{
			IceFronzeEffect iceFronzeEffect = living.EffectList.GetOfType(eEffectType.IceFronzeEffect) as IceFronzeEffect;
			if (iceFronzeEffect != null)
			{
				iceFronzeEffect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			living.IsFrost = true;
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.IsFrost = false;
		}
		private void player_BeginFitting(Living player)
		{
			this.m_count--;
			if (this.m_count < 0)
			{
				this.Stop();
			}
		}
	}
}
