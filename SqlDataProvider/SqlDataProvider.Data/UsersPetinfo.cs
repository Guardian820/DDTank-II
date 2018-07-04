using System;
using System.Collections.Generic;
namespace SqlDataProvider.Data
{
	public class UsersPetinfo : DataObject
	{
		private string _skillEquip;
		private string _skill;
		private int _ID;
		private int _templateID;
		private string _name;
		private int _userID;
		private int _attack;
		private int _defence;
		private int _luck;
		private int _agility;
		private int _blood;
		private int _damage;
		private int _guard;
		private int _attackGrow;
		private int _defenceGrow;
		private int _luckGrow;
		private int _agilityGrow;
		private int _bloodGrow;
		private int _damageGrow;
		private int _guardGrow;
		private int _level;
		private int _gp;
		private int _maxGP;
		private int _hunger;
		private int _petHappyStar;
		private int _mp;
		private bool _isEquip;
		private int _place;
		private bool _isExit;
		public string SkillEquip
		{
			get
			{
				return this._skillEquip;
			}
			set
			{
				this._skillEquip = value;
				this._isDirty = true;
			}
		}
		public string Skill
		{
			get
			{
				return this._skill;
			}
			set
			{
				this._skill = value;
				this._isDirty = true;
			}
		}
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				this._ID = value;
				this._isDirty = true;
			}
		}
		public int TemplateID
		{
			get
			{
				return this._templateID;
			}
			set
			{
				this._templateID = value;
				this._isDirty = true;
			}
		}
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
				this._isDirty = true;
			}
		}
		public int UserID
		{
			get
			{
				return this._userID;
			}
			set
			{
				this._userID = value;
				this._isDirty = true;
			}
		}
		public int Attack
		{
			get
			{
				return this._attack;
			}
			set
			{
				this._attack = value;
				this._isDirty = true;
			}
		}
		public int Defence
		{
			get
			{
				return this._defence;
			}
			set
			{
				this._defence = value;
				this._isDirty = true;
			}
		}
		public int Luck
		{
			get
			{
				return this._luck;
			}
			set
			{
				this._luck = value;
				this._isDirty = true;
			}
		}
		public int Agility
		{
			get
			{
				return this._agility;
			}
			set
			{
				this._agility = value;
				this._isDirty = true;
			}
		}
		public int Blood
		{
			get
			{
				return this._blood;
			}
			set
			{
				this._blood = value;
				this._isDirty = true;
			}
		}
		public int Damage
		{
			get
			{
				return this._damage;
			}
			set
			{
				this._damage = value;
				this._isDirty = true;
			}
		}
		public int Guard
		{
			get
			{
				return this._guard;
			}
			set
			{
				this._guard = value;
				this._isDirty = true;
			}
		}
		public int AttackGrow
		{
			get
			{
				return this._attackGrow;
			}
			set
			{
				this._attackGrow = value;
				this._isDirty = true;
			}
		}
		public int DefenceGrow
		{
			get
			{
				return this._defenceGrow;
			}
			set
			{
				this._defenceGrow = value;
				this._isDirty = true;
			}
		}
		public int LuckGrow
		{
			get
			{
				return this._luckGrow;
			}
			set
			{
				this._luckGrow = value;
				this._isDirty = true;
			}
		}
		public int AgilityGrow
		{
			get
			{
				return this._agilityGrow;
			}
			set
			{
				this._agilityGrow = value;
				this._isDirty = true;
			}
		}
		public int BloodGrow
		{
			get
			{
				return this._bloodGrow;
			}
			set
			{
				this._bloodGrow = value;
				this._isDirty = true;
			}
		}
		public int DamageGrow
		{
			get
			{
				return this._damageGrow;
			}
			set
			{
				this._damageGrow = value;
				this._isDirty = true;
			}
		}
		public int GuardGrow
		{
			get
			{
				return this._guardGrow;
			}
			set
			{
				this._guardGrow = value;
				this._isDirty = true;
			}
		}
		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				this._level = value;
				this._isDirty = true;
			}
		}
		public int GP
		{
			get
			{
				return this._gp;
			}
			set
			{
				this._gp = value;
				this._isDirty = true;
			}
		}
		public int MaxGP
		{
			get
			{
				return this._maxGP;
			}
			set
			{
				this._maxGP = value;
				this._isDirty = true;
			}
		}
		public int Hunger
		{
			get
			{
				return this._hunger;
			}
			set
			{
				this._hunger = value;
				this._isDirty = true;
			}
		}
		public int PetHappyStar
		{
			get
			{
				return this._petHappyStar;
			}
			set
			{
				this._petHappyStar = value;
				this._isDirty = true;
			}
		}
		public int MP
		{
			get
			{
				return this._mp;
			}
			set
			{
				this._mp = value;
				this._isDirty = true;
			}
		}
		public bool IsEquip
		{
			get
			{
				return this._isEquip;
			}
			set
			{
				this._isEquip = value;
				this._isDirty = true;
			}
		}
		public int Place
		{
			get
			{
				return this._place;
			}
			set
			{
				this._place = value;
				this._isDirty = true;
			}
		}
		public bool IsExit
		{
			get
			{
				return this._isExit;
			}
			set
			{
				this._isExit = value;
				this._isDirty = true;
			}
		}
		public static PetType GetPetType(int Id)
		{
			if (Id <= 59)
			{
				switch (Id)
				{
				case 1:
					return PetType.FORZEN;

				case 2:
				case 4:
					return PetType.Normal;

				case 3:
					return PetType.TRANFORM;

				case 5:
					break;

				default:
					if (Id != 59)
					{
						return PetType.Normal;
					}
					break;
				}
			}
			else
			{
				if (Id != 64)
				{
					switch (Id)
					{
					case 97:
					case 98:
						break;

					default:
						return PetType.Normal;
					}
				}
			}
			return PetType.CURE;
		}
		public List<string> GetSkill()
		{
			List<string> list = new List<string>();
			string[] array = this._skill.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(array[i]);
			}
			return list;
		}
		public List<string> GetSkillEquip()
		{
			List<string> list = new List<string>();
			string[] array = this._skillEquip.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(array[i]);
			}
			return list;
		}
	}
}
