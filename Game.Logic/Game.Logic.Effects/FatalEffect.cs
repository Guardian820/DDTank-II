using Bussiness;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class FatalEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public FatalEffect(int count, int probability) : base(eEffectType.FatalEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			FatalEffect fatalEffect = living.EffectList.GetOfType(eEffectType.FatalEffect) as FatalEffect;
			if (fatalEffect != null)
			{
				fatalEffect.m_probability = ((this.m_probability > fatalEffect.m_probability) ? this.m_probability : fatalEffect.m_probability);
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
			this.IsTrigger = false;
			if (this.rand.Next(100) < this.m_probability)
			{
				player.ShootMovieDelay = 50;
				this.IsTrigger = true;
				player.EffectTrigger = true;
				if (player.CurrentBall.ID != 3)
				{
					player.ControlBall = true;
				}
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("FatalEffect.Success", new object[0]));
			}
		}
	}
}
