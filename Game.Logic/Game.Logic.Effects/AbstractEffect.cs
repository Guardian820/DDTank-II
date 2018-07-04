using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public abstract class AbstractEffect
	{
		private eEffectType m_type;
		protected Living m_living;
		protected Random rand;
		public bool IsTrigger;
		public eEffectType Type
		{
			get
			{
				return this.m_type;
			}
		}
		public int TypeValue
		{
			get
			{
				return (int)this.m_type;
			}
		}
		public AbstractEffect(eEffectType type)
		{
			this.rand = new Random();
			this.m_type = type;
		}
		public virtual bool Start(Living living)
		{
			this.m_living = living;
			return this.m_living.EffectList.Add(this);
		}
		public virtual bool Stop()
		{
			return this.m_living != null && this.m_living.EffectList.Remove(this);
		}
		public virtual void OnAttached(Living living)
		{
		}
		public virtual void OnRemoved(Living living)
		{
		}
	}
}
