using log4net;
using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
namespace Game.Base.Events
{
	public class RoadEventHandlerCollection
	{
		protected const int TIMEOUT = 3000;
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected readonly ReaderWriterLock m_lock;
		protected readonly HybridDictionary m_events;
		public RoadEventHandlerCollection()
		{
			this.m_lock = new ReaderWriterLock();
			this.m_events = new HybridDictionary();
		}
		public void AddHandler(RoadEvent e, RoadEventHandler del)
		{
			try
			{
				this.m_lock.AcquireWriterLock(3000);
				try
				{
					WeakMulticastDelegate weakMulticastDelegate = (WeakMulticastDelegate)this.m_events[e];
					if (weakMulticastDelegate == null)
					{
						this.m_events[e] = new WeakMulticastDelegate(del);
					}
					else
					{
						this.m_events[e] = WeakMulticastDelegate.Combine(weakMulticastDelegate, del);
					}
				}
				finally
				{
					this.m_lock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException exception)
			{
				if (RoadEventHandlerCollection.log.IsErrorEnabled)
				{
					RoadEventHandlerCollection.log.Error("Failed to add event handler!", exception);
				}
			}
		}
		public void AddHandlerUnique(RoadEvent e, RoadEventHandler del)
		{
			try
			{
				this.m_lock.AcquireWriterLock(3000);
				try
				{
					WeakMulticastDelegate weakMulticastDelegate = (WeakMulticastDelegate)this.m_events[e];
					if (weakMulticastDelegate == null)
					{
						this.m_events[e] = new WeakMulticastDelegate(del);
					}
					else
					{
						this.m_events[e] = WeakMulticastDelegate.CombineUnique(weakMulticastDelegate, del);
					}
				}
				finally
				{
					this.m_lock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException exception)
			{
				if (RoadEventHandlerCollection.log.IsErrorEnabled)
				{
					RoadEventHandlerCollection.log.Error("Failed to add event handler!", exception);
				}
			}
		}
		public void RemoveHandler(RoadEvent e, RoadEventHandler del)
		{
			try
			{
				this.m_lock.AcquireWriterLock(3000);
				try
				{
					WeakMulticastDelegate weakMulticastDelegate = (WeakMulticastDelegate)this.m_events[e];
					if (weakMulticastDelegate != null)
					{
						weakMulticastDelegate = WeakMulticastDelegate.Remove(weakMulticastDelegate, del);
						if (weakMulticastDelegate == null)
						{
							this.m_events.Remove(e);
						}
						else
						{
							this.m_events[e] = weakMulticastDelegate;
						}
					}
				}
				finally
				{
					this.m_lock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException exception)
			{
				if (RoadEventHandlerCollection.log.IsErrorEnabled)
				{
					RoadEventHandlerCollection.log.Error("Failed to remove event handler!", exception);
				}
			}
		}
		public void RemoveAllHandlers(RoadEvent e)
		{
			try
			{
				this.m_lock.AcquireWriterLock(3000);
				try
				{
					this.m_events.Remove(e);
				}
				finally
				{
					this.m_lock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException exception)
			{
				if (RoadEventHandlerCollection.log.IsErrorEnabled)
				{
					RoadEventHandlerCollection.log.Error("Failed to remove event handlers!", exception);
				}
			}
		}
		public void RemoveAllHandlers()
		{
			try
			{
				this.m_lock.AcquireWriterLock(3000);
				try
				{
					this.m_events.Clear();
				}
				finally
				{
					this.m_lock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException exception)
			{
				if (RoadEventHandlerCollection.log.IsErrorEnabled)
				{
					RoadEventHandlerCollection.log.Error("Failed to remove all event handlers!", exception);
				}
			}
		}
		public void Notify(RoadEvent e)
		{
			this.Notify(e, null, null);
		}
		public void Notify(RoadEvent e, object sender)
		{
			this.Notify(e, sender, null);
		}
		public void Notify(RoadEvent e, EventArgs args)
		{
			this.Notify(e, null, args);
		}
		public void Notify(RoadEvent e, object sender, EventArgs eArgs)
		{
			try
			{
				this.m_lock.AcquireReaderLock(3000);
				WeakMulticastDelegate weakMulticastDelegate;
				try
				{
					weakMulticastDelegate = (WeakMulticastDelegate)this.m_events[e];
				}
				finally
				{
					this.m_lock.ReleaseReaderLock();
				}
				if (weakMulticastDelegate != null)
				{
					weakMulticastDelegate.InvokeSafe(new object[]
					{
						e,
						sender,
						eArgs
					});
				}
			}
			catch (ApplicationException exception)
			{
				if (RoadEventHandlerCollection.log.IsErrorEnabled)
				{
					RoadEventHandlerCollection.log.Error("Failed to notify event handler!", exception);
				}
			}
		}
	}
}
