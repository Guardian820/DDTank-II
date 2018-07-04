using log4net;
using System;
using System.Configuration;
using System.Reflection;
namespace Game.Base.Config
{
	public abstract class BaseAppConfig
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public BaseAppConfig()
		{
		}
		protected virtual void Load(Type type)
		{
			ConfigurationManager.RefreshSection("appSettings");
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
				if (customAttributes.Length != 0)
				{
					ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)customAttributes[0];
					fieldInfo.SetValue(this, this.LoadConfigProperty(attrib));
				}
			}
		}
		private object LoadConfigProperty(ConfigPropertyAttribute attrib)
		{
			string key = attrib.Key;
			string text = ConfigurationManager.AppSettings[key];
			if (text == null)
			{
				text = attrib.DefaultValue.ToString();
				BaseAppConfig.log.Warn("Loading " + key + " value is null,using default vaule:" + text);
			}
			else
			{
				BaseAppConfig.log.Debug("Loading " + key + " Value is " + text);
			}
			object result;
			try
			{
				result = Convert.ChangeType(text, attrib.DefaultValue.GetType());
			}
			catch (Exception exception)
			{
				BaseAppConfig.log.Error("Exception in ServerProperties Load: ", exception);
				result = null;
			}
			return result;
		}
	}
}
