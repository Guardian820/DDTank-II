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

    public class NormalNpc : Living
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ABrain m_ai;
        private SqlDataProvider.Data.NpcInfo m_npcInfo;
        public int TotalCure;

        public NormalNpc(int id, BaseGame game, SqlDataProvider.Data.NpcInfo npcInfo, int direction, int type) : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
        {
            if (type == 0)
            {
                base.Type = eLivingType.SimpleNpcSpecial;
            }
            if (type == 10)
            {
                base.Type = eLivingType.SimpleNpcNormal;
            }
            else
            {
                base.Type = eLivingType.SimpleNpcNormal;
            }
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
                log.ErrorFormat("SimpleNpc Created error:{1}", ex);
            }
        }

        public override void Die()
        {
            this.GetDropItemInfo();
            base.Die();
        }

        public override void Die(int delay)
        {
            this.GetDropItemInfo();
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
                log.ErrorFormat("SimpleNpc Dispose error:{1}", ex);
            }
        }

        public void GetDropItemInfo()
        {
            if (base.m_game.CurrentLiving is Player)
            {
                Player p = base.m_game.CurrentLiving as Player;
                List<SqlDataProvider.Data.ItemInfo> infos = null;
                int gold = 0;
                int money = 0;
                int gifttoken = 0;
                int medal = 0;
                DropInventory.NPCDrop(this.m_npcInfo.DropId, ref infos);
                if (infos != null)
                {
                    foreach (SqlDataProvider.Data.ItemInfo info in infos)
                    {
                        SqlDataProvider.Data.ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref gifttoken, ref medal);
                        if (info != null)
                        {
                            if (info.Template.CategoryID == 10)
                            {
                                p.PlayerDetail.AddTemplate(info, eBageType.FightBag, info.Count);
                            }
                            else
                            {
                                p.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count);
                            }
                        }
                    }
                    p.PlayerDetail.AddGold(gold);
                    p.PlayerDetail.AddMoney(money);
                    p.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_Drop, p.PlayerDetail.PlayerCharacter.ID, money, p.PlayerDetail.PlayerCharacter.Money);
                    p.PlayerDetail.AddGiftToken(gifttoken);
                    p.PlayerDetail.AddMedal(medal);
                }
            }
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
                log.ErrorFormat("SimpleNpc BeginNewTurn error:{1}", ex);
            }
        }

        public override void Reset()
        {
            base.m_maxBlood = this.m_npcInfo.Blood / 2;
            base.Agility = this.m_npcInfo.Agility;
            base.Attack = this.m_npcInfo.Attack;
            base.BaseDamage = this.m_npcInfo.BaseDamage;
            base.BaseGuard = this.m_npcInfo.BaseGuard;
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
                log.ErrorFormat("SimpleNpc StartAttacking error:{1}", ex);
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

