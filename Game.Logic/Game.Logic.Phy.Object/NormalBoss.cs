namespace Game.Logic.Phy.Object
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using Game.Logic;
    using Game.Logic.Actions;
    using Game.Logic.AI;
    using Game.Logic.AI.Npc;
    using Game.Server.Managers;
    using log4net;

    public class NormalBoss : TurnedLiving
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ABrain m_ai;
        private List<SimpleBoss> m_boss;
        private List<SimpleNpc> m_child;
        private Dictionary<Player, int> m_mostHateful;
        private List<NormalNpc> m_npc;
        private SqlDataProvider.Data.NpcInfo m_npcInfo;
        public int TotalCure;

        public NormalBoss(int id, BaseGame game, SqlDataProvider.Data.NpcInfo npcInfo, int direction, int type) : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
        {
            this.m_child = new List<SimpleNpc>();
            this.m_boss = new List<SimpleBoss>();
            this.m_npc = new List<NormalNpc>();
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
            this.m_mostHateful = new Dictionary<Player, int>();
            this.m_npcInfo = npcInfo;
            this.m_ai = ScriptMgr.CreateInstance(npcInfo.Script) as ABrain;
            if (this.m_ai == null)
            {
                log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
                this.m_ai = SimpleBrain.Simple;
            }
            this.m_ai.Game = base.m_game;
            this.m_ai.Body = this;
            try
            {
                this.m_ai.OnCreated();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss Created error:{1}", ex);
            }
        }

        public void CreateBoss(int id, int x, int y, int direction, int disToSecond, int maxCount)
        {
            if (this.CurrentLivingBossNum < maxCount)
            {
                if ((maxCount - this.CurrentLivingNpcNum) >= 2)
                {
                    this.Boss.Add(((PVEGame)base.Game).CreateBoss(id, x + disToSecond, y, direction, 0, ""));
                    this.Boss.Add(((PVEGame)base.Game).CreateBoss(id, x, y, direction, 0, ""));
                }
                else if ((maxCount - this.CurrentLivingBossNum) == 1)
                {
                    this.Boss.Add(((PVEGame)base.Game).CreateBoss(id, x, y, direction, 0, ""));
                }
            }
        }

        public void CreateChild(int id, int x, int y, int direction, int disToSecond, int maxCount)
        {
            if (this.CurrentLivingNpcNum < maxCount)
            {
                if ((maxCount - this.CurrentLivingNpcNum) >= 2)
                {
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x + disToSecond, y, direction, 1));
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x, y, direction, 1));
                }
                else if ((maxCount - this.CurrentLivingNpcNum) == 1)
                {
                    this.Child.Add(((PVEGame) base.Game).CreateNpc(id, x, y, direction, 1));
                }
            }
        }

        public override void Die()
        {
            base.Die();
        }

        public override void Die(int delay)
        {
            base.Die(delay);
        }

        public override void Dispose()
        {
            base.Dispose();
            try
            {
                this.m_ai.Dispose();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss Dispose error:{1}", ex);
            }
        }

        public Player FindMostHatefulPlayer()
        {
            if (this.m_mostHateful.Count <= 0)
            {
                return null;
            }
            KeyValuePair<Player, int> k = this.m_mostHateful.ElementAt<KeyValuePair<Player, int>>(0);
            foreach (KeyValuePair<Player, int> kvp in this.m_mostHateful)
            {
                if (k.Value < kvp.Value)
                {
                    k = kvp;
                }
            }
            return k.Key;
        }

        public override void PrepareNewTurn()
        {
            base.PrepareNewTurn();
            try
            {
                this.m_ai.OnBeginNewTurn();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss BeginNewTurn error:{1}", ex);
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
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss BeginSelfTurn error:{1}", ex);
            }
        }

        public void RandomSay(string[] msg, int type, int delay, int finishTime)
        {
            string[] content = msg;
            string text = null;
            if (base.Game.Random.Next(0, 2) == 1)
            {
                int index = base.Game.Random.Next(0, content.Count<string>());
                text = content[index];
                base.m_game.AddAction(new LivingSayAction(this, text, type, delay, finishTime));
            }
        }

        public override void Reset()
        {
            base.m_maxBlood = this.m_npcInfo.Blood;
            
            base.BaseDamage = this.m_npcInfo.BaseDamage;
            base.BaseGuard = this.m_npcInfo.BaseGuard;
            base.Attack = this.m_npcInfo.Attack;
            base.Defence = this.m_npcInfo.Defence;
            base.Agility = this.m_npcInfo.Agility;
            base.Lucky = this.m_npcInfo.Lucky;
            base.Grade = this.m_npcInfo.Level;
            base.Experience = this.m_npcInfo.Experience;
            base.SetRect(this.m_npcInfo.X, this.m_npcInfo.Y, this.m_npcInfo.Width, this.m_npcInfo.Height);
            base.Reset();
        }

        public override void StartAttacking()
        {
            base.StartAttacking();
            try
            {
                this.m_ai.OnStartAttacking();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss StartAttacking error:{1}", ex);
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
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss StopAttacking error:{1}", ex);
            }
        }

        public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            bool result = false;
            result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
            if (source is Player)
            {
                Player p = source as Player;
                int damage = damageAmount + criticalAmount;
                if (this.m_mostHateful.ContainsKey(p))
                {
                    this.m_mostHateful[p] += damage;
                    return result;
                }
                this.m_mostHateful.Add(p, damage);
            }
            return result;
        }

        public List<SimpleBoss> Boss
        {
            get
            {
                return this.m_boss;
            }
        }

        public List<SimpleNpc> Child
        {
            get
            {
                return this.m_child;
            }
        }

        public int CurrentLivingBossNum
        {
            get
            {
                int count = 0;
                foreach (SimpleBoss boss in this.Boss)
                {
                    if (!boss.IsLiving)
                    {
                        count++;
                    }
                }
                return (this.Boss.Count - count);
            }
        }

        public int CurrentLivingNpc
        {
            get
            {
                int count = 0;
                foreach (NormalNpc child in this.Npc)
                {
                    if (!child.IsLiving)
                    {
                        count++;
                    }
                }
                return (this.Child.Count - count);
            }
        }

        public int CurrentLivingNpcNum
        {
            get
            {
                int count = 0;
                foreach (SimpleNpc child in this.Child)
                {
                    if (!child.IsLiving)
                    {
                        count++;
                    }
                }
                return (this.Child.Count - count);
            }
        }

        public List<NormalNpc> Npc
        {
            get
            {
                return this.m_npc;
            }
        }

        public SqlDataProvider.Data.NpcInfo NpcInfo
        {
            get
            {
                return this.m_npcInfo;
            }
        }
    }
}

