using System;
using System.Collections;
using System.ServiceProcess;
namespace Game.Service.actions
{
	public class ServiceStart : IAction
	{
		public string Name
		{
			get
			{
				return "--servicestart";
			}
		}
		public string Syntax
		{
			get
			{
				return "--servicestart";
			}
		}
		public string Description
		{
			get
			{
				return "Starts the DOL system service";
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
			if (dOLService.Status != ServiceControllerStatus.Stopped)
			{
				Console.WriteLine("The DOL service is not stopped");
				return;
			}
			try
			{
				Console.WriteLine("Starting the DOL service...");
				dOLService.Start();
				dOLService.WaitForStatus(ServiceControllerStatus.StartPending, TimeSpan.FromSeconds(10.0));
				Console.WriteLine("Starting can take some time, please check the logfile for progress information!");
				Console.WriteLine("Finished!");
			}
			catch (InvalidOperationException ex)
			{
				Console.WriteLine("Could not start the DOL service!");
				Console.WriteLine(ex.Message);
			}
			catch (System.ServiceProcess.TimeoutException)
			{
				Console.WriteLine("Error starting the service, please check the logfile for further info!");
			}
		}
	}
}
