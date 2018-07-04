using Game.Logic.Phy.Object;
using log4net;
using System;
using System.Collections;
using System.Reflection;
using System.Threading;
namespace Game.Logic.Effects
{
	public class EffectList
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected ArrayList m_effects;
		protected readonly Living m_owner;
		protected volatile sbyte m_changesCount;
		protected int m_immunity;
		public ArrayList List
		{
			get
			{
				return this.m_effects;
			}
		}
		public EffectList(Living owner, int immunity)
		{
			this.m_owner = owner;
			this.m_effects = new ArrayList(3);
			this.m_immunity = immunity;
		}
		public bool CanAddEffect(int id)
		{
			return id > 35 || id < 0 || (1 << id - 1 & this.m_immunity) == 0;
		}
		public virtual bool Add(AbstractEffect effect)
		{
			if (this.CanAddEffect(effect.TypeValue))
			{
				ArrayList effects;
				Monitor.Enter(effects = this.m_effects);
				try
				{
					this.m_effects.Add(effect);
				}
				finally
				{
					Monitor.Exit(effects);
				}
				effect.OnAttached(this.m_owner);
				this.OnEffectsChanged(effect);
				return true;
			}
			if (effect.TypeValue == 9 && this.m_owner is SimpleBoss)
			{
				this.m_owner.State = 0;
			}
			return false;
		}
		public virtual bool Remove(AbstractEffect effect)
		{
			int num = -1;
			ArrayList effects;
			Monitor.Enter(effects = this.m_effects);
			try
			{
				num = this.m_effects.IndexOf(effect);
				if (num < 0)
				{
					bool result = false;
					return result;
				}
				this.m_effects.RemoveAt(num);
			}
			finally
			{
				Monitor.Exit(effects);
			}
			if (num != -1)
			{
				effect.OnRemoved(this.m_owner);
				this.OnEffectsChanged(effect);
				return true;
			}
			return false;
		}
		public virtual void OnEffectsChanged(AbstractEffect changedEffect)
		{
			if (this.m_changesCount > 0)
			{
				return;
			}
			this.UpdateChangedEffects();
		}
		public void BeginChanges()
		{
			this.m_changesCount += 1;
		}
		public virtual void CommitChanges()
		{
			if ((this.m_changesCount -= 1) < 0)
			{
				if (EffectList.log.IsWarnEnabled)
				{
					EffectList.log.Warn("changes count is less than zero, forgot BeginChanges()?\n" + Environment.StackTrace);
				}
				this.m_changesCount = 0;
			}
			bool flag = this.m_changesCount == 0;
			if (flag)
			{
				this.UpdateChangedEffects();
			}
		}
		protected virtual void UpdateChangedEffects()
		{
		}
		public virtual AbstractEffect GetOfType(eEffectType effectType)
		{
			ArrayList effects;
			Monitor.Enter(effects = this.m_effects);
			try
			{
				foreach (AbstractEffect abstractEffect in this.m_effects)
				{
					if (abstractEffect.Type == effectType)
					{
						return abstractEffect;
					}
				}
			}
			finally
			{
				Monitor.Exit(effects);
			}
			return null;
		}
		public virtual IList GetAllOfType(Type effectType)
		{
			ArrayList arrayList = new ArrayList();
			ArrayList effects;
			Monitor.Enter(effects = this.m_effects);
			try
			{
				foreach (AbstractEffect abstractEffect in this.m_effects)
				{
					if (abstractEffect.GetType().Equals(effectType))
					{
						arrayList.Add(abstractEffect);
					}
				}
			}
			finally
			{
				Monitor.Exit(effects);
			}
			return arrayList;
		}
		public void StopEffect(Type effectType)
		{
			IList allOfType = this.GetAllOfType(effectType);
			this.BeginChanges();
			foreach (AbstractEffect abstractEffect in allOfType)
			{
				abstractEffect.Stop();
			}
			this.CommitChanges();
		}
		public void StopAllEffect()
		{
			if (this.m_effects.Count > 0)
			{
				AbstractEffect[] array = new AbstractEffect[this.m_effects.Count];
				this.m_effects.CopyTo(array);
				AbstractEffect[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					AbstractEffect abstractEffect = array2[i];
					abstractEffect.Stop();
				}
				this.m_effects.Clear();
			}
		}
	}
}
