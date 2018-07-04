using System;
namespace Game.Server.Packets
{
	public enum eMailType
	{
		Default,
		Common,
		AuctionSuccess,
		AuctionFail,
		BidSuccess,
		BidFail,
		ReturnPayment,
		PaymentCancel,
		BuyItem,
		ItemOverdue,
		PresentItem,
		PaymentFinish,
		OpenUpArk,
		StoreCanel,
		Marry,
		DailyAward,
		Manage = 51,
		Active,
		GiftGuide = 55,
		AdvertMail = 58,
		ConsortionEmail,
		FriendBrithday,
		MyseftBrithday,
		ConsortionSkillMail,
		Payment = 101
	}
}
