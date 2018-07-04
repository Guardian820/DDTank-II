using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.SceneMarryRooms
{
	public class MarryRoom
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static object _syncStop = new object();
		private List<GamePlayer> _guestsList;
		private IMarryProcessor _processor;
		private int _count;
		public MarryRoomInfo Info;
		private eRoomState _roomState;
		private Timer _timer;
		private Timer _timerForHymeneal;
		private List<int> _userForbid;
		private List<int> _userRemoveList;
		public eRoomState RoomState
		{
			get
			{
				return this._roomState;
			}
			set
			{
				if (this._roomState != value)
				{
					this._roomState = value;
					this.SendMarryRoomInfoUpdateToScenePlayers(this);
				}
			}
		}
		public int Count
		{
			get
			{
				return this._count;
			}
		}
		public MarryRoom(MarryRoomInfo info, IMarryProcessor processor)
		{
			this.Info = info;
			this._processor = processor;
			this._guestsList = new List<GamePlayer>();
			this._count = 0;
			this._roomState = eRoomState.FREE;
			this._userForbid = new List<int>();
			this._userRemoveList = new List<int>();
		}
		public bool AddPlayer(GamePlayer player)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			try
			{
				if (player.CurrentRoom != null || player.IsInMarryRoom)
				{
					bool result = false;
					return result;
				}
				if (this._guestsList.Count > this.Info.MaxCount)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoom.Msg1", new object[0]));
					bool result = false;
					return result;
				}
				this._count++;
				this._guestsList.Add(player);
				player.CurrentMarryRoom = this;
				player.MarryMap = 1;
				if (player.CurrentRoom != null)
				{
					player.CurrentRoom.RemovePlayerUnsafe(player);
				}
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
			return true;
		}
		public void RemovePlayer(GamePlayer player)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			try
			{
				if (this.RoomState == eRoomState.FREE)
				{
					this._count--;
					this._guestsList.Remove(player);
					GSPacketIn packet = player.Out.SendPlayerLeaveMarryRoom(player);
					player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(packet, player);
					player.CurrentMarryRoom = null;
					player.MarryMap = 0;
				}
				else
				{
					if (this.RoomState == eRoomState.Hymeneal)
					{
						this._userRemoveList.Add(player.PlayerCharacter.ID);
						this._count--;
						this._guestsList.Remove(player);
						player.CurrentMarryRoom = null;
					}
				}
				this.SendMarryRoomInfoUpdateToScenePlayers(this);
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
		}
		public void BeginTimer(int interval)
		{
			if (this._timer == null)
			{
				this._timer = new Timer(new TimerCallback(this.OnTick), null, interval, interval);
				return;
			}
			this._timer.Change(interval, interval);
		}
		protected void OnTick(object obj)
		{
			this._processor.OnTick(this);
		}
		public void StopTimer()
		{
			if (this._timer != null)
			{
				this._timer.Dispose();
				this._timer = null;
			}
		}
		public void BeginTimerForHymeneal(int interval)
		{
			if (this._timerForHymeneal == null)
			{
				this._timerForHymeneal = new Timer(new TimerCallback(this.OnTickForHymeneal), null, interval, interval);
				return;
			}
			this._timerForHymeneal.Change(interval, interval);
		}
		protected void OnTickForHymeneal(object obj)
		{
			try
			{
				this._roomState = eRoomState.FREE;
				GSPacketIn gSPacketIn = new GSPacketIn(249);
				gSPacketIn.WriteByte(9);
				this.SendToAll(gSPacketIn);
				this.StopTimerForHymeneal();
				this.SendUserRemoveLate();
				this.SendMarryRoomInfoUpdateToScenePlayers(this);
			}
			catch (Exception exception)
			{
				if (MarryRoom.log.IsErrorEnabled)
				{
					MarryRoom.log.Error("OnTickForHymeneal", exception);
				}
			}
		}
		public void StopTimerForHymeneal()
		{
			if (this._timerForHymeneal != null)
			{
				this._timerForHymeneal.Dispose();
				this._timerForHymeneal = null;
			}
		}
		public GamePlayer[] GetAllPlayers()
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			GamePlayer[] result;
			try
			{
				result = this._guestsList.ToArray();
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
			return result;
		}
		public void SendToRoomPlayer(GSPacketIn packet)
		{
			GamePlayer[] allPlayers = this.GetAllPlayers();
			if (allPlayers != null)
			{
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer gamePlayer = array[i];
					gamePlayer.Out.SendTCP(packet);
				}
			}
		}
		public void SendToAll(GSPacketIn packet)
		{
			this.SendToAll(packet, null, false);
		}
		public void SendToAll(GSPacketIn packet, GamePlayer self, bool isChat)
		{
			GamePlayer[] allPlayers = this.GetAllPlayers();
			if (allPlayers != null)
			{
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer gamePlayer = array[i];
					if (!isChat || !gamePlayer.IsBlackFriend(self.PlayerCharacter.ID))
					{
						gamePlayer.Out.SendTCP(packet);
					}
				}
			}
		}
		public void SendToAllForScene(GSPacketIn packet, int sceneID)
		{
			GamePlayer[] allPlayers = this.GetAllPlayers();
			if (allPlayers != null)
			{
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer gamePlayer = array[i];
					if (gamePlayer.MarryMap == sceneID)
					{
						gamePlayer.Out.SendTCP(packet);
					}
				}
			}
		}
		public void SendToPlayerExceptSelf(GSPacketIn packet, GamePlayer self)
		{
			GamePlayer[] allPlayers = this.GetAllPlayers();
			if (allPlayers != null)
			{
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer gamePlayer = array[i];
					if (gamePlayer != self)
					{
						gamePlayer.Out.SendTCP(packet);
					}
				}
			}
		}
		public void SendToPlayerExceptSelfForScene(GSPacketIn packet, GamePlayer self)
		{
			GamePlayer[] allPlayers = this.GetAllPlayers();
			if (allPlayers != null)
			{
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer gamePlayer = array[i];
					if (gamePlayer != self && gamePlayer.MarryMap == self.MarryMap)
					{
						gamePlayer.Out.SendTCP(packet);
					}
				}
			}
		}
		public void SendToScenePlayer(GSPacketIn packet)
		{
			WorldMgr.MarryScene.SendToALL(packet);
		}
		public void ProcessData(GamePlayer player, GSPacketIn data)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			try
			{
				this._processor.OnGameData(this, player, data);
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
		}
		public void ReturnPacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = packet.Clone();
			gSPacketIn.ClientID = player.PlayerCharacter.ID;
			this.SendToPlayerExceptSelf(gSPacketIn, player);
		}
		public void ReturnPacketForScene(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn gSPacketIn = packet.Clone();
			gSPacketIn.ClientID = player.PlayerCharacter.ID;
			this.SendToPlayerExceptSelfForScene(gSPacketIn, player);
		}
		public bool KickPlayerByUserID(GamePlayer player, int userID)
		{
			GamePlayer playerByUserID = this.GetPlayerByUserID(userID);
			if (playerByUserID != null && playerByUserID.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && playerByUserID.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
			{
				this.RemovePlayer(playerByUserID);
				playerByUserID.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom", new object[0]));
				GSPacketIn packet = player.Out.SendMessage(eMessageType.ChatERROR, playerByUserID.PlayerCharacter.NickName + "  " + LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom2", new object[0]));
				player.CurrentMarryRoom.SendToPlayerExceptSelf(packet, player);
				return true;
			}
			return false;
		}
		public void KickAllPlayer()
		{
			GamePlayer[] allPlayers = this.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				this.RemovePlayer(gamePlayer);
				gamePlayer.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoom.TimeOver", new object[0]));
			}
		}
		public GamePlayer GetPlayerByUserID(int userID)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			try
			{
				foreach (GamePlayer current in this._guestsList)
				{
					if (current.PlayerCharacter.ID == userID)
					{
						return current;
					}
				}
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
			return null;
		}
		public void RoomContinuation(int time)
		{
			TimeSpan timeSpan = DateTime.Now - this.Info.BeginTime;
			int num = this.Info.AvailTime * 60 - timeSpan.Minutes + time * 60;
			this.Info.AvailTime += time;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				playerBussiness.UpdateMarryRoomInfo(this.Info);
			}
			this.BeginTimer(60000 * num);
		}
		public void SetUserForbid(int userID)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			try
			{
				this._userForbid.Add(userID);
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
		}
		public bool CheckUserForbid(int userID)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			bool result;
			try
			{
				result = this._userForbid.Contains(userID);
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
			return result;
		}
		public void SendUserRemoveLate()
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			try
			{
				foreach (int current in this._userRemoveList)
				{
					GSPacketIn packet = new GSPacketIn(244, current);
					this.SendToAllForScene(packet, 1);
				}
				this._userRemoveList.Clear();
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
		}
		public GSPacketIn SendMarryRoomInfoUpdateToScenePlayers(MarryRoom room)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(255);
			bool flag = room != null;
			gSPacketIn.WriteBoolean(flag);
			if (flag)
			{
				gSPacketIn.WriteInt(room.Info.ID);
				gSPacketIn.WriteBoolean(room.Info.IsHymeneal);
				gSPacketIn.WriteString(room.Info.Name);
				gSPacketIn.WriteBoolean(!(room.Info.Pwd == ""));
				gSPacketIn.WriteInt(room.Info.MapIndex);
				gSPacketIn.WriteInt(room.Info.AvailTime);
				gSPacketIn.WriteInt(room.Count);
				gSPacketIn.WriteInt(room.Info.PlayerID);
				gSPacketIn.WriteString(room.Info.PlayerName);
				gSPacketIn.WriteInt(room.Info.GroomID);
				gSPacketIn.WriteString(room.Info.GroomName);
				gSPacketIn.WriteInt(room.Info.BrideID);
				gSPacketIn.WriteString(room.Info.BrideName);
				gSPacketIn.WriteDateTime(room.Info.BeginTime);
				gSPacketIn.WriteByte((byte)room.RoomState);
				gSPacketIn.WriteString(room.Info.RoomIntroduction);
			}
			this.SendToScenePlayer(gSPacketIn);
			return gSPacketIn;
		}
	}
}
