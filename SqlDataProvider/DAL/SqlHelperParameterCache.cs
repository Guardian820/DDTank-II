using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
namespace DAL
{
	public sealed class SqlHelperParameterCache
	{
		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
		private SqlHelperParameterCache()
		{
		}
		private static SqlParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			SqlParameter[] result;
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				using (SqlCommand sqlCommand = new SqlCommand(spName, sqlConnection))
				{
					sqlConnection.Open();
					sqlCommand.CommandType = CommandType.StoredProcedure;
					SqlCommandBuilder.DeriveParameters(sqlCommand);
					if (!includeReturnValueParameter)
					{
						sqlCommand.Parameters.RemoveAt(0);
					}
					SqlParameter[] array = new SqlParameter[sqlCommand.Parameters.Count];
					sqlCommand.Parameters.CopyTo(array, 0);
					result = array;
				}
			}
			return result;
		}
		private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
		{
			SqlParameter[] array = new SqlParameter[originalParameters.Length];
			int i = 0;
			int num = originalParameters.Length;
			while (i < num)
			{
				array[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
				i++;
			}
			return array;
		}
		public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
		{
			string key = connectionString + ":" + commandText;
			SqlHelperParameterCache.paramCache[key] = commandParameters;
		}
		public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			string key = connectionString + ":" + commandText;
			SqlParameter[] array = (SqlParameter[])SqlHelperParameterCache.paramCache[key];
			if (array == null)
			{
				return null;
			}
			return SqlHelperParameterCache.CloneParameters(array);
		}
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return SqlHelperParameterCache.GetSpParameterSet(connectionString, spName, false);
		}
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			string key = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
			SqlParameter[] array = (SqlParameter[])SqlHelperParameterCache.paramCache[key];
			if (array == null)
			{
				array = (SqlParameter[])(SqlHelperParameterCache.paramCache[key] = SqlHelperParameterCache.DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
			}
			return SqlHelperParameterCache.CloneParameters(array);
		}
	}
}
