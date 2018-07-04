using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class LockDirectionEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public LockDirectionEquipEffect(int count, int probability) : base(eEffectType.LockDirectionEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			LockDirectionEquipEffect lockDirectionEquipEffect = living.EffectList.GetOfType(eEffectType.LockDirectionEquipEffect) as LockDirectionEquipEffect;
			if (lockDirectionEquipEffect != null)
			{
				lockDirectionEquipEffect.m_probability = ((this.m_probability > lockDirectionEquipEffect.m_probability) ? this.m_probability : lockDirectionEquipEffect.m_probability);
				return true;
			}
			return base.Start(living);
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.PlayerShoot += new PlayerEventHandle(this.ChangeProperty);
			player.AfterKillingLiving += new KillLivingEventHanlde(this.player_AfterKillingLiving);
		}
		private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
		{
			if (this.IsTrigger)
			{
				target.AddEffect(new LockDirectionEffect(2), 0);
			}
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.PlayerShoot -= new PlayerEventHandle(this.ChangeProperty);
			player.AfterKillingLiving -= new KillLivingEventHanlde(this.player_AfterKillingLiving);
		}
		private void ChangeProperty(Player player)
		{
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				player.EffectTrigger = true;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("IceFronzeEquipEffect.Success", new object[0]));
			}
		}
	}
}
