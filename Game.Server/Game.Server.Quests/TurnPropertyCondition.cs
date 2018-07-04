using Bussiness.Managers;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Statics;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Quests
{
	public class TurnPropertyCondition : BaseCondition
	{
		private BaseQuest m_quest;
		private GamePlayer m_player;
		public TurnPropertyCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
			this.m_quest = quest;
		}
		public override void AddTrigger(GamePlayer player)
		{
			this.m_player = player;
			player.GameKillDrop += new GamePlayer.GameKillDropEventHandel(this.QuestDropItem);
			base.AddTrigger(player);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			bool result = false;
			if (player.GetItemCount(this.m_info.Para1) >= this.m_info.Para2)
			{
				base.Value = 0;
				result = true;
			}
			return result;
		}
		public override bool Finish(GamePlayer player)
		{
			return player.RemoveTemplate(this.m_info.Para1, this.m_info.Para2);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.GameKillDrop -= new GamePlayer.GameKillDropEventHandel(this.QuestDropItem);
			base.RemoveTrigger(player);
		}
		public override bool CancelFinish(GamePlayer player)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(this.m_info.Para1);
			if (itemTemplateInfo != null)
			{
				ItemInfo cloneItem = ItemInfo.CreateFromTemplate(itemTemplateInfo, this.m_info.Para2, 117);
				return player.AddTemplate(cloneItem, eBageType.TempBag, this.m_info.Para2);
			}
			return false;
		}
		private void QuestDropItem(AbstractGame game, int copyId, int npcId, bool playResult)
		{
			if (this.m_player.GetItemCount(this.m_info.Para1) < this.m_info.Para2)
			{
				List<ItemInfo> list = null;
				int value = 0;
				int num = 0;
				int value2 = 0;
				int value3 = 0;
				if (game is PVEGame)
				{
					DropInventory.PvEQuestsDrop(npcId, ref list);
				}
				if (game is PVPGame)
				{
					DropInventory.PvPQuestsDrop(game.RoomType, playResult, ref list);
				}
				if (list != null)
				{
					foreach (ItemInfo current in list)
					{
						ItemInfo.FindSpecialItemInfo(current, ref value, ref num, ref value2, ref value3);
						if (current != null)
						{
							this.m_player.TempBag.AddTemplate(current, current.Count);
						}
					}
					this.m_player.AddGold(value);
					this.m_player.AddGiftToken(value2);
					this.m_player.AddMoney(num);
					this.m_player.AddMedal(value3);
					LogMgr.LogMoneyAdd(LogMoneyType.Award, LogMoneyType.Award_Drop, this.m_player.PlayerCharacter.ID, num, this.m_player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
				}
			}
		}
	}
}
