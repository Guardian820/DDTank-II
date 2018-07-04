using System;
namespace SqlDataProvider.Data
{
	public class UserTreasureInfo : DataObject
	{
		private int _ID;
		private int _userID;
		private string _NickName;
		private int _logoinDays;
		private int _treasure;
		private int _treasureAdd;
		private int _friendHelpTimes;
		private bool _isEndTreasure;
		private int _isBeginTreasure;
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
		public string NickName
		{
			get
			{
				return this._NickName;
			}
			set
			{
				this._NickName = value;
				this._isDirty = true;
			}
		}
		public int logoinDays
		{
			get
			{
				return this._logoinDays;
			}
			set
			{
				this._logoinDays = value;
				this._isDirty = true;
			}
		}
		public int treasure
		{
			get
			{
				return this._treasure;
			}
			set
			{
				this._treasure = value;
				this._isDirty = true;
			}
		}
		public int treasureAdd
		{
			get
			{
				return this._treasureAdd;
			}
			set
			{
				this._treasureAdd = value;
				this._isDirty = true;
			}
		}
		public int friendHelpTimes
		{
			get
			{
				return this._friendHelpTimes;
			}
			set
			{
				this._friendHelpTimes = value;
				this._isDirty = true;
			}
		}
		public bool isEndTreasure
		{
			get
			{
				return this._isEndTreasure;
			}
			set
			{
				this._isEndTreasure = value;
				this._isDirty = true;
			}
		}
		public int isBeginTreasure
		{
			get
			{
				return this._isBeginTreasure;
			}
			set
			{
				this._isBeginTreasure = value;
				this._isDirty = true;
			}
		}
	}
}
