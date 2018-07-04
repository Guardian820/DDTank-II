using Bussiness.Managers;
using Game.Logic;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.GameUtils
{
	public abstract class PlayerFarmInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected object m_lock = new object();
		private int m_capalility;
		private int m_beginSlot;
		protected UserFarmInfo m_farm;
		protected UserFieldInfo[] m_fields;
		protected int m_farmstatus;
		public int Status
		{
			get
			{
				return this.m_farmstatus;
			}
		}
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
				this.m_capalility = ((value < 0) ? 0 : ((value > this.m_fields.Length) ? this.m_fields.Length : value));
			}
		}
		public bool IsEmpty(int slot)
		{
			return slot < 0 || slot >= this.m_capalility || this.m_fields[slot] == null;
		}
		public PlayerFarmInventory(int capability, int beginSlot)
		{
			this.m_capalility = capability;
			this.m_beginSlot = beginSlot;
			this.m_fields = new UserFieldInfo[capability];
			this.m_farm = new UserFarmInfo();
			this.m_farmstatus = 0;
		}
		public bool AddField(UserFieldInfo item)
		{
			return this.AddField(item, this.m_beginSlot);
		}
		public bool AddField(UserFieldInfo item, int minSlot)
		{
			if (item == null)
			{
				return false;
			}
			int place = this.FindFirstEmptySlot(minSlot);
			return this.AddFieldTo(item, place);
		}
		public virtual bool AddFieldTo(UserFieldInfo item, int place)
		{
			if (item == null || place >= this.m_capalility || place < 0)
			{
				return false;
			}
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_fields[place] = item;
				if (this.m_fields[place] != null)
				{
					place = -1;
				}
				else
				{
					this.m_fields[place] = item;
					item.FieldID = place;
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return place != -1;
		}
		public virtual bool RemoveItem(UserFieldInfo item)
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
					if (this.m_fields[i] == item)
					{
						num = i;
						this.m_fields[i] = null;
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return num != -1;
		}
		public bool RemoveItemAt(int place)
		{
			return this.RemoveItem(this.GetItemAt(place));
		}
		public virtual UserFieldInfo GetItemAt(int slot)
		{
			if (slot < 0 || slot >= this.m_capalility)
			{
				return null;
			}
			return this.m_fields[slot];
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
					if (this.m_fields[i] == null)
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
		public void ClearFields()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = this.m_beginSlot; i < this.m_capalility; i++)
				{
					if (this.m_fields[i] != null)
					{
						this.RemoveItem(this.m_fields[i]);
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
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
					if (this.m_fields[i] == null)
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
		public virtual void ClearFarm()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_farm = null;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public virtual void UpdateGainCount(int fieldId, int count)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_fields[fieldId].GainCount = count;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public virtual void loadFarm(UserFarmInfo farm)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_farm = farm;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public virtual bool GrowField(int fieldId, int templateID)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateID);
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_fields[fieldId].SeedID = itemTemplateInfo.TemplateID;
				this.m_fields[fieldId].PlantTime = DateTime.Now;
				this.m_fields[fieldId].GainCount = itemTemplateInfo.Property2;
				this.m_fields[fieldId].FieldValidDate = itemTemplateInfo.Property3;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return true;
		}
		public virtual bool killCropField(int fieldId)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				if (this.m_fields[fieldId] != null)
				{
					this.m_fields[fieldId].SeedID = 0;
					this.m_fields[fieldId].FieldValidDate = 1;
					this.m_fields[fieldId].AccelerateTime = 0;
					this.m_fields[fieldId].GainCount = 0;
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return true;
		}
		public virtual void CreateFarm(int ID, string name)
		{
			string value = PetMgr.FindConfig("PayFieldMoney").Value;
			string value2 = PetMgr.FindConfig("PayAutoMoney").Value;
			UserFarmInfo userFarmInfo = new UserFarmInfo();
			userFarmInfo.FarmID = ID;
			userFarmInfo.FarmerName = name;
			userFarmInfo.isFarmHelper = false;
			userFarmInfo.isAutoId = 0;
			userFarmInfo.AutoPayTime = DateTime.Now;
			userFarmInfo.AutoValidDate = 1;
			userFarmInfo.GainFieldId = 0;
			userFarmInfo.KillCropId = 0;
			userFarmInfo.PayAutoMoney = value2;
			userFarmInfo.PayFieldMoney = value;
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_farm = userFarmInfo;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			this.CreateNewField(ID, 0, 8);
		}
		public virtual bool HelperSwitchFields(bool isHelper, int seedID, int seedTime, int haveCount, int getCount)
		{
			if (isHelper)
			{
				for (int i = 0; i < this.m_fields.Length; i++)
				{
					object @lock;
					Monitor.Enter(@lock = this.m_lock);
					try
					{
						if (this.m_fields[i] != null)
						{
							this.m_fields[i].SeedID = 0;
							this.m_fields[i].FieldValidDate = 1;
							this.m_fields[i].AccelerateTime = 0;
							this.m_fields[i].GainCount = 0;
						}
					}
					finally
					{
						Monitor.Exit(@lock);
					}
				}
			}
			object lock2;
			Monitor.Enter(lock2 = this.m_lock);
			try
			{
				this.m_farm.isFarmHelper = isHelper;
				this.m_farm.isAutoId = seedID;
				this.m_farm.AutoPayTime = DateTime.Now;
				this.m_farm.AutoValidDate = seedTime;
				this.m_farm.GainFieldId = getCount / 10;
				this.m_farm.KillCropId = getCount;
			}
			finally
			{
				Monitor.Exit(lock2);
			}
			return true;
		}
		public virtual void CreateNewField(int ID, int minslot, int maxslot)
		{
			for (int i = minslot; i < maxslot; i++)
			{
				this.AddFieldTo(new UserFieldInfo
				{
					FarmID = ID,
					FieldID = i,
					SeedID = 0,
					PayTime = DateTime.Now.AddYears(100),
					payFieldTime = 876000,
					PlantTime = DateTime.Now,
					GainCount = 0,
					FieldValidDate = 1,
					AccelerateTime = 0,
					AutomaticTime = DateTime.Now,
					IsExit = true
				}, i);
			}
		}
		public virtual bool CreateField(int ID, List<int> fieldIds, int payFieldTime)
		{
			for (int i = 0; i < fieldIds.Count; i++)
			{
				int num = fieldIds[i];
				if (this.m_fields[num] == null)
				{
					this.AddFieldTo(new UserFieldInfo
					{
						FarmID = ID,
						FieldID = num,
						SeedID = 0,
						PayTime = DateTime.Now.AddDays((double)(payFieldTime / 24)),
						payFieldTime = payFieldTime,
						PlantTime = DateTime.Now,
						GainCount = 0,
						FieldValidDate = 1,
						AccelerateTime = 0,
						AutomaticTime = DateTime.Now,
						IsExit = true
					}, num);
				}
				else
				{
					this.m_fields[num].PayTime = DateTime.Now.AddDays((double)(payFieldTime / 24));
					this.m_fields[num].payFieldTime = payFieldTime;
				}
			}
			return true;
		}
		public virtual int AccelerateTimeFields(DateTime PlantTime, int FieldValidDate)
		{
			DateTime now = DateTime.Now;
			int num = now.Hour - PlantTime.Hour;
			int num2 = now.Minute - PlantTime.Minute;
			if (num < 0)
			{
				num = 24 + num;
			}
			if (num2 < 0)
			{
				num2 = 60 + num2;
			}
			int num3 = num * 60 + num2;
			if (num3 > FieldValidDate)
			{
				num3 = FieldValidDate;
			}
			return num3;
		}
		public virtual bool AccelerateTimeFields()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = 0; i < this.m_capalility; i++)
				{
					if (this.m_fields[i] != null && this.m_fields[i].SeedID > 0)
					{
						DateTime plantTime = this.m_fields[i].PlantTime;
						int fieldValidDate = this.m_fields[i].FieldValidDate;
						this.m_fields[i].AccelerateTime = this.AccelerateTimeFields(plantTime, fieldValidDate);
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return true;
		}
		public virtual UserFarmInfo GetFarm()
		{
			return this.m_farm;
		}
		public virtual UserFieldInfo[] GetFields()
		{
			List<UserFieldInfo> list = new List<UserFieldInfo>();
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = 0; i < this.m_capalility; i++)
				{
					if (this.m_fields[i] != null && (this.m_fields[i].PayTime - DateTime.Now).Days > 0)
					{
						list.Add(this.m_fields[i]);
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return list.ToArray();
		}
		public virtual UserFieldInfo GetFieldAt(int slot)
		{
			if (slot < 0 || slot >= this.m_capalility)
			{
				return null;
			}
			return this.m_fields[slot];
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
					if (this.m_fields[i] == null)
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
		public virtual int payFieldMoneyToWeek()
		{
			return int.Parse(this.m_farm.PayFieldMoney.Split(new char[]
			{
				'|'
			})[0].Split(new char[]
			{
				','
			})[1]);
		}
		public virtual int payFieldTimeToWeek()
		{
			return int.Parse(this.m_farm.PayFieldMoney.Split(new char[]
			{
				'|'
			})[0].Split(new char[]
			{
				','
			})[0]);
		}
		public virtual int payFieldMoneyToMonth()
		{
			return int.Parse(this.m_farm.PayFieldMoney.Split(new char[]
			{
				'|'
			})[1].Split(new char[]
			{
				','
			})[1]);
		}
		public virtual int payFieldTimeToMonth()
		{
			return int.Parse(this.m_farm.PayFieldMoney.Split(new char[]
			{
				'|'
			})[1].Split(new char[]
			{
				','
			})[0]);
		}
		public virtual UserFarmInfo CreateFarmForNulll(int ID)
		{
			return new UserFarmInfo
			{
				FarmID = ID,
				FarmerName = "Null",
				isFarmHelper = false,
				isAutoId = 0,
				AutoPayTime = DateTime.Now,
				AutoValidDate = 1,
				GainFieldId = 0,
				KillCropId = 0
			};
		}
		public virtual UserFieldInfo[] CreateFieldsForNull(int ID)
		{
			List<UserFieldInfo> list = new List<UserFieldInfo>();
			for (int i = 0; i < 8; i++)
			{
				list.Add(new UserFieldInfo
				{
					FarmID = ID,
					FieldID = i,
					SeedID = 0,
					PayTime = DateTime.Now,
					payFieldTime = 365000,
					PlantTime = DateTime.Now,
					GainCount = 0,
					FieldValidDate = 1,
					AccelerateTime = 0,
					AutomaticTime = DateTime.Now
				});
			}
			return list.ToArray();
		}
	}
}
