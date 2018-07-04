using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ReduceDamageEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public ReduceDamageEffect(int count, int probability) : base(eEffectType.ReduceDamageEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
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
				damageAmount -= this.m_count;
				living.EffectTrigger = true;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("ReduceDamageEffect.Success", new object[0]));
			}
		}
	}
}
