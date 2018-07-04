using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class HideEffect : AbstractEffect
	{
		private int m_count;
		public HideEffect(int count) : base(eEffectType.HideEffect)
		{
			this.m_count = count;
		}
		public override bool Start(Living living)
		{
			HideEffect hideEffect = living.EffectList.GetOfType(eEffectType.HideEffect) as HideEffect;
			if (hideEffect != null)
			{
				hideEffect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			living.IsHide = true;
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			living.IsHide = false;
		}
		private void player_BeginFitting(Living player)
		{
			this.m_count--;
			if (this.m_count <= 0)
			{
				this.Stop();
			}
		}
	}
}
