using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddBloodEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public AddBloodEffect(int count, int probability) : base(eEffectType.AddBloodEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddBloodEffect addBloodEffect = living.EffectList.GetOfType(eEffectType.AddBloodEffect) as AddBloodEffect;
			if (addBloodEffect != null)
			{
				this.m_probability = ((this.m_probability > addBloodEffect.m_probability) ? this.m_probability : addBloodEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
			player.BeginAttacked += new LivingEventHandle(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
			player.BeginAttacked -= new LivingEventHandle(this.ChangeProperty);
		}
		public void ChangeProperty(Living living)
		{
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				living.EffectTrigger = true;
				living.Blood += this.m_count;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AddBloodEffect.Success", new object[]
				{
					this.m_count
				}));
			}
		}
	}
}
