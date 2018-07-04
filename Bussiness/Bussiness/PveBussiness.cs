using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace Bussiness
{
	public class PveBussiness : BaseBussiness
	{
		public PveInfo[] GetAllPveInfos()
		{
			List<PveInfo> list = new List<PveInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_PveInfos_All");
				while (sqlDataReader.Read())
				{
					PveInfo item = new PveInfo
					{
						ID = (int)sqlDataReader["Id"],
						Name = (sqlDataReader["Name"] == null) ? "" : sqlDataReader["Name"].ToString(),
						Type = (int)sqlDataReader["Type"],
						LevelLimits = (int)sqlDataReader["LevelLimits"],
						SimpleTemplateIds = (sqlDataReader["SimpleTemplateIds"] == null) ? "" : sqlDataReader["SimpleTemplateIds"].ToString(),
						NormalTemplateIds = (sqlDataReader["NormalTemplateIds"] == null) ? "" : sqlDataReader["NormalTemplateIds"].ToString(),
						HardTemplateIds = (sqlDataReader["HardTemplateIds"] == null) ? "" : sqlDataReader["HardTemplateIds"].ToString(),
						TerrorTemplateIds = (sqlDataReader["TerrorTemplateIds"] == null) ? "" : sqlDataReader["TerrorTemplateIds"].ToString(),
						Pic = (sqlDataReader["Pic"] == null) ? "" : sqlDataReader["Pic"].ToString(),
						Description = (sqlDataReader["Description"] == null) ? "" : sqlDataReader["Description"].ToString(),
						Ordering = (int)sqlDataReader["Ordering"],
						AdviceTips = (sqlDataReader["AdviceTips"] == null) ? "" : sqlDataReader["AdviceTips"].ToString(),
						SimpleGameScript = sqlDataReader["SimpleGameScript"] as string,
						NormalGameScript = sqlDataReader["NormalGameScript"] as string,
						HardGameScript = sqlDataReader["HardGameScript"] as string,
						TerrorGameScript = sqlDataReader["TerrorGameScript"] as string
					};
					list.Add(item);
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
	}
}
