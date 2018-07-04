using SqlDataProvider.Data;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class ItemRecordBussiness : BaseBussiness
	{
		public void PropertyString(ItemInfo item, ref string Property)
		{
			if (item != null)
			{
				Property = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", new object[]
				{
					item.StrengthenLevel,
					item.Attack,
					item.Defence,
					item.Agility,
					item.Luck,
					item.AttackCompose,
					item.DefendCompose,
					item.AgilityCompose,
					item.LuckCompose
				});
			}
		}
		public void FusionItem(ItemInfo item, ref string Property)
		{
			if (item != null)
			{
				Property = Property + string.Format("{0}:{1},{2}", item.ItemID, item.Template.Name, Convert.ToInt32(item.IsBinds)) + "|";
			}
		}
		public bool LogItemDb(DataTable dt)
		{
			bool result = false;
			if (dt == null)
			{
				return result;
			}
			SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(ConfigurationManager.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
			try
			{
				sqlBulkCopy.NotifyAfter = dt.Rows.Count;
				sqlBulkCopy.DestinationTableName = "Log_Item";
				sqlBulkCopy.ColumnMappings.Add(0, "ApplicationId");
				sqlBulkCopy.ColumnMappings.Add(1, "SubId");
				sqlBulkCopy.ColumnMappings.Add(2, "LineId");
				sqlBulkCopy.ColumnMappings.Add(3, "EnterTime");
				sqlBulkCopy.ColumnMappings.Add(4, "UserId");
				sqlBulkCopy.ColumnMappings.Add(5, "Operation");
				sqlBulkCopy.ColumnMappings.Add(6, "ItemName");
				sqlBulkCopy.ColumnMappings.Add(7, "ItemID");
				sqlBulkCopy.ColumnMappings.Add(8, "AddItem");
				sqlBulkCopy.ColumnMappings.Add(9, "BeginProperty");
				sqlBulkCopy.ColumnMappings.Add(10, "EndProperty");
				sqlBulkCopy.ColumnMappings.Add(11, "Result");
				sqlBulkCopy.WriteToServer(dt);
				result = true;
				dt.Clear();
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Smith Log Error:" + ex.ToString());
				}
			}
			finally
			{
				sqlBulkCopy.Close();
			}
			return result;
		}
		public bool LogMoneyDb(DataTable dt)
		{
			bool result = false;
			if (dt == null)
			{
				return result;
			}
			SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(ConfigurationManager.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
			try
			{
				sqlBulkCopy.NotifyAfter = dt.Rows.Count;
				sqlBulkCopy.DestinationTableName = "Log_Money";
				sqlBulkCopy.ColumnMappings.Add(0, "ApplicationId");
				sqlBulkCopy.ColumnMappings.Add(1, "SubId");
				sqlBulkCopy.ColumnMappings.Add(2, "LineId");
				sqlBulkCopy.ColumnMappings.Add(3, "MastType");
				sqlBulkCopy.ColumnMappings.Add(4, "SonType");
				sqlBulkCopy.ColumnMappings.Add(5, "UserId");
				sqlBulkCopy.ColumnMappings.Add(6, "EnterTime");
				sqlBulkCopy.ColumnMappings.Add(7, "Moneys");
				sqlBulkCopy.ColumnMappings.Add(8, "Gold");
				sqlBulkCopy.ColumnMappings.Add(9, "GiftToken");
				sqlBulkCopy.ColumnMappings.Add(10, "Offer");
				sqlBulkCopy.ColumnMappings.Add(11, "OtherPay");
				sqlBulkCopy.ColumnMappings.Add(12, "GoodId");
				sqlBulkCopy.ColumnMappings.Add(13, "ShopId");
				sqlBulkCopy.ColumnMappings.Add(14, "Datas");
				sqlBulkCopy.WriteToServer(dt);
				result = true;
			}
			catch
			{
			}
			finally
			{
				sqlBulkCopy.Close();
				dt.Clear();
			}
			return result;
		}
		public bool LogFightDb(DataTable dt)
		{
			bool result = false;
			if (dt == null)
			{
				return result;
			}
			SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(ConfigurationManager.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
			try
			{
				sqlBulkCopy.NotifyAfter = dt.Rows.Count;
				sqlBulkCopy.DestinationTableName = "Log_Fight";
				sqlBulkCopy.ColumnMappings.Add(0, "ApplicationId");
				sqlBulkCopy.ColumnMappings.Add(1, "SubId");
				sqlBulkCopy.ColumnMappings.Add(2, "LineId");
				sqlBulkCopy.ColumnMappings.Add(3, "RoomId");
				sqlBulkCopy.ColumnMappings.Add(4, "RoomType");
				sqlBulkCopy.ColumnMappings.Add(5, "FightType");
				sqlBulkCopy.ColumnMappings.Add(6, "ChangeTeam");
				sqlBulkCopy.ColumnMappings.Add(7, "PlayBegin");
				sqlBulkCopy.ColumnMappings.Add(8, "PlayEnd");
				sqlBulkCopy.ColumnMappings.Add(9, "UserCount");
				sqlBulkCopy.ColumnMappings.Add(10, "MapId");
				sqlBulkCopy.ColumnMappings.Add(11, "TeamA");
				sqlBulkCopy.ColumnMappings.Add(12, "TeamB");
				sqlBulkCopy.ColumnMappings.Add(13, "PlayResult");
				sqlBulkCopy.ColumnMappings.Add(14, "WinTeam");
				sqlBulkCopy.ColumnMappings.Add(15, "Detail");
				sqlBulkCopy.WriteToServer(dt);
				result = true;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Fight Log Error:" + ex.ToString());
				}
			}
			finally
			{
				sqlBulkCopy.Close();
				dt.Clear();
			}
			return result;
		}
		public bool LogServerDb(DataTable dt)
		{
			bool result = false;
			if (dt == null)
			{
				return result;
			}
			SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(ConfigurationManager.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
			try
			{
				sqlBulkCopy.NotifyAfter = dt.Rows.Count;
				sqlBulkCopy.DestinationTableName = "Log_Server";
				sqlBulkCopy.ColumnMappings.Add(0, "ApplicationId");
				sqlBulkCopy.ColumnMappings.Add(1, "SubId");
				sqlBulkCopy.ColumnMappings.Add(2, "EnterTime");
				sqlBulkCopy.ColumnMappings.Add(3, "Online");
				sqlBulkCopy.ColumnMappings.Add(4, "Reg");
				sqlBulkCopy.WriteToServer(dt);
				result = true;
			}
			catch (Exception ex)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Server Log Error:" + ex.ToString());
				}
			}
			finally
			{
				sqlBulkCopy.Close();
				dt.Clear();
			}
			return result;
		}
	}
}
