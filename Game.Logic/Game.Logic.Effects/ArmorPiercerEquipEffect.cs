using Bussiness;
using Bussiness.Managers;
using Game.Logic.Phy.Object;
using Game.Logic.Spells;
using System;
namespace Game.Logic.Effects
{
	public class ArmorPiercerEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public ArmorPiercerEquipEffect(int count, int probability) : base(eEffectType.ArmorPiercer)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			ArmorPiercerEquipEffect armorPiercerEquipEffect = living.EffectList.GetOfType(eEffectType.ArmorPiercer) as ArmorPiercerEquipEffect;
			if (armorPiercerEquipEffect != null)
			{
				armorPiercerEquipEffect.m_probability = ((this.m_probability > armorPiercerEquipEffect.m_probability) ? this.m_probability : armorPiercerEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
		}
		private void ChangeProperty(Player player)
		{
			if (this.rand.Next(100) < this.m_probability)
			{
				SpellMgr.ExecuteSpell(player.Game, player, ItemMgr.FindItemTemplate(10020));
				player.EffectTrigger = true;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("ArmorPiercerEquipEffect.Success", new object[0]));
			}
		}
	}
}
