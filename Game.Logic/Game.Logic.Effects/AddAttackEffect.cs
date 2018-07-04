using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddAttackEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		private int m_added;
		public AddAttackEffect(int count, int probability) : base(eEffectType.AddAttackEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddAttackEffect addAttackEffect = living.EffectList.GetOfType(eEffectType.AddAttackEffect) as AddAttackEffect;
			if (addAttackEffect != null)
			{
				this.m_probability = ((this.m_probability > addAttackEffect.m_probability) ? this.m_probability : addAttackEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.BeginAttacking += new LivingEventHandle(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeginAttacking -= new LivingEventHandle(this.ChangeProperty);
		}
		private void ChangeProperty(Living living)
		{
			living.Attack -= (double)this.m_added;
			this.m_added = 0;
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				living.EffectTrigger = true;
				living.Attack += (double)this.m_count;
				this.m_added = this.m_count;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddAttackEffect.Success", new object[]
				{
					this.m_count
				}));
			}
		}
	}
}
