using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameKillByRoomCondition : BaseCondition
	{
		public GameKillByRoomCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.AfterKillingLiving += new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.AfterKillingLiving -= new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
		}
		private void player_AfterKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage)
		{
			if (!isLiving && type == 1)
			{
				eRoomType roomType = game.RoomType;
				switch (roomType)
				{
				case eRoomType.Match:
					if ((this.m_info.Para1 == 0 || this.m_info.Para1 == -1) && base.Value > 0)
					{
						base.Value--;
					}
					break;

				case eRoomType.Freedom:
					if ((this.m_info.Para1 == 1 || this.m_info.Para1 == -1) && base.Value > 0)
					{
						base.Value--;
					}
					break;

				case eRoomType.Exploration:
				case (eRoomType)3:
					break;

				case eRoomType.Dungeon:
					if ((this.m_info.Para1 == 4 || this.m_info.Para1 == -1) && base.Value > 0)
					{
						base.Value--;
					}
					break;

				default:
					if (roomType == eRoomType.Freshman)
					{
						if ((this.m_info.Para1 == 2 || this.m_info.Para1 == -1) && base.Value > 0)
						{
							base.Value--;
						}
					}
					break;
				}
				if (base.Value < 0)
				{
					base.Value = 0;
				}
			}
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
