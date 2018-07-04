using Bussiness.Protocol;
using Center.Server;
using Game.Base;
using log4net;
using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
namespace Game.Service.actions
{
	public class ConsoleStart : IAction
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public string HelpStr
		{
			get
			{
				return ConfigurationManager.AppSettings["HelpStr"];
			}
		}
		public string Name
		{
			get
			{
				return "--start";
			}
		}
		public string Syntax
		{
			get
			{
				return "--start [-config=./config/serverconfig.xml]";
			}
		}
		public string Description
		{
			get
			{
				return "Starts the DOL server in console mode";
			}
		}
		private static bool StartServer()
		{
			System.Console.WriteLine("Starting the server");
			return CenterServer.Instance.Start();
		}
		public void OnAction(System.Collections.Hashtable parameters)
		{
			System.Console.WriteLine("This server GunnyII, edit and build by MrPhuong!");
			System.Console.WriteLine("Starting GameServer ... please wait a moment!");
			CenterServer.CreateInstance(new CenterServerConfig());
			ConsoleStart.StartServer();
			ConsoleClient client = new ConsoleClient();
			bool flag = true;
			while (flag)
			{
				try
				{
					System.Console.Write("> ");
					string text = System.Console.ReadLine();
					string[] array = text.Split(new char[]
					{
						'&'
					});
					string key;
					switch (key = array[0].ToLower())
					{
					case "exit":
						flag = false;
						continue;

					case "notice":
						if (array.Length < 2)
						{
							System.Console.WriteLine("公告需要公告内容,用&隔开!");
							continue;
						}
						CenterServer.Instance.SendSystemNotice(array[1]);
						continue;

					case "reload":
						if (array.Length < 2)
						{
							System.Console.WriteLine("加载需要指定表,用&隔开!");
							continue;
						}
						CenterServer.Instance.SendReload(array[1]);
						continue;

					case "shutdown":
						CenterServer.Instance.SendShutdown();
						continue;

					case "help":
						System.Console.WriteLine(this.HelpStr);
						continue;

					case "AAS":
						if (array.Length < 2)
						{
							System.Console.WriteLine("加载需要指定状态true or false,用&隔开!");
							continue;
						}
						CenterServer.Instance.SendAAS(bool.Parse(array[1]));
						continue;
					}
					if (text.Length > 0)
					{
						if (text[0] == '/')
						{
							text = text.Remove(0, 1);
							text = text.Insert(0, "&");
						}
						try
						{
							if (!CommandMgr.HandleCommandNoPlvl(client, text))
							{
								System.Console.WriteLine("Unknown command: " + text);
							}
						}
						catch (System.Exception ex)
						{
							System.Console.WriteLine(ex.ToString());
						}
					}
				}
				catch (System.Exception ex2)
				{
					System.Console.WriteLine("Error:" + ex2.ToString());
				}
			}
			if (CenterServer.Instance != null)
			{
				CenterServer.Instance.Stop();
			}
		}
		public void Reload(eReloadType type)
		{
		}
	}
}
