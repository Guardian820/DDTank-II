using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
namespace DAL
{
	public sealed class SqlHelper
	{
		private enum SqlConnectionOwnership
		{
			Internal,
			External
		}
		private SqlHelper()
		{
		}
		private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
		{
			for (int i = 0; i < commandParameters.Length; i++)
			{
				SqlParameter sqlParameter = commandParameters[i];
				if (sqlParameter.Direction == ParameterDirection.InputOutput && sqlParameter.Value == null)
				{
					sqlParameter.Value = DBNull.Value;
				}
				command.Parameters.Add(sqlParameter);
			}
		}
		public static void AssignParameterValues(SqlParameter[] commandParameters, params object[] parameterValues)
		{
			if (commandParameters == null || parameterValues == null)
			{
				return;
			}
			if (commandParameters.Length != parameterValues.Length)
			{
				throw new ArgumentException("Parameter count does not match Parameter Value count.");
			}
			int i = 0;
			int num = commandParameters.Length;
			while (i < num)
			{
				if (parameterValues[i] != null && (commandParameters[i].Direction == ParameterDirection.Input || commandParameters[i].Direction == ParameterDirection.InputOutput))
				{
					commandParameters[i].Value = parameterValues[i];
				}
				i++;
			}
		}
		public static void AssignParameterValues(SqlParameter[] commandParameters, Hashtable parameterValues)
		{
			if (commandParameters == null || parameterValues == null)
			{
				return;
			}
			if (commandParameters.Length != parameterValues.Count)
			{
				throw new ArgumentException("Parameter count does not match Parameter Value count.");
			}
			int i = 0;
			int num = commandParameters.Length;
			while (i < num)
			{
				if (parameterValues[commandParameters[i].ParameterName] != null && (commandParameters[i].Direction == ParameterDirection.Input || commandParameters[i].Direction == ParameterDirection.InputOutput))
				{
					commandParameters[i].Value = parameterValues[commandParameters[i].ParameterName];
				}
				i++;
			}
		}
		private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
		{
			if (connection.State != ConnectionState.Open)
			{
				connection.Open();
			}
			command.Connection = connection;
			command.CommandText = commandText;
			if (transaction != null)
			{
				command.Transaction = transaction;
			}
			command.CommandType = commandType;
			if (commandParameters != null)
			{
				SqlHelper.AttachParameters(command, commandParameters);
			}
		}
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteNonQuery(connectionString, commandType, commandText, null);
		}
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				result = SqlHelper.ExecuteNonQuery(sqlConnection, commandType, commandText, commandParameters);
			}
			return result;
		}
		public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
		}
		public static int ExecuteNonQuery(string connectionString, string spName, Hashtable parameterValues)
		{
			if (parameterValues != null && parameterValues.Count > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
		}
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteNonQuery(connection, commandType, commandText, null);
		}
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters);
			int result = sqlCommand.ExecuteNonQuery();
			sqlCommand.Parameters.Clear();
			return result;
		}
		public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
		}
		public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters);
			int result = sqlCommand.ExecuteNonQuery();
			sqlCommand.Parameters.Clear();
			return result;
		}
		public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, new SqlParameter[0]);
		}
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteDataset(connectionString, commandType, commandText, null);
		}
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			DataSet result;
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				result = SqlHelper.ExecuteDataset(sqlConnection, commandType, commandText, commandParameters);
			}
			return result;
		}
		public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
		}
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteDataset(connection, commandType, commandText, null);
		}
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters);
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
			DataSet dataSet = new DataSet();
			sqlDataAdapter.Fill(dataSet);
			sqlCommand.Parameters.Clear();
			return dataSet;
		}
		public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
		}
		public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteDataset(transaction, commandType, commandText, null);
		}
		public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters);
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
			DataSet dataSet = new DataSet();
			sqlDataAdapter.Fill(dataSet);
			sqlCommand.Parameters.Clear();
			return dataSet;
		}
		public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
		}
		private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlHelper.SqlConnectionOwnership connectionOwnership)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, connection, transaction, commandType, commandText, commandParameters);
			SqlDataReader result;
			if (connectionOwnership == SqlHelper.SqlConnectionOwnership.External)
			{
				result = sqlCommand.ExecuteReader();
			}
			else
			{
				result = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
			}
			sqlCommand.Parameters.Clear();
			return result;
		}
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteReader(connectionString, commandType, commandText, null);
		}
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlConnection sqlConnection = new SqlConnection(connectionString);
			sqlConnection.Open();
			SqlDataReader result;
			try
			{
				result = SqlHelper.ExecuteReader(sqlConnection, null, commandType, commandText, commandParameters, SqlHelper.SqlConnectionOwnership.Internal);
			}
			catch
			{
				sqlConnection.Close();
				throw;
			}
			return result;
		}
		public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteReader(connection, commandType, commandText, null);
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlHelper.SqlConnectionOwnership.External);
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName);
		}
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteReader(transaction, commandType, commandText, null);
		}
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlHelper.SqlConnectionOwnership.External);
		}
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
		}
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteScalar(connectionString, commandType, commandText, null);
		}
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			object result;
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				result = SqlHelper.ExecuteScalar(sqlConnection, commandType, commandText, commandParameters);
			}
			return result;
		}
		public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
		}
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteScalar(connection, commandType, commandText, null);
		}
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters);
			object result = sqlCommand.ExecuteScalar();
			sqlCommand.Parameters.Clear();
			return result;
		}
		public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
		}
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteScalar(transaction, commandType, commandText, null);
		}
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters);
			object result = sqlCommand.ExecuteScalar();
			sqlCommand.Parameters.Clear();
			return result;
		}
		public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteXmlReader(connection, commandType, commandText, null);
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters);
			XmlReader result = sqlCommand.ExecuteXmlReader();
			sqlCommand.Parameters.Clear();
			return result;
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
		}
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteXmlReader(transaction, commandType, commandText, null);
		}
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters);
			XmlReader result = sqlCommand.ExecuteXmlReader();
			sqlCommand.Parameters.Clear();
			return result;
		}
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
		}
		public static void BeginExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters);
			sqlCommand.BeginExecuteNonQuery();
			sqlCommand.Parameters.Clear();
		}
		public static void BeginExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				SqlHelper.ExecuteNonQuery(sqlConnection, commandType, commandText, commandParameters);
			}
		}
		public static void BeginExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
				return;
			}
			SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
		}
	}
}
