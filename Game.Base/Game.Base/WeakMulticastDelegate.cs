using log4net;
using System;
using System.Reflection;
using System.Text;
namespace Game.Base
{
	public class WeakMulticastDelegate
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private WeakReference weakRef;
		private MethodInfo method;
		private WeakMulticastDelegate prev;
		public WeakMulticastDelegate(Delegate realDelegate)
		{
			if (realDelegate.Target != null)
			{
				this.weakRef = new WeakRef(realDelegate.Target);
			}
			this.method = realDelegate.Method;
		}
		public static WeakMulticastDelegate Combine(WeakMulticastDelegate weakDelegate, Delegate realDelegate)
		{
			if (realDelegate == null)
			{
				return null;
			}
			if (weakDelegate != null)
			{
				return weakDelegate.Combine(realDelegate);
			}
			return new WeakMulticastDelegate(realDelegate);
		}
		public static WeakMulticastDelegate CombineUnique(WeakMulticastDelegate weakDelegate, Delegate realDelegate)
		{
			if (realDelegate == null)
			{
				return null;
			}
			if (weakDelegate != null)
			{
				return weakDelegate.CombineUnique(realDelegate);
			}
			return new WeakMulticastDelegate(realDelegate);
		}
		private WeakMulticastDelegate Combine(Delegate realDelegate)
		{
			this.prev = new WeakMulticastDelegate(realDelegate)
			{
				prev = this.prev
			};
			return this;
		}
		protected bool Equals(Delegate realDelegate)
		{
			if (this.weakRef == null)
			{
				return realDelegate.Target == null && this.method == realDelegate.Method;
			}
			return this.weakRef.Target == realDelegate.Target && this.method == realDelegate.Method;
		}
		private WeakMulticastDelegate CombineUnique(Delegate realDelegate)
		{
			bool flag = this.Equals(realDelegate);
			if (!flag && this.prev != null)
			{
				WeakMulticastDelegate weakMulticastDelegate = this.prev;
				while (!flag && weakMulticastDelegate != null)
				{
					if (weakMulticastDelegate.Equals(realDelegate))
					{
						flag = true;
					}
					weakMulticastDelegate = weakMulticastDelegate.prev;
				}
			}
			if (!flag)
			{
				return this.Combine(realDelegate);
			}
			return this;
		}
		public static WeakMulticastDelegate operator +(WeakMulticastDelegate d, Delegate realD)
		{
			return WeakMulticastDelegate.Combine(d, realD);
		}
		public static WeakMulticastDelegate operator -(WeakMulticastDelegate d, Delegate realD)
		{
			return WeakMulticastDelegate.Remove(d, realD);
		}
		public static WeakMulticastDelegate Remove(WeakMulticastDelegate weakDelegate, Delegate realDelegate)
		{
			if (realDelegate == null || weakDelegate == null)
			{
				return null;
			}
			return weakDelegate.Remove(realDelegate);
		}
		private WeakMulticastDelegate Remove(Delegate realDelegate)
		{
			if (this.Equals(realDelegate))
			{
				return this.prev;
			}
			WeakMulticastDelegate weakMulticastDelegate = this.prev;
			WeakMulticastDelegate weakMulticastDelegate2 = this;
			while (weakMulticastDelegate != null)
			{
				if (weakMulticastDelegate.Equals(realDelegate))
				{
					weakMulticastDelegate2.prev = weakMulticastDelegate.prev;
					weakMulticastDelegate.prev = null;
					break;
				}
				weakMulticastDelegate2 = weakMulticastDelegate;
				weakMulticastDelegate = weakMulticastDelegate.prev;
			}
			return this;
		}
		public void Invoke(object[] args)
		{
			for (WeakMulticastDelegate weakMulticastDelegate = this; weakMulticastDelegate != null; weakMulticastDelegate = weakMulticastDelegate.prev)
			{
				int tickCount = Environment.TickCount;
				if (weakMulticastDelegate.weakRef == null)
				{
					weakMulticastDelegate.method.Invoke(null, args);
				}
				else
				{
					if (weakMulticastDelegate.weakRef.IsAlive)
					{
						weakMulticastDelegate.method.Invoke(weakMulticastDelegate.weakRef.Target, args);
					}
				}
				if (Environment.TickCount - tickCount > 500 && WeakMulticastDelegate.log.IsWarnEnabled)
				{
					WeakMulticastDelegate.log.Warn(string.Concat(new object[]
					{
						"Invoke took ",
						Environment.TickCount - tickCount,
						"ms! ",
						weakMulticastDelegate.ToString()
					}));
				}
			}
		}
		public void InvokeSafe(object[] args)
		{
			for (WeakMulticastDelegate weakMulticastDelegate = this; weakMulticastDelegate != null; weakMulticastDelegate = weakMulticastDelegate.prev)
			{
				int tickCount = Environment.TickCount;
				try
				{
					if (weakMulticastDelegate.weakRef == null)
					{
						weakMulticastDelegate.method.Invoke(null, args);
					}
					else
					{
						if (weakMulticastDelegate.weakRef.IsAlive)
						{
							weakMulticastDelegate.method.Invoke(weakMulticastDelegate.weakRef.Target, args);
						}
					}
				}
				catch (Exception exception)
				{
					if (WeakMulticastDelegate.log.IsErrorEnabled)
					{
						WeakMulticastDelegate.log.Error("InvokeSafe", exception);
					}
				}
				if (Environment.TickCount - tickCount > 500 && WeakMulticastDelegate.log.IsWarnEnabled)
				{
					WeakMulticastDelegate.log.Warn(string.Concat(new object[]
					{
						"InvokeSafe took ",
						Environment.TickCount - tickCount,
						"ms! ",
						weakMulticastDelegate.ToString()
					}));
				}
			}
		}
		public string Dump()
		{
			StringBuilder stringBuilder = new StringBuilder();
			WeakMulticastDelegate weakMulticastDelegate = this;
			int num = 0;
			while (weakMulticastDelegate != null)
			{
				num++;
				if (weakMulticastDelegate.weakRef == null)
				{
					stringBuilder.Append("\t");
					stringBuilder.Append(num);
					stringBuilder.Append(") ");
					stringBuilder.Append(weakMulticastDelegate.method.Name);
					stringBuilder.Append(Environment.NewLine);
				}
				else
				{
					if (weakMulticastDelegate.weakRef.IsAlive)
					{
						stringBuilder.Append("\t");
						stringBuilder.Append(num);
						stringBuilder.Append(") ");
						stringBuilder.Append(weakMulticastDelegate.weakRef.Target);
						stringBuilder.Append(".");
						stringBuilder.Append(weakMulticastDelegate.method.Name);
						stringBuilder.Append(Environment.NewLine);
					}
					else
					{
						stringBuilder.Append("\t");
						stringBuilder.Append(num);
						stringBuilder.Append(") INVALID.");
						stringBuilder.Append(weakMulticastDelegate.method.Name);
						stringBuilder.Append(Environment.NewLine);
					}
				}
				weakMulticastDelegate = weakMulticastDelegate.prev;
			}
			return stringBuilder.ToString();
		}
		public override string ToString()
		{
			Type type = null;
			if (this.method != null)
			{
				type = this.method.DeclaringType;
			}
			object obj = null;
			if (this.weakRef != null && this.weakRef.IsAlive)
			{
				obj = this.weakRef.Target;
			}
			return new StringBuilder(64).Append("method: ").Append((type == null) ? "(null)" : type.FullName).Append('.').Append((this.method == null) ? "(null)" : this.method.Name).Append(" target: ").Append((obj == null) ? "null" : obj.ToString()).ToString();
		}
	}
}
