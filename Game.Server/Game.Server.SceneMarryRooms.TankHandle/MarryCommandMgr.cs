using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	public class MarryCommandMgr
	{
		private Dictionary<int, IMarryCommandHandler> handles = new Dictionary<int, IMarryCommandHandler>();
		public IMarryCommandHandler LoadCommandHandler(int code)
		{
			return this.handles[code];
		}
		public MarryCommandMgr()
		{
			this.handles.Clear();
			this.SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
		}
		protected int SearchCommandHandlers(Assembly assembly)
		{
			int num = 0;
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsClass && type.GetInterface("Game.Server.SceneMarryRooms.TankHandle.IMarryCommandHandler") != null)
				{
					MarryCommandAttbute[] array = (MarryCommandAttbute[])type.GetCustomAttributes(typeof(MarryCommandAttbute), true);
					if (array.Length > 0)
					{
						num++;
						this.RegisterCommandHandler((int)array[0].Code, Activator.CreateInstance(type) as IMarryCommandHandler);
					}
				}
			}
			return num;
		}
		protected void RegisterCommandHandler(int code, IMarryCommandHandler handle)
		{
			this.handles.Add(code, handle);
		}
	}
}
