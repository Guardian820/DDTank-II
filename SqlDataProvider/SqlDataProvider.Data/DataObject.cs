using System;
namespace SqlDataProvider.Data
{
	public class DataObject
	{
		protected bool _isDirty;
		public bool IsDirty
		{
			get
			{
				return this._isDirty;
			}
			set
			{
				this._isDirty = value;
			}
		}
	}
}
