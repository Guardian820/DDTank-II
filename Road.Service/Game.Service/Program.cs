using Game.Service.actions;
using System;
using System.Collections;
using System.IO;
using System.Threading;
namespace Game.Service
{
	internal class Program
	{
		private static ArrayList _actions = new ArrayList();
		[MTAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = "." + Path.DirectorySeparatorChar + "lib";
			Thread.CurrentThread.Name = "MAIN";
			Program.RegisterActions();
			if (args.Length == 0)
			{
				args = new string[]
				{
					"--start"
				};
			}
			string name;
			Hashtable parameters;
			try
			{
				Program.ParseParameters(args, out name, out parameters);
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine(ex.Message);
				return;
			}
			IAction action = Program.GetAction(name);
			if (action != null)
			{
				action.OnAction(parameters);
				return;
			}
			Program.ShowSyntax();
		}
		private static void RegisterActions()
		{
			Program.RegisterAction(new ServiceRun());
			Program.RegisterAction(new ConsoleStart());
			Program.RegisterAction(new ServiceInstall());
			Program.RegisterAction(new ServiceUninstall());
			Program.RegisterAction(new ServiceStart());
			Program.RegisterAction(new ServiceStop());
		}
		private static void RegisterAction(IAction action)
		{
			if (action == null)
			{
				throw new ArgumentException("Action can't be bull", "actioni");
			}
			Program._actions.Add(action);
		}
		public static void ShowSyntax()
		{
			Console.WriteLine("Syntax: RoadServer.exe {action} [param1=value1] [param2=value2] ...");
			Console.WriteLine("Possible actions:");
			foreach (IAction action in Program._actions)
			{
				if (action.Syntax != null && action.Description != null)
				{
					Console.WriteLine(string.Format("{0,-20}\t{1}", action.Syntax, action.Description));
				}
			}
		}
		private static IAction GetAction(string name)
		{
			foreach (IAction action in Program._actions)
			{
				if (action.Name.Equals(name))
				{
					return action;
				}
			}
			return null;
		}
		private static void ParseParameters(string[] args, out string actionName, out Hashtable parameters)
		{
			parameters = new Hashtable();
			actionName = null;
			if (!args[0].StartsWith("--"))
			{
				throw new ArgumentException("First argument must be the action");
			}
			actionName = args[0];
			if (args.Length == 1)
			{
				return;
			}
			for (int i = 1; i < args.Length; i++)
			{
				string text = args[i];
				if (text.StartsWith("--"))
				{
					throw new ArgumentException("At least two actions given and only one action allowed!");
				}
				if (text.StartsWith("-"))
				{
					int num = text.IndexOf('=');
					if (num == -1)
					{
						parameters.Add(text, "");
					}
					else
					{
						string key = text.Substring(0, num);
						string value = "";
						if (num + 1 < text.Length)
						{
							value = text.Substring(num + 1);
						}
						parameters.Add(key, value);
					}
				}
			}
		}
	}
}
