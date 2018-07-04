using Bussiness;
using Bussiness.Managers;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Statics;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Server.Quests
{
	public class QuestInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private object m_lock;
		protected List<BaseQuest> m_list;
		protected List<QuestDataInfo> m_datas;
		protected ArrayList m_clearList;
		private GamePlayer m_player;
		private byte[] m_states;
		private UnicodeEncoding m_converter;
		protected List<BaseQuest> m_changedQuests = new List<BaseQuest>();
		private int m_changeCount;
		public QuestInventory(GamePlayer player)
		{
			this.m_converter = new UnicodeEncoding();
			this.m_player = player;
			this.m_lock = new object();
			this.m_list = new List<BaseQuest>();
			this.m_clearList = new ArrayList();
			this.m_datas = new List<QuestDataInfo>();
		}
		public void LoadFromDatabase(int playerId)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_states = ((this.m_player.PlayerCharacter.QuestSite.Count<byte>() == 0) ? this.InitQuest() : this.m_player.PlayerCharacter.QuestSite);
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					QuestDataInfo[] userQuest = playerBussiness.GetUserQuest(playerId);
					this.BeginChanges();
					QuestDataInfo[] array = userQuest;
					for (int i = 0; i < array.Length; i++)
					{
						QuestDataInfo questDataInfo = array[i];
						QuestInfo singleQuest = QuestMgr.GetSingleQuest(questDataInfo.QuestID);
						if (singleQuest != null)
						{
							this.AddQuest(new BaseQuest(singleQuest, questDataInfo));
						}
						this.AddQuestData(questDataInfo);
					}
					this.CommitChanges();
				}
				List<BaseQuest> arg_B0_0 = this.m_list;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public void SaveToDatabase()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					foreach (BaseQuest current in this.m_list)
					{
						current.SaveData();
						if (current.Data.IsDirty)
						{
							playerBussiness.UpdateDbQuestDataInfo(current.Data);
						}
					}
					foreach (BaseQuest baseQuest in this.m_clearList)
					{
						baseQuest.SaveData();
						playerBussiness.UpdateDbQuestDataInfo(baseQuest.Data);
					}
					this.m_clearList.Clear();
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		private bool AddQuest(BaseQuest quest)
		{
			List<BaseQuest> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				this.m_list.Add(quest);
			}
			finally
			{
				Monitor.Exit(list);
			}
			this.OnQuestsChanged(quest);
			quest.AddToPlayer(this.m_player);
			return true;
		}
		private bool AddQuestData(QuestDataInfo data)
		{
			List<BaseQuest> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				this.m_datas.Add(data);
			}
			finally
			{
				Monitor.Exit(list);
			}
			return true;
		}
		public bool AddQuest(QuestInfo info, out string msg)
		{
			msg = "";
			try
			{
				if (info == null)
				{
					msg = "Game.Server.Quests.NoQuest";
					bool result = false;
					return result;
				}
				if (info.TimeMode && DateTime.Now.CompareTo(info.StartDate) < 0)
				{
					msg = "Game.Server.Quests.NoTime";
				}
				if (info.TimeMode && DateTime.Now.CompareTo(info.EndDate) > 0)
				{
					msg = "Game.Server.Quests.TimeOver";
				}
				if (this.m_player.PlayerCharacter.Grade < info.NeedMinLevel)
				{
					msg = "Game.Server.Quests.LevelLow";
				}
				if (this.m_player.PlayerCharacter.Grade > info.NeedMaxLevel)
				{
					msg = "Game.Server.Quests.LevelTop";
				}
				if (info.PreQuestID != "0,")
				{
					string[] array = info.PreQuestID.Split(new char[]
					{
						','
					});
					for (int i = 0; i < array.Length - 1; i++)
					{
						if (!this.IsQuestFinish(Convert.ToInt32(array[i])))
						{
							msg = "Game.Server.Quests.NoFinish";
						}
					}
				}
			}
			catch (Exception ex)
			{
				QuestInventory.log.Info(ex.InnerException);
			}
			if (info.IsOther == 1 && !this.m_player.PlayerCharacter.IsConsortia)
			{
				msg = "Game.Server.Quest.QuestInventory.HaveMarry";
			}
			if (info.IsOther == 2 && !this.m_player.PlayerCharacter.IsMarried)
			{
				msg = "Game.Server.Quest.QuestInventory.HaveMarry";
			}
			BaseQuest baseQuest = this.FindQuest(info.ID);
			if (baseQuest != null && baseQuest.Data.IsComplete)
			{
				msg = "Game.Server.Quests.Have";
			}
			if (baseQuest != null && !baseQuest.Info.CanRepeat)
			{
				msg = "Game.Server.Quests.NoRepeat";
			}
			if (baseQuest != null && DateTime.Now.CompareTo(baseQuest.Data.CompletedDate.Date.AddDays((double)baseQuest.Info.RepeatInterval)) < 0 && baseQuest.Data.RepeatFinish < 1)
			{
				msg = "Game.Server.Quests.Rest";
			}
			BaseQuest baseQuest2 = this.m_player.QuestInventory.FindQuest(info.ID);
			if (baseQuest2 != null)
			{
				msg = "Game.Server.Quests.Have";
			}
			if (msg == "")
			{
				QuestMgr.GetQuestCondiction(info);
				int rand = 1;
				if (ThreadSafeRandom.NextStatic(1000000) <= info.Rands)
				{
					rand = info.RandDouble;
				}
				this.BeginChanges();
				if (baseQuest == null)
				{
					baseQuest = new BaseQuest(info, new QuestDataInfo());
					this.AddQuest(baseQuest);
					baseQuest.Reset(this.m_player, rand);
				}
				else
				{
					baseQuest.Reset(this.m_player, rand);
					baseQuest.AddToPlayer(this.m_player);
					this.OnQuestsChanged(baseQuest);
				}
				this.CommitChanges();
				this.SaveToDatabase();
				return true;
			}
			msg = LanguageMgr.GetTranslation(msg, new object[0]);
			return false;
		}
		public bool FindFinishQuestData(int ID, int UserID)
		{
			bool result = false;
			List<QuestDataInfo> datas;
			Monitor.Enter(datas = this.m_datas);
			try
			{
				foreach (QuestDataInfo current in this.m_datas)
				{
					if (current.QuestID == ID && current.UserID == UserID)
					{
						result = current.IsComplete;
					}
				}
			}
			finally
			{
				Monitor.Exit(datas);
			}
			return result;
		}
		public bool RemoveQuest(BaseQuest quest)
		{
			if (!quest.Info.CanRepeat)
			{
				bool flag = false;
				List<BaseQuest> list;
				Monitor.Enter(list = this.m_list);
				try
				{
					if (this.m_list.Remove(quest))
					{
						this.m_clearList.Add(quest);
						flag = true;
					}
				}
				finally
				{
					Monitor.Exit(list);
				}
				if (flag)
				{
					quest.RemoveFromPlayer(this.m_player);
					this.OnQuestsChanged(quest);
				}
				return flag;
			}
			quest.Reset(this.m_player, 2);
			quest.Data.RepeatFinish++;
			quest.SaveData();
			this.OnQuestsChanged(quest);
			return true;
		}
		public void Update(BaseQuest quest)
		{
			this.OnQuestsChanged(quest);
		}
		public bool Finish(BaseQuest baseQuest, int selectedItem)
		{
			string text = "";
			string arg_0B_0 = string.Empty;
			QuestInfo info = baseQuest.Info;
			QuestDataInfo data = baseQuest.Data;
			this.m_player.BeginAllChanges();
			try
			{
				if (baseQuest.Finish(this.m_player))
				{
					this.RemoveQuest(baseQuest);
					List<QuestAwardInfo> questGoods = QuestMgr.GetQuestGoods(info);
					List<ItemInfo> list = new List<ItemInfo>();
					List<ItemInfo> list2 = new List<ItemInfo>();
					List<ItemInfo> list3 = new List<ItemInfo>();
					List<ItemInfo> list4 = new List<ItemInfo>();
					foreach (QuestAwardInfo current in questGoods)
					{
						if (!current.IsSelect || current.RewardItemID == selectedItem)
						{
							ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(current.RewardItemID);
							if (itemTemplateInfo != null)
							{
								text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardProp", new object[]
								{
									itemTemplateInfo.Name,
									current.RewardItemCount1
								}) + " ";
								int num = current.RewardItemCount1;
								if (current.IsCount)
								{
									num *= data.RandDobule;
								}
								for (int i = 0; i < num; i += itemTemplateInfo.MaxCount)
								{
									int count = (i + itemTemplateInfo.MaxCount > current.RewardItemCount1) ? (current.RewardItemCount1 - i) : itemTemplateInfo.MaxCount;
									ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, count, 106);
									if (itemInfo != null)
									{
										itemInfo.ValidDate = current.RewardItemValid;
										itemInfo.IsBinds = true;
										itemInfo.StrengthenLevel = current.StrengthenLevel;
										itemInfo.AttackCompose = current.AttackCompose;
										itemInfo.DefendCompose = current.DefendCompose;
										itemInfo.AgilityCompose = current.AgilityCompose;
										itemInfo.LuckCompose = current.LuckCompose;
										if (itemTemplateInfo.BagType == eBageType.PropBag)
										{
											list2.Add(itemInfo);
										}
										else
										{
											if (itemTemplateInfo.BagType == eBageType.Farm)
											{
												list3.Add(itemInfo);
											}
											else
											{
												list.Add(itemInfo);
											}
										}
									}
								}
							}
						}
					}
					if (list.Count > 0 && this.m_player.MainBag.GetEmptyCount() < list.Count)
					{
						baseQuest.CancelFinish(this.m_player);
						this.m_player.Out.SendMessage(eMessageType.ERROR, this.m_player.GetInventoryName(eBageType.MainBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull", new object[0]) + " ");
						bool result = false;
						return result;
					}
					if (list2.Count > 0 && this.m_player.PropBag.GetEmptyCount() < list2.Count)
					{
						baseQuest.CancelFinish(this.m_player);
						this.m_player.Out.SendMessage(eMessageType.ERROR, this.m_player.GetInventoryName(eBageType.PropBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull", new object[0]) + " ");
						bool result = false;
						return result;
					}
					foreach (ItemInfo current2 in list)
					{
						if (!this.m_player.MainBag.StackItemToAnother(current2) && !this.m_player.MainBag.AddItem(current2))
						{
							list4.Add(current2);
						}
					}
					foreach (ItemInfo current3 in list2)
					{
						if (current3.Template.CategoryID != 10)
						{
							if (!this.m_player.PropBag.StackItemToAnother(current3) && !this.m_player.PropBag.AddItem(current3))
							{
								list4.Add(current3);
							}
						}
						else
						{
							int templateID = current3.TemplateID;
							switch (templateID)
							{
							case 10001:
								this.m_player.PlayerCharacter.openFunction(Step.PICK_TWO_TWENTY);
								break;

							case 10002:
								break;

							case 10003:
								this.m_player.PlayerCharacter.openFunction(Step.POP_WIN);
								break;

							case 10004:
								this.m_player.PlayerCharacter.openFunction(Step.FIFTY_OPEN);
								this.m_player.AddGift(eGiftType.NEWBIE);
								break;

							case 10005:
								this.m_player.PlayerCharacter.openFunction(Step.FORTY_OPEN);
								break;

							case 10006:
								this.m_player.PlayerCharacter.openFunction(Step.THIRTY_OPEN);
								break;

							case 10007:
								this.m_player.PlayerCharacter.openFunction(Step.POP_TWO_TWENTY);
								break;

							case 10008:
								this.m_player.PlayerCharacter.openFunction(Step.POP_TIP_ONE);
								break;

							default:
								switch (templateID)
								{
								case 10024:
									this.m_player.PlayerCharacter.openFunction(Step.PICK_ONE);
									break;

								case 10025:
									this.m_player.PlayerCharacter.openFunction(Step.POP_EXPLAIN_ONE);
									break;
								}
								break;
							}
						}
					}
					foreach (ItemInfo current4 in list3)
					{
						if (!this.m_player.FarmBag.StackItemToAnother(current4) && !this.m_player.FarmBag.AddItem(current4))
						{
							list4.Add(current4);
						}
					}
					if (list4.Count > 0)
					{
						this.m_player.SendItemsToMail(list4, "Bagfull trả về thư!", "Phần thưởng nhiệm vụ!", eMailType.ItemOverdue);
						this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
					text = LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.Reward", new object[0]) + text;
					if (info.RewardBuffID > 0 && info.RewardBuffDate > 0)
					{
						ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(info.RewardBuffID);
						if (itemTemplateInfo2 != null)
						{
							int num2 = info.RewardBuffDate * data.RandDobule;
							AbstractBuffer abstractBuffer = BufferList.CreateBufferHour(itemTemplateInfo2, num2);
							abstractBuffer.Start(this.m_player);
							text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardBuff", new object[]
							{
								itemTemplateInfo2.Name,
								num2
							}) + " ";
						}
					}
					if (info.RewardGold != 0)
					{
						int num3 = info.RewardGold * data.RandDobule;
						this.m_player.AddGold(num3);
						text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGold", new object[]
						{
							num3
						}) + " ";
					}
					if (info.RewardMoney != 0)
					{
						int num4 = info.RewardMoney * data.RandDobule;
						this.m_player.AddMoney(info.RewardMoney * data.RandDobule);
						LogMgr.LogMoneyAdd(LogMoneyType.Award, LogMoneyType.Award_Quest, this.m_player.PlayerCharacter.ID, num4, this.m_player.PlayerCharacter.Money, 0, 0, 0, 0, "", "", "");
						text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardMoney", new object[]
						{
							num4
						}) + " ";
					}
					if (info.RewardGP != 0)
					{
						int num5 = info.RewardGP * data.RandDobule;
						this.m_player.AddGP(num5);
						text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGB1", new object[]
						{
							num5
						}) + " ";
					}
					if (info.RewardRiches != 0 && this.m_player.PlayerCharacter.ConsortiaID != 0)
					{
						int num6 = info.RewardRiches * data.RandDobule;
						this.m_player.AddRichesOffer(num6);
						using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
						{
							consortiaBussiness.ConsortiaRichAdd(this.m_player.PlayerCharacter.ConsortiaID, ref num6);
						}
						text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardRiches", new object[]
						{
							num6
						}) + " ";
					}
					if (info.RewardOffer != 0)
					{
						int num7 = info.RewardOffer * data.RandDobule;
						this.m_player.AddOffer(num7, false);
						text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardOffer", new object[]
						{
							num7
						}) + " ";
					}
					if (info.RewardBindMoney != 0)
					{
						int num8 = info.RewardBindMoney * data.RandDobule;
						this.m_player.AddGiftToken(num8);
						text += LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGiftToken", new object[]
						{
							num8 + " "
						});
					}
					this.m_player.Out.SendMessage(eMessageType.Normal, text);
					this.SetQuestFinish(baseQuest.Info.ID);
					this.m_player.PlayerCharacter.QuestSite = this.m_states;
				}
				this.OnQuestsChanged(baseQuest);
			}
			catch (Exception arg)
			{
				if (QuestInventory.log.IsErrorEnabled)
				{
					QuestInventory.log.Error("Quest Finish：" + arg);
				}
				bool result = false;
				return result;
			}
			finally
			{
				this.m_player.CommitAllChanges();
			}
			return true;
		}
		public BaseQuest FindQuest(int id)
		{
			foreach (BaseQuest current in this.m_list)
			{
				if (current.Info.ID == id)
				{
					return current;
				}
			}
			return null;
		}
		protected void OnQuestsChanged(BaseQuest quest)
		{
			if (!this.m_changedQuests.Contains(quest))
			{
				this.m_changedQuests.Add(quest);
			}
			if (this.m_changeCount <= 0 && this.m_changedQuests.Count > 0)
			{
				this.UpdateChangedQuests();
			}
		}
		private void BeginChanges()
		{
			Interlocked.Increment(ref this.m_changeCount);
		}
		private void CommitChanges()
		{
			int num = Interlocked.Decrement(ref this.m_changeCount);
			if (num < 0)
			{
				if (QuestInventory.log.IsErrorEnabled)
				{
					QuestInventory.log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
				}
				Thread.VolatileWrite(ref this.m_changeCount, 0);
			}
			if (num <= 0 && this.m_changedQuests.Count > 0)
			{
				this.UpdateChangedQuests();
			}
		}
		public void UpdateChangedQuests()
		{
			this.m_player.Out.SendUpdateQuests(this.m_player, this.m_states, this.m_changedQuests.ToArray());
			this.m_changedQuests.Clear();
		}
		private byte[] InitQuest()
		{
			byte[] array = new byte[200];
			for (int i = 0; i < 200; i++)
			{
				array[i] = 0;
			}
			return array;
		}
		private bool SetQuestFinish(int questId)
		{
			if (questId > this.m_states.Length * 8 || questId < 1)
			{
				return false;
			}
			questId--;
			int num = questId / 8;
			int num2 = questId % 8;
			byte[] expr_2C_cp_0 = this.m_states;
			int expr_2C_cp_1 = num;
			expr_2C_cp_0[expr_2C_cp_1] |= (byte)(1 << num2);
			return true;
		}
		private bool IsQuestFinish(int questId)
		{
			if (questId > this.m_states.Length * 8 || questId < 1)
			{
				return false;
			}
			questId--;
			int num = questId / 8;
			int num2 = questId % 8;
			int num3 = (int)this.m_states[num] & 1 << num2;
			return num3 != 0;
		}
		public bool ClearConsortiaQuest()
		{
			return true;
		}
		public bool ClearMarryQuest()
		{
			return true;
		}
	}
}
