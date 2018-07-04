using System;
using System.Collections;
using System.ServiceProcess;
namespace Game.Service.actions
{
	public class ServiceStop : IAction
	{
		public string Name
		{
			get
			{
				return "--servicestop";
			}
		}
		public string Syntax
		{
			get
			{
				return "--servicestop";
			}
		}
		public string Description
		{
			get
			{
				return "Stops the DOL system service";
			}
		}
		public void OnAction(Hashtable parameters)
		{
			ServiceController dOLService = GameServerService.GetDOLService();
			if (dOLService == null)
			{
				Console.WriteLine("You have to install the service first!");
				return;
			}
			if (dOLService.Status == ServiceControllerStatus.StartPending)
			{
				Console.WriteLine("Server is still starting, please check the logfile for progress information!");
				return;
			}
			if (dOLService.Status != ServiceControllerStatus.Running)
			{
				Console.WriteLine("The DOL service is not running");
				return;
			}
			try
			{
				Console.WriteLine("Stopping the DOL service...");
				dOLService.Stop();
				dOLService.WaitForStatus(ServiceControllerStatus.Stopped);
				Console.WriteLine("Finished!");
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine("Could not stop the DOL service!");
				Console.WriteLine(ex.Message);
			}
		}
	}
}
