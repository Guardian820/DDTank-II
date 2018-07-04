using Bussiness.WebLogin;
using System;
namespace Bussiness.Interface
{
	public class SRInterface : BaseInterface
	{
		public override bool GetUserSex(string name)
		{
			bool result;
			try
			{
				PassPortSoapClient passPortSoapClient = new PassPortSoapClient();
				result = passPortSoapClient.Get_UserSex(string.Empty, name).Value;
			}
			catch (Exception exception)
			{
				BaseInterface.log.Error("获取性别失败", exception);
				result = true;
			}
			return result;
		}
	}
}
