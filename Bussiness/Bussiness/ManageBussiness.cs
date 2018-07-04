using Bussiness.CenterService;
using SqlDataProvider.Data;
using System;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class ManageBussiness : BaseBussiness
	{
		public int KitoffUserByUserName(string name, string msg)
		{
			int result = 1;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				PlayerInfo userSingleByUserName = playerBussiness.GetUserSingleByUserName(name);
				if (userSingleByUserName == null)
				{
					return 2;
				}
				result = this.KitoffUser(userSingleByUserName.ID, msg);
			}
			return result;
		}
		public int KitoffUserByNickName(string name, string msg)
		{
			int result = 1;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				PlayerInfo userSingleByNickName = playerBussiness.GetUserSingleByNickName(name);
				if (userSingleByNickName == null)
				{
					return 2;
				}
				result = this.KitoffUser(userSingleByNickName.ID, msg);
			}
			return result;
		}
		public int KitoffUser(int id, string msg)
		{
			int result;
			try
			{
				using (CenterServiceClient centerServiceClient = new CenterServiceClient())
				{
					if (centerServiceClient.KitoffUser(id, msg))
					{
						result = 0;
					}
					else
					{
						result = 3;
					}
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("KitoffUser", exception);
				}
				result = 1;
			}
			return result;
		}
		public bool SystemNotice(string msg)
		{
			bool result = false;
			try
			{
				if (!string.IsNullOrEmpty(msg))
				{
					using (CenterServiceClient centerServiceClient = new CenterServiceClient())
					{
						if (centerServiceClient.SystemNotice(msg))
						{
							result = true;
						}
					}
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SystemNotice", exception);
				}
			}
			return result;
		}
		private bool ForbidPlayer(string userName, string nickName, int userID, DateTime forbidDate, bool isExist)
		{
			return this.ForbidPlayer(userName, nickName, userID, forbidDate, isExist, "");
		}
		private bool ForbidPlayer(string userName, string nickName, int userID, DateTime forbidDate, bool isExist, string ForbidReason)
		{
			bool result = false;
			try
			{
				SqlParameter[] array = new SqlParameter[6];
				array[0] = new SqlParameter("@UserName", userName);
				array[1] = new SqlParameter("@NickName", nickName);
				array[2] = new SqlParameter("@UserID", userID);
				array[2].Direction = ParameterDirection.InputOutput;
				array[3] = new SqlParameter("@ForbidDate", forbidDate);
				array[4] = new SqlParameter("@IsExist", isExist);
				array[5] = new SqlParameter("@ForbidReason", ForbidReason);
				this.db.RunProcedure("SP_Admin_ForbidUser", array);
				userID = (int)array[2].Value;
				if (userID > 0)
				{
					result = true;
					if (!isExist)
					{
						this.KitoffUser(userID, "You are kicking out by GM!!");
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
		public bool ForbidPlayerByUserName(string userName, DateTime date, bool isExist)
		{
			return this.ForbidPlayer(userName, "", 0, date, isExist);
		}
		public bool ForbidPlayerByNickName(string nickName, DateTime date, bool isExist)
		{
			return this.ForbidPlayer("", nickName, 0, date, isExist);
		}
		public bool ForbidPlayerByUserID(int userID, DateTime date, bool isExist)
		{
			return this.ForbidPlayer("", "", userID, date, isExist);
		}
		public bool ForbidPlayerByUserName(string userName, DateTime date, bool isExist, string ForbidReason)
		{
			return this.ForbidPlayer(userName, "", 0, date, isExist, ForbidReason);
		}
		public bool ForbidPlayerByNickName(string nickName, DateTime date, bool isExist, string ForbidReason)
		{
			return this.ForbidPlayer("", nickName, 0, date, isExist, ForbidReason);
		}
		public bool ForbidPlayerByUserID(int userID, DateTime date, bool isExist, string ForbidReason)
		{
			return this.ForbidPlayer("", "", userID, date, isExist, ForbidReason);
		}
		public bool ReLoadServerList()
		{
			bool result = false;
			try
			{
				using (CenterServiceClient centerServiceClient = new CenterServiceClient())
				{
					if (centerServiceClient.ReLoadServerList())
					{
						result = true;
					}
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("ReLoadServerList", exception);
				}
			}
			return result;
		}
		public int GetConfigState(int type)
		{
			int result = 2;
			try
			{
				using (CenterServiceClient centerServiceClient = new CenterServiceClient())
				{
					return centerServiceClient.GetConfigState(type);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetConfigState", exception);
				}
			}
			return result;
		}
		public bool UpdateConfigState(int type, bool state)
		{
			bool result = false;
			try
			{
				using (CenterServiceClient centerServiceClient = new CenterServiceClient())
				{
					return centerServiceClient.UpdateConfigState(type, state);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdateConfigState", exception);
				}
			}
			return result;
		}
		public bool Reload(string type)
		{
			bool result = false;
			try
			{
				using (CenterServiceClient centerServiceClient = new CenterServiceClient())
				{
					return centerServiceClient.Reload(type);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Reload", exception);
				}
			}
			return result;
		}
	}
}
