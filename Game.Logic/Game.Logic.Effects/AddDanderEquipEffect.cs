using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddDanderEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AddDanderEquipEffect(int count, int probability) : base(eEffectType.AddDander)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddDanderEquipEffect addDanderEquipEffect = living.EffectList.GetOfType(eEffectType.AddDander) as AddDanderEquipEffect;
			if (addDanderEquipEffect != null)
			{
				this.m_probability = ((this.m_probability > addDanderEquipEffect.m_probability) ? this.m_probability : addDanderEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.BeginAttacked += new LivingEventHandle(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeginAttacked -= new LivingEventHandle(this.ChangeProperty);
		}
		private void ChangeProperty(Living player)
		{
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				if (player is Player)
				{
					(player as Player).AddDander(this.m_count);
				}
				player.EffectTrigger = true;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AddDanderEquipEffect.Success", new object[0]));
			}
		}
	}
}
