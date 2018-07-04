using log4net;
using SqlDataProvider.BaseClass;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
namespace Bussiness
{
	public class BaseBussiness : IDisposable
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected Sql_DbObject db;
		public BaseBussiness()
		{
			this.db = new Sql_DbObject("AppConfig", "conString");
		}
		public DataTable GetPage(string queryStr, string queryWhere, int pageCurrent, int pageSize, string fdShow, string fdOreder, string fdKey, ref int total)
		{
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@QueryStr", queryStr),
					new SqlParameter("@QueryWhere", queryWhere),
					new SqlParameter("@PageSize", pageSize),
					new SqlParameter("@PageCurrent", pageCurrent),
					new SqlParameter("@FdShow", fdShow),
					new SqlParameter("@FdOrder", fdOreder),
					new SqlParameter("@FdKey", fdKey),
					new SqlParameter("@TotalRow", total)
				};
				array[7].Direction = ParameterDirection.Output;
				DataTable dataTable = this.db.GetDataTable(queryStr, "SP_CustomPage", array, 120);
				total = (int)array[7].Value;
				return dataTable;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", exception);
				}
			}
			return new DataTable(queryStr);
		}
		public void Dispose()
		{
			this.db.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
