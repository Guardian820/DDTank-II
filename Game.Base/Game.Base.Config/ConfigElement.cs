using System;
using System.Collections;
using System.Threading;
namespace Game.Base.Config
{
	public class ConfigElement
	{
		protected ConfigElement m_parent;
		protected Hashtable m_children = new Hashtable();
		protected string m_value;
		public ConfigElement this[string key]
		{
			get
			{
				Hashtable children;
				Monitor.Enter(children = this.m_children);
				try
				{
					if (!this.m_children.Contains(key))
					{
						this.m_children.Add(key, this.GetNewConfigElement(this));
					}
				}
				finally
				{
					Monitor.Exit(children);
				}
				return (ConfigElement)this.m_children[key];
			}
			set
			{
				Hashtable children;
				Monitor.Enter(children = this.m_children);
				try
				{
					this.m_children[key] = value;
				}
				finally
				{
					Monitor.Exit(children);
				}
			}
		}
		public ConfigElement Parent
		{
			get
			{
				return this.m_parent;
			}
		}
		public bool HasChildren
		{
			get
			{
				return this.m_children.Count > 0;
			}
		}
		public Hashtable Children
		{
			get
			{
				return this.m_children;
			}
		}
		public ConfigElement(ConfigElement parent)
		{
			this.m_parent = parent;
		}
		protected virtual ConfigElement GetNewConfigElement(ConfigElement parent)
		{
			return new ConfigElement(parent);
		}
		public string GetString()
		{
			return this.m_value;
		}
		public string GetString(string defaultValue)
		{
			if (this.m_value == null)
			{
				return defaultValue;
			}
			return this.m_value;
		}
		public int GetInt()
		{
			return int.Parse(this.m_value);
		}
		public int GetInt(int defaultValue)
		{
			if (this.m_value == null)
			{
				return defaultValue;
			}
			return int.Parse(this.m_value);
		}
		public long GetLong()
		{
			return long.Parse(this.m_value);
		}
		public long GetLong(long defaultValue)
		{
			if (this.m_value == null)
			{
				return defaultValue;
			}
			return long.Parse(this.m_value);
		}
		public bool GetBoolean()
		{
			return bool.Parse(this.m_value);
		}
		public bool GetBoolean(bool defaultValue)
		{
			if (this.m_value == null)
			{
				return defaultValue;
			}
			return bool.Parse(this.m_value);
		}
		public void Set(object value)
		{
			this.m_value = value.ToString();
		}
	}
}
