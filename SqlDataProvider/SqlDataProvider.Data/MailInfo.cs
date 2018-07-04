using System;
namespace SqlDataProvider.Data
{
	public class MailInfo
	{
		public int ID
		{
			get;
			set;
		}
		public int SenderID
		{
			get;
			set;
		}
		public string Sender
		{
			get;
			set;
		}
		public int ReceiverID
		{
			get;
			set;
		}
		public string Receiver
		{
			get;
			set;
		}
		public string Title
		{
			get;
			set;
		}
		public string Content
		{
			get;
			set;
		}
		public string Annex1
		{
			get;
			set;
		}
		public string Annex2
		{
			get;
			set;
		}
		public int Gold
		{
			get;
			set;
		}
		public int Money
		{
			get;
			set;
		}
		public bool IsExist
		{
			get;
			set;
		}
		public int Type
		{
			get;
			set;
		}
		public int ValidDate
		{
			get;
			set;
		}
		public bool IsRead
		{
			get;
			set;
		}
		public DateTime SendTime
		{
			get;
			set;
		}
		public string Annex1Name
		{
			get;
			set;
		}
		public string Annex2Name
		{
			get;
			set;
		}
		public string Annex3
		{
			get;
			set;
		}
		public string Annex4
		{
			get;
			set;
		}
		public string Annex5
		{
			get;
			set;
		}
		public string Annex3Name
		{
			get;
			set;
		}
		public string Annex4Name
		{
			get;
			set;
		}
		public string Annex5Name
		{
			get;
			set;
		}
		public string AnnexRemark
		{
			get;
			set;
		}
		public int GiftToken
		{
			get;
			set;
		}
		public void Revert()
		{
			this.ID = 0;
			this.SenderID = 0;
			this.Sender = "";
			this.ReceiverID = 0;
			this.Receiver = "";
			this.Title = "";
			this.Content = "";
			this.Annex1 = "";
			this.Annex2 = "";
			this.Gold = 0;
			this.Money = 0;
			this.GiftToken = 0;
			this.IsExist = false;
			this.Type = 0;
			this.ValidDate = 0;
			this.IsRead = false;
			this.SendTime = DateTime.Now;
			this.Annex1Name = "";
			this.Annex2Name = "";
			this.Annex3 = "";
			this.Annex4 = "";
			this.Annex5 = "";
			this.Annex3Name = "";
			this.Annex4Name = "";
			this.Annex5Name = "";
			this.AnnexRemark = "";
		}
	}
}
