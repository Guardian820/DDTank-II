using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class BaseGame : AbstractGame
	{
		public delegate void GameOverLogEventHandle(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA, string teamB, string playResult, int winTeam, string BossWar);
		public delegate void GameNpcDieEventHandle(int NpcId);
		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected int turnIndex;
		protected int m_nextWind;
		protected eGameState m_gameState;
		protected Map m_map;
		protected Dictionary<int, Player> m_players;
		protected List<Living> m_livings;
		protected Random m_random;
		protected TurnedLiving m_currentLiving;
		public int PhysicalId;
		public int CurrentTurnTotalDamage;
		public int TotalHurt;
		public int ConsortiaAlly;
		public int RichesRate;
		public string BossWarField;
		private ArrayList m_actions;
		private List<TurnedLiving> m_turnQueue;
		private int m_roomId;
		public int[] Cards;
		private int m_lifeTime;
		private long m_waitTimer;
		private long m_passTick;
		public int CurrentActionCount;
		private List<Box> m_tempBox;
		private List<Point> m_tempPoints;
		private List<LoadingFileInfo> m_loadingFiles = new List<LoadingFileInfo>();
		private List<PetSkillElementInfo> GameNeedPetSkillInfo = new List<PetSkillElementInfo>();
		public int TotalCostMoney;
		public int TotalCostGold;
		public event GameEventHandle GameOverred;
		public event GameEventHandle BeginNewTurn;
		public event BaseGame.GameOverLogEventHandle GameOverLog;
		public event BaseGame.GameNpcDieEventHandle GameNpcDie;
		protected int m_turnIndex
		{
			get
			{
				return this.turnIndex;
			}
			set
			{
				this.turnIndex = value;
			}
		}
		public int RoomId
		{
			get
			{
				return this.m_roomId;
			}
		}
		public Dictionary<int, Player> Players
		{
			get
			{
				return this.m_players;
			}
		}
		public int PlayerCount
		{
			get
			{
				Dictionary<int, Player> players;
				Monitor.Enter(players = this.m_players);
				int count;
				try
				{
					count = this.m_players.Count;
				}
				finally
				{
					Monitor.Exit(players);
				}
				return count;
			}
		}
		public int TurnIndex
		{
			get
			{
				return this.m_turnIndex;
			}
			set
			{
				this.m_turnIndex = value;
			}
		}
		public eGameState GameState
		{
			get
			{
				return this.m_gameState;
			}
		}
		public float Wind
		{
			get
			{
				return this.m_map.wind;
			}
		}
		public Map Map
		{
			get
			{
				return this.m_map;
			}
		}
		public List<TurnedLiving> TurnQueue
		{
			get
			{
				return this.m_turnQueue;
			}
		}
		public bool HasPlayer
		{
			get
			{
				return this.m_players.Count > 0;
			}
		}
		public Random Random
		{
			get
			{
				return this.m_random;
			}
		}
		public TurnedLiving CurrentLiving
		{
			get
			{
				return this.m_currentLiving;
			}
		}
		public int LifeTime
		{
			get
			{
				return this.m_lifeTime;
			}
		}
		public BaseGame(int id, int roomId, Map map, eRoomType roomType, eGameType gameType, int timeType, List<PetSkillElementInfo> GameNeedPetSkillInfoList) : base(id, roomType, gameType, timeType)
		{
			this.m_roomId = roomId;
			this.m_players = new Dictionary<int, Player>();
			this.m_turnQueue = new List<TurnedLiving>();
			this.m_livings = new List<Living>();
			this.m_random = new Random();
			this.m_map = map;
			this.m_actions = new ArrayList();
			this.PhysicalId = 0;
			this.BossWarField = "";
			this.m_tempBox = new List<Box>();
			this.m_tempPoints = new List<Point>();
			this.GameNeedPetSkillInfo = GameNeedPetSkillInfoList;
			if (roomType == eRoomType.Dungeon)
			{
				this.Cards = new int[21];
			}
			else
			{
				this.Cards = new int[9];
			}
			this.m_gameState = eGameState.Inited;
		}
		public void SetWind(int wind)
		{
			this.m_map.wind = (float)wind;
		}
		public bool SetMap(int mapId)
		{
			if (this.GameState == eGameState.Playing)
			{
				return false;
			}
			Map map = MapMgr.CloneMap(mapId);
			if (map != null)
			{
				this.m_map = map;
				return true;
			}
			return false;
		}
		public int GetTurnWaitTime()
		{
			return this.m_timeType;
		}
		protected void AddPlayer(IGamePlayer gp, Player fp)
		{
			Dictionary<int, Player> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				this.m_players.Add(fp.Id, fp);
				if (fp.Weapon != null)
				{
					this.m_turnQueue.Add(fp);
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
		}
		public virtual void AddLiving(Living living)
		{
			this.m_map.AddPhysical(living);
			if (living is Player)
			{
				Player player = living as Player;
				if (player.Weapon == null)
				{
					return;
				}
			}
			if (living is TurnedLiving)
			{
				this.m_turnQueue.Add(living as TurnedLiving);
			}
			else
			{
				this.m_livings.Add(living);
			}
			this.SendAddLiving(living);
		}
        public virtual void AddBoss(Living living)
        {
            this.m_map.AddPhysical(living);
            if (living is Player)
            {
                Player player = living as Player;
                if (player.Weapon == null)
                {
                    return;
                }
            }
            if (living is TurnedLiving)
            {
                this.m_turnQueue.Add(living as TurnedLiving);
            }
            else
            {
                this.m_livings.Add(living);
            }
            this.SendAddBoss(living);
        }
		public virtual void AddPhysicalObj(PhysicalObj phy, bool sendToClient)
		{
			this.m_map.AddPhysical(phy);
			phy.SetGame(this);
			if (sendToClient)
			{
				this.SendAddPhysicalObj(phy);
			}
		}
        public virtual void AddPhysical(PhysicalObj phy, bool sendToClient)
        {
            this.m_map.AddPhysical(phy);
            phy.SetGame(this);
            if (sendToClient)
            {
                this.SendAddPhysical(phy);
            }
        }
		public virtual void AddPhysicalTip(PhysicalObj phy, bool sendToClient)
		{
			this.m_map.AddPhysical(phy);
			phy.SetGame(this);
			if (sendToClient)
			{
				this.SendAddPhysicalTip(phy);
			}
		}
		public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
		{
			Player player = null;
			Dictionary<int, Player> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				foreach (Player current in this.m_players.Values)
				{
					if (current.PlayerDetail == gp)
					{
						player = current;
						this.m_players.Remove(current.Id);
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
			if (player != null)
			{
				this.AddAction(new RemovePlayerAction(player));
			}
			return player;
		}
		public void RemovePhysicalObj(PhysicalObj phy, bool sendToClient)
		{
			this.m_map.RemovePhysical(phy);
			phy.SetGame(null);
			if (sendToClient)
			{
				this.SendRemovePhysicalObj(phy);
			}
		}
		public void RemoveLiving(int id)
		{
			this.SendRemoveLiving(id);
		}
		public List<Living> GetLivedLivings()
		{
			List<Living> list = new List<Living>();
			foreach (Living current in this.m_livings)
			{
				if (current.IsLiving)
				{
					list.Add(current);
				}
			}
			return list;
		}
		public void ClearDiedPhysicals()
		{
			List<Living> list = new List<Living>();
			foreach (Living current in this.m_livings)
			{
				if (!current.IsLiving)
				{
					list.Add(current);
				}
			}
			foreach (Living current2 in list)
			{
				this.m_livings.Remove(current2);
				current2.Dispose();
			}
			List<Living> list2 = new List<Living>();
			foreach (TurnedLiving current3 in this.m_turnQueue)
			{
				if (!current3.IsLiving)
				{
					list2.Add(current3);
				}
			}
			using (List<Living>.Enumerator enumerator4 = list2.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					TurnedLiving item = (TurnedLiving)enumerator4.Current;
					this.m_turnQueue.Remove(item);
				}
			}
			List<Physics> allPhysicalSafe = this.m_map.GetAllPhysicalSafe();
			foreach (Physics current4 in allPhysicalSafe)
			{
				if (!current4.IsLiving && !(current4 is Player))
				{
					this.m_map.RemovePhysical(current4);
				}
			}
		}
		public bool IsAllComplete()
		{
			List<Player> allFightPlayers = this.GetAllFightPlayers();
			foreach (Player current in allFightPlayers)
			{
				if (current.LoadingProcess < 100)
				{
					return false;
				}
			}
			return true;
		}
		public Player FindPlayer(int id)
		{
			Dictionary<int, Player> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				if (this.m_players.ContainsKey(id))
				{
					return this.m_players[id];
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
			return null;
		}
		public TurnedLiving FindNextTurnedLiving()
		{
			if (this.m_turnQueue.Count == 0)
			{
				return null;
			}
			int index = this.m_random.Next(this.m_turnQueue.Count - 1);
			TurnedLiving turnedLiving = this.m_turnQueue[index];
			int delay = turnedLiving.Delay;
			for (int i = 0; i < this.m_turnQueue.Count; i++)
			{
				if (this.m_turnQueue[i].Delay < delay && this.m_turnQueue[i].IsLiving)
				{
					delay = this.m_turnQueue[i].Delay;
					turnedLiving = this.m_turnQueue[i];
				}
			}
			turnedLiving.TurnNum++;
			return turnedLiving;
		}
		public virtual void MinusDelays(int lowestDelay)
		{
			foreach (TurnedLiving current in this.m_turnQueue)
			{
				current.Delay -= lowestDelay;
			}
		}
		public SimpleBoss[] FindAllBoss()
		{
			List<SimpleBoss> list = new List<SimpleBoss>();
			foreach (Living current in this.m_livings)
			{
				if (current is SimpleBoss)
				{
					list.Add(current as SimpleBoss);
				}
			}
			return list.ToArray();
		}
		public SimpleNpc[] FindAllNpc()
		{
			List<SimpleNpc> list = new List<SimpleNpc>();
			foreach (Living current in this.m_livings)
			{
				if (current is SimpleNpc)
				{
					list.Add(current as SimpleNpc);
					return list.ToArray();
				}
			}
			return null;
		}
		public float GetNextWind()
		{
			int num = (int)(this.Wind * 10f);
			int num2;
			if (num > this.m_nextWind)
			{
				num2 = num - this.m_random.Next(11);
				if (num <= this.m_nextWind)
				{
					this.m_nextWind = this.m_random.Next(-40, 40);
				}
			}
			else
			{
				num2 = num + this.m_random.Next(11);
				if (num >= this.m_nextWind)
				{
					this.m_nextWind = this.m_random.Next(-40, 40);
				}
			}
			return (float)num2 / 10f;
		}
		public void UpdateWind(float wind, bool sendToClient)
		{
			if (this.m_map.wind != wind)
			{
				this.m_map.wind = wind;
				if (sendToClient)
				{
					this.SendGameUpdateWind(wind);
				}
			}
		}
		public int GetDiedPlayerCount()
		{
			int num = 0;
			foreach (Player current in this.m_players.Values)
			{
				if (current.IsActive && !current.IsLiving)
				{
					num++;
				}
			}
			return num;
		}
		protected Point GetPlayerPoint(MapPoint mapPos, int team)
		{
			List<Point> list = (team == 1) ? mapPos.PosX : mapPos.PosX1;
			int index = this.m_random.Next(list.Count);
			Point point = list[index];
			list.Remove(point);
			return point;
		}
		public virtual void CheckState(int delay)
		{
		}
		public override void ProcessData(GSPacketIn packet)
		{
			if (this.m_players.ContainsKey(packet.Parameter1))
			{
				Player player = this.m_players[packet.Parameter1];
				this.AddAction(new ProcessPacketAction(player, packet));
			}
		}
		public Player GetPlayerByIndex(int index)
		{
			return this.m_players.ElementAt(index).Value;
		}
		public Player FindNearestPlayer(int x, int y)
		{
			double num = 1.7976931348623157E+308;
			Player result = null;
			foreach (Player current in this.m_players.Values)
			{
				if (current.IsLiving)
				{
					double num2 = current.Distance(x, y);
					if (num2 < num)
					{
						num = num2;
						result = current;
					}
				}
			}
			return result;
		}
		public Player FindRandomPlayer()
		{
			List<Player> list = new List<Player>();
			foreach (Player current in this.m_players.Values)
			{
				if (current.IsLiving)
				{
					list.Add(current);
				}
			}
			int index = this.Random.Next(0, list.Count);
			return list.ElementAt(index);
		}
		public Living FindRandomLiving()
		{
			List<Living> list = new List<Living>();
			foreach (Living current in this.m_livings)
			{
				if (current.IsLiving && current is NormalNpc)
				{
					list.Add(current);
				}
			}
			int index = this.Random.Next(0, list.Count);
			return list.ElementAt(index);
		}
		public int FindlivingbyDir(Living npc)
		{
			int num = 0;
			int num2 = 0;
			foreach (Player current in this.m_players.Values)
			{
				if (current.IsLiving)
				{
					if (current.X > npc.X)
					{
						num2++;
					}
					else
					{
						num++;
					}
				}
			}
			if (num2 > num)
			{
				return 1;
			}
			if (num2 < num)
			{
				return -1;
			}
			return -npc.Direction;
		}
		public PhysicalObj[] FindPhysicalObjByName(string name)
		{
			List<PhysicalObj> list = new List<PhysicalObj>();
			foreach (PhysicalObj current in this.m_map.GetAllPhysicalObjSafe())
			{
				if (current.Name == name)
				{
					list.Add(current);
				}
			}
			return list.ToArray();
		}
		public PhysicalObj[] FindPhysicalObjByName(string name, bool CanPenetrate)
		{
			List<PhysicalObj> list = new List<PhysicalObj>();
			foreach (PhysicalObj current in this.m_map.GetAllPhysicalObjSafe())
			{
				if (current.Name == name && current.CanPenetrate == CanPenetrate)
				{
					list.Add(current);
				}
			}
			return list.ToArray();
		}
		public Player GetFrostPlayerRadom()
		{
			List<Player> allFightPlayers = this.GetAllFightPlayers();
			List<Player> list = new List<Player>();
			foreach (Player current in allFightPlayers)
			{
				if (current.IsFrost)
				{
					list.Add(current);
				}
			}
			if (list.Count > 0)
			{
				int index = this.Random.Next(0, list.Count);
				return list.ElementAt(index);
			}
			return null;
		}
		public virtual bool TakeCard(Player player)
		{
			return false;
		}
		public virtual bool TakeCard(Player player, int index)
		{
			return false;
		}
		public override void Pause(int time)
		{
			this.m_passTick = Math.Max(this.m_passTick, TickHelper.GetTickCount() + (long)time);
		}
		public override void Resume()
		{
			this.m_passTick = 0L;
		}
		public void AddAction(IAction action)
		{
			ArrayList actions;
			Monitor.Enter(actions = this.m_actions);
			try
			{
				this.m_actions.Add(action);
			}
			finally
			{
				Monitor.Exit(actions);
			}
		}
		public void AddAction(ArrayList actions)
		{
			ArrayList actions2;
			Monitor.Enter(actions2 = this.m_actions);
			try
			{
				this.m_actions.AddRange(actions);
			}
			finally
			{
				Monitor.Exit(actions2);
			}
		}
		public void ClearWaitTimer()
		{
			this.m_waitTimer = 0L;
		}
		public void WaitTime(int delay)
		{
			this.m_waitTimer = Math.Max(this.m_waitTimer, TickHelper.GetTickCount() + (long)delay);
		}
		public long GetWaitTimer()
		{
			return this.m_waitTimer;
		}
        public void Update(long tick)
        {
            if (m_passTick < tick)
            {
                m_lifeTime++;
                ArrayList temp;

                lock (m_actions)
                {
                    temp = (ArrayList)m_actions.Clone();
                    m_actions.Clear();
                }

                if (temp != null && GameState != eGameState.Stopped)
                {
                    CurrentActionCount = temp.Count;
                    if (temp.Count > 0)
                    {
                        ArrayList left = new ArrayList();
                        foreach (IAction action in temp)
                        {
                            try
                            {
                                action.Execute(this, tick);
                                if (action.IsFinished(tick) == false)
                                {
                                    left.Add(action);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("Map update error:", ex);
                            }
                        }
                        AddAction(left);
                    }
                    else if (m_waitTimer < tick)
                    {
                        CheckState(0);
                    }
                }
            }
        }
		public List<Player> GetAllFightPlayers()
		{
			List<Player> list = new List<Player>();
			Dictionary<int, Player> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				list.AddRange(this.m_players.Values);
			}
			finally
			{
				Monitor.Exit(players);
			}
			return list;
		}
		public List<Player> GetAllLivingPlayers()
		{
			List<Player> list = new List<Player>();
			Dictionary<int, Player> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				foreach (Player current in this.m_players.Values)
				{
					if (current.IsLiving)
					{
						list.Add(current);
					}
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
			return list;
		}
		public bool GetSameTeam()
		{
			bool result = false;
			Player[] allPlayers = this.GetAllPlayers();
			Player[] array = allPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				Player player = array[i];
				if (player.Team != allPlayers[0].Team)
				{
					result = false;
					break;
				}
				result = true;
			}
			return result;
		}
		public Player[] GetAllPlayers()
		{
			return this.GetAllFightPlayers().ToArray();
		}
		public Player GetPlayer(IGamePlayer gp)
		{
			Player result = null;
			Dictionary<int, Player> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				foreach (Player current in this.m_players.Values)
				{
					if (current.PlayerDetail == gp)
					{
						result = current;
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
			return result;
		}
		public int GetPlayerCount()
		{
			return this.GetAllFightPlayers().Count;
		}
		public virtual void SendToAll(GSPacketIn pkg)
		{
			this.SendToAll(pkg, null);
		}
		public virtual void SendToAll(GSPacketIn pkg, IGamePlayer except)
		{
			if (pkg.Parameter2 == 0)
			{
				pkg.Parameter2 = this.LifeTime;
			}
			List<Player> allFightPlayers = this.GetAllFightPlayers();
			foreach (Player current in allFightPlayers)
			{
				if (current.IsActive && current.PlayerDetail != except)
				{
					current.PlayerDetail.SendTCP(pkg);
				}
			}
		}
		public virtual void SendToTeam(GSPacketIn pkg, int team)
		{
			this.SendToTeam(pkg, team, null);
		}
		public virtual void SendToTeam(GSPacketIn pkg, int team, IGamePlayer except)
		{
			List<Player> allFightPlayers = this.GetAllFightPlayers();
			foreach (Player current in allFightPlayers)
			{
				if (current.IsActive && current.PlayerDetail != except && current.Team == team)
				{
					current.PlayerDetail.SendTCP(pkg);
				}
			}
		}
		public void AddTempPoint(int x, int y)
		{
			this.m_tempPoints.Add(new Point(x, y));
		}
		public Box AddBox(ItemInfo item, Point pos, bool sendToClient)
		{
			Box box = new Box(this.PhysicalId++, "1", item);
			box.SetXY(pos);
			this.AddPhysicalObj(box, sendToClient);
			return this.AddBox(box, sendToClient);
		}
		public Box AddBox(Box box, bool sendToClient)
		{
			this.m_tempBox.Add(box);
			this.AddPhysicalObj(box, sendToClient);
			return box;
		}
		public void CheckBox()
		{
			List<Box> list = new List<Box>();
			foreach (Box current in this.m_tempBox)
			{
				if (!current.IsLiving)
				{
					list.Add(current);
				}
			}
			foreach (Box current2 in list)
			{
				this.m_tempBox.Remove(current2);
				this.RemovePhysicalObj(current2, true);
			}
		}
		public List<Box> CreateBox()
		{
			int num = this.m_players.Count + 2;
			int num2 = 0;
			List<ItemInfo> list = null;
			if (this.CurrentTurnTotalDamage > 0)
			{
				num2 = this.m_random.Next(1, 3);
				if (this.m_tempBox.Count + num2 > num)
				{
					num2 = num - this.m_tempBox.Count;
				}
				if (num2 > 0)
				{
					DropInventory.BoxDrop(this.m_roomType, ref list);
				}
			}
			int diedPlayerCount = this.GetDiedPlayerCount();
			int num3 = 0;
			if (diedPlayerCount > 0)
			{
				num3 = this.m_random.Next(diedPlayerCount);
			}
			if (this.m_tempBox.Count + num2 + num3 > num)
			{
				num3 = num - this.m_tempBox.Count - num2;
			}
			List<Box> list2 = new List<Box>();
			if (list != null)
			{
				for (int i = 0; i < this.m_tempPoints.Count; i++)
				{
					int index = this.m_random.Next(this.m_tempPoints.Count);
					Point value = this.m_tempPoints[index];
					this.m_tempPoints[index] = this.m_tempPoints[i];
					this.m_tempPoints[i] = value;
				}
				int num4 = Math.Min(list.Count, this.m_tempPoints.Count);
				for (int j = 0; j < num4; j++)
				{
					list2.Add(this.AddBox(list[j], this.m_tempPoints[j], false));
				}
			}
			this.m_tempPoints.Clear();
			return list2;
		}
		public void AddLoadingFile(int type, string file, string className)
		{
			if (file == null || className == null)
			{
				return;
			}
			this.m_loadingFiles.Add(new LoadingFileInfo(type, file, className));
		}
		public void ClearLoadingFiles()
		{
			this.m_loadingFiles.Clear();
		}
		public void AfterUseItem(ItemInfo item)
		{
		}
        internal void SendCreateGame()//__createGame
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(101);
			gSPacketIn.WriteInt((int)((byte)this.m_roomType));
			gSPacketIn.WriteInt((int)((byte)this.m_gameType));
			gSPacketIn.WriteInt(this.m_timeType);
			List<Player> allFightPlayers = this.GetAllFightPlayers();
			gSPacketIn.WriteInt(allFightPlayers.Count);
			foreach (Player current in allFightPlayers)
			{
				IGamePlayer playerDetail = current.PlayerDetail;
				gSPacketIn.WriteInt(4);
				gSPacketIn.WriteString("zonename");
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
				gSPacketIn.WriteInt(playerDetail.MainWeapon.TemplateID);
				gSPacketIn.WriteInt(playerDetail.MainWeapon.RefineryLevel);
				gSPacketIn.WriteString(playerDetail.MainWeapon.Name);
				gSPacketIn.WriteDateTime(DateTime.Now);
				if (playerDetail.SecondWeapon == null)
				{
					gSPacketIn.WriteInt(0);
				}
				else
				{
					gSPacketIn.WriteInt(playerDetail.SecondWeapon.TemplateID);
				}
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Nimbus);
				gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.IsShowConsortia);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ConsortiaID);
				gSPacketIn.WriteString(playerDetail.PlayerCharacter.ConsortiaName);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.badgeID);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Win);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Total);
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.FightPower);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteString("");
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.AchievementPoint);
				gSPacketIn.WriteString("");
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Offer);
				gSPacketIn.WriteBoolean(false);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.IsMarried);
				if (playerDetail.PlayerCharacter.IsMarried)
				{
					gSPacketIn.WriteInt(playerDetail.PlayerCharacter.SpouseID);
					gSPacketIn.WriteString(playerDetail.PlayerCharacter.SpouseName);
				}
				gSPacketIn.WriteInt(5);
				gSPacketIn.WriteInt(5);
				gSPacketIn.WriteInt(5);
				gSPacketIn.WriteInt(5);
				gSPacketIn.WriteInt(5);
				gSPacketIn.WriteInt(5);
                gSPacketIn.WriteInt(current.Team);//_local10.team = _local2.readInt();
				gSPacketIn.WriteInt(current.Id);
                gSPacketIn.WriteInt(current.MaxBlood);
                gSPacketIn.WriteInt(0);//_local15 = _local2.readInt();
				if (current.Pet != null)
				{
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(current.Pet.Place);
					gSPacketIn.WriteInt(current.Pet.TemplateID);
					gSPacketIn.WriteInt(current.Pet.ID);
					gSPacketIn.WriteString(current.Pet.Name);
					gSPacketIn.WriteInt(current.Pet.UserID);
					gSPacketIn.WriteInt(current.Pet.Level);
					List<string> skillEquip = current.Pet.GetSkillEquip();
					gSPacketIn.WriteInt(skillEquip.Count);
					using (List<string>.Enumerator enumerator2 = skillEquip.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							string current2 = enumerator2.Current;
							gSPacketIn.WriteInt(int.Parse(current2.Split(new char[]
							{
								','
							})[1]));
							gSPacketIn.WriteInt(int.Parse(current2.Split(new char[]
							{
								','
							})[0]));
						}
						continue;
					}
				}
				gSPacketIn.WriteInt(0);
			}
			this.SendToAll(gSPacketIn);
		}
		internal void SendOpenSelectLeaderWindow(int maxTime)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(102);
			gSPacketIn.WriteInt(maxTime);
			this.SendToAll(gSPacketIn);
		}
		internal void SendIsLastMission(bool isLast)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(160);
			gSPacketIn.WriteBoolean(isLast);
			this.SendToAll(gSPacketIn);
		}
		internal void SendMissionTryAgain(int TryAgain)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(119);
			gSPacketIn.WriteInt(TryAgain);
			this.SendToAll(gSPacketIn);
		}
		internal bool isTrainer()
		{
			int iD = this.m_map.Info.ID;
			if (iD <= 1129)
			{
				if (iD != 1015 && iD != 1129)
				{
					return false;
				}
			}
			else
			{
				if (iD != 1132)
				{
					switch (iD)
					{
					case 2012:
					case 2013:
						break;

					default:
						return false;
					}
				}
			}
			return true;
		}
		internal void SendStartLoading(int maxTime)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(103);
			gSPacketIn.WriteInt(maxTime);
			gSPacketIn.WriteInt(this.m_map.Info.ID);
			gSPacketIn.WriteInt(this.m_loadingFiles.Count);
			foreach (LoadingFileInfo current in this.m_loadingFiles)
			{
				gSPacketIn.WriteInt(current.Type);
				gSPacketIn.WriteString(current.Path);
				gSPacketIn.WriteString(current.ClassName);
			}
			if (this.isTrainer())
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(this.GameNeedPetSkillInfo.Count);
				foreach (PetSkillElementInfo current2 in this.GameNeedPetSkillInfo)
				{
					gSPacketIn.WriteString(current2.Pic.ToString());
					gSPacketIn.WriteString(current2.EffectPic);
				}
			}
			this.SendToAll(gSPacketIn);
		}
		internal void SendAddPhysicalObj(PhysicalObj obj)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(48);
			gSPacketIn.WriteInt(obj.Id);
			gSPacketIn.WriteInt(obj.Type);
			gSPacketIn.WriteInt(obj.X);
			gSPacketIn.WriteInt(obj.Y);
			gSPacketIn.WriteString(obj.Model);
			gSPacketIn.WriteString(obj.CurrentAction);
			gSPacketIn.WriteInt(obj.Scale);
			gSPacketIn.WriteInt(obj.Scale);
			gSPacketIn.WriteInt(obj.Rotation);
			gSPacketIn.WriteInt(-1);
			gSPacketIn.WriteInt(0);
			this.SendToAll(gSPacketIn);
		}
        internal void SendAddPhysical(PhysicalObj obj)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(91);
            gSPacketIn.WriteByte(48);
            gSPacketIn.WriteInt(obj.Id);
            gSPacketIn.WriteInt(obj.Type);
            gSPacketIn.WriteInt(obj.X);
            gSPacketIn.WriteInt(obj.Y);
            gSPacketIn.WriteString(obj.Model);
            gSPacketIn.WriteString(obj.CurrentAction);
            gSPacketIn.WriteInt(obj.Scale);
            gSPacketIn.WriteInt(obj.Scale);
            gSPacketIn.WriteInt(obj.Rotation);
            gSPacketIn.WriteInt(1);
            gSPacketIn.WriteInt(0);
            this.SendToAll(gSPacketIn);
        }
		internal void SendAddPhysicalTip(PhysicalObj obj)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(68);
			gSPacketIn.WriteInt(obj.Id);
			gSPacketIn.WriteInt(obj.Type);
			gSPacketIn.WriteInt(obj.X);
			gSPacketIn.WriteInt(obj.Y);
			gSPacketIn.WriteString(obj.Model);
			gSPacketIn.WriteString(obj.CurrentAction);
			gSPacketIn.WriteInt(obj.Scale);
			gSPacketIn.WriteInt(obj.Rotation);
			this.SendToAll(gSPacketIn);
		}
		internal void SendPhysicalObjFocus(Physics obj, int type)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(62);
			gSPacketIn.WriteInt(type);
			gSPacketIn.WriteInt(obj.X);
			gSPacketIn.WriteInt(obj.Y);
			this.SendToAll(gSPacketIn);
		}
		internal void SendPhysicalObjPlayAction(PhysicalObj obj)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(66);
			gSPacketIn.WriteInt(obj.Id);
			gSPacketIn.WriteString(obj.CurrentAction);
			this.SendToAll(gSPacketIn);
		}
		internal void SendRemovePhysicalObj(PhysicalObj obj)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(53);
			gSPacketIn.WriteInt(obj.Id);
			this.SendToAll(gSPacketIn);
		}
		internal void SendRemoveLiving(int id)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(53);
			gSPacketIn.WriteInt(id);
			this.SendToAll(gSPacketIn);
		}
		internal void SendAddLiving(Living living)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(64);
			gSPacketIn.WriteByte((byte)living.Type);
			gSPacketIn.WriteInt(living.Id);
			gSPacketIn.WriteString(living.Name);
			gSPacketIn.WriteString(living.ModelId);
			gSPacketIn.WriteString(living.ActionStr);
			gSPacketIn.WriteInt(living.X);
			gSPacketIn.WriteInt(living.Y);
			gSPacketIn.WriteInt(living.Blood);
			gSPacketIn.WriteInt(living.MaxBlood);
			gSPacketIn.WriteInt(living.Team);
			gSPacketIn.WriteByte((byte)living.Direction);
			byte val = 0;
			if (living is SimpleBoss)
			{
				val = 1;
			}
			gSPacketIn.WriteByte(val);
			this.SendToAll(gSPacketIn);
		}
        internal void SendAddBoss(Living living)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(91);
            gSPacketIn.Parameter1 = living.Id;
            gSPacketIn.WriteByte(64);
            gSPacketIn.WriteByte((byte)living.Type);
            gSPacketIn.WriteInt(living.Id);
            gSPacketIn.WriteString(living.Name);
            gSPacketIn.WriteString(living.ModelId);
            gSPacketIn.WriteString(living.ActionStr);
            gSPacketIn.WriteInt(living.X);
            gSPacketIn.WriteInt(living.Y);
            gSPacketIn.WriteInt(living.Blood);
            gSPacketIn.WriteInt(living.MaxBlood);
            gSPacketIn.WriteInt(living.Team);
            gSPacketIn.WriteByte((byte)living.Direction);
            byte val = 0;
            if (living is SimpleNpc)
            {
                val = 1;
            }
            gSPacketIn.WriteByte(val);
            this.SendToAll(gSPacketIn);
        }

		internal void SendPlayerMove(Player player, int type, int x, int y, byte dir)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(9);
			gSPacketIn.WriteByte((byte)type);
			gSPacketIn.WriteInt(x);
			gSPacketIn.WriteInt(y);
			gSPacketIn.WriteByte(dir);
			gSPacketIn.WriteBoolean(player.IsLiving);
			if (type == 2)
			{
				gSPacketIn.WriteInt(this.m_tempBox.Count);
				foreach (Box current in this.m_tempBox)
				{
					gSPacketIn.WriteInt(current.X);
					gSPacketIn.WriteInt(current.Y);
				}
			}
			this.SendToAll(gSPacketIn);
		}
		internal void SendLivingMoveTo(Living living, int fromX, int fromY, int toX, int toY, string action, int speed, string sAction)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(55);
			gSPacketIn.WriteInt(fromX);
			gSPacketIn.WriteInt(fromY);
			gSPacketIn.WriteInt(toX);
			gSPacketIn.WriteInt(toY);
			gSPacketIn.WriteInt(speed);
			gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
			gSPacketIn.WriteString(sAction);
			this.SendToAll(gSPacketIn);
		}
		internal void SendLivingSay(Living living, string msg, int type)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(59);
			gSPacketIn.WriteString(msg);
			gSPacketIn.WriteInt(type);
			this.SendToAll(gSPacketIn);
		}
		internal void SendLivingFall(Living living, int toX, int toY, int speed, string action, int type)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(56);
			gSPacketIn.WriteInt(toX);
			gSPacketIn.WriteInt(toY);
			gSPacketIn.WriteInt(speed);
			gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
			gSPacketIn.WriteInt(type);
			this.SendToAll(gSPacketIn);
		}
		internal void SendLivingJump(Living living, int toX, int toY, int speed, string action, int type)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(57);
			gSPacketIn.WriteInt(toX);
			gSPacketIn.WriteInt(toY);
			gSPacketIn.WriteInt(speed);
			gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
			gSPacketIn.WriteInt(type);
			this.SendToAll(gSPacketIn);
		}
		internal void SendLivingBeat(Living living, Living target, int totalDemageAmount, string action, int livingCount, int attackEffect)
		{
			int val = 0;
			if (target is Player)
			{
				Player player = target as Player;
				val = player.Dander;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(58);
			gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
			gSPacketIn.WriteInt(livingCount);
			for (int i = 1; i <= livingCount; i++)
			{
				gSPacketIn.WriteInt(target.Id);
				gSPacketIn.WriteInt(totalDemageAmount);
				gSPacketIn.WriteInt(target.Blood);
				gSPacketIn.WriteInt(val);
				gSPacketIn.WriteInt(attackEffect);
			}
			this.SendToAll(gSPacketIn);
		}
		internal void SendLivingPlayMovie(Living living, string action)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(60);
			gSPacketIn.WriteString(action);
			this.SendToAll(gSPacketIn);
		}
		internal void SendGameUpdateHealth(Living player, int type, int value)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(11);
			gSPacketIn.WriteByte((byte)type);
			gSPacketIn.WriteInt(player.Blood);
			gSPacketIn.WriteInt(value);
			this.SendToAll(gSPacketIn);
		}
		internal void SendGameUpdateDander(TurnedLiving player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(14);
			gSPacketIn.WriteInt(player.Dander);
			this.SendToAll(gSPacketIn);
		}
		internal void SendGameUpdateFrozenState(Living player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(33);
			gSPacketIn.WriteBoolean(player.IsFrost);
			this.SendToAll(gSPacketIn);
		}
		internal void SendGameUpdateNoHoleState(Living player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(82);
			gSPacketIn.WriteBoolean(player.IsNoHole);
			this.SendToAll(gSPacketIn);
		}
		internal void SendGameUpdateHideState(Living player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(35);
			gSPacketIn.WriteBoolean(player.IsHide);
			this.SendToAll(gSPacketIn);
		}
		internal void SendGameUpdateSealState(Living player, int type)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte((byte)type);
			gSPacketIn.WriteBoolean(player.GetSealState());
			this.SendToAll(gSPacketIn);
		}
		internal void SendGameUpdateShootCount(Player player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.WriteByte(46);
			gSPacketIn.WriteByte((byte)player.ShootCount);
			this.SendToAll(gSPacketIn);
		}
		internal void SendGameUpdateBall(Player player, bool Special)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(20);
			gSPacketIn.WriteBoolean(Special);
			gSPacketIn.WriteInt(player.CurrentBall.ID);
			this.SendToAll(gSPacketIn);
		}
		internal void SendGamePickBox(Living player, int index, int arkType, string goods)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.WriteByte(49);
			gSPacketIn.WriteByte((byte)index);
			gSPacketIn.WriteByte((byte)arkType);
			gSPacketIn.WriteString(goods);
			this.SendToAll(gSPacketIn);
		}
		internal void SendGameUpdateWind(float wind)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(38);
			int num = (int)(wind * 10f);
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteBoolean(num > 0);
			gSPacketIn.WriteByte(this.GetVane(num, 1));
			gSPacketIn.WriteByte(this.GetVane(num, 2));
			gSPacketIn.WriteByte(this.GetVane(num, 3));
			this.SendToAll(gSPacketIn);
		}
		public byte GetVane(int Wind, int param)
		{
			int wind = Math.Abs(Wind);
			switch (param)
			{
			case 1:
				return WindMgr.GetWindID(wind, 1);

			case 3:
				return WindMgr.GetWindID(wind, 3);
			}
			return 0;
		}
		public void VaneLoading()
		{
			List<WindInfo> wind = WindMgr.GetWind();
			foreach (WindInfo current in wind)
			{
				this.SendGameWindPic((byte)current.WindID, current.WindPic);
			}
		}
		internal void SendGameWindPic(byte windId, byte[] windpic)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(241);
			gSPacketIn.WriteByte(windId);
			gSPacketIn.Write(windpic);
			this.SendToAll(gSPacketIn);
		}
		internal void SendPetUseKill(Player player, int killId, bool isUse)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(144);
			gSPacketIn.WriteInt(killId);
			gSPacketIn.WriteBoolean(isUse);
			this.SendToAll(gSPacketIn);
		}
		internal void SendUseDeputyWeapon(Player player, int ResCount)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(84);
			gSPacketIn.WriteInt(ResCount);
			this.SendToAll(gSPacketIn);
		}
		internal void SendPlayerUseProp(Player player, int type, int place, int templateID)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(32);
			gSPacketIn.WriteByte((byte)type);
			gSPacketIn.WriteInt(place);
			gSPacketIn.WriteInt(templateID);
			gSPacketIn.WriteInt(player.Id);
			gSPacketIn.WriteBoolean(false);
			this.SendToAll(gSPacketIn);
		}
		internal void SendGamePlayerTakeCard(Player player, int index, int templateID, int count, bool isSysTake)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(98);
			gSPacketIn.WriteBoolean(isSysTake);
			gSPacketIn.WriteByte((byte)index);
			gSPacketIn.WriteInt(templateID);
			gSPacketIn.WriteInt(count);
			gSPacketIn.WriteBoolean(false);
			this.SendToAll(gSPacketIn);
		}
		public static int getTurnTime(int timeType)
		{
			switch (timeType)
			{
			case 1:
				return 6;

			case 2:
				return 8;

			case 3:
				return 11;

			case 4:
				return 16;

			case 5:
				return 21;

			case 6:
				return 31;

			default:
				return -1;
			}
		}
		internal void SendGameNextTurn(Living living, BaseGame game, List<Box> newBoxes)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(6);
			int num = (int)(this.m_map.wind * 10f);
			gSPacketIn.WriteBoolean(num > 0);
			gSPacketIn.WriteByte(this.GetVane(num, 1));
			gSPacketIn.WriteByte(this.GetVane(num, 2));
			gSPacketIn.WriteByte(this.GetVane(num, 3));
			gSPacketIn.WriteBoolean(living.IsHide);
			gSPacketIn.WriteInt(BaseGame.getTurnTime(base.TimeType));
			gSPacketIn.WriteInt(newBoxes.Count);
			foreach (Box current in newBoxes)
			{
				gSPacketIn.WriteInt(current.Id);
				gSPacketIn.WriteInt(current.X);
				gSPacketIn.WriteInt(current.Y);
				gSPacketIn.WriteInt(current.Type);
			}
			if (living is TurnedLiving)
			{
				List<Player> allFightPlayers = game.GetAllFightPlayers();
				gSPacketIn.WriteInt(allFightPlayers.Count);
				foreach (Player current2 in allFightPlayers)
				{
					gSPacketIn.WriteInt(current2.Id);
					gSPacketIn.WriteBoolean(current2.IsLiving);
					gSPacketIn.WriteInt(current2.X);
					gSPacketIn.WriteInt(current2.Y);
					gSPacketIn.WriteInt(current2.Blood);
					gSPacketIn.WriteBoolean(current2.IsNoHole);
					gSPacketIn.WriteInt(current2.Energy);
					gSPacketIn.WriteInt(current2.psychic);
					gSPacketIn.WriteInt(current2.Dander);
					if (current2.Pet == null)
					{
						gSPacketIn.WriteInt(0);
						gSPacketIn.WriteInt(0);
					}
					else
					{
						gSPacketIn.WriteInt(current2.PetMaxMP);
						gSPacketIn.WriteInt(current2.PetMP);
					}
					gSPacketIn.WriteInt(current2.ShootCount);
				}
				gSPacketIn.WriteInt(game.TurnIndex);
			}
			this.SendToAll(gSPacketIn);
		}
		internal void SendLivingUpdateDirection(Living living)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(7);
			gSPacketIn.WriteInt(living.Direction);
			this.SendToAll(gSPacketIn);
		}
		internal void SendLivingUpdateAngryState(Living living)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(118);
			gSPacketIn.WriteInt(living.State);
			this.SendToAll(gSPacketIn);
		}
		internal void SendEquipEffect(Living player, string buffer)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(3);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteString(buffer);
			this.SendToAll(gSPacketIn);
		}
		internal void SendMessage(IGamePlayer player, string msg, string msg1, int type)
		{
			if (msg != null)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(3);
				gSPacketIn.WriteInt(type);
				gSPacketIn.WriteString(msg);
				player.SendTCP(gSPacketIn);
			}
			if (msg1 != null)
			{
				GSPacketIn gSPacketIn2 = new GSPacketIn(3);
				gSPacketIn2.WriteInt(type);
				gSPacketIn2.WriteString(msg1);
				this.SendToAll(gSPacketIn2, player);
			}
		}
		internal void SendPlayerPicture(Living living, int type, bool state)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.Parameter1 = living.Id;
			gSPacketIn.WriteByte(128);
			gSPacketIn.WriteInt(type);
			gSPacketIn.WriteBoolean(state);
			this.SendToAll(gSPacketIn);
		}
		internal void SendPlayerRemove(Player player)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(94, player.PlayerDetail.PlayerCharacter.ID);
			gSPacketIn.WriteByte(5);
			gSPacketIn.WriteInt(4);
			this.SendToAll(gSPacketIn);
		}
		internal void SendAttackEffect(Living player, int type)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(129);
			gSPacketIn.WriteBoolean(true);
			gSPacketIn.WriteInt(type);
			this.SendToAll(gSPacketIn);
		}
		internal void SendSyncLifeTime()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(131);
			gSPacketIn.WriteInt(this.m_lifeTime);
			this.SendToAll(gSPacketIn);
		}
		protected void OnGameOverred()
		{
			if (this.GameOverred != null)
			{
				this.GameOverred(this);
			}
		}
		protected void OnBeginNewTurn()
		{
			if (this.BeginNewTurn != null)
			{
				this.BeginNewTurn(this);
			}
		}
		public void OnGameOverLog(int _roomId, eRoomType _roomType, eGameType _fightType, int _changeTeam, DateTime _playBegin, DateTime _playEnd, int _userCount, int _mapId, string _teamA, string _teamB, string _playResult, int _winTeam, string BossWar)
		{
			if (this.GameOverLog != null)
			{
				this.GameOverLog(_roomId, _roomType, _fightType, _changeTeam, _playBegin, _playEnd, _userCount, _mapId, _teamA, _teamB, _playResult, _winTeam, this.BossWarField);
			}
		}
		public void OnGameNpcDie(int Id)
		{
			if (this.GameNpcDie != null)
			{
				this.GameNpcDie(Id);
			}
		}
		public override string ToString()
		{
			return string.Format("Id:{0},player:{1},state:{2},current:{3},turnIndex:{4},actions:{5}", new object[]
			{
				base.Id,
				this.PlayerCount,
				this.GameState,
				this.CurrentLiving,
				this.m_turnIndex,
				this.m_actions.Count
			});
		}
	}
}
