using Game.Base;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using System;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Server
{
	public class GameClient : BaseClient
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static readonly byte[] POLICY = Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><!DOCTYPE cross-domain-policy SYSTEM \"http://www.adobe.com/xml/dtds/cross-domain-policy.dtd\"><cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\" /></cross-domain-policy>\0");
		protected GamePlayer m_player;
		public int Version;
		protected long m_pingTime = DateTime.Now.Ticks;
		protected IPacketLib m_packetLib;
		protected PacketProcessor m_packetProcessor;
		protected GameServer _srvr;
		public int beadRequestBtn1;
		public int beadRequestBtn2;
		public int beadRequestBtn3;
		public int Lottery;
		public string tempData;
		public GamePlayer Player
		{
			get
			{
				return this.m_player;
			}
			set
			{
				GamePlayer gamePlayer = Interlocked.Exchange<GamePlayer>(ref this.m_player, value);
				if (gamePlayer != null)
				{
					gamePlayer.Quit();
				}
			}
		}
		public long PingTime
		{
			get
			{
				return this.m_pingTime;
			}
		}
		public IPacketLib Out
		{
			get
			{
				return this.m_packetLib;
			}
			set
			{
				this.m_packetLib = value;
			}
		}
		public PacketProcessor PacketProcessor
		{
			get
			{
				return this.m_packetProcessor;
			}
		}
		public GameServer Server
		{
			get
			{
				return this._srvr;
			}
		}
		public override void OnRecv(int num_bytes)
		{
			if (this.m_packetProcessor != null)
			{
				base.OnRecv(num_bytes);
			}
			else
			{
				if (this.m_readBuffer[0] == 60)
				{
					this.m_sock.Send(GameClient.POLICY);
				}
				else
				{
					base.OnRecv(num_bytes);
				}
			}
			this.m_pingTime = DateTime.Now.Ticks;
		}
		public override void OnRecvPacket(GSPacketIn pkg)
		{
			if (this.m_packetProcessor == null)
			{
				this.m_packetLib = AbstractPacketLib.CreatePacketLibForVersion(1, this);
				this.m_packetProcessor = new PacketProcessor(this);
			}
			if (this.m_player != null)
			{
				pkg.ClientID = this.m_player.PlayerId;
				pkg.WriteHeader();
			}
			this.m_packetProcessor.HandlePacket(pkg);
		}
		public override void SendTCP(GSPacketIn pkg)
		{
			base.SendTCP(pkg);
		}
		public override void DisplayMessage(string msg)
		{
			base.DisplayMessage(msg);
		}
		protected override void OnConnect()
		{
			base.OnConnect();
			this.m_pingTime = DateTime.Now.Ticks;
		}
		public override void Disconnect()
		{
			base.Disconnect();
		}
		private void SaveBag(GamePlayer player)
		{
			player.ClearCaddyBag();
			player.ClearStoreBag();
		}
		protected override void OnDisconnect()
		{
			try
			{
				GamePlayer gamePlayer = Interlocked.Exchange<GamePlayer>(ref this.m_player, null);
				if (gamePlayer != null)
				{
					gamePlayer.FightBag.ClearBag();
					this.SaveBag(gamePlayer);
					LoginMgr.ClearLoginPlayer(gamePlayer.PlayerCharacter.ID, this);
					gamePlayer.Quit();
				}
				byte[] buf = this.m_sendBuffer;
				this.m_sendBuffer = null;
				this._srvr.ReleasePacketBuffer(buf);
				buf = this.m_readBuffer;
				this.m_readBuffer = null;
				this._srvr.ReleasePacketBuffer(buf);
				base.OnDisconnect();
			}
			catch (Exception exception)
			{
				if (GameClient.log.IsErrorEnabled)
				{
					GameClient.log.Error("OnDisconnect", exception);
				}
			}
		}
		public void SavePlayer()
		{
			try
			{
				if (this.m_player != null)
				{
					this.m_player.SaveIntoDatabase();
				}
			}
			catch (Exception exception)
			{
				if (GameClient.log.IsErrorEnabled)
				{
					GameClient.log.Error("SavePlayer", exception);
				}
			}
		}
		public GameClient(GameServer svr, byte[] read, byte[] send) : base(read, send)
		{
			this.Lottery = -1;
			this.tempData = string.Empty;
			this.m_pingTime = DateTime.Now.Ticks;
			this._srvr = svr;
			this.m_player = null;
			base.Encryted = true;
			base.AsyncPostSend = true;
		}
		public override string ToString()
		{
			return new StringBuilder(128).Append(" pakLib:").Append((this.Out == null) ? "(null)" : this.Out.GetType().FullName).Append(" IP:").Append(base.TcpEndpoint).Append(" char:").Append((this.Player == null) ? "null" : this.Player.PlayerCharacter.NickName).ToString();
		}
	}
}
