using Bussiness;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.GameUtils
{
	public class PetInventory : PetAbstractInventory
	{
		protected GamePlayer m_player;
		private bool m_saveToDb;
		private List<UsersPetinfo> m_removedList = new List<UsersPetinfo>();
		private List<UsersPetinfo> m_removedAdoptPetList = new List<UsersPetinfo>();
		public GamePlayer Player
		{
			get
			{
				return this.m_player;
			}
		}
		public PetInventory(GamePlayer player, bool saveTodb, int capibility, int aCapability, int beginSlot) : base(capibility, aCapability, beginSlot)
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
					UsersPetinfo[] userPetSingles = playerBussiness.GetUserPetSingles(this.m_player.PlayerCharacter.ID);
					UsersPetinfo[] userAdoptPetSingles = playerBussiness.GetUserAdoptPetSingles(this.m_player.PlayerCharacter.ID);
					base.BeginChanges();
					try
					{
						UsersPetinfo[] array = userPetSingles;
						for (int i = 0; i < array.Length; i++)
						{
							UsersPetinfo usersPetinfo = array[i];
							this.AddPetTo(usersPetinfo, usersPetinfo.Place);
						}
						UsersPetinfo[] array2 = userAdoptPetSingles;
						for (int j = 0; j < array2.Length; j++)
						{
							UsersPetinfo usersPetinfo2 = array2[j];
							this.AddAdoptPetTo(usersPetinfo2, usersPetinfo2.Place);
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
						for (int i = 0; i < this.m_pets.Length; i++)
						{
							UsersPetinfo usersPetinfo = this.m_pets[i];
							if (usersPetinfo != null && usersPetinfo.IsDirty)
							{
								playerBussiness.SaveUserPet(usersPetinfo);
							}
						}
					}
					finally
					{
						Monitor.Exit(@lock);
					}
					List<UsersPetinfo> removedList;
					Monitor.Enter(removedList = this.m_removedList);
					try
					{
						foreach (UsersPetinfo current in this.m_removedList)
						{
							playerBussiness.RemovePetSingle(current.ID);
						}
						this.m_removedList.Clear();
					}
					finally
					{
						Monitor.Exit(removedList);
					}
					List<UsersPetinfo> removedAdoptPetList;
					Monitor.Enter(removedAdoptPetList = this.m_removedAdoptPetList);
					try
					{
						foreach (UsersPetinfo current2 in this.m_removedAdoptPetList)
						{
							playerBussiness.RemoveUserAdoptPet(current2.ID);
						}
						this.m_removedAdoptPetList.Clear();
					}
					finally
					{
						Monitor.Exit(removedAdoptPetList);
					}
				}
			}
		}
		public override bool AddPetTo(UsersPetinfo pet, int place)
		{
			if (base.AddPetTo(pet, place))
			{
				pet.UserID = this.m_player.PlayerCharacter.ID;
				pet.IsExit = true;
				return true;
			}
			return false;
		}
		public override bool RemovePet(UsersPetinfo pet)
		{
			if (base.RemovePet(pet))
			{
				if (this.m_saveToDb)
				{
					List<UsersPetinfo> removedList;
					Monitor.Enter(removedList = this.m_removedList);
					try
					{
						pet.IsExit = false;
						this.m_removedList.Add(pet);
					}
					finally
					{
						Monitor.Exit(removedList);
					}
				}
				return true;
			}
			return false;
		}
		public override bool AddAdoptPetTo(UsersPetinfo pet, int place)
		{
			if (base.AddAdoptPetTo(pet, place))
			{
				pet.UserID = this.m_player.PlayerCharacter.ID;
				pet.IsExit = true;
				return true;
			}
			return false;
		}
		public override bool RemoveAdoptPet(UsersPetinfo pet)
		{
			if (base.RemoveAdoptPet(pet))
			{
				if (this.m_saveToDb)
				{
					List<UsersPetinfo> removedAdoptPetList;
					Monitor.Enter(removedAdoptPetList = this.m_removedAdoptPetList);
					try
					{
						this.m_removedAdoptPetList.Add(pet);
					}
					finally
					{
						Monitor.Exit(removedAdoptPetList);
					}
				}
				return true;
			}
			return false;
		}
		public override void UpdateChangedPlaces()
		{
			int[] slots = this.m_changedPlaces.ToArray();
			this.m_player.Out.SendUpdateUserPet(this, slots);
			base.UpdateChangedPlaces();
		}
		public virtual void ClearAdoptPets()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = 0; i < base.ACapalility; i++)
				{
					if (this.m_adoptPets[i] != null)
					{
						this.m_adoptPets[i].IsExit = false;
						this.m_removedAdoptPetList.Add(this.m_adoptPets[i]);
					}
					this.m_adoptPets[i] = null;
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
	}
}
