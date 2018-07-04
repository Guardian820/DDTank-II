using log4net;
using SqlDataProvider.BaseClass;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
namespace Bussiness
{
	public class MemberShipBussiness : IDisposable
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected Sql_DbObject db;
		public MemberShipBussiness()
		{
			this.db = new Sql_DbObject("AppConfig", "membershipDb");
		}
		public bool CheckUsername(string applicationname, string username, string password)
		{
			SqlParameter[] array = new SqlParameter[]
			{
				new SqlParameter("@ApplicationName", applicationname),
				new SqlParameter("@UserName", username),
				new SqlParameter("@password", password),
				new SqlParameter("@UserId", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.Output;
			this.db.RunProcedure("Mem_Users_Accede", array);
			int num = 0;
			int.TryParse(array[3].Value.ToString(), out num);
			return num > 0;
		}
		public bool CreateUsername(string applicationname, string username, string password, string email, string passwordformat, string passwordsalt, bool usersex)
		{
			SqlParameter[] array = new SqlParameter[]
			{
				new SqlParameter("@ApplicationName", applicationname),
				new SqlParameter("@UserName", username),
				new SqlParameter("@password", password),
				new SqlParameter("@email", email),
				new SqlParameter("@PasswordFormat", passwordformat),
				new SqlParameter("@PasswordSalt", passwordsalt),
				new SqlParameter("@UserSex", usersex),
				new SqlParameter("@UserId", SqlDbType.Int)
			};
			array[7].Direction = ParameterDirection.Output;
			bool flag = this.db.RunProcedure("Mem_Users_CreateUser", array);
			if (flag)
			{
				flag = ((int)array[7].Value > 0);
			}
			return flag;
		}
		public void Dispose()
		{
			this.db.Dispose();
			GC.SuppressFinalize(this);
		}
		public bool ExistsUsername(string username)
		{
			SqlParameter[] array = new SqlParameter[]
			{
				new SqlParameter("@UserName", username),
				new SqlParameter("@UserCOUNT", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.Output;
			this.db.RunProcedure("Mem_UserInfo_SearchName", array);
			return (int)array[1].Value > 0;
		}
	}
}
