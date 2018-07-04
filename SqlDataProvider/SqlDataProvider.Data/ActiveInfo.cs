using System;
namespace SqlDataProvider.Data
{
	public class ActiveInfo
	{
		public int ActiveID
		{
			get;
			set;
		}
		public string Title
		{
			get;
			set;
		}
		public string Description
		{
			get;
			set;
		}
		public string Content
		{
			get;
			set;
		}
		public string AwardContent
		{
			get;
			set;
		}
		public int HasKey
		{
			get;
			set;
		}
		public DateTime StartDate
		{
			get;
			set;
		}
		public DateTime? EndDate
		{
			get;
			set;
		}
		public int IsOnly
		{
			get;
			set;
		}
		public int Type
		{
			get;
			set;
		}
		public string ActionTimeContent
		{
			get;
			set;
		}
		public bool IsAdvance
		{
			get;
			set;
		}
		public string GoodsExchangeTypes
		{
			get;
			set;
		}
		public string GoodsExchangeNum
		{
			get;
			set;
		}
		public string limitType
		{
			get;
			set;
		}
		public string limitValue
		{
			get;
			set;
		}
		public bool IsShow
		{
			get;
			set;
		}
	}
}
