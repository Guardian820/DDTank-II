using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class NoHoleEffect : AbstractEffect
	{
		private int m_count;
		public NoHoleEffect(int count) : base(eEffectType.NoHoleEffect)
		{
			this.m_count = count;
		}
		public override bool Start(Living living)
		{
			NoHoleEffect noHoleEffect = living.EffectList.GetOfType(eEffectType.NoHoleEffect) as NoHoleEffect;
			if (noHoleEffect != null)
			{
				noHoleEffect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.IsNoHole = true;
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.IsNoHole = false;
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
