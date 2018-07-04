using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.GameUtils
{
	public abstract class CardAbstractInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected object m_lock = new object();
		private int m_capalility;
		private int m_beginSlot;
		protected UsersCardInfo[] m_cards;
		protected UsersCardInfo temp_card;
		protected List<int> m_changedPlaces = new List<int>();
		private int m_changeCount;
		public int BeginSlot
		{
			get
			{
				return this.m_beginSlot;
			}
		}
		public int Capalility
		{
			get
			{
				return this.m_capalility;
			}
			set
			{
				this.m_capalility = ((value < 0) ? 0 : ((value > this.m_cards.Length) ? this.m_cards.Length : value));
			}
		}
		public bool IsEmpty(int slot)
		{
			return slot < 0 || slot >= this.m_capalility || this.m_cards[slot] == null;
		}
		public CardAbstractInventory(int capability, int beginSlot)
		{
			this.m_capalility = capability;
			this.m_beginSlot = beginSlot;
			this.m_cards = new UsersCardInfo[capability];
			this.temp_card = new UsersCardInfo();
		}
		public virtual void UpdateTempCard(UsersCardInfo card)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.temp_card = card;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public virtual void UpdateCard()
		{
			int place = this.temp_card.Place;
			int templateID = this.temp_card.TemplateID;
			int num;
			if (place < 5)
			{
				this.ReplaceCardTo(this.temp_card, place);
				num = this.FindPlaceByTamplateId(5, templateID);
				this.MoveItem(place, num);
				return;
			}
			this.ReplaceCardTo(this.temp_card, place);
			num = this.FindPlaceByTamplateId(0, 5, templateID);
			if (this.GetItemAt(num) != null && this.GetItemAt(num).TemplateID == templateID)
			{
				this.MoveItem(place, num);
			}
		}
		public bool RemoveCardAt(int place)
		{
			return this.RemoveCard(this.GetItemAt(place));
		}
		public virtual bool RemoveCard(UsersCardInfo item)
		{
			if (item == null)
			{
				return false;
			}
			int num = -1;
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = 0; i < this.m_capalility; i++)
				{
					if (this.m_cards[i] == item)
					{
						num = i;
						this.m_cards[i] = null;
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			if (num != -1)
			{
				this.OnPlaceChanged(num);
				item.Place = -1;
			}
			return num != -1;
		}
		public bool AddCard(UsersCardInfo card)
		{
			return this.AddCard(card, this.m_beginSlot);
		}
		public bool AddCard(UsersCardInfo card, int minSlot)
		{
			if (card == null)
			{
				return false;
			}
			int place = this.FindFirstEmptySlot(minSlot);
			return this.AddCardTo(card, place);
		}
		public virtual bool AddCardTo(UsersCardInfo card, int place)
		{
			if (card == null || place >= this.m_capalility || place < 0)
			{
				return false;
			}
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				if (this.m_cards[place] != null)
				{
					place = -1;
				}
				else
				{
					this.m_cards[place] = card;
					card.Place = place;
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			if (place != -1)
			{
				this.OnPlaceChanged(place);
			}
			return place != -1;
		}
		public virtual bool ReplaceCardTo(UsersCardInfo card, int place)
		{
			if (card == null || place >= this.m_capalility || place < 0)
			{
				return false;
			}
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_cards[place] = card;
				card.Place = place;
				this.OnPlaceChanged(place);
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return true;
		}
		public virtual bool MoveItem(int fromSlot, int toSlot)
		{
			if (fromSlot < 0 || toSlot < 0 || fromSlot >= this.m_capalility || toSlot >= this.m_capalility)
			{
				return false;
			}
			bool flag = false;
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				flag = this.ExchangeCards(fromSlot, toSlot);
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			if (flag)
			{
				this.BeginChanges();
				try
				{
					this.OnPlaceChanged(toSlot);
				}
				finally
				{
					this.CommitChanges();
				}
			}
			return flag;
		}
		public bool IsSolt(int slot)
		{
			return slot >= 0 && slot < this.m_capalility;
		}
		protected virtual bool ExchangeCards(int fromSlot, int toSlot)
		{
			UsersCardInfo usersCardInfo = this.m_cards[fromSlot];
			if (fromSlot != toSlot)
			{
				this.m_cards[toSlot].TemplateID = usersCardInfo.TemplateID;
				this.m_cards[toSlot].Attack = usersCardInfo.Attack;
				this.m_cards[toSlot].Defence = usersCardInfo.Defence;
				this.m_cards[toSlot].Agility = usersCardInfo.Agility;
				this.m_cards[toSlot].Luck = usersCardInfo.Luck;
				this.m_cards[toSlot].Damage = usersCardInfo.Damage;
				this.m_cards[toSlot].Guard = usersCardInfo.Guard;
			}
			else
			{
				this.m_cards[toSlot].TemplateID = 0;
				this.m_cards[toSlot].Attack = 0;
				this.m_cards[toSlot].Defence = 0;
				this.m_cards[toSlot].Agility = 0;
				this.m_cards[toSlot].Luck = 0;
				this.m_cards[toSlot].Damage = 0;
				this.m_cards[toSlot].Guard = 0;
			}
			return true;
		}
		public virtual bool ResetCardSoul()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = 0; i < 5; i++)
				{
					this.m_cards[i].Level = 0;
					this.m_cards[i].CardGP = 0;
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return true;
		}
		public virtual bool UpGraceSlot(int soulPoint, int lv, int place)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_cards[place].CardGP += soulPoint;
				this.m_cards[place].Level = lv;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return true;
		}
		public virtual UsersCardInfo GetItemAt(int slot)
		{
			if (slot < 0 || slot >= this.m_capalility)
			{
				return null;
			}
			return this.m_cards[slot];
		}
		public int FindFirstEmptySlot()
		{
			return this.FindFirstEmptySlot(this.m_beginSlot);
		}
		public int FindFirstEmptySlot(int minSlot)
		{
			if (minSlot >= this.m_capalility)
			{
				return -1;
			}
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			int result;
			try
			{
				for (int i = minSlot; i < this.m_capalility; i++)
				{
					if (this.m_cards[i] == null)
					{
						result = i;
						return result;
					}
				}
				result = -1;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public int FindPlaceByTamplateId(int minSlot, int templateId)
		{
			if (minSlot >= this.m_capalility)
			{
				return -1;
			}
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			int result;
			try
			{
				for (int i = minSlot; i < this.m_capalility; i++)
				{
					if (this.m_cards[i] == null)
					{
						result = -1;
						return result;
					}
					if (this.m_cards[i].TemplateID == templateId)
					{
						result = this.m_cards[i].Place;
						return result;
					}
				}
				result = -1;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public bool FindEquipCard(int templateId)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			bool result;
			try
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.m_cards[i].TemplateID == templateId)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public int FindPlaceByTamplateId(int minSlot, int maxSlot, int templateId)
		{
			if (minSlot >= this.m_capalility)
			{
				return -1;
			}
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			int result;
			try
			{
				for (int i = minSlot; i < maxSlot; i++)
				{
					if (this.m_cards[i].TemplateID == templateId)
					{
						result = this.m_cards[i].Place;
						return result;
					}
				}
				result = -1;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public int FindLastEmptySlot()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			int result;
			try
			{
				for (int i = this.m_capalility - 1; i >= 0; i--)
				{
					if (this.m_cards[i] == null)
					{
						result = i;
						return result;
					}
				}
				result = -1;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public virtual void Clear()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = 0; i < this.m_capalility; i++)
				{
					this.m_cards[i] = null;
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public virtual UsersCardInfo GetItemByTemplateID(int minSlot, int templateId)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			UsersCardInfo result;
			try
			{
				for (int i = minSlot; i < this.m_capalility; i++)
				{
					if (this.m_cards[i] != null && this.m_cards[i].TemplateID == templateId)
					{
						result = this.m_cards[i];
						return result;
					}
				}
				result = null;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public virtual UsersCardInfo GetItemByPlace(int minSlot, int place)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			UsersCardInfo result;
			try
			{
				for (int i = minSlot; i < this.m_capalility; i++)
				{
					if (this.m_cards[i] != null && this.m_cards[i].Place == place)
					{
						result = this.m_cards[i];
						return result;
					}
				}
				result = null;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public virtual List<UsersCardInfo> GetItems()
		{
			return this.GetItems(0, this.m_capalility);
		}
		public virtual List<UsersCardInfo> GetItems(int minSlot, int maxSlot)
		{
			List<UsersCardInfo> list = new List<UsersCardInfo>();
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = minSlot; i < maxSlot; i++)
				{
					if (this.m_cards[i] != null)
					{
						list.Add(this.m_cards[i]);
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return list;
		}
		public int GetEmptyCount()
		{
			return this.GetEmptyCount(this.m_beginSlot);
		}
		public virtual int GetEmptyCount(int minSlot)
		{
			if (minSlot < 0 || minSlot > this.m_capalility - 1)
			{
				return 0;
			}
			int num = 0;
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = minSlot; i < this.m_capalility; i++)
				{
					if (this.m_cards[i] == null)
					{
						num++;
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return num;
		}
		protected void OnPlaceChanged(int place)
		{
			if (!this.m_changedPlaces.Contains(place))
			{
				this.m_changedPlaces.Add(place);
			}
			if (this.m_changeCount <= 0 && this.m_changedPlaces.Count > 0)
			{
				this.UpdateChangedPlaces();
			}
		}
		public void BeginChanges()
		{
			Interlocked.Increment(ref this.m_changeCount);
		}
		public void CommitChanges()
		{
			int num = Interlocked.Decrement(ref this.m_changeCount);
			if (num < 0)
			{
				if (CardAbstractInventory.log.IsErrorEnabled)
				{
					CardAbstractInventory.log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
				}
				Thread.VolatileWrite(ref this.m_changeCount, 0);
			}
			if (num <= 0 && this.m_changedPlaces.Count > 0)
			{
				this.UpdateChangedPlaces();
			}
		}
		public virtual void UpdateChangedPlaces()
		{
			this.m_changedPlaces.Clear();
		}
	}
}
