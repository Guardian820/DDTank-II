using System;
namespace SqlDataProvider.Data
{
	public class QuestInfo : DataObject
	{
		public int ID
		{
			get;
			set;
		}
		public int QuestID
		{
			get;
			set;
		}
		public string Title
		{
			get;
			set;
		}
		public string Detail
		{
			get;
			set;
		}
		public string Objective
		{
			get;
			set;
		}
		public int NeedMinLevel
		{
			get;
			set;
		}
		public int NeedMaxLevel
		{
			get;
			set;
		}
		public string PreQuestID
		{
			get;
			set;
		}
		public string NextQuestID
		{
			get;
			set;
		}
		public int IsOther
		{
			get;
			set;
		}
		public bool CanRepeat
		{
			get;
			set;
		}
		public int RepeatInterval
		{
			get;
			set;
		}
		public int RepeatMax
		{
			get;
			set;
		}
		public int RewardGP
		{
			get;
			set;
		}
		public int RewardGold
		{
			get;
			set;
		}
		public int RewardBindMoney
		{
			get;
			set;
		}
		public int RewardOffer
		{
			get;
			set;
		}
		public int RewardRiches
		{
			get;
			set;
		}
		public int RewardBuffID
		{
			get;
			set;
		}
		public int RewardBuffDate
		{
			get;
			set;
		}
		public int RewardMoney
		{
			get;
			set;
		}
		public decimal Rands
		{
			get;
			set;
		}
		public int RandDouble
		{
			get;
			set;
		}
		public bool TimeMode
		{
			get;
			set;
		}
		public DateTime StartDate
		{
			get;
			set;
		}
		public DateTime EndDate
		{
			get;
			set;
		}
		public int MapID
		{
			get;
			set;
		}
		public bool AutoEquip
		{
			get;
			set;
		}
		public int OneKeyFinishNeedMoney
		{
			get;
			set;
		}
		public string Rank
		{
			get;
			set;
		}
		public int StarLev
		{
			get;
			set;
		}
		public int NotMustCount
		{
			get;
			set;
		}
		public int Level2NeedMoney
		{
			get;
			set;
		}
		public int Level3NeedMoney
		{
			get;
			set;
		}
		public int Level4NeedMoney
		{
			get;
			set;
		}
		public int Level5NeedMoney
		{
			get;
			set;
		}
	}
}
