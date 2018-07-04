using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Server.GameUtils
{
	public class Scene
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected ReaderWriterLock _locker = new ReaderWriterLock();
		protected Dictionary<int, GamePlayer> _players = new Dictionary<int, GamePlayer>();
		public Scene(ServerInfo info)
		{
		}
		public bool AddPlayer(GamePlayer player)
		{
			this._locker.AcquireWriterLock(15000);
			bool result;
			try
			{
				if (this._players.ContainsKey(player.PlayerCharacter.ID))
				{
					this._players[player.PlayerCharacter.ID] = player;
					result = true;
				}
				else
				{
					this._players.Add(player.PlayerCharacter.ID, player);
					result = true;
				}
			}
			finally
			{
				this._locker.ReleaseWriterLock();
			}
			return result;
		}
		public void RemovePlayer(GamePlayer player)
		{
			this._locker.AcquireWriterLock(15000);
			try
			{
				if (this._players.ContainsKey(player.PlayerCharacter.ID))
				{
					this._players.Remove(player.PlayerCharacter.ID);
				}
			}
			finally
			{
				this._locker.ReleaseWriterLock();
			}
			GamePlayer[] allPlayer = this.GetAllPlayer();
			GSPacketIn gSPacketIn = null;
			GamePlayer[] array = allPlayer;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gSPacketIn == null)
				{
					gSPacketIn = gamePlayer.Out.SendSceneRemovePlayer(player);
				}
				else
				{
					gamePlayer.Out.SendTCP(gSPacketIn);
				}
			}
		}
		public GamePlayer[] GetAllPlayer()
		{
			GamePlayer[] array = null;
			this._locker.AcquireReaderLock(15000);
			try
			{
				array = this._players.Values.ToArray<GamePlayer>();
			}
			finally
			{
				this._locker.ReleaseReaderLock();
			}
			if (array != null)
			{
				return array;
			}
			return new GamePlayer[0];
		}
		public GamePlayer GetClientFromID(int id)
		{
			if (this._players.Keys.Contains(id))
			{
				return this._players[id];
			}
			return null;
		}
		public void SendToALL(GSPacketIn pkg)
		{
			this.SendToALL(pkg, null);
		}
		public void SendToALL(GSPacketIn pkg, GamePlayer except)
		{
			GamePlayer[] allPlayer = this.GetAllPlayer();
			GamePlayer[] array = allPlayer;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer != except)
				{
					gamePlayer.Out.SendTCP(pkg);
				}
			}
		}
	}
}
