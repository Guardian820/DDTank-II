using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class MakeCriticalEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public MakeCriticalEffect(int count, int probability) : base(eEffectType.MakeCriticalEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			MakeCriticalEffect makeCriticalEffect = living.EffectList.GetOfType(eEffectType.MakeCriticalEffect) as MakeCriticalEffect;
			if (makeCriticalEffect != null)
			{
				makeCriticalEffect.m_probability = ((this.m_probability > makeCriticalEffect.m_probability) ? this.m_probability : makeCriticalEffect.m_probability);
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
				criticalAmount = (int)(0.5 + living.Lucky * 0.0005 * (double)damageAmount);
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("MakeCriticalEffect.Success", new object[0]));
			}
		}
	}
}
