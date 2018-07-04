using System;
namespace SqlDataProvider.Data
{
	public class TexpInfo : DataObject
	{
		private int _userID;
		private int _spdTexpExp;
		private int _attTexpExp;
		private int _defTexpExp;
		private int _hpTexpExp;
		private int _lukTexpExp;
		private int _texpTaskCount;
		private int _texpCount;
		private DateTime _texpTaskDate;
		public int ID
		{
			get;
			set;
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
		public int spdTexpExp
		{
			get
			{
				return this._spdTexpExp;
			}
			set
			{
				this._spdTexpExp = value;
				this._isDirty = true;
			}
		}
		public int attTexpExp
		{
			get
			{
				return this._attTexpExp;
			}
			set
			{
				this._attTexpExp = value;
				this._isDirty = true;
			}
		}
		public int defTexpExp
		{
			get
			{
				return this._defTexpExp;
			}
			set
			{
				this._defTexpExp = value;
				this._isDirty = true;
			}
		}
		public int hpTexpExp
		{
			get
			{
				return this._hpTexpExp;
			}
			set
			{
				this._hpTexpExp = value;
				this._isDirty = true;
			}
		}
		public int lukTexpExp
		{
			get
			{
				return this._lukTexpExp;
			}
			set
			{
				this._lukTexpExp = value;
				this._isDirty = true;
			}
		}
		public int texpTaskCount
		{
			get
			{
				return this._texpTaskCount;
			}
			set
			{
				this._texpTaskCount = value;
				this._isDirty = true;
			}
		}
		public int texpCount
		{
			get
			{
				return this._texpCount;
			}
			set
			{
				this._texpCount = value;
				this._isDirty = true;
			}
		}
		public DateTime texpTaskDate
		{
			get
			{
				return this._texpTaskDate;
			}
			set
			{
				this._texpTaskDate = value;
				this._isDirty = true;
			}
		}
	}
}
