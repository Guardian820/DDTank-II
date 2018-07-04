using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace Center.Server
{
	public class LoginMgr
	{
		private static Dictionary<int, Player> m_players = new Dictionary<int, Player>();
		private static object syc_obj = new object();
		public static void CreatePlayer(Player player)
		{
			Player player2 = null;
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				player.LastTime = DateTime.Now.Ticks;
				if (LoginMgr.m_players.ContainsKey(player.Id))
				{
					player2 = LoginMgr.m_players[player.Id];
					player.State = player2.State;
					player.CurrentServer = player2.CurrentServer;
					LoginMgr.m_players[player.Id] = player;
				}
				else
				{
					player2 = LoginMgr.GetPlayerByName(player.Name);
					if (player2 != null && LoginMgr.m_players.ContainsKey(player2.Id))
					{
						LoginMgr.m_players.Remove(player2.Id);
					}
					player.State = ePlayerState.NotLogin;
					LoginMgr.m_players.Add(player.Id, player);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			if (player2 != null && player2.CurrentServer != null)
			{
				player2.CurrentServer.SendKitoffUser(player2.Id);
			}
		}
		public static bool TryLoginPlayer(int id, ServerClient server)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			bool result;
			try
			{
				if (LoginMgr.m_players.ContainsKey(id))
				{
					Player player = LoginMgr.m_players[id];
					if (player.CurrentServer == null)
					{
						player.CurrentServer = server;
						player.State = ePlayerState.Logining;
						result = true;
					}
					else
					{
						if (player.State == ePlayerState.Play)
						{
							player.CurrentServer.SendKitoffUser(id);
						}
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static void PlayerLogined(int id, ServerClient server)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				if (LoginMgr.m_players.ContainsKey(id))
				{
					Player player = LoginMgr.m_players[id];
					if (player != null)
					{
						player.CurrentServer = server;
						player.State = ePlayerState.Play;
					}
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static void PlayerLoginOut(int id, ServerClient server)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				if (LoginMgr.m_players.ContainsKey(id))
				{
					Player player = LoginMgr.m_players[id];
					if (player != null && player.CurrentServer == server)
					{
						player.CurrentServer = null;
						player.State = ePlayerState.NotLogin;
					}
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static Player GetPlayerByName(string name)
		{
			Player[] allPlayer = LoginMgr.GetAllPlayer();
			if (allPlayer != null)
			{
				Player[] array = allPlayer;
				for (int i = 0; i < array.Length; i++)
				{
					Player player = array[i];
					if (player.Name == name)
					{
						return player;
					}
				}
			}
			return null;
		}
		public static Player[] GetAllPlayer()
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			Player[] result;
			try
			{
				result = LoginMgr.m_players.Values.ToArray<Player>();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static void RemovePlayer(int playerId)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				if (LoginMgr.m_players.ContainsKey(playerId))
				{
					LoginMgr.m_players.Remove(playerId);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static void RemovePlayer(List<Player> players)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				foreach (Player current in players)
				{
					LoginMgr.m_players.Remove(current.Id);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static Player GetPlayer(int playerId)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				if (LoginMgr.m_players.ContainsKey(playerId))
				{
					return LoginMgr.m_players[playerId];
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return null;
		}
		public static ServerClient GetServerClient(int playerId)
		{
			Player player = LoginMgr.GetPlayer(playerId);
			if (player != null)
			{
				return player.CurrentServer;
			}
			return null;
		}
		public static int GetOnlineCount()
		{
			Player[] allPlayer = LoginMgr.GetAllPlayer();
			int num = 0;
			Player[] array = allPlayer;
			for (int i = 0; i < array.Length; i++)
			{
				Player player = array[i];
				if (player.State != ePlayerState.NotLogin)
				{
					num++;
				}
			}
			return num;
		}
		public static Dictionary<int, int> GetOnlineForLine()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Player[] allPlayer = LoginMgr.GetAllPlayer();
			Player[] array = allPlayer;
			for (int i = 0; i < array.Length; i++)
			{
				Player player = array[i];
				if (player.CurrentServer != null)
				{
					if (dictionary.ContainsKey(player.CurrentServer.Info.ID))
					{
						Dictionary<int, int> dictionary2;
						int iD;
						(dictionary2 = dictionary)[iD = player.CurrentServer.Info.ID] = dictionary2[iD] + 1;
					}
					else
					{
						dictionary.Add(player.CurrentServer.Info.ID, 1);
					}
				}
			}
			return dictionary;
		}
		public static List<Player> GetServerPlayers(ServerClient server)
		{
			List<Player> list = new List<Player>();
			Player[] allPlayer = LoginMgr.GetAllPlayer();
			Player[] array = allPlayer;
			for (int i = 0; i < array.Length; i++)
			{
				Player player = array[i];
				if (player.CurrentServer == server)
				{
					list.Add(player);
				}
			}
			return list;
		}
	}
}
