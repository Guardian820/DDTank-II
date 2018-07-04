using Game.Base;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.Battle
{
	public class BattleServer
	{
		private int m_serverId;
		private FightServerConnector m_server;
		private Dictionary<int, BaseRoom> m_rooms;
		private string m_ip;
		private int m_port;
		public FightServerConnector Server
		{
			get
			{
				return this.m_server;
			}
		}
		public bool IsActive
		{
			get
			{
				return this.m_server.IsConnected;
			}
		}
		public string Ip
		{
			get
			{
				return this.m_ip;
			}
		}
		public int Port
		{
			get
			{
				return this.m_port;
			}
		}
		public BattleServer(int serverId, string ip, int port, string loginKey)
		{
			this.m_serverId = serverId;
			this.m_ip = ip;
			this.m_port = port;
			this.m_server = new FightServerConnector(this, ip, port, loginKey);
			this.m_rooms = new Dictionary<int, BaseRoom>();
			this.m_server.Disconnected += new ClientEventHandle(this.m_server_Disconnected);
			this.m_server.Connected += new ClientEventHandle(this.m_server_Connected);
		}
		public void Start()
		{
			this.m_server.Connect();
		}
		private void m_server_Connected(BaseClient client)
		{
		}
		private void m_server_Disconnected(BaseClient client)
		{
		}
		public BaseRoom FindRoom(int roomId)
		{
			BaseRoom result = null;
			Dictionary<int, BaseRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(roomId))
				{
					result = this.m_rooms[roomId];
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			return result;
		}
		public bool AddRoom(BaseRoom room)
		{
			bool flag = false;
			Dictionary<int, BaseRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (!this.m_rooms.ContainsKey(room.RoomId))
				{
					this.m_rooms.Add(room.RoomId, room);
					flag = true;
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (flag)
			{
				this.m_server.SendAddRoom(room);
			}
			return flag;
		}
		public bool RemoveRoom(BaseRoom room)
		{
			bool flag = false;
			Dictionary<int, BaseRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				flag = this.m_rooms.ContainsKey(room.RoomId);
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (flag)
			{
				this.m_server.SendRemoveRoom(room);
			}
			return flag;
		}
		public void RemoveRoomImp(int roomId)
		{
			BaseRoom baseRoom = null;
			Dictionary<int, BaseRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(roomId))
				{
					baseRoom = this.m_rooms[roomId];
					this.m_rooms.Remove(roomId);
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (baseRoom != null)
			{
				if (baseRoom.IsPlaying && baseRoom.Game == null)
				{
					RoomMgr.CancelPickup(this, baseRoom);
					return;
				}
				RoomMgr.StopProxyGame(baseRoom);
			}
		}
		public void StartGame(int roomId, ProxyGame game)
		{
			BaseRoom baseRoom = this.FindRoom(roomId);
			if (baseRoom != null)
			{
				RoomMgr.StartProxyGame(baseRoom, game);
			}
		}
		public void StopGame(int roomId, int gameId)
		{
			BaseRoom baseRoom = this.FindRoom(roomId);
			if (baseRoom != null)
			{
				RoomMgr.StopProxyGame(baseRoom);
				Dictionary<int, BaseRoom> rooms;
				Monitor.Enter(rooms = this.m_rooms);
				try
				{
					this.m_rooms.Remove(roomId);
				}
				finally
				{
					Monitor.Exit(rooms);
				}
			}
		}
		public void SendToRoom(int roomId, GSPacketIn pkg, int exceptId, int exceptGameId)
		{
			BaseRoom baseRoom = this.FindRoom(roomId);
			if (baseRoom != null)
			{
				if (exceptId != 0)
				{
					GamePlayer playerById = WorldMgr.GetPlayerById(exceptId);
					if (playerById != null)
					{
						if (playerById.GamePlayerId == exceptGameId)
						{
							baseRoom.SendToAll(pkg, playerById);
							return;
						}
						baseRoom.SendToAll(pkg);
						return;
					}
				}
				else
				{
					baseRoom.SendToAll(pkg);
				}
			}
		}
		public void SendToUser(int playerid, GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(playerid);
			if (playerById != null)
			{
				playerById.SendTCP(pkg);
			}
		}
		public void UpdatePlayerGameId(int playerid, int gamePlayerId)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(playerid);
			if (playerById != null)
			{
				playerById.GamePlayerId = gamePlayerId;
			}
		}
		public override string ToString()
		{
			return string.Format("ServerID:{0},Ip:{1},Port:{2},IsConnected:{3},RoomCount:{4}", new object[]
			{
				this.m_serverId,
				this.m_server.RemoteEP.Address,
				this.m_server.RemoteEP.Port,
				this.m_server.IsConnected,
				this.m_rooms.Count
			});
		}
	}
}
