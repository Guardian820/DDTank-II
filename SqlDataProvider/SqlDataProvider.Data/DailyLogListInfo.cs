using System;
namespace SqlDataProvider.Data
{
	public class DailyLogListInfo
	{
		public int ID
		{
			get;
			set;
		}
		public int UserAwardLog
		{
			get;
			set;
		}
		public int UserID
		{
			get;
			set;
		}
		public string DayLog
		{
			get;
			set;
		}
		public DateTime LastDate
		{
			get;
			set;
		}
	}
}
