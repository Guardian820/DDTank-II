using System;
namespace SqlDataProvider.Data
{
	public class RateInfo
	{
		public int ServerID
		{
			get;
			set;
		}
		public int Type
		{
			get;
			set;
		}
		public float Rate
		{
			get;
			set;
		}
		public DateTime BeginDay
		{
			get;
			set;
		}
		public DateTime EndDay
		{
			get;
			set;
		}
		public DateTime BeginTime
		{
			get;
			set;
		}
		public DateTime EndTime
		{
			get;
			set;
		}
	}
}
