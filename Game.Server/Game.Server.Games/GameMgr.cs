using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Server.Statics;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Games
{
	public class GameMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static readonly long THREAD_INTERVAL = 40L;
		private static List<BaseGame> m_games;
		private static Thread m_thread;
		private static bool m_running;
		private static int m_serverId;
		private static int m_boxBroadcastLevel;
		private static int m_gameId;
		private static readonly int CLEAR_GAME_INTERVAL = 10000;
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
			GameMgr.m_games = new List<BaseGame>();
			GameMgr.m_serverId = serverId;
			GameMgr.m_boxBroadcastLevel = boxBroadcastLevel;
			GameMgr.m_gameId = 0;
			return true;
		}
		public static bool Start()
		{
			if (!GameMgr.m_running)
			{
				GameMgr.m_running = true;
				GameMgr.m_thread.Start();
			}
			return true;
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
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			long num = 0L;
			GameMgr.m_clearGamesTimer = TickHelper.GetTickCount();
			while (GameMgr.m_running)
			{
				long tickCount = TickHelper.GetTickCount();
				int num2 = 0;
				try
				{
					num2 = GameMgr.UpdateGames(tickCount);
					if (GameMgr.m_clearGamesTimer <= tickCount)
					{
						GameMgr.m_clearGamesTimer += (long)GameMgr.CLEAR_GAME_INTERVAL;
						ArrayList arrayList = new ArrayList();
						foreach (BaseGame current in GameMgr.m_games)
						{
							if (current.GameState == eGameState.Stopped)
							{
								arrayList.Add(current);
							}
						}
						foreach (BaseGame item in arrayList)
						{
							GameMgr.m_games.Remove(item);
						}
						ThreadPool.QueueUserWorkItem(new WaitCallback(GameMgr.ClearStoppedGames), arrayList);
					}
				}
				catch (Exception exception)
				{
					GameMgr.log.Error("Game Mgr Thread Error:", exception);
				}
				long tickCount2 = TickHelper.GetTickCount();
				num += GameMgr.THREAD_INTERVAL - (tickCount2 - tickCount);
				if (tickCount2 - tickCount > GameMgr.THREAD_INTERVAL * 2L)
				{
					GameMgr.log.WarnFormat("Game Mgr spent too much times: {0} ms, count:{1}", tickCount2 - tickCount, num2);
				}
				if (num > 0L)
				{
					Thread.Sleep((int)num);
					num = 0L;
				}
				else
				{
					if (num < -1000L)
					{
						num += 1000L;
					}
				}
			}
		}
		private static void ClearStoppedGames(object state)
		{
			ArrayList arrayList = state as ArrayList;
			foreach (BaseGame baseGame in arrayList)
			{
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
		private static int UpdateGames(long tick)
		{
			IList allGame = GameMgr.GetAllGame();
			if (allGame != null)
			{
				foreach (BaseGame baseGame in allGame)
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
				return allGame.Count;
			}
			return 0;
		}
		public static List<BaseGame> GetAllGame()
		{
			List<BaseGame> list = new List<BaseGame>();
			List<BaseGame> games;
			Monitor.Enter(games = GameMgr.m_games);
			try
			{
				list.AddRange(GameMgr.m_games);
			}
			finally
			{
				Monitor.Exit(games);
			}
			return list;
		}
		public static BaseGame StartPVPGame(int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
		{
			BaseGame result;
			try
			{
				int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, GameMgr.m_serverId);
				Map map = MapMgr.CloneMap(mapIndex2);
				List<PetSkillElementInfo> gameNeedPetSkillInfoList = PetMgr.GameNeedPetSkill();
				if (map != null)
				{
					PVPGame pVPGame = new PVPGame(GameMgr.m_gameId++, roomId, red, blue, map, roomType, gameType, timeType, gameNeedPetSkillInfoList);
					pVPGame.GameOverLog += new BaseGame.GameOverLogEventHandle(LogMgr.LogFightAdd);
					List<BaseGame> games;
					Monitor.Enter(games = GameMgr.m_games);
					try
					{
						GameMgr.m_games.Add(pVPGame);
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
		public static BaseGame StartPVEGame(int roomId, List<IGamePlayer> players, int copyId, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel, int levelLimits)
		{
			BaseGame result;
			try
			{
				List<PetSkillElementInfo> gameNeedPetSkillInfoList = PetMgr.GameNeedPetSkill();
				PveInfo pveInfo;
				if (copyId == 0 || copyId == 100000)
				{
					pveInfo = PveInfoMgr.GetPveInfoByType(roomType, levelLimits);
				}
				else
				{
					pveInfo = PveInfoMgr.GetPveInfoById(copyId);
				}
				if (pveInfo != null)
				{
					PVEGame pVEGame = new PVEGame(GameMgr.m_gameId++, roomId, pveInfo, players, null, roomType, gameType, timeType, hardLevel, gameNeedPetSkillInfoList);
					pVEGame.GameOverLog += new BaseGame.GameOverLogEventHandle(LogMgr.LogFightAdd);
					List<BaseGame> games;
					Monitor.Enter(games = GameMgr.m_games);
					try
					{
						GameMgr.m_games.Add(pVEGame);
					}
					finally
					{
						Monitor.Exit(games);
					}
					pVEGame.Prepare();
					result = pVEGame;
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
	}
}
