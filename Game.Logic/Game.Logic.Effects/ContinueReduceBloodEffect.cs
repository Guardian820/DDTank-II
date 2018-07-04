using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ContinueReduceBloodEffect : AbstractEffect
	{
		private int m_count;
		private int m_blood;
		public ContinueReduceBloodEffect(int count, int blood) : base(eEffectType.ContinueReduceBloodEffect)
		{
			this.m_count = count;
			this.m_blood = blood;
		}
		public override bool Start(Living living)
		{
			ContinueReduceBloodEffect continueReduceBloodEffect = living.EffectList.GetOfType(eEffectType.ContinueDamageEffect) as ContinueReduceBloodEffect;
			if (continueReduceBloodEffect != null)
			{
				continueReduceBloodEffect.m_count = this.m_count;
				return true;
			}
			return base.Start(living);
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, 2, true);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, 2, false);
		}
		private void player_BeginFitting(Living living)
		{
			this.m_count--;
            if (living is Player)
            {
                Player p = (living as Player);

                if (p.Blood < Math.Abs(m_blood))
                {
                    p.ReducedBlood(-p.Blood + 2);//MrPhuong
                }
                else
                {
                    p.ReducedBlood(m_blood);//MrPhuong
                }
            }
            if (living is NormalNpc)
            {
                NormalNpc p = (living as NormalNpc);

                if (p.Blood < Math.Abs(m_blood))
                {
                    p.ReducedBlood(-p.Blood + 2);//MrPhuong
                }
                else
                {
                    p.ReducedBlood(m_blood);//MrPhuong
                }
            }
            if (m_count < 0)
            {
                Stop();
            }
		}
	}
}
