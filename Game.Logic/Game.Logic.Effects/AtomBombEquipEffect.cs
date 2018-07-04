using Bussiness;
using Bussiness.Managers;
using Game.Logic.Phy.Object;
using Game.Logic.Spells;
using System;
namespace Game.Logic.Effects
{
	public class AtomBombEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AtomBombEquipEffect(int count, int probability) : base(eEffectType.AtomBomb)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AtomBombEquipEffect atomBombEquipEffect = living.EffectList.GetOfType(eEffectType.AtomBomb) as AtomBombEquipEffect;
			if (atomBombEquipEffect != null)
			{
				atomBombEquipEffect.m_probability = ((this.m_probability > atomBombEquipEffect.m_probability) ? this.m_probability : atomBombEquipEffect.m_probability);
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
				SpellMgr.ExecuteSpell(player.Game, player, ItemMgr.FindItemTemplate(10022));
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AtomBombEquipEffect.Success", new object[0]));
			}
		}
	}
}
