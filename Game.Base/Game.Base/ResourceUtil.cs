using System;
using System.IO;
using System.Reflection;
namespace Game.Base
{
	public class ResourceUtil
	{
		public static Stream GetResourceStream(string fileName, Assembly assem)
		{
			fileName = fileName.ToLower();
			string[] manifestResourceNames = assem.GetManifestResourceNames();
			for (int i = 0; i < manifestResourceNames.Length; i++)
			{
				string text = manifestResourceNames[i];
				if (text.ToLower().EndsWith(fileName))
				{
					return assem.GetManifestResourceStream(text);
				}
			}
			return null;
		}
		public static void ExtractResource(string fileName, Assembly assembly)
		{
			ResourceUtil.ExtractResource(fileName, fileName, assembly);
		}
		public static void ExtractResource(string resourceName, string fileName, Assembly assembly)
		{
			FileInfo fileInfo = new FileInfo(fileName);
			if (!fileInfo.Directory.Exists)
			{
				fileInfo.Directory.Create();
			}
			using (StreamReader streamReader = new StreamReader(ResourceUtil.GetResourceStream(resourceName, assembly)))
			{
				using (StreamWriter streamWriter = new StreamWriter(File.Create(fileName)))
				{
					streamWriter.Write(streamReader.ReadToEnd());
				}
			}
		}
	}
}
