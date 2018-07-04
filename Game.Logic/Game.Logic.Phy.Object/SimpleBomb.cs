using Game.Logic.Effects;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Maths;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace Game.Logic.Phy.Object
{
    public class SimpleBomb : BombObject
    {
        private Living m_owner;
        private BaseGame m_game;
        protected Tile m_shape;
        protected int m_radius;
        protected int m_petRadius;
        protected double m_power;
        protected List<BombAction> m_actions;
        protected List<BombAction> m_petActions;
        protected BombType m_type;
        protected bool m_controled;
        private float m_lifeTime;
        private BallInfo m_info;
        private bool m_bombed;
        private bool digMap;
        private int IsEixt = 0;
        private eLivingType m_value;
        public eLivingType Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }
        public bool DigMap
        {
            get
            {
                return this.digMap;
            }
        }
        public BallInfo BallInfo
        {
            get
            {
                return this.m_info;
            }
        }
        public Living Owner
        {
            get
            {
                return this.m_owner;
            }
        }
        public List<BombAction> PetActions
        {
            get
            {
                return this.m_petActions;
            }
        }
        public List<BombAction> Actions
        {
            get
            {
                return this.m_actions;
            }
        }
        public float LifeTime
        {
            get
            {
                return this.m_lifeTime;
            }
        }
        public SimpleBomb(int id, BombType type, Living owner, BaseGame game, BallInfo info, Tile shape, bool controled)
            : base(id, (float)info.Mass, (float)info.Weight, (float)info.Wind, (float)info.DragIndex)
        {
            this.m_owner = owner;
            this.m_game = game;
            this.m_info = info;
            this.m_shape = shape;
            this.m_type = type;
            this.m_power = info.Power;
            this.m_radius = info.Radii;
            this.m_controled = controled;
            this.m_bombed = false;
            this.m_petRadius = 100;
            this.m_lifeTime = 0f;
            this.digMap = true;
        }
        public override void StartMoving()
        {
            base.StartMoving();
            this.m_actions = new List<BombAction>();
            this.m_petActions = new List<BombAction>();
            int arg_27_0 = this.m_game.LifeTime;
            while (this.m_isMoving && this.m_isLiving)
            {
                this.m_lifeTime += 0.04f;
                Point point = base.CompleteNextMovePoint(0.04f);
                base.MoveTo(point.X, point.Y);
                if (this.m_isLiving)
                {
                    if (Math.Round((double)(this.m_lifeTime * 100f)) % 40.0 == 0.0 && point.Y > 0)
                    {
                        this.m_game.AddTempPoint(point.X, point.Y);
                    }
                    if (this.m_controled && base.vY > 0f)
                    {
                        Living living = this.m_map.FindNearestEnemy(this.m_x, this.m_y, 150.0, this.m_owner);
                        if (living != null)
                        {
                            Point point2;
                            if (living is SimpleBoss)
                            {
                                Rectangle directDemageRect = living.GetDirectDemageRect();
                                point2 = new Point(directDemageRect.X - this.m_x, directDemageRect.Y - this.m_y);
                            }
                            else
                            {
                                point2 = new Point(living.X - this.m_x, living.Y - this.m_y);
                            }
                            point2 = point2.Normalize(1000);
                            base.setSpeedXY(point2.X, point2.Y);
                            base.UpdateForceFactor(0f, 0f, 0f);
                            this.m_controled = false;
                            this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.CHANGE_SPEED, point2.X, point2.Y, 0, 0));
                        }
                    }
                }
                if (this.m_bombed)
                {
                    this.m_bombed = false;
                    this.BombImp();
                }
            }
        }
        protected override void CollideObjects(Physics[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Physics physics = list[i];
                physics.CollidedByObject(this);
                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.PICK, physics.Id, 0, 0, 0));
            }
        }
        protected override void CollideGround()
        {
            base.CollideGround();
            this.Bomb();
        }
        public void Bomb()
        {
            this.StopMoving();
            this.m_isLiving = false;
            this.m_bombed = true;
        }
        
        private void BombImp()
        {
            List<Living> list = this.m_map.FindHitByHitPiont(this.GetCollidePoint(), this.m_radius);
            foreach (Living current in list)
            {
                if (current.IsNoHole || current.NoHoleTurn)
                {
                    current.NoHoleTurn = true;
                    this.digMap = false;
                }
                current.SyncAtTime = false;
            }
            this.m_owner.SyncAtTime = false;
            try
            {
                if (this.digMap)
                {
                    this.m_map.Dig(this.m_x, this.m_y, this.m_shape, null);
                }
                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.BOMB, this.m_x, this.m_y, this.digMap ? 1 : 0, 0));
                switch (this.m_type)
                {
                    case BombType.FORZEN:
                        using (List<Living>.Enumerator enumerator2 = list.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                Living current2 = enumerator2.Current;
                                if (this.m_owner is SimpleBoss && new IceFronzeEffect(100).Start(current2))
                                {
                                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FORZEN, current2.Id, 0, 0, 0));
                                }

                                if (current2 is NormalBoss && new IceFronzeEffect(2).Start(current2))
                                {
                                    current2.PlayMovie("cryB", 1000, 0);
                                }
                                if (current2 is NormalNpc && m_map.Info.ID == 1154)
                                {
                                    if (IsEixt == 0 && current2.IsFrost == true)
                                    {
                                        current2.PlayMovie("standC", 1500, 0);
                                        IsEixt = 2;
                                    }

                                    if (IsEixt == 0)
                                    {
                                        current2.PlayMovie("standB", 1500, 0);
                                        current2.IsFrost = true;
                                    }
                                }
                                if (current2 is SimpleNpc && m_map.Info.ID == 1153)
                                {
                                    current2.PlayMovie("cry", 2000, 1000);
                                    current2.Die(3500);
                                }
                                else
                                {
                                    if (new IceFronzeEffect(2).Start(current2))
                                    {
                                        this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FORZEN, current2.Id, 0, 0, 0));
                                    }
                                    else
                                    {
                                        this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FORZEN, -1, 0, 0, 0));
                                        this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.UNANGLE, current2.Id, 0, 0, 0));
                                    }
                                }
                            }
                            goto IL_752;
                        }
                        break;

                    case BombType.TRANFORM:
                        break;

                    case BombType.CURE:
                        using (List<Living>.Enumerator enumerator3 = list.GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                Living current3 = enumerator3.Current;
                                double num;
                                if (this.m_map.FindPlayers(this.GetCollidePoint(), this.m_radius))
                                {
                                    num = 0.4;
                                }
                                else
                                {
                                    num = 1.0;
                                }
                                int num2 = (int)((double)((Player)this.m_owner).PlayerDetail.SecondWeapon.Template.Property7 * Math.Pow(1.1, (double)((Player)this.m_owner).PlayerDetail.SecondWeapon.StrengthenLevel) * num);
                                if (current3 is Player)
                                {
                                    current3.AddBlood(num2);
                                    ((Player)current3).TotalCure += num2;
                                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.CURE, current3.Id, current3.Blood, num2, 0));
                                }
                                if (current3 is SimpleNpc && !current3.NoTakeDamage)
                                {
                                    current3.AddBlood(num2);
                                    ((SimpleNpc)current3).TotalCure += num2;
                                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.CURE, current3.Id, current3.Blood, num2, 0));
                                }

                                if (current3 is NormalNpc)//MrPhuong
                                {
                                    current3.AddBlood(num2);
                                    ((NormalNpc)current3).TotalCure += num2;
                                    m_actions.Add(new BombAction(m_lifeTime, ActionType.CURE, current3.Id, current3.Blood, num2, 0));
                                }
                                if (current3 is NormalBoss)//MrPhuong
                                {
                                    current3.AddBlood(num2);
                                    ((NormalBoss)current3).TotalCure += num2;
                                    m_actions.Add(new BombAction(m_lifeTime, ActionType.CURE, current3.Id, current3.Blood, num2, 0));
                                }
                            }
                            goto IL_752;
                        }
                        goto IL_44C;

                    default:
                        goto IL_44C;
                }
                if (this.m_y > 10 && this.m_lifeTime > 0.04f)
                {
                    if (!this.m_map.IsEmpty(this.m_x, this.m_y))
                    {
                        PointF point = new PointF(-base.vX, -base.vY);
                        point = point.Normalize(5f);
                        this.m_x -= (int)point.X;
                        this.m_y -= (int)point.Y;
                    }
                    this.m_owner.SetXY(this.m_x, this.m_y);
                    this.m_owner.StartMoving();
                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.TRANSLATE, this.m_x, this.m_y, 0, 0));
                    this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.START_MOVE, this.m_owner.Id, this.m_owner.X, this.m_owner.Y, this.m_owner.IsLiving ? 1 : 0));
                    goto IL_752;
                }
                goto IL_752;
            IL_44C:
                foreach (Living current4 in list)
                {
                    if (!this.m_owner.IsFriendly(current4))
                    {
                        int num3 = this.MakeDamage(current4);
                        int num4 = 0;
                        if (num3 != 0)
                        {
                            num4 = this.MakeCriticalDamage(current4, num3);
                            this.m_owner.OnTakedDamage(this.m_owner, ref num3, ref num3);
                            if (current4.TakeDamage(this.m_owner, ref num3, ref num4, "Fire") && m_map.Info.ID != 1151)
                            {
                                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.KILL_PLAYER, current4.Id, num3 + num4, (num4 != 0) ? 2 : 1, current4.Blood));
                            }
                            if (m_map.Info.ID == 1151)
                            {
                                m_actions.Add(new BombAction(m_lifeTime, ActionType.PETSHOOT, current4.Id, num3 + num4, (num4 != 0) ? 2 : 1, current4.Blood));
                                current4.PlayMovie("cryA", (int)((m_lifeTime + 1f) * 1000f), 0);

                            }
                            else
                            {
                                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.UNFORZEN, current4.Id, 0, 0, 0));
                            }
                            if (current4 is Player)
                            {
                                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.DANDER, current4.Id, ((Player)current4).Dander, 0, 0));
                            }
                            if (current4 is SimpleBoss)
                            {
                                ((PVEGame)this.m_game).OnShooted();
                            }
                        }
                        else
                        {
                            if (current4 is SimpleBoss)
                            {
                                this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.PLAYMOVIE, current4.Id, 0, 0, 2));
                            }
                        }
                        m_owner.OnAfterKillingLiving(current4, num3, num4);
                        if (current4.IsLiving)
                        {
                            current4.StartMoving((int)((m_lifeTime + 1f) * 1000f), 12);
                            m_actions.Add(new BombAction(m_lifeTime, ActionType.START_MOVE, current4.Id, current4.X, current4.Y, current4.IsLiving ? 1 : 0));
                        }
                    }
                }
                if (this.m_owner.isPet && this.m_owner.activePetHit)
                {
                    list = this.m_map.FindHitByHitPiont(this.GetCollidePoint(), this.m_petRadius);
                    foreach (Living current5 in list)
                    {
                        int num5 = 0;
                        int num6 = 0;
                        if (current5 != this.m_owner)
                        {
                            num5 = this.MakePetDamage(current5) * this.m_owner.PetBaseAtt / 100;
                            num6 = this.MakeCriticalDamage(current5, num5);
                            if (current5.PetTakeDamage(this.m_owner, ref num5, ref num6, "PetFire"))
                            {
                                if (current5 is Player)
                                {
                                    this.m_petActions.Add(new BombAction(this.m_lifeTime, ActionType.PETSHOOT, current5.Id, num5 + num6, ((Player)current5).Dander, current5.Blood));
                                }
                                else
                                {
                                    this.m_petActions.Add(new BombAction(this.m_lifeTime, ActionType.PETSHOOT, current5.Id, num5 + num6, 1, current5.Blood));
                                }
                            }
                        }
                    }
                    this.m_owner.activePetHit = false;
                }
            IL_752:
                this.Die();
            }
            finally
            {
                this.m_owner.SyncAtTime = true;
                foreach (Living current6 in list)
                {
                    current6.SyncAtTime = true;
                }
            }
        }
        protected int MakeDamage(Living target)
        {
            double baseDamage = this.m_owner.BaseDamage;
            double arg_12_0 = target.BaseGuard;
            double arg_19_0 = target.Defence;
            double attack = this.m_owner.Attack;
            bool arg_31_0 = this.m_owner.IgnoreArmor;
            float currentDamagePlus = this.m_owner.CurrentDamagePlus;
            float currentShootMinus = this.m_owner.CurrentShootMinus;
            double num = 0.95 * (target.BaseGuard - (double)(3 * this.m_owner.Grade)) / (500.0 + target.BaseGuard - (double)(3 * this.m_owner.Grade));
            double num2;
            if (target.Defence - this.m_owner.Lucky < 0.0)
            {
                num2 = 0.0;
            }
            else
            {
                num2 = 0.95 * (target.Defence - this.m_owner.Lucky) / (600.0 + target.Defence - this.m_owner.Lucky);
            }
            double num3 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num + num2 - num * num2)) * (double)currentDamagePlus * (double)currentShootMinus;
            Point p = new Point(this.X, this.Y);
            double num4 = target.Distance(p);
            if (num4 >= (double)this.m_radius)
            {
                return 0;
            }
            num3 *= 1.0 - num4 / (double)this.m_radius / 4.0;
            if (num3 < 0.0)
            {
                return 1;
            }
            return (int)num3;
        }
        protected int MakePetDamage(Living target)
        {
            double baseDamage = this.m_owner.BaseDamage;
            double arg_12_0 = target.BaseGuard;
            double arg_19_0 = target.Defence;
            double attack = this.m_owner.Attack;
            bool arg_31_0 = this.m_owner.IgnoreArmor;
            float currentDamagePlus = this.m_owner.CurrentDamagePlus;
            float currentShootMinus = this.m_owner.CurrentShootMinus;
            double num = 0.95 * (target.BaseGuard - (double)(3 * this.m_owner.Grade)) / (500.0 + target.BaseGuard - (double)(3 * this.m_owner.Grade));
            double num2;
            if (target.Defence - this.m_owner.Lucky < 0.0)
            {
                num2 = 0.0;
            }
            else
            {
                num2 = 0.95 * (target.Defence - this.m_owner.Lucky) / (600.0 + target.Defence - this.m_owner.Lucky);
            }
            double num3 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num + num2 - num * num2)) * (double)currentDamagePlus * (double)currentShootMinus;
            Point p = new Point(this.X, this.Y);
            double num4 = target.Distance(p);
            if (num4 >= (double)this.m_petRadius)
            {
                return 0;
            }
            num3 *= 1.0 - num4 / (double)this.m_petRadius / 4.0;
            if (num3 < 0.0)
            {
                return 1;
            }
            return (int)num3;
        }
        protected int MakeCriticalDamage(Living target, int baseDamage)
        {
            double lucky = this.m_owner.Lucky;
            Random random = new Random();
            bool flag = lucky * 75.0 / (800.0 + lucky) > (double)random.Next(100);
            if (this.m_owner.critActive)
            {
                flag = this.m_owner.critActive;
                this.m_owner.critActive = false;
            }
            if (flag)
            {
                return (int)((0.5 + lucky * 0.0003) * (double)baseDamage);
            }
            return 0;
        }
        protected override void FlyoutMap()
        {
            this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FLY_OUT, 0, 0, 0, 0));
            base.FlyoutMap();
        }
    }
}
