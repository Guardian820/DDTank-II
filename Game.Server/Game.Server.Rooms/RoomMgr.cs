using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Rooms
{
	public class RoomMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static bool m_running;
		private static Queue<IAction> m_actionQueue;
		private static Thread m_thread;
		private static BaseRoom[] m_rooms;
		private static BaseWaitingRoom m_waitingRoom;
		private static BaseWorldBossRoom m_worldbossRoom;
		private static long m_worldbossRoom_countdown = 3600000L;
		private static long m_worldbossRoom_countdown_tick = 0L;
		public static bool m_worldbossRoom_update = true;
		public static bool m_worldbossRoom_open = true;
		public static bool m_worldbossRoom_die = false;
		public static readonly int THREAD_INTERVAL = 40;
		public static readonly int PICK_UP_INTERVAL = 10000;
		public static readonly int CLEAR_ROOM_INTERVAL = 400;
		private static long m_clearTick = 0L;
		public static BaseRoom[] Rooms
		{
			get
			{
				return RoomMgr.m_rooms;
			}
		}
		public static BaseWaitingRoom WaitingRoom
		{
			get
			{
				return RoomMgr.m_waitingRoom;
			}
		}
		public static BaseWorldBossRoom WorldBossRoom
		{
			get
			{
				return RoomMgr.m_worldbossRoom;
			}
		}
		public static bool Setup(int maxRoom)
		{
			maxRoom = ((maxRoom < 1) ? 1 : maxRoom);
			RoomMgr.m_thread = new Thread(new ThreadStart(RoomMgr.RoomThread));
			RoomMgr.m_actionQueue = new Queue<IAction>();
			RoomMgr.m_rooms = new BaseRoom[maxRoom];
			for (int i = 0; i < maxRoom; i++)
			{
				RoomMgr.m_rooms[i] = new BaseRoom(i + 1);
			}
			RoomMgr.m_waitingRoom = new BaseWaitingRoom();
			RoomMgr.m_worldbossRoom = new BaseWorldBossRoom();
			return true;
		}
		public static void Start()
		{
			if (!RoomMgr.m_running)
			{
				RoomMgr.m_running = true;
				RoomMgr.m_thread.Start();
			}
		}
		public static void Stop()
		{
			if (RoomMgr.m_running)
			{
				RoomMgr.m_running = false;
				RoomMgr.m_thread.Join();
			}
		}
		private static void RoomThread()
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			long num = 0L;
			RoomMgr.m_clearTick = TickHelper.GetTickCount();
			while (RoomMgr.m_running)
			{
				long tickCount = TickHelper.GetTickCount();
				int num2 = 0;
				try
				{
					num2 = RoomMgr.ExecuteActions();
					if (RoomMgr.m_clearTick <= tickCount)
					{
						RoomMgr.m_clearTick += (long)RoomMgr.CLEAR_ROOM_INTERVAL;
						RoomMgr.ClearRooms(tickCount);
					}
				}
				catch (Exception exception)
				{
					RoomMgr.log.Error("Room Mgr Thread Error:", exception);
				}
				long tickCount2 = TickHelper.GetTickCount();
				num += (long)RoomMgr.THREAD_INTERVAL - (tickCount2 - tickCount);
				if (tickCount2 - tickCount > (long)(RoomMgr.THREAD_INTERVAL * 2))
				{
					RoomMgr.log.WarnFormat("Room Mgr is spent too much times: {0} ms,count:{1}", tickCount2 - tickCount, num2);
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
		private static int ExecuteActions()
		{
			IAction[] array = null;
			Queue<IAction> actionQueue;
			Monitor.Enter(actionQueue = RoomMgr.m_actionQueue);
			try
			{
				if (RoomMgr.m_actionQueue.Count > 0)
				{
					array = new IAction[RoomMgr.m_actionQueue.Count];
					RoomMgr.m_actionQueue.CopyTo(array, 0);
					RoomMgr.m_actionQueue.Clear();
				}
			}
			finally
			{
				Monitor.Exit(actionQueue);
			}
			if (array != null)
			{
				IAction[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					IAction action = array2[i];
					try
					{
						long tickCount = TickHelper.GetTickCount();
						action.Execute();
						long tickCount2 = TickHelper.GetTickCount();
						if (tickCount2 - tickCount > 40L)
						{
							RoomMgr.log.WarnFormat("RoomMgr action spent too much times:{0},{1}ms!", action.GetType(), tickCount2 - tickCount);
						}
					}
					catch (Exception exception)
					{
						RoomMgr.log.Error("RoomMgr execute action error:", exception);
					}
				}
				return array.Length;
			}
			return 0;
		}
		public static void ClearRooms(long tick)
		{
			BaseRoom[] rooms = RoomMgr.m_rooms;
			for (int i = 0; i < rooms.Length; i++)
			{
				BaseRoom baseRoom = rooms[i];
				if (baseRoom.IsUsing && baseRoom.PlayerCount == 0)
				{
					baseRoom.Stop();
				}
			}
		}
		public static void AddAction(IAction action)
		{
			Queue<IAction> actionQueue;
			Monitor.Enter(actionQueue = RoomMgr.m_actionQueue);
			try
			{
				RoomMgr.m_actionQueue.Enqueue(action);
			}
			finally
			{
				Monitor.Exit(actionQueue);
			}
		}
		public static void CreateRoom(GamePlayer player, string name, string password, eRoomType roomType, byte timeType)
		{
			RoomMgr.AddAction(new CreateRoomAction(player, name, password, roomType, timeType));
		}
		public static void EnterRoom(GamePlayer player, int roomId, string pwd, int type)
		{
			RoomMgr.AddAction(new EnterRoomAction(player, roomId, pwd, type));
		}
		public static void EnterRoom(GamePlayer player)
		{
			RoomMgr.EnterRoom(player, -1, null, 1);
		}
		public static void ExitRoom(BaseRoom room, GamePlayer player)
		{
			RoomMgr.AddAction(new ExitRoomAction(room, player));
		}
		public static void StartGame(BaseRoom room)
		{
			RoomMgr.AddAction(new StartGameAction(room));
		}
		public static void StartGameMission(BaseRoom room)
		{
			RoomMgr.AddAction(new StartGameMissionAction(room));
		}
		public static void UpdatePlayerState(GamePlayer player, byte state)
		{
			RoomMgr.AddAction(new UpdatePlayerStateAction(player, player.CurrentRoom, state));
		}
		public static void UpdateRoomPos(BaseRoom room, int pos, bool isOpened, int place, int placeView)
		{
			RoomMgr.AddAction(new UpdateRoomPosAction(room, pos, isOpened, place, placeView));
		}
		public static void KickPlayer(BaseRoom baseRoom, byte index)
		{
			RoomMgr.AddAction(new KickPlayerAction(baseRoom, (int)index));
		}
		public static void EnterWaitingRoom(GamePlayer player)
		{
			RoomMgr.AddAction(new EnterWaitingRoomAction(player));
		}
		public static void ExitWaitingRoom(GamePlayer player)
		{
			RoomMgr.AddAction(new ExitWaitRoomAction(player));
		}
		public static void CancelPickup(BattleServer server, BaseRoom room)
		{
			RoomMgr.AddAction(new CancelPickupAction(server, room));
		}
		public static void UpdateRoomGameType(BaseRoom room, eRoomType roomType, byte timeMode, eHardLevel hardLevel, int levelLimits, int mapId, string password, string roomname, bool isCrosszone, bool isOpenBoss)
		{
			RoomMgr.AddAction(new RoomSetupChangeAction(room, roomType, timeMode, hardLevel, levelLimits, mapId, password, roomname, isCrosszone, isOpenBoss));
		}
		internal static void SwitchTeam(GamePlayer gamePlayer)
		{
			RoomMgr.AddAction(new SwitchTeamAction(gamePlayer));
		}
		public static List<BaseRoom> GetAllUsingRoom()
		{
			List<BaseRoom> list = new List<BaseRoom>();
			BaseRoom[] rooms;
			Monitor.Enter(rooms = RoomMgr.m_rooms);
			try
			{
				BaseRoom[] rooms2 = RoomMgr.m_rooms;
				for (int i = 0; i < rooms2.Length; i++)
				{
					BaseRoom baseRoom = rooms2[i];
					if (baseRoom.IsUsing)
					{
						list.Add(baseRoom);
					}
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			return list;
		}
		public static void StartProxyGame(BaseRoom room, ProxyGame game)
		{
			RoomMgr.AddAction(new StartProxyGameAction(room, game));
		}
		public static void StopProxyGame(BaseRoom room)
		{
			RoomMgr.AddAction(new StopProxyGameAction(room));
		}
	}
}
