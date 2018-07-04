using System;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class OrderBussiness : BaseBussiness
	{
		public bool AddOrder(string order, double amount, string username, string payWay, string serverId)
		{
			bool result = false;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@Order", order),
					new SqlParameter("@Amount", amount),
					new SqlParameter("@Username", username),
					new SqlParameter("@PayWay", payWay),
					new SqlParameter("@ServerId", serverId),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				array[5].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Charge_Order", array);
				int num = (int)array[5].Value;
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
		public string GetOrderToName(string order, ref string serverId)
		{
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@Order", order)
				};
				this.db.GetReader(ref sqlDataReader, "SP_Charge_Order_Single", sqlParameters);
				if (sqlDataReader.Read())
				{
					serverId = ((sqlDataReader["ServerId"] == null) ? "" : sqlDataReader["ServerId"].ToString());
					return (sqlDataReader["UserName"] == null) ? "" : sqlDataReader["UserName"].ToString();
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return "";
		}
	}
}
