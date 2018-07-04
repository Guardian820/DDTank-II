using Bussiness;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.GameUtils
{
	public class CardInventory : CardAbstractInventory
	{
		protected GamePlayer m_player;
		private bool m_saveToDb;
		private List<UsersCardInfo> m_removedList = new List<UsersCardInfo>();
		public GamePlayer Player
		{
			get
			{
				return this.m_player;
			}
		}
		public CardInventory(GamePlayer player, bool saveTodb, int capibility, int beginSlot) : base(capibility, beginSlot)
		{
			this.m_player = player;
			this.m_saveToDb = saveTodb;
		}
		public virtual void LoadFromDatabase()
		{
			if (this.m_saveToDb)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					UsersCardInfo[] userCardSingles = playerBussiness.GetUserCardSingles(this.m_player.PlayerCharacter.ID);
					base.BeginChanges();
					try
					{
						UsersCardInfo[] array = userCardSingles;
						for (int i = 0; i < array.Length; i++)
						{
							UsersCardInfo usersCardInfo = array[i];
							this.AddCardTo(usersCardInfo, usersCardInfo.Place);
						}
					}
					finally
					{
						base.CommitChanges();
					}
				}
			}
		}
		public virtual void SaveToDatabase()
		{
			if (this.m_saveToDb)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					object @lock;
					Monitor.Enter(@lock = this.m_lock);
					try
					{
						for (int i = 0; i < this.m_cards.Length; i++)
						{
							UsersCardInfo usersCardInfo = this.m_cards[i];
							if (usersCardInfo != null && usersCardInfo.IsDirty)
							{
								if (usersCardInfo.CardID > 0)
								{
									playerBussiness.UpdateCards(usersCardInfo);
								}
								else
								{
									playerBussiness.AddCards(usersCardInfo);
								}
							}
						}
					}
					finally
					{
						Monitor.Exit(@lock);
					}
					List<UsersCardInfo> removedList;
					Monitor.Enter(removedList = this.m_removedList);
					try
					{
						foreach (UsersCardInfo current in this.m_removedList)
						{
							if (current.CardID > 0)
							{
								playerBussiness.UpdateCards(current);
							}
						}
						this.m_removedList.Clear();
					}
					finally
					{
						Monitor.Exit(removedList);
					}
				}
			}
		}
		public override bool AddCardTo(UsersCardInfo item, int place)
		{
			if (base.AddCardTo(item, place))
			{
				item.UserID = this.m_player.PlayerCharacter.ID;
				item.IsExit = true;
				return true;
			}
			return false;
		}
		public override void UpdateChangedPlaces()
		{
			int[] updatedSlots = this.m_changedPlaces.ToArray();
			this.m_player.Out.SendPlayerCardInfo(this, updatedSlots);
			base.UpdateChangedPlaces();
		}
	}
}
