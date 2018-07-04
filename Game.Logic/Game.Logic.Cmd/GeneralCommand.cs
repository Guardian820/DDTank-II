using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(23, "使用飞行技能")]
	public class GeneralCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			packet.ReadInt();
		}
	}
}
