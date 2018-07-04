using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddAgilityEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probablity;
		private int m_added;
		public AddAgilityEffect(int count, int probability) : base(eEffectType.AddAgilityEffect)
		{
			this.m_count = count;
			this.m_probablity = probability;
		}
		public override bool Start(Living living)
		{
			AddAgilityEffect addAgilityEffect = living.EffectList.GetOfType(eEffectType.AddAgilityEffect) as AddAgilityEffect;
			if (addAgilityEffect != null)
			{
				this.m_probablity = ((this.m_probablity > addAgilityEffect.m_probablity) ? this.m_probablity : addAgilityEffect.m_probablity);
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
			living.Agility -= (double)this.m_added;
			this.m_added = 0;
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probablity)
			{
				living.EffectTrigger = true;
				this.IsTrigger = true;
				living.Agility += (double)this.m_count;
				this.m_added = this.m_count;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddAgilityEffect.Success", new object[]
				{
					this.m_count
				}));
			}
		}
		private void DefaultProperty(Living living)
		{
		}
	}
}
