using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class NewGearCondition : BaseCondition
	{
		public NewGearCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.NewGearEvent += new GamePlayer.PlayerNewGearEventHandle(this.player_NewGear);
		}
		private void player_NewGear(int CategoryID)
		{
			if ((CategoryID == this.m_info.Para1 || CategoryID == this.m_info.Para2) && base.Value > 0)
			{
				base.Value--;
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.NewGearEvent -= new GamePlayer.PlayerNewGearEventHandle(this.player_NewGear);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
