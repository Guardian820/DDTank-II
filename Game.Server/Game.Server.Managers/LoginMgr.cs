using Bussiness;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.Managers
{
	public class LoginMgr
	{
		private static Dictionary<int, GameClient> _players = new Dictionary<int, GameClient>();
		private static object _locker = new object();
		public static void Add(int player, GameClient server)
		{
			GameClient gameClient = null;
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			try
			{
				if (LoginMgr._players.ContainsKey(player))
				{
					GameClient gameClient2 = LoginMgr._players[player];
					if (gameClient2 != null)
					{
						gameClient = gameClient2;
					}
					LoginMgr._players[player] = server;
				}
				else
				{
					LoginMgr._players.Add(player, server);
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
			if (gameClient != null)
			{
				gameClient.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext", new object[0]));
				gameClient.Disconnect();
			}
		}
		public static void Remove(int player)
		{
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			try
			{
				if (LoginMgr._players.ContainsKey(player))
				{
					LoginMgr._players.Remove(player);
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
		}
		public static GamePlayer LoginClient(int playerId)
		{
			GameClient gameClient = null;
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			try
			{
				if (LoginMgr._players.ContainsKey(playerId))
				{
					gameClient = LoginMgr._players[playerId];
					LoginMgr._players.Remove(playerId);
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
			if (gameClient != null)
			{
				return gameClient.Player;
			}
			return null;
		}
		public static void ClearLoginPlayer(int playerId)
		{
			GameClient gameClient = null;
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			try
			{
				if (LoginMgr._players.ContainsKey(playerId))
				{
					gameClient = LoginMgr._players[playerId];
					LoginMgr._players.Remove(playerId);
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
			if (gameClient != null)
			{
				gameClient.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext", new object[0]));
				gameClient.Disconnect();
			}
		}
		public static void ClearLoginPlayer(int playerId, GameClient client)
		{
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			try
			{
				if (LoginMgr._players.ContainsKey(playerId) && LoginMgr._players[playerId] == client)
				{
					LoginMgr._players.Remove(playerId);
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
		}
		public static bool ContainsUser(int playerId)
		{
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			bool result;
			try
			{
				if (LoginMgr._players.ContainsKey(playerId) && LoginMgr._players[playerId].IsConnected)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
			return result;
		}
		public static bool ContainsUser(string account)
		{
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			bool result;
			try
			{
				foreach (GameClient current in LoginMgr._players.Values)
				{
					if (current != null && current.Player != null && current.Player.Account == account)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			finally
			{
				Monitor.Exit(locker);
			}
			return result;
		}
	}
}
