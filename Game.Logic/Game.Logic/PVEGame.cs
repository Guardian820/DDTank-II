using Bussiness;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.AI.Game;
using Game.Logic.AI.Mission;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Logic
{
	public class PVEGame : BaseGame
	{
		private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private APVEGameControl m_gameAI;
		private AMissionControl m_missionAI;
		public int SessionId;
		public bool IsWin;
		public int TotalMissionCount;
		public int TotalCount;
		public int TotalTurn;
		public int Param1;
		public int Param2;
		public int Param3;
		public int Param4;
		public int TotalKillCount;
		public double TotalNpcExperience;
		public double TotalNpcGrade;
		private int BeginPlayersCount;
		private PveInfo m_info;
		private List<string> m_gameOverResources;
		public Dictionary<int, MissionInfo> Misssions;
		private MapPoint mapPos;
		public int WantTryAgain;
		private eHardLevel m_hardLevel;
		private DateTime beginTime;
		private string m_IsBossType;
		private MissionInfo m_missionInfo;
		private List<int> m_mapHistoryIds;
		public int[] BossCards;
		private int m_bossCardCount;
		private int m_pveGameDelay;
		public MissionInfo MissionInfo
		{
			get
			{
				return this.m_missionInfo;
			}
			set
			{
				this.m_missionInfo = value;
			}
		}
		public Player CurrentPlayer
		{
			get
			{
				return this.m_currentLiving as Player;
			}
		}
		public TurnedLiving CurrentTurnLiving
		{
			get
			{
				return this.m_currentLiving;
			}
		}
		public List<int> MapHistoryIds
		{
			get
			{
				return this.m_mapHistoryIds;
			}
			set
			{
				this.m_mapHistoryIds = value;
			}
		}
		public eHardLevel HandLevel
		{
			get
			{
				return this.m_hardLevel;
			}
		}
		public MapPoint MapPos
		{
			get
			{
				return this.mapPos;
			}
		}
		public string IsBossWar
		{
			get
			{
				return this.m_IsBossType;
			}
			set
			{
				this.m_IsBossType = value;
			}
		}
		public List<string> GameOverResources
		{
			get
			{
				return this.m_gameOverResources;
			}
		}
		public int BossCardCount
		{
			get
			{
				return this.m_bossCardCount;
			}
			set
			{
				if (value > 0)
				{
					this.BossCards = new int[9];
					this.m_bossCardCount = value;
				}
			}
		}
		public int PveGameDelay
		{
			get
			{
				return this.m_pveGameDelay;
			}
			set
			{
				this.m_pveGameDelay = value;
			}
		}
		public PVEGame(int id, int roomId, PveInfo info, List<IGamePlayer> players, Map map, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel, List<PetSkillElementInfo> GameNeedPetSkillInfoList) : base(id, roomId, map, roomType, gameType, timeType, GameNeedPetSkillInfoList)
		{
			foreach (IGamePlayer current in players)
			{
				base.AddPlayer(current, new Player(current, this.PhysicalId++, this, 1, current.PlayerCharacter.hp)
				{
					Direction = (this.m_random.Next(0, 1) == 0) ? 1 : -1
				});
			}
			this.m_info = info;
			this.BeginPlayersCount = players.Count;
			this.TotalKillCount = 0;
			this.TotalNpcGrade = 0.0;
			this.TotalNpcExperience = 0.0;
			this.TotalHurt = 0;
			this.m_IsBossType = "";
			this.WantTryAgain = 0;
			this.SessionId = 0;
			this.m_gameOverResources = new List<string>();
			this.Misssions = new Dictionary<int, MissionInfo>();
			this.m_mapHistoryIds = new List<int>();
			this.m_hardLevel = hardLevel;
			string script = this.GetScript(info, hardLevel);
			this.m_gameAI = (ScriptMgr.CreateInstance(script) as APVEGameControl);
			if (this.m_gameAI == null)
			{
				PVEGame.log.ErrorFormat("Can't create game ai :{0}", script);
				this.m_gameAI = SimplePVEGameControl.Simple;
			}
			this.m_gameAI.Game = this;
			this.m_gameAI.OnCreated();
			this.m_missionAI = SimpleMissionControl.Simple;
			this.beginTime = DateTime.Now;
			this.m_bossCardCount = 0;
		}
		private string GetScript(PveInfo pveInfo, eHardLevel hardLevel)
		{
			string result = string.Empty;
			switch (hardLevel)
			{
			case eHardLevel.Simple:
				result = pveInfo.SimpleGameScript;
				break;

			case eHardLevel.Normal:
				result = pveInfo.NormalGameScript;
				break;

			case eHardLevel.Hard:
				result = pveInfo.HardGameScript;
				break;

			case eHardLevel.Terror:
				result = pveInfo.TerrorGameScript;
				break;

			default:
				result = pveInfo.SimpleGameScript;
				break;
			}
			return result;
		}
		public string GetMissionIdStr(string missionIds, int randomCount)
		{
			if (string.IsNullOrEmpty(missionIds))
			{
				return "";
			}
			string[] array = missionIds.Split(new char[]
			{
				','
			});
			if (array.Length < randomCount)
			{
				return "";
			}
			List<string> list = new List<string>();
			int maxValue = array.Length;
			int i = 0;
			while (i < randomCount)
			{
				int num = base.Random.Next(maxValue);
				string item = array[num];
				if (!list.Contains(item))
				{
					list.Add(item);
					i++;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string current in list)
			{
				stringBuilder.Append(current).Append(",");
			}
			return stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString();
		}
		public void SetupMissions(string missionIds)
		{
			if (string.IsNullOrEmpty(missionIds))
			{
				return;
			}
			int num = 0;
			string[] array = missionIds.Split(new char[]
			{
				','
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string s = array2[i];
				num++;
				MissionInfo missionInfo = MissionInfoMgr.GetMissionInfo(int.Parse(s));
				this.Misssions.Add(num, missionInfo);
			}
		}
        public SimpleNpc CreateNpc(int npcId, int x, int y, int direction, int type)
		{
			NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleNpc simpleNpc = new SimpleNpc(this.PhysicalId++, this, npcInfoById, direction, type);
			simpleNpc.Reset();
			simpleNpc.SetXY(x, y);
            if (type == 5)
            {
                AddBoss(simpleNpc);
            }
            else
            {
                AddLiving(simpleNpc);
            }
			simpleNpc.StartMoving();
			return simpleNpc;
		}
        public SimpleNpc CreateNpc(int npcId, int direction, int type)
		{
			NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleNpc simpleNpc = new SimpleNpc(this.PhysicalId++, this, npcInfoById, direction, type);
			Point playerPoint = base.GetPlayerPoint(this.mapPos, npcInfoById.Camp);
			simpleNpc.Reset();
			simpleNpc.SetXY(playerPoint);
			this.AddLiving(simpleNpc);
			simpleNpc.StartMoving();
			return simpleNpc;
		}
		public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, string action)
		{
			NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
			SimpleBoss simpleBoss = new SimpleBoss(this.PhysicalId++, this, npcInfoById, direction, type, action);
			simpleBoss.Reset();
			simpleBoss.SetXY(x, y);
            if (type == 1154)
            {
                this.AddBoss(simpleBoss);
            }
            else
            {
                this.AddLiving(simpleBoss);
            }
			simpleBoss.StartMoving();
			return simpleBoss;
		}
		public Box CreateBox(int x, int y, string model, ItemInfo item)
		{
			Box box = new Box(this.PhysicalId++, model, item);
			box.SetXY(x, y);
			this.m_map.AddPhysical(box);
			base.AddBox(box, true);
			return box;
		}
		public PhysicalObj CreatePhysicalObj(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
		{
			PhysicalObj physicalObj = new PhysicalObj(this.PhysicalId++, name, model, defaultAction, scale, rotation);
			physicalObj.SetXY(x, y);
			this.AddPhysicalObj(physicalObj, true);
			return physicalObj;
		}
		public Layer Createlayer(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
		{
			Layer layer = new Layer(this.PhysicalId++, name, model, defaultAction, scale, rotation);
			layer.SetXY(x, y);
			this.AddPhysicalObj(layer, true);
			return layer;
		}
		public Layer CreateTip(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
		{
			Layer layer = new Layer(this.PhysicalId++, name, model, defaultAction, scale, rotation);
			layer.SetXY(x, y);
			this.AddPhysicalTip(layer, true);
			return layer;
		}
		public void ClearMissionData()
		{
			foreach (Living current in this.m_livings)
			{
				current.Dispose();
			}
			this.m_livings.Clear();
			List<TurnedLiving> list = new List<TurnedLiving>();
			foreach (TurnedLiving current2 in base.TurnQueue)
			{
				if (current2 is Player)
				{
					if (current2.IsLiving)
					{
						list.Add(current2);
					}
				}
				else
				{
					current2.Dispose();
				}
			}
			base.TurnQueue.Clear();
			foreach (TurnedLiving current3 in list)
			{
				base.TurnQueue.Add(current3);
			}
			if (this.m_map != null)
			{
				foreach (PhysicalObj current4 in this.m_map.GetAllPhysicalObjSafe())
				{
					current4.Dispose();
				}
			}
		}
		public void AddAllPlayerToTurn()
		{
			foreach (Player current in base.Players.Values)
			{
				base.TurnQueue.Add(current);
			}
		}
		public override void AddLiving(Living living)
		{
			base.AddLiving(living);
			living.Died += new LivingEventHandle(this.living_Died);
		}
		private void living_Died(Living living)
		{
			if (base.CurrentLiving != null && base.CurrentLiving is Player && !(living is Player) && living != base.CurrentLiving)
			{
				this.TotalKillCount++;
				this.TotalNpcExperience += (double)living.Experience;
				this.TotalNpcGrade += (double)living.Grade;
			}
		}
		public override void MissionStart(IGamePlayer host)
		{
			if (base.GameState == eGameState.SessionPrepared || base.GameState == eGameState.GameOver)
			{
				foreach (Player current in base.Players.Values)
				{
					current.Ready = true;
				}
				this.CheckState(0);
			}
		}
		public override bool CanAddPlayer()
		{
			Dictionary<int, Player> players;
			Monitor.Enter(players = this.m_players);
			bool result;
			try
			{
				result = (base.GameState == eGameState.SessionPrepared && this.m_players.Count < 4);
			}
			finally
			{
				Monitor.Exit(players);
			}
			return result;
		}
		public override Player AddPlayer(IGamePlayer gp)
		{
			if (this.CanAddPlayer())
			{
				Player player = new Player(gp, this.PhysicalId++, this, 1, gp.PlayerCharacter.hp);
				player.Direction = ((this.m_random.Next(0, 1) == 0) ? 1 : -1);
				base.AddPlayer(gp, player);
				this.SendCreateGameToSingle(this, gp);
				this.SendPlayerInfoInGame(this, gp, player);
				return player;
			}
			return null;
		}
		public override Player RemovePlayer(IGamePlayer gp, bool isKick)
		{
			Player player = base.GetPlayer(gp);
			if (player != null)
			{
				player.PlayerDetail.RemoveGP(gp.PlayerCharacter.Grade * 12);
				string msg = null;
				if (player.IsLiving && base.GameState == eGameState.Playing)
				{
					msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", new object[]
					{
						gp.PlayerCharacter.Grade * 12
					});
					string translation = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", new object[]
					{
						gp.PlayerCharacter.NickName,
						gp.PlayerCharacter.Grade * 12
					});
					base.SendMessage(gp, msg, translation, 3);
				}
				else
				{
					string translation = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg1", new object[]
					{
						gp.PlayerCharacter.NickName
					});
					base.SendMessage(gp, msg, translation, 3);
				}
				base.RemovePlayer(gp, isKick);
			}
			return player;
		}
		public void LoadResources(int[] npcIds)
		{
			if (npcIds == null || npcIds.Length == 0)
			{
				return;
			}
			for (int i = 0; i < npcIds.Length; i++)
			{
				int id = npcIds[i];
				NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(id);
				if (npcInfoById == null)
				{
					PVEGame.log.Error("LoadResources npcInfo resoure is not exits");
				}
				else
				{
					base.AddLoadingFile(2, npcInfoById.ResourcesPath, npcInfoById.ModelID);
				}
			}
		}
		public void LoadNpcGameOverResources(int[] npcIds)
		{
			if (npcIds == null || npcIds.Length == 0)
			{
				return;
			}
			for (int i = 0; i < npcIds.Length; i++)
			{
				int id = npcIds[i];
				NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(id);
				if (npcInfoById == null)
				{
					PVEGame.log.Error("LoadGameOverResources npcInfo resoure is not exits");
				}
				else
				{
					this.m_gameOverResources.Add(npcInfoById.ModelID);
				}
			}
		}
		public void Prepare()
		{
			if (base.GameState == eGameState.Inited)
			{
				this.m_gameState = eGameState.Prepared;
				base.SendCreateGame();
				this.CheckState(0);
				try
				{
					this.m_gameAI.OnPrepated();
				}
				catch (Exception arg)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
				}
			}
		}
		public void PrepareNewSession()
		{
			if (base.GameState == eGameState.Prepared || base.GameState == eGameState.GameOver || base.GameState == eGameState.ALLSessionStopped)
			{
				this.m_gameState = eGameState.SessionPrepared;
				this.SessionId++;
				base.ClearLoadingFiles();
				this.ClearMissionData();
				this.m_gameOverResources.Clear();
				this.WantTryAgain = 0;
				this.m_missionInfo = this.Misssions[this.SessionId];
				this.m_pveGameDelay = this.m_missionInfo.Delay;
				this.TotalCount = this.m_missionInfo.TotalCount;
				this.TotalTurn = this.m_missionInfo.TotalTurn;
				this.Param1 = this.m_missionInfo.Param1;
				this.Param2 = this.m_missionInfo.Param2;
				this.Param3 = -1;
				this.Param4 = -1;
				this.m_missionAI = (ScriptMgr.CreateInstance(this.m_missionInfo.Script) as AMissionControl);
				if (this.m_missionAI == null)
				{
					PVEGame.log.ErrorFormat("Can't create game mission ai :{0}", this.m_missionInfo.Script);
					this.m_missionAI = SimpleMissionControl.Simple;
				}
				this.IsBossWar = "";
				this.m_missionAI.Game = this;
				try
				{
					this.m_missionAI.OnPrepareNewSession();
				}
				catch (Exception arg)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
				}
			}
		}
		public bool CanStartNewSession()
		{
			return base.m_turnIndex == 0 || this.IsAllReady();
		}
		public bool IsAllReady()
		{
			foreach (Player current in base.Players.Values)
			{
				if (!current.Ready)
				{
					return false;
				}
			}
			return true;
		}
		public void StartLoading()
		{
			if (base.GameState == eGameState.SessionPrepared)
			{
				this.m_gameState = eGameState.Loading;
				base.m_turnIndex = 0;
				this.SendMissionInfo();
				base.SendStartLoading(60);
				base.VaneLoading();
				base.AddAction(new WaitPlayerLoadingAction(this, 61000));
			}
		}
		public void StartGameMovie()
		{
			if (base.GameState == eGameState.Loading)
			{
				try
				{
					this.m_missionAI.OnStartMovie();
				}
				catch (Exception arg)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
				}
			}
		}
		public void StartGame()
		{
			if (base.GameState == eGameState.Loading)
			{
				this.m_gameState = eGameState.GameStart;
				base.SendSyncLifeTime();
				this.TotalKillCount = 0;
				this.TotalNpcGrade = 0.0;
				this.TotalNpcExperience = 0.0;
				this.TotalHurt = 0;
				this.m_bossCardCount = 0;
				this.BossCards = null;
				List<Player> allFightPlayers = base.GetAllFightPlayers();
				this.mapPos = MapMgr.GetPVEMapRandomPos(this.m_map.Info.ID);
				GSPacketIn gSPacketIn = new GSPacketIn(91);
				gSPacketIn.WriteByte(99);
				gSPacketIn.WriteInt(allFightPlayers.Count);
				foreach (Player current in allFightPlayers)
				{
					if (!current.IsLiving)
					{
						this.AddLiving(current);
					}
					current.Reset();
					Point playerPoint = base.GetPlayerPoint(this.mapPos, current.Team);
					current.SetXY(playerPoint);
					this.m_map.AddPhysical(current);
					current.StartMoving();
					current.StartGame();
					gSPacketIn.WriteInt(current.Id);
					gSPacketIn.WriteInt(current.X);
					gSPacketIn.WriteInt(current.Y);
					if (playerPoint.X < 600)
					{
						current.Direction = 1;
					}
					else
					{
						current.Direction = -1;
					}
					gSPacketIn.WriteInt(current.Direction);
					gSPacketIn.WriteInt(current.Blood);
					gSPacketIn.WriteInt(current.Team);
					gSPacketIn.WriteInt(current.Weapon.RefineryLevel);
					gSPacketIn.WriteInt(34);
					gSPacketIn.WriteInt(current.Dander);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(215);
					gSPacketIn.WriteInt(1);
				}
				this.SendToAll(gSPacketIn);
				this.SendUpdateUiData();
				base.WaitTime(base.PlayerCount * 2500 + 1000);
				base.OnGameStarted();
			}
		}
		public void PrepareNewGame()
		{
			if (base.GameState == eGameState.GameStart)
			{
				this.m_gameState = eGameState.Playing;
				base.WaitTime(base.PlayerCount * 1000);
				try
				{
					this.m_missionAI.OnStartGame();
				}
				catch (Exception arg)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
				}
			}
		}
		public void NextTurn()
		{
			if (base.GameState == eGameState.Playing)
			{
				base.ClearWaitTimer();
				base.ClearDiedPhysicals();
				base.CheckBox();
				this.LivingRandSay();
				List<Physics> allPhysicalSafe = this.m_map.GetAllPhysicalSafe();
				foreach (Physics current in allPhysicalSafe)
				{
					current.PrepareNewTurn();
				}
				List<Box> newBoxes = base.CreateBox();
				this.m_currentLiving = base.FindNextTurnedLiving();
				try
				{
					this.m_missionAI.OnNewTurnStarted();
				}
				catch (Exception arg)
				{
					//PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
				}
				if (this.m_currentLiving != null)
				{
					base.m_turnIndex++;
					this.SendUpdateUiData();
					List<Living> livedLivings = base.GetLivedLivings();
					if (livedLivings.Count > 0 && this.m_currentLiving.Delay >= this.m_pveGameDelay)
					{
						Living living = null;
						this.MinusDelays(this.m_pveGameDelay);
						foreach (Living current2 in this.m_livings)
						{
							current2.PrepareSelfTurn();
							if (!current2.IsFrost)
							{
								living = current2;
								current2.StartAttacking();
							}
						}
						if (living != null)
						{
							base.SendGameNextTurn(living, this, newBoxes);
						}
						foreach (Living current3 in this.m_livings)
						{
							if (current3.IsAttacking)
							{
								current3.StopAttacking();
							}
						}
						this.m_pveGameDelay += this.MissionInfo.IncrementDelay;
						this.CheckState(0);
					}
					else
					{
						this.MinusDelays(this.m_currentLiving.Delay);
						base.UpdateWind(base.GetNextWind(), false);
						this.CurrentTurnTotalDamage = 0;
						this.m_currentLiving.PrepareSelfTurn();
						if (this.m_currentLiving.IsLiving && !this.m_currentLiving.IsFrost)
						{
							this.m_currentLiving.StartAttacking();
							base.SendGameNextTurn(this.m_currentLiving, this, newBoxes);
							if (this.m_currentLiving.IsAttacking)
							{
								base.AddAction(new WaitLivingAttackingAction(this.m_currentLiving, base.m_turnIndex, (this.m_timeType + 20) * 1000));
							}
						}
					}
				}
				base.OnBeginNewTurn();
				try
				{
					this.m_missionAI.OnBeginNewTurn();
				}
				catch (Exception arg2)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg2);
				}
			}
		}
		public void LivingRandSay()
		{
			if (this.m_livings == null || this.m_livings.Count == 0)
			{
				return;
			}
			int count = this.m_livings.Count;
			foreach (Living current in this.m_livings)
			{
				current.IsSay = false;
			}
			if (base.TurnIndex % 2 == 0)
			{
				return;
			}
			int num;
			if (count <= 5)
			{
				num = base.Random.Next(0, 2);
			}
			else
			{
				if (count > 5 && count <= 10)
				{
					num = base.Random.Next(1, 3);
				}
				else
				{
					num = base.Random.Next(1, 4);
				}
			}
			if (num > 0)
			{
				int i = 0;
				while (i < num)
				{
					int index = base.Random.Next(0, count);
					if (!this.m_livings[index].IsSay)
					{
						this.m_livings[index].IsSay = true;
						i++;
					}
				}
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
			if (player.CanTakeOut == 0)
			{
				return false;
			}
			if (!player.IsActive || index < 0 || index > this.Cards.Length || player.FinishTakeCard || this.Cards[index] > 0)
			{
				return false;
			}
			int value = 0;
			int num = 0;
			int value2 = 0;
			int num2 = 0;
			int templateID = 0;
			int count = 0;
			List<ItemInfo> list = null;
			if (DropInventory.CopyDrop(this.m_missionInfo.Id, 1, ref list) && list != null)
			{
				foreach (ItemInfo current in list)
				{
					ItemInfo.FindSpecialItemInfo(current, ref value, ref num, ref value2, ref num2);
					if (current != null)
					{
						templateID = current.TemplateID;
						count = current.Count;
						player.PlayerDetail.AddTemplate(current, eBageType.TempBag, current.Count);
					}
				}
				player.PlayerDetail.AddGold(value);
				player.PlayerDetail.AddMoney(num);
				player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_TakeCard, player.PlayerDetail.PlayerCharacter.ID, num, player.PlayerDetail.PlayerCharacter.Money);
				player.PlayerDetail.AddGiftToken(value2);
			}
			if (base.RoomType == eRoomType.Dungeon)
			{
				player.CanTakeOut--;
				if (player.CanTakeOut == 0)
				{
					player.FinishTakeCard = true;
				}
			}
			else
			{
				player.FinishTakeCard = true;
			}
			this.Cards[index] = 1;
			base.SendGamePlayerTakeCard(player, index, templateID, count, false);
			return true;
		}
		public bool CanGameOver()
		{
            if (base.PlayerCount == 0)
			{
				return true;
			}
            
			if (base.GetDiedPlayerCount() == base.PlayerCount)
			{
				this.IsWin = false;
				return true;
			}
			try
			{
				return this.m_missionAI.CanGameOver();
			}
			catch (Exception arg)
			{
				PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
			return true;
		}
		public void GameOver()
		{
           
            if (base.GameState == eGameState.Playing)
			{
				this.m_gameState = eGameState.GameOver;
				this.SendUpdateUiData();
				try
				{
					this.m_missionAI.OnGameOver();
				}
				catch (Exception arg)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
				}
				List<Player> allFightPlayers = base.GetAllFightPlayers();
				this.BossCardCount = 9;
				this.CurrentTurnTotalDamage = 0;
				bool val = false;
				bool flag = this.HasNextSession();
				if (!this.IsWin || !flag)
				{
					this.m_bossCardCount = 0;
				}
				if (this.IsWin && !flag)
				{
					this.m_bossCardCount = 21;
				}
                
				GSPacketIn gSPacketIn = new GSPacketIn(91);
				gSPacketIn.WriteByte(112);
				if (!flag && !base.isTrainer())
				{
					gSPacketIn.WriteInt(2);
				}
				else
				{
					gSPacketIn.WriteInt(1);
				}
				gSPacketIn.WriteBoolean(flag);
				if (flag)
				{
					gSPacketIn.WriteString("show" + (this.SessionId + 1) + ".jpg");
				}
				gSPacketIn.WriteBoolean(val);
				gSPacketIn.WriteInt(base.PlayerCount);
				foreach (Player current in allFightPlayers)
				{
					int gp = this.CalculateExperience(current);
					int num = this.CalculateScore(current);
					this.m_missionAI.CalculateScoreGrade(current.TotalAllScore);
					current.CanTakeOut = 1;
					if (current.CurrentIsHitTarget)
					{
						current.TotalHitTargetCount++;
					}
					this.CalculateHitRate(current.TotalHitTargetCount, current.TotalShootCount);
					current.TotalAllHurt += current.TotalHurt;
					current.TotalAllCure += current.TotalCure;
					current.TotalAllHitTargetCount += current.TotalHitTargetCount;
					current.TotalAllShootCount += current.TotalShootCount;
					current.GainGP = current.PlayerDetail.AddGP(gp);
					current.TotalAllExperience += current.GainGP;
					current.TotalAllScore += num;
					current.BossCardCount = this.BossCardCount;
					gSPacketIn.WriteInt(current.PlayerDetail.PlayerCharacter.ID);
					gSPacketIn.WriteInt(current.PlayerDetail.PlayerCharacter.Grade);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(current.GainGP);
					gSPacketIn.WriteBoolean(this.IsWin);
					gSPacketIn.WriteInt(current.BossCardCount);
					gSPacketIn.WriteBoolean(false);
					gSPacketIn.WriteBoolean(false);
				}
				gSPacketIn.WriteInt(this.m_gameOverResources.Count);
				foreach (string current2 in this.m_gameOverResources)
				{
					gSPacketIn.WriteString(current2);
				}
				this.SendToAll(gSPacketIn);
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Player current3 in allFightPlayers)
				{
					stringBuilder.Append(current3.PlayerDetail.PlayerCharacter.ID).Append(",");
					current3.Ready = false;
					current3.PlayerDetail.OnMissionOver(current3.Game, this.IsWin, this.MissionInfo.Id, current3.TurnNum);
				}
				int winTeam = this.IsWin ? 1 : 2;
				string teamA = stringBuilder.ToString();
				string teamB = "";
				string playResult = "";
				if (!this.IsWin && this.SessionId < 2)
				{
					base.OnGameStopped();
				}
				StringBuilder stringBuilder2 = new StringBuilder();
				if (this.IsWin && this.IsBossWar != "")
				{
					stringBuilder2.Append(this.IsBossWar).Append(",");
					foreach (Player current4 in allFightPlayers)
					{
						stringBuilder2.Append("玩家ID:").Append(current4.PlayerDetail.PlayerCharacter.ID).Append(",");
						stringBuilder2.Append("等级:").Append(current4.PlayerDetail.PlayerCharacter.Grade).Append(",");
						stringBuilder2.Append("攻击回合数:").Append(current4.TurnNum).Append(",");
						stringBuilder2.Append("攻击:").Append(current4.PlayerDetail.PlayerCharacter.Attack).Append(",");
						stringBuilder2.Append("防御:").Append(current4.PlayerDetail.PlayerCharacter.Defence).Append(",");
						stringBuilder2.Append("敏捷:").Append(current4.PlayerDetail.PlayerCharacter.Agility).Append(",");
						stringBuilder2.Append("幸运:").Append(current4.PlayerDetail.PlayerCharacter.Luck).Append(",");
						stringBuilder2.Append("伤害:").Append(current4.PlayerDetail.GetBaseAttack()).Append(",");
						stringBuilder2.Append("总血量:").Append(current4.MaxBlood).Append(",");
						stringBuilder2.Append("护甲:").Append(current4.PlayerDetail.GetBaseDefence()).Append(",");
						if (current4.PlayerDetail.SecondWeapon != null)
						{
							stringBuilder2.Append("副武器:").Append(current4.PlayerDetail.SecondWeapon.TemplateID).Append(",");
							stringBuilder2.Append("副武器强化等级:").Append(current4.PlayerDetail.SecondWeapon.StrengthenLevel).Append(".");
						}
					}
				}
				this.BossWarField = stringBuilder2.ToString();
				base.OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, this.beginTime, DateTime.Now, this.BeginPlayersCount, this.MissionInfo.Id, teamA, teamB, playResult, winTeam, this.BossWarField);
				base.OnGameOverred();
			}
		}
		public bool HasNextSession()
		{
			if (base.PlayerCount == 0 || !this.IsWin)
			{
				return false;
			}
			int key = this.SessionId + 1;
			return this.Misssions.ContainsKey(key);
		}
		public void GameOverAllSession()
		{
			if (base.GameState == eGameState.GameOver)
			{
				this.m_gameState = eGameState.ALLSessionStopped;
				try
				{
					this.m_gameAI.OnGameOverAllSession();
				}
				catch (Exception arg)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
				}
				List<Player> allFightPlayers = base.GetAllFightPlayers();
				GSPacketIn gSPacketIn = new GSPacketIn(91);
				gSPacketIn.WriteByte(115);
				int canTakeOut = 1;
				if (!this.IsWin)
				{
					canTakeOut = 0;
				}
				else
				{
					if (this.m_roomType == eRoomType.Dungeon)
					{
						canTakeOut = 2;
					}
				}
				gSPacketIn.WriteInt(base.PlayerCount);
				foreach (Player current in allFightPlayers)
				{
					current.CanTakeOut = canTakeOut;
					current.PlayerDetail.OnGameOver(this, this.IsWin, current.GainGP);
					gSPacketIn.WriteInt(current.PlayerDetail.PlayerCharacter.ID);
					gSPacketIn.WriteInt(current.TotalAllKill);
					gSPacketIn.WriteInt(current.TotalAllHurt);
					gSPacketIn.WriteInt(current.TotalAllScore);
					gSPacketIn.WriteInt(current.TotalAllCure);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(current.TotalAllExperience);
					gSPacketIn.WriteBoolean(this.IsWin);
				}
				gSPacketIn.WriteInt(this.m_gameOverResources.Count);
				foreach (string current2 in this.m_gameOverResources)
				{
					gSPacketIn.WriteString(current2);
				}
				this.SendToAll(gSPacketIn);
				base.WaitTime(21000);
				this.CanStopGame();
			}
		}
		public void CanStopGame()
		{
			if (!this.IsWin && base.GameType == eGameType.WordBoss)
			{
				this.WantTryAgain = 0;
				base.ClearWaitTimer();
			}
		}
		public override void Stop()
		{
			if (base.GameState == eGameState.ALLSessionStopped)
			{
				this.m_gameState = eGameState.Stopped;
				if (this.IsWin)
				{
					List<Player> allFightPlayers = base.GetAllFightPlayers();
					foreach (Player current in allFightPlayers)
					{
						if (current.IsActive && current.CanTakeOut > 0)
						{
							current.HasPaymentTakeCard = true;
							int canTakeOut = current.CanTakeOut;
							for (int i = 0; i < canTakeOut; i++)
							{
								this.TakeCard(current);
							}
						}
					}
					if (base.RoomType == eRoomType.Dungeon)
					{
						this.SendShowCards();
					}
					if (base.RoomType == eRoomType.Dungeon)
					{
						foreach (Player current2 in allFightPlayers)
						{
							current2.PlayerDetail.SetPvePermission(this.m_info.ID, this.m_hardLevel);
						}
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
				base.OnGameStopped();
			}
		}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			foreach (Living current in this.m_livings)
			{
				current.Dispose();
			}
			try
			{
				this.m_missionAI.Dispose();
			}
			catch (Exception arg)
			{
				PVEGame.log.ErrorFormat("game ai script m_missionAI.Dispose() error:{1}", arg);
			}
			try
			{
				this.m_gameAI.Dispose();
			}
			catch (Exception arg2)
			{
				PVEGame.log.ErrorFormat("game ai script m_gameAI.Dispose() error:{1}", arg2);
			}
		}
		public void DoOther()
		{
			try
			{
				this.m_missionAI.DoOther();
			}
			catch (Exception arg)
			{
				PVEGame.log.ErrorFormat("game ai script m_gameAI.DoOther() error:{1}", arg);
			}
		}
		internal void OnShooted()
		{
			try
			{
				this.m_missionAI.OnShooted();
			}
			catch (Exception arg)
			{
				PVEGame.log.ErrorFormat("game ai script m_gameAI.OnShooted() error:{1}", arg);
			}
		}
		private int CalculateExperience(Player p)
		{
			if (this.TotalKillCount == 0)
			{
				return 1;
			}
			double num = Math.Abs((double)p.Grade - this.TotalNpcGrade / (double)this.TotalKillCount);
			if (num >= 7.0)
			{
				return 1;
			}
			double num2 = 0.0;
			if (this.TotalKillCount > 0)
			{
				num2 += (double)p.TotalKill / (double)this.TotalKillCount * 0.4;
			}
			if (this.TotalHurt > 0)
			{
				num2 += (double)p.TotalHurt / (double)this.TotalHurt * 0.4;
			}
			if (p.IsLiving)
			{
				num2 += 0.4;
			}
			double num3 = 1.0;
			if (num >= 3.0 && num <= 4.0)
			{
				num3 = 0.7;
			}
			else
			{
				if (num >= 5.0 && num <= 6.0)
				{
					num3 = 0.4;
				}
			}
			double num4 = (0.9 + (double)(this.BeginPlayersCount - 1) * 0.4) / (double)base.PlayerCount;
			double num5 = this.TotalNpcExperience * num2 * num3 * num4;
			num5 = ((num5 == 0.0) ? 1.0 : num5);
			return (int)num5;
		}
		private int CalculateScore(Player p)
		{
			int num = (200 - base.TurnIndex) * 5 + p.TotalKill * 5 + (int)((double)p.Blood / (double)p.MaxBlood) * 10;
			if (!this.IsWin)
			{
				num -= 400;
			}
			return num;
		}
		private int CalculateHitRate(int hitTargetCount, int shootCount)
		{
			double num = 0.0;
			if (shootCount > 0)
			{
				num = (double)hitTargetCount / (double)shootCount;
			}
			return (int)(num * 100.0);
		}
		public override void CheckState(int delay)
		{
			base.AddAction(new CheckPVEGameStateAction(delay));
		}
		public bool TakeBossCard(Player player)
		{
			int index = 0;
			for (int i = 0; i < this.BossCards.Length; i++)
			{
				if (this.Cards[i] == 0)
				{
					index = i;
					break;
				}
			}
			return this.TakeCard(player, index);
		}
		public bool TakeBossCard(Player player, int index)
		{
			if (!player.IsActive || player.BossCardCount <= 0 || index < 0 || index > this.BossCards.Length || this.BossCards[index] > 0)
			{
				return false;
			}
			List<ItemInfo> list = null;
			int value = 0;
			int num = 0;
			int value2 = 0;
			int num2 = 0;
			DropInventory.BossDrop(base.Map.Info.ID, ref list);
			if (list != null)
			{
				foreach (ItemInfo current in list)
				{
					ItemInfo.FindSpecialItemInfo(current, ref value, ref num, ref value2, ref num2);
					if (current != null)
					{
						player.PlayerDetail.AddTemplate(current, eBageType.TempBag, current.Count);
						int arg_99_0 = current.TemplateID;
					}
				}
				player.PlayerDetail.AddGold(value);
				player.PlayerDetail.AddMoney(num);
				player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_BossDrop, player.PlayerDetail.PlayerCharacter.ID, num, player.PlayerDetail.PlayerCharacter.Money);
				player.PlayerDetail.AddGiftToken(value2);
			}
			player.BossCardCount--;
			this.BossCards[index] = 1;
			return true;
		}
		public void SendMissionInfo()
		{
			if (this.m_missionInfo == null)
			{
				return;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(113);
			gSPacketIn.WriteString(this.m_missionInfo.Name);
			gSPacketIn.WriteString(this.m_missionInfo.Success);
			gSPacketIn.WriteString(this.m_missionInfo.Failure);
			gSPacketIn.WriteString(this.m_missionInfo.Description);
			gSPacketIn.WriteString(this.m_missionInfo.Title);
			gSPacketIn.WriteInt(this.TotalMissionCount);
			gSPacketIn.WriteInt(this.SessionId);
			gSPacketIn.WriteInt(this.TotalTurn);
			gSPacketIn.WriteInt(this.TotalCount);
			gSPacketIn.WriteInt(this.Param1);
			gSPacketIn.WriteInt(this.Param2);
			gSPacketIn.WriteInt(0);
			this.SendToAll(gSPacketIn);
		}
		public void SendUpdateUiData()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(104);
			int val = 0;
			try
			{
				val = this.m_missionAI.UpdateUIData();
			}
			catch (Exception arg)
			{
				PVEGame.log.ErrorFormat("game ai script {0} error:{1}", string.Format("m_missionAI.UpdateUIData()", new object[0]), arg);
			}
			gSPacketIn.WriteInt(base.TurnIndex);
			gSPacketIn.WriteInt(val);
			gSPacketIn.WriteInt(this.Param3);
			gSPacketIn.WriteInt(this.Param4);
			this.SendToAll(gSPacketIn);
		}
		internal void SendShowCards()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(89);
			int num = 0;
			List<int> list = new List<int>();
			for (int i = 0; i < this.Cards.Length; i++)
			{
				if (this.Cards[i] == 0)
				{
					list.Add(i);
					num++;
				}
			}
			gSPacketIn.WriteInt(num);
			int val = 0;
			foreach (int current in list)
			{
				List<ItemInfo> list2 = DropInventory.CopySystemDrop(this.m_missionInfo.Id, list.Count);
				if (list2 != null)
				{
					foreach (ItemInfo current2 in list2)
					{
						val = current2.TemplateID;
					}
				}
				gSPacketIn.WriteByte((byte)current);
				gSPacketIn.WriteInt(val);
				gSPacketIn.WriteInt(1);
			}
			this.SendToAll(gSPacketIn);
		}
		public void SendGameObjectFocus(int type, string name, int delay, int finishTime)
		{
			Physics[] array = base.FindPhysicalObjByName(name);
			Physics[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Physics obj = array2[i];
				base.AddAction(new FocusAction(obj, type, delay, finishTime));
			}
		}
		private void SendCreateGameToSingle(PVEGame game, IGamePlayer gamePlayer)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(121);
			gSPacketIn.WriteInt(game.Map.Info.ID);
			gSPacketIn.WriteInt((int)((byte)game.RoomType));
			gSPacketIn.WriteInt((int)((byte)game.GameType));
			gSPacketIn.WriteInt(game.TimeType);
			List<Player> allFightPlayers = game.GetAllFightPlayers();
			gSPacketIn.WriteInt(allFightPlayers.Count);
			foreach (Player current in allFightPlayers)
			{
				IGamePlayer playerDetail = current.PlayerDetail;
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ID);
				gSPacketIn.WriteString(playerDetail.PlayerCharacter.NickName);
				gSPacketIn.WriteBoolean(false);
				gSPacketIn.WriteByte(playerDetail.PlayerCharacter.typeVIP);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.VIPLevel);
				gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.Sex);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Hide);
				gSPacketIn.WriteString(playerDetail.PlayerCharacter.Style);
				gSPacketIn.WriteString(playerDetail.PlayerCharacter.Colors);
				gSPacketIn.WriteString(playerDetail.PlayerCharacter.Skin);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Grade);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Repute);
				if (playerDetail.MainWeapon == null)
				{
					gSPacketIn.WriteInt(0);
				}
				else
				{
					gSPacketIn.WriteInt(playerDetail.MainWeapon.TemplateID);
					gSPacketIn.WriteInt(playerDetail.MainWeapon.RefineryLevel);
					gSPacketIn.WriteString(playerDetail.MainWeapon.Name);
					gSPacketIn.WriteDateTime(DateTime.MinValue);
				}
				if (playerDetail.SecondWeapon == null)
				{
					gSPacketIn.WriteInt(0);
				}
				else
				{
					gSPacketIn.WriteInt(playerDetail.SecondWeapon.TemplateID);
				}
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ConsortiaID);
				gSPacketIn.WriteString(playerDetail.PlayerCharacter.ConsortiaName);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.badgeID);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteBoolean(false);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(current.Team);
				gSPacketIn.WriteInt(current.Id);
				gSPacketIn.WriteInt(current.MaxBlood);
				gSPacketIn.WriteBoolean(current.Ready);
			}
			int num = game.SessionId - 1;
			MissionInfo missionInfo = game.Misssions[num];
			gSPacketIn.WriteString(missionInfo.Name);
			gSPacketIn.WriteString("show" + num + ".jpg");
			gSPacketIn.WriteString(missionInfo.Success);
			gSPacketIn.WriteString(missionInfo.Failure);
			gSPacketIn.WriteString(missionInfo.Description);
			gSPacketIn.WriteInt(game.TotalMissionCount);
			gSPacketIn.WriteInt(num);
			gamePlayer.SendTCP(gSPacketIn);
		}
		public void SendPlayerInfoInGame(PVEGame game, IGamePlayer gp, Player p)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(120);
			gSPacketIn.WriteInt(gp.PlayerCharacter.ID);
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteInt(p.Team);
			gSPacketIn.WriteInt(p.Id);
			gSPacketIn.WriteInt(p.MaxBlood);
			gSPacketIn.WriteBoolean(p.Ready);
			game.SendToAll(gSPacketIn);
		}
		public void SendPlaySound(string playStr)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(63);
			gSPacketIn.WriteString(playStr);
			this.SendToAll(gSPacketIn);
		}
		public void SendLoadResource(List<LoadingFileInfo> loadingFileInfos)
		{
			if (loadingFileInfos != null && loadingFileInfos.Count > 0)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(91);
				gSPacketIn.WriteByte(67);
				gSPacketIn.WriteInt(loadingFileInfos.Count);
				foreach (LoadingFileInfo current in loadingFileInfos)
				{
					gSPacketIn.WriteInt(current.Type);
					gSPacketIn.WriteString(current.Path);
					gSPacketIn.WriteString(current.ClassName);
				}
				this.SendToAll(gSPacketIn);
			}
		}
		public override void MinusDelays(int lowestDelay)
		{
			this.m_pveGameDelay -= lowestDelay;
			base.MinusDelays(lowestDelay);
		}
		public void Print(string str)
		{
			Console.WriteLine(str);
		}
        //
        public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleBoss boss = new SimpleBoss(base.PhysicalId++, this, npcInfo, direction, type, "");

            if (type == 2)
            {
                boss.Blood = npcInfo.Blood / 4;
                boss.BaseDamage = npcInfo.BaseDamage;
                boss.BaseGuard = npcInfo.BaseGuard;
                boss.Attack = npcInfo.Attack;
                boss.Defence = npcInfo.Defence;
                boss.Agility = npcInfo.Agility;
                boss.Lucky = npcInfo.Lucky;
                boss.Grade = npcInfo.Level;
                boss.Experience = npcInfo.Experience;
                boss.SetRect(npcInfo.X, npcInfo.Y, npcInfo.Width, npcInfo.Height);
            }
            else
            {
                boss.Reset();
            }
            boss.SetXY(x, y);
            AddLiving(boss);
            boss.StartMoving();
            return boss;
        }
        public Layer Createlayerboss(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
        {
            Layer layer = new Layer(this.PhysicalId++, name, model, defaultAction, scale, rotation);
            layer.SetXY(x, y);
            this.AddPhysical(layer, true);
            return layer;
        }
        public LayerLabyrinth CreateLayerLabyrinth(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
        {
            LayerLabyrinth layer = new LayerLabyrinth(this.PhysicalId++, name, model, defaultAction, scale, rotation);
            layer.SetXY(x, y);
            this.AddPhysical(layer, true);
            return layer;
        }

        public NormalBoss CreateSimple(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            NormalBoss boss = new NormalBoss(base.PhysicalId++, this, npcInfo, direction, type);
            if (type != 2)
            {
                boss.Blood = npcInfo.Blood / 2;
                boss.BaseDamage = npcInfo.BaseDamage;
                boss.BaseGuard = npcInfo.BaseGuard;
                boss.Attack = npcInfo.Attack;
                boss.Defence = npcInfo.Defence;
                boss.Agility = npcInfo.Agility;
                boss.Lucky = npcInfo.Lucky;
                boss.Grade = npcInfo.Level;
                boss.Experience = npcInfo.Experience;
            }
            else
            {
                boss.Reset();
            }
            boss.SetRect(npcInfo.X, npcInfo.Y, npcInfo.Width, npcInfo.Height);
            boss.SetXY(x, y);
            AddLiving(boss);
            boss.StartMoving();
            return boss;
        }

        public NormalNpc CreateNormal(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            NormalNpc boss = new NormalNpc(base.PhysicalId++, this, npcInfo, direction, type);
            boss.Reset();
            boss.SetXY(x, y);
            AddLiving(boss);
            boss.StartMoving();
            return boss;
        }
        public void SendGameFocus(int x, int y, int type, int delay, int finishTime)
        {
            Createlayer(x, y, "pic", "", "", 1, 0);
            SendGameObjectFocus(1, "pic", delay, finishTime);
        }

        public BossNoDie CreateNpcNoDie(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
            BossNoDie living = new BossNoDie(base.PhysicalId++, this, npcInfoById, direction, type);
            living.Reset();
            living.SetXY(x, y);
            AddBoss(living);
            living.StartMoving();
            return living;
        }
	}
}
