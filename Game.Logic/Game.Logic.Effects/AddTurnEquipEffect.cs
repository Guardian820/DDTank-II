using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddTurnEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AddTurnEquipEffect(int count, int probability) : base(eEffectType.AddTurnEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddTurnEquipEffect addTurnEquipEffect = living.EffectList.GetOfType(eEffectType.AddTurnEquipEffect) as AddTurnEquipEffect;
			if (addTurnEquipEffect != null)
			{
				addTurnEquipEffect.m_probability = ((this.m_probability > addTurnEquipEffect.m_probability) ? this.m_probability : addTurnEquipEffect.m_probability);
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
				player.Delay = player.DefaultDelay;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AddTurnEquipEffect.Success", new object[0]));
			}
		}
	}
}
