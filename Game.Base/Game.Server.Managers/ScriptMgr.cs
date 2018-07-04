using log4net;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Server.Managers
{
	public class ScriptMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static readonly Dictionary<string, Assembly> m_scripts = new Dictionary<string, Assembly>();
		public static Assembly[] Scripts
		{
			get
			{
				Dictionary<string, Assembly> scripts;
				Monitor.Enter(scripts = ScriptMgr.m_scripts);
				Assembly[] result;
				try
				{
					result = ScriptMgr.m_scripts.Values.ToArray<Assembly>();
				}
				finally
				{
					Monitor.Exit(scripts);
				}
				return result;
			}
		}
		public static bool InsertAssembly(Assembly ass)
		{
			Dictionary<string, Assembly> scripts;
			Monitor.Enter(scripts = ScriptMgr.m_scripts);
			bool result;
			try
			{
				if (!ScriptMgr.m_scripts.ContainsKey(ass.FullName))
				{
					ScriptMgr.m_scripts.Add(ass.FullName, ass);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				Monitor.Exit(scripts);
			}
			return result;
		}
		public static bool RemoveAssembly(Assembly ass)
		{
			Dictionary<string, Assembly> scripts;
			Monitor.Enter(scripts = ScriptMgr.m_scripts);
			bool result;
			try
			{
				result = ScriptMgr.m_scripts.Remove(ass.FullName);
			}
			finally
			{
				Monitor.Exit(scripts);
			}
			return result;
		}
		public static bool CompileScripts(bool compileVB, string path, string dllName, string[] asm_names)
		{
            if (!path.EndsWith("\\") && !path.EndsWith("/"))
			{
				path += "/";
			}
			ArrayList arrayList = ScriptMgr.ParseDirectory(new DirectoryInfo(path), compileVB ? "*.vb" : "*.cs", true);
			if (arrayList.Count == 0)
			{
				return true;
			}
			if (File.Exists(dllName))
			{
				File.Delete(dllName);
			}
			CompilerResults compilerResults = null;
			try
			{
				CodeDomProvider codeDomProvider;
				if (compileVB)
				{
					codeDomProvider = new VBCodeProvider();
				}
				else
				{
					codeDomProvider = new CSharpCodeProvider();
				}
				CompilerParameters compilerParameters = new CompilerParameters(asm_names, dllName, true);
				compilerParameters.GenerateExecutable = false;
				compilerParameters.GenerateInMemory = false;
				compilerParameters.WarningLevel = 2;
				compilerParameters.CompilerOptions = "/lib:.";
				string[] array = new string[arrayList.Count];
				for (int i = 0; i < arrayList.Count; i++)
				{
					array[i] = ((FileInfo)arrayList[i]).FullName;
				}
				compilerResults = codeDomProvider.CompileAssemblyFromFile(compilerParameters, array);
				GC.Collect();
				if (compilerResults.Errors.HasErrors)
				{
					foreach (CompilerError compilerError in compilerResults.Errors)
					{
						if (!compilerError.IsWarning)
						{
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.Append("   ");
							stringBuilder.Append(compilerError.FileName);
							stringBuilder.Append(" Line:");
							stringBuilder.Append(compilerError.Line);
							stringBuilder.Append(" Col:");
							stringBuilder.Append(compilerError.Column);
							if (ScriptMgr.log.IsErrorEnabled)
							{
								ScriptMgr.log.Error("Script compilation failed because: ");
								ScriptMgr.log.Error(compilerError.ErrorText);
								ScriptMgr.log.Error(stringBuilder.ToString());
							}
						}
					}
					bool result = false;
					return result;
				}
			}
			catch (Exception exception)
			{
				if (ScriptMgr.log.IsErrorEnabled)
				{
					ScriptMgr.log.Error("CompileScripts", exception);
				}
			}
			if (compilerResults != null && !compilerResults.Errors.HasErrors)
			{
				ScriptMgr.InsertAssembly(compilerResults.CompiledAssembly);
				return true;
			}
			return true;
		}
		private static ArrayList ParseDirectory(DirectoryInfo path, string filter, bool deep)
		{
			ArrayList arrayList = new ArrayList();
			if (!path.Exists)
			{
				return arrayList;
			}
			arrayList.AddRange(path.GetFiles(filter));
			if (deep)
			{
				DirectoryInfo[] directories = path.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					DirectoryInfo path2 = directories[i];
					arrayList.AddRange(ScriptMgr.ParseDirectory(path2, filter, deep));
				}
			}
			return arrayList;
		}
		public static Type GetType(string name)
		{
			Assembly[] scripts = ScriptMgr.Scripts;
			for (int i = 0; i < scripts.Length; i++)
			{
				Assembly assembly = scripts[i];
				Type type = assembly.GetType(name);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}
		public static object CreateInstance(string name)
		{
			Assembly[] scripts = ScriptMgr.Scripts;
			for (int i = 0; i < scripts.Length; i++)
			{
				Assembly assembly = scripts[i];
				Type type = assembly.GetType(name);
				if (type != null && type.IsClass)
				{
					return Activator.CreateInstance(type);
				}
			}
			return null;
		}
		public static object CreateInstance(string name, Type baseType)
		{
			Assembly[] scripts = ScriptMgr.Scripts;
			for (int i = 0; i < scripts.Length; i++)
			{
				Assembly assembly = scripts[i];
				Type type = assembly.GetType(name);
				if (type != null && type.IsClass && baseType.IsAssignableFrom(type))
				{
					return Activator.CreateInstance(type);
				}
			}
			return null;
		}
		public static Type[] GetDerivedClasses(Type baseType)
		{
			if (baseType == null)
			{
				return new Type[0];
			}
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList(ScriptMgr.Scripts);
			foreach (Assembly assembly in arrayList2)
			{
				Type[] types = assembly.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					Type type = types[i];
					if (type.IsClass && baseType.IsAssignableFrom(type))
					{
						arrayList.Add(type);
					}
				}
			}
			return (Type[])arrayList.ToArray(typeof(Type));
		}
		public static Type[] GetImplementedClasses(string baseInterface)
		{
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList(ScriptMgr.Scripts);
			foreach (Assembly assembly in arrayList2)
			{
				Type[] types = assembly.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					Type type = types[i];
					if (type.IsClass && type.GetInterface(baseInterface) != null)
					{
						arrayList.Add(type);
					}
				}
			}
			return (Type[])arrayList.ToArray(typeof(Type));
		}
	}
}
