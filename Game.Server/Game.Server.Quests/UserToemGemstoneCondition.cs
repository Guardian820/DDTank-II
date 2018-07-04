using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class UserToemGemstoneCondition : BaseCondition
	{
		public UserToemGemstoneCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.UserToemGemstonetEvent += new GamePlayer.PlayerUserToemGemstoneEventHandle(this.player_UserToemGemstonet);
		}
		private void player_UserToemGemstonet()
		{
			if (base.Value > 0)
			{
				base.Value--;
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.UserToemGemstonetEvent -= new GamePlayer.PlayerUserToemGemstoneEventHandle(this.player_UserToemGemstonet);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
