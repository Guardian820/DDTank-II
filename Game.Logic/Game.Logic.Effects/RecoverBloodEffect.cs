using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class RecoverBloodEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public RecoverBloodEffect(int count, int probability) : base(eEffectType.RecoverBloodEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			RecoverBloodEffect recoverBloodEffect = living.EffectList.GetOfType(eEffectType.RecoverBloodEffect) as RecoverBloodEffect;
			if (recoverBloodEffect == null)
			{
				return base.Start(living);
			}
			this.m_probability = ((this.m_probability > recoverBloodEffect.m_probability) ? this.m_probability : recoverBloodEffect.m_probability);
			return true;
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.AfterKilledByLiving += new KillLivingEventHanlde(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.AfterKilledByLiving -= new KillLivingEventHanlde(this.ChangeProperty);
		}
		public void ChangeProperty(Living living, Living target, int damageAmount, int criticalAmount)
		{
			if (!living.IsLiving)
			{
				return;
			}
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				this.IsTrigger = true;
				living.EffectTrigger = true;
				living.SyncAtTime = true;
				living.AddBlood(this.m_count);
				living.SyncAtTime = false;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("ContinueRecoverBloodEquipEffect.Success", new object[0]));
			}
		}
	}
}
