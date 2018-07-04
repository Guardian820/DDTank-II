using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ReflexDamageEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public ReflexDamageEquipEffect(int count, int probability) : base(eEffectType.ReflexDamageEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			ReflexDamageEquipEffect reflexDamageEquipEffect = living.EffectList.GetOfType(eEffectType.ReflexDamageEquipEffect) as ReflexDamageEquipEffect;
			if (reflexDamageEquipEffect != null)
			{
				reflexDamageEquipEffect.m_probability = ((this.m_probability > reflexDamageEquipEffect.m_probability) ? this.m_probability : reflexDamageEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.BeginAttacked += new LivingEventHandle(this.ChangeProperty);
			player.AfterKilledByLiving += new KillLivingEventHanlde(this.player_AfterKilledByLiving);
		}
		private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
		{
			if (this.IsTrigger)
			{
				target.AddBlood(-this.m_count);
			}
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeginAttacked -= new LivingEventHandle(this.ChangeProperty);
		}
		public void ChangeProperty(Living living)
		{
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				living.EffectTrigger = true;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("ReflexDamageEquipEffect.Success", new object[]
				{
					this.m_count
				}));
			}
		}
	}
}
