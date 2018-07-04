using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms.TankHandle;
using log4net;
using System;
using System.Reflection;
namespace Game.Server.SceneMarryRooms
{
	[MarryProcessor(9, "礼堂逻辑")]
	public class TankMarryLogicProcessor : AbstractMarryProcessor
	{
		private MarryCommandMgr _commandMgr;
		private ThreadSafeRandom random = new ThreadSafeRandom();
		public readonly int TIMEOUT = 60000;
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public TankMarryLogicProcessor()
		{
			this._commandMgr = new MarryCommandMgr();
		}
		public override void OnTick(MarryRoom room)
		{
			try
			{
				if (room != null)
				{
					room.KickAllPlayer();
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						playerBussiness.DisposeMarryRoomInfo(room.Info.ID);
					}
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(room.Info.GroomID);
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(room.Info.BrideID);
					GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(room.Info.GroomID, false, room.Info);
					GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(room.Info.BrideID, false, room.Info);
					MarryRoomMgr.RemoveMarryRoom(room);
					GSPacketIn gSPacketIn = new GSPacketIn(254);
					gSPacketIn.WriteInt(room.Info.ID);
					WorldMgr.MarryScene.SendToALL(gSPacketIn);
					room.StopTimer();
				}
			}
			catch (Exception exception)
			{
				if (TankMarryLogicProcessor.log.IsErrorEnabled)
				{
					TankMarryLogicProcessor.log.Error("OnTick", exception);
				}
			}
		}
		public override void OnGameData(MarryRoom room, GamePlayer player, GSPacketIn packet)
		{
			MarryCmdType code = (MarryCmdType)packet.ReadByte();
			try
			{
				IMarryCommandHandler marryCommandHandler = this._commandMgr.LoadCommandHandler((int)code);
				if (marryCommandHandler != null)
				{
					marryCommandHandler.HandleCommand(this, player, packet);
				}
				else
				{
					TankMarryLogicProcessor.log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
				}
			}
			catch (Exception ex)
			{
				TankMarryLogicProcessor.log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", ex.ToString(), player.Client.TcpEndpoint));
			}
		}
	}
}
