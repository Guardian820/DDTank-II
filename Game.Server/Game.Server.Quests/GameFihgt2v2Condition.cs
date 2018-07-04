using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameFihgt2v2Condition : BaseCondition
	{
		public GameFihgt2v2Condition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
		}
		private void player_MissionOver(AbstractGame game, int missionId, int turnCount)
		{
		}
		public override void RemoveTrigger(GamePlayer player)
		{
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
