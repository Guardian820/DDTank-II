using System;
namespace SqlDataProvider.Data
{
	public class UserFieldInfo : DataObject
	{
		private int _id;
		private int _farmID;
		private int _fieldID;
		private int _seedID;
		private DateTime _plantTime;
		private int _accelerateTime;
		private int _fieldValidDate;
		private DateTime _payTime;
		private int _payFieldTime;
		private int _gainCount;
		private int _gainFieldId;
		private int _autoSeedID;
		private int _autoFertilizerID;
		private int _autoSeedIDCount;
		private int _autoFertilizerCount;
		private bool _isAutomatic;
		private bool _isExist;
		private DateTime _automaticTime;
		public int ID
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
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
		public int FieldID
		{
			get
			{
				return this._fieldID;
			}
			set
			{
				this._fieldID = value;
				this._isDirty = true;
			}
		}
		public int SeedID
		{
			get
			{
				return this._seedID;
			}
			set
			{
				this._seedID = value;
				this._isDirty = true;
			}
		}
		public DateTime PlantTime
		{
			get
			{
				return this._plantTime;
			}
			set
			{
				this._plantTime = value;
				this._isDirty = true;
			}
		}
		public int AccelerateTime
		{
			get
			{
				return this._accelerateTime;
			}
			set
			{
				this._accelerateTime = value;
				this._isDirty = true;
			}
		}
		public int FieldValidDate
		{
			get
			{
				return this._fieldValidDate;
			}
			set
			{
				this._fieldValidDate = value;
				this._isDirty = true;
			}
		}
		public DateTime PayTime
		{
			get
			{
				return this._payTime;
			}
			set
			{
				this._payTime = value;
				this._isDirty = true;
			}
		}
		public int payFieldTime
		{
			get
			{
				return this._payFieldTime;
			}
			set
			{
				this._payFieldTime = value;
				this._isDirty = true;
			}
		}
		public int GainCount
		{
			get
			{
				return this._gainCount;
			}
			set
			{
				this._gainCount = value;
				this._isDirty = true;
			}
		}
		public int gainFieldId
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
		public int AutoSeedID
		{
			get
			{
				return this._autoSeedID;
			}
			set
			{
				this._autoSeedID = value;
				this._isDirty = true;
			}
		}
		public int AutoFertilizerID
		{
			get
			{
				return this._autoFertilizerID;
			}
			set
			{
				this._autoFertilizerID = value;
				this._isDirty = true;
			}
		}
		public int AutoSeedIDCount
		{
			get
			{
				return this._autoSeedIDCount;
			}
			set
			{
				this._autoSeedIDCount = value;
				this._isDirty = true;
			}
		}
		public int AutoFertilizerCount
		{
			get
			{
				return this._autoFertilizerCount;
			}
			set
			{
				this._autoFertilizerCount = value;
				this._isDirty = true;
			}
		}
		public bool isAutomatic
		{
			get
			{
				return this._isAutomatic;
			}
			set
			{
				this._isAutomatic = value;
				this._isDirty = true;
			}
		}
		public bool IsExit
		{
			get
			{
				return this._isExist;
			}
			set
			{
				this._isExist = value;
				this._isDirty = true;
			}
		}
		public DateTime AutomaticTime
		{
			get
			{
				return this._automaticTime;
			}
			set
			{
				this._automaticTime = value;
				this._isDirty = true;
			}
		}
		public bool isDig()
		{
			TimeSpan timeSpan = DateTime.Now - this._plantTime;
			int num = this._fieldValidDate - (int)timeSpan.TotalMinutes;
			return num > 0;
		}
	}
}
