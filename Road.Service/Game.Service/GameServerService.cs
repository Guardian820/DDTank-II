using Game.Server;
using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
namespace Game.Service
{
	public class GameServerService : ServiceBase
	{
		public GameServerService()
		{
			base.ServiceName = "ROAD";
			base.AutoLog = false;
			base.CanHandlePowerEvent = false;
			base.CanPauseAndContinue = false;
			base.CanShutdown = true;
			base.CanStop = true;
		}
		private static bool StartServer()
		{
			FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
			Directory.SetCurrentDirectory(fileInfo.DirectoryName);
			new FileInfo("./config/serverconfig.xml");
			GameServerConfig config = new GameServerConfig();
			GameServer.CreateInstance(config);
			return GameServer.Instance.Start();
		}
		private static void StopServer()
		{
			GameServer.Instance.Stop();
		}
		protected override void OnStart(string[] args)
		{
			if (!GameServerService.StartServer())
			{
				throw new ApplicationException("Failed to start server!");
			}
		}
		protected override void OnStop()
		{
			GameServerService.StopServer();
		}
		protected override void OnShutdown()
		{
			GameServerService.StopServer();
		}
		public static ServiceController GetDOLService()
		{
			ServiceController[] services = ServiceController.GetServices();
			for (int i = 0; i < services.Length; i++)
			{
				ServiceController serviceController = services[i];
				if (serviceController.ServiceName.ToLower().Equals("ROAD"))
				{
					return serviceController;
				}
			}
			return null;
		}
	}
}
