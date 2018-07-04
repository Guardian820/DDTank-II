using System;
namespace Game.Server.Packets
{
	public enum ePetType
	{
		UPDATE_PET = 1,
		MOVE_PETBAG = 3,
		FEED_PET,
		EQUIP_PET_SKILL = 7,
		RENAME_PET = 9,
		RELEASE_PET = 8,
		ADOPT_PET = 6,
		REFRESH_PET = 5,
		PAY_SKILL = 16,
		FIGHT_PET,
		ADD_PET = 2
	}
}
