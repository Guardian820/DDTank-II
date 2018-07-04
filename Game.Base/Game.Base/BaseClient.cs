using Game.Base.Packets;
using log4net;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
namespace Game.Base
{
	public class BaseClient
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected Socket m_sock;
		protected byte[] m_readBuffer;
		protected int m_readBufEnd;
		private SocketAsyncEventArgs rc_event;
		private int m_isConnected;
		public bool IsClientPacketSended = true;
		public int numPacketProcces;
		protected byte[] m_sendBuffer;
		private bool m_encryted;
		private bool m_strict;
		private bool m_asyncPostSend;
		public StreamProcessor m_processor;
		public byte[] RECEIVE_KEY;
		public byte[] SEND_KEY;
		public event ClientEventHandle Connected;
		public event ClientEventHandle Disconnected;
		public Socket Socket
		{
			get
			{
				return this.m_sock;
			}
			set
			{
				this.m_sock = value;
			}
		}
		public byte[] PacketBuf
		{
			get
			{
				return this.m_readBuffer;
			}
		}
		public bool IsConnected
		{
			get
			{
				return this.m_isConnected == 1;
			}
		}
		public int PacketBufSize
		{
			get
			{
				return this.m_readBufEnd;
			}
			set
			{
				this.m_readBufEnd = value;
			}
		}
		public string TcpEndpoint
		{
			get
			{
				Socket sock = this.m_sock;
				if (sock != null && sock.Connected && sock.RemoteEndPoint != null)
				{
					return sock.RemoteEndPoint.ToString();
				}
				return "not connected";
			}
		}
		public byte[] SendBuffer
		{
			get
			{
				return this.m_sendBuffer;
			}
		}
		public bool Encryted
		{
			get
			{
				return this.m_encryted;
			}
			set
			{
				this.m_encryted = value;
			}
		}
		public bool Strict
		{
			get
			{
				return this.m_strict;
			}
			set
			{
				this.m_strict = value;
			}
		}
		public bool AsyncPostSend
		{
			get
			{
				return this.m_asyncPostSend;
			}
			set
			{
				this.m_asyncPostSend = value;
			}
		}
		public virtual void OnRecv(int num_bytes)
		{
			this.m_processor.ReceiveBytes(num_bytes);
		}
		public virtual void OnRecvPacket(GSPacketIn pkg)
		{
		}
		protected virtual void OnConnect()
		{
			if (Interlocked.Exchange(ref this.m_isConnected, 1) == 0 && this.Connected != null)
			{
				this.Connected(this);
			}
		}
		protected virtual void OnDisconnect()
		{
			if (this.Disconnected != null)
			{
				this.Disconnected(this);
			}
		}
		public BaseClient(byte[] readBuffer, byte[] sendBuffer)
		{
			this.m_readBuffer = readBuffer;
			this.m_sendBuffer = sendBuffer;
			this.m_readBufEnd = 0;
			this.rc_event = new SocketAsyncEventArgs();
			this.rc_event.Completed += new EventHandler<SocketAsyncEventArgs>(this.RecvEventCallback);
			this.m_processor = new StreamProcessor(this);
			this.m_encryted = false;
			this.m_strict = true;
		}
		public void SetFsm(int adder, int muliter)
		{
			this.m_processor.SetFsm(adder, muliter);
		}
		public void ReceiveAsync()
		{
			this.ReceiveAsyncImp(this.rc_event);
		}
		private void ReceiveAsyncImp(SocketAsyncEventArgs e)
		{
			if (this.m_sock != null && this.m_sock.Connected)
			{
				int num = this.m_readBuffer.Length;
				if (this.m_readBufEnd >= num)
				{
					if (BaseClient.log.IsErrorEnabled)
					{
						BaseClient.log.Error(this.TcpEndpoint + " disconnected because of buffer overflow!");
						BaseClient.log.Error(string.Concat(new object[]
						{
							"m_pBufEnd=",
							this.m_readBufEnd,
							"; buf size=",
							num
						}));
						BaseClient.log.Error(this.m_readBuffer);
					}
					this.Disconnect();
					return;
				}
				e.SetBuffer(this.m_readBuffer, this.m_readBufEnd, num - this.m_readBufEnd);
				if (!this.m_sock.ReceiveAsync(e))
				{
					this.RecvEventCallback(this.m_sock, e);
					return;
				}
			}
			else
			{
				this.Disconnect();
			}
		}
		private void RecvEventCallback(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				int bytesTransferred = e.BytesTransferred;
				if (bytesTransferred > 0)
				{
					this.OnRecv(bytesTransferred);
					this.ReceiveAsyncImp(e);
				}
				else
				{
					BaseClient.log.InfoFormat("Disconnecting client ({0}), received bytes={1}", this.TcpEndpoint, bytesTransferred);
					this.Disconnect();
				}
			}
			catch (Exception arg)
			{
				BaseClient.log.ErrorFormat("{0} RecvCallback:{1}", this.TcpEndpoint, arg);
				this.Disconnect();
			}
		}
		public virtual void SendTCP(GSPacketIn pkg)
		{
			this.m_processor.SendTCP(pkg);
		}
		public bool SendAsync(SocketAsyncEventArgs e)
		{
			int tickCount = Environment.TickCount;
			BaseClient.log.Debug(string.Format("Send To ({0}) {1} bytes", this.TcpEndpoint, e.Count));
			bool result = true;
			if (this.m_sock.Connected)
			{
				result = this.m_sock.SendAsync(e);
			}
			int num = Environment.TickCount - tickCount;
			if (num > 100)
			{
				BaseClient.log.WarnFormat("AsyncTcpSendCallback.BeginSend took {0}ms! (TCP to client: {1})", num, this.TcpEndpoint);
			}
			return result;
		}
		protected void CloseConnections()
		{
			if (this.m_sock != null)
			{
				try
				{
					this.m_sock.Shutdown(SocketShutdown.Both);
				}
				catch
				{
				}
				try
				{
					this.m_sock.Close();
				}
				catch
				{
				}
			}
		}
		public virtual bool Connect(Socket connectedSocket)
		{
			this.m_sock = connectedSocket;
			if (this.m_sock.Connected)
			{
				if (Interlocked.Exchange(ref this.m_isConnected, 1) == 0)
				{
					this.OnConnect();
				}
				return true;
			}
			return false;
		}
		public virtual void Disconnect()
		{
			try
			{
				int num = Interlocked.Exchange(ref this.m_isConnected, 0);
				if (num == 1)
				{
					this.CloseConnections();
					this.OnDisconnect();
					this.rc_event.Dispose();
					this.m_processor.Dispose();
				}
			}
			catch (Exception exception)
			{
				if (BaseClient.log.IsErrorEnabled)
				{
					BaseClient.log.Error("Exception", exception);
				}
			}
		}
		public virtual void DisplayMessage(string msg)
		{
		}
		public virtual void resetKey()
		{
			this.RECEIVE_KEY = StreamProcessor.cloneArrary(StreamProcessor.KEY, 8);
			this.SEND_KEY = StreamProcessor.cloneArrary(StreamProcessor.KEY, 8);
		}
		public virtual void setKey(byte[] data)
		{
			for (int i = 0; i < 8; i++)
			{
				this.RECEIVE_KEY[i] = data[i];
				this.SEND_KEY[i] = data[i];
			}
		}
	}
}
