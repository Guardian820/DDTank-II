using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Rooms
{
	public class BaseRoom
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private GamePlayer[] m_places;
		private int[] m_placesState;
		private byte[] m_playerState;
		private int m_playerCount;
		private int m_placesCount = 10;
		private bool m_isUsing;
		private GamePlayer m_host;
		public bool IsPlaying;
		public int RoomId;
		public int maxViewerCnt = 2;
		private int m_viewerCnt;
		public int GameStyle;
		public string Name;
		public string Pic;
		public string Password;
		public bool isCrosszone;
		public bool isWithinLeageTime;
		public bool isOpenBoss;
		public eRoomType RoomType;
		public eGameType GameType;
		public eHardLevel HardLevel;
		public int LevelLimits;
		public byte TimeMode;
		public int MapId;
		public string m_roundName;
		private int m_avgLevel;
		private AbstractGame m_game;
		public BattleServer BattleServer;
		public int viewerCnt
		{
			get
			{
				return this.m_viewerCnt;
			}
		}
		public GamePlayer Host
		{
			get
			{
				return this.m_host;
			}
		}
		public byte[] PlayerState
		{
			get
			{
				return this.m_playerState;
			}
			set
			{
				this.m_playerState = value;
			}
		}
		public int PlayerCount
		{
			get
			{
				return this.m_playerCount;
			}
		}
		public int PlacesCount
		{
			get
			{
				return this.m_placesCount;
			}
		}
		public int GuildId
		{
			get
			{
				return this.m_host.PlayerCharacter.ConsortiaID;
			}
		}
		public bool IsUsing
		{
			get
			{
				return this.m_isUsing;
			}
		}
		public string RoundName
		{
			get
			{
				return this.m_roundName;
			}
			set
			{
				this.m_roundName = value;
			}
		}
		public bool NeedPassword
		{
			get
			{
				return !string.IsNullOrEmpty(this.Password);
			}
		}
		public bool IsEmpty
		{
			get
			{
				return this.m_playerCount == 0;
			}
		}
		public int AvgLevel
		{
			get
			{
				return this.m_avgLevel;
			}
		}
		public AbstractGame Game
		{
			get
			{
				return this.m_game;
			}
		}
		public BaseRoom(int roomId)
		{
			this.RoomId = roomId;
			this.m_places = new GamePlayer[10];
			this.m_placesState = new int[10];
			this.m_playerState = new byte[8];
			this.Reset();
		}
		public void Start()
		{
			if (!this.m_isUsing)
			{
				this.m_isUsing = true;
				this.Reset();
			}
		}
		public void Stop()
		{
			if (this.m_isUsing)
			{
				this.m_isUsing = false;
				if (this.m_game != null)
				{
					this.m_game.GameStopped -= new GameEventHandle(this.m_game_GameStopped);
					this.m_game = null;
					this.IsPlaying = false;
				}
				RoomMgr.WaitingRoom.SendUpdateRoom(this);
			}
		}
		private void Reset()
		{
			for (int i = 0; i < 10; i++)
			{
				this.m_places[i] = null;
				this.m_placesState[i] = -1;
				if (i < 8)
				{
					this.m_playerState[i] = 0;
				}
			}
			this.m_host = null;
			this.IsPlaying = false;
			this.m_placesCount = 10;
			this.m_playerCount = 0;
			this.HardLevel = eHardLevel.Simple;
		}
		public bool CanStart()
		{
			if (this.RoomType == eRoomType.Freedom)
			{
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < 8; i++)
				{
					if (i % 2 == 0)
					{
						if (this.m_playerState[i] > 0)
						{
							num++;
						}
					}
					else
					{
						if (this.m_playerState[i] > 0)
						{
							num2++;
						}
					}
				}
				return num > 0 && num2 > 0;
			}
			int num3 = 0;
			for (int j = 0; j < 8; j++)
			{
				if (this.m_playerState[j] > 0)
				{
					num3++;
				}
			}
			return num3 == this.m_playerCount;
		}
		public bool CanAddPlayer()
		{
			return this.m_playerCount < this.m_placesCount;
		}
		public List<GamePlayer> GetPlayers()
		{
			List<GamePlayer> list = new List<GamePlayer>();
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				for (int i = 0; i < 10; i++)
				{
					if (this.m_places[i] != null)
					{
						list.Add(this.m_places[i]);
					}
				}
			}
			finally
			{
				Monitor.Exit(places);
			}
			return list;
		}
		public void SetHost(GamePlayer player)
		{
			if (this.m_host == player)
			{
				return;
			}
			if (this.m_host != null)
			{
				this.UpdatePlayerState(player, 0, false);
			}
			this.m_host = player;
			this.UpdatePlayerState(player, 2, true);
		}
		public void UpdateRoom(string name, string pwd, eRoomType roomType, byte timeMode, int mapId)
		{
			this.Name = name;
			this.Password = pwd;
			this.RoomType = roomType;
			this.TimeMode = timeMode;
			this.MapId = mapId;
			this.UpdateRoomGameType();
			if (roomType == eRoomType.Freedom)
			{
				this.m_placesCount = 8;
				return;
			}
			this.m_placesCount = 4;
		}
		public void UpdateRoomGameType()
		{
			eRoomType roomType = this.RoomType;
			switch (roomType)
			{
			case eRoomType.Match:
			case eRoomType.Freedom:
				this.GameType = eGameType.Free;
				return;

			case eRoomType.Exploration:
			case (eRoomType)3:
				goto IL_77;

			case eRoomType.Dungeon:
				break;

			case eRoomType.FightLib:
				this.GameType = eGameType.FightLib;
				return;

			default:
				switch (roomType)
				{
				case eRoomType.Freshman:
					this.GameType = eGameType.Freshman;
					return;

				case eRoomType.AcademyDungeon:
					break;

				default:
					switch (roomType)
					{
					case eRoomType.Lanbyrinth:
						break;

					case eRoomType.Encounter:
						goto IL_77;

					case eRoomType.ConsortiaBoss:
						this.GameType = eGameType.ConsortiaBoss;
						return;

					default:
						goto IL_77;
					}
					break;
				}
				break;
			}
			this.GameType = eGameType.Dungeon;
			return;
			IL_77:
			this.GameType = eGameType.ALL;
		}
		public void UpdatePlayerState(GamePlayer player, byte state, bool sendToClient)
		{
			this.m_playerState[player.CurrentRoomIndex] = state;
			if (sendToClient)
			{
				this.SendPlayerState();
			}
		}
		public void UpdateAvgLevel()
		{
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				if (this.m_places[i] != null)
				{
					num += this.m_places[i].PlayerCharacter.Grade;
				}
			}
			this.m_avgLevel = num / this.m_playerCount;
		}
		public void SendToAll(GSPacketIn pkg)
		{
			this.SendToAll(pkg, null);
		}
		public void SendToAll(GSPacketIn pkg, GamePlayer except)
		{
			GamePlayer[] array = null;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				array = (GamePlayer[])this.m_places.Clone();
			}
			finally
			{
				Monitor.Exit(places);
			}
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null && array[i] != except)
					{
						array[i].Out.SendTCP(pkg);
					}
				}
			}
		}
		public void SendToTeam(GSPacketIn pkg, int team)
		{
			this.SendToTeam(pkg, team, null);
		}
		public void SendToTeam(GSPacketIn pkg, int team, GamePlayer except)
		{
			GamePlayer[] array = null;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				array = (GamePlayer[])this.m_places.Clone();
			}
			finally
			{
				Monitor.Exit(places);
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].CurrentRoomTeam == team && array[i] != except)
				{
					array[i].Out.SendTCP(pkg);
				}
			}
		}
		public void SendToHost(GSPacketIn pkg)
		{
			GamePlayer[] array = null;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				array = (GamePlayer[])this.m_places.Clone();
			}
			finally
			{
				Monitor.Exit(places);
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i] == this.Host)
				{
					array[i].Out.SendTCP(pkg);
				}
			}
		}
		public void SendPlayerState()
		{
			GSPacketIn pkg = this.m_host.Out.SendRoomUpdatePlayerStates(this.m_playerState);
			this.SendToAll(pkg, this.m_host);
		}
		public void SendPlaceState()
		{
			if (this.m_host != null)
			{
				GSPacketIn pkg = this.m_host.Out.SendRoomUpdatePlacesStates(this.m_placesState);
				this.SendToAll(pkg, this.m_host);
			}
		}
		public void SendCancelPickUp()
		{
			if (this.m_host != null)
			{
				GSPacketIn pkg = this.m_host.Out.SendRoomPairUpCancel(this);
				this.SendToAll(pkg, this.m_host);
			}
		}
		public void SendStartPickUp()
		{
			if (this.m_host != null)
			{
				GSPacketIn pkg = this.m_host.Out.SendRoomPairUpStart(this);
				this.SendToAll(pkg, this.m_host);
			}
		}
		public void SendMessage(eMessageType type, string msg)
		{
			if (this.m_host != null)
			{
				GSPacketIn pkg = this.m_host.Out.SendMessage(type, msg);
				this.SendToAll(pkg, this.m_host);
			}
		}
		public void SendRoomSetupChange(BaseRoom room)
		{
			if (this.m_host != null)
			{
				GSPacketIn pkg = this.m_host.Out.SendRoomChange(room);
				this.SendToAll(pkg);
			}
		}
		public bool UpdatePosUnsafe(int pos, bool isOpened, int place, int placeView)
		{
			if (pos < 0 || pos > 9)
			{
				return false;
			}
			if (this.m_placesState[pos] != place)
			{
				if (this.m_places[pos] != null)
				{
					this.RemovePlayerUnsafe(this.m_places[pos]);
				}
				this.m_placesState[pos] = place;
				this.SendPlaceState();
				if (place == -1)
				{
					if (pos < 8)
					{
						this.m_placesCount++;
					}
				}
				else
				{
					if (pos < 8)
					{
						this.m_placesCount--;
					}
				}
				return true;
			}
			return false;
		}
		public bool IsAllSameGuild()
		{
			int guildId = this.GuildId;
			if (guildId == 0)
			{
				return false;
			}
			List<GamePlayer> players = this.GetPlayers();
			if (players.Count >= 2)
			{
				foreach (GamePlayer current in players)
				{
					if (current.PlayerCharacter.ConsortiaID != guildId)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		public void UpdateGameStyle()
		{
			if (this.m_host != null && this.RoomType == eRoomType.Match)
			{
				if (this.IsAllSameGuild())
				{
					this.GameStyle = 1;
					this.GameType = eGameType.Guild;
				}
				else
				{
					this.GameStyle = 0;
					this.GameType = eGameType.Free;
				}
				GSPacketIn pkg = this.m_host.Out.SendRoomType(this.m_host, this);
				this.SendToAll(pkg);
			}
		}
		public bool AddPlayerUnsafe(GamePlayer player)
		{
			int num = -1;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				for (int i = 0; i < 10; i++)
				{
					if (this.m_places[i] == null && this.m_placesState[i] == -1)
					{
						this.m_places[i] = player;
						this.m_placesState[i] = player.PlayerId;
						this.m_playerCount++;
						num = i;
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(places);
			}
			if (num != -1)
			{
				this.UpdatePosUnsafe(8, false, 0, -100);
				this.UpdatePosUnsafe(9, false, 0, -100);
				player.CurrentRoom = this;
				player.CurrentRoomIndex = num;
				if (this.RoomType == eRoomType.Freedom)
				{
					player.CurrentRoomTeam = num % 2 + 1;
				}
				else
				{
					player.CurrentRoomTeam = 1;
				}
				GSPacketIn pkg = player.Out.SendRoomPlayerAdd(player);
				this.SendToAll(pkg, player);
				GSPacketIn pkg2 = player.Out.SendBufferList(player, player.BufferList.GetAllBuffer());
				this.SendToAll(pkg2, player);
				List<GamePlayer> players = this.GetPlayers();
				foreach (GamePlayer current in players)
				{
					if (current != player)
					{
						player.Out.SendRoomPlayerAdd(current);
						player.Out.SendBufferList(current, current.BufferList.GetAllBuffer());
					}
				}
				if (this.m_host == null)
				{
					this.m_host = player;
					this.UpdatePlayerState(player, 2, true);
				}
				else
				{
					this.UpdatePlayerState(player, 0, true);
				}
				this.SendPlaceState();
				this.UpdateGameStyle();
			}
			return num != -1;
		}
		public bool RemovePlayerUnsafe(GamePlayer player)
		{
			return this.RemovePlayerUnsafe(player, false);
		}
		public bool RemovePlayerUnsafe(GamePlayer player, bool isKick)
		{
			int num = -1;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				for (int i = 0; i < 10; i++)
				{
					if (this.m_places[i] == player)
					{
						this.m_places[i] = null;
						this.m_playerState[i] = 0;
						this.m_placesState[i] = -1;
						this.m_playerCount--;
						num = i;
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(places);
			}
			if (num != -1)
			{
				this.UpdatePosUnsafe(num, false, -1, -100);
				player.CurrentRoom = null;
				player.TempBag.ClearBag();
				GSPacketIn pkg = player.Out.SendRoomPlayerRemove(player);
				this.SendToAll(pkg);
				if (isKick)
				{
					player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom", new object[0]));
				}
				bool flag = false;
				if (this.m_host == player)
				{
					if (this.m_playerCount > 0)
					{
						for (int j = 0; j < 10; j++)
						{
							if (this.m_places[j] != null)
							{
								this.SetHost(this.m_places[j]);
								flag = true;
								break;
							}
						}
					}
					else
					{
						this.m_host = null;
					}
				}
				if (this.IsPlaying)
				{
					if (this.m_game != null)
					{
						if (flag && this.m_game is PVEGame)
						{
							PVEGame pVEGame = this.m_game as PVEGame;
							foreach (Player current in pVEGame.Players.Values)
							{
								if (current.PlayerDetail == this.m_host)
								{
									current.Ready = false;
								}
							}
						}
						this.m_game.RemovePlayer(player, isKick);
					}
					if (this.BattleServer != null)
					{
						if (this.m_game != null)
						{
							this.BattleServer.Server.SendPlayerDisconnet(this.Game.Id, player.GamePlayerId, this.RoomId);
							if (this.PlayerCount == 0)
							{
								this.BattleServer.RemoveRoom(this);
							}
						}
						else
						{
							this.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed", new object[0]));
							RoomMgr.AddAction(new CancelPickupAction(this.BattleServer, this));
							this.BattleServer.RemoveRoom(this);
							this.IsPlaying = false;
						}
					}
				}
				else
				{
					this.UpdateGameStyle();
					if (flag)
					{
						foreach (GamePlayer current2 in this.GetPlayers())
						{
							current2.Out.SendRoomChange(this);
						}
					}
				}
			}
			return num != -1;
		}
		public void RemovePlayerAtUnsafe(int pos)
		{
			if (pos < 0 || pos > 9)
			{
				return;
			}
			if (this.m_places[pos].KickProtect)
			{
				string translation = LanguageMgr.GetTranslation("Game.Server.SceneGames.Protect", new object[]
				{
					this.m_places[pos].PlayerCharacter.NickName
				});
				GSPacketIn gSPacketIn = new GSPacketIn(3);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteString(translation);
				this.SendToHost(gSPacketIn);
				return;
			}
			if (this.m_places[pos] != null)
			{
				this.RemovePlayerUnsafe(this.m_places[pos], true);
			}
		}
		public bool SwitchTeamUnsafe(GamePlayer m_player)
		{
			if (this.RoomType == eRoomType.Match)
			{
				return false;
			}
			int num = -1;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				for (int i = (m_player.CurrentRoomIndex + 1) % 2; i < 8; i += 2)
				{
					if (this.m_places[i] == null && this.m_placesState[i] == -1)
					{
						num = i;
						this.m_places[m_player.CurrentRoomIndex] = null;
						this.m_places[i] = m_player;
						this.m_placesState[m_player.CurrentRoomIndex] = -1;
						this.m_placesState[i] = m_player.PlayerId;
						this.m_playerState[i] = this.m_playerState[m_player.CurrentRoomIndex];
						this.m_playerState[m_player.CurrentRoomIndex] = 0;
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(places);
			}
			if (num != -1)
			{
				m_player.CurrentRoomIndex = num;
				m_player.CurrentRoomTeam = num % 2 + 1;
				GSPacketIn pkg = m_player.Out.SendRoomPlayerChangedTeam(m_player);
				this.SendToAll(pkg, m_player);
				this.SendPlaceState();
				return true;
			}
			return false;
		}
		public eLevelLimits GetLevelLimit(GamePlayer player)
		{
			if (player.PlayerCharacter.Grade <= 10)
			{
				return eLevelLimits.ZeroToTen;
			}
			if (player.PlayerCharacter.Grade <= 20)
			{
				return eLevelLimits.ElevenToTwenty;
			}
			return eLevelLimits.TwentyOneToThirty;
		}
		public void StartGame(AbstractGame game)
		{
			if (this.m_game != null)
			{
				List<GamePlayer> players = this.GetPlayers();
				foreach (GamePlayer current in players)
				{
					this.m_game.RemovePlayer(current, false);
				}
				this.m_game_GameStopped(this.m_game);
			}
			this.m_game = game;
			this.IsPlaying = true;
			this.m_game.GameStopped += new GameEventHandle(this.m_game_GameStopped);
		}
		private void m_game_GameStopped(AbstractGame game)
		{
			if (game != null)
			{
				this.m_game.GameStopped -= new GameEventHandle(this.m_game_GameStopped);
				this.m_game = null;
				this.IsPlaying = false;
				RoomMgr.WaitingRoom.SendUpdateRoom(this);
			}
		}
		public void ProcessData(GSPacketIn packet)
		{
			if (this.m_game != null)
			{
				this.m_game.ProcessData(packet);
			}
		}
		public override string ToString()
		{
			return string.Format("Id:{0},player:{1},game:{2},isPlaying:{3}", new object[]
			{
				this.RoomId,
				this.PlayerCount,
				this.Game,
				this.IsPlaying
			});
		}
	}
}
