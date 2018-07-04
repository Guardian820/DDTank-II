using System;
namespace Game.Server.Packets
{
	public enum FarmPackageType
	{
		ENTER_FARM = 1,
		GROW_FIELD,
		ACCELERATE_FIELD,
		GAIN_FIELD,
		PAY_FIELD = 6,
		COMPOSE_FOOD = 5,
		FRUSH_FIELD = 0,
		KILLCROP_FIELD = 7,
		HELPER_PAY_FIELD,
		HELPER_SWITCH_FIELD,
		EXIT_FARM = 16,
		BUY_PET_EXP_ITEM = 19
	}
}
