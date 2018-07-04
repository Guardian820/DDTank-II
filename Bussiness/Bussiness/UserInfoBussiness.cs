using System;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class UserInfoBussiness : BaseBussiness
	{
		public bool GetFromDbByUid(string uid, ref string userName, ref string portrait)
		{
			SqlDataReader sqlDataReader = null;
			bool result;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@Uid", uid)
				};
				this.db.GetReader(ref sqlDataReader, "SP_User_Info_QueryByUid", sqlParameters);
				while (sqlDataReader.Read())
				{
					userName = ((sqlDataReader["UserName"] == null) ? "" : sqlDataReader["UserName"].ToString());
					portrait = ((sqlDataReader["Portrait"] == null) ? "" : sqlDataReader["Portrait"].ToString());
				}
				if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(portrait))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
				result = false;
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return result;
		}
		public bool AddUserInfo(string uid, string userName, string portrait)
		{
			bool result = false;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@Uid", uid),
					new SqlParameter("@UserName", userName),
					new SqlParameter("@Portrait", portrait),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				array[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_User_Info_Insert", array);
				int num = (int)array[3].Value;
				result = (num == 0);
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			return result;
		}
	}
}
