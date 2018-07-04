using System;
using System.Collections;
using System.Configuration.Install;
using System.Reflection;
namespace Game.Service.actions
{
	public class ServiceUninstall : IAction
	{
		public string Name
		{
			get
			{
				return "--serviceuninstall";
			}
		}
		public string Syntax
		{
			get
			{
				return "--serviceuninstall";
			}
		}
		public string Description
		{
			get
			{
				return "Uninstalls the DOL system service";
			}
		}
		public void OnAction(Hashtable parameters)
		{
			AssemblyInstaller assemblyInstaller = new AssemblyInstaller(Assembly.GetExecutingAssembly(), new string[]
			{
				"/LogToConsole=false"
			});
			Hashtable savedState = new Hashtable();
			if (GameServerService.GetDOLService() == null)
			{
				Console.WriteLine("No service named \"DOL\" found!");
				return;
			}
			Console.WriteLine("Uninstalling DOL system service...");
			try
			{
				assemblyInstaller.Uninstall(savedState);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error uninstalling system service");
				Console.WriteLine(ex.Message);
				return;
			}
			Console.WriteLine("Finished!");
		}
	}
}
