using Game.Logic;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.GameUtils
{
	public abstract class PetAbstractInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected object m_lock = new object();
		private int m_capalility;
		private int m_aCapalility;
		private int m_beginSlot;
		protected UsersPetinfo[] m_pets;
		protected UsersPetinfo[] m_adoptPets;
		protected ItemInfo[] m_adoptItems;
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
				this.m_capalility = ((value < 0) ? 0 : ((value > this.m_pets.Length) ? this.m_pets.Length : value));
			}
		}
		public int ACapalility
		{
			get
			{
				return this.m_aCapalility;
			}
			set
			{
				this.m_aCapalility = ((value < 0) ? 0 : ((value > this.m_adoptPets.Length) ? this.m_adoptPets.Length : value));
			}
		}
		public bool IsEmpty(int slot)
		{
			return slot < 0 || slot >= this.m_capalility || this.m_pets[slot] == null;
		}
		public PetAbstractInventory(int capability, int aCapability, int beginSlot)
		{
			this.m_capalility = capability;
			this.m_aCapalility = aCapability;
			this.m_beginSlot = beginSlot;
			this.m_pets = new UsersPetinfo[capability];
			this.m_adoptPets = new UsersPetinfo[aCapability];
		}
		public virtual UsersPetinfo GetPetIsEquip()
		{
			for (int i = 0; i < this.m_capalility; i++)
			{
				if (this.m_pets[i] != null && this.m_pets[i].IsEquip)
				{
					return this.m_pets[i];
				}
			}
			return null;
		}
		public virtual bool AddAdoptPetTo(UsersPetinfo pet, int place)
		{
			if (pet == null || place >= this.m_aCapalility || place < 0)
			{
				return false;
			}
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				if (this.m_adoptPets[place] != null)
				{
					place = -1;
				}
				else
				{
					this.m_adoptPets[place] = pet;
					pet.Place = place;
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return place != -1;
		}
		public virtual bool RemoveAdoptPet(UsersPetinfo pet)
		{
			if (pet == null)
			{
				return false;
			}
			int num = -1;
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = 0; i < this.m_aCapalility; i++)
				{
					if (this.m_adoptPets[i] == pet)
					{
						num = i;
						this.m_adoptPets[i] = null;
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
		public bool AddPet(UsersPetinfo pet)
		{
			return this.AddPet(pet, this.m_beginSlot);
		}
		public bool AddPet(UsersPetinfo pet, int minSlot)
		{
			if (pet == null)
			{
				return false;
			}
			int place = this.FindFirstEmptySlot(minSlot);
			return this.AddPetTo(pet, place);
		}
		public virtual bool AddPetTo(UsersPetinfo pet, int place)
		{
			if (pet == null || place >= this.m_capalility || place < 0)
			{
				return false;
			}
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				if (this.m_pets[place] == null)
				{
					this.m_pets[place] = pet;
					pet.Place = place;
				}
				else
				{
					place = -1;
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
		public virtual bool RemovePet(UsersPetinfo pet)
		{
			if (pet == null)
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
					if (this.m_pets[i] == pet)
					{
						num = i;
						this.m_pets[i] = null;
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
				pet.Place = -1;
			}
			return num != -1;
		}
		public bool RemovePetAt(int place)
		{
			return this.RemovePet(this.GetPetAt(place));
		}
		public virtual UsersPetinfo GetAdoptPetAt(int slot)
		{
			if (slot < 0 || slot >= this.m_aCapalility)
			{
				return null;
			}
			return this.m_adoptPets[slot];
		}
		public virtual UsersPetinfo GetPetAt(int slot)
		{
			if (slot < 0 || slot >= this.m_capalility)
			{
				return null;
			}
			return this.m_pets[slot];
		}
		public virtual UsersPetinfo[] GetAdoptPet()
		{
			List<UsersPetinfo> list = new List<UsersPetinfo>();
			for (int i = 0; i < this.m_aCapalility; i++)
			{
				if (this.m_adoptPets[i] != null && this.m_adoptPets[i].IsExit)
				{
					list.Add(this.m_adoptPets[i]);
				}
			}
			return list.ToArray();
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
					if (this.m_pets[i] == null)
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
		public int FindLastEmptySlot()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			int result;
			try
			{
				for (int i = this.m_capalility - 1; i >= 0; i--)
				{
					if (this.m_pets[i] == null)
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
					this.m_pets[i] = null;
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public virtual UsersPetinfo GetPetByTemplateID(int minSlot, int templateId)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			UsersPetinfo result;
			try
			{
				for (int i = minSlot; i < this.m_capalility; i++)
				{
					if (this.m_pets[i] != null && this.m_pets[i].TemplateID == templateId)
					{
						result = this.m_pets[i];
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
		public virtual UsersPetinfo[] GetPets()
		{
			return this.m_pets;
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
					if (this.m_pets[i] == null)
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
				if (PetAbstractInventory.log.IsErrorEnabled)
				{
					PetAbstractInventory.log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
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
		public virtual bool RenamePet(int place, string name)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			bool result;
			try
			{
				this.m_pets[place].Name = name;
				result = true;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public bool IsEquipSkill(int slot, string kill)
		{
			List<string> skillEquip = this.m_pets[slot].GetSkillEquip();
			for (int i = 0; i < skillEquip.Count; i++)
			{
				if (skillEquip[i].Split(new char[]
				{
					','
				})[0] == kill)
				{
					return false;
				}
			}
			return true;
		}
		public virtual bool EquipSkillPet(int place, int killId, int killindex)
		{
			string skill = killId + "," + killindex;
			UsersPetinfo pet = this.m_pets[place];
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				if (killId == 0)
				{
					this.m_pets[place].SkillEquip = this.SetSkillEquip(pet, killindex, skill);
					bool result = true;
					return result;
				}
				if (this.IsEquipSkill(place, killId.ToString()))
				{
					this.m_pets[place].SkillEquip = this.SetSkillEquip(pet, killindex, skill);
					bool result = true;
					return result;
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return false;
		}
		public string SetSkillEquip(UsersPetinfo pet, int place, string skill)
		{
			List<string> skillEquip = pet.GetSkillEquip();
			skillEquip[place] = skill;
			string text = skillEquip[0];
			for (int i = 1; i < skillEquip.Count; i++)
			{
				text = text + "|" + skillEquip[i];
			}
			return text;
		}
		public virtual bool SetIsEquip(int place, bool isEquip)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = 0; i < this.m_pets.Length; i++)
				{
					if (this.m_pets[i] != null)
					{
						if (this.m_pets[i].Place == place)
						{
							this.m_pets[i].IsEquip = isEquip;
						}
						else
						{
							this.m_pets[i].IsEquip = false;
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return true;
		}
		public virtual bool UpGracePet(UsersPetinfo pet, int place, bool isUpdateProp, int min, int max, ref string msg)
		{
			if (isUpdateProp)
			{
				int blood = 0;
				int attack = 0;
				int defence = 0;
				int agility = 0;
				int luck = 0;
				PetMgr.PlusPetProp(pet, min, max, ref blood, ref attack, ref defence, ref agility, ref luck);
				pet.Blood = blood;
				pet.Attack = attack;
				pet.Defence = defence;
				pet.Agility = agility;
				pet.Luck = luck;
				int num = PetMgr.UpdateEvolution(pet.TemplateID, max);
				pet.TemplateID = ((num == 0) ? pet.TemplateID : num);
				string skill = pet.Skill;
				string text = PetMgr.UpdateSkillPet(max, pet.TemplateID);
				pet.Skill = ((text == "") ? skill : text);
				pet.SkillEquip = PetMgr.ActiveEquipSkill(max);
				if (max > min)
				{
					msg = pet.Name + " thăng cấp " + max;
				}
			}
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_pets[place] = pet;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return true;
		}
	}
}
