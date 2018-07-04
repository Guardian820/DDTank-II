using System;
using System.Collections.Generic;
namespace SqlDataProvider.Data
{
	public class GiftInfo : DataObject
	{
		private DateTime _addDate;
		private int _count;
		private int _itemID;
		private Dictionary<string, object> _tempInfo = new Dictionary<string, object>();
		private ItemTemplateInfo _template;
		private int _templateId;
		private int _userID;
		public DateTime AddDate
		{
			get
			{
				return this._addDate;
			}
			set
			{
				if (!(this._addDate == value))
				{
					this._addDate = value;
					this._isDirty = true;
				}
			}
		}
		public int Count
		{
			get
			{
				return this._count;
			}
			set
			{
				if (this._count != value)
				{
					this._count = value;
					this._isDirty = true;
				}
			}
		}
		public int ItemID
		{
			get
			{
				return this._itemID;
			}
			set
			{
				this._itemID = value;
				this._isDirty = true;
			}
		}
		public Dictionary<string, object> TempInfo
		{
			get
			{
				return this._tempInfo;
			}
		}
		public ItemTemplateInfo Template
		{
			get
			{
				return this._template;
			}
		}
		public int TemplateID
		{
			get
			{
				return this._templateId;
			}
			set
			{
				if (this._templateId != value)
				{
					this._templateId = value;
					this._isDirty = true;
				}
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
				if (this._userID != value)
				{
					this._userID = value;
					this._isDirty = true;
				}
			}
		}
		internal GiftInfo(ItemTemplateInfo template)
		{
			this._template = template;
			if (this._template != null)
			{
				this._templateId = this._template.TemplateID;
			}
			if (this._tempInfo == null)
			{
				this._tempInfo = new Dictionary<string, object>();
			}
		}
		public bool CanStackedTo(GiftInfo to)
		{
			return this._templateId == to.TemplateID && this.Template.MaxCount > 1;
		}
		public static GiftInfo CreateFromTemplate(ItemTemplateInfo template, int count)
		{
			if (template == null)
			{
				throw new ArgumentNullException("template");
			}
			return new GiftInfo(template)
			{
				TemplateID = template.TemplateID,
				IsDirty = false,
				AddDate = DateTime.Now,
				Count = count
			};
		}
		public static GiftInfo CreateWithoutInit(ItemTemplateInfo template)
		{
			return new GiftInfo(template);
		}
	}
}
