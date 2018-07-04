using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AssimilateDamageEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AssimilateDamageEffect(int count, int probability) : base(eEffectType.AssimilateDamageEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AssimilateDamageEffect assimilateDamageEffect = living.EffectList.GetOfType(eEffectType.AssimilateDamageEffect) as AssimilateDamageEffect;
			if (assimilateDamageEffect != null)
			{
				assimilateDamageEffect.m_probability = ((this.m_probability > assimilateDamageEffect.m_probability) ? this.m_probability : assimilateDamageEffect.m_probability);
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
				damageAmount = -damageAmount;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AvoidDamageEffect.Success", new object[0]));
			}
		}
	}
}
