using System;
namespace SqlDataProvider.Data
{
	public class UsersCardInfo : DataObject
	{
		private int _cardID;
		private int _cardType;
		private int _userID;
		private int _place;
		private int _type;
		private int _templateID;
		private bool _isFirstGet;
		private int _attack;
		private int _defence;
		private int _luck;
		private int _agility;
		private int _damage;
		private int _guard;
		private bool _isExit;
		private int _level;
		private int _cardGp;
		public int CardID
		{
			get
			{
				return this._cardID;
			}
			set
			{
				this._cardID = value;
				this._isDirty = true;
			}
		}
		public int CardType
		{
			get
			{
				return this._cardType;
			}
			set
			{
				this._cardType = value;
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
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
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
		public bool isFirstGet
		{
			get
			{
				return this._isFirstGet;
			}
			set
			{
				this._isFirstGet = value;
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
		public int CardGP
		{
			get
			{
				return this._cardGp;
			}
			set
			{
				this._cardGp = value;
				this._isDirty = true;
			}
		}
	}
}
