using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class ServiceBussiness : BaseBussiness
	{
		public ServerInfo GetServiceSingle(int ID)
		{
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				array[0].Value = ID;
				this.db.GetReader(ref sqlDataReader, "SP_Service_Single", array);
				if (sqlDataReader.Read())
				{
					return new ServerInfo
					{
						ID = (int)sqlDataReader["ID"],
						IP = sqlDataReader["IP"].ToString(),
						Name = sqlDataReader["Name"].ToString(),
						Online = (int)sqlDataReader["Online"],
						Port = (int)sqlDataReader["Port"],
						Remark = sqlDataReader["Remark"].ToString(),
						Room = (int)sqlDataReader["Room"],
						State = (int)sqlDataReader["State"],
						Total = (int)sqlDataReader["Total"],
						RSA = sqlDataReader["RSA"].ToString(),
						NewerServer = (bool)sqlDataReader["NewerServer"]
					};
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
			return null;
		}
		public ServerInfo[] GetServiceByIP(string IP)
		{
			List<ServerInfo> list = new List<ServerInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@IP", SqlDbType.VarChar, 50)
				};
				array[0].Value = IP;
				this.db.GetReader(ref sqlDataReader, "SP_Service_ListByIP", array);
				while (sqlDataReader.Read())
				{
					list.Add(new ServerInfo
					{
						ID = (int)sqlDataReader["ID"],
						IP = sqlDataReader["IP"].ToString(),
						Name = sqlDataReader["Name"].ToString(),
						Online = (int)sqlDataReader["Online"],
						Port = (int)sqlDataReader["Port"],
						Remark = sqlDataReader["Remark"].ToString(),
						Room = (int)sqlDataReader["Room"],
						State = (int)sqlDataReader["State"],
						Total = (int)sqlDataReader["Total"],
						RSA = sqlDataReader["RSA"].ToString()
					});
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
			return list.ToArray();
		}
		public ServerInfo[] GetServerList()
		{
			List<ServerInfo> list = new List<ServerInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Service_List");
				while (sqlDataReader.Read())
				{
					list.Add(new ServerInfo
					{
						ID = (int)sqlDataReader["ID"],
						IP = sqlDataReader["IP"].ToString(),
						Name = sqlDataReader["Name"].ToString(),
						Online = (int)sqlDataReader["Online"],
						Port = (int)sqlDataReader["Port"],
						Remark = sqlDataReader["Remark"].ToString(),
						Room = (int)sqlDataReader["Room"],
						State = (int)sqlDataReader["State"],
						Total = (int)sqlDataReader["Total"],
						RSA = sqlDataReader["RSA"].ToString(),
						MustLevel = (int)sqlDataReader["MustLevel"],
						LowestLevel = (int)sqlDataReader["LowestLevel"]
					});
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
			return list.ToArray();
		}
		public RecordInfo GetRecordInfo(DateTime date, int SaveRecordSecond)
		{
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")),
					new SqlParameter("@Second", SaveRecordSecond)
				};
				this.db.GetReader(ref sqlDataReader, "SP_Server_Record", sqlParameters);
				if (sqlDataReader.Read())
				{
					return new RecordInfo
					{
						ActiveExpendBoy = (int)sqlDataReader["ActiveExpendBoy"],
						ActiveExpendGirl = (int)sqlDataReader["ActiveExpendGirl"],
						ActviePayBoy = (int)sqlDataReader["ActviePayBoy"],
						ActviePayGirl = (int)sqlDataReader["ActviePayGirl"],
						ExpendBoy = (int)sqlDataReader["ExpendBoy"],
						ExpendGirl = (int)sqlDataReader["ExpendGirl"],
						OnlineBoy = (int)sqlDataReader["OnlineBoy"],
						OnlineGirl = (int)sqlDataReader["OnlineGirl"],
						TotalBoy = (int)sqlDataReader["TotalBoy"],
						TotalGirl = (int)sqlDataReader["TotalGirl"],
						ActiveOnlineBoy = (int)sqlDataReader["ActiveOnlineBoy"],
						ActiveOnlineGirl = (int)sqlDataReader["ActiveOnlineGirl"]
					};
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
			return null;
		}
		public bool UpdateService(ServerInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@Online", info.Online),
					new SqlParameter("@State", info.State)
				};
				result = this.db.RunProcedure("SP_Service_Update", sqlParameters);
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
		public bool UpdateRSA(int ID, string RSA)
		{
			bool result = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", ID),
					new SqlParameter("@RSA", RSA)
				};
				result = this.db.RunProcedure("SP_Service_UpdateRSA", sqlParameters);
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
		public Dictionary<string, string> GetServerConfig()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Server_Config");
				while (sqlDataReader.Read())
				{
					if (!dictionary.ContainsKey(sqlDataReader["Name"].ToString()))
					{
						dictionary.Add(sqlDataReader["Name"].ToString(), sqlDataReader["Value"].ToString());
					}
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetServerConfig", exception);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return dictionary;
		}
		public ServerProperty GetServerPropertyByKey(string key)
		{
			ServerProperty serverProperty = null;
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@Key", key)
				};
				this.db.GetReader(ref sqlDataReader, "SP_Server_Config_Single", sqlParameters);
				while (sqlDataReader.Read())
				{
					serverProperty = new ServerProperty();
					serverProperty.Key = sqlDataReader["Name"].ToString();
					serverProperty.Value = sqlDataReader["Value"].ToString();
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetServerConfig", exception);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return serverProperty;
		}
		public bool UpdateServerPropertyByKey(string key, string value)
		{
			bool result = false;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@Key", key),
					new SqlParameter("@Value", value)
				};
				result = this.db.RunProcedure("SP_Server_Config_Update", sqlParameters);
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
		public ArrayList GetRate(int serverId)
		{
			SqlDataReader sqlDataReader = null;
			try
			{
				ArrayList arrayList = new ArrayList();
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ServerID", serverId)
				};
				this.db.GetReader(ref sqlDataReader, "SP_Rate", sqlParameters);
				while (sqlDataReader.Read())
				{
					arrayList.Add(new RateInfo
					{
						ServerID = (int)sqlDataReader["ServerID"],
						Rate = (float)((decimal)sqlDataReader["Rate"]),
						BeginDay = (DateTime)sqlDataReader["BeginDay"],
						EndDay = (DateTime)sqlDataReader["EndDay"],
						BeginTime = (DateTime)sqlDataReader["BeginTime"],
						EndTime = (DateTime)sqlDataReader["EndTime"],
						Type = (int)sqlDataReader["Type"]
					});
				}
				arrayList.TrimToSize();
				return arrayList;
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetRates", exception);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return null;
		}
		public RateInfo GetRateWithType(int serverId, int type)
		{
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ServerID", serverId),
					new SqlParameter("@Type", type)
				};
				this.db.GetReader(ref sqlDataReader, "SP_Rate_WithType", sqlParameters);
				if (sqlDataReader.Read())
				{
					return new RateInfo
					{
						ServerID = (int)sqlDataReader["ServerID"],
						Type = type,
						Rate = (float)sqlDataReader["Rate"],
						BeginDay = (DateTime)sqlDataReader["BeginDay"],
						EndDay = (DateTime)sqlDataReader["EndDay"],
						BeginTime = (DateTime)sqlDataReader["BeginTime"],
						EndTime = (DateTime)sqlDataReader["EndTime"]
					};
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetRate type: " + type, exception);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return null;
		}
		public FightRateInfo[] GetFightRate(int serverId)
		{
			SqlDataReader sqlDataReader = null;
			List<FightRateInfo> list = new List<FightRateInfo>();
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ServerID", serverId)
				};
				this.db.GetReader(ref sqlDataReader, "SP_Fight_Rate", sqlParameters);
				if (sqlDataReader.Read())
				{
					list.Add(new FightRateInfo
					{
						ID = (int)sqlDataReader["ID"],
						ServerID = (int)sqlDataReader["ServerID"],
						Rate = (int)sqlDataReader["Rate"],
						BeginDay = (DateTime)sqlDataReader["BeginDay"],
						EndDay = (DateTime)sqlDataReader["EndDay"],
						BeginTime = (DateTime)sqlDataReader["BeginTime"],
						EndTime = (DateTime)sqlDataReader["EndTime"],
						SelfCue = (sqlDataReader["SelfCue"] == null) ? "" : sqlDataReader["SelfCue"].ToString(),
						EnemyCue = (sqlDataReader["EnemyCue"] == null) ? "" : sqlDataReader["EnemyCue"].ToString(),
						BoyTemplateID = (int)sqlDataReader["BoyTemplateID"],
						GirlTemplateID = (int)sqlDataReader["GirlTemplateID"],
						Name = (sqlDataReader["Name"] == null) ? "" : sqlDataReader["Name"].ToString()
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetFightRate", exception);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return list.ToArray();
		}
		public string GetGameEdition()
		{
			string result = string.Empty;
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Server_Edition");
				if (sqlDataReader.Read())
				{
					result = ((sqlDataReader["value"] == null) ? "" : sqlDataReader["value"].ToString());
					return result;
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
			return result;
		}
	}
}
