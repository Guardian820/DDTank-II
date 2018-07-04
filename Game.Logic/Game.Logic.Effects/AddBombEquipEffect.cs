using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddBombEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AddBombEquipEffect(int count, int probability) : base(eEffectType.AddBombEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddBombEquipEffect addBombEquipEffect = living.EffectList.GetOfType(eEffectType.AddBombEquipEffect) as AddBombEquipEffect;
			if (addBombEquipEffect != null)
			{
				this.m_probability = ((this.m_probability > addBombEquipEffect.m_probability) ? this.m_probability : addBombEquipEffect.m_probability);
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
				player.BallCount += this.m_count;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("IceFronzeEquipEffect.Success", new object[0]));
			}
		}
	}
}
