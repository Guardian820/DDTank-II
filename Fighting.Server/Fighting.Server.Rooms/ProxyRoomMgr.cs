using Fighting.Server.Games;
using Fighting.Server.Guild;
using Game.Logic;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Fighting.Server.Rooms
{
	public class ProxyRoomMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static readonly int THREAD_INTERVAL = 40;
		public static readonly int PICK_UP_INTERVAL = 10000;
		public static readonly int CLEAR_ROOM_INTERVAL = 1000;
		private static bool m_running = false;
		private static int m_serverId = 1;
		private static Queue<IAction> m_actionQueue = new Queue<IAction>();
		private static Thread m_thread;
		private static Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();
		private static int RoomIndex = 0;
		private static long m_nextPickTick = 0L;
		private static long m_nextClearTick = 0L;
		public static bool Setup()
		{
			ProxyRoomMgr.m_thread = new Thread(new ThreadStart(ProxyRoomMgr.RoomThread));
			return true;
		}
		public static void Start()
		{
			if (!ProxyRoomMgr.m_running)
			{
				ProxyRoomMgr.m_running = true;
				ProxyRoomMgr.m_thread.Start();
			}
		}
		public static void Stop()
		{
			if (ProxyRoomMgr.m_running)
			{
				ProxyRoomMgr.m_running = false;
				ProxyRoomMgr.m_thread.Join();
			}
		}
		public static void AddAction(IAction action)
		{
			Queue<IAction> actionQueue;
			Monitor.Enter(actionQueue = ProxyRoomMgr.m_actionQueue);
			try
			{
				ProxyRoomMgr.m_actionQueue.Enqueue(action);
			}
			finally
			{
				Monitor.Exit(actionQueue);
			}
		}
		private static void RoomThread()
		{
			long num = 0L;
			ProxyRoomMgr.m_nextClearTick = TickHelper.GetTickCount();
			ProxyRoomMgr.m_nextPickTick = TickHelper.GetTickCount();
			while (ProxyRoomMgr.m_running)
			{
				long tickCount = TickHelper.GetTickCount();
				try
				{
					ProxyRoomMgr.ExecuteActions();
					if (ProxyRoomMgr.m_nextPickTick <= tickCount)
					{
						ProxyRoomMgr.m_nextPickTick += (long)ProxyRoomMgr.PICK_UP_INTERVAL;
						ProxyRoomMgr.PickUpRooms(tickCount);
					}
					if (ProxyRoomMgr.m_nextClearTick <= tickCount)
					{
						ProxyRoomMgr.m_nextClearTick += (long)ProxyRoomMgr.CLEAR_ROOM_INTERVAL;
						ProxyRoomMgr.ClearRooms(tickCount);
					}
				}
				catch (Exception exception)
				{
					ProxyRoomMgr.log.Error("Room Mgr Thread Error:", exception);
				}
				long tickCount2 = TickHelper.GetTickCount();
				num += (long)ProxyRoomMgr.THREAD_INTERVAL - (tickCount2 - tickCount);
				if (num > 0L)
				{
					Thread.Sleep((int)num);
					num = 0L;
				}
				else
				{
					if (num < -1000L)
					{
						ProxyRoomMgr.log.WarnFormat("Room Mgr is delay {0} ms!", num);
						num += 1000L;
					}
				}
			}
		}
		private static void ExecuteActions()
		{
			IAction[] array = null;
			Queue<IAction> actionQueue;
			Monitor.Enter(actionQueue = ProxyRoomMgr.m_actionQueue);
			try
			{
				if (ProxyRoomMgr.m_actionQueue.Count > 0)
				{
					array = new IAction[ProxyRoomMgr.m_actionQueue.Count];
					ProxyRoomMgr.m_actionQueue.CopyTo(array, 0);
					ProxyRoomMgr.m_actionQueue.Clear();
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
						action.Execute();
					}
					catch (Exception exception)
					{
						ProxyRoomMgr.log.Error("RoomMgr execute action error:", exception);
					}
				}
			}
		}
		private static void PickUpRooms(long tick)
		{
			List<ProxyRoom> waitMatchRoomUnsafe = ProxyRoomMgr.GetWaitMatchRoomUnsafe();
			foreach (ProxyRoom current in waitMatchRoomUnsafe)
			{
				int num = -2147483648;
				ProxyRoom proxyRoom = null;
				if (!current.IsPlaying)
				{
					if (current.GameType == eGameType.ALL)
					{
						using (List<ProxyRoom>.Enumerator enumerator2 = waitMatchRoomUnsafe.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								ProxyRoom current2 = enumerator2.Current;
								if ((current2.GuildId == 0 || current2.GuildId != current.GuildId) && current2 != current && !current2.IsPlaying && current2.PlayerCount == current.PlayerCount)
								{
									int num2 = GuildMgr.FindGuildRelationShip(current.GuildId, current2.GuildId) + 1;
									int gameType = (int)current2.GameType;
									int num3 = Math.Abs(current.AvgLevel - current2.AvgLevel);
									int num4 = Math.Abs(current.FightPower - current2.FightPower);
									int num5 = num2 * 10000 + gameType * 1000 + num4 + num3;
									if (num5 > num)
									{
										proxyRoom = current2;
									}
								}
							}
							goto IL_2A7;
						}
						goto IL_114;
					}
					goto IL_114;
					IL_2A7:
					if (proxyRoom != null)
					{
						ProxyRoomMgr.StartMatchGame(current, proxyRoom);
						continue;
					}
					continue;
					IL_114:
					if (current.GameType == eGameType.Guild)
					{
						using (List<ProxyRoom>.Enumerator enumerator3 = waitMatchRoomUnsafe.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								ProxyRoom current3 = enumerator3.Current;
								if ((current3.GuildId == 0 || current3.GuildId != current.GuildId) && current3 != current && current3.GameType != eGameType.Free && !current3.IsPlaying && current3.PlayerCount == current.PlayerCount)
								{
									int num6 = GuildMgr.FindGuildRelationShip(current.GuildId, current3.GuildId) + 1;
									int gameType2 = (int)current3.GameType;
									int num7 = Math.Abs(current.FightPower - current3.FightPower);
									int num8 = Math.Abs(current.AvgLevel - current3.AvgLevel);
									int num9 = num6 * 10000 + gameType2 * 1000 + num7 + num8;
									if (num9 > num)
									{
										proxyRoom = current3;
									}
								}
							}
							goto IL_2A7;
						}
					}
					foreach (ProxyRoom current4 in waitMatchRoomUnsafe)
					{
						if (current4 != current && current4.GameType != eGameType.Guild && !current4.IsPlaying && current4.PlayerCount == current.PlayerCount)
						{
							int gameType3 = (int)current4.GameType;
							int num10 = Math.Abs(current.AvgLevel - current4.AvgLevel);
							int num11 = Math.Abs(current.FightPower - current4.FightPower);
							int num12 = gameType3 * 1000 + num11 + num10;
							if (num12 > num)
							{
								proxyRoom = current4;
							}
						}
					}
					goto IL_2A7;
				}
				break;
			}
		}
		private static void ClearRooms(long tick)
		{
			List<ProxyRoom> list = new List<ProxyRoom>();
			foreach (ProxyRoom current in ProxyRoomMgr.m_rooms.Values)
			{
				if (!current.IsPlaying && current.Game != null)
				{
					list.Add(current);
				}
			}
			foreach (ProxyRoom current2 in list)
			{
				ProxyRoomMgr.m_rooms.Remove(current2.RoomId);
				try
				{
					current2.Dispose();
				}
				catch (Exception exception)
				{
					ProxyRoomMgr.log.Error("Room dispose error:", exception);
				}
			}
		}
		private static void StartMatchGame(ProxyRoom red, ProxyRoom blue)
		{
			int mapIndex = MapMgr.GetMapIndex(0, 0, ProxyRoomMgr.m_serverId);
			eGameType gameType = eGameType.Free;
			if (red.GameType == blue.GameType)
			{
				gameType = red.GameType;
			}
			BaseGame baseGame = GameMgr.StartBattleGame(red.GetPlayers(), red, blue.GetPlayers(), blue, mapIndex, eRoomType.Match, gameType, 2);
			if (baseGame != null)
			{
				blue.StartGame(baseGame);
				red.StartGame(baseGame);
			}
			if (baseGame.GameType == eGameType.Guild)
			{
				red.Client.SendConsortiaAlly(red.GetPlayers()[0].PlayerCharacter.ConsortiaID, blue.GetPlayers()[0].PlayerCharacter.ConsortiaID, baseGame.Id);
			}
		}
		public static bool AddRoomUnsafe(ProxyRoom room)
		{
			if (!ProxyRoomMgr.m_rooms.ContainsKey(room.RoomId))
			{
				ProxyRoomMgr.m_rooms.Add(room.RoomId, room);
				return true;
			}
			return false;
		}
		public static bool RemoveRoomUnsafe(int roomId)
		{
			if (ProxyRoomMgr.m_rooms.ContainsKey(roomId))
			{
				ProxyRoomMgr.m_rooms.Remove(roomId);
				return true;
			}
			return false;
		}
		public static ProxyRoom GetRoomUnsafe(int roomId)
		{
			if (ProxyRoomMgr.m_rooms.ContainsKey(roomId))
			{
				return ProxyRoomMgr.m_rooms[roomId];
			}
			return null;
		}
		public static ProxyRoom[] GetAllRoom()
		{
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = ProxyRoomMgr.m_rooms);
			ProxyRoom[] allRoomUnsafe;
			try
			{
				allRoomUnsafe = ProxyRoomMgr.GetAllRoomUnsafe();
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			return allRoomUnsafe;
		}
		public static ProxyRoom[] GetAllRoomUnsafe()
		{
			ProxyRoom[] array = new ProxyRoom[ProxyRoomMgr.m_rooms.Values.Count];
			ProxyRoomMgr.m_rooms.Values.CopyTo(array, 0);
			return array;
		}
		public static List<ProxyRoom> GetWaitMatchRoomUnsafe()
		{
			List<ProxyRoom> list = new List<ProxyRoom>();
			foreach (ProxyRoom current in ProxyRoomMgr.m_rooms.Values)
			{
				if (!current.IsPlaying && current.Game == null)
				{
					list.Add(current);
				}
			}
			return list;
		}
		public static int NextRoomId()
		{
			return Interlocked.Increment(ref ProxyRoomMgr.RoomIndex);
		}
		public static void AddRoom(ProxyRoom room)
		{
			ProxyRoomMgr.AddAction(new AddRoomAction(room));
		}
		public static void RemoveRoom(ProxyRoom room)
		{
			ProxyRoomMgr.AddAction(new RemoveRoomAction(room));
		}
	}
}
