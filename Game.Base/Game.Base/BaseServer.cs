using log4net;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
namespace Game.Base
{
	public class BaseServer
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static readonly int SEND_BUFF_SIZE = 30720;
		protected readonly HybridDictionary _clients = new HybridDictionary();
		protected Socket _linstener;
		protected SocketAsyncEventArgs ac_event;
		public int ClientCount
		{
			get
			{
				return this._clients.Count;
			}
		}
		public BaseServer()
		{
			this.ac_event = new SocketAsyncEventArgs();
			this.ac_event.Completed += new EventHandler<SocketAsyncEventArgs>(this.AcceptAsyncCompleted);
		}
		private void AcceptAsync()
		{
			try
			{
				if (this._linstener != null)
				{
					SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
					socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(this.AcceptAsyncCompleted);
					this._linstener.AcceptAsync(socketAsyncEventArgs);
				}
			}
			catch (Exception exception)
			{
				BaseServer.log.Error("AcceptAsync is error!", exception);
			}
		}
		private void AcceptAsyncCompleted(object sender, SocketAsyncEventArgs e)
		{
			Socket socket = null;
			try
			{
				socket = e.AcceptSocket;
				socket.SendBufferSize = BaseServer.SEND_BUFF_SIZE;
				BaseClient newClient = this.GetNewClient();
				try
				{
					if (BaseServer.log.IsInfoEnabled)
					{
						string str = socket.Connected ? socket.RemoteEndPoint.ToString() : "socket disconnected";
						BaseServer.log.Info("Incoming connection from " + str);
					}
					object syncRoot;
					Monitor.Enter(syncRoot = this._clients.SyncRoot);
					try
					{
						this._clients.Add(newClient, newClient);
						newClient.Disconnected += new ClientEventHandle(this.client_Disconnected);
					}
					finally
					{
						Monitor.Exit(syncRoot);
					}
					newClient.Connect(socket);
					newClient.ReceiveAsync();
				}
				catch (Exception arg)
				{
					BaseServer.log.ErrorFormat("create client failed:{0}", arg);
					newClient.Disconnect();
				}
			}
			catch
			{
				if (socket != null)
				{
					try
					{
						socket.Close();
					}
					catch
					{
					}
				}
			}
			finally
			{
				e.Dispose();
				this.AcceptAsync();
			}
		}
		private void client_Disconnected(BaseClient client)
		{
			client.Disconnected -= new ClientEventHandle(this.client_Disconnected);
			this.RemoveClient(client);
		}
		protected virtual BaseClient GetNewClient()
		{
			return new BaseClient(new byte[30720], new byte[30720]);
		}
		public virtual bool InitSocket(IPAddress ip, int port)
		{
			try
			{
				this._linstener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this._linstener.Bind(new IPEndPoint(ip, port));
			}
			catch (Exception exception)
			{
				BaseServer.log.Error("InitSocket", exception);
				return false;
			}
			return true;
		}
		public virtual bool Start()
		{
			if (this._linstener == null)
			{
				return false;
			}
			try
			{
				this._linstener.Listen(100);
				this.AcceptAsync();
				if (BaseServer.log.IsDebugEnabled)
				{
					BaseServer.log.Debug("Server is now listening to incoming connections!");
				}
			}
			catch (Exception exception)
			{
				if (BaseServer.log.IsErrorEnabled)
				{
					BaseServer.log.Error("Start", exception);
				}
				if (this._linstener != null)
				{
					this._linstener.Close();
				}
				return false;
			}
			return true;
		}
		public virtual void Stop()
		{
			BaseServer.log.Debug("Stopping server! - Entering method");
			try
			{
				if (this._linstener != null)
				{
					Socket linstener = this._linstener;
					this._linstener = null;
					linstener.Close();
					BaseServer.log.Debug("Server is no longer listening for incoming connections!");
				}
			}
			catch (Exception exception)
			{
				BaseServer.log.Error("Stop", exception);
			}
			if (this._clients != null)
			{
				object syncRoot;
				Monitor.Enter(syncRoot = this._clients.SyncRoot);
				try
				{
					BaseClient[] array = new BaseClient[this._clients.Keys.Count];
					this._clients.Keys.CopyTo(array, 0);
					BaseClient[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						BaseClient baseClient = array2[i];
						baseClient.Disconnect();
					}
					BaseServer.log.Debug("Stopping server! - Cleaning up client list!");
				}
				catch (Exception exception2)
				{
					BaseServer.log.Error("Stop", exception2);
				}
				finally
				{
					Monitor.Exit(syncRoot);
				}
			}
			BaseServer.log.Debug("Stopping server! - End of method!");
		}
		public virtual void RemoveClient(BaseClient client)
		{
			object syncRoot;
			Monitor.Enter(syncRoot = this._clients.SyncRoot);
			try
			{
				this._clients.Remove(client);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}
		public BaseClient[] GetAllClients()
		{
			object syncRoot;
			Monitor.Enter(syncRoot = this._clients.SyncRoot);
			BaseClient[] result;
			try
			{
				BaseClient[] array = new BaseClient[this._clients.Count];
				this._clients.Keys.CopyTo(array, 0);
				result = array;
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			return result;
		}
		public void Dispose()
		{
			this.ac_event.Dispose();
		}
	}
}
