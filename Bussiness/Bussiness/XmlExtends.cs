using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;
namespace Bussiness
{
	public static class XmlExtends
	{
		public static string ToString(this XElement node, bool check)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings
			{
				CheckCharacters = check,
				OmitXmlDeclaration = true,
				Indent = true
			}))
			{
				node.WriteTo(xmlWriter);
			}
			return stringBuilder.ToString();
		}
	}
}
