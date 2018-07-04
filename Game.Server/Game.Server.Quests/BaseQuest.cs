using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Quests
{
	public class BaseQuest
	{
		private QuestInfo m_info;
		private QuestDataInfo m_data;
		private List<BaseCondition> m_list;
		private GamePlayer m_player;
		private DateTime m_oldFinishDate;
		public QuestInfo Info
		{
			get
			{
				return this.m_info;
			}
		}
		public QuestDataInfo Data
		{
			get
			{
				return this.m_data;
			}
		}
		public BaseQuest(QuestInfo info, QuestDataInfo data)
		{
			this.m_info = info;
			this.m_data = data;
			this.m_data.QuestID = this.m_info.ID;
			this.m_list = new List<BaseCondition>();
			List<QuestConditionInfo> questCondiction = QuestMgr.GetQuestCondiction(info);
			int num = 0;
			foreach (QuestConditionInfo current in questCondiction)
			{
				BaseCondition baseCondition = BaseCondition.CreateCondition(this, current, data.GetConditionValue(num++));
				if (baseCondition != null)
				{
					this.m_list.Add(baseCondition);
				}
			}
		}
		public BaseCondition GetConditionById(int id)
		{
			foreach (BaseCondition current in this.m_list)
			{
				if (current.Info.CondictionID == id)
				{
					return current;
				}
			}
			return null;
		}
		public void AddToPlayer(GamePlayer player)
		{
			this.m_player = player;
			if (!this.m_data.IsComplete)
			{
				this.AddTrigger(player);
			}
		}
		public void RemoveFromPlayer(GamePlayer player)
		{
			if (!this.m_data.IsComplete)
			{
				this.RemveTrigger(player);
			}
			this.m_player = null;
		}
		public void Reset(GamePlayer player, int rand)
		{
			this.m_data.QuestID = this.m_info.ID;
			this.m_data.UserID = player.PlayerId;
			this.m_data.IsComplete = false;
			this.m_data.IsExist = true;
			if (this.m_data.CompletedDate == DateTime.MinValue)
			{
				this.m_data.CompletedDate = new DateTime(2000, 1, 1);
			}
			if ((DateTime.Now - this.m_data.CompletedDate).TotalDays >= (double)this.m_info.RepeatInterval)
			{
				this.m_data.RepeatFinish = this.m_info.RepeatMax;
			}
			this.m_data.RepeatFinish--;
			this.m_data.RandDobule = rand;
			foreach (BaseCondition current in this.m_list)
			{
				current.Reset(player);
			}
			this.SaveData();
		}
		private void AddTrigger(GamePlayer player)
		{
			foreach (BaseCondition current in this.m_list)
			{
				current.AddTrigger(player);
			}
		}
		private void RemveTrigger(GamePlayer player)
		{
			foreach (BaseCondition current in this.m_list)
			{
				current.RemoveTrigger(player);
			}
		}
		public void SaveData()
		{
			int num = 0;
			foreach (BaseCondition current in this.m_list)
			{
				this.m_data.SaveConditionValue(num++, current.Value);
			}
		}
		public void Update()
		{
			this.SaveData();
			if (this.m_data.IsDirty && this.m_player != null)
			{
				this.m_player.QuestInventory.Update(this);
			}
		}
		public bool CanCompleted(GamePlayer player)
		{
			if (this.m_data.IsComplete)
			{
				return false;
			}
			foreach (BaseCondition current in this.m_list)
			{
				if (!current.IsCompleted(player))
				{
					return false;
				}
			}
			return true;
		}
		public bool Finish(GamePlayer player)
		{
			if (this.CanCompleted(player))
			{
				foreach (BaseCondition current in this.m_list)
				{
					if (!current.Finish(player))
					{
						bool result = false;
						return result;
					}
				}
				if (!this.Info.CanRepeat)
				{
					this.m_data.IsComplete = true;
					this.RemveTrigger(player);
				}
				this.m_oldFinishDate = this.m_data.CompletedDate;
				this.m_data.CompletedDate = DateTime.Now;
				return true;
			}
			return false;
		}
		public bool CancelFinish(GamePlayer player)
		{
			this.m_data.IsComplete = false;
			this.m_data.CompletedDate = this.m_oldFinishDate;
			foreach (BaseCondition current in this.m_list)
			{
				current.CancelFinish(player);
			}
			return true;
		}
	}
}
