using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
namespace Game.Server.Managers
{
	public sealed class WorldMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();
		private static Dictionary<int, GamePlayer> m_players = new Dictionary<int, GamePlayer>();
		public static Scene _marryScene;
		public static Scene _hotSpring;
		private static RSACryptoServiceProvider m_rsa;
		public static Scene MarryScene
		{
			get
			{
				return WorldMgr._marryScene;
			}
		}
		public static Scene HotSpring
		{
			get
			{
				return WorldMgr._hotSpring;
			}
		}
		public static RSACryptoServiceProvider RsaCryptor
		{
			get
			{
				return WorldMgr.m_rsa;
			}
		}
		public static bool Init()
		{
			bool result = false;
			try
			{
				WorldMgr.m_rsa = new RSACryptoServiceProvider();
				WorldMgr.m_rsa.FromXmlString(GameServer.Instance.Configuration.PrivateKey);
				WorldMgr.m_players.Clear();
				using (ServiceBussiness serviceBussiness = new ServiceBussiness())
				{
					ServerInfo serviceSingle = serviceBussiness.GetServiceSingle(GameServer.Instance.Configuration.ServerID);
					if (serviceSingle != null)
					{
						WorldMgr._marryScene = new Scene(serviceSingle);
						result = true;
					}
				}
			}
			catch (Exception exception)
			{
				WorldMgr.log.Error("WordMgr Init", exception);
			}
			return result;
		}
		public static void SendToAll(GSPacketIn pkg)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				gamePlayer.SendTCP(pkg);
			}
		}
		public static GSPacketIn SendSysNotice(string msg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(10);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteString(msg);
			WorldMgr.SendToAll(gSPacketIn);
			return gSPacketIn;
		}
		public static bool AddPlayer(int playerId, GamePlayer player)
		{
			WorldMgr.m_clientLocker.AcquireWriterLock(15000);
			try
			{
				if (WorldMgr.m_players.ContainsKey(playerId))
				{
					return false;
				}
				WorldMgr.m_players.Add(playerId, player);
			}
			finally
			{
				WorldMgr.m_clientLocker.ReleaseWriterLock();
			}
			return true;
		}
		public static bool RemovePlayer(int playerId)
		{
			WorldMgr.m_clientLocker.AcquireWriterLock(15000);
			GamePlayer gamePlayer = null;
			try
			{
				if (WorldMgr.m_players.ContainsKey(playerId))
				{
					gamePlayer = WorldMgr.m_players[playerId];
					WorldMgr.m_players.Remove(playerId);
				}
			}
			finally
			{
				WorldMgr.m_clientLocker.ReleaseWriterLock();
			}
			if (gamePlayer == null)
			{
				return false;
			}
			GameServer.Instance.LoginServer.SendUserOffline(playerId, gamePlayer.PlayerCharacter.ConsortiaID);
			return true;
		}
		public static GamePlayer GetPlayerById(int playerId)
		{
			GamePlayer result = null;
			WorldMgr.m_clientLocker.AcquireReaderLock(15000);
			try
			{
				if (WorldMgr.m_players.ContainsKey(playerId))
				{
					result = WorldMgr.m_players[playerId];
				}
			}
			finally
			{
				WorldMgr.m_clientLocker.ReleaseReaderLock();
			}
			return result;
		}
		public static GamePlayer GetClientByPlayerNickName(string nickName)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.NickName == nickName)
				{
					return gamePlayer;
				}
			}
			return null;
		}
		public static GamePlayer[] GetAllPlayers()
		{
			List<GamePlayer> list = new List<GamePlayer>();
			WorldMgr.m_clientLocker.AcquireReaderLock(15000);
			try
			{
				foreach (GamePlayer current in WorldMgr.m_players.Values)
				{
					if (current != null && current.PlayerCharacter != null)
					{
						list.Add(current);
					}
				}
			}
			finally
			{
				WorldMgr.m_clientLocker.ReleaseReaderLock();
			}
			return list.ToArray();
		}
		public static GamePlayer[] GetAllPlayersNoGame()
		{
			List<GamePlayer> list = new List<GamePlayer>();
			WorldMgr.m_clientLocker.AcquireReaderLock(15000);
			try
			{
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				for (int i = 0; i < allPlayers.Length; i++)
				{
					GamePlayer gamePlayer = allPlayers[i];
					if (gamePlayer.CurrentRoom == null)
					{
						list.Add(gamePlayer);
					}
				}
			}
			finally
			{
				WorldMgr.m_clientLocker.ReleaseReaderLock();
			}
			return list.ToArray();
		}
		public static string GetPlayerStringByPlayerNickName(string nickName)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.NickName == nickName)
				{
					return gamePlayer.ToString();
				}
			}
			return nickName + " is not online!";
		}
		public static string DisconnectPlayerByName(string nickName)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer.PlayerCharacter.NickName == nickName)
				{
					gamePlayer.Disconnect();
					return "OK";
				}
			}
			return nickName + " is not online!";
		}
		public static void OnPlayerOffline(int playerid, int consortiaID)
		{
			WorldMgr.ChangePlayerState(playerid, 0, consortiaID);
		}
		public static void OnPlayerOnline(int playerid, int consortiaID)
		{
			WorldMgr.ChangePlayerState(playerid, 1, consortiaID);
		}
		public static void ChangePlayerState(int playerID, int state, int consortiaID)
		{
			GSPacketIn gSPacketIn = null;
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if ((gamePlayer.Friends != null && gamePlayer.Friends.ContainsKey(playerID) && gamePlayer.Friends[playerID] == 0) || (gamePlayer.PlayerCharacter.ConsortiaID != 0 && gamePlayer.PlayerCharacter.ConsortiaID == consortiaID))
				{
					if (gSPacketIn == null)
					{
						gSPacketIn = gamePlayer.Out.SendFriendState(playerID, state, gamePlayer.PlayerCharacter.typeVIP, gamePlayer.PlayerCharacter.VIPLevel);
					}
					else
					{
						gamePlayer.SendTCP(gSPacketIn);
					}
				}
			}
		}
	}
}
