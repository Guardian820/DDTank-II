using Game.Base.Events;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Logic.Spells
{
	public class SpellMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, ISpellHandler> handles = new Dictionary<int, ISpellHandler>();
		public static ISpellHandler LoadSpellHandler(int code)
		{
			return SpellMgr.handles[code];
		}
		[ScriptLoadedEvent]
		public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
		{
			SpellMgr.handles.Clear();
			int num = SpellMgr.SearchSpellHandlers(Assembly.GetAssembly(typeof(BaseGame)));
			if (SpellMgr.log.IsInfoEnabled)
			{
				SpellMgr.log.Info("SpellMgr: Loaded " + num + " spell handlers from GameServer Assembly!");
			}
		}
		protected static int SearchSpellHandlers(Assembly assembly)
		{
			int num = 0;
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsClass && type.GetInterface("Game.Logic.Spells.ISpellHandler") != null)
				{
					SpellAttibute[] array = (SpellAttibute[])type.GetCustomAttributes(typeof(SpellAttibute), true);
					if (array.Length > 0)
					{
						num++;
						SpellMgr.RegisterSpellHandler(array[0].Type, Activator.CreateInstance(type) as ISpellHandler);
					}
				}
			}
			return num;
		}
		protected static void RegisterSpellHandler(int type, ISpellHandler handle)
		{
			SpellMgr.handles.Add(type, handle);
		}
		public static void ExecuteSpell(BaseGame game, Player player, ItemTemplateInfo item)
		{
			try
			{
				ISpellHandler spellHandler = SpellMgr.LoadSpellHandler(item.Property1);
				spellHandler.Execute(game, player, item);
			}
			catch (Exception exception)
			{
				SpellMgr.log.Error("Execute Spell Error:", exception);
			}
		}
	}
}
