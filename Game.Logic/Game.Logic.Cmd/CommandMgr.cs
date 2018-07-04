using Game.Base.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Logic.Cmd
{
	public class CommandMgr
	{
		private static Dictionary<int, ICommandHandler> handles = new Dictionary<int, ICommandHandler>();
		public static ICommandHandler LoadCommandHandler(int code)
		{
			return CommandMgr.handles[code];
		}
		[ScriptLoadedEvent]
		public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
		{
			CommandMgr.handles.Clear();
			CommandMgr.SearchCommandHandlers(Assembly.GetAssembly(typeof(BaseGame)));
		}
		protected static int SearchCommandHandlers(Assembly assembly)
		{
			int num = 0;
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsClass && type.GetInterface("Game.Logic.Cmd.ICommandHandler") != null)
				{
					GameCommandAttribute[] array = (GameCommandAttribute[])type.GetCustomAttributes(typeof(GameCommandAttribute), true);
					if (array.Length > 0)
					{
						num++;
						CommandMgr.RegisterCommandHandler(array[0].Code, Activator.CreateInstance(type) as ICommandHandler);
					}
				}
			}
			return num;
		}
		protected static void RegisterCommandHandler(int code, ICommandHandler handle)
		{
			CommandMgr.handles.Add(code, handle);
		}
	}
}
