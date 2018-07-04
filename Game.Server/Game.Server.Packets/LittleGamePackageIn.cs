using System;
namespace Game.Server.Packets
{
	public enum LittleGamePackageIn
	{
		LOAD_WORLD_LIST = 1,
		ENTER_WORLD,
		LOAD_COMPLETED,
		LEAVE_WORLD,
		PING = 6,
		MOVE = 32,
		POS_SYNC,
		REPORT_SCORE = 64,
		CLICK,
		CANCEL_CLICK
	}
}
