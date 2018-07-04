using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class ActiveBussiness : BaseBussiness
	{
		public ActiveInfo[] GetAllActives()
		{
			List<ActiveInfo> list = new List<ActiveInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Active_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitActiveInfo(sqlDataReader));
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
		public ActiveInfo GetSingleActives(int activeID)
		{
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				array[0].Value = activeID;
				this.db.GetReader(ref sqlDataReader, "SP_Active_Single", array);
				if (sqlDataReader.Read())
				{
					return this.InitActiveInfo(sqlDataReader);
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
		public ActiveInfo InitActiveInfo(SqlDataReader reader)
		{
			ActiveInfo activeInfo = new ActiveInfo();
			activeInfo.ActiveID = (int)reader["ActiveID"];
			activeInfo.Description = ((reader["Description"] == null) ? "" : reader["Description"].ToString());
			activeInfo.Content = ((reader["Content"] == null) ? "" : reader["Content"].ToString());
			activeInfo.AwardContent = ((reader["AwardContent"] == null) ? "" : reader["AwardContent"].ToString());
			activeInfo.HasKey = (int)reader["HasKey"];
			if (!string.IsNullOrEmpty(reader["EndDate"].ToString()))
			{
				activeInfo.EndDate = new DateTime?((DateTime)reader["EndDate"]);
			}
			activeInfo.IsOnly = (int)reader["IsOnly"];
			activeInfo.StartDate = (DateTime)reader["StartDate"];
			activeInfo.Title = reader["Title"].ToString();
			activeInfo.Type = (int)reader["Type"];
			activeInfo.ActionTimeContent = ((reader["ActionTimeContent"] == null) ? "" : reader["ActionTimeContent"].ToString());
			activeInfo.IsAdvance = (bool)reader["IsAdvance"];
			activeInfo.GoodsExchangeTypes = ((reader["GoodsExchangeTypes"] == null) ? "" : reader["GoodsExchangeTypes"].ToString());
			activeInfo.GoodsExchangeNum = ((reader["GoodsExchangeNum"] == null) ? "" : reader["GoodsExchangeNum"].ToString());
			activeInfo.limitType = ((reader["limitType"] == null) ? "" : reader["limitType"].ToString());
			activeInfo.limitValue = ((reader["limitValue"] == null) ? "" : reader["limitValue"].ToString());
			activeInfo.IsShow = (bool)reader["IsShow"];
			return activeInfo;
		}
		public int PullDown(int activeID, string awardID, int userID, ref string msg)
		{
			int result = 1;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@ActiveID", activeID),
					new SqlParameter("@AwardID", awardID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				array[3].Direction = ParameterDirection.ReturnValue;
				if (this.db.RunProcedure("SP_Active_PullDown", array))
				{
					result = (int)array[3].Value;
					switch (result)
					{
					case 0:
						msg = "ActiveBussiness.Msg0";
						break;

					case 1:
						msg = "ActiveBussiness.Msg1";
						break;

					case 2:
						msg = "ActiveBussiness.Msg2";
						break;

					case 3:
						msg = "ActiveBussiness.Msg3";
						break;

					case 4:
						msg = "ActiveBussiness.Msg4";
						break;

					case 5:
						msg = "ActiveBussiness.Msg5";
						break;

					case 6:
						msg = "ActiveBussiness.Msg6";
						break;

					case 7:
						msg = "ActiveBussiness.Msg7";
						break;

					case 8:
						msg = "ActiveBussiness.Msg8";
						break;

					default:
						msg = "ActiveBussiness.Msg9";
						break;
					}
				}
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
