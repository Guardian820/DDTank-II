using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
namespace Bussiness.WebLogin
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Advanced), DebuggerStepThrough, DataContract(Namespace = "dandantang")]
	public class Get_UserSexResponseBody
	{
		[DataMember(Order = 0)]
		public bool? Get_UserSexResult;
		public Get_UserSexResponseBody()
		{
		}
		public Get_UserSexResponseBody(bool? Get_UserSexResult)
		{
			this.Get_UserSexResult = Get_UserSexResult;
		}
	}
}
