using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class OwnGradeCondition : BaseCondition
	{
		public OwnGradeCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
		}
		private void player_UpdateGrade(int grade)
		{
		}
		public override void RemoveTrigger(GamePlayer player)
		{
		}
		public override bool IsCompleted(GamePlayer player)
		{
			if (player.Level >= this.m_info.Para2)
			{
				base.Value = 0;
				return true;
			}
			return false;
		}
	}
}
