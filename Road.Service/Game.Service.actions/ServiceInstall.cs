using System;
using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.Text;
namespace Game.Service.actions
{
	public class ServiceInstall : IAction
	{
		public string Name
		{
			get
			{
				return "--serviceinstall";
			}
		}
		public string Syntax
		{
			get
			{
				return "--serviceinstall";
			}
		}
		public string Description
		{
			get
			{
				return "Installs DOL as system service with he given parameters";
			}
		}
		public void OnAction(Hashtable parameters)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add("/LogToConsole=false");
			StringBuilder stringBuilder = new StringBuilder();
			foreach (DictionaryEntry dictionaryEntry in parameters)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(dictionaryEntry.Key);
				stringBuilder.Append("=");
				stringBuilder.Append(dictionaryEntry.Value);
			}
			arrayList.Add("commandline=" + stringBuilder.ToString());
			string[] commandLine = (string[])arrayList.ToArray(typeof(string));
			AssemblyInstaller assemblyInstaller = new AssemblyInstaller(Assembly.GetExecutingAssembly(), commandLine);
			Hashtable hashtable = new Hashtable();
			if (GameServerService.GetDOLService() != null)
			{
				Console.WriteLine("DOL service is already installed!");
				return;
			}
			Console.WriteLine("Installing Road as system service...");
			try
			{
				assemblyInstaller.Install(hashtable);
				assemblyInstaller.Commit(hashtable);
			}
			catch (Exception ex)
			{
				assemblyInstaller.Rollback(hashtable);
				Console.WriteLine("Error installing as system service");
				Console.WriteLine(ex.Message);
				return;
			}
			Console.WriteLine("Finished!");
		}
	}
}
