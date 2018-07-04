using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ReduceStrengthEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public ReduceStrengthEquipEffect(int count, int probability) : base(eEffectType.ReduceStrengthEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			ReduceStrengthEquipEffect reduceStrengthEquipEffect = living.EffectList.GetOfType(eEffectType.ReduceStrengthEquipEffect) as ReduceStrengthEquipEffect;
			if (reduceStrengthEquipEffect != null)
			{
				reduceStrengthEquipEffect.m_probability = ((this.m_probability > reduceStrengthEquipEffect.m_probability) ? this.m_probability : reduceStrengthEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
			player.AfterKillingLiving += new KillLivingEventHanlde(this.player_AfterKillingLiving);
		}
		private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
		{
			if (this.IsTrigger)
			{
				target.AddEffect(new ReduceStrengthEffect(2), 0);
			}
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
			player.AfterKillingLiving -= new KillLivingEventHanlde(this.player_AfterKillingLiving);
		}
		private void ChangeProperty(Player player)
		{
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				player.EffectTrigger = true;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("ArmorPiercerEquipEffect.Success", new object[0]));
			}
		}
	}
}
