using Bussiness;
using Bussiness.Managers;
using Game.Logic.Phy.Object;
using Game.Logic.Spells;
using System;
namespace Game.Logic.Effects
{
	public class NoHoleEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public NoHoleEquipEffect(int count, int probability) : base(eEffectType.NoHoleEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			NoHoleEquipEffect noHoleEquipEffect = living.EffectList.GetOfType(eEffectType.NoHoleEquipEffect) as NoHoleEquipEffect;
			if (noHoleEquipEffect != null)
			{
				noHoleEquipEffect.m_probability = ((this.m_probability > noHoleEquipEffect.m_probability) ? this.m_probability : noHoleEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.AfterKilledByLiving += new KillLivingEventHanlde(this.player_AfterKilledByLiving);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.AfterKilledByLiving -= new KillLivingEventHanlde(this.player_AfterKilledByLiving);
		}
		private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
		{
			if (this.rand.Next(100) < this.m_probability)
			{
				living.EffectTrigger = true;
				SpellMgr.ExecuteSpell(living.Game, living as Player, ItemMgr.FindItemTemplate(10021));
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("NoHoleEquipEffect.Success", new object[0]));
			}
		}
	}
}
