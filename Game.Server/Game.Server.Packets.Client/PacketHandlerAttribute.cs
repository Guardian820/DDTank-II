using System;
namespace Game.Server.Packets.Client
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PacketHandlerAttribute : Attribute
	{
		protected int m_code;
		protected string m_desc;
		public int Code
		{
			get
			{
				return this.m_code;
			}
		}
		public string Description
		{
			get
			{
				return this.m_desc;
			}
		}
		public PacketHandlerAttribute(int code, string desc)
		{
			this.m_code = code;
			this.m_desc = desc;
		}
	}
}
