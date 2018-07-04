using System;
namespace Game.Server.Packets
{
	public enum BattleGoundPackageType
	{
		OPEN = 1,
		OVER,
		UPDATE_VALUE_REQ,
		UPDATE_VALUE_REP,
		UPDATE_PLAYER_DATA,
		UPDATE_VALUE = 7
	}
}
