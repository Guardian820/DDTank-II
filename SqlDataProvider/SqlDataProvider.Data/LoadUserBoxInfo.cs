using System;
namespace SqlDataProvider.Data
{
	public class LoadUserBoxInfo : DataObject
	{
		public int ID
		{
			get;
			set;
		}
		public int Type
		{
			get;
			set;
		}
		public int Level
		{
			get;
			set;
		}
		public int Condition
		{
			get;
			set;
		}
		public int TemplateID
		{
			get;
			set;
		}
	}
}
