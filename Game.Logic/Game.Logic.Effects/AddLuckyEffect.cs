using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddLuckyEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		private int m_added;
		public AddLuckyEffect(int count, int probability) : base(eEffectType.AddLuckyEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_added = 0;
		}
		public override bool Start(Living living)
		{
			AddLuckyEffect addLuckyEffect = living.EffectList.GetOfType(eEffectType.AddLuckyEffect) as AddLuckyEffect;
			if (addLuckyEffect != null)
			{
				addLuckyEffect.m_probability = ((this.m_probability > addLuckyEffect.m_probability) ? this.m_probability : addLuckyEffect.m_probability);
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
			living.Lucky -= (double)this.m_added;
			this.m_added = 0;
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				living.Lucky += (double)this.m_count;
				living.EffectTrigger = true;
				this.m_added = this.m_count;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddLuckyEffect.Success", new object[]
				{
					this.m_count
				}));
			}
		}
	}
}
