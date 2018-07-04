using System;
namespace Game.Server.Packets
{
	public enum ConsortiaPackageType
	{
        CONSORTIA_TRYIN = 0,
        CONSORTIA_CREATE = 1, //Tao G
        CONSORTIA_DISBAND = 2,
        CONSORTIA_RENEGADE = 3,
        CONSORTIA_TRYIN_PASS = 4,
        CONSORTIA_TRYIN_DEL = 5,
        CONSORTIA_RICHES_OFFER = 6, //Cong hien G
        CONSORTIA_APPLY_STATE = 7,
        CONSORTIA_DUTY_DELETE = 9,
        CONSORTIA_DUTY_UPDATE = 10,//Doi ten chuc vi
        CONSORTIA_INVITE = 11, // chieu mo thanh` vien
        CONSORTIA_INVITE_PASS = 12, //
        CONSORTIA_INVITE_DELETE = 13, //
        CONSORTIA_DESCRIPTION_UPDATE = 14, //tuyen bo G
        CONSORTIA_PLACARD_UPDATE = 15,//Thong Bao
        CONSORTIA_BANCHAT_UPDATE = 16,
        CONSORTIA_USER_REMARK_UPDATE = 17,
        CONSORTIA_USER_GRADE_UPDATE = 18, //tang ha chuc
        CONSORTIA_CHAIRMAN_CHAHGE = 19,//nhuong G
        CONSORTIA_CHAT = 20,
        CONSORTIA_LEVEL_UP = 21,//n�ng c?p Guild
        CONSORTIA_TASK_RELEASE = 22, // su menh G
        DONATE = 23,
        CONSORTIA_EQUIP_CONTROL = 24,//Quan Li
        POLL_CANDIDATE = 25,
        SKILL_SOCKET = 26,
        CONSORTION_MAIL = 29, //thu G
        BUY_BADGE = 28, //Mua Huy hieu G
		BOSS_OPEN_CLOSE = 31,
		CONSORTIA_BOSS_INFO = 30
	}
}
