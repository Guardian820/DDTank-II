using System;
namespace SqlDataProvider.Data
{
	public class QuestDataInfo : DataObject
	{
		private int _userID;
		private int _questID;
		private int _questLevel;
		private int _condition1;
		private int _condition2;
		private int _condition3;
		private int _condition4;
		private bool _isComplete;
		private DateTime _completeDate;
		private bool _isExist;
		private int _repeatFinish;
		private int _randDobule;
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
		public int QuestID
		{
			get
			{
				return this._questID;
			}
			set
			{
				this._questID = value;
				this._isDirty = true;
			}
		}
		public int QuestLevel
		{
			get
			{
				return this._questLevel;
			}
			set
			{
				this._questLevel = value;
				this._isDirty = true;
			}
		}
		public int Condition1
		{
			get
			{
				return this._condition1;
			}
			set
			{
				this._condition1 = value;
				this._isDirty = true;
			}
		}
		public int Condition2
		{
			get
			{
				return this._condition2;
			}
			set
			{
				this._condition2 = value;
				this._isDirty = true;
			}
		}
		public int Condition3
		{
			get
			{
				return this._condition3;
			}
			set
			{
				this._condition3 = value;
				this._isDirty = true;
			}
		}
		public int Condition4
		{
			get
			{
				return this._condition4;
			}
			set
			{
				this._condition4 = value;
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
		public DateTime CompletedDate
		{
			get
			{
				return this._completeDate;
			}
			set
			{
				this._completeDate = value;
				this._isDirty = true;
			}
		}
		public bool IsExist
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
		public int RepeatFinish
		{
			get
			{
				return this._repeatFinish;
			}
			set
			{
				this._repeatFinish = value;
				this._isDirty = true;
			}
		}
		public int RandDobule
		{
			get
			{
				return this._randDobule;
			}
			set
			{
				this._randDobule = value;
				this._isDirty = true;
			}
		}
		public int GetConditionValue(int index)
		{
			switch (index)
			{
			case 0:
				return this.Condition1;

			case 1:
				return this.Condition2;

			case 2:
				return this.Condition3;

			case 3:
				return this.Condition4;

			default:
				throw new Exception("Quest condition index out of range.");
			}
		}
		public void SaveConditionValue(int index, int value)
		{
			switch (index)
			{
			case 0:
				this.Condition1 = value;
				return;

			case 1:
				this.Condition2 = value;
				return;

			case 2:
				this.Condition3 = value;
				return;

			case 3:
				this.Condition4 = value;
				return;

			default:
				throw new Exception("Quest condition index out of range.");
			}
		}
	}
}
