using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
namespace Game.Logic.Phy.Object
{
    public class SimpleBoss : TurnedLiving
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private NpcInfo m_npcInfo;
        private ABrain m_ai;
        private List<SimpleNpc> m_child = new List<SimpleNpc>();
        private List<SimpleBoss> m_boss = new List<SimpleBoss>();
        private Dictionary<Player, int> m_mostHateful;
        public NpcInfo NpcInfo
        {
            get
            {
                return this.m_npcInfo;
            }
        }
        public List<SimpleNpc> Child
        {
            get
            {
                return this.m_child;
            }
        }
        public int CurrentLivingNpcNum
        {
            get
            {
                int num = 0;
                foreach (SimpleNpc current in this.Child)
                {
                    if (!current.IsLiving)
                    {
                        num++;
                    }
                }
                return this.Child.Count - num;
            }
        }
        public List<SimpleBoss> Boss
        {
            get
            {
                return this.m_boss;
            }
        }
        public int CurrentLivingBossNum
        {
            get
            {
                int num = 0;
                foreach (SimpleBoss current in this.Boss)
                {
                    if (!current.IsLiving)
                    {
                        num++;
                    }
                }
                return this.Boss.Count - num;
            }
        }
        public SimpleBoss(int id, BaseGame game, NpcInfo npcInfo, int direction, int type, string actions)
            : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
        {
            if (type == 0)
            {
                base.Type = eLivingType.SimpleBossSpecial;
            }
            if (type == 10)
            {
                base.Type = eLivingType.SimpleNpcNormal;
            }
            else
            {
                base.Type = eLivingType.SimpleBossHard;
            }
            base.ActionStr = actions;
            this.m_mostHateful = new Dictionary<Player, int>();
            this.m_npcInfo = npcInfo;
            this.m_ai = (ScriptMgr.CreateInstance(npcInfo.Script) as ABrain);
            if (this.m_ai == null)
            {
                SimpleBoss.log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
                this.m_ai = SimpleBrain.Simple;
            }
            this.m_ai.Game = this.m_game;
            this.m_ai.Body = this;
            try
            {
                this.m_ai.OnCreated();
            }
            catch (Exception arg)
            {
                SimpleBoss.log.ErrorFormat("SimpleBoss Created error:{1}", arg);
            }
        }
        public override void Reset()
        {
            this.m_maxBlood = ((this.m_npcInfo.Blood == -1) ? 8000000 : this.m_npcInfo.Blood);
            this.BaseDamage = (double)this.m_npcInfo.BaseDamage;
            this.BaseGuard = (double)this.m_npcInfo.BaseGuard;
            this.Attack = (double)this.m_npcInfo.Attack;
            this.Defence = (double)this.m_npcInfo.Defence;
            this.Agility = (double)this.m_npcInfo.Agility;
            this.Lucky = (double)this.m_npcInfo.Lucky;
            this.Grade = this.m_npcInfo.Level;
            this.Experience = this.m_npcInfo.Experience;
            base.SetRect(this.m_npcInfo.X, this.m_npcInfo.Y, this.m_npcInfo.Width, this.m_npcInfo.Height);
            base.Reset();
        }
        public override void Die()
        {
            base.Die();
        }
        public override void Die(int delay)
        {
            base.Die(delay);
        }
        public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            bool result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
            if (source is Player)
            {
                Player key = source as Player;
                int num = damageAmount + criticalAmount;
                if (this.m_mostHateful.ContainsKey(key))
                {
                    this.m_mostHateful[key] = this.m_mostHateful[key] + num;
                }
                else
                {
                    this.m_mostHateful.Add(key, num);
                }
            }
            return result;
        }
        public Player FindMostHatefulPlayer()
        {
            if (this.m_mostHateful.Count > 0)
            {
                KeyValuePair<Player, int> keyValuePair = this.m_mostHateful.ElementAt(0);
                foreach (KeyValuePair<Player, int> current in this.m_mostHateful)
                {
                    if (keyValuePair.Value < current.Value)
                    {
                        keyValuePair = current;
                    }
                }
                return keyValuePair.Key;
            }
            return null;
        }
        public void CreateChild(int id, int x, int y, int direction, int disToSecond, int maxCount)
        {
            if (this.CurrentLivingNpcNum < maxCount)
            {
                if (maxCount - this.CurrentLivingNpcNum >= 2)
                {
                    this.Child.Add(((PVEGame)base.Game).CreateNpc(id, x + disToSecond, y, direction, 1));
                    this.Child.Add(((PVEGame)base.Game).CreateNpc(id, x, y, direction, 1));
                    return;
                }
                if (maxCount - this.CurrentLivingNpcNum == 1)
                {
                    this.Child.Add(((PVEGame)base.Game).CreateNpc(id, x, y, direction, 1));
                }
            }
        }
        private List<NormalNpc> m_npc = new List<NormalNpc>();
        public List<NormalNpc> Npc
        {
            get { return m_npc; }
        }

        public int CurrentLivingNormalNpc
        {
            get
            {
                int count = 0;
                foreach (NormalNpc child in Npc)
                {
                    if (child.IsLiving == false)
                    {
                        count++;
                    }
                }
                return Child.Count - count;
            }
        }
        public void CreateNpc(int id, int x, int y, int direction, int disToSecond, int maxCount)
        {
            if (CurrentLivingBossNum < maxCount)
            {
                if (maxCount - CurrentLivingNpcNum >= 2)
                {
                    Npc.Add(((PVEGame)Game).CreateNormal(id, x + disToSecond, y, direction, 0));
                    Npc.Add(((PVEGame)Game).CreateNormal(id, x, y, direction, 0));
                }
                else if (maxCount - CurrentLivingBossNum == 1)
                {
                    Npc.Add(((PVEGame)Game).CreateNormal(id, x, y, direction, 0));
                }
            }
        }
        public void CreateChild(int id, Point[] brithPoint, int direction, int maxCount, int maxCountForOnce, int type)
        {
            int num = base.Game.Random.Next(0, maxCountForOnce);
            for (int i = 0; i < num; i++)
            {
                int num2 = base.Game.Random.Next(0, brithPoint.Length);
                this.CreateChild(id, brithPoint[num2].X, brithPoint[num2].Y, 4, maxCount, direction);
            }
        }
        public void CreateBoss(int id, int x, int y, int direction, int disToSecond, int maxCount)
        {
            if (this.CurrentLivingBossNum < maxCount)
            {
                if (maxCount - this.CurrentLivingNpcNum >= 2)
                {
                    this.Boss.Add(((PVEGame)base.Game).CreateBoss(id, x + disToSecond, y, direction, 10));
                    this.Boss.Add(((PVEGame)base.Game).CreateBoss(id, x, y, direction, 10));
                    return;
                }
                if (maxCount - this.CurrentLivingBossNum == 1)
                {
                    this.Boss.Add(((PVEGame)base.Game).CreateBoss(id, x, y, direction, 10));
                }
            }
        }
        public void RandomSay(string[] msg, int type, int delay, int finishTime)
        {
            int num = base.Game.Random.Next(0, 2);
            if (num == 1)
            {
                int num2 = base.Game.Random.Next(0, msg.Count<string>());
                string msg2 = msg[num2];
                this.m_game.AddAction(new LivingSayAction(this, msg2, type, delay, finishTime));
            }
        }
        public override void PrepareNewTurn()
        {
            base.PrepareNewTurn();
            try
            {
                this.m_ai.OnBeginNewTurn();
            }
            catch (Exception arg)
            {
                SimpleBoss.log.ErrorFormat("SimpleBoss BeginNewTurn error:{1}", arg);
            }
        }
        public override void PrepareSelfTurn()
        {
            base.PrepareSelfTurn();
            base.AddDelay(this.m_npcInfo.Delay);
            try
            {
                this.m_ai.OnBeginSelfTurn();
            }
            catch (Exception arg)
            {
                SimpleBoss.log.ErrorFormat("SimpleBoss BeginSelfTurn error:{1}", arg);
            }
        }
        public override void StartAttacking()
        {
            base.StartAttacking();
            try
            {
                this.m_ai.OnStartAttacking();
            }
            catch (Exception arg)
            {
                SimpleBoss.log.ErrorFormat("SimpleBoss StartAttacking error:{1}", arg);
            }
            if (base.IsAttacking)
            {
                this.StopAttacking();
            }
        }
        public override void StopAttacking()
        {
            base.StopAttacking();
            try
            {
                this.m_ai.OnStopAttacking();
            }
            catch (Exception arg)
            {
                SimpleBoss.log.ErrorFormat("SimpleBoss StopAttacking error:{1}", arg);
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            try
            {
                this.m_ai.Dispose();
            }
            catch (Exception arg)
            {
                SimpleBoss.log.ErrorFormat("SimpleBoss Dispose error:{1}", arg);
            }
        }
    }
}
