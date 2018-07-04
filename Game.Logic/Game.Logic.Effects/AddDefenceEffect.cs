using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddDefenceEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		private int m_added;
		public AddDefenceEffect(int count, int probability) : base(eEffectType.AddDefenceEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_added = 0;
		}
		public override bool Start(Living living)
		{
			AddDefenceEffect addDefenceEffect = living.EffectList.GetOfType(eEffectType.AddDefenceEffect) as AddDefenceEffect;
			if (addDefenceEffect != null)
			{
				addDefenceEffect.m_probability = ((this.m_probability > addDefenceEffect.m_probability) ? this.m_probability : addDefenceEffect.m_probability);
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
		public void ChangeProperty(Living living)
		{
			living.Defence -= (double)this.m_added;
			this.m_added = 0;
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				living.Defence += (double)this.m_count;
				this.m_added = this.m_count;
				living.EffectTrigger = true;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddDefenceEffect.Success", new object[]
				{
					this.m_count
				}));
			}
		}
	}
}
