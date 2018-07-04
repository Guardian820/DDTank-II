using Bussiness;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Logic
{
	public class PVPGame : BaseGame
	{
		private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static readonly int Money_Rate = int.Parse(ConfigurationManager.AppSettings["Money_Rate"]);
		private List<Player> m_redTeam;
		private float m_redAvgLevel;
		private List<Player> m_blueTeam;
		private float m_blueAvgLevel;
		private int BeginPlayerCount;
		private string teamAStr;
		private string teamBStr;
		private DateTime beginTime;
		public Player CurrentPlayer
		{
			get
			{
				return this.m_currentLiving as Player;
			}
		}
		public PVPGame(int id, int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, Map map, eRoomType roomType, eGameType gameType, int timeType, List<PetSkillElementInfo> GameNeedPetSkillInfoList) : base(id, roomId, map, roomType, gameType, timeType, GameNeedPetSkillInfoList)
		{
			this.m_redTeam = new List<Player>();
			this.m_blueTeam = new List<Player>();
			StringBuilder stringBuilder = new StringBuilder();
			this.m_redAvgLevel = 0f;
			foreach (IGamePlayer current in red)
			{
				Player player = new Player(current, this.PhysicalId++, this, 1, current.PlayerCharacter.hp);
				stringBuilder.Append(current.PlayerCharacter.ID).Append(",");
				player.Reset();
				player.Direction = ((this.m_random.Next(0, 1) == 0) ? 1 : -1);
				base.AddPlayer(current, player);
				this.m_redTeam.Add(player);
				this.m_redAvgLevel += (float)current.PlayerCharacter.Grade;
			}
			this.m_redAvgLevel /= (float)this.m_redTeam.Count;
			this.teamAStr = stringBuilder.ToString();
			StringBuilder stringBuilder2 = new StringBuilder();
			this.m_blueAvgLevel = 0f;
			foreach (IGamePlayer current2 in blue)
			{
				Player player2 = new Player(current2, this.PhysicalId++, this, 2, current2.PlayerCharacter.hp);
				stringBuilder2.Append(current2.PlayerCharacter.ID).Append(",");
				player2.Reset();
				player2.Direction = ((this.m_random.Next(0, 1) == 0) ? 1 : -1);
				base.AddPlayer(current2, player2);
				this.m_blueTeam.Add(player2);
				this.m_blueAvgLevel += (float)current2.PlayerCharacter.Grade;
			}
			this.m_blueAvgLevel /= (float)blue.Count;
			this.teamBStr = stringBuilder2.ToString();
			this.BeginPlayerCount = this.m_redTeam.Count + this.m_blueTeam.Count;
			this.beginTime = DateTime.Now;
		}
		public void Prepare()
		{
			if (base.GameState == eGameState.Inited)
			{
				base.SendCreateGame();
				this.m_gameState = eGameState.Prepared;
				this.CheckState(0);
			}
		}
		public void StartLoading()
		{
			if (base.GameState == eGameState.Prepared)
			{
				base.ClearWaitTimer();
				base.SendStartLoading(60);
				base.VaneLoading();
				base.AddAction(new WaitPlayerLoadingAction(this, 61000));
				this.m_gameState = eGameState.Loading;
			}
		}
		public void StartGame()
		{
			if (base.GameState == eGameState.Loading)
			{
				this.m_gameState = eGameState.Playing;
				base.ClearWaitTimer();
				base.SendSyncLifeTime();
				List<Player> allFightPlayers = base.GetAllFightPlayers();
				MapPoint mapRandomPos = MapMgr.GetMapRandomPos(this.m_map.Info.ID);
				GSPacketIn gSPacketIn = new GSPacketIn(91);
				gSPacketIn.WriteByte(99);
				gSPacketIn.WriteInt(allFightPlayers.Count);
				foreach (Player current in allFightPlayers)
				{
					current.Reset();
					Point playerPoint = base.GetPlayerPoint(mapRandomPos, current.Team);
					current.SetXY(playerPoint);
					this.m_map.AddPhysical(current);
					current.StartMoving();
					current.StartGame();
					gSPacketIn.WriteInt(current.Id);
					gSPacketIn.WriteInt(current.X);
					gSPacketIn.WriteInt(current.Y);
					gSPacketIn.WriteInt(current.Direction);
					gSPacketIn.WriteInt(current.Blood);
					gSPacketIn.WriteInt(current.Team);
					gSPacketIn.WriteInt(current.Weapon.RefineryLevel);
					gSPacketIn.WriteInt(34);
					gSPacketIn.WriteInt(current.Dander);
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(102);
					gSPacketIn.WriteInt(1);
				}
				this.SendToAll(gSPacketIn);
				base.VaneLoading();
				base.WaitTime(allFightPlayers.Count * 1000);
				base.OnGameStarted();
			}
		}
		public void NextTurn()
		{
			if (base.GameState == eGameState.Playing)
			{
				base.ClearWaitTimer();
				base.ClearDiedPhysicals();
				base.CheckBox();
				base.m_turnIndex++;
				List<Box> newBoxes = base.CreateBox();
				List<Physics> allPhysicalSafe = this.m_map.GetAllPhysicalSafe();
				foreach (Physics current in allPhysicalSafe)
				{
					current.PrepareNewTurn();
				}
				this.m_currentLiving = base.FindNextTurnedLiving();
				if (this.m_currentLiving.vaneOpen)
				{
					base.UpdateWind(base.GetNextWind(), false);
				}
				this.MinusDelays(this.m_currentLiving.Delay);
				this.m_currentLiving.PrepareSelfTurn();
				if (!base.CurrentLiving.IsFrost && this.m_currentLiving.IsLiving)
				{
					this.m_currentLiving.StartAttacking();
					base.SendGameNextTurn(this.m_currentLiving, this, newBoxes);
					if (this.m_currentLiving.IsAttacking)
					{
						base.AddAction(new WaitLivingAttackingAction(this.m_currentLiving, base.m_turnIndex, (this.m_timeType + 20) * 1000));
					}
				}
				base.OnBeginNewTurn();
			}
		}
		public override bool TakeCard(Player player)
		{
			int index = 0;
			for (int i = 0; i < this.Cards.Length; i++)
			{
				if (this.Cards[i] == 0)
				{
					index = i;
					break;
				}
			}
			return this.TakeCard(player, index);
		}
		public override bool TakeCard(Player player, int index)
		{
			if (player.CanTakeOut == 0 || !player.IsActive || index < 0 || index > this.Cards.Length || player.FinishTakeCard || this.Cards[index] > 0)
			{
				return false;
			}
			player.CanTakeOut--;
			int value = 0;
			int num = 0;
			int value2 = 0;
			int num2 = 0;
			int num3 = 0;
			int count = 0;
			List<ItemInfo> list = null;
			if (DropInventory.CardDrop(base.RoomType, ref list) && list != null)
			{
				foreach (ItemInfo current in list)
				{
					num3 = current.TemplateID;
					count = current.Count;
					ItemInfo.FindSpecialItemInfo(current, ref value, ref num, ref value2, ref num2);
					if (num3 > 0)
					{
						player.PlayerDetail.AddTemplate(current, current.Template.BagType, current.Count);
					}
				}
			}
			player.FinishTakeCard = true;
			this.Cards[index] = 1;
			player.PlayerDetail.AddGold(value);
			player.PlayerDetail.AddMoney(num);
			player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_TakeCard, player.PlayerDetail.PlayerCharacter.ID, num, player.PlayerDetail.PlayerCharacter.Money);
			player.PlayerDetail.AddGiftToken(value2);
			base.SendGamePlayerTakeCard(player, index, num3, count, false);
			return true;
		}
		public void GameOver()
		{
			if (base.GameState == eGameState.Playing)
			{
				this.m_gameState = eGameState.GameOver;
				base.ClearWaitTimer();
				this.CurrentTurnTotalDamage = 0;
				List<Player> allFightPlayers = base.GetAllFightPlayers();
				int num = -1;
				foreach (Player current in allFightPlayers)
				{
					if (current.IsLiving)
					{
						num = current.Team;
						break;
					}
				}
				if (num == -1 && this.CurrentPlayer != null)
				{
					num = this.CurrentPlayer.Team;
				}
				int num2 = this.CalculateGuildMatchResult(allFightPlayers, num);
				if (base.RoomType == eRoomType.Match)
				{
					if (base.GameType == eGameType.Guild)
					{
						int num3 = 10;
						int num4 = -10;
						num3 += allFightPlayers.Count / 2;
						num4 += (int)Math.Round((double)(allFightPlayers.Count / 2) * 0.5);
					}
				}
				int num5 = 0;
				int num6 = 0;
				foreach (Player current2 in allFightPlayers)
				{
					if (current2.TotalHurt > 0)
					{
						if (current2.Team == 1)
						{
							num6 = 1;
						}
						else
						{
							num5 = 1;
						}
					}
				}
				GSPacketIn gSPacketIn = new GSPacketIn(91);
				gSPacketIn.WriteByte(100);
				gSPacketIn.WriteInt(base.PlayerCount);
				foreach (Player current3 in allFightPlayers)
				{
					float num7 = (current3.Team == 1) ? this.m_blueAvgLevel : this.m_redAvgLevel;
					float num8 = (float)((current3.Team == 1) ? this.m_blueTeam.Count : this.m_redTeam.Count);
					float num9 = Math.Abs(num7 - (float)current3.PlayerDetail.PlayerCharacter.Grade);
					float num10 = (float)((current3.Team == num) ? 2 : 0);
					int num11 = 0;
					int num12 = (current3.TotalShootCount == 0) ? 1 : current3.TotalShootCount;
					if (this.m_roomType == eRoomType.Match || num9 < 5f)
					{
						num11 = (int)Math.Ceiling(((double)num10 + (double)current3.TotalHurt * 0.001 + (double)current3.TotalKill * 0.5 + (double)(current3.TotalHitTargetCount / num12 * 2)) * (double)num7 * (0.9 + (double)(num8 - 1f) * 0.3));
					}
					num11 = ((num11 == 0) ? 1 : num11);
					current3.CanTakeOut = ((current3.Team == 1) ? num6 : num5);
					num2 += current3.GainOffer;
					if (base.RoomType != eRoomType.Freedom)
					{
						new Random();
						int num13;
						if (num10 == 2f)
						{
							num13 = 300;
						}
						else
						{
							num13 = 50;
						}
						string a = DateTime.Now.ToString("HH");
						int num14 = 0;
						if (a == "5" || a == "9" || a == "13")
						{
							num14 = 500;
						}
						else
						{
							if (a == "21" || a == "23")
							{
								num14 = 2000;
							}
							else
							{
								if (a == "0" || a == "24")
								{
									num14 = num11;
								}
							}
						}
						string text = string.Concat(new object[]
						{
							"Bạn nhận được ",
							num11,
							" kinh nghiệm và ",
							num13,
							" Xu."
						});
						if (num14 > 0)
						{
							object obj = text;
							text = string.Concat(new object[]
							{
								obj,
								" Bạn online vào giờ đặc biệt nên được cộng thêm ",
								num14,
								" kinh nghiệm. Chúc bạn chơi game vui vẻ."
							});
							num11 += num14;
						}
						current3.PlayerDetail.AddMoney(num13);
						current3.PlayerDetail.SendMessage(text);
					}
					current3.GainGP = current3.PlayerDetail.AddGP(num11);
					current3.GainOffer = current3.PlayerDetail.AddOffer(num2);
					gSPacketIn.WriteInt(current3.Id);
					gSPacketIn.WriteBoolean(current3.Team == num);
					gSPacketIn.WriteInt(current3.Grade);
					gSPacketIn.WriteInt(current3.PlayerDetail.PlayerCharacter.GP);
					gSPacketIn.WriteInt(current3.TotalKill);
					gSPacketIn.WriteInt(current3.TotalHurt);
					gSPacketIn.WriteInt(current3.TotalShootCount);
					gSPacketIn.WriteInt(current3.TotalCure);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(current3.GainGP);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(current3.GainOffer);
					gSPacketIn.WriteInt(current3.CanTakeOut);
				}
				gSPacketIn.WriteInt(num2);
				this.SendToAll(gSPacketIn);
				new StringBuilder();
				foreach (Player current4 in allFightPlayers)
				{
					current4.PlayerDetail.OnGameOver(this, current4.Team == num, current4.GainGP);
				}
				string playResult = "";
				base.OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, this.beginTime, DateTime.Now, this.BeginPlayerCount, base.Map.Info.ID, this.teamAStr, this.teamBStr, playResult, num, this.BossWarField);
				base.WaitTime(15000);
				base.OnGameOverred();
			}
		}
		public override void Stop()
		{
			if (base.GameState == eGameState.GameOver)
			{
				this.m_gameState = eGameState.Stopped;
				List<Player> allFightPlayers = base.GetAllFightPlayers();
				foreach (Player current in allFightPlayers)
				{
					if (current.IsActive && !current.FinishTakeCard && current.CanTakeOut > 0)
					{
						this.TakeCard(current);
					}
				}
				Dictionary<int, Player> players;
				Monitor.Enter(players = this.m_players);
				try
				{
					this.m_players.Clear();
				}
				finally
				{
					Monitor.Exit(players);
				}
				base.Stop();
			}
		}
		private int CalculateGuildMatchResult(List<Player> players, int winTeam)
		{
			if (base.RoomType == eRoomType.Match)
			{
				StringBuilder stringBuilder = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5", new object[0]));
				StringBuilder stringBuilder2 = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5", new object[0]));
				IGamePlayer gamePlayer = null;
				IGamePlayer gamePlayer2 = null;
				int num = 0;
				foreach (Player current in players)
				{
					if (current.Team == winTeam)
					{
						stringBuilder.Append(string.Format("[{0}]", current.PlayerDetail.PlayerCharacter.NickName));
						gamePlayer = current.PlayerDetail;
					}
					else
					{
						stringBuilder2.Append(string.Format("{0}", current.PlayerDetail.PlayerCharacter.NickName));
						gamePlayer2 = current.PlayerDetail;
						num++;
					}
				}
				if (gamePlayer2 != null)
				{
					stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1", new object[0]) + gamePlayer2.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2", new object[0]));
					stringBuilder2.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg3", new object[0]) + gamePlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg4", new object[0]));
					int num2 = 0;
					if (base.GameType == eGameType.Guild)
					{
						num2 = num + this.TotalHurt / 2000;
					}
					gamePlayer.ConsortiaFight(gamePlayer.PlayerCharacter.ConsortiaID, gamePlayer2.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, this.TotalHurt, players.Count);
					if (gamePlayer.ServerID != gamePlayer2.ServerID)
					{
						gamePlayer2.ConsortiaFight(gamePlayer.PlayerCharacter.ConsortiaID, gamePlayer2.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, this.TotalHurt, players.Count);
					}
					if (base.GameType == eGameType.Guild)
					{
						gamePlayer.SendConsortiaFight(gamePlayer.PlayerCharacter.ConsortiaID, num2, stringBuilder.ToString());
					}
					return num2;
				}
			}
			return 0;
		}
		public bool CanGameOver()
		{
			bool flag = true;
			bool flag2 = true;
			foreach (Player current in this.m_redTeam)
			{
				if (current.IsLiving)
				{
					flag = false;
					break;
				}
			}
			foreach (Player current2 in this.m_blueTeam)
			{
				if (current2.IsLiving)
				{
					flag2 = false;
					break;
				}
			}
			return flag || flag2;
		}
		public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
		{
			Player player = base.RemovePlayer(gp, IsKick);
			if (player != null && player.IsLiving && base.GameState != eGameState.Loading)
			{
				gp.RemoveGP(gp.PlayerCharacter.Grade * 12);
				string msg = null;
				string msg2 = null;
				if (base.RoomType == eRoomType.Match)
				{
					if (base.GameType == eGameType.Guild)
					{
						msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[]
						{
							gp.PlayerCharacter.Grade * 12,
							15
						});
						gp.RemoveOffer(15);
						msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[]
						{
							gp.PlayerCharacter.NickName,
							gp.PlayerCharacter.Grade * 12,
							15
						});
					}
					else
					{
						if (base.GameType == eGameType.Free)
						{
							msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[]
							{
								gp.PlayerCharacter.Grade * 12,
								5
							});
							gp.RemoveOffer(5);
							msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[]
							{
								gp.PlayerCharacter.NickName,
								gp.PlayerCharacter.Grade * 12,
								5
							});
						}
					}
				}
				else
				{
					msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", new object[]
					{
						gp.PlayerCharacter.Grade * 12
					});
					msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", new object[]
					{
						gp.PlayerCharacter.NickName,
						gp.PlayerCharacter.Grade * 12
					});
				}
				base.SendMessage(gp, msg, msg2, 3);
				if (base.GetSameTeam())
				{
					base.CurrentLiving.StopAttacking();
					this.CheckState(0);
				}
			}
			return player;
		}
		public override void CheckState(int delay)
		{
			base.AddAction(new CheckPVPGameStateAction(delay));
		}
	}
}
