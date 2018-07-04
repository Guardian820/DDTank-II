using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
namespace Game.Server.WebLogin
{
	[GeneratedCode("System.ServiceModel", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Advanced), DebuggerStepThrough, DataContract(Namespace = "dandantang")]
	public class ChenckValidateResponseBody
	{
		[DataMember(EmitDefaultValue = false, Order = 0)]
		public string ChenckValidateResult;
		public ChenckValidateResponseBody()
		{
		}
		public ChenckValidateResponseBody(string ChenckValidateResult)
		{
			this.ChenckValidateResult = ChenckValidateResult;
		}
	}
}
