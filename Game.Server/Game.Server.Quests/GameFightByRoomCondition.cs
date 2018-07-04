using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameFightByRoomCondition : BaseCondition
	{
		public GameFightByRoomCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.GameOver += new GamePlayer.PlayerGameOverEventHandle(this.player_GameOver);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.GameOver -= new GamePlayer.PlayerGameOverEventHandle(this.player_GameOver);
		}
		private void player_GameOver(AbstractGame game, bool isWin, int gainXp)
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
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
