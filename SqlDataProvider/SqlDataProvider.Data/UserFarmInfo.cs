using System;
namespace SqlDataProvider.Data
{
	public class UserFarmInfo : DataObject
	{
		private UserFieldInfo _field;
		private int _ID;
		private int _farmID;
		private string _payFieldMoney;
		private string _payAutoMoney;
		private DateTime _autoPayTime;
		private int _autoValidDate;
		private int _vipLimitLevel;
		private string _farmerName;
		private int _gainFieldId;
		private int _matureId;
		private int _killCropId;
		private int _isAutoId;
		private bool _isFarmHelper;
		private int _buyExpRemainNum;
		private bool _isArrange;
		public UserFieldInfo Field
		{
			get
			{
				return this._field;
			}
			set
			{
				this._field = value;
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
		public int FarmID
		{
			get
			{
				return this._farmID;
			}
			set
			{
				this._farmID = value;
				this._isDirty = true;
			}
		}
		public string PayFieldMoney
		{
			get
			{
				return this._payFieldMoney;
			}
			set
			{
				this._payFieldMoney = value;
				this._isDirty = true;
			}
		}
		public string PayAutoMoney
		{
			get
			{
				return this._payAutoMoney;
			}
			set
			{
				this._payAutoMoney = value;
				this._isDirty = true;
			}
		}
		public DateTime AutoPayTime
		{
			get
			{
				return this._autoPayTime;
			}
			set
			{
				this._autoPayTime = value;
				this._isDirty = true;
			}
		}
		public int AutoValidDate
		{
			get
			{
				return this._autoValidDate;
			}
			set
			{
				this._autoValidDate = value;
				this._isDirty = true;
			}
		}
		public int VipLimitLevel
		{
			get
			{
				return this._vipLimitLevel;
			}
			set
			{
				this._vipLimitLevel = value;
				this._isDirty = true;
			}
		}
		public string FarmerName
		{
			get
			{
				return this._farmerName;
			}
			set
			{
				this._farmerName = value;
				this._isDirty = true;
			}
		}
		public int GainFieldId
		{
			get
			{
				return this._gainFieldId;
			}
			set
			{
				this._gainFieldId = value;
				this._isDirty = true;
			}
		}
		public int MatureId
		{
			get
			{
				return this._matureId;
			}
			set
			{
				this._matureId = value;
				this._isDirty = true;
			}
		}
		public int KillCropId
		{
			get
			{
				return this._killCropId;
			}
			set
			{
				this._killCropId = value;
				this._isDirty = true;
			}
		}
		public int isAutoId
		{
			get
			{
				return this._isAutoId;
			}
			set
			{
				this._isAutoId = value;
				this._isDirty = true;
			}
		}
		public bool isFarmHelper
		{
			get
			{
				return this._isFarmHelper;
			}
			set
			{
				this._isFarmHelper = value;
				this._isDirty = true;
			}
		}
		public int buyExpRemainNum
		{
			get
			{
				return this._buyExpRemainNum;
			}
			set
			{
				this._buyExpRemainNum = value;
				this._isDirty = true;
			}
		}
		public bool isArrange
		{
			get
			{
				return this._isArrange;
			}
			set
			{
				this._isArrange = value;
				this._isDirty = true;
			}
		}
	}
}
