using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Packets;
using System;
using System.Collections.Generic;
namespace Game.Server.Rooms
{
	public class StartGameAction : IAction
	{
		private BaseRoom m_room;
		public StartGameAction(BaseRoom room)
		{
			this.m_room = room;
		}
		public void Execute()
		{
			if (this.m_room.CanStart())
			{
				List<GamePlayer> players = this.m_room.GetPlayers();
				if (this.m_room.RoomType == eRoomType.Freedom)
				{
					List<IGamePlayer> list = new List<IGamePlayer>();
					List<IGamePlayer> list2 = new List<IGamePlayer>();
					foreach (GamePlayer current in players)
					{
						if (current != null)
						{
							if (current.CurrentRoomTeam == 1)
							{
								list.Add(current);
							}
							else
							{
								list2.Add(current);
							}
						}
					}
					BaseGame game = GameMgr.StartPVPGame(this.m_room.RoomId, list, list2, this.m_room.MapId, this.m_room.RoomType, this.m_room.GameType, (int)this.m_room.TimeMode);
					this.StartGame(game);
				}
				else
				{
					if (this.m_room.RoomType == eRoomType.Dungeon || this.m_room.RoomType == eRoomType.Freshman || this.m_room.RoomType == eRoomType.Lanbyrinth || this.m_room.RoomType == eRoomType.ConsortiaBoss || this.m_room.RoomType == eRoomType.AcademyDungeon || this.m_room.RoomType == eRoomType.FightLib || this.m_room.RoomType == eRoomType.WordBoss)
					{
						List<IGamePlayer> list3 = new List<IGamePlayer>();
						foreach (GamePlayer current2 in players)
						{
							if (current2 != null)
							{
								list3.Add(current2);
							}
						}
						this.UpdatePveRoomTimeMode();
						BaseGame game2 = GameMgr.StartPVEGame(this.m_room.RoomId, list3, this.m_room.MapId, this.m_room.RoomType, this.m_room.GameType, (int)this.m_room.TimeMode, this.m_room.HardLevel, this.m_room.LevelLimits);
						this.StartGame(game2);
					}
					else
					{
						if (this.m_room.RoomType == eRoomType.Match)
						{
							this.m_room.UpdateAvgLevel();
							BattleServer battleServer = BattleMgr.AddRoom(this.m_room);
							if (battleServer != null)
							{
								this.m_room.BattleServer = battleServer;
								this.m_room.IsPlaying = true;
								this.m_room.SendStartPickUp();
							}
							else
							{
								GSPacketIn pkg = this.m_room.Host.Out.SendMessage(eMessageType.ChatERROR, "Game đã bắt đầu!");
								this.m_room.SendToAll(pkg, this.m_room.Host);
								this.m_room.SendCancelPickUp();
							}
						}
					}
				}
				RoomMgr.WaitingRoom.SendUpdateRoom(this.m_room);
			}
		}
		private void StartGame(BaseGame game)
		{
			if (game != null)
			{
				this.m_room.IsPlaying = true;
				this.m_room.StartGame(game);
				return;
			}
			this.m_room.IsPlaying = false;
			this.m_room.SendPlayerState();
		}
		private void UpdatePveRoomTimeMode()
		{
			if (this.m_room.RoomType == eRoomType.Dungeon)
			{
				switch (this.m_room.HardLevel)
				{
				case eHardLevel.Simple:
					this.m_room.TimeMode = 3;
					return;

				case eHardLevel.Normal:
					this.m_room.TimeMode = 2;
					return;

				case eHardLevel.Hard:
					this.m_room.TimeMode = 1;
					return;

				case eHardLevel.Terror:
					this.m_room.TimeMode = 1;
					break;

				default:
					return;
				}
			}
		}
	}
}
