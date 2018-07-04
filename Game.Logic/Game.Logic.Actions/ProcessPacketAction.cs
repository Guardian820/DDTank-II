using Game.Base.Packets;
using Game.Logic.Cmd;
using Game.Logic.Phy.Object;
using log4net;
using System;
using System.Reflection;
namespace Game.Logic.Actions
{
	public class ProcessPacketAction : IAction
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private Player m_player;
		private GSPacketIn m_packet;
		public ProcessPacketAction(Player player, GSPacketIn pkg)
		{
			this.m_player = player;
			this.m_packet = pkg;
		}
		public void Execute(BaseGame game, long tick)
		{
			if (this.m_player.IsActive)
			{
				eTankCmdType eTankCmdType = (eTankCmdType)this.m_packet.ReadByte();
				try
				{
					ICommandHandler commandHandler = CommandMgr.LoadCommandHandler((int)eTankCmdType);
					if (commandHandler != null)
					{
						commandHandler.HandleCommand(game, this.m_player, this.m_packet);
					}
					else
					{
						ProcessPacketAction.log.Error(string.Format("Player Id: {0}", this.m_player.Id));
					}
				}
				catch (Exception exception)
				{
					ProcessPacketAction.log.Error(string.Format("Player Id: {0}  cmd:0x{1:X2}", this.m_player.Id, (byte)eTankCmdType), exception);
				}
			}
		}
		public bool IsFinished(long tick)
		{
			return true;
		}
	}
}
