using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
using System.Threading;
namespace Game.Logic
{
	public class AbstractGame
	{
		private int m_id;
		protected eRoomType m_roomType;
		protected eGameType m_gameType;
		protected eMapType m_mapType;
		protected int m_timeType;
		private int m_disposed;
		public event GameEventHandle GameStarted;
		public event GameEventHandle GameStopped;
		public int Id
		{
			get
			{
				return this.m_id;
			}
		}
		public eRoomType RoomType
		{
			get
			{
				return this.m_roomType;
			}
		}
		public eGameType GameType
		{
			get
			{
				return this.m_gameType;
			}
		}
		public int TimeType
		{
			get
			{
				return this.m_timeType;
			}
		}
		public AbstractGame(int id, eRoomType roomType, eGameType gameType, int timeType)
		{
			this.m_id = id;
			this.m_roomType = roomType;
			this.m_gameType = gameType;
			this.m_timeType = timeType;
			switch (this.m_roomType)
			{
			case eRoomType.Match:
				this.m_mapType = eMapType.PairUp;
				return;

			case eRoomType.Freedom:
				this.m_mapType = eMapType.Normal;
				return;

			default:
				this.m_mapType = eMapType.Normal;
				return;
			}
		}
		public virtual void Start()
		{
			this.OnGameStarted();
		}
		public virtual void Stop()
		{
			this.OnGameStopped();
		}
		public virtual bool CanAddPlayer()
		{
			return false;
		}
		public virtual void Pause(int time)
		{
		}
		public virtual void Resume()
		{
		}
		public virtual void MissionStart(IGamePlayer host)
		{
		}
		public virtual void ProcessData(GSPacketIn pkg)
		{
		}
		public virtual Player AddPlayer(IGamePlayer player)
		{
			return null;
		}
		public virtual Player RemovePlayer(IGamePlayer player, bool IsKick)
		{
			return null;
		}
		public void Dispose()
		{
			if (Interlocked.Exchange(ref this.m_disposed, 1) == 0)
			{
				this.Dispose(true);
			}
		}
		protected virtual void Dispose(bool disposing)
		{
		}
		protected void OnGameStarted()
		{
			if (this.GameStarted != null)
			{
				this.GameStarted(this);
			}
		}
		protected void OnGameStopped()
		{
			if (this.GameStopped != null)
			{
				this.GameStopped(this);
			}
		}
	}
}
