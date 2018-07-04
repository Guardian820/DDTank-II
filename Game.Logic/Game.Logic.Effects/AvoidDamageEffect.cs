using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AvoidDamageEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AvoidDamageEffect(int count, int probability) : base(eEffectType.AvoidDamageEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AvoidDamageEffect avoidDamageEffect = living.EffectList.GetOfType(eEffectType.AvoidDamageEffect) as AvoidDamageEffect;
			if (avoidDamageEffect != null)
			{
				avoidDamageEffect.m_probability = ((this.m_probability > avoidDamageEffect.m_probability) ? this.m_probability : avoidDamageEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
		{
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				living.EffectTrigger = true;
				damageAmount *= 1 - this.m_count / 100;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AvoidDamageEffect.Success", new object[0]));
			}
		}
	}
}
