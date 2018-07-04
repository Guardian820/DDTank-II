using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AssimilateBloodEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AssimilateBloodEffect(int count, int probability) : base(eEffectType.AssimilateBloodEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AssimilateBloodEffect assimilateBloodEffect = living.EffectList.GetOfType(eEffectType.AssimilateBloodEffect) as AssimilateBloodEffect;
			if (assimilateBloodEffect != null)
			{
				assimilateBloodEffect.m_probability = ((this.m_probability > assimilateBloodEffect.m_probability) ? this.m_probability : assimilateBloodEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.TakePlayerDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.PlayerShoot += new PlayerEventHandle(this.player_PlayerShoot);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.TakePlayerDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.PlayerShoot -= new PlayerEventHandle(this.player_PlayerShoot);
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
		{
			if (this.IsTrigger)
			{
				living.AddBlood(damageAmount * this.m_count / 100);
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AssimilateBloodEffect.Success", new object[0]));
			}
		}
		private void player_PlayerShoot(Player player)
		{
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				player.EffectTrigger = true;
			}
		}
	}
}
