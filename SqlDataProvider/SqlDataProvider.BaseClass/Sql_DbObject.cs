using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace SqlDataProvider.BaseClass
{
	public sealed class Sql_DbObject : IDisposable
	{
		private SqlConnection _SqlConnection;
		private SqlCommand _SqlCommand;
		private SqlDataAdapter _SqlDataAdapter;
		public Sql_DbObject()
		{
			this._SqlConnection = new SqlConnection();
		}
		public Sql_DbObject(string Path_Source, string Conn_DB)
		{
			if (Path_Source != null)
			{
				if (Path_Source == "WebConfig")
				{
					this._SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[Conn_DB].ConnectionString);
					return;
				}
				if (Path_Source == "File")
				{
					this._SqlConnection = new SqlConnection(Conn_DB);
					return;
				}
				if (Path_Source == "AppConfig")
				{
					string connectionString = ConfigurationManager.AppSettings[Conn_DB];
					this._SqlConnection = new SqlConnection(connectionString);
					return;
				}
			}
			this._SqlConnection = new SqlConnection(Conn_DB);
		}
		private static bool OpenConnection(SqlConnection _SqlConnection)
		{
			bool result = false;
			try
			{
				if (_SqlConnection.State != ConnectionState.Open)
				{
					_SqlConnection.Open();
					result = true;
				}
				else
				{
					result = true;
				}
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("´ò¿ªÊý¾Ý¿âÁ¬½Ó´íÎó:" + ex.Message.Trim());
				result = false;
			}
			return result;
		}
		public bool Exesqlcomm(string Sqlcomm)
		{
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return false;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.CommandType = CommandType.Text;
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandText = Sqlcomm;
				this._SqlCommand.ExecuteNonQuery();
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐsqlÓï¾ä: " + Sqlcomm + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
				return false;
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return true;
		}
		public int GetRecordCount(string Sqlcomm)
		{
			int result = 0;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = 0;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.Text;
					this._SqlCommand.CommandText = Sqlcomm;
					if (this._SqlCommand.ExecuteScalar() == null)
					{
						result = 0;
					}
					else
					{
						result = (int)this._SqlCommand.ExecuteScalar();
					}
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("Ö´ÐÐsqlÓï¾ä: " + Sqlcomm + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
			}
			return result;
		}
		public DataTable GetDataTableBySqlcomm(string TableName, string Sqlcomm)
		{
			DataTable dataTable = new DataTable(TableName);
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return dataTable;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.Text;
				this._SqlCommand.CommandText = Sqlcomm;
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				this._SqlDataAdapter.Fill(dataTable);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐsqlÓï¾ä: " + Sqlcomm + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return dataTable;
		}
		public DataSet GetDataSetBySqlcomm(string TableName, string Sqlcomm)
		{
			DataSet dataSet = new DataSet();
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return dataSet;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.Text;
				this._SqlCommand.CommandText = Sqlcomm;
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				this._SqlDataAdapter.Fill(dataSet);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐSqlÓï¾ä£º" + Sqlcomm + "´íÎóÐÅÏ¢Îª£º" + ex.Message.Trim());
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return dataSet;
		}
		public bool FillSqlDataReader(ref SqlDataReader Sdr, string SqlComm)
		{
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return false;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.Text;
				this._SqlCommand.CommandText = SqlComm;
				Sdr = this._SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				return true;
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐSqlÓï¾ä£º" + SqlComm + "´íÎóÐÅÏ¢Îª£º" + ex.Message.Trim());
			}
			finally
			{
				this.Dispose(true);
			}
			return false;
		}
		public DataTable GetDataTableBySqlcomm(string TableName, string Sqlcomm, int StartRecordNo, int PageSize)
		{
			DataTable dataTable = new DataTable(TableName);
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				dataTable.Dispose();
				this.Dispose(true);
				return dataTable;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.Text;
				this._SqlCommand.CommandText = Sqlcomm;
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable);
				this._SqlDataAdapter.Fill(dataSet, StartRecordNo, PageSize, TableName);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐsqlÓï¾ä: " + Sqlcomm + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return dataTable;
		}
		public bool RunProcedure(string ProcedureName, SqlParameter[] SqlParameters)
		{
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return false;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				for (int i = 0; i < SqlParameters.Length; i++)
				{
					SqlParameter value = SqlParameters[i];
					this._SqlCommand.Parameters.Add(value);
				}
				this._SqlCommand.ExecuteNonQuery();
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("ProcedureName: " + ProcedureName + "Message: " + ex.Message.Trim());
				return false;
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return true;
		}
		public bool RunProcedure(string ProcedureName)
		{
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return false;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				this._SqlCommand.ExecuteNonQuery();
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì: " + ProcedureName + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
				return false;
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return true;
		}
		public bool GetReader(ref SqlDataReader ResultDataReader, string ProcedureName)
		{
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return false;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				ResultDataReader = this._SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("ProcedureName: " + ProcedureName + "Message: " + ex.Message.Trim());
				return false;
			}
			return true;
		}
		public bool GetReader(ref SqlDataReader ResultDataReader, string ProcedureName, SqlParameter[] SqlParameters)
		{
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return false;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				for (int i = 0; i < SqlParameters.Length; i++)
				{
					SqlParameter value = SqlParameters[i];
					this._SqlCommand.Parameters.Add(value);
				}
				ResultDataReader = this._SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì: " + ProcedureName + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
				return false;
			}
			return true;
		}
		public DataSet GetDataSet(string ProcedureName, SqlParameter[] SqlParameters)
		{
			DataSet dataSet = new DataSet();
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				dataSet.Dispose();
				return dataSet;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				for (int i = 0; i < SqlParameters.Length; i++)
				{
					SqlParameter value = SqlParameters[i];
					this._SqlCommand.Parameters.Add(value);
				}
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				this._SqlDataAdapter.Fill(dataSet);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì£º" + ProcedureName + "´íÐÅÐÅÏ¢Îª£º" + ex.Message.Trim());
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return dataSet;
		}
		public bool GetDataSet(ref DataSet ResultDataSet, ref int row_total, string TableName, string ProcedureName, int StartRecordNo, int PageSize, SqlParameter[] SqlParameters)
		{
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return false;
			}
			try
			{
				row_total = 0;
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				for (int i = 0; i < SqlParameters.Length; i++)
				{
					SqlParameter value = SqlParameters[i];
					this._SqlCommand.Parameters.Add(value);
				}
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				DataSet dataSet = new DataSet();
				row_total = this._SqlDataAdapter.Fill(dataSet);
				this._SqlDataAdapter.Fill(ResultDataSet, StartRecordNo, PageSize, TableName);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì£º" + ProcedureName + "´íÎóÐÅÏ¢Îª£º" + ex.Message.Trim());
				return false;
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return true;
		}
		public DataSet GetDateSet(string DatesetName, string ProcedureName, SqlParameter[] SqlParameters)
		{
			DataSet dataSet = new DataSet(DatesetName);
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				dataSet.Dispose();
				return dataSet;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				for (int i = 0; i < SqlParameters.Length; i++)
				{
					SqlParameter value = SqlParameters[i];
					this._SqlCommand.Parameters.Add(value);
				}
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				this._SqlDataAdapter.Fill(dataSet);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì£º" + ProcedureName + "´íÐÅÐÅÏ¢Îª£º" + ex.Message.Trim());
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return dataSet;
		}
		public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters)
		{
			return this.GetDataTable(TableName, ProcedureName, SqlParameters, -1);
		}
		public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters, int commandTimeout)
		{
			DataTable dataTable = new DataTable(TableName);
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				dataTable.Dispose();
				this.Dispose(true);
				return dataTable;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				if (commandTimeout >= 0)
				{
					this._SqlCommand.CommandTimeout = commandTimeout;
				}
				for (int i = 0; i < SqlParameters.Length; i++)
				{
					SqlParameter value = SqlParameters[i];
					this._SqlCommand.Parameters.Add(value);
				}
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				this._SqlDataAdapter.Fill(dataTable);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì: " + ProcedureName + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return dataTable;
		}
		public DataTable GetDataTable(string TableName, string ProcedureName)
		{
			DataTable dataTable = new DataTable(TableName);
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				dataTable.Dispose();
				this.Dispose(true);
				return dataTable;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				this._SqlDataAdapter.Fill(dataTable);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì: " + ProcedureName + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return dataTable;
		}
		public DataTable GetDataTable(string TableName, string ProcedureName, int StartRecordNo, int PageSize)
		{
			DataTable dataTable = new DataTable(TableName);
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				dataTable.Dispose();
				this.Dispose(true);
				return dataTable;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable);
				this._SqlDataAdapter.Fill(dataSet, StartRecordNo, PageSize, TableName);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì: " + ProcedureName + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return dataTable;
		}
		public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters, int StartRecordNo, int PageSize)
		{
			DataTable dataTable = new DataTable(TableName);
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				dataTable.Dispose();
				this.Dispose(true);
				return dataTable;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				for (int i = 0; i < SqlParameters.Length; i++)
				{
					SqlParameter value = SqlParameters[i];
					this._SqlCommand.Parameters.Add(value);
				}
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable);
				this._SqlDataAdapter.Fill(dataSet, StartRecordNo, PageSize, TableName);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì: " + ProcedureName + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return dataTable;
		}
		public bool GetDataTable(ref DataTable ResultTable, string TableName, string ProcedureName, int StartRecordNo, int PageSize)
		{
			ResultTable = null;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return false;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(ResultTable);
				this._SqlDataAdapter.Fill(dataSet, StartRecordNo, PageSize, TableName);
				ResultTable = dataSet.Tables[TableName];
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì: " + ProcedureName + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
				return false;
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return true;
		}
		public bool GetDataTable(ref DataTable ResultTable, string TableName, string ProcedureName, int StartRecordNo, int PageSize, SqlParameter[] SqlParameters)
		{
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return false;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				for (int i = 0; i < SqlParameters.Length; i++)
				{
					SqlParameter value = SqlParameters[i];
					this._SqlCommand.Parameters.Add(value);
				}
				this._SqlDataAdapter = new SqlDataAdapter();
				this._SqlDataAdapter.SelectCommand = this._SqlCommand;
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(ResultTable);
				this._SqlDataAdapter.Fill(dataSet, StartRecordNo, PageSize, TableName);
				ResultTable = dataSet.Tables[TableName];
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì: " + ProcedureName + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
				return false;
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
			return true;
		}
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(true);
		}
		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (this._SqlDataAdapter != null)
			{
				if (this._SqlDataAdapter.SelectCommand != null)
				{
					if (this._SqlCommand.Connection != null)
					{
						this._SqlDataAdapter.SelectCommand.Connection.Dispose();
					}
					this._SqlDataAdapter.SelectCommand.Dispose();
				}
				this._SqlDataAdapter.Dispose();
				this._SqlDataAdapter = null;
			}
		}
		public void BeginRunProcedure(string ProcedureName, SqlParameter[] SqlParameters)
		{
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				return;
			}
			try
			{
				this._SqlCommand = new SqlCommand();
				this._SqlCommand.Connection = this._SqlConnection;
				this._SqlCommand.CommandType = CommandType.StoredProcedure;
				this._SqlCommand.CommandText = ProcedureName;
				for (int i = 0; i < SqlParameters.Length; i++)
				{
					SqlParameter value = SqlParameters[i];
					this._SqlCommand.Parameters.Add(value);
				}
				this._SqlCommand.BeginExecuteNonQuery();
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("Ö´ÐÐ´æ´¢¹ý³Ì: " + ProcedureName + "´íÎóÐÅÏ¢Îª: " + ex.Message.Trim());
			}
			finally
			{
				this._SqlConnection.Close();
				this.Dispose(true);
			}
		}
	}
}
