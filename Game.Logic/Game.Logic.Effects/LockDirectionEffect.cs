using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class LockDirectionEffect : AbstractEffect
	{
		private int m_count;
		public LockDirectionEffect(int count) : base(eEffectType.LockDirectionEffect)
		{
			this.m_count = count;
		}
		public override bool Start(Living living)
		{
			LockDirectionEffect lockDirectionEffect = living.EffectList.GetOfType(eEffectType.ContinueDamageEffect) as LockDirectionEffect;
			if (lockDirectionEffect != null)
			{
				lockDirectionEffect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, 3, true);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, 3, false);
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
