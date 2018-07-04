using log4net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
namespace Game.Base
{
	public class BaseConnector : BaseClient
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static readonly int RECONNECT_INTERVAL = 10000;
		private SocketAsyncEventArgs e;
		private IPEndPoint _remoteEP;
		private bool _autoReconnect;
		private Timer timer;
		public IPEndPoint RemoteEP
		{
			get
			{
				return this._remoteEP;
			}
		}
		public bool Connect()
		{
			try
			{
				this.m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this.m_readBufEnd = 0;
				this.m_sock.Connect(this._remoteEP);
				BaseConnector.log.InfoFormat("Connected to {0}", this._remoteEP);
			}
			catch
			{
				BaseConnector.log.ErrorFormat("Connect {0} failed!", this._remoteEP);
				this.m_sock = null;
				return false;
			}
			this.OnConnect();
			base.ReceiveAsync();
			return true;
		}
		private void TryReconnect()
		{
			if (this.Connect())
			{
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
				base.ReceiveAsync();
				return;
			}
			BaseConnector.log.ErrorFormat("Reconnect {0} failed:", this._remoteEP);
			BaseConnector.log.ErrorFormat("Retry after {0} ms!", BaseConnector.RECONNECT_INTERVAL);
			if (this.timer == null)
			{
				this.timer = new Timer(new TimerCallback(BaseConnector.RetryTimerCallBack), this, -1, -1);
			}
			this.timer.Change(BaseConnector.RECONNECT_INTERVAL, -1);
		}
		private static void RetryTimerCallBack(object target)
		{
			BaseConnector baseConnector = target as BaseConnector;
			if (baseConnector != null)
			{
				baseConnector.TryReconnect();
				return;
			}
			BaseConnector.log.Error("BaseConnector retryconnect timer return NULL!");
		}
		public BaseConnector(string ip, int port, bool autoReconnect, byte[] readBuffer, byte[] sendBuffer) : base(readBuffer, sendBuffer)
		{
			this._remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
			this._autoReconnect = autoReconnect;
			this.e = new SocketAsyncEventArgs();
		}
	}
}
