using System;
using System.Collections;
using System.ServiceProcess;
namespace Game.Service.actions
{
	public class ServiceRun : IAction
	{
		public string Name
		{
			get
			{
				return "--SERVICERUN";
			}
		}
		public string Syntax
		{
			get
			{
				return null;
			}
		}
		public string Description
		{
			get
			{
				return null;
			}
		}
		public void OnAction(Hashtable parameters)
		{
			ServiceBase.Run(new GameServerService());
		}
	}
}
