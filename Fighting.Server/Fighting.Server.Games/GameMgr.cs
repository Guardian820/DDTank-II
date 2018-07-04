using Fighting.Server.GameObjects;
using Fighting.Server.Rooms;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Fighting.Server.Games
{
	public class GameMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static readonly long THREAD_INTERVAL = 40L;
		private static Dictionary<int, BaseGame> m_games;
		private static Thread m_thread;
		private static bool m_running;
		private static int m_serverId;
		private static int m_boxBroadcastLevel;
		private static int m_gameId;
		private static readonly int CLEAR_GAME_INTERVAL = 60000;
		private static long m_clearGamesTimer;
		public static int BoxBroadcastLevel
		{
			get
			{
				return GameMgr.m_boxBroadcastLevel;
			}
		}
		public static bool Setup(int serverId, int boxBroadcastLevel)
		{
			GameMgr.m_thread = new Thread(new ThreadStart(GameMgr.GameThread));
			GameMgr.m_games = new Dictionary<int, BaseGame>();
			GameMgr.m_serverId = serverId;
			GameMgr.m_boxBroadcastLevel = boxBroadcastLevel;
			GameMgr.m_gameId = 0;
			return true;
		}
		public static void Start()
		{
			if (!GameMgr.m_running)
			{
				GameMgr.m_running = true;
				GameMgr.m_thread.Start();
			}
		}
		public static void Stop()
		{
			if (GameMgr.m_running)
			{
				GameMgr.m_running = false;
				GameMgr.m_thread.Join();
			}
		}
		private static void GameThread()
		{
			long num = 0L;
			GameMgr.m_clearGamesTimer = TickHelper.GetTickCount();
			while (GameMgr.m_running)
			{
				long tickCount = TickHelper.GetTickCount();
				try
				{
					GameMgr.UpdateGames(tickCount);
					GameMgr.ClearStoppedGames(tickCount);
				}
				catch (Exception exception)
				{
					GameMgr.log.Error("Room Mgr Thread Error:", exception);
				}
				long tickCount2 = TickHelper.GetTickCount();
				num += GameMgr.THREAD_INTERVAL - (tickCount2 - tickCount);
				if (num > 0L)
				{
					Thread.Sleep((int)num);
					num = 0L;
				}
				else
				{
					if (num < -1000L)
					{
						GameMgr.log.WarnFormat("Room Mgr is delay {0} ms!", num);
						num += 1000L;
					}
				}
			}
		}
		private static void UpdateGames(long tick)
		{
			IList games = GameMgr.GetGames();
			if (games != null)
			{
				foreach (BaseGame baseGame in games)
				{
					try
					{
						baseGame.Update(tick);
					}
					catch (Exception exception)
					{
						GameMgr.log.Error("Game  updated error:", exception);
					}
				}
			}
		}
		private static void ClearStoppedGames(long tick)
		{
			if (GameMgr.m_clearGamesTimer <= tick)
			{
				GameMgr.m_clearGamesTimer += (long)GameMgr.CLEAR_GAME_INTERVAL;
				ArrayList arrayList = new ArrayList();
				Dictionary<int, BaseGame> games;
				Monitor.Enter(games = GameMgr.m_games);
				try
				{
					foreach (BaseGame current in GameMgr.m_games.Values)
					{
						if (current.GameState == eGameState.Stopped)
						{
							arrayList.Add(current);
						}
					}
					foreach (BaseGame baseGame in arrayList)
					{
						GameMgr.m_games.Remove(baseGame.Id);
						try
						{
							baseGame.Dispose();
						}
						catch (Exception exception)
						{
							GameMgr.log.Error("game dispose error:", exception);
						}
					}
				}
				finally
				{
					Monitor.Exit(games);
				}
			}
		}
		public static List<BaseGame> GetGames()
		{
			List<BaseGame> list = new List<BaseGame>();
			Dictionary<int, BaseGame> games;
			Monitor.Enter(games = GameMgr.m_games);
			try
			{
				list.AddRange(GameMgr.m_games.Values);
			}
			finally
			{
				Monitor.Exit(games);
			}
			return list;
		}
		public static BaseGame FindGame(int id)
		{
			Dictionary<int, BaseGame> games;
			Monitor.Enter(games = GameMgr.m_games);
			try
			{
				if (GameMgr.m_games.ContainsKey(id))
				{
					return GameMgr.m_games[id];
				}
			}
			finally
			{
				Monitor.Exit(games);
			}
			return null;
		}
		public static BaseGame StartPVPGame(List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
		{
			BaseGame result;
			try
			{
				int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, GameMgr.m_serverId);
				Map map = MapMgr.CloneMap(mapIndex2);
				List<PetSkillElementInfo> gameNeedPetSkillInfoList = PetMgr.GameNeedPetSkill();
				if (map != null)
				{
					PVPGame pVPGame = new PVPGame(GameMgr.m_gameId++, 0, red, blue, map, roomType, gameType, timeType, gameNeedPetSkillInfoList);
					Dictionary<int, BaseGame> games;
					Monitor.Enter(games = GameMgr.m_games);
					try
					{
						GameMgr.m_games.Add(pVPGame.Id, pVPGame);
					}
					finally
					{
						Monitor.Exit(games);
					}
					pVPGame.Prepare();
					result = pVPGame;
				}
				else
				{
					result = null;
				}
			}
			catch (Exception exception)
			{
				GameMgr.log.Error("Create game error:", exception);
				result = null;
			}
			return result;
		}
		public static BattleGame StartBattleGame(List<IGamePlayer> red, ProxyRoom roomRed, List<IGamePlayer> blue, ProxyRoom roomBlue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
		{
			BattleGame result;
			try
			{
				int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, GameMgr.m_serverId);
				Map map = MapMgr.CloneMap(mapIndex2);
				List<PetSkillElementInfo> gameNeedPetSkill = PetMgr.GameNeedPetSkill();
				if (map != null)
				{
					BattleGame battleGame = new BattleGame(GameMgr.m_gameId++, red, roomRed, blue, roomBlue, map, roomType, gameType, timeType, gameNeedPetSkill);
					Dictionary<int, BaseGame> games;
					Monitor.Enter(games = GameMgr.m_games);
					try
					{
						GameMgr.m_games.Add(battleGame.Id, battleGame);
					}
					finally
					{
						Monitor.Exit(games);
					}
					battleGame.Prepare();
					GameMgr.SendStartMessage(battleGame);
					result = battleGame;
				}
				else
				{
					result = null;
				}
			}
			catch (Exception exception)
			{
				GameMgr.log.Error("Create battle game error:", exception);
				result = null;
			}
			return result;
		}
		public static void SendStartMessage(BattleGame game)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(3);
			gSPacketIn.WriteInt(2);
			if (game.GameType == eGameType.Free)
			{
				foreach (Player current in game.GetAllFightPlayers())
				{
					(current.PlayerDetail as ProxyPlayer).m_antiAddictionRate = 1.0;
					GSPacketIn pkg = GameMgr.SendBufferList(current, (current.PlayerDetail as ProxyPlayer).Buffers);
					game.SendToAll(pkg);
				}
                gSPacketIn.WriteString("Tham chiến thành công, chúc bạn may mắn!");
			}
			else
			{
				gSPacketIn.WriteString("Kết nối thất bại!");
			}
			game.SendToAll(gSPacketIn, null);
		}
		public static GSPacketIn SendBufferList(Player player, List<BufferInfo> infos)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(168, player.Id);
			gSPacketIn.WriteInt(infos.Count);
			foreach (BufferInfo current in infos)
			{
				gSPacketIn.WriteInt(current.Type);
				gSPacketIn.WriteBoolean(current.IsExist);
				gSPacketIn.WriteDateTime(current.BeginDate);
				gSPacketIn.WriteInt(current.ValidDate);
				gSPacketIn.WriteInt(current.Value);
			}
			return gSPacketIn;
		}
	}
}
