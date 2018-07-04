using Bussiness;
using Bussiness.Managers;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.Phy.Maths;
using Game.Logic.Spells;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
namespace Game.Logic.Phy.Object
{
	public class Player : TurnedLiving
	{
		private IGamePlayer m_player;
		private UsersPetinfo m_pet;
		private ItemTemplateInfo m_weapon;
		private ItemInfo m_DeputyWeapon;
		private int m_mainBallId;
		private int m_spBallId;
		private int m_sp2BallId;
		private int m_AddWoundBallId;
		private int m_MultiBallId;
		private BallInfo m_currentBall;
		private int m_energy;
		private bool m_isActive;
		private int m_prop;
		public Point TargetPoint;
		public int GainGP;
		public int GainOffer;
		public bool LockDirection;
		public int TotalCure;
		private bool m_canGetProp;
		public int TotalAllHurt;
		public int TotalAllHitTargetCount;
		public int TotalAllShootCount;
		public int TotalAllKill;
		public int TotalAllExperience;
		public int TotalAllScore;
		public int TotalAllCure;
		public int CanTakeOut;
		public bool FinishTakeCard;
		public bool HasPaymentTakeCard;
		public int BossCardCount;
		public bool Ready;
		private int m_loadingProcess;
		private int m_shootCount;
		private int m_ballCount;
		private ArrayList m_tempBoxes = new ArrayList();
		private int skillcu;
		private int deputyWeaponResCount;
		private static readonly int FLY_COOLDOWN = 2;
		private static readonly int CARRY_TEMPLATE_ID = 10016;
		private int m_flyCoolDown;
		public event PlayerEventHandle LoadingCompleted;
		public event PlayerEventHandle PlayerShoot;
		public IGamePlayer PlayerDetail
		{
			get
			{
				return this.m_player;
			}
		}
		public UsersPetinfo Pet
		{
			get
			{
				return this.m_pet;
			}
		}
		public ItemTemplateInfo Weapon
		{
			get
			{
				return this.m_weapon;
			}
		}
		public bool IsActive
		{
			get
			{
				return this.m_isActive;
			}
		}
		public int Prop
		{
			get
			{
				return this.m_prop;
			}
			set
			{
				this.m_prop = value;
			}
		}
		public bool CanGetProp
		{
			get
			{
				return this.m_canGetProp;
			}
			set
			{
				if (this.m_canGetProp != value)
				{
					this.m_canGetProp = value;
				}
			}
		}
		public int LoadingProcess
		{
			get
			{
				return this.m_loadingProcess;
			}
			set
			{
				if (this.m_loadingProcess != value)
				{
					this.m_loadingProcess = value;
					if (this.m_loadingProcess >= 100)
					{
						this.OnLoadingCompleted();
					}
				}
			}
		}
		public int Energy
		{
			get
			{
				return this.m_energy;
			}
			set
			{
				this.m_energy = value;
			}
		}
		public BallInfo CurrentBall
		{
			get
			{
				return this.m_currentBall;
			}
		}
		public bool IsSpecialSkill
		{
			get
			{
				return this.m_currentBall.ID == this.m_spBallId;
			}
		}
		public int ShootCount
		{
			get
			{
				return this.m_shootCount;
			}
			set
			{
				if (this.m_shootCount != value)
				{
					this.m_shootCount = value;
					this.m_game.SendGameUpdateShootCount(this);
				}
			}
		}
		public int BallCount
		{
			get
			{
				return this.m_ballCount;
			}
			set
			{
				if (this.m_ballCount != value)
				{
					this.m_ballCount = value;
				}
			}
		}
		public Player(IGamePlayer player, int id, BaseGame game, int team, int maxBlood) : base(id, game, team, "", "", maxBlood, 0, 1)
		{
			this.m_rect = new Rectangle(-15, -20, 30, 30);
			this.m_player = player;
			this.m_player.GamePlayerId = id;
			this.m_isActive = true;
			this.m_canGetProp = true;
			this.Grade = player.PlayerCharacter.Grade;
			base.vaneOpen = player.PlayerCharacter.IsWeakGuildFinish(9);
			this.m_pet = player.Pet;
			if (this.m_pet != null)
			{
				base.isPet = true;
				base.PetBaseAtt = this.GetPetBaseAtt();
			}
			this.TotalAllHurt = 0;
			this.TotalAllHitTargetCount = 0;
			this.TotalAllShootCount = 0;
			this.TotalAllKill = 0;
			this.TotalAllExperience = 0;
			this.TotalAllScore = 0;
			this.TotalAllCure = 0;
			this.m_DeputyWeapon = this.m_player.SecondWeapon;
			if (this.m_DeputyWeapon != null)
			{
				this.deputyWeaponResCount = this.m_DeputyWeapon.StrengthenLevel + 1;
			}
			else
			{
				this.deputyWeaponResCount = 1;
			}
			this.m_weapon = this.m_player.MainWeapon;
			if (this.m_weapon != null)
			{
				BallConfigInfo ballConfigInfo = BallConfigMgr.FindBall(this.m_weapon.TemplateID);
				this.m_mainBallId = ballConfigInfo.Common;
				this.m_spBallId = ballConfigInfo.Special;
				this.m_sp2BallId = ballConfigInfo.SpecialII;
				this.m_AddWoundBallId = ballConfigInfo.CommonAddWound;
				this.m_MultiBallId = ballConfigInfo.CommonMultiBall;
			}
			this.m_loadingProcess = 0;
			this.m_prop = 0;
			this.InitBuffer(this.m_player.EquipEffect);
			this.m_energy = this.m_player.PlayerCharacter.Agility / 30 + 240;
			this.m_maxBlood = this.m_player.PlayerCharacter.hp;
			this.skillcu = 0;
		}
		public int GetPetBaseAtt()
		{
			try
			{
				string[] array = this.m_pet.SkillEquip.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					int skillID = Convert.ToInt32(array[i].Split(new char[]
					{
						','
					})[0]);
					PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(skillID);
					if (petSkillInfo != null && petSkillInfo.Damage > 0)
					{
						int result = petSkillInfo.Damage;
						return result;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("______________GetPetBaseAtt ERROR______________");
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine("_______________________________________________");
				int result = 0;
				return result;
			}
			return 0;
		}
		public override void Reset()
		{
			this.m_maxBlood = this.m_player.PlayerCharacter.hp;
			if (this.m_game.RoomType == eRoomType.Dungeon)
			{
				this.m_game.Cards = new int[21];
			}
			else
			{
				this.m_game.Cards = new int[9];
			}
			base.Dander = 0;
			base.PetMP = 10;
			base.psychic = 40;
			this.m_energy = this.m_player.PlayerCharacter.Agility / 30 + 240;
			base.IsLiving = true;
			this.FinishTakeCard = false;
			this.m_DeputyWeapon = this.m_player.SecondWeapon;
			this.m_weapon = this.m_player.MainWeapon;
			BallConfigInfo ballConfigInfo = BallConfigMgr.FindBall(this.m_weapon.TemplateID);
			this.m_mainBallId = ballConfigInfo.Common;
			this.m_spBallId = ballConfigInfo.Special;
			this.m_sp2BallId = ballConfigInfo.SpecialII;
			this.m_AddWoundBallId = ballConfigInfo.CommonAddWound;
			this.m_MultiBallId = ballConfigInfo.CommonMultiBall;
			this.BaseDamage = this.m_player.GetBaseAttack();
			this.BaseGuard = this.m_player.GetBaseDefence();
			this.Attack = (double)this.m_player.PlayerCharacter.Attack;
			this.Defence = (double)this.m_player.PlayerCharacter.Defence;
			this.Agility = (double)this.m_player.PlayerCharacter.Agility;
			this.Lucky = (double)this.m_player.PlayerCharacter.Luck;
			this.m_currentBall = BallMgr.FindBall(this.m_mainBallId);
			this.m_shootCount = 1;
			this.m_ballCount = 1;
			this.m_prop = 0;
			this.CurrentIsHitTarget = false;
			this.TotalCure = 0;
			this.TotalHitTargetCount = 0;
			this.TotalHurt = 0;
			this.TotalKill = 0;
			this.TotalShootCount = 0;
			this.LockDirection = false;
			this.GainGP = 0;
			this.GainOffer = 0;
			this.Ready = false;
			this.PlayerDetail.ClearTempBag();
			this.LoadingProcess = 0;
			this.skillcu = 0;
			base.Reset();
		}
		public void InitBuffer(List<ItemInfo> equpedEffect)
		{
			base.EffectList.StopAllEffect();
			foreach (ItemInfo current in equpedEffect)
			{
				int num = 0;
				int num2 = 0;
				RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneTemplateID(current.TemplateID);
				string[] array = runeTemplateInfo.Attribute1.Split(new char[]
				{
					'|'
				});
				string[] array2 = runeTemplateInfo.Attribute2.Split(new char[]
				{
					'|'
				});
				if (current.Hole1 > runeTemplateInfo.BaseLevel)
				{
					if (array.Length > 1)
					{
						num = 1;
					}
					if (array2.Length > 1)
					{
						num2 = 1;
					}
				}
				int num3 = runeTemplateInfo.Type1;
				int num4 = Convert.ToInt32(array[num]);
				int probability = runeTemplateInfo.Rate1;
				if (num3 == 37 || num3 == 39)
				{
					num3 = runeTemplateInfo.Type2;
					num4 = Convert.ToInt32(array2[num2]);
					probability = runeTemplateInfo.Rate2;
				}
				switch (num3)
				{
				case 1:
					new AddAttackEffect(num4, probability).Start(this);
					break;

				case 2:
					new AddDefenceEffect(num4, probability).Start(this);
					break;

				case 3:
					new AddAgilityEffect(num4, probability).Start(this);
					break;

				case 4:
					new AddLuckyEffect(num4, probability).Start(this);
					break;

				case 5:
					new AddDamageEffect(num4, probability).Start(this);
					break;

				case 6:
					new ReduceDamageEffect(num4, probability).Start(this);
					break;

				case 7:
					new AddBloodEffect(num4, probability).Start(this);
					break;

				case 8:
					new FatalEffect(num4, probability).Start(this);
					break;

				case 9:
					new IceFronzeEquipEffect(num4, probability).Start(this);
					break;

				case 10:
					new NoHoleEquipEffect(num4, probability).Start(this);
					break;

				case 11:
					new AtomBombEquipEffect(num4, probability).Start(this);
					break;

				case 12:
					new ArmorPiercerEquipEffect(num4, probability).Start(this);
					break;

				case 13:
					new AvoidDamageEffect(num4, probability).Start(this);
					break;

				case 14:
					new MakeCriticalEffect(num4, probability).Start(this);
					break;

				case 15:
					new AssimilateDamageEffect(num4, probability).Start(this);
					break;

				case 16:
					new AssimilateBloodEffect(num4, probability).Start(this);
					break;

				case 17:
					new SealEquipEffect(num4, probability).Start(this);
					break;

				case 18:
					new AddTurnEquipEffect(num4, probability).Start(this);
					break;

				case 19:
					new AddDanderEquipEffect(num4, probability).Start(this);
					break;

				case 20:
					new ReflexDamageEquipEffect(num4, probability).Start(this);
					break;

				case 21:
					new ReduceStrengthEquipEffect(num4, probability).Start(this);
					break;

				case 22:
					new ContinueReduceBloodEquipEffect(num4, probability).Start(this);
					break;

				case 23:
					new LockDirectionEquipEffect(num4, probability).Start(this);
					break;

				case 24:
					new AddBombEquipEffect(num4, probability).Start(this);
					break;

				case 25:
					new ContinueReduceDamageEquipEffect(num4, probability).Start(this);
					break;

				case 26:
					new RecoverBloodEffect(num4, probability).Start(this);
					break;

				default:
					Console.WriteLine("??????????????????????????? Not Found Effect: " + num3);
					break;
				}
			}
		}
		public bool ReduceEnergy(int value)
		{
			if (value > this.m_energy)
			{
				return false;
			}
			this.m_energy -= value;
			return true;
		}
		public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
		{
			if ((source == this || source.Team == base.Team) && damageAmount + criticalAmount >= this.m_blood)
			{
				damageAmount = this.m_blood - 1;
				criticalAmount = 0;
			}
			bool result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
			if (base.IsLiving)
			{
				base.AddDander((damageAmount * 2 / 5 + 5) / 2);
			}
			return result;
		}
		public void UseSpecialSkill()
		{
			if (base.Dander >= 200)
			{
				this.SetBall(this.m_spBallId, true);
				base.SetDander(0);
			}
		}
		public void SetBall(int ballId)
		{
			this.SetBall(ballId, false);
		}
		public void SetBall(int ballId, bool special)
		{
			if (ballId != this.m_currentBall.ID)
			{
				if (BallMgr.FindBall(ballId) != null)
				{
					this.m_currentBall = BallMgr.FindBall(ballId);
				}
				this.BallCount = this.m_currentBall.Amount;
				if (!special || ballId == 4)
				{
					this.ShootCount = 1;
				}
				this.m_game.SendGameUpdateBall(this, special);
			}
		}
		public void SetCurrentWeapon(ItemTemplateInfo item)
		{
			this.m_weapon = item;
			BallConfigInfo ballConfigInfo = BallConfigMgr.FindBall(this.m_weapon.TemplateID);
			this.m_mainBallId = ballConfigInfo.Common;
			this.m_spBallId = ballConfigInfo.Special;
			this.m_sp2BallId = ballConfigInfo.SpecialII;
			this.m_AddWoundBallId = ballConfigInfo.CommonAddWound;
			this.m_MultiBallId = ballConfigInfo.CommonMultiBall;
			this.SetBall(this.m_mainBallId);
		}
		public override void StartMoving()
		{
			if (this.m_map != null)
			{
				Point point = this.m_map.FindYLineNotEmptyPoint(this.m_x, this.m_y);
				if (point.IsEmpty)
				{
					this.m_y = this.m_map.Ground.Height;
				}
				else
				{
					this.m_x = point.X;
					this.m_y = point.Y;
				}
				if (point.IsEmpty)
				{
					this.m_syncAtTime = false;
					this.Die();
				}
			}
		}
		public override void StartMoving(int delay, int speed)
		{
			if (this.m_map != null)
			{
				Point point = this.m_map.FindYLineNotEmptyPoint(this.m_x, this.m_y);
				if (point.IsEmpty)
				{
					this.m_y = this.m_map.Ground.Height;
				}
				else
				{
					this.m_x = point.X;
					this.m_y = point.Y;
				}
				base.StartMoving(delay, speed);
				if (point.IsEmpty)
				{
					this.m_syncAtTime = false;
					this.Die();
				}
			}
		}
		public void StartGhostMoving()
		{
			if (this.TargetPoint.IsEmpty)
			{
				return;
			}
			Point point = new Point(this.TargetPoint.X - this.X, this.TargetPoint.Y - this.Y);
			if (point.Length() > 160.0)
			{
				point.Normalize(160);
			}
			this.m_game.AddAction(new GhostMoveAction(this, new Point(this.X + point.X, this.Y + point.Y)));
		}
		public override void SetXY(int x, int y)
		{
			if (this.m_x == x && this.m_y == y)
			{
				return;
			}
			this.m_x = x;
			this.m_y = y;
			if (base.IsLiving)
			{
				this.m_energy -= Math.Abs(this.m_x - x);
				return;
			}
			Rectangle rect = this.m_rect;
			rect.Offset(this.m_x, this.m_y);
			Physics[] array = this.m_map.FindPhysicalObjects(rect, this);
			Physics[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Physics physics = array2[i];
				if (physics is Box)
				{
					Box box = physics as Box;
					this.PickBox(box);
					this.OpenBox(box.Id);
				}
			}
		}
		public override void Die()
		{
			if (base.IsLiving)
			{
				this.m_y -= 70;
				base.Die();
			}
		}
		public override void PickBox(Box box)
		{
			this.m_tempBoxes.Add(box);
			base.PickBox(box);
		}
		public void OpenBox(int boxId)
		{
			Box box = null;
			foreach (Box box2 in this.m_tempBoxes)
			{
				if (box2.Id == boxId)
				{
					box = box2;
					break;
				}
			}
			if (box != null && box.Item != null)
			{
				ItemInfo item = box.Item;
				int templateID = item.TemplateID;
				if (templateID <= -200)
				{
					if (templateID == -1100)
					{
						this.m_player.AddGiftToken(item.Count);
						goto IL_162;
					}
					if (templateID == -200)
					{
						this.m_player.AddMoney(item.Count);
						this.m_player.LogAddMoney(AddMoneyType.Box, AddMoneyType.Box_Open, this.m_player.PlayerCharacter.ID, item.Count, this.m_player.PlayerCharacter.Money);
						goto IL_162;
					}
				}
				else
				{
					if (templateID == -100)
					{
						this.m_player.AddGold(item.Count);
						goto IL_162;
					}
					if (templateID == 11408)
					{
						this.m_player.AddMedal(item.Count);
						goto IL_162;
					}
				}
				if (item.Template.CategoryID == 10)
				{
					this.m_player.AddTemplate(item, eBageType.FightBag, item.Count);
				}
				else
				{
					this.m_player.AddTemplate(item, eBageType.TempBag, item.Count);
				}
				IL_162:
				this.m_tempBoxes.Remove(box);
			}
		}
		public override void PrepareNewTurn()
		{
			if (this.CurrentIsHitTarget)
			{
				this.TotalHitTargetCount++;
			}
			this.m_energy = this.m_player.PlayerCharacter.Agility / 30 + 240;
			this.m_shootCount = 1;
			this.m_ballCount = 1;
			this.m_flyCoolDown--;
			this.SetCurrentWeapon(this.PlayerDetail.MainWeapon);
			if (this.m_currentBall.ID != this.m_mainBallId)
			{
				this.m_currentBall = BallMgr.FindBall(this.m_mainBallId);
			}
			if (!base.IsLiving)
			{
				this.StartGhostMoving();
				this.TargetPoint = Point.Empty;
			}
			base.PrepareNewTurn();
		}
		public override void StartAttacking()
		{
			if (!base.IsAttacking)
			{
				base.AddDelay(1600 - 1200 * this.PlayerDetail.PlayerCharacter.Agility / (this.PlayerDetail.PlayerCharacter.Agility + 1200) + this.PlayerDetail.PlayerCharacter.Attack / 10);
				base.StartAttacking();
			}
		}
		public override void Skip(int spendTime)
		{
			if (base.IsAttacking)
			{
				base.Skip(spendTime);
				this.m_prop = 0;
				base.AddDelay(100);
				base.AddDander(20);
				base.AddPetMP();
			}
		}
		public void PrepareShoot(byte speedTime)
		{
			int turnWaitTime = this.m_game.GetTurnWaitTime();
			int num = ((int)speedTime > turnWaitTime) ? turnWaitTime : ((int)speedTime);
			base.AddDelay(num * 20);
			this.TotalShootCount++;
		}
		public void PetUseKill(int skillID)
		{
			if (skillID == this.skillcu)
			{
				return;
			}
			this.skillcu = skillID;
			PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(skillID);
			this.m_game.SendPetUseKill(this, skillID, true);
			if (petSkillInfo.NewBallID != -1)
			{
				this.m_delay += petSkillInfo.Delay;
				this.SetBall(petSkillInfo.NewBallID);
				base.critActive = true;
			}
			base.PetMP -= petSkillInfo.CostMP;
			this.CurrentDamagePlus += (float)(petSkillInfo.DamageCrit / 100);
		}
		public bool Shoot(int x, int y, int force, int angle)
		{
			if (this.m_shootCount == 1)
			{
				base.activePetHit = true;
			}
			if (this.m_shootCount > 0)
			{
				this.EffectTrigger = false;
				this.OnPlayerShoot();
				if (this.EffectTrigger)
				{
					base.Game.SendMessage(this.PlayerDetail, LanguageMgr.GetTranslation("PlayerEquipEffect.Success", new object[0]), LanguageMgr.GetTranslation("PlayerEquipEffect.Success1", new object[]
					{
						this.PlayerDetail.PlayerCharacter.NickName
					}), 3);
				}
				int bombId = this.m_currentBall.ID;
				if (this.m_ballCount == 1 && !this.IsSpecialSkill)
				{
					if (this.Prop == 20002)
					{
						bombId = this.m_MultiBallId;
					}
					if (this.Prop == 20008)
					{
						bombId = this.m_AddWoundBallId;
					}
				}
				if (base.ShootImp(bombId, x, y, force, angle, this.m_ballCount, this.ShootCount))
				{
					this.m_shootCount--;
					if (this.m_shootCount <= 0 || !base.IsLiving)
					{
						this.StopAttacking();
						base.AddDelay(this.m_currentBall.Delay + this.m_weapon.Property8);
						base.AddDander(20);
						base.AddPetMP();
						this.m_prop = 0;
						if (this.CanGetProp)
						{
							int value = 0;
							int num = 0;
							int value2 = 0;
							int value3 = 0;
							List<ItemInfo> list = null;
							if (DropInventory.FireDrop(this.m_game.RoomType, ref list) && list != null)
							{
								foreach (ItemInfo current in list)
								{
									ItemInfo.FindSpecialItemInfo(current, ref value, ref num, ref value2, ref value3);
									if (current != null)
									{
										this.PlayerDetail.AddTemplate(current, eBageType.FightBag, current.Count);
									}
								}
								this.PlayerDetail.AddGold(value);
								this.PlayerDetail.AddMoney(num);
								this.PlayerDetail.LogAddMoney(AddMoneyType.Game, AddMoneyType.Game_Shoot, this.PlayerDetail.PlayerCharacter.ID, num, this.PlayerDetail.PlayerCharacter.Money);
								this.PlayerDetail.AddGiftToken(value2);
								this.PlayerDetail.AddMedal(value3);
							}
						}
					}
					return true;
				}
			}
			return false;
		}
		public bool CanUseItem(ItemTemplateInfo item)
		{
			return this.m_energy >= item.Property4 && (base.IsAttacking || (!base.IsLiving && base.Team == this.m_game.CurrentLiving.Team));
		}
		public bool UseItem(ItemTemplateInfo item)
		{
			if (this.CanUseItem(item))
			{
				this.m_energy -= item.Property4;
				this.m_delay += item.Property5;
				this.m_game.SendPlayerUseProp(this, -2, -2, item.TemplateID);
				SpellMgr.ExecuteSpell(this.m_game, this.m_game.CurrentLiving as Player, item);
				return true;
			}
			return false;
		}
		public void UseFlySkill()
		{
			if (this.m_flyCoolDown <= 0)
			{
				this.m_flyCoolDown = Player.FLY_COOLDOWN;
				this.m_game.SendPlayerUseProp(this, -2, -2, Player.CARRY_TEMPLATE_ID);
				this.SetBall(3);
			}
		}
		public void UseSecondWeapon()
		{
			if (this.CanUseItem(this.m_DeputyWeapon.Template))
			{
				if (this.m_DeputyWeapon.Template.Property3 == 31)
				{
					int count = this.m_DeputyWeapon.Template.Property7 + (int)Math.Pow(1.1, (double)this.m_DeputyWeapon.StrengthenLevel);
					new AddDefenceEffect(count, 100).Start(this);
				}
				else
				{
					this.SetCurrentWeapon(this.m_DeputyWeapon.Template);
				}
				this.m_energy -= this.m_DeputyWeapon.Template.Property4;
				this.m_delay += this.m_DeputyWeapon.Template.Property5;
				this.m_game.SendPlayerUseProp(this, -2, -2, this.m_weapon.TemplateID);
				if (this.deputyWeaponResCount > 0)
				{
					this.deputyWeaponResCount--;
					this.m_game.SendUseDeputyWeapon(this, this.deputyWeaponResCount);
				}
			}
		}
		public void DeadLink()
		{
			this.m_isActive = false;
			if (base.IsLiving)
			{
				this.Die();
			}
		}
		public bool CheckShootPoint(int x, int y)
		{
			if (Math.Abs(this.X - x) > 100)
			{
				string arg_21_0 = this.m_player.PlayerCharacter.UserName;
				string arg_32_0 = this.m_player.PlayerCharacter.NickName;
				this.m_player.Disconnect();
				return false;
			}
			return true;
		}
		protected void OnLoadingCompleted()
		{
			if (this.LoadingCompleted != null)
			{
				this.LoadingCompleted(this);
			}
		}
		public void OnPlayerShoot()
		{
			if (this.PlayerShoot != null)
			{
				this.PlayerShoot(this);
			}
		}
		public override void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
		{
			base.OnAfterKillingLiving(target, damageAmount, criticalAmount);
			if (target is Player)
			{
				this.m_player.OnKillingLiving(this.m_game, 1, target.Id, target.IsLiving, damageAmount + criticalAmount);
				return;
			}
			int id = 0;
			if (target is SimpleBoss)
			{
				SimpleBoss simpleBoss = target as SimpleBoss;
				id = simpleBoss.NpcInfo.ID;
			}
			if (target is SimpleNpc)
			{
				SimpleNpc simpleNpc = target as SimpleNpc;
				id = simpleNpc.NpcInfo.ID;
			}
			this.m_player.OnKillingLiving(this.m_game, 2, id, target.IsLiving, damageAmount + criticalAmount);
		}
	}
}
