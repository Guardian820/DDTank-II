using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class CropPrimaryCondition : BaseCondition
	{
		public CropPrimaryCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.CropPrimaryEvent += new GamePlayer.PlayerCropPrimaryEventHandle(this.player_CropPrimary);
		}
		private void player_CropPrimary()
		{
			if (base.Value > 0)
			{
				base.Value--;
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.CropPrimaryEvent -= new GamePlayer.PlayerCropPrimaryEventHandle(this.player_CropPrimary);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
