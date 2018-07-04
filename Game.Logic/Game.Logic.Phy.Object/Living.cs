using Bussiness;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace Game.Logic.Phy.Object
{
    public class Living : Physics
    {
        protected BaseGame m_game;
        protected int m_maxBlood;
        protected int m_blood;
        private int m_team;
        private string m_name;
        private string m_action;
        private string m_modelId;
        private Rectangle m_demageRect;
        private int m_state;
        public int m_direction;
        private eLivingType m_type;
        public double BaseDamage = 10.0;
        public double BaseGuard = 10.0;
        public double Defence = 10.0;
        public double Attack = 10.0;
        public double Agility = 10.0;
        public double Lucky = 10.0;
        public int Grade = 1;
        public int Experience = 10;
        public float CurrentDamagePlus;
        public float CurrentShootMinus;
        public bool IgnoreArmor;
        public bool ControlBall;
        public bool NoHoleTurn;
        public bool CurrentIsHitTarget;
        public int TurnNum;
        public int TotalHurt;
        public int TotalHitTargetCount;
        public int TotalShootCount;
        public int TotalKill;
        public int MaxBeatDis;
        public int EffectsCount;
        public int ShootMovieDelay;
        private EffectList m_effectList;
        public bool EffectTrigger;
        private bool m_noTakeDamage;
        protected bool m_syncAtTime;
        private bool m_isPet;
        private bool m_activePetHit;
        private int m_petBaseAtt;
        private bool m_critActive;
        private bool m_vaneOpen;
        private bool m_isAttacking;
        protected static int STEP_X = 2;
        protected static int STEP_Y = 7;
        private bool m_isFrost;
        private bool m_isHide;
        private bool m_isNoHole;
        private bool m_isSeal;
        public event LivingEventHandle Died;
        public event LivingTakedDamageEventHandle BeforeTakeDamage;
        public event LivingTakedDamageEventHandle TakePlayerDamage;
        public event LivingEventHandle BeginNextTurn;
        public event LivingEventHandle BeginSelfTurn;
        public event LivingEventHandle BeginAttacking;
        public event LivingEventHandle BeginAttacked;
        public event LivingEventHandle EndAttacking;
        public event KillLivingEventHanlde AfterKillingLiving;
        public event KillLivingEventHanlde AfterKilledByLiving;
        public bool vaneOpen
        {
            get
            {
                return this.m_vaneOpen;
            }
            set
            {
                this.m_vaneOpen = value;
            }
        }
        public bool critActive
        {
            get
            {
                return this.m_critActive;
            }
            set
            {
                this.m_critActive = value;
            }
        }
        public bool isPet
        {
            get
            {
                return this.m_isPet;
            }
            set
            {
                this.m_isPet = value;
            }
        }
        public bool activePetHit
        {
            get
            {
                return this.m_activePetHit;
            }
            set
            {
                this.m_activePetHit = value;
            }
        }
        public int PetBaseAtt
        {
            get
            {
                return this.m_petBaseAtt;
            }
            set
            {
                this.m_petBaseAtt = value;
            }
        }
        public bool NoTakeDamage
        {
            get
            {
                return this.m_noTakeDamage;
            }
            set
            {
                this.m_noTakeDamage = value;
            }
        }
        public string ActionStr
        {
            get
            {
                return this.m_action;
            }
            set
            {
                this.m_action = value;
            }
        }
        public BaseGame Game
        {
            get
            {
                return this.m_game;
            }
        }
        public string Name
        {
            get
            {
                return this.m_name;
            }
        }
        public string ModelId
        {
            get
            {
                return this.m_modelId;
            }
        }
        public int Team
        {
            get
            {
                return this.m_team;
            }
        }
        public bool SyncAtTime
        {
            get
            {
                return this.m_syncAtTime;
            }
            set
            {
                this.m_syncAtTime = value;
            }
        }
        public int Direction
        {
            get
            {
                return this.m_direction;
            }
            set
            {
                if (this.m_direction != value)
                {
                    this.m_direction = value;
                    base.SetRect(-this.m_rect.X - this.m_rect.Width, this.m_rect.Y, this.m_rect.Width, this.m_rect.Height);
                    base.SetRectBomb(-this.m_rectBomb.X - this.m_rectBomb.Width, this.m_rectBomb.Y, this.m_rectBomb.Width, this.m_rectBomb.Height);
                    this.SetRelateDemagemRect(-this.m_demageRect.X - this.m_demageRect.Width, this.m_demageRect.Y, this.m_demageRect.Width, this.m_demageRect.Height);
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendLivingUpdateDirection(this);
                    }
                }
            }
        }
        public eLivingType Type
        {
            get
            {
                return this.m_type;
            }
            set
            {
                this.m_type = value;
            }
        }
        public bool IsSay
        {
            get;
            set;
        }
        public EffectList EffectList
        {
            get
            {
                return this.m_effectList;
            }
        }
        public bool IsAttacking
        {
            get
            {
                return this.m_isAttacking;
            }
        }
        public bool IsFrost
        {
            get
            {
                return this.m_isFrost;
            }
            set
            {
                if (this.m_isFrost != value)
                {
                    this.m_isFrost = value;
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendGameUpdateFrozenState(this);
                    }
                }
            }
        }
        public bool IsNoHole
        {
            get
            {
                return this.m_isNoHole;
            }
            set
            {
                if (this.m_isNoHole != value)
                {
                    this.m_isNoHole = value;
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendGameUpdateNoHoleState(this);
                    }
                }
            }
        }
        public bool IsHide
        {
            get
            {
                return this.m_isHide;
            }
            set
            {
                if (this.m_isHide != value)
                {
                    this.m_isHide = value;
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendGameUpdateHideState(this);
                    }
                }
            }
        }
        public int State
        {
            get
            {
                return this.m_state;
            }
            set
            {
                if (this.m_state != value)
                {
                    this.m_state = value;
                    if (this.m_syncAtTime)
                    {
                        this.m_game.SendLivingUpdateAngryState(this);
                    }
                }
            }
        }
        public int MaxBlood
        {
            get
            {
                return this.m_maxBlood;
            }
        }
        public int Blood
        {
            get
            {
                return this.m_blood;
            }
            set
            {
                this.m_blood = value;
            }
        }
        public Living(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction)
            : base(id)
        {
            this.m_vaneOpen = false;
            this.m_critActive = false;
            this.m_petBaseAtt = 0;
            this.m_activePetHit = false;
            this.m_isPet = false;
            this.m_noTakeDamage = true;
            this.m_action = "";
            this.m_game = game;
            this.m_team = team;
            this.m_name = name;
            this.m_modelId = modelId;
            this.m_maxBlood = ((maxBlood == -1) ? 8000000 : maxBlood);
            this.m_direction = direction;
            this.m_state = 0;
            this.MaxBeatDis = 100;
            this.m_effectList = new EffectList(this, immunity);
            this.m_syncAtTime = true;
            this.m_type = eLivingType.Living;
        }
        public void SetRelateDemagemRect(int x, int y, int width, int height)
        {
            this.m_demageRect.X = x;
            this.m_demageRect.Y = y;
            this.m_demageRect.Width = width;
            this.m_demageRect.Height = height;
        }
        public Point GetShootPoint()
        {
            if (this is SimpleBoss)
            {
                if (this.m_direction <= 0)
                {
                    return new Point(this.X + ((SimpleBoss)this).NpcInfo.FireX, this.Y + ((SimpleBoss)this).NpcInfo.FireY);
                }
                return new Point(this.X - ((SimpleBoss)this).NpcInfo.FireX, this.Y + ((SimpleBoss)this).NpcInfo.FireY);
            }
            else
            {
                if (this.m_direction <= 0)
                {
                    return new Point(this.X + this.m_rect.X - 5, this.Y + this.m_rect.Y - 5);
                }
                return new Point(this.X - this.m_rect.X + 5, this.Y + this.m_rect.Y - 5);
            }
        }
        public Rectangle GetDirectDemageRect()
        {
            return new Rectangle(this.X + this.m_demageRect.X, this.Y + this.m_demageRect.Y, this.m_demageRect.Width, this.m_demageRect.Height);
        }
        public List<Rectangle> GetDirectBoudRect()
        {
            return new List<Rectangle>
			{
				new Rectangle(this.X + base.Bound.X, this.Y + base.Bound.Y, base.Bound.Width, base.Bound.Height),
				new Rectangle(this.X + base.Bound1.X, this.Y + base.Bound1.Y, base.Bound1.Width, base.Bound1.Height)
			};
        }
        public double Distance(Point p)
        {
            List<double> list = new List<double>();
            Rectangle directDemageRect = this.GetDirectDemageRect();
            for (int i = directDemageRect.X; i <= directDemageRect.X + directDemageRect.Width; i += 10)
            {
                list.Add(Math.Sqrt((double)((i - p.X) * (i - p.X) + (directDemageRect.Y - p.Y) * (directDemageRect.Y - p.Y))));
                list.Add(Math.Sqrt((double)((i - p.X) * (i - p.X) + (directDemageRect.Y + directDemageRect.Height - p.Y) * (directDemageRect.Y + directDemageRect.Height - p.Y))));
            }
            for (int j = directDemageRect.Y; j <= directDemageRect.Y + directDemageRect.Height; j += 10)
            {
                list.Add(Math.Sqrt((double)((directDemageRect.X - p.X) * (directDemageRect.X - p.X) + (j - p.Y) * (j - p.Y))));
                list.Add(Math.Sqrt((double)((directDemageRect.X + directDemageRect.Width - p.X) * (directDemageRect.X + directDemageRect.Width - p.X) + (j - p.Y) * (j - p.Y))));
            }
            return list.Min();
        }
        public double BoundDistance(Point p)
        {
            List<double> list = new List<double>();
            foreach (Rectangle current in this.GetDirectBoudRect())
            {
                for (int i = current.X; i <= current.X + current.Width; i += 10)
                {
                    list.Add(Math.Sqrt((double)((i - p.X) * (i - p.X) + (current.Y - p.Y) * (current.Y - p.Y))));
                    list.Add(Math.Sqrt((double)((i - p.X) * (i - p.X) + (current.Y + current.Height - p.Y) * (current.Y + current.Height - p.Y))));
                }
                for (int j = current.Y; j <= current.Y + current.Height; j += 10)
                {
                    list.Add(Math.Sqrt((double)((current.X - p.X) * (current.X - p.X) + (j - p.Y) * (j - p.Y))));
                    list.Add(Math.Sqrt((double)((current.X + current.Width - p.X) * (current.X + current.Width - p.X) + (j - p.Y) * (j - p.Y))));
                }
            }
            return list.Min();
        }
        public virtual void Reset()
        {
            if (this.NoTakeDamage)
            {
                this.m_blood = this.m_maxBlood;
            }
            else
            {
                this.m_blood = this.m_maxBlood / 2;
            }
            this.m_isFrost = false;
            this.m_isHide = false;
            this.m_isNoHole = false;
            this.m_isLiving = true;
            this.TurnNum = 0;
            this.TotalHurt = 0;
            this.TotalKill = 0;
            this.TotalShootCount = 0;
            this.TotalHitTargetCount = 0;
        }
        public virtual void PickBox(Box box)
        {
            box.UserID = base.Id;
            box.Die();
            if (this.m_syncAtTime)
            {
                this.m_game.SendGamePickBox(this, box.Id, 0, "");
            }
        }
        public override void PrepareNewTurn()
        {
            this.ShootMovieDelay = 0;
            this.CurrentDamagePlus = 1f;
            this.CurrentShootMinus = 1f;
            this.IgnoreArmor = false;
            this.ControlBall = false;
            this.NoHoleTurn = false;
            this.CurrentIsHitTarget = false;
            this.OnBeginNewTurn();
        }
        public virtual void PrepareSelfTurn()
        {
            this.OnBeginSelfTurn();
        }
        public void StartAttacked()
        {
            this.OnStartAttacked();
        }
        public virtual void StartAttacking()
        {
            if (!this.m_isAttacking)
            {
                this.m_isAttacking = true;
                this.OnStartAttacking();
            }
        }
        public virtual void StopAttacking()
        {
            if (this.m_isAttacking)
            {
                this.m_isAttacking = false;
                this.OnStopAttacking();
            }
        }
        public override void CollidedByObject(Physics phy)
        {
            if (phy is SimpleBomb)
            {
                ((SimpleBomb)phy).Bomb();
            }
        }
        public override void StartMoving()
        {
            this.StartMoving(0, 30);
        }
        public virtual void StartMoving(int delay, int speed)
        {
            /*if (this.m_map.IsEmpty(this.X, this.Y) && this.Type != eLivingType.SimpleBossSpecial && this.Type != eLivingType.SimpleNpcSpecial)
            {
                this.FallFrom(this.X, this.Y, null, delay, 0, speed);
            }*/
            if (this is SimpleNpc)
            {
                FallFrom(X, Y, null, delay, 0, speed);
            }
            if (this is SimpleBoss)
            {
                this.FallFromTo(this.X, this.Y, "", 0, 0, speed, null);
            }
            base.StartMoving();
        }
        public void SetXY(int x, int y, int delay)
        {
            this.m_game.AddAction(new LivingDirectSetXYAction(this, x, y, delay));
        }
        public void AddEffect(AbstractEffect effect, int delay)
        {
            this.m_game.AddAction(new LivingDelayEffectAction(this, effect, delay));
        }
        public void Say(string msg, int type, int delay, int finishTime)
        {
            this.m_game.AddAction(new LivingSayAction(this, msg, type, delay, finishTime));
        }
        public void Say(string msg, int type, int delay)
        {
            this.m_game.AddAction(new LivingSayAction(this, msg, type, delay, 1000));
        }

        public bool MoveTo(int x, int y, string action, int delay)
        {
            return this.MoveTo(x, y, action, delay, "", 4, null);
        }

        public bool MoveTo(int x, int y, string action, int delay, LivingCallBack callback)
        {
            return this.MoveTo(x, y, action, delay, "", 4, callback);
        }

        public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback)
        {
            bool result;
            if (this.m_x == x && this.m_y == y)
            {
                result = false;
            }
            else
            {
                if (x < 0 || x > this.m_map.Bound.Width)
                {
                    result = false;
                }
                else
                {
                    List<Point> path = new List<Point>();
                    int tx = this.m_x;
                    int ty = this.m_y;
                    int direction = (x > tx) ? 1 : -1;
                    if (action == "fly")
                    {
                        Point p = new Point(x, y);
                        Point p2 = new Point(tx - p.X, ty - p.Y);
                        p = new Point(p.X + p2.X, y + p2.X);
                        p2 = new Point(x - p.X, x - p.Y);
                        if (p != Point.Empty)
                        {
                        }
                        p = new Point(x, y);
                        path.Add(p);
                    }
                    else
                    {
                        while ((x - tx) * direction > 0)
                        {
                            Point p = this.m_map.FindNextWalkPoint(tx, ty, direction, Living.STEP_X, Living.STEP_Y);
                            if (!(p != Point.Empty))
                            {
                                break;
                            }
                            path.Add(p);
                            tx = p.X;
                            ty = p.Y;
                        }
                    }
                    if (path.Count > 0)
                    {
                        this.m_game.AddAction(new LivingMoveToAction(this, path, action, delay, speed, sAction, callback));
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        
        public bool MoveTo(int x, int y, string action, int delay, int speed, LivingCallBack callback)
        {
            if (m_x == x && m_y == y)
                return false;

            if (x < 0 || x > m_map.Bound.Width) return false;

            List<Point> path = new List<Point>();

            int tx = m_x;
            int ty = m_y;
            int direction = x > tx ? 1 : -1;
            while ((x - tx) * direction > 0)
            {
                Point p = m_map.FindNextWalkPoint(tx, ty, direction, speed / 2 - 2, STEP_Y);
                if (p != Point.Empty)
                {
                    path.Add(p);
                    tx = p.X;
                    ty = p.Y;
                }
                else
                {
                    break;
                }
            }

            if (path.Count > 0)
            {
                m_game.AddAction(new LivingMoveToAction(this, path, action, delay, speed, "", callback));
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool JumpToSpeed(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
        {
            Point point = this.m_map.FindYLineNotEmptyPoint(x, y);
            int arg_15_0 = point.Y;
            this.m_game.AddAction(new LivingJumpAction(this, point.X, point.Y, speed, action, delay, type, callback));
            return true;
        }
        public bool FlyTo(int x, int y, string action, int delay, int speed)
        {
            return FlyTo(x, y, action, delay, speed, null);
        }
        public bool FlyTo(int x, int y, string action, int delay, int speed, LivingCallBack callback)
        {
            m_game.AddAction(new LivingFlyToAction(this, this.X, this.Y, x, y, action, delay, speed, callback));
            m_game.AddAction(new LivingFallingAction(this, x, y, 0, action, delay, 0, callback));
            return true;
        }
        public bool FallFrom(int x, int y, string action, int delay, int type, int speed)
        {
            return this.FallFrom(x, y, action, delay, type, speed, null);
        }
        public bool FallFrom(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
        {
            Point left = this.m_map.FindYLineNotEmptyPoint(x, y);
            if (left == Point.Empty)
            {
                left = new Point(x, this.m_game.Map.Bound.Height + 1);
            }
            if (this.Y < left.Y)
            {
                this.m_game.AddAction(new LivingFallingAction(this, left.X, left.Y, speed, action, delay, type, callback));
                return true;
            }
            return false;
        }
        public bool FallFromTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
        {
            this.m_game.AddAction(new LivingFallingAction(this, x, y, speed, action, delay, type, callback));
            return true;
        }
        public bool JumpTo(int x, int y, string action, int delay, int type)
        {
            return this.JumpTo(x, y, action, delay, type, 20, null);
        }
        public bool JumpTo(int x, int y, string ation, int delay, int type, LivingCallBack callback)
        {
            return this.JumpTo(x, y, ation, delay, type, 20, callback);
        }
        public bool JumpTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
        {
            Point point = this.m_map.FindYLineNotEmptyPoint(x, y);
            if (point.Y < this.Y)
            {
                this.m_game.AddAction(new LivingJumpAction(this, point.X, point.Y, speed, action, delay, type, callback));
                return true;
            }
            return false;
        }
        public void ChangeDirection(int direction, int delay)
        {
            if (delay > 0)
            {
                this.m_game.AddAction(new LivingChangeDirectionAction(this, direction, delay));
                return;
            }
            this.Direction = direction;
        }
        protected int MakeDamage(Living target)
        {
            double baseDamage = this.BaseDamage;
            double arg_0D_0 = target.BaseGuard;
            double arg_14_0 = target.Defence;
            double attack = this.Attack;
            bool arg_22_0 = this.IgnoreArmor;
            float currentDamagePlus = this.CurrentDamagePlus;
            float currentShootMinus = this.CurrentShootMinus;
            double num = 0.95 * (target.BaseGuard - (double)(3 * this.Grade)) / (500.0 + target.BaseGuard - (double)(3 * this.Grade));
            double num2;
            if (target.Defence - this.Lucky < 0.0)
            {
                num2 = 0.0;
            }
            else
            {
                num2 = 0.95 * (target.Defence - this.Lucky) / (600.0 + target.Defence - this.Lucky);
            }
            double num3 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num + num2 - num * num2)) * (double)currentDamagePlus * (double)currentShootMinus;
            new Point(this.X, this.Y);
            if (num3 < 0.0)
            {
                return 1;
            }
            return (int)num3;
        }
        public bool Beat(Living target, string action, int demageAmount, int criticalAmount, int delay, int livingCount, int attackEffect)
        {
            if (target == null || !target.IsLiving)
            {
                return false;
            }
            demageAmount = this.MakeDamage(target);
            int num = (int)target.Distance(this.X, this.Y);
            if (num <= this.MaxBeatDis)
            {
                if (this.X - target.X > 0)
                {
                    this.Direction = -1;
                }
                else
                {
                    this.Direction = 1;
                }
                this.m_game.AddAction(new LivingBeatAction(this, target, demageAmount, criticalAmount, action, delay, livingCount, attackEffect));
                return true;
            }
            return false;
        }
        public bool RangeAttacking(int fx, int tx, string action, int delay, List<Player> players)
        {
            if (base.IsLiving)
            {
                this.m_game.AddAction(new LivingRangeAttackingAction(this, fx, tx, action, delay, players));
                return true;
            }
            return false;
        }

        public void GetShootForceAndAngle(ref int x, ref int y, int bombId, int minTime, int maxTime, int bombCount, float time, ref int force, ref int angle)
        {
            if (minTime >= maxTime)
            {
                return;
            }
            BallInfo ballInfo = BallMgr.FindBall(bombId);
            if (this.m_game != null && ballInfo != null)
            {
                Map map = this.m_game.Map;
                Point shootPoint = this.GetShootPoint();
                float num = (float)(x - shootPoint.X);
                float num2 = (float)(y - shootPoint.Y);
                float af = map.airResistance * (float)ballInfo.DragIndex;
                float f = map.gravity * (float)ballInfo.Weight * (float)ballInfo.Mass;
                float f2 = map.wind * (float)ballInfo.Wind;
                float m = (float)ballInfo.Mass;
                for (float num3 = time; num3 <= 4f; num3 += 0.6f)
                {
                    double num4 = Living.ComputeVx((double)num, m, af, f2, num3);
                    double num5 = Living.ComputeVy((double)num2, m, af, f, num3);
                    if (num5 < 0.0 && num4 * (double)this.m_direction > 0.0)
                    {
                        double num6 = Math.Sqrt(num4 * num4 + num5 * num5);
                        if (num6 < 2000.0)
                        {
                            force = (int)num6;
                            angle = (int)(Math.Atan(num5 / num4) / 3.1415926535897931 * 180.0);
                            if (num4 < 0.0)
                            {
                                angle += 180;
                                break;
                            }
                            break;
                        }
                    }
                }
                x = shootPoint.X;
                y = shootPoint.Y;
            }
        }
        public bool ShootPoint(int x, int y, int bombId, int minTime, int maxTime, int bombCount, float time, int delay)
        {
            this.m_game.AddAction(new LivingShootAction(this, bombId, x, y, 0, 0, bombCount, minTime, maxTime, time, delay));
            return true;
        }
        public bool IsFriendly(Living living)
        {
            return !(living is Player) && living.Team == this.Team;
        }
        public bool Shoot(int bombId, int x, int y, int force, int angle, int bombCount, int delay)
        {
            this.m_game.AddAction(new LivingShootAction(this, bombId, x, y, force, angle, bombCount, delay, 0, 0f, 0));
            return true;
        }
        public static double ComputeVx(double dx, float m, float af, float f, float t)
        {
            return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 0.7;
        }
        public static double ComputeVy(double dx, float m, float af, float f, float t)
        {
            return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 1.3;
        }
        public static double ComputDX(double v, float m, float af, float f, float dt)
        {
            return v * (double)dt + ((double)f - (double)af * v) / (double)m * (double)dt * (double)dt;
        }
        public bool ShootImp(int bombId, int x, int y, int force, int angle, int bombCount, int shootCount)
        {
            BallInfo ballInfo = BallMgr.FindBall(bombId);
            Tile shape = BallMgr.FindTile(bombId);
            BombType ballType = BallMgr.GetBallType(bombId);
            int num = (int)(this.m_map.wind * 10f);
            if (ballInfo != null)
            {
                GSPacketIn gSPacketIn = new GSPacketIn(91, base.Id);
                gSPacketIn.Parameter1 = base.Id;
                gSPacketIn.WriteByte(2);
                gSPacketIn.WriteInt(num);
                gSPacketIn.WriteBoolean(num > 0);
                gSPacketIn.WriteByte(this.m_game.GetVane(num, 1));
                gSPacketIn.WriteByte(this.m_game.GetVane(num, 2));
                gSPacketIn.WriteByte(this.m_game.GetVane(num, 3));
                gSPacketIn.WriteInt(bombCount);
                float num2 = 0f;
                SimpleBomb simpleBomb = null;
                for (int i = 0; i < bombCount; i++)
                {
                    double num3 = 1.0;
                    int num4 = 0;
                    if (i == 1)
                    {
                        num3 = 0.9;
                        num4 = -5;
                    }
                    else
                    {
                        if (i == 2)
                        {
                            num3 = 1.1;
                            num4 = 5;
                        }
                    }
                    int num5 = (int)((double)force * num3 * Math.Cos((double)(angle + num4) / 180.0 * 3.1415926535897931));
                    int num6 = (int)((double)force * num3 * Math.Sin((double)(angle + num4) / 180.0 * 3.1415926535897931));
                    simpleBomb = new SimpleBomb(this.m_game.PhysicalId++, ballType, this, this.m_game, ballInfo, shape, this.ControlBall);
                    simpleBomb.SetXY(x, y);
                    simpleBomb.setSpeedXY(num5, num6);
                    this.m_map.AddPhysical(simpleBomb);
                    simpleBomb.StartMoving();
                    gSPacketIn.WriteInt(0);
                    gSPacketIn.WriteInt(0);
                    gSPacketIn.WriteBoolean(simpleBomb.DigMap);
                    gSPacketIn.WriteInt(simpleBomb.Id);
                    gSPacketIn.WriteInt(x);
                    gSPacketIn.WriteInt(y);
                    gSPacketIn.WriteInt(num5);
                    gSPacketIn.WriteInt(num6);
                    gSPacketIn.WriteInt(simpleBomb.BallInfo.ID);
                    gSPacketIn.WriteString(simpleBomb.BallInfo.FlyingPartical);
                    gSPacketIn.WriteInt(simpleBomb.BallInfo.Radii * 1000 / 4);
                    gSPacketIn.WriteInt((int)simpleBomb.BallInfo.Power * 1000);
                    gSPacketIn.WriteInt(simpleBomb.Actions.Count);
                    foreach (BombAction current in simpleBomb.Actions)
                    {
                        gSPacketIn.WriteInt(current.TimeInt);
                        gSPacketIn.WriteInt(current.Type);
                        gSPacketIn.WriteInt(current.Param1);
                        gSPacketIn.WriteInt(current.Param2);
                        gSPacketIn.WriteInt(current.Param3);
                        gSPacketIn.WriteInt(current.Param4);
                    }
                    num2 = Math.Max(num2, simpleBomb.LifeTime);
                }
                int num7 = 0;
                int count = simpleBomb.PetActions.Count;
                if (count > 0 && this.PetBaseAtt > 0)
                {
                    gSPacketIn.WriteInt(count);
                    foreach (BombAction current2 in simpleBomb.PetActions)
                    {
                        gSPacketIn.WriteInt(current2.Param1);
                        gSPacketIn.WriteInt(current2.Param2);
                        gSPacketIn.WriteInt(current2.Param4);
                        gSPacketIn.WriteInt(current2.Param3);
                    }
                    gSPacketIn.WriteInt(1);
                    num7 = 2;
                }
                else
                {
                    gSPacketIn.WriteInt(0);
                    gSPacketIn.WriteInt(0);
                }
                this.m_game.SendToAll(gSPacketIn);
                this.m_game.WaitTime((int)((num2 + 2f + (float)num7 + (float)(bombCount / 3)) * 1000f));
                return true;
            }
            return false;
        }
        public void PlayMovie(string action, int delay, int MovieTime)
        {
            this.m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
        }
        public bool PlayMovieBo(string action, int delay, int MovieTime)
        {
            this.m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
            return true;
        }
        public void PlayMovie(string action, int delay, int MovieTime, LivingCallBack call)
        {
            this.m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
        }
        public void SetSeal(bool state, int type)
        {
            if (this.m_isSeal != state)
            {
                this.m_isSeal = state;
                bool arg_16_0 = this.m_syncAtTime;
            }
        }
        public bool GetSealState()
        {
            return this.m_isSeal;
        }
        public void Seal(Player player, int type, int delay)
        {
            this.m_game.AddAction(new LivingSealAction(this, player, type, delay));
        }
        public virtual int AddBlood(int value)
        {
            this.m_blood += value;
            if (this.m_blood > this.m_maxBlood)
            {
                this.m_blood = this.m_maxBlood;
            }
            if (this.m_syncAtTime)
            {
                this.m_game.SendGameUpdateHealth(this, 0, value);
            }
            return value;
        }
        public virtual bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            if (!this.NoTakeDamage && this is SimpleNpc)
            {
                return false;
            }
            bool result = false;
            if (!this.IsFrost && this.m_blood > 0)
            {
                if (source != this || source.Team == this.Team)
                {
                    this.OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
                }
                this.m_blood -= damageAmount + criticalAmount;
                if (this.EffectTrigger && this is Player)
                {
                    this.Game.SendMessage((this as Player).PlayerDetail, LanguageMgr.GetTranslation("PlayerEquipEffect.Success2", new object[0]), LanguageMgr.GetTranslation("PlayerEquipEffect.Success3", new object[]
					{
						(this as Player).PlayerDetail.PlayerCharacter.NickName
					}), 3);
                }
                if (this.m_syncAtTime)
                {
                    this.m_game.SendGameUpdateHealth(this, 6, damageAmount + criticalAmount);
                }
                this.OnAfterTakedDamage(source, damageAmount, criticalAmount);
                if (this.m_blood <= 0)
                {
                    this.Die();
                }
                Player arg_F3_0 = this as Player;
                result = true;
            }
            this.EffectList.StopEffect(typeof(IceFronzeEffect));
            this.EffectList.StopEffect(typeof(HideEffect));
            this.EffectList.StopEffect(typeof(NoHoleEffect));
            return result;
        }
        public virtual bool PetTakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            if (!this.NoTakeDamage && this is SimpleNpc)
            {
                return false;
            }
            bool result = false;
            if (this.m_blood > 0)
            {
                if (source != this || source.Team == this.Team)
                {
                    this.OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
                }
                this.m_blood -= damageAmount + criticalAmount;
                this.OnAfterTakedDamage(source, damageAmount, criticalAmount);
                if (this.m_blood <= 0)
                {
                    this.Die();
                }
                result = true;
            }
            this.EffectList.StopEffect(typeof(IceFronzeEffect));
            this.EffectList.StopEffect(typeof(HideEffect));
            this.EffectList.StopEffect(typeof(NoHoleEffect));
            return result;
        }
        public virtual void Die(int delay)
        {
            if (base.IsLiving && this.m_game != null)
            {
                this.m_game.AddAction(new LivingDieAction(this, delay));
            }
        }
        public override void Die()
        {
            if (this.m_blood > 0)
            {
                this.m_blood = 0;
                if (this.m_syncAtTime)
                {
                    this.m_game.SendGameUpdateHealth(this, 6, 0);
                }
            }
            if (base.IsLiving)
            {
                if (this.IsAttacking)
                {
                    this.StopAttacking();
                }
                base.Die();
                this.OnDied();
                this.m_game.CheckState(0);
                if (m_map.Info.ID == 1208)
                {
                    this.PlayMovie("dieB", 1000, 2000);
                }
            }
        }
        protected void OnDied()
        {
            if (this.Died != null)
            {
                this.Died(this);
            }
            if (this is Player && this.Game is PVEGame)
            {
                ((PVEGame)this.Game).DoOther();
            }
        }
        protected void OnBeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (this.BeforeTakeDamage != null)
            {
                this.BeforeTakeDamage(this, source, ref damageAmount, ref criticalAmount);
            }
        }
        public void OnTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
        {
            if (this.TakePlayerDamage != null)
            {
                this.TakePlayerDamage(this, source, ref damageAmount, ref criticalAmount);
            }
        }
        protected void OnBeginNewTurn()
        {
            if (this.BeginNextTurn != null)
            {
                this.BeginNextTurn(this);
            }
        }
        protected void OnBeginSelfTurn()
        {
            if (this.BeginSelfTurn != null)
            {
                this.BeginSelfTurn(this);
            }
        }
        protected void OnStartAttacked()
        {
            if (this.BeginAttacked != null)
            {
                this.BeginAttacked(this);
            }
        }
        protected void OnStartAttacking()
        {
            if (this.BeginAttacking != null)
            {
                this.BeginAttacking(this);
            }
        }
        protected void OnStopAttacking()
        {
            if (this.EndAttacking != null)
            {
                this.EndAttacking(this);
            }
        }
        public virtual void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
        {
            if (target.Team != this.Team)
            {
                this.CurrentIsHitTarget = true;
                this.TotalHurt += damageAmount + criticalAmount;
                if (!target.IsLiving)
                {
                    this.TotalKill++;
                }
                this.m_game.CurrentTurnTotalDamage = damageAmount + criticalAmount;
                this.m_game.TotalHurt += damageAmount + criticalAmount;
            }
            if (this.AfterKillingLiving != null)
            {
                this.AfterKillingLiving(this, target, damageAmount, criticalAmount);
            }
        }
        public void OnAfterTakedDamage(Living target, int damageAmount, int criticalAmount)
        {
            if (this.AfterKilledByLiving != null)
            {
                this.AfterKilledByLiving(this, target, damageAmount, criticalAmount);
            }
        }
        public void CallFuction(LivingCallBack func, int delay)
        {
            if (this.m_game != null)
            {
                this.m_game.AddAction(new LivingCallFunctionAction(this, func, delay));
            }
        }
        //
        public virtual int ReducedBlood(int value)
        {
            m_blood += value;
            if (m_blood > m_maxBlood)
            {
                m_blood = m_maxBlood;
            }
            if (m_syncAtTime)
            {
                m_game.SendGameUpdateHealth(this, 1, value);
            }

            return value;

        }

        public int MakeCritical(Living living)
        {
            return MakeDamage(living);
        }

        public bool BeatNpc(int fx, int tx, string action, int delay, List<NormalNpc> players)
        {
            m_game.AddAction(new NpcRangeAttackingAction(this, fx, tx, action, delay, players));
            return true;
        }

        public bool ReduceBlood(Living living, int count)
        {
            living.AddEffect(new ContinueReduceBloodEffect(2, -count), 0);
            return true;
        }

    }
}
