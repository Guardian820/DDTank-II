using Bussiness;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class OwnConsortiaCondition : BaseCondition
	{
		public OwnConsortiaCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.GuildChanged += new GamePlayer.PlayerOwnConsortiaEventHandle(this.player_OwnConsortia);
		}
		private void player_OwnConsortia()
		{
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.GuildChanged -= new GamePlayer.PlayerOwnConsortiaEventHandle(this.player_OwnConsortia);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			bool flag = false;
			int num = 0;
			bool result;
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				ConsortiaInfo consortiaSingle = consortiaBussiness.GetConsortiaSingle(player.PlayerCharacter.ConsortiaID);
				switch (this.m_info.Para1)
				{
				case 0:
					num = consortiaSingle.Count;
					break;

				case 1:
					num = player.PlayerCharacter.RichesOffer + player.PlayerCharacter.RichesRob;
					break;

				case 2:
					num = consortiaSingle.SmithLevel;
					break;

				case 3:
					num = consortiaSingle.ShopLevel;
					break;

				case 4:
					num = consortiaSingle.StoreLevel;
					break;
				}
				if (num >= this.m_info.Para2)
				{
					base.Value = 0;
					flag = true;
				}
				result = flag;
			}
			return result;
		}
	}
}
