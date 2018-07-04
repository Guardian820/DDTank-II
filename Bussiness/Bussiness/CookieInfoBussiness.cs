using System;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class CookieInfoBussiness : BaseBussiness
	{
		public bool GetFromDbByUser(string bdSigUser, ref string bdSigPortrait, ref string bdSigSessionKey)
		{
			SqlDataReader sqlDataReader = null;
			bool result;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@BdSigUser", bdSigUser)
				};
				this.db.GetReader(ref sqlDataReader, "SP_Cookie_Info_QueryByUser", sqlParameters);
				while (sqlDataReader.Read())
				{
					bdSigPortrait = ((sqlDataReader["BdSigPortrait"] == null) ? "" : sqlDataReader["BdSigPortrait"].ToString());
					bdSigSessionKey = ((sqlDataReader["BdSigSessionKey"] == null) ? "" : sqlDataReader["BdSigSessionKey"].ToString());
				}
				if (!string.IsNullOrEmpty(bdSigPortrait) && !string.IsNullOrEmpty(bdSigSessionKey))
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
		public bool AddCookieInfo(string bdSigUser, string bdSigPortrait, string bdSigSessionKey)
		{
			bool result = false;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@BdSigUser", bdSigUser),
					new SqlParameter("@BdSigPortrait", bdSigPortrait),
					new SqlParameter("@BdSigSessionKey", bdSigSessionKey),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				array[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Cookie_Info_Insert", array);
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
