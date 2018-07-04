using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
namespace Game.Service
{
	[RunInstaller(true)]
	public class GameServerServiceInstaller : Installer
	{
		private ServiceInstaller m_gameServerServiceInstaller;
		private ServiceProcessInstaller m_gameServerServiceProcessInstaller;
		public GameServerServiceInstaller()
		{
			this.m_gameServerServiceProcessInstaller = new ServiceProcessInstaller();
			this.m_gameServerServiceProcessInstaller.Account = ServiceAccount.LocalSystem;
			this.m_gameServerServiceInstaller = new ServiceInstaller();
			this.m_gameServerServiceInstaller.StartType = ServiceStartMode.Manual;
			this.m_gameServerServiceInstaller.ServiceName = "ROAD";
			base.Installers.Add(this.m_gameServerServiceProcessInstaller);
			base.Installers.Add(this.m_gameServerServiceInstaller);
		}
		public override void Install(IDictionary stateSaver)
		{
			StringDictionary parameters;
			(parameters = base.Context.Parameters)["assemblyPath"] = parameters["assemblyPath"] + " --SERVICERUN " + base.Context.Parameters["commandline"];
			base.Install(stateSaver);
		}
	}
}
