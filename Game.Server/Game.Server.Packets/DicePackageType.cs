using System;
namespace Game.Server.Packets
{
	public enum DicePackageType
	{
		DICE_ACTIVE_OPEN = 1,
		DICE_ACTIVE_CLOSE,
		DICE_RECEIVE_DATA,
		DICE_RECEIVE_RESULT
	}
}
