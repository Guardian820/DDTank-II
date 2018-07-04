using System;
namespace SqlDataProvider.Data
{
	public class UserDrillInfo : DataObject
	{
		private int _userID;
		private int _beadPlace;
		private int _holeExp;
		private int _holeLv;
		private int _drillPlace;
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
		public int BeadPlace
		{
			get
			{
				return this._beadPlace;
			}
			set
			{
				this._beadPlace = value;
				this._isDirty = true;
			}
		}
		public int HoleExp
		{
			get
			{
				return this._holeExp;
			}
			set
			{
				this._holeExp = value;
				this._isDirty = true;
			}
		}
		public int HoleLv
		{
			get
			{
				return this._holeLv;
			}
			set
			{
				this._holeLv = value;
				this._isDirty = true;
			}
		}
		public int DrillPlace
		{
			get
			{
				return this._drillPlace;
			}
			set
			{
				this._drillPlace = value;
				this._isDirty = true;
			}
		}
	}
}
