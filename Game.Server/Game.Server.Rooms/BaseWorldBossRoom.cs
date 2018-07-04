using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.Rooms
{
	public class BaseWorldBossRoom
	{
		private Dictionary<int, GamePlayer> m_list;
		public BaseWorldBossRoom()
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
					player.IsInWorldBossRoom = true;
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
				GSPacketIn gSPacketIn = new GSPacketIn(102);
				gSPacketIn.WriteByte(3);
				gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
				gSPacketIn.WriteInt(player.PlayerCharacter.Hide);
				gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
				gSPacketIn.WriteInt(player.PlayerCharacter.ID);
				gSPacketIn.WriteString(player.PlayerCharacter.NickName);
				gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
				gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
				gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
				gSPacketIn.WriteString(player.PlayerCharacter.Style);
				gSPacketIn.WriteString(player.PlayerCharacter.Colors);
				gSPacketIn.WriteString(player.PlayerCharacter.Skin);
				gSPacketIn.WriteInt(player.WorldBossX);
				gSPacketIn.WriteInt(player.WorldBossY);
				gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
				gSPacketIn.WriteInt(player.PlayerCharacter.Win);
				gSPacketIn.WriteInt(player.PlayerCharacter.Total);
				gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
				gSPacketIn.WriteByte(player.WorldBossState);
				player.SendTCP(gSPacketIn);
				this.SendToALL(gSPacketIn, player);
			}
			return flag;
		}
		public void UpdateRoom(GamePlayer player)
		{
			GamePlayer[] playersSafe = this.GetPlayersSafe();
			GamePlayer[] array = playersSafe;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer gamePlayer = array[i];
				if (gamePlayer != player)
				{
					GSPacketIn gSPacketIn = new GSPacketIn(102);
					gSPacketIn.WriteByte(3);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Grade);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Hide);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Repute);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ID);
					gSPacketIn.WriteString(gamePlayer.PlayerCharacter.NickName);
					gSPacketIn.WriteByte(gamePlayer.PlayerCharacter.typeVIP);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.VIPLevel);
					gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.Sex);
					gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Style);
					gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Colors);
					gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Skin);
					gSPacketIn.WriteInt(gamePlayer.WorldBossX);
					gSPacketIn.WriteInt(gamePlayer.WorldBossY);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.FightPower);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Win);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Total);
					gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Offer);
					gSPacketIn.WriteByte(gamePlayer.WorldBossState);
					player.SendTCP(gSPacketIn);
				}
			}
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
	}
}
