using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
namespace Game.Logic
{
	public class NpcStatementsMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static List<string> m_npcstatement = new List<string>();
		private static string filePath;
		private static Random random;
		public static bool Init()
		{
			NpcStatementsMgr.filePath = Directory.GetCurrentDirectory() + "\\ai\\npc\\npc_statements.txt";
			NpcStatementsMgr.random = new Random();
			return NpcStatementsMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			bool result;
			try
			{
				string item = string.Empty;
				StreamReader streamReader = new StreamReader(NpcStatementsMgr.filePath, Encoding.Default);
				while (!string.IsNullOrEmpty(item = streamReader.ReadLine()))
				{
					NpcStatementsMgr.m_npcstatement.Add(item);
				}
				result = true;
			}
			catch (Exception exception)
			{
				NpcStatementsMgr.log.Error("NpcStatementsMgr.Reload()", exception);
				result = false;
			}
			return result;
		}
		public static int[] RandomStatementIndexs(int count)
		{
			int[] array = new int[count];
			int i = 0;
			while (i < count)
			{
				int num = NpcStatementsMgr.random.Next(0, NpcStatementsMgr.m_npcstatement.Count);
				if (!array.Contains(num))
				{
					array[i] = num;
					i++;
				}
			}
			return array;
		}
		public static string[] RandomStatement(int count)
		{
			string[] array = new string[count];
			int[] array2 = NpcStatementsMgr.RandomStatementIndexs(count);
			for (int i = 0; i < count; i++)
			{
				int index = array2[i];
				array[i] = NpcStatementsMgr.m_npcstatement[index];
			}
			return array;
		}
		public static string GetStatement(int index)
		{
			if (index < 0 || index > NpcStatementsMgr.m_npcstatement.Count)
			{
				return null;
			}
			return NpcStatementsMgr.m_npcstatement[index];
		}
		public static string GetRandomStatement()
		{
			int index = NpcStatementsMgr.random.Next(0, NpcStatementsMgr.m_npcstatement.Count);
			return NpcStatementsMgr.m_npcstatement[index];
		}
	}
}
