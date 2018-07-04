using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace Bussiness
{
	public class MapBussiness : BaseBussiness
	{
		public MapInfo[] GetAllMap()
		{
			List<MapInfo> list = new List<MapInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Maps_All");
				while (sqlDataReader.Read())
				{
					list.Add(new MapInfo
					{
						BackMusic = (sqlDataReader["BackMusic"] == null) ? "" : sqlDataReader["BackMusic"].ToString(),
						BackPic = (sqlDataReader["BackPic"] == null) ? "" : sqlDataReader["BackPic"].ToString(),
						BackroundHeight = (int)sqlDataReader["BackroundHeight"],
						BackroundWidht = (int)sqlDataReader["BackroundWidht"],
						DeadHeight = (int)sqlDataReader["DeadHeight"],
						DeadPic = (sqlDataReader["DeadPic"] == null) ? "" : sqlDataReader["DeadPic"].ToString(),
						DeadWidth = (int)sqlDataReader["DeadWidth"],
						Description = (sqlDataReader["Description"] == null) ? "" : sqlDataReader["Description"].ToString(),
						DragIndex = (int)sqlDataReader["DragIndex"],
						ForegroundHeight = (int)sqlDataReader["ForegroundHeight"],
						ForegroundWidth = (int)sqlDataReader["ForegroundWidth"],
						ForePic = (sqlDataReader["ForePic"] == null) ? "" : sqlDataReader["ForePic"].ToString(),
						ID = (int)sqlDataReader["ID"],
						Name = (sqlDataReader["Name"] == null) ? "" : sqlDataReader["Name"].ToString(),
						Pic = (sqlDataReader["Pic"] == null) ? "" : sqlDataReader["Pic"].ToString(),
						Remark = (sqlDataReader["Remark"] == null) ? "" : sqlDataReader["Remark"].ToString(),
						Weight = (int)sqlDataReader["Weight"],
						PosX = (sqlDataReader["PosX"] == null) ? "" : sqlDataReader["PosX"].ToString(),
						PosX1 = (sqlDataReader["PosX1"] == null) ? "" : sqlDataReader["PosX1"].ToString(),
						Type = (byte)((int)sqlDataReader["Type"])
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllMap", exception);
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
		public ServerMapInfo[] GetAllServerMap()
		{
			List<ServerMapInfo> list = new List<ServerMapInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Maps_Server_All");
				while (sqlDataReader.Read())
				{
					list.Add(new ServerMapInfo
					{
						ServerID = (int)sqlDataReader["ServerID"],
						OpenMap = sqlDataReader["OpenMap"].ToString(),
						IsSpecial = (int)sqlDataReader["IsSpecial"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllMapWeek", exception);
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
	}
}
