using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameKillByGameCondition : BaseCondition
	{
		public GameKillByGameCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
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
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
