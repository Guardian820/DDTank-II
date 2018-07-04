using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameFightByGameCondition : BaseCondition
	{
		public GameFightByGameCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
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
			eGameType gameType = game.GameType;
			switch (gameType)
			{
			case eGameType.Free:
				if ((this.m_info.Para1 == 0 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;

			case eGameType.Guild:
				if ((this.m_info.Para1 == 1 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;

			case eGameType.Training:
			case (eGameType)3:
				break;

			case eGameType.ALL:
				if ((this.m_info.Para1 == 4 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;

			default:
				if (gameType == eGameType.Dungeon)
				{
					if ((this.m_info.Para1 == 7 || this.m_info.Para1 == -1) && base.Value > 0)
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
