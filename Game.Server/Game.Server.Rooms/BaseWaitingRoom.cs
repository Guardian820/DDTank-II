using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.Rooms
{
	public class BaseWaitingRoom
	{
		private Dictionary<int, GamePlayer> m_list;
		public BaseWaitingRoom()
		{
			this.m_list = new Dictionary<int, GamePlayer>();
		}
		public bool AddPlayer(GamePlayer player)
		{
			bool flag = false;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				if (!this.m_list.ContainsKey(player.PlayerId))
				{
					this.m_list.Add(player.PlayerId, player);
					flag = true;
				}
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (flag)
			{
				GSPacketIn packet = player.Out.SendSceneAddPlayer(player);
				this.SendToALL(packet, player);
			}
			return flag;
		}
		public bool RemovePlayer(GamePlayer player)
		{
			bool flag = false;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				flag = this.m_list.Remove(player.PlayerId);
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (flag)
			{
				GSPacketIn packet = player.Out.SendSceneRemovePlayer(player);
				this.SendToALL(packet, player);
			}
			return true;
		}
		public void SendUpdateRoom(BaseRoom room)
		{
		}
		public void SendToALL(GSPacketIn packet)
		{
			this.SendToALL(packet, null);
		}
		public void SendToALL(GSPacketIn packet, GamePlayer except)
		{
			GamePlayer[] array = null;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				array = new GamePlayer[this.m_list.Count];
				this.m_list.Values.CopyTo(array, 0);
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (array != null)
			{
				GamePlayer[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					GamePlayer gamePlayer = array2[i];
					if (gamePlayer != null && gamePlayer != except)
					{
						gamePlayer.Out.SendTCP(packet);
					}
				}
			}
		}
		public GamePlayer[] GetPlayersSafe()
		{
			GamePlayer[] array = null;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				array = new GamePlayer[this.m_list.Count];
				this.m_list.Values.CopyTo(array, 0);
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (array != null)
			{
				return array;
			}
			return new GamePlayer[0];
		}
	}
}
