using System;
namespace SqlDataProvider.Data
{
	public class AchievementDataInfo : DataObject
	{
		private int _achievementID;
		private DateTime _completedDate;
		private bool _isComplete;
		private int _userID;
		public int AchievementID
		{
			get
			{
				return this._achievementID;
			}
			set
			{
				this._achievementID = value;
				this._isDirty = true;
			}
		}
		public DateTime CompletedDate
		{
			get
			{
				return this._completedDate;
			}
			set
			{
				this._completedDate = value;
				this._isDirty = true;
			}
		}
		public bool IsComplete
		{
			get
			{
				return this._isComplete;
			}
			set
			{
				this._isComplete = value;
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
	}
}
