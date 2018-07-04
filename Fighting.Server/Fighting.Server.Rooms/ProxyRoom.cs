using Game.Base.Packets;
using Game.Logic;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Fighting.Server.Rooms
{
	public class ProxyRoom
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private List<IGamePlayer> m_players;
		private int m_roomId;
		private int m_orientRoomId;
		private ServerClient m_client;
		public bool IsPlaying;
		public eGameType GameType;
		public int GuildId;
		public string GuildName;
		public int AvgLevel;
		public int FightPower;
		private BaseGame m_game;
		public int RoomId
		{
			get
			{
				return this.m_roomId;
			}
		}
		public ServerClient Client
		{
			get
			{
				return this.m_client;
			}
		}
		public int PlayerCount
		{
			get
			{
				return this.m_players.Count;
			}
		}
		public BaseGame Game
		{
			get
			{
				return this.m_game;
			}
		}
		public ProxyRoom(int roomId, int orientRoomId, IGamePlayer[] players, ServerClient client)
		{
			this.m_roomId = roomId;
			this.m_orientRoomId = orientRoomId;
			this.m_players = new List<IGamePlayer>();
			this.m_players.AddRange(players);
			this.m_client = client;
		}
		public void SendToAll(GSPacketIn pkg)
		{
			this.SendToAll(pkg, null);
		}
		public void SendToAll(GSPacketIn pkg, IGamePlayer except)
		{
			this.m_client.SendToRoom(this.m_orientRoomId, pkg, except);
		}
		public List<IGamePlayer> GetPlayers()
		{
			List<IGamePlayer> list = new List<IGamePlayer>();
			List<IGamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				list.AddRange(this.m_players);
			}
			finally
			{
				Monitor.Exit(players);
			}
			return list;
		}
		public bool RemovePlayer(IGamePlayer player)
		{
			bool result = false;
			List<IGamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				if (this.m_players.Remove(player))
				{
					result = true;
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
			if (this.PlayerCount == 0)
			{
				ProxyRoomMgr.RemoveRoom(this);
			}
			return result;
		}
		public void StartGame(BaseGame game)
		{
			this.IsPlaying = true;
			this.m_game = game;
			game.GameStopped += new GameEventHandle(this.game_GameStopped);
			this.m_client.SendStartGame(this.m_orientRoomId, game);
		}
		private void game_GameStopped(AbstractGame game)
		{
			this.m_game.GameStopped -= new GameEventHandle(this.game_GameStopped);
			this.IsPlaying = false;
			this.m_client.SendStopGame(this.m_orientRoomId, this.m_game.Id);
		}
		public void Dispose()
		{
			this.m_client.RemoveRoom(this.m_orientRoomId, this);
		}
		public override string ToString()
		{
			return string.Format("RoomId:{0} OriendId:{1} PlayerCount:{2},IsPlaying:{3},GuildId:{4},GuildName:{5}", new object[]
			{
				this.m_roomId,
				this.m_orientRoomId,
				this.m_players.Count,
				this.IsPlaying,
				this.GuildId,
				this.GuildName
			});
		}
	}
}
