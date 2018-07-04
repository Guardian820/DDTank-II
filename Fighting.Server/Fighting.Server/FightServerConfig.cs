using System;
using System.Configuration;
using System.Net;
namespace Fighting.Server
{
	public class FightServerConfig
	{
		public string LogConfigFile = "logconfig.xml";
		public IPAddress Ip = IPAddress.Any;
		public int Port = 9208;
		public int ZoneId = 1;
		public void Load()
		{
			this.LogConfigFile = ConfigurationSettings.AppSettings["Logconfig"];
			this.Ip = IPAddress.Parse(ConfigurationSettings.AppSettings["Ip"]);
			this.Port = int.Parse(ConfigurationSettings.AppSettings["Port"]);
			this.ZoneId = int.Parse(ConfigurationSettings.AppSettings["ServerID"]);
		}
	}
}
