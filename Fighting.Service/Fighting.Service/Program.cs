using Fighting.Service.action;
using System;
using System.Collections;
using System.IO;
using System.Threading;
namespace Fighting.Service
{
	internal class Program
	{
		private static System.Collections.ArrayList _actions = new System.Collections.ArrayList();
		[System.MTAThread]
		private static void Main(string[] args)
		{
			System.AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = "." + System.IO.Path.DirectorySeparatorChar + "lib";
			System.Threading.Thread.CurrentThread.Name = "MAIN";
			Program.RegisterActions();
			if (args.Length == 0)
			{
				args = new string[]
				{
					"--start"
				};
			}
			string name;
			System.Collections.Hashtable parameters;
			try
			{
				Program.ParseParameters(args, out name, out parameters);
			}
			catch (System.ArgumentException ex)
			{
				System.Console.WriteLine(ex.Message);
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
			Program.RegisterAction(new ConsoleStart());
		}
		private static void RegisterAction(IAction action)
		{
			if (action == null)
			{
				throw new System.ArgumentException("Action can't be bull", "actioni");
			}
			Program._actions.Add(action);
		}
		public static void ShowSyntax()
		{
			System.Console.WriteLine("Syntax: RoadServer.exe {action} [param1=value1] [param2=value2] ...");
			System.Console.WriteLine("Possible actions:");
			foreach (IAction action in Program._actions)
			{
				if (action.Syntax != null && action.Description != null)
				{
					System.Console.WriteLine(string.Format("{0,-20}\t{1}", action.Syntax, action.Description));
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
		private static void ParseParameters(string[] args, out string actionName, out System.Collections.Hashtable parameters)
		{
			parameters = new System.Collections.Hashtable();
			actionName = null;
			if (!args[0].StartsWith("--"))
			{
				throw new System.ArgumentException("First argument must be the action");
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
					throw new System.ArgumentException("At least two actions given and only one action allowed!");
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
