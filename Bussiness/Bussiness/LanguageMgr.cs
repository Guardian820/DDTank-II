using log4net;
using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Bussiness
{
	public class LanguageMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Hashtable LangsSentences = new Hashtable();
		private static string LanguageFile
		{
			get
			{
				return ConfigurationManager.AppSettings["LanguagePath"];
			}
		}
		public static bool Setup(string path)
		{
			return LanguageMgr.Reload(path);
		}
		public static bool Reload(string path)
		{
			try
			{
				Hashtable hashtable = LanguageMgr.LoadLanguage(path);
				if (hashtable.Count > 0)
				{
					Interlocked.Exchange<Hashtable>(ref LanguageMgr.LangsSentences, hashtable);
					return true;
				}
			}
			catch (Exception exception)
			{
				LanguageMgr.log.Error("Load language file error:", exception);
			}
			return false;
		}
		private static Hashtable LoadLanguage(string path)
		{
			Hashtable hashtable = new Hashtable();
			string text = path + LanguageMgr.LanguageFile;
			if (!File.Exists(text))
			{
				LanguageMgr.log.Error("Language file : " + text + " not found !");
			}
			else
			{
				string[] c = File.ReadAllLines(text, Encoding.UTF8);
				IList list = new ArrayList(c);
				foreach (string text2 in list)
				{
					if (!text2.StartsWith("#") && text2.IndexOf(':') != -1)
					{
						string[] array = new string[]
						{
							text2.Substring(0, text2.IndexOf(':')),
							text2.Substring(text2.IndexOf(':') + 1)
						};
						array[1] = array[1].Replace("\t", "");
						hashtable[array[0]] = array[1];
					}
				}
			}
			return hashtable;
		}
		public static string GetTranslation(string translateId, params object[] args)
		{
			if (!LanguageMgr.LangsSentences.ContainsKey(translateId))
			{
				return translateId;
			}
			string text = (string)LanguageMgr.LangsSentences[translateId];
			try
			{
				text = string.Format(text, args);
			}
			catch (Exception exception)
			{
				LanguageMgr.log.Error(string.Concat(new object[]
				{
					"Parameters number error, ID: ",
					translateId,
					" (Arg count=",
					args.Length,
					")"
				}), exception);
			}
			if (text != null)
			{
				return text;
			}
			return translateId;
		}
	}
}
