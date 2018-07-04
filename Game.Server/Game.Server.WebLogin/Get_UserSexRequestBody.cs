using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
namespace Game.Server.WebLogin
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Advanced), DebuggerStepThrough, DataContract(Namespace = "dandantang")]
	public class Get_UserSexRequestBody
	{
		[DataMember(EmitDefaultValue = false, Order = 0)]
		public string applicationname;
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string username;
		public Get_UserSexRequestBody()
		{
		}
		public Get_UserSexRequestBody(string applicationname, string username)
		{
			this.applicationname = applicationname;
			this.username = username;
		}
	}
}
