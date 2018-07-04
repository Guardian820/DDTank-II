using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Logic.Phy.Object
{
	public class SimpleNpc : Living
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private NpcInfo m_npcInfo;
		private ABrain m_ai;
		public int TotalCure;
		public NpcInfo NpcInfo
		{
			get
			{
				return this.m_npcInfo;
			}
		}
        public SimpleNpc(int id, BaseGame game, NpcInfo npcInfo, int direction, int type)
            : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
		{
			if (type == 0)
			{
				base.Type = eLivingType.SimpleNpcSpecial;
			}
			else
			{
				base.Type = eLivingType.SimpleNpcNormal;
			}
			this.m_npcInfo = npcInfo;
			this.m_ai = (ScriptMgr.CreateInstance(npcInfo.Script) as ABrain);
			if (this.m_ai == null)
			{
				SimpleNpc.log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
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
				SimpleNpc.log.ErrorFormat("SimpleNpc Created error:{1}", arg);
			}
		}
		public override void Reset()
		{
			this.Agility = (double)this.m_npcInfo.Agility;
			this.Attack = (double)this.m_npcInfo.Attack;
			this.BaseDamage = (double)this.m_npcInfo.BaseDamage;
			this.BaseGuard = (double)this.m_npcInfo.BaseGuard;
			this.Lucky = (double)this.m_npcInfo.Lucky;
			this.Grade = this.m_npcInfo.Level;
			this.Experience = this.m_npcInfo.Experience;
			this.TotalCure = 0;
			base.SetRect(this.m_npcInfo.X, this.m_npcInfo.Y, this.m_npcInfo.Width, this.m_npcInfo.Height);
			base.Reset();
		}
		public void GetDropItemInfo()
		{
			if (this.m_game.CurrentLiving is Player)
			{
				Player player = this.m_game.CurrentLiving as Player;
				List<ItemInfo> list = null;
				int value = 0;
				int num = 0;
				int value2 = 0;
				int value3 = 0;
				DropInventory.NPCDrop(this.m_npcInfo.DropId, ref list);
				if (list != null)
				{
					foreach (ItemInfo current in list)
					{
						ItemInfo.FindSpecialItemInfo(current, ref value, ref num, ref value2, ref value3);
						if (current != null)
						{
							if (current.Template.CategoryID == 10)
							{
								player.PlayerDetail.AddTemplate(current, eBageType.FightBag, current.Count);
							}
							else
							{
								player.PlayerDetail.AddTemplate(current, eBageType.TempBag, current.Count);
							}
						}
					}
					player.PlayerDetail.AddGold(value);
					player.PlayerDetail.AddMoney(num);
					player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_Drop, player.PlayerDetail.PlayerCharacter.ID, num, player.PlayerDetail.PlayerCharacter.Money);
					player.PlayerDetail.AddGiftToken(value2);
					player.PlayerDetail.AddMedal(value3);
				}
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
		public override void PrepareNewTurn()
		{
			base.PrepareNewTurn();
			try
			{
				this.m_ai.OnBeginNewTurn();
			}
			catch (Exception arg)
			{
				SimpleNpc.log.ErrorFormat("SimpleNpc BeginNewTurn error:{1}", arg);
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
				SimpleNpc.log.ErrorFormat("SimpleNpc StartAttacking error:{1}", arg);
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
				SimpleNpc.log.ErrorFormat("SimpleNpc Dispose error:{1}", arg);
			}
		}
	}
}
