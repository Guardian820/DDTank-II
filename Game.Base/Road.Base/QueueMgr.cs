using System;
using System.Collections.Generic;
using System.Threading;
namespace Road.Base
{
	public class QueueMgr<T>
	{
		public delegate void Exectue(T msg);
		public delegate void AsynExecute(T info);
		private Queue<T> _queue;
		private bool _executing;
		private QueueMgr<T>.Exectue _exectue;
		private object _syncRoot;
		private QueueMgr<T>.AsynExecute _asynExecute;
		private AsyncCallback _asynCallBack;
		public QueueMgr(QueueMgr<T>.Exectue exec)
		{
			this._queue = new Queue<T>();
			this._executing = false;
			this._exectue = exec;
			this._syncRoot = new object();
			this._asynExecute = new QueueMgr<T>.AsynExecute(this.Exectuing);
			this._asynCallBack = new AsyncCallback(this.AsynCallBack);
		}
		public void ExecuteQueue(T info)
		{
			object syncRoot;
			Monitor.Enter(syncRoot = this._syncRoot);
			try
			{
				if (this._executing)
				{
					this._queue.Enqueue(info);
					return;
				}
				this._executing = true;
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			this._asynExecute.BeginInvoke(info, this._asynCallBack, this._asynExecute);
		}
		private void AsynCallBack(IAsyncResult ar)
		{
			((QueueMgr<T>.AsynExecute)ar.AsyncState).EndInvoke(ar);
		}
		private void Exectuing(T info)
		{
			this._exectue(info);
			object syncRoot;
			Monitor.Enter(syncRoot = this._syncRoot);
			T info2;
			try
			{
				if (this._queue.Count <= 0)
				{
					this._executing = false;
					return;
				}
				info2 = this._queue.Peek();
				this._queue.Dequeue();
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			this.Exectuing(info2);
		}
	}
}
