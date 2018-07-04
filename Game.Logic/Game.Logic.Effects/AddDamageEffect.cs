using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddDamageEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AddDamageEffect(int count, int probability) : base(eEffectType.AddDamageEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddDamageEffect addDamageEffect = living.EffectList.GetOfType(eEffectType.AddDamageEffect) as AddDamageEffect;
			if (addDamageEffect != null)
			{
				this.m_probability = ((this.m_probability > addDamageEffect.m_probability) ? this.m_probability : addDamageEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.TakePlayerDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.PlayerShoot += new PlayerEventHandle(this.playerShot);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.TakePlayerDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.PlayerShoot -= new PlayerEventHandle(this.playerShot);
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
		{
			if (this.IsTrigger)
			{
				damageAmount += this.m_count;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddDamageEffect.Success", new object[0]));
			}
		}
		private void playerShot(Player player)
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
