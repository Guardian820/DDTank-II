using Bussiness;
using Game.Server.GameObjects;
using Game.Server.SceneMarryRooms;
using log4net;
using log4net.Util;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Game.Server.Managers
{
	public class MarryRoomMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected static ReaderWriterLock _locker = new ReaderWriterLock();
		protected static Dictionary<int, MarryRoom> _Rooms;
		protected static TankMarryLogicProcessor _processor = new TankMarryLogicProcessor();
		public static bool Init()
		{
			MarryRoomMgr._Rooms = new Dictionary<int, MarryRoom>();
			MarryRoomMgr.CheckRoomStatus();
			return true;
		}
		private static void CheckRoomStatus()
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				MarryRoomInfo[] marryRoomInfo = playerBussiness.GetMarryRoomInfo();
				MarryRoomInfo[] array = marryRoomInfo;
				for (int i = 0; i < array.Length; i++)
				{
					MarryRoomInfo marryRoomInfo2 = array[i];
					if (marryRoomInfo2.ServerID == GameServer.Instance.Configuration.ServerID)
					{
						TimeSpan timeSpan = DateTime.Now - marryRoomInfo2.BeginTime;
						int num = marryRoomInfo2.AvailTime * 60 - (int)timeSpan.TotalMinutes;
						if (num > 0)
						{
							MarryRoomMgr.CreateMarryRoomFromDB(marryRoomInfo2, num);
						}
						else
						{
							playerBussiness.DisposeMarryRoomInfo(marryRoomInfo2.ID);
							if (GameServer.Instance.LoginServer != null)
							{
								Console.WriteLine(marryRoomInfo2.Name);
								GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(marryRoomInfo2.GroomID);
								GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(marryRoomInfo2.BrideID);
								GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(marryRoomInfo2.GroomID, false, marryRoomInfo2);
								GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(marryRoomInfo2.BrideID, false, marryRoomInfo2);
							}
						}
					}
				}
			}
		}
		public static MarryRoom[] GetAllMarryRoom()
		{
			MarryRoom[] array = null;
			MarryRoomMgr._locker.AcquireReaderLock();
			try
			{
				array = new MarryRoom[MarryRoomMgr._Rooms.Count];
				MarryRoomMgr._Rooms.Values.CopyTo(array, 0);
			}
			finally
			{
				MarryRoomMgr._locker.ReleaseReaderLock();
			}
			if (array != null)
			{
				return array;
			}
			return new MarryRoom[0];
		}
		public static MarryRoom CreateMarryRoom(GamePlayer player, MarryRoomInfo info)
		{
			if (!player.PlayerCharacter.IsMarried)
			{
				return null;
			}
			MarryRoom marryRoom = null;
			DateTime now = DateTime.Now;
			info.PlayerID = player.PlayerCharacter.ID;
			info.PlayerName = player.PlayerCharacter.NickName;
			if (player.PlayerCharacter.Sex)
			{
				info.GroomID = info.PlayerID;
				info.GroomName = info.PlayerName;
				info.BrideID = player.PlayerCharacter.SpouseID;
				info.BrideName = player.PlayerCharacter.SpouseName;
			}
			else
			{
				info.BrideID = info.PlayerID;
				info.BrideName = info.PlayerName;
				info.GroomID = player.PlayerCharacter.SpouseID;
				info.GroomName = player.PlayerCharacter.SpouseName;
			}
			info.BeginTime = now;
			info.BreakTime = now;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				if (playerBussiness.InsertMarryRoomInfo(info))
				{
					marryRoom = new MarryRoom(info, MarryRoomMgr._processor);
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(info.GroomID);
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(info.BrideID);
					GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(info.GroomID, true, info);
					GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(info.BrideID, true, info);
				}
			}
			if (marryRoom != null)
			{
				MarryRoomMgr._locker.AcquireWriterLock();
				try
				{
					MarryRoomMgr._Rooms.Add(marryRoom.Info.ID, marryRoom);
				}
				finally
				{
					MarryRoomMgr._locker.ReleaseWriterLock();
				}
				if (marryRoom.AddPlayer(player))
				{
					marryRoom.BeginTimer(3600000 * marryRoom.Info.AvailTime);
					return marryRoom;
				}
			}
			return null;
		}
		public static MarryRoom CreateMarryRoomFromDB(MarryRoomInfo roomInfo, int timeLeft)
		{
			MarryRoomMgr._locker.AcquireWriterLock();
			try
			{
				MarryRoom marryRoom = new MarryRoom(roomInfo, MarryRoomMgr._processor);
				if (marryRoom != null)
				{
					MarryRoomMgr._Rooms.Add(marryRoom.Info.ID, marryRoom);
					marryRoom.BeginTimer(60000 * timeLeft);
					return marryRoom;
				}
			}
			finally
			{
				MarryRoomMgr._locker.ReleaseWriterLock();
			}
			return null;
		}
		public static MarryRoom GetMarryRoombyID(int id, string pwd, ref string msg)
		{
			MarryRoom result = null;
			MarryRoomMgr._locker.AcquireReaderLock();
			try
			{
				if (id > 0 && MarryRoomMgr._Rooms.Keys.Contains(id))
				{
					if (MarryRoomMgr._Rooms[id].Info.Pwd != pwd)
					{
						msg = "Game.Server.Managers.PWDError";
					}
					else
					{
						result = MarryRoomMgr._Rooms[id];
					}
				}
			}
			finally
			{
				MarryRoomMgr._locker.ReleaseReaderLock();
			}
			return result;
		}
		public static bool UpdateBreakTimeWhereServerStop()
		{
			bool result;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				result = playerBussiness.UpdateBreakTimeWhereServerStop();
			}
			return result;
		}
		public static void RemoveMarryRoom(MarryRoom room)
		{
			MarryRoomMgr._locker.AcquireReaderLock();
			try
			{
				if (MarryRoomMgr._Rooms.Keys.Contains(room.Info.ID))
				{
					MarryRoomMgr._Rooms.Remove(room.Info.ID);
				}
			}
			finally
			{
				MarryRoomMgr._locker.ReleaseReaderLock();
			}
		}
	}
}
