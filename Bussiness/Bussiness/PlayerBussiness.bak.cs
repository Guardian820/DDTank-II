using Bussiness.CenterService;
using Bussiness.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
    public class PlayerBussiness : BaseBussiness
    {
        public ConsortiaUserInfo[] GetAllMemberByConsortia(int ConsortiaID)
        {
            List<ConsortiaUserInfo> infos = new List<ConsortiaUserInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", SqlDbType.Int, 4)
				};
                para[0].Value = ConsortiaID;
                this.db.GetReader(ref reader, "SP_Consortia_Users_All", para);
                while (reader.Read())
                {
                    infos.Add(this.InitConsortiaUserInfo(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public ConsortiaUserInfo InitConsortiaUserInfo(SqlDataReader dr)
        {
            ConsortiaUserInfo info = new ConsortiaUserInfo();
            info.ID = (int)dr["ID"];
            info.ConsortiaID = (int)dr["ConsortiaID"];
            info.DutyID = (int)dr["DutyID"];
            info.DutyName = dr["DutyName"].ToString();
            info.IsExist = (bool)dr["IsExist"];
            info.RatifierID = (int)dr["RatifierID"];
            info.RatifierName = dr["RatifierName"].ToString();
            info.Remark = dr["Remark"].ToString();
            info.UserID = (int)dr["UserID"];
            info.UserName = dr["UserName"].ToString();
            info.Grade = (int)dr["Grade"];
            info.GP = (int)dr["GP"];
            info.Repute = (int)dr["Repute"];
            info.State = (int)dr["State"];
            info.Right = (int)dr["Right"];
            info.Offer = (int)dr["Offer"];
            info.Colors = dr["Colors"].ToString();
            info.Style = dr["Style"].ToString();
            info.Hide = (int)dr["Hide"];
            info.Skin = ((dr["Skin"] == null) ? "" : info.Skin);
            info.Level = (int)dr["Level"];
            info.LastDate = (DateTime)dr["LastDate"];
            info.Sex = (bool)dr["Sex"];
            info.IsBanChat = (bool)dr["IsBanChat"];
            info.Win = (int)dr["Win"];
            info.Total = (int)dr["Total"];
            info.Escape = (int)dr["Escape"];
            info.RichesOffer = (int)dr["RichesOffer"];
            info.RichesRob = (int)dr["RichesRob"];
            info.LoginName = ((dr["LoginName"] == null) ? "" : dr["LoginName"].ToString());
            info.Nimbus = (int)dr["Nimbus"];
            info.FightPower = (int)dr["FightPower"];
            info.typeVIP = Convert.ToByte(dr["typeVIP"]);
            info.VIPLevel = (int)dr["VIPLevel"];
            return info;
        }
        public bool ActivePlayer(ref PlayerInfo player, string userName, string passWord, bool sex, int gold, int money, string IP, string site)
        {
            bool result = false;
            try
            {
                player = new PlayerInfo();
                player.Agility = 0;
                player.Attack = 0;
                player.Colors = ",,,,,,";
                player.Skin = "";
                player.ConsortiaID = 0;
                player.Defence = 0;
                player.Gold = 0;
                player.GP = 1;
                player.Grade = 1;
                player.ID = 0;
                player.Luck = 0;
                player.Money = 0;
                player.NickName = "";
                player.Sex = sex;
                player.State = 0;
                player.Style = ",,,,,,";
                player.Hide = 1111111111;
                SqlParameter[] para = new SqlParameter[21];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@Attack", player.Attack);
                para[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
                para[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
                para[4] = new SqlParameter("@Defence", player.Defence);
                para[5] = new SqlParameter("@Gold", player.Gold);
                para[6] = new SqlParameter("@GP", player.GP);
                para[7] = new SqlParameter("@Grade", player.Grade);
                para[8] = new SqlParameter("@Luck", player.Luck);
                para[9] = new SqlParameter("@Money", player.Money);
                para[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
                para[11] = new SqlParameter("@Agility", player.Agility);
                para[12] = new SqlParameter("@State", player.State);
                para[13] = new SqlParameter("@UserName", userName);
                para[14] = new SqlParameter("@PassWord", passWord);
                para[15] = new SqlParameter("@Sex", sex);
                para[16] = new SqlParameter("@Hide", player.Hide);
                para[17] = new SqlParameter("@ActiveIP", IP);
                para[18] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
                para[19] = new SqlParameter("@Result", SqlDbType.Int);
                para[19].Direction = ParameterDirection.ReturnValue;
                para[20] = new SqlParameter("@Site", site);
                result = this.db.RunProcedure("SP_Users_Active", para);
                player.ID = (int)para[0].Value;
                result = ((int)para[19].Value == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool RegisterPlayer(string userName, string passWord, string nickName, string bStyle, string gStyle, string armColor, string hairColor, string faceColor, string clothColor, string hatColor, int sex, ref string msg, int validDate)
        {
            bool result = false;
            try
            {
                string[] bStyles = bStyle.Split(new char[]
				{
					','
				});
                string[] gStyles = gStyle.Split(new char[]
				{
					','
				});
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserName", userName),
					new SqlParameter("@PassWord", passWord),
					new SqlParameter("@NickName", nickName),
					new SqlParameter("@BArmID", int.Parse(bStyles[0])),
					new SqlParameter("@BHairID", int.Parse(bStyles[1])),
					new SqlParameter("@BFaceID", int.Parse(bStyles[2])),
					new SqlParameter("@BClothID", int.Parse(bStyles[3])),
					new SqlParameter("@BHatID", int.Parse(bStyles[4])),
					new SqlParameter("@GArmID", int.Parse(gStyles[0])),
					new SqlParameter("@GHairID", int.Parse(gStyles[1])),
					new SqlParameter("@GFaceID", int.Parse(gStyles[2])),
					new SqlParameter("@GClothID", int.Parse(gStyles[3])),
					new SqlParameter("@GHatID", int.Parse(gStyles[4])),
					new SqlParameter("@ArmColor", armColor),
					new SqlParameter("@HairColor", hairColor),
					new SqlParameter("@FaceColor", faceColor),
					new SqlParameter("@ClothColor", clothColor),
					new SqlParameter("@HatColor", clothColor),
					new SqlParameter("@Sex", sex),
					new SqlParameter("@StyleDate", validDate),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[20].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Users_RegisterNotValidate", para);
                int returnValue = (int)para[20].Value;
                result = (returnValue == 0);
                switch (returnValue)
                {
                    case 2:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg2", new object[0]);
                        break;

                    case 3:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg3", new object[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool RenameNick(string userName, string nickName, string newNickName, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserName", userName),
					new SqlParameter("@NickName", nickName),
					new SqlParameter("@NewNickName", newNickName),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[3].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Users_RenameNick", para);
                int returnValue = (int)para[3].Value;
                result = (returnValue == 0);
                switch (returnValue)
                {
                    case 4:
                    case 5:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.RenameNick.Msg4", new object[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("RenameNick", e);
                }
            }
            return result;
        }
        public bool DisableUser(string userName, bool isExit)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserName", userName),
					new SqlParameter("@IsExist", isExit),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[2].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Disable_User", para);
                if ((int)para[2].Value == 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("DisableUser", e);
                }
            }
            return result;
        }
        public bool RenameConsortiaName(string userName, string nickName, string consortiaName, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserName", userName),
					new SqlParameter("@NickName", nickName),
					new SqlParameter("@ConsortiaName", consortiaName),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[3].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Users_RenameConsortiaName", para);
                int returnValue = (int)para[3].Value;
                result = (returnValue == 0);
                switch (returnValue)
                {
                    case 4:
                    case 5:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.SP_Users_RenameConsortiaName.Msg4", new object[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("RenameNick", e);
                }
            }
            return result;
        }
        public bool UpdatePassWord(int userID, string password)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Password", password)
				};
                result = this.db.RunProcedure("SP_Users_UpdatePassword", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdatePasswordInfo(int userID, string PasswordQuestion1, string PasswordAnswer1, string PasswordQuestion2, string PasswordAnswer2, int Count)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", userID),
					new SqlParameter("@PasswordQuestion1", PasswordQuestion1),
					new SqlParameter("@PasswordAnswer1", PasswordAnswer1),
					new SqlParameter("@PasswordQuestion2", PasswordQuestion2),
					new SqlParameter("@PasswordAnswer2", PasswordAnswer2),
					new SqlParameter("@FailedPasswordAttemptCount", Count)
				};
                result = this.db.RunProcedure("SP_Sys_Users_Password_Add", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public void GetPasswordInfo(int userID, ref string PasswordQuestion1, ref string PasswordAnswer1, ref string PasswordQuestion2, ref string PasswordAnswer2, ref int Count)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", userID)
				};
                this.db.GetReader(ref reader, "SP_Users_PasswordInfo", para);
                while (reader.Read())
                {
                    PasswordQuestion1 = ((reader["PasswordQuestion1"] == null) ? "" : reader["PasswordQuestion1"].ToString());
                    PasswordAnswer1 = ((reader["PasswordAnswer1"] == null) ? "" : reader["PasswordAnswer1"].ToString());
                    PasswordQuestion2 = ((reader["PasswordQuestion2"] == null) ? "" : reader["PasswordQuestion2"].ToString());
                    PasswordAnswer2 = ((reader["PasswordAnswer2"] == null) ? "" : reader["PasswordAnswer2"].ToString());
                    DateTime Today = (DateTime)reader["LastFindDate"];
                    if (Today == DateTime.Today)
                    {
                        Count = (int)reader["FailedPasswordAttemptCount"];
                    }
                    else
                    {
                        Count = 5;
                    }
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
        }
        public bool UpdatePasswordTwo(int userID, string passwordTwo)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", userID),
					new SqlParameter("@PasswordTwo", passwordTwo)
				};
                result = this.db.RunProcedure("SP_Users_UpdatePasswordTwo", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public PlayerInfo[] GetUserLoginList(string userName)
        {
            List<PlayerInfo> infos = new List<PlayerInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserName", SqlDbType.VarChar, 200)
				};
                para[0].Value = userName;
                this.db.GetReader(ref reader, "SP_Users_LoginList", para);
                while (reader.Read())
                {
                    infos.Add(this.InitPlayerInfo(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public PlayerInfo LoginGame(string username, ref int isFirst, ref bool isExist, ref bool isError, bool firstValidate, ref DateTime forbidDate, string nickname, string ActiveIP)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserName", username),
					new SqlParameter("@Password", ""),
					new SqlParameter("@FirstValidate", firstValidate),
					new SqlParameter("@Nickname", nickname),
					new SqlParameter("@ActiveIP", ActiveIP)
				};
                this.db.GetReader(ref reader, "SP_Users_LoginWeb", para);
                if (reader.Read())
                {
                    isFirst = (int)reader["IsFirst"];
                    isExist = (bool)reader["IsExist"];
                    forbidDate = (DateTime)reader["ForbidDate"];
                    if (isFirst > 1)
                    {
                        isFirst--;
                    }
                    return this.InitPlayerInfo(reader);
                }
            }
            catch (Exception e)
            {
                isError = true;
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public PlayerInfo LoginGame(string username, string password)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserName", username),
					new SqlParameter("@Password", password)
				};
                this.db.GetReader(ref reader, "SP_Users_Login", para);
                if (reader.Read())
                {
                    return this.InitPlayerInfo(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public PlayerInfo ReLoadPlayer(int ID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", ID)
				};
                this.db.GetReader(ref reader, "SP_Users_Reload", para);
                if (reader.Read())
                {
                    return this.InitPlayerInfo(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public bool UpdatePlayer(PlayerInfo player)
        {
            bool result = false;
            try
            {
                if (player.Grade < 1)
                {
                    return result;
                }
                SqlParameter[] para = new SqlParameter[77];
                para[0] = new SqlParameter("@UserID", player.ID);
                para[1] = new SqlParameter("@Attack", player.Attack);
                para[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
                para[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
                para[4] = new SqlParameter("@Defence", player.Defence);
                para[5] = new SqlParameter("@Gold", player.Gold);
                para[6] = new SqlParameter("@GP", player.GP);
                para[7] = new SqlParameter("@Grade", player.Grade);
                para[8] = new SqlParameter("@Luck", player.Luck);
                para[9] = new SqlParameter("@Money", player.Money);
                para[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
                para[11] = new SqlParameter("@Agility", player.Agility);
                para[12] = new SqlParameter("@State", player.State);
                para[13] = new SqlParameter("@Hide", player.Hide);
                para[14] = new SqlParameter("@ExpendDate", (!player.ExpendDate.HasValue) ? "" : player.ExpendDate.ToString());
                para[15] = new SqlParameter("@Win", player.Win);
                para[16] = new SqlParameter("@Total", player.Total);
                para[17] = new SqlParameter("@Escape", player.Escape);
                para[18] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
                para[19] = new SqlParameter("@Offer", player.Offer);
                para[20] = new SqlParameter("@AntiAddiction", player.AntiAddiction);
                para[20].Direction = ParameterDirection.InputOutput;
                para[21] = new SqlParameter("@Result", SqlDbType.Int);
                para[21].Direction = ParameterDirection.ReturnValue;
                para[22] = new SqlParameter("@RichesOffer", player.RichesOffer);
                para[23] = new SqlParameter("@RichesRob", player.RichesRob);
                para[24] = new SqlParameter("@CheckCount", player.CheckCount);
                para[24].Direction = ParameterDirection.InputOutput;
                para[25] = new SqlParameter("@MarryInfoID", player.MarryInfoID);
                para[26] = new SqlParameter("@DayLoginCount", player.DayLoginCount);
                para[27] = new SqlParameter("@Nimbus", player.Nimbus);
                para[28] = new SqlParameter("@LastAward", player.LastAward);
                para[29] = new SqlParameter("@GiftToken", player.GiftToken);
                para[30] = new SqlParameter("@QuestSite", player.QuestSite);
                para[31] = new SqlParameter("@PvePermission", player.PvePermission);
                para[32] = new SqlParameter("@FightPower", player.FightPower);
                para[33] = new SqlParameter("@AnswerSite", player.AnswerSite);
                para[34] = new SqlParameter("@LastAuncherAward", player.LastAward);
                para[35] = new SqlParameter("@hp", player.hp);
                para[36] = new SqlParameter("@ChatCount", player.ChatCount);
                para[37] = new SqlParameter("@SpaPubGoldRoomLimit", player.SpaPubGoldRoomLimit);
                para[38] = new SqlParameter("@LastSpaDate", player.LastSpaDate);
                para[39] = new SqlParameter("@FightLabPermission", player.FightLabPermission);
                para[40] = new SqlParameter("@SpaPubMoneyRoomLimit", player.SpaPubMoneyRoomLimit);
                para[41] = new SqlParameter("@IsInSpaPubGoldToday", player.IsInSpaPubGoldToday);
                para[42] = new SqlParameter("@IsInSpaPubMoneyToday", player.IsInSpaPubMoneyToday);
                para[43] = new SqlParameter("@AchievementPoint", player.AchievementPoint);
                para[44] = new SqlParameter("@LastWeekly", player.LastWeekly);
                para[45] = new SqlParameter("@LastWeeklyVersion", player.LastWeeklyVersion);
                para[46] = new SqlParameter("@GiftGp", player.GiftGp);
                para[47] = new SqlParameter("@GiftLevel", player.GiftLevel);
                para[48] = new SqlParameter("@IsOpenGift", player.IsOpenGift);
                para[49] = new SqlParameter("@WeaklessGuildProgressStr", player.WeaklessGuildProgressStr);
                para[50] = new SqlParameter("@IsOldPlayer", player.IsOldPlayer);
                para[51] = new SqlParameter("@VIPLevel", player.VIPLevel);
                para[52] = new SqlParameter("@VIPExp", player.VIPExp);
                para[53] = new SqlParameter("@Score", player.Score);
                para[54] = new SqlParameter("@OptionOnOff", player.OptionOnOff);
                para[55] = new SqlParameter("@isOldPlayerHasValidEquitAtLogin", player.isOldPlayerHasValidEquitAtLogin);
                para[56] = new SqlParameter("@badLuckNumber", player.badLuckNumber);
                para[57] = new SqlParameter("@luckyNum", player.luckyNum);
                para[58] = new SqlParameter("@lastLuckyNumDate", player.lastLuckyNumDate.ToString());
                para[59] = new SqlParameter("@lastLuckNum", player.lastLuckNum);
                para[60] = new SqlParameter("@CardSoul", player.CardSoul);
                para[61] = new SqlParameter("@uesedFinishTime", player.uesedFinishTime);
                para[62] = new SqlParameter("@totemId", player.totemId);
                para[63] = new SqlParameter("@damageScores", player.damageScores);
                para[64] = new SqlParameter("@petScore", player.petScore);
                para[65] = new SqlParameter("@IsShowConsortia", player.IsShowConsortia);
                para[66] = new SqlParameter("@LastRefreshPet", player.LastRefreshPet.ToString());
                para[67] = new SqlParameter("@receiebox", player.receiebox);
                para[68] = new SqlParameter("@receieGrade", player.receieGrade);
                para[69] = new SqlParameter("@needGetBoxTime", player.needGetBoxTime);
                para[70] = new SqlParameter("@myScore", player.myScore);
                para[71] = new SqlParameter("@TimeBox", player.TimeBox.ToString());
                para[72] = new SqlParameter("@IsFistGetPet", player.IsFistGetPet);
                para[73] = new SqlParameter("@MaxBuyHonor", player.MaxBuyHonor);
                para[74] = new SqlParameter("@Medal", player.medal);
                para[75] = new SqlParameter("@myHonor", player.myHonor);
                para[76] = new SqlParameter("@LeagueMoney", player.LeagueMoney);
                this.db.RunProcedure("SP_Users_Update", para);
                result = ((int)para[21].Value == 0);
                if (result)
                {
                    player.AntiAddiction = (int)para[20].Value;
                    player.CheckCount = (int)para[24].Value;
                }
                Console.Write("___Loi luu item da duoc fix _______");
                player.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdatePlayerMarry(PlayerInfo player)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", player.ID),
					new SqlParameter("@IsMarried", player.IsMarried),
					new SqlParameter("@SpouseID", player.SpouseID),
					new SqlParameter("@SpouseName", player.SpouseName),
					new SqlParameter("@IsCreatedMarryRoom", player.IsCreatedMarryRoom),
					new SqlParameter("@SelfMarryRoomID", player.SelfMarryRoomID),
					new SqlParameter("@IsGotRing", player.IsGotRing)
				};
                result = this.db.RunProcedure("SP_Users_Marry", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdatePlayerMarry", e);
                }
            }
            return result;
        }
        public bool UpdatePlayerLastAward(int id, int type)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", id),
					new SqlParameter("@Type", type)
				};
                result = this.db.RunProcedure("SP_Users_LastAward", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdatePlayerAward", e);
                }
            }
            return result;
        }
        public PlayerInfo GetUserSingleByUserID(int UserID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_SingleByUserID", para);
                if (reader.Read())
                {
                    return this.InitPlayerInfo(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public PlayerInfo GetUserSingleByUserName(string userName)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserName", SqlDbType.VarChar, 200)
				};
                para[0].Value = userName;
                this.db.GetReader(ref reader, "SP_Users_SingleByUserName", para);
                if (reader.Read())
                {
                    return this.InitPlayerInfo(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public PlayerInfo GetUserSingleByNickName(string nickName)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@NickName", SqlDbType.VarChar, 200)
				};
                para[0].Value = nickName;
                this.db.GetReader(ref reader, "SP_Users_SingleByNickName", para);
                if (reader.Read())
                {
                    return this.InitPlayerInfo(reader);
                }
            }
            catch
            {
                throw new Exception();
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public PlayerInfo InitPlayerInfo(SqlDataReader reader)
        {
            PlayerInfo player = new PlayerInfo();
            player.Password = (string)reader["Password"];
            player.IsConsortia = (bool)reader["IsConsortia"];
            player.Agility = (int)reader["Agility"];
            player.Attack = (int)reader["Attack"];
            player.hp = (int)reader["hp"];
            player.Colors = ((reader["Colors"] == null) ? "" : reader["Colors"].ToString());
            player.ConsortiaID = (int)reader["ConsortiaID"];
            player.Defence = (int)reader["Defence"];
            player.Gold = (int)reader["Gold"];
            player.GP = (int)reader["GP"];
            player.Grade = (int)reader["Grade"];
            player.ID = (int)reader["UserID"];
            player.Luck = (int)reader["Luck"];
            player.Money = (int)reader["Money"];
            player.NickName = ((reader["NickName"] == null) ? "" : reader["NickName"].ToString());
            player.Sex = (bool)reader["Sex"];
            player.State = (int)reader["State"];
            player.Style = ((reader["Style"] == null) ? "" : reader["Style"].ToString());
            player.Hide = (int)reader["Hide"];
            player.Repute = (int)reader["Repute"];
            player.UserName = ((reader["UserName"] == null) ? "" : reader["UserName"].ToString());
            player.ConsortiaName = ((reader["ConsortiaName"] == null) ? "" : reader["ConsortiaName"].ToString());
            player.Offer = (int)reader["Offer"];
            player.Win = (int)reader["Win"];
            player.Total = (int)reader["Total"];
            player.Escape = (int)reader["Escape"];
            player.Skin = ((reader["Skin"] == null) ? "" : reader["Skin"].ToString());
            player.IsBanChat = (bool)reader["IsBanChat"];
            player.ReputeOffer = (int)reader["ReputeOffer"];
            player.ConsortiaRepute = (int)reader["ConsortiaRepute"];
            player.ConsortiaLevel = (int)reader["ConsortiaLevel"];
            player.StoreLevel = (int)reader["StoreLevel"];
            player.ShopLevel = (int)reader["ShopLevel"];
            player.SmithLevel = (int)reader["SmithLevel"];
            player.ConsortiaHonor = (int)reader["ConsortiaHonor"];
            player.RichesOffer = (int)reader["RichesOffer"];
            player.RichesRob = (int)reader["RichesRob"];
            player.AntiAddiction = (int)reader["AntiAddiction"];
            player.DutyLevel = (int)reader["DutyLevel"];
            player.DutyName = ((reader["DutyName"] == null) ? "" : reader["DutyName"].ToString());
            player.Right = (int)reader["Right"];
            player.ChairmanName = ((reader["ChairmanName"] == null) ? "" : reader["ChairmanName"].ToString());
            player.AddDayGP = (int)reader["AddDayGP"];
            player.AddDayOffer = (int)reader["AddDayOffer"];
            player.AddWeekGP = (int)reader["AddWeekGP"];
            player.AddWeekOffer = (int)reader["AddWeekOffer"];
            player.ConsortiaRiches = (int)reader["ConsortiaRiches"];
            player.CheckCount = (int)reader["CheckCount"];
            player.IsMarried = (bool)reader["IsMarried"];
            player.SpouseID = (int)reader["SpouseID"];
            player.SpouseName = ((reader["SpouseName"] == null) ? "" : reader["SpouseName"].ToString());
            player.MarryInfoID = (int)reader["MarryInfoID"];
            player.IsCreatedMarryRoom = (bool)reader["IsCreatedMarryRoom"];
            player.DayLoginCount = (int)reader["DayLoginCount"];
            player.PasswordTwo = ((reader["PasswordTwo"] == null) ? "" : reader["PasswordTwo"].ToString());
            player.SelfMarryRoomID = (int)reader["SelfMarryRoomID"];
            player.IsGotRing = (bool)reader["IsGotRing"];
            player.Rename = (bool)reader["Rename"];
            player.ConsortiaRename = (bool)reader["ConsortiaRename"];
            player.IsDirty = false;
            player.IsFirst = (int)reader["IsFirst"];
            player.Nimbus = (int)reader["Nimbus"];
            player.LastAward = (DateTime)reader["LastAward"];
            player.GiftToken = (int)reader["GiftToken"];
            player.QuestSite = ((reader["QuestSite"] == null) ? new byte[200] : ((byte[])reader["QuestSite"]));
            player.PvePermission = ((reader["PvePermission"] == null) ? "" : reader["PvePermission"].ToString());
            player.FightPower = (int)reader["FightPower"];
            player.PasswordQuest1 = ((reader["PasswordQuestion1"] == null) ? "" : reader["PasswordQuestion1"].ToString());
            player.PasswordQuest2 = ((reader["PasswordQuestion2"] == null) ? "" : reader["PasswordQuestion2"].ToString());
            PlayerInfo arg_724_0 = player;
            DateTime arg_713_0 = (DateTime)reader["LastFindDate"];
            arg_724_0.FailedPasswordAttemptCount = (int)reader["FailedPasswordAttemptCount"];
            player.AnswerSite = (int)reader["AnswerSite"];
            player.medal = (int)reader["Medal"];
            player.ChatCount = (int)reader["ChatCount"];
            player.SpaPubGoldRoomLimit = (int)reader["SpaPubGoldRoomLimit"];
            player.LastSpaDate = (DateTime)reader["LastSpaDate"];
            player.FightLabPermission = (string)reader["FightLabPermission"];
            player.SpaPubMoneyRoomLimit = (int)reader["SpaPubMoneyRoomLimit"];
            player.IsInSpaPubGoldToday = (bool)reader["IsInSpaPubGoldToday"];
            player.IsInSpaPubMoneyToday = (bool)reader["IsInSpaPubMoneyToday"];
            player.AchievementPoint = (int)reader["AchievementPoint"];
            player.LastWeekly = (DateTime)reader["LastWeekly"];
            player.LastWeeklyVersion = (int)reader["LastWeeklyVersion"];
            player.GiftGp = (int)reader["GiftGp"];
            player.GiftLevel = (int)reader["GiftLevel"];
            player.IsOpenGift = (bool)reader["IsOpenGift"];
            player.badgeID = (int)reader["badgeID"];
            player.typeVIP = Convert.ToByte(reader["typeVIP"]);
            player.VIPLevel = (int)reader["VIPLevel"];
            player.VIPExp = (int)reader["VIPExp"];
            player.VIPExpireDay = (DateTime)reader["VIPExpireDay"];
            player.LastVIPPackTime = (DateTime)reader["LastVIPPackTime"];
            player.CanTakeVipReward = (bool)reader["CanTakeVipReward"];
            player.WeaklessGuildProgressStr = (string)reader["WeaklessGuildProgressStr"];
            player.IsOldPlayer = (bool)reader["IsOldPlayer"];
            player.LastDate = (DateTime)reader["LastDate"];
            player.VIPLastDate = (DateTime)reader["VIPLastDate"];
            player.Score = (int)reader["Score"];
            player.OptionOnOff = (int)reader["OptionOnOff"];
            player.isOldPlayerHasValidEquitAtLogin = (bool)reader["isOldPlayerHasValidEquitAtLogin"];
            player.badLuckNumber = (int)reader["badLuckNumber"];
            player.luckyNum = (int)reader["luckyNum"];
            player.lastLuckyNumDate = (DateTime)reader["lastLuckyNumDate"];
            player.lastLuckNum = (int)reader["lastLuckNum"];
            player.CardSoul = (int)reader["CardSoul"];
            player.uesedFinishTime = (int)reader["uesedFinishTime"];
            player.totemId = (int)reader["totemId"];
            player.damageScores = (int)reader["damageScores"];
            player.petScore = (int)reader["petScore"];
            player.IsShowConsortia = (bool)reader["IsShowConsortia"];
            player.LastRefreshPet = (DateTime)reader["LastRefreshPet"];
            player.receiebox = (int)reader["receiebox"];
            player.receieGrade = (int)reader["receieGrade"];
            player.needGetBoxTime = (int)reader["needGetBoxTime"];
            player.myScore = (int)reader["myScore"];
            player.LastGetEgg = (DateTime)reader["LastGetEgg"];
            player.TimeBox = (DateTime)reader["TimeBox"];
            player.IsFistGetPet = (bool)reader["IsFistGetPet"];
            player.myHonor = (int)reader["myHonor"];
            player.hardCurrency = (int)reader["hardCurrency"];
            player.MaxBuyHonor = (int)reader["MaxBuyHonor"];
            player.LeagueMoney = (int)reader["LeagueMoney"];
            return player;
        }
        public PlayerInfo[] GetPlayerPage(int page, int size, ref int total, int order, int userID, ref bool resultValue)
        {
            List<PlayerInfo> infos = new List<PlayerInfo>();
            try
            {
                string sWhere = " IsExist=1 and IsFirst<> 0 ";
                if (userID != -1)
                {
                    object obj = sWhere;
                    sWhere = string.Concat(new object[]
					{
						obj,
						" and UserID =",
						userID,
						" "
					});
                }
                string sOrder = "GP desc";
                switch (order)
                {
                    case 1:
                        sOrder = "Offer desc";
                        break;

                    case 2:
                        sOrder = "AddDayGP desc";
                        break;

                    case 3:
                        sOrder = "AddWeekGP desc";
                        break;

                    case 4:
                        sOrder = "AddDayOffer desc";
                        break;

                    case 5:
                        sOrder = "AddWeekOffer desc";
                        break;

                    case 6:
                        sOrder = "FightPower desc";
                        break;
                }
                sOrder += ",UserID";
                DataTable dt = base.GetPage("V_Sys_Users_Detail", sWhere, page, size, "*", sOrder, "UserID", ref total);
                foreach (DataRow dr in dt.Rows)
                {
                    infos.Add(new PlayerInfo
                    {
                        Agility = (int)dr["Agility"],
                        Attack = (int)dr["Attack"],
                        Colors = (dr["Colors"] == null) ? "" : dr["Colors"].ToString(),
                        ConsortiaID = (int)dr["ConsortiaID"],
                        Defence = (int)dr["Defence"],
                        Gold = (int)dr["Gold"],
                        GP = (int)dr["GP"],
                        Grade = (int)dr["Grade"],
                        ID = (int)dr["UserID"],
                        Luck = (int)dr["Luck"],
                        Money = (int)dr["Money"],
                        NickName = (dr["NickName"] == null) ? "" : dr["NickName"].ToString(),
                        Sex = (bool)dr["Sex"],
                        State = (int)dr["State"],
                        Style = (dr["Style"] == null) ? "" : dr["Style"].ToString(),
                        Hide = (int)dr["Hide"],
                        Repute = (int)dr["Repute"],
                        UserName = (dr["UserName"] == null) ? "" : dr["UserName"].ToString(),
                        ConsortiaName = (dr["ConsortiaName"] == null) ? "" : dr["ConsortiaName"].ToString(),
                        Offer = (int)dr["Offer"],
                        Skin = (dr["Skin"] == null) ? "" : dr["Skin"].ToString(),
                        IsBanChat = (bool)dr["IsBanChat"],
                        ReputeOffer = (int)dr["ReputeOffer"],
                        ConsortiaRepute = (int)dr["ConsortiaRepute"],
                        ConsortiaLevel = (int)dr["ConsortiaLevel"],
                        StoreLevel = (int)dr["StoreLevel"],
                        ShopLevel = (int)dr["ShopLevel"],
                        SmithLevel = (int)dr["SmithLevel"],
                        ConsortiaHonor = (int)dr["ConsortiaHonor"],
                        RichesOffer = (int)dr["RichesOffer"],
                        RichesRob = (int)dr["RichesRob"],
                        DutyLevel = (int)dr["DutyLevel"],
                        DutyName = (dr["DutyName"] == null) ? "" : dr["DutyName"].ToString(),
                        Right = (int)dr["Right"],
                        ChairmanName = (dr["ChairmanName"] == null) ? "" : dr["ChairmanName"].ToString(),
                        Win = (int)dr["Win"],
                        Total = (int)dr["Total"],
                        Escape = (int)dr["Escape"],
                        AddDayGP = (int)dr["AddDayGP"],
                        AddDayOffer = (int)dr["AddDayOffer"],
                        AddWeekGP = (int)dr["AddWeekGP"],
                        AddWeekOffer = (int)dr["AddWeekOffer"],
                        ConsortiaRiches = (int)dr["ConsortiaRiches"],
                        CheckCount = (int)dr["CheckCount"],
                        Nimbus = (int)dr["Nimbus"],
                        GiftToken = (int)dr["GiftToken"],
                        QuestSite = (dr["QuestSite"] == null) ? new byte[200] : ((byte[])dr["QuestSite"]),
                        PvePermission = (dr["PvePermission"] == null) ? "" : dr["PvePermission"].ToString(),
                        FightPower = (int)dr["FightPower"]
                    });
                }
                resultValue = true;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return infos.ToArray();
        }
        public ItemInfo[] GetUserItem(int UserID)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_Items_All", para);
                while (reader.Read())
                {
                    items.Add(this.InitItem(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items.ToArray();
        }
        public ItemInfo[] GetUserBagByType(int UserID, int bagType)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int, 4);
                para[0].Value = UserID;
                para[1] = new SqlParameter("@BagType", bagType);
                this.db.GetReader(ref reader, "SP_Users_BagByType", para);
                while (reader.Read())
                {
                    items.Add(this.InitItem(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items.ToArray();
        }
        public List<ItemInfo> GetUserEuqip(int UserID)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_Items_Equip", para);
                while (reader.Read())
                {
                    items.Add(this.InitItem(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items;
        }
        public List<ItemInfo> GetUserBeadEuqip(int UserID)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_Bead_Equip", para);
                while (reader.Read())
                {
                    items.Add(this.InitItem(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items;
        }
        public List<ItemInfo> GetUserEuqipByNick(string Nick)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@NickName", SqlDbType.NVarChar, 200)
				};
                para[0].Value = Nick;
                this.db.GetReader(ref reader, "SP_Users_Items_Equip_By_Nick", para);
                while (reader.Read())
                {
                    items.Add(this.InitItem(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items;
        }
        public ItemInfo GetUserItemSingle(int itemID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
                para[0].Value = itemID;
                this.db.GetReader(ref reader, "SP_Users_Items_Single", para);
                if (reader.Read())
                {
                    return this.InitItem(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public ItemInfo InitItem(SqlDataReader reader)
        {
            return new ItemInfo(ItemMgr.FindItemTemplate((int)reader["TemplateID"]))
            {
                AgilityCompose = (int)reader["AgilityCompose"],
                AttackCompose = (int)reader["AttackCompose"],
                Color = reader["Color"].ToString(),
                Count = (int)reader["Count"],
                DefendCompose = (int)reader["DefendCompose"],
                ItemID = (int)reader["ItemID"],
                LuckCompose = (int)reader["LuckCompose"],
                Place = (int)reader["Place"],
                StrengthenLevel = (int)reader["StrengthenLevel"],
                TemplateID = (int)reader["TemplateID"],
                UserID = (int)reader["UserID"],
                ValidDate = (int)reader["ValidDate"],
                IsDirty = false,
                IsExist = (bool)reader["IsExist"],
                IsBinds = (bool)reader["IsBinds"],
                IsUsed = (bool)reader["IsUsed"],
                BeginDate = (DateTime)reader["BeginDate"],
                IsJudge = (bool)reader["IsJudge"],
                BagType = (int)reader["BagType"],
                Skin = reader["Skin"].ToString(),
                RemoveDate = (DateTime)reader["RemoveDate"],
                RemoveType = (int)reader["RemoveType"],
                Hole1 = (int)reader["Hole1"],
                Hole2 = (int)reader["Hole2"],
                Hole3 = (int)reader["Hole3"],
                Hole4 = (int)reader["Hole4"],
                Hole5 = (int)reader["Hole5"],
                Hole6 = (int)reader["Hole6"],
                StrengthenTimes = (int)reader["StrengthenTimes"],
                StrengthenExp = (int)reader["StrengthenExp"],
                Hole5Level = (int)reader["Hole5Level"],
                Hole5Exp = (int)reader["Hole5Exp"],
                Hole6Level = (int)reader["Hole6Level"],
                Hole6Exp = (int)reader["Hole6Exp"],
                IsGold = (bool)reader["IsGold"],
                goldBeginTime = (DateTime)reader["goldBeginTime"],
                goldValidDate = (int)reader["goldValidDate"],
                beadExp = (int)reader["beadExp"],
                beadLevel = (int)reader["beadLevel"],
                beadIsLock = (bool)reader["beadIsLock"],
                isShowBind = (bool)reader["isShowBind"],
                latentEnergyCurStr = (string)reader["latentEnergyCurStr"],
                latentEnergyNewStr = (string)reader["latentEnergyNewStr"],
                latentEnergyEndTime = (DateTime)reader["latentEnergyEndTime"],
                Damage = (int)reader["Damage"],
                Guard = (int)reader["Guard"],
                Blood = (int)reader["Blood"],
                Bless = (int)reader["Bless"]
            };
        }
        public bool AddGoods(ItemInfo item)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[46];
                para[0] = new SqlParameter("@ItemID", item.ItemID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@UserID", item.UserID);
                para[2] = new SqlParameter("@TemplateID", item.TemplateID);
                para[3] = new SqlParameter("@Place", item.Place);
                para[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                para[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                para[6] = new SqlParameter("@BeginDate", item.BeginDate);
                para[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
                para[8] = new SqlParameter("@Count", item.Count);
                para[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                para[10] = new SqlParameter("@IsBinds", item.IsBinds);
                para[11] = new SqlParameter("@IsExist", item.IsExist);
                para[12] = new SqlParameter("@IsJudge", item.IsJudge);
                para[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                para[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
                para[15] = new SqlParameter("@ValidDate", item.ValidDate);
                para[16] = new SqlParameter("@BagType", item.BagType);
                para[17] = new SqlParameter("@Skin", (item.Skin == null) ? "" : item.Skin);
                para[18] = new SqlParameter("@IsUsed", item.IsUsed);
                para[19] = new SqlParameter("@RemoveType", item.RemoveType);
                para[20] = new SqlParameter("@Hole1", item.Hole1);
                para[21] = new SqlParameter("@Hole2", item.Hole2);
                para[22] = new SqlParameter("@Hole3", item.Hole3);
                para[23] = new SqlParameter("@Hole4", item.Hole4);
                para[24] = new SqlParameter("@Hole5", item.Hole5);
                para[25] = new SqlParameter("@Hole6", item.Hole6);
                para[26] = new SqlParameter("@StrengthenTimes", item.StrengthenTimes);
                para[27] = new SqlParameter("@Hole5Level", item.Hole5Level);
                para[28] = new SqlParameter("@Hole5Exp", item.Hole5Exp);
                para[29] = new SqlParameter("@Hole6Level", item.Hole6Level);
                para[30] = new SqlParameter("@Hole6Exp", item.Hole6Exp);
                para[31] = new SqlParameter("@IsGold", item.IsGold);
                para[32] = new SqlParameter("@goldValidDate", item.goldValidDate);
                para[33] = new SqlParameter("@StrengthenExp", item.StrengthenExp);
                para[34] = new SqlParameter("@beadExp", item.beadExp);
                para[35] = new SqlParameter("@beadLevel", item.beadLevel);
                para[36] = new SqlParameter("@beadIsLock", item.beadIsLock);
                para[37] = new SqlParameter("@isShowBind", item.isShowBind);
                para[38] = new SqlParameter("@Damage", item.Damage);
                para[39] = new SqlParameter("@Guard", item.Guard);
                para[40] = new SqlParameter("@Blood", item.Blood);
                para[41] = new SqlParameter("@Bless", item.Bless);
                para[42] = new SqlParameter("@goldBeginTime", item.goldBeginTime);
                para[43] = new SqlParameter("@latentEnergyEndTime", item.latentEnergyEndTime);
                para[44] = new SqlParameter("@latentEnergyCurStr", item.latentEnergyCurStr);
                para[45] = new SqlParameter("@latentEnergyNewStr", item.latentEnergyNewStr);
                result = this.db.RunProcedure("SP_Users_Items_Add", para);
                item.ItemID = (int)para[0].Value;
                item.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdateGoods(ItemInfo item)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ItemID", item.ItemID),
					new SqlParameter("@UserID", item.UserID),
					new SqlParameter("@TemplateID", item.TemplateID),
					new SqlParameter("@Place", item.Place),
					new SqlParameter("@AgilityCompose", item.AgilityCompose),
					new SqlParameter("@AttackCompose", item.AttackCompose),
					new SqlParameter("@BeginDate", item.BeginDate),
					new SqlParameter("@Color", (item.Color == null) ? "" : item.Color),
					new SqlParameter("@Count", item.Count),
					new SqlParameter("@DefendCompose", item.DefendCompose),
					new SqlParameter("@IsBinds", item.IsBinds),
					new SqlParameter("@IsExist", item.IsExist),
					new SqlParameter("@IsJudge", item.IsJudge),
					new SqlParameter("@LuckCompose", item.LuckCompose),
					new SqlParameter("@StrengthenLevel", item.StrengthenLevel),
					new SqlParameter("@ValidDate", item.ValidDate),
					new SqlParameter("@BagType", item.BagType),
					new SqlParameter("@Skin", item.Skin),
					new SqlParameter("@IsUsed", item.IsUsed),
					new SqlParameter("@RemoveDate", item.RemoveDate),
					new SqlParameter("@RemoveType", item.RemoveType),
					new SqlParameter("@Hole1", item.Hole1),
					new SqlParameter("@Hole2", item.Hole2),
					new SqlParameter("@Hole3", item.Hole3),
					new SqlParameter("@Hole4", item.Hole4),
					new SqlParameter("@Hole5", item.Hole5),
					new SqlParameter("@Hole6", item.Hole6),
					new SqlParameter("@StrengthenTimes", item.StrengthenTimes),
					new SqlParameter("@Hole5Level", item.Hole5Level),
					new SqlParameter("@Hole5Exp", item.Hole5Exp),
					new SqlParameter("@Hole6Level", item.Hole6Level),
					new SqlParameter("@Hole6Exp", item.Hole6Exp),
					new SqlParameter("@IsGold", item.IsGold),
					new SqlParameter("@goldBeginTime", item.goldBeginTime.ToString()),
					new SqlParameter("@goldValidDate", item.goldValidDate),
					new SqlParameter("@StrengthenExp", item.StrengthenExp),
					new SqlParameter("@beadExp", item.beadExp),
					new SqlParameter("@beadLevel", item.beadLevel),
					new SqlParameter("@beadIsLock", item.beadIsLock),
					new SqlParameter("@isShowBind", item.isShowBind),
					new SqlParameter("@latentEnergyCurStr", item.latentEnergyCurStr),
					new SqlParameter("@latentEnergyNewStr", item.latentEnergyNewStr),
					new SqlParameter("@latentEnergyEndTime", item.latentEnergyEndTime.ToString()),
					new SqlParameter("@Damage", item.Damage),
					new SqlParameter("@Guard", item.Guard),
					new SqlParameter("@Blood", item.Blood),
					new SqlParameter("@Bless", item.Bless)
				};
                result = this.db.RunProcedure("SP_Users_Items_Update", para);
                item.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool DeleteGoods(int itemID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", itemID)
				};
                result = this.db.RunProcedure("SP_Users_Items_Delete", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public BestEquipInfo[] GetCelebByDayBestEquip()
        {
            List<BestEquipInfo> infos = new List<BestEquipInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_Users_BestEquip");
                while (reader.Read())
                {
                    infos.Add(new BestEquipInfo
                    {
                        Date = (DateTime)reader["RemoveDate"],
                        GP = (int)reader["GP"],
                        Grade = (int)reader["Grade"],
                        ItemName = (reader["Name"] == null) ? "" : reader["Name"].ToString(),
                        NickName = (reader["NickName"] == null) ? "" : reader["NickName"].ToString(),
                        Sex = (bool)reader["Sex"],
                        Strengthenlevel = (int)reader["Strengthenlevel"],
                        UserName = (reader["UserName"] == null) ? "" : reader["UserName"].ToString()
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public MailInfo InitMail(SqlDataReader reader)
        {
            return new MailInfo
            {
                Annex1 = reader["Annex1"].ToString(),
                Annex2 = reader["Annex2"].ToString(),
                Content = reader["Content"].ToString(),
                Gold = (int)reader["Gold"],
                ID = (int)reader["ID"],
                IsExist = (bool)reader["IsExist"],
                Money = (int)reader["Money"],
                GiftToken = (int)reader["GiftToken"],
                Receiver = reader["Receiver"].ToString(),
                ReceiverID = (int)reader["ReceiverID"],
                Sender = reader["Sender"].ToString(),
                SenderID = (int)reader["SenderID"],
                Title = reader["Title"].ToString(),
                Type = (int)reader["Type"],
                ValidDate = (int)reader["ValidDate"],
                IsRead = (bool)reader["IsRead"],
                SendTime = (DateTime)reader["SendTime"],
                Annex1Name = (reader["Annex1Name"] == null) ? "" : reader["Annex1Name"].ToString(),
                Annex2Name = (reader["Annex2Name"] == null) ? "" : reader["Annex2Name"].ToString(),
                Annex3 = reader["Annex3"].ToString(),
                Annex4 = reader["Annex4"].ToString(),
                Annex5 = reader["Annex5"].ToString(),
                Annex3Name = (reader["Annex3Name"] == null) ? "" : reader["Annex3Name"].ToString(),
                Annex4Name = (reader["Annex4Name"] == null) ? "" : reader["Annex4Name"].ToString(),
                Annex5Name = (reader["Annex5Name"] == null) ? "" : reader["Annex5Name"].ToString(),
                AnnexRemark = (reader["AnnexRemark"] == null) ? "" : reader["AnnexRemark"].ToString()
            };
        }
        public MailInfo[] GetMailByUserID(int userID)
        {
            List<MailInfo> items = new List<MailInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = userID;
                this.db.GetReader(ref reader, "SP_Mail_ByUserID", para);
                while (reader.Read())
                {
                    items.Add(this.InitMail(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items.ToArray();
        }
        public MailInfo[] GetMailBySenderID(int userID)
        {
            List<MailInfo> items = new List<MailInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = userID;
                this.db.GetReader(ref reader, "SP_Mail_BySenderID", para);
                while (reader.Read())
                {
                    items.Add(this.InitMail(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items.ToArray();
        }
        public MailInfo GetMailSingle(int UserID, int mailID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", mailID),
					new SqlParameter("@UserID", UserID)
				};
                this.db.GetReader(ref reader, "SP_Mail_Single", para);
                if (reader.Read())
                {
                    return this.InitMail(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public bool SendMail(MailInfo mail)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[29];
                para[0] = new SqlParameter("@ID", mail.ID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                para[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                para[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                para[4] = new SqlParameter("@Gold", mail.Gold);
                para[5] = new SqlParameter("@IsExist", true);
                para[6] = new SqlParameter("@Money", mail.Money);
                para[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                para[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                para[10] = new SqlParameter("@SenderID", mail.SenderID);
                para[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                para[12] = new SqlParameter("@IfDelS", false);
                para[13] = new SqlParameter("@IsDelete", false);
                para[14] = new SqlParameter("@IsDelR", false);
                para[15] = new SqlParameter("@IsRead", false);
                para[16] = new SqlParameter("@SendTime", DateTime.Now);
                para[17] = new SqlParameter("@Type", mail.Type);
                para[18] = new SqlParameter("@Annex1Name", (mail.Annex1Name == null) ? "" : mail.Annex1Name);
                para[19] = new SqlParameter("@Annex2Name", (mail.Annex2Name == null) ? "" : mail.Annex2Name);
                para[20] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
                para[21] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
                para[22] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
                para[23] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
                para[24] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
                para[25] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
                para[26] = new SqlParameter("@ValidDate", mail.ValidDate);
                para[27] = new SqlParameter("@AnnexRemark", (mail.AnnexRemark == null) ? "" : mail.AnnexRemark);
                para[28] = new SqlParameter("GiftToken", mail.GiftToken);
                result = this.db.RunProcedure("SP_Mail_Send", para);
                mail.ID = (int)para[0].Value;
                using (CenterServiceClient client = new CenterServiceClient())
                {
                    client.MailNotice(mail.ReceiverID);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool DeleteMail(int UserID, int mailID, out int senderID)
        {
            bool result = false;
            senderID = 0;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ID", mailID);
                para[1] = new SqlParameter("@UserID", UserID);
                para[2] = new SqlParameter("@SenderID", SqlDbType.Int);
                para[2].Value = senderID;
                para[2].Direction = ParameterDirection.InputOutput;
                para[3] = new SqlParameter("@Result", SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Mail_Delete", para);
                if ((int)para[3].Value == 0)
                {
                    result = true;
                    senderID = (int)para[2].Value;
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdateMail(MailInfo mail, int oldMoney)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[30];
                para[0] = new SqlParameter("@ID", mail.ID);
                para[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                para[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                para[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                para[4] = new SqlParameter("@Gold", mail.Gold);
                para[5] = new SqlParameter("@IsExist", mail.IsExist);
                para[6] = new SqlParameter("@Money", mail.Money);
                para[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                para[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                para[10] = new SqlParameter("@SenderID", mail.SenderID);
                para[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                para[12] = new SqlParameter("@IfDelS", false);
                para[13] = new SqlParameter("@IsDelete", false);
                para[14] = new SqlParameter("@IsDelR", false);
                para[15] = new SqlParameter("@IsRead", mail.IsRead);
                para[16] = new SqlParameter("@SendTime", mail.SendTime);
                para[17] = new SqlParameter("@Type", mail.Type);
                para[18] = new SqlParameter("@OldMoney", oldMoney);
                para[19] = new SqlParameter("@ValidDate", mail.ValidDate);
                para[20] = new SqlParameter("@Annex1Name", mail.Annex1Name);
                para[21] = new SqlParameter("@Annex2Name", mail.Annex2Name);
                para[22] = new SqlParameter("@Result", SqlDbType.Int);
                para[22].Direction = ParameterDirection.ReturnValue;
                para[23] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
                para[24] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
                para[25] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
                para[26] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
                para[27] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
                para[28] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
                para[29] = new SqlParameter("GiftToken", mail.GiftToken);
                this.db.RunProcedure("SP_Mail_Update", para);
                int returnValue = (int)para[22].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool CancelPaymentMail(int userid, int mailID, ref int senderID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@userid", userid);
                para[1] = new SqlParameter("@mailID", mailID);
                para[2] = new SqlParameter("@senderID", SqlDbType.Int);
                para[2].Value = senderID;
                para[2].Direction = ParameterDirection.InputOutput;
                para[3] = new SqlParameter("@Result", SqlDbType.Int);
                para[3].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Mail_PaymentCancel", para);
                int returnValue = (int)para[3].Value;
                result = (returnValue == 0);
                if (result)
                {
                    senderID = (int)para[2].Value;
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool ScanMail(ref string noticeUserID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
				};
                para[0].Direction = ParameterDirection.Output;
                this.db.RunProcedure("SP_Mail_Scan", para);
                noticeUserID = para[0].Value.ToString();
                result = true;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool SendMailAndItem(MailInfo mail, ItemInfo item, ref int returnValue)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[34];
                para[0] = new SqlParameter("@ItemID", item.ItemID);
                para[1] = new SqlParameter("@UserID", item.UserID);
                para[2] = new SqlParameter("@TemplateID", item.TemplateID);
                para[3] = new SqlParameter("@Place", item.Place);
                para[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
                para[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
                para[6] = new SqlParameter("@BeginDate", item.BeginDate);
                para[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
                para[8] = new SqlParameter("@Count", item.Count);
                para[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
                para[10] = new SqlParameter("@IsBinds", item.IsBinds);
                para[11] = new SqlParameter("@IsExist", item.IsExist);
                para[12] = new SqlParameter("@IsJudge", item.IsJudge);
                para[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
                para[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
                para[15] = new SqlParameter("@ValidDate", item.ValidDate);
                para[16] = new SqlParameter("@BagType", item.BagType);
                para[17] = new SqlParameter("@ID", mail.ID);
                para[17].Direction = ParameterDirection.Output;
                para[18] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                para[19] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                para[20] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                para[21] = new SqlParameter("@Gold", mail.Gold);
                para[22] = new SqlParameter("@Money", mail.Money);
                para[23] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                para[24] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[25] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                para[26] = new SqlParameter("@SenderID", mail.SenderID);
                para[27] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                para[28] = new SqlParameter("@IfDelS", false);
                para[29] = new SqlParameter("@IsDelete", false);
                para[30] = new SqlParameter("@IsDelR", false);
                para[31] = new SqlParameter("@IsRead", false);
                para[32] = new SqlParameter("@SendTime", DateTime.Now);
                para[33] = new SqlParameter("@Result", SqlDbType.Int);
                para[33].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Admin_SendUserItem", para);
                returnValue = (int)para[33].Value;
                result = (returnValue == 0);
                if (result)
                {
                    using (CenterServiceClient client = new CenterServiceClient())
                    {
                        client.MailNotice(mail.ReceiverID);
                    }
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool SendMailAndMoney(MailInfo mail, ref int returnValue)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[18];
                para[0] = new SqlParameter("@ID", mail.ID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
                para[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
                para[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
                para[4] = new SqlParameter("@Gold", mail.Gold);
                para[5] = new SqlParameter("@IsExist", true);
                para[6] = new SqlParameter("@Money", mail.Money);
                para[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
                para[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
                para[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
                para[10] = new SqlParameter("@SenderID", mail.SenderID);
                para[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
                para[12] = new SqlParameter("@IfDelS", false);
                para[13] = new SqlParameter("@IsDelete", false);
                para[14] = new SqlParameter("@IsDelR", false);
                para[15] = new SqlParameter("@IsRead", false);
                para[16] = new SqlParameter("@SendTime", DateTime.Now);
                para[17] = new SqlParameter("@Result", SqlDbType.Int);
                para[17].Direction = ParameterDirection.ReturnValue;
                result = this.db.RunProcedure("SP_Admin_SendUserMoney", para);
                returnValue = (int)para[17].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public int SendMailAndItem(string title, string content, int UserID, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            MailInfo message = new MailInfo();
            message.Annex1 = "";
            message.Content = title;
            message.Gold = gold;
            message.Money = money;
            message.Receiver = "";
            message.ReceiverID = UserID;
            message.Sender = "Administrators";
            message.SenderID = 0;
            message.Title = content;
            ItemInfo userGoods = new ItemInfo(null);
            userGoods.AgilityCompose = AgilityCompose;
            userGoods.AttackCompose = AttackCompose;
            userGoods.BeginDate = DateTime.Now;
            userGoods.Color = "";
            userGoods.DefendCompose = DefendCompose;
            userGoods.IsDirty = false;
            userGoods.IsExist = true;
            userGoods.IsJudge = true;
            userGoods.LuckCompose = LuckCompose;
            userGoods.StrengthenLevel = StrengthenLevel;
            userGoods.TemplateID = templateID;
            userGoods.ValidDate = validDate;
            userGoods.Count = count;
            userGoods.IsBinds = isBinds;
            int returnValue = 1;
            this.SendMailAndItem(message, userGoods, ref returnValue);
            return returnValue;
        }
        public int SendMailAndItemByUserName(string title, string content, string userName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            PlayerInfo player = this.GetUserSingleByUserName(userName);
            if (player != null)
            {
                return this.SendMailAndItem(title, content, player.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
            }
            return 2;
        }
        public int SendMailAndItemByNickName(string title, string content, string NickName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
        {
            PlayerInfo player = this.GetUserSingleByNickName(NickName);
            if (player != null)
            {
                return this.SendMailAndItem(title, content, player.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
            }
            return 2;
        }
        public int SendMailAndItem(string title, string content, int userID, int gold, int money, string param)
        {
            int returnValue = 1;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Title", title),
					new SqlParameter("@Content", content),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Gold", gold),
					new SqlParameter("@Money", money),
					new SqlParameter("@GiftToken", SqlDbType.BigInt),
					new SqlParameter("@Param", param),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[7].Direction = ParameterDirection.ReturnValue;
                bool result = this.db.RunProcedure("SP_Admin_SendAllItem", para);
                returnValue = (int)para[7].Value;
                result = (returnValue == 0);
                if (result)
                {
                    using (CenterServiceClient client = new CenterServiceClient())
                    {
                        client.MailNotice(userID);
                    }
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return returnValue;
        }
        public int SendMailAndItemByUserName(string title, string content, string userName, int gold, int money, string param)
        {
            PlayerInfo player = this.GetUserSingleByUserName(userName);
            if (player != null)
            {
                return this.SendMailAndItem(title, content, player.ID, gold, money, param);
            }
            return 2;
        }
        public int SendMailAndItemByNickName(string title, string content, string nickName, int gold, int money, string param)
        {
            PlayerInfo player = this.GetUserSingleByNickName(nickName);
            if (player != null)
            {
                return this.SendMailAndItem(title, content, player.ID, gold, money, param);
            }
            return 2;
        }
        public Dictionary<int, int> GetFriendsIDAll(int UserID)
        {
            Dictionary<int, int> info = new Dictionary<int, int>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_Friends_All", para);
                while (reader.Read())
                {
                    if (!info.ContainsKey((int)reader["FriendID"]))
                    {
                        info.Add((int)reader["FriendID"], (int)reader["Relation"]);
                    }
                    else
                    {
                        info[(int)reader["FriendID"]] = (int)reader["Relation"];
                    }
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return info;
        }
        public bool AddFriends(FriendInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@AddDate", DateTime.Now),
					new SqlParameter("@FriendID", info.FriendID),
					new SqlParameter("@IsExist", true),
					new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark),
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@Relation", info.Relation)
				};
                result = this.db.RunProcedure("SP_Users_Friends_Add", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool DeleteFriends(int UserID, int FriendID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", FriendID),
					new SqlParameter("@UserID", UserID)
				};
                result = this.db.RunProcedure("SP_Users_Friends_Delete", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public FriendInfo[] GetFriendsAll(int UserID)
        {
            List<FriendInfo> infos = new List<FriendInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_Friends", para);
                while (reader.Read())
                {
                    infos.Add(new FriendInfo
                    {
                        AddDate = (DateTime)reader["AddDate"],
                        Colors = (reader["Colors"] == null) ? "" : reader["Colors"].ToString(),
                        FriendID = (int)reader["FriendID"],
                        Grade = (int)reader["Grade"],
                        Hide = (int)reader["Hide"],
                        ID = (int)reader["ID"],
                        IsExist = (bool)reader["IsExist"],
                        NickName = (reader["NickName"] == null) ? "" : reader["NickName"].ToString(),
                        Remark = (reader["Remark"] == null) ? "" : reader["Remark"].ToString(),
                        Sex = ((bool)reader["Sex"]) ? 1 : 0,
                        State = (int)reader["State"],
                        Style = (reader["Style"] == null) ? "" : reader["Style"].ToString(),
                        UserID = (int)reader["UserID"],
                        ConsortiaName = (reader["ConsortiaName"] == null) ? "" : reader["ConsortiaName"].ToString(),
                        Offer = (int)reader["Offer"],
                        Win = (int)reader["Win"],
                        Total = (int)reader["Total"],
                        Escape = (int)reader["Escape"],
                        Relation = (int)reader["Relation"],
                        Repute = (int)reader["Repute"],
                        UserName = (reader["UserName"] == null) ? "" : reader["UserName"].ToString(),
                        DutyName = (reader["DutyName"] == null) ? "" : reader["DutyName"].ToString(),
                        Nimbus = (int)reader["Nimbus"],
                        typeVIP = Convert.ToByte(reader["typeVIP"]),
                        VIPLevel = (int)reader["VIPLevel"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public ArrayList GetFriendsGood(string UserName)
        {
            ArrayList friends = new ArrayList();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserName", SqlDbType.VarChar)
				};
                para[0].Value = UserName;
                this.db.GetReader(ref reader, "SP_Users_Friends_Good", para);
                while (reader.Read())
                {
                    friends.Add((reader["UserName"] == null) ? "" : reader["UserName"].ToString());
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return friends;
        }
        public FriendInfo[] GetFriendsBbs(string condictArray)
        {
            List<FriendInfo> infos = new List<FriendInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@SearchUserName", SqlDbType.NVarChar, 4000)
				};
                para[0].Value = condictArray;
                this.db.GetReader(ref reader, "SP_Users_FriendsBbs", para);
                while (reader.Read())
                {
                    infos.Add(new FriendInfo
                    {
                        NickName = (reader["NickName"] == null) ? "" : reader["NickName"].ToString(),
                        UserID = (int)reader["UserID"],
                        UserName = (reader["UserName"] == null) ? "" : reader["UserName"].ToString(),
                        IsExist = (int)reader["UserID"] > 0
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public QuestDataInfo[] GetUserQuest(int userID)
        {
            List<QuestDataInfo> infos = new List<QuestDataInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = userID;
                this.db.GetReader(ref reader, "SP_QuestData_All", para);
                while (reader.Read())
                {
                    infos.Add(new QuestDataInfo
                    {
                        CompletedDate = (DateTime)reader["CompletedDate"],
                        IsComplete = (bool)reader["IsComplete"],
                        Condition1 = (int)reader["Condition1"],
                        Condition2 = (int)reader["Condition2"],
                        Condition3 = (int)reader["Condition3"],
                        Condition4 = (int)reader["Condition4"],
                        QuestID = (int)reader["QuestID"],
                        UserID = (int)reader["UserId"],
                        IsExist = (bool)reader["IsExist"],
                        RandDobule = (int)reader["RandDobule"],
                        RepeatFinish = (int)reader["RepeatFinish"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public bool UpdateDbQuestDataInfo(QuestDataInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@QuestID", info.QuestID),
					new SqlParameter("@CompletedDate", info.CompletedDate),
					new SqlParameter("@IsComplete", info.IsComplete),
					new SqlParameter("@Condition1", info.Condition1),
					new SqlParameter("@Condition2", info.Condition2),
					new SqlParameter("@Condition3", info.Condition3),
					new SqlParameter("@Condition4", info.Condition4),
					new SqlParameter("@IsExist", info.IsExist),
					new SqlParameter("@RepeatFinish", info.RepeatFinish),
					new SqlParameter("@RandDobule", info.RandDobule)
				};
                result = this.db.RunProcedure("SP_QuestData_Add", para);
                info.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public BufferInfo[] GetUserBuffer(int userID)
        {
            List<BufferInfo> infos = new List<BufferInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = userID;
                this.db.GetReader(ref reader, "SP_User_Buff_All", para);
                while (reader.Read())
                {
                    infos.Add(new BufferInfo
                    {
                        BeginDate = (DateTime)reader["BeginDate"],
                        Data = (reader["Data"] == null) ? "" : reader["Data"].ToString(),
                        Type = (int)reader["Type"],
                        UserID = (int)reader["UserID"],
                        ValidDate = (int)reader["ValidDate"],
                        Value = (int)reader["Value"],
                        IsExist = (bool)reader["IsExist"],
                        ValidCount = (int)reader["ValidCount"],
                        IsDirty = false
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public bool SaveBuffer(BufferInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@Type", info.Type),
					new SqlParameter("@BeginDate", info.BeginDate),
					new SqlParameter("@Data", (info.Data == null) ? "" : info.Data),
					new SqlParameter("@IsExist", info.IsExist),
					new SqlParameter("@ValidDate", info.ValidDate),
					new SqlParameter("@Value", info.Value),
					new SqlParameter("@Value", info.ValidCount)
				};
                result = this.db.RunProcedure("SP_User_Buff_Add", para);
                info.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool AddChargeMoney(string chargeID, string userName, int money, string payWay, decimal needMoney, out int userID, ref int isResult, DateTime date, string IP, string nickName)
        {
            bool result = false;
            userID = 0;
            try
            {
                SqlParameter[] para = new SqlParameter[10];
                para[0] = new SqlParameter("@ChargeID", chargeID);
                para[1] = new SqlParameter("@UserName", userName);
                para[2] = new SqlParameter("@Money", money);
                para[3] = new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss"));
                para[4] = new SqlParameter("@PayWay", payWay);
                para[5] = new SqlParameter("@NeedMoney", needMoney);
                para[6] = new SqlParameter("@UserID", userID);
                para[6].Direction = ParameterDirection.InputOutput;
                para[7] = new SqlParameter("@Result", SqlDbType.Int);
                para[7].Direction = ParameterDirection.ReturnValue;
                para[8] = new SqlParameter("@IP", IP);
                para[9] = new SqlParameter("@NickName", nickName);
                result = this.db.RunProcedure("SP_Charge_Money_Add", para);
                userID = (int)para[6].Value;
                isResult = (int)para[7].Value;
                result = (isResult == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool ChargeToUser(string userName, ref int money, string nickName)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@UserName", userName);
                para[1] = new SqlParameter("@money", SqlDbType.Int);
                para[1].Direction = ParameterDirection.Output;
                para[2] = new SqlParameter("@NickName", nickName);
                result = this.db.RunProcedure("SP_Charge_To_User", para);
                money = (int)para[1].Value;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public ChargeRecordInfo[] GetChargeRecordInfo(DateTime date, int SaveRecordSecond)
        {
            List<ChargeRecordInfo> list = new List<ChargeRecordInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")),
					new SqlParameter("@Second", SaveRecordSecond)
				};
                this.db.GetReader(ref reader, "SP_Charge_Record", para);
                while (reader.Read())
                {
                    list.Add(new ChargeRecordInfo
                    {
                        BoyTotalPay = (int)reader["BoyTotalPay"],
                        GirlTotalPay = (int)reader["GirlTotalPay"],
                        PayWay = (reader["PayWay"] == null) ? "" : reader["PayWay"].ToString(),
                        TotalBoy = (int)reader["TotalBoy"],
                        TotalGirl = (int)reader["TotalGirl"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return list.ToArray();
        }
        public AuctionInfo GetAuctionSingle(int auctionID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@AuctionID", auctionID)
				};
                this.db.GetReader(ref reader, "SP_Auction_Single", para);
                if (reader.Read())
                {
                    return this.InitAuctionInfo(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public bool AddAuction(AuctionInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[18];
                para[0] = new SqlParameter("@AuctionID", info.AuctionID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@AuctioneerID", info.AuctioneerID);
                para[2] = new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName);
                para[3] = new SqlParameter("@BeginDate", info.BeginDate);
                para[4] = new SqlParameter("@BuyerID", info.BuyerID);
                para[5] = new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName);
                para[6] = new SqlParameter("@IsExist", info.IsExist);
                para[7] = new SqlParameter("@ItemID", info.ItemID);
                para[8] = new SqlParameter("@Mouthful", info.Mouthful);
                para[9] = new SqlParameter("@PayType", info.PayType);
                para[10] = new SqlParameter("@Price", info.Price);
                para[11] = new SqlParameter("@Rise", info.Rise);
                para[12] = new SqlParameter("@ValidDate", info.ValidDate);
                para[13] = new SqlParameter("@TemplateID", info.TemplateID);
                para[14] = new SqlParameter("Name", info.Name);
                para[15] = new SqlParameter("Category", info.Category);
                para[16] = new SqlParameter("Random", info.Random);
                para[17] = new SqlParameter("goodsCount", info.goodsCount);
                result = this.db.RunProcedure("SP_Auction_Add", para);
                info.AuctionID = (int)para[0].Value;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdateAuction(AuctionInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@AuctionID", info.AuctionID),
					new SqlParameter("@AuctioneerID", info.AuctioneerID),
					new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName),
					new SqlParameter("@BeginDate", info.BeginDate),
					new SqlParameter("@BuyerID", info.BuyerID),
					new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName),
					new SqlParameter("@IsExist", info.IsExist),
					new SqlParameter("@ItemID", info.ItemID),
					new SqlParameter("@Mouthful", info.Mouthful),
					new SqlParameter("@PayType", info.PayType),
					new SqlParameter("@Price", info.Price),
					new SqlParameter("@Rise", info.Rise),
					new SqlParameter("@ValidDate", info.ValidDate),
					new SqlParameter("Name", info.Name),
					new SqlParameter("Category", info.Category),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[15].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Auction_Update", para);
                int returnValue = (int)para[15].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool DeleteAuction(int auctionID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@AuctionID", auctionID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Auction_Delete", para);
                int returnValue = (int)para[2].Value;
                result = (returnValue == 0);
                switch (returnValue)
                {
                    case 0:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg1", new object[0]);
                        break;

                    case 1:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg2", new object[0]);
                        break;

                    case 2:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg3", new object[0]);
                        break;

                    default:
                        msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg4", new object[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public AuctionInfo[] GetAuctionPage(int page, string name, int type, int pay, ref int total, int userID, int buyID, int order, bool sort, int size, string AuctionIDs)
        {
            List<AuctionInfo> infos = new List<AuctionInfo>();
            try
            {
                string sWhere = " IsExist=1 ";
                if (!string.IsNullOrEmpty(name))
                {
                    sWhere = sWhere + " and Name like '%" + name + "%' ";
                }
                if (type != -1)
                {
                    switch (type)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                            {
                                object obj = sWhere;
                                sWhere = string.Concat(new object[]
							{
								obj,
								" and Category =",
								type,
								" "
							});
                                break;
                            }

                        case 18:
                        case 19:
                        case 20:
                            break;

                        case 21:
                            sWhere += " and Category in(1,2,5,8,9) ";
                            break;

                        case 22:
                            sWhere += " and Category in(13,15,6,4,3) ";
                            break;

                        case 23:
                            sWhere += " and Category in(16,11,10) ";
                            break;

                        case 24:
                            sWhere += " and Category in(8,9) ";
                            break;

                        case 25:
                            sWhere += " and Category in (7,17) ";
                            break;

                        case 26:
                            sWhere += " and TemplateId>=311000 and TemplateId<=313999";
                            break;

                        case 27:
                            sWhere += " and TemplateId>=311000 and TemplateId<=311999 ";
                            break;

                        case 28:
                            sWhere += " and TemplateId>=312000 and TemplateId<=312999 ";
                            break;

                        case 29:
                            sWhere += " and TemplateId>=313000 and TempLateId<=313999";
                            break;

                        default:
                            switch (type)
                            {
                                case 1100:
                                    sWhere += " and TemplateID in (11019,11021,11022,11023) ";
                                    break;

                                case 1101:
                                    sWhere += " and TemplateID='11019' ";
                                    break;

                                case 1102:
                                    sWhere += " and TemplateID='11021' ";
                                    break;

                                case 1103:
                                    sWhere += " and TemplateID='11022' ";
                                    break;

                                case 1104:
                                    sWhere += " and TemplateID='11023' ";
                                    break;

                                case 1105:
                                    sWhere += " and TemplateID in (11001,11002,11003,11004,11005,11006,11007,11008,11009,11010,11011,11012,11013,11014,11015,11016) ";
                                    break;

                                case 1106:
                                    sWhere += " and TemplateID in (11001,11002,11003,11004) ";
                                    break;

                                case 1107:
                                    sWhere += " and TemplateID in (11005,11006,11007,11008) ";
                                    break;

                                case 1108:
                                    sWhere += " and TemplateID in (11009,11010,11011,11012) ";
                                    break;

                                case 1109:
                                    sWhere += " and TemplateID in (11013,11014,11015,11016) ";
                                    break;
                            }
                            break;
                    }
                }
                if (pay != -1)
                {
                    object obj2 = sWhere;
                    sWhere = string.Concat(new object[]
					{
						obj2,
						" and PayType =",
						pay,
						" "
					});
                }
                if (userID != -1)
                {
                    object obj3 = sWhere;
                    sWhere = string.Concat(new object[]
					{
						obj3,
						" and AuctioneerID =",
						userID,
						" "
					});
                }
                if (buyID != -1)
                {
                    object obj4 = sWhere;
                    sWhere = string.Concat(new object[]
					{
						obj4,
						" and (BuyerID =",
						buyID,
						" or AuctionID in (",
						AuctionIDs,
						")) "
					});
                }
                string sOrder = "Category,Name,Price,dd,AuctioneerID";
                switch (order)
                {
                    case 0:
                        sOrder = "Name";
                        break;

                    case 2:
                        sOrder = "dd";
                        break;

                    case 3:
                        sOrder = "AuctioneerName";
                        break;

                    case 4:
                        sOrder = "Price";
                        break;

                    case 5:
                        sOrder = "BuyerName";
                        break;
                }
                sOrder += (sort ? " desc" : "");
                sOrder += ",AuctionID ";
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@QueryStr", "V_Auction_Scan"),
					new SqlParameter("@QueryWhere", sWhere),
					new SqlParameter("@PageSize", size),
					new SqlParameter("@PageCurrent", page),
					new SqlParameter("@FdShow", "*"),
					new SqlParameter("@FdOrder", sOrder),
					new SqlParameter("@FdKey", "AuctionID"),
					new SqlParameter("@TotalRow", total)
				};
                para[7].Direction = ParameterDirection.Output;
                DataTable dt = this.db.GetDataTable("Auction", "SP_CustomPage", para);
                total = (int)para[7].Value;
                foreach (DataRow dr in dt.Rows)
                {
                    infos.Add(new AuctionInfo
                    {
                        AuctioneerID = (int)dr["AuctioneerID"],
                        AuctioneerName = dr["AuctioneerName"].ToString(),
                        AuctionID = (int)dr["AuctionID"],
                        BeginDate = (DateTime)dr["BeginDate"],
                        BuyerID = (int)dr["BuyerID"],
                        BuyerName = dr["BuyerName"].ToString(),
                        Category = (int)dr["Category"],
                        IsExist = (bool)dr["IsExist"],
                        ItemID = (int)dr["ItemID"],
                        Name = dr["Name"].ToString(),
                        Mouthful = (int)dr["Mouthful"],
                        PayType = (int)dr["PayType"],
                        Price = (int)dr["Price"],
                        Rise = (int)dr["Rise"],
                        ValidDate = (int)dr["ValidDate"],
                        goodsCount = (int)dr["dd"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return infos.ToArray();
        }
        public AuctionInfo InitAuctionInfo(SqlDataReader reader)
        {
            return new AuctionInfo
            {
                AuctioneerID = (int)reader["AuctioneerID"],
                AuctioneerName = (reader["AuctioneerName"] == null) ? "" : reader["AuctioneerName"].ToString(),
                AuctionID = (int)reader["AuctionID"],
                BeginDate = (DateTime)reader["BeginDate"],
                BuyerID = (int)reader["BuyerID"],
                BuyerName = (reader["BuyerName"] == null) ? "" : reader["BuyerName"].ToString(),
                IsExist = (bool)reader["IsExist"],
                ItemID = (int)reader["ItemID"],
                Mouthful = (int)reader["Mouthful"],
                PayType = (int)reader["PayType"],
                Price = (int)reader["Price"],
                Rise = (int)reader["Rise"],
                ValidDate = (int)reader["ValidDate"],
                Name = reader["Name"].ToString(),
                Category = (int)reader["Category"],
                goodsCount = (int)reader["goodsCount"]
            };
        }
        public bool ScanAuction(ref string noticeUserID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
				};
                para[0].Direction = ParameterDirection.Output;
                this.db.RunProcedure("SP_Auction_Scan", para);
                noticeUserID = para[0].Value.ToString();
                result = true;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool AddMarryInfo(MarryInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[5];
                para[0] = new SqlParameter("@ID", info.ID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@UserID", info.UserID);
                para[2] = new SqlParameter("@IsPublishEquip", info.IsPublishEquip);
                para[3] = new SqlParameter("@Introduction", info.Introduction);
                para[4] = new SqlParameter("@RegistTime", info.RegistTime);
                result = this.db.RunProcedure("SP_MarryInfo_Add", para);
                info.ID = (int)para[0].Value;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("AddMarryInfo", e);
                }
            }
            return result;
        }
        public bool DeleteMarryInfo(int ID, int userID, ref string msg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", ID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_MarryInfo_Delete", para);
                int returnValue = (int)para[2].Value;
                result = (returnValue == 0);
                if (returnValue == 0)
                {
                    msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Succeed", new object[0]);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("DeleteAuction", e);
                }
            }
            return result;
        }
        public MarryInfo GetMarryInfoSingle(int ID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", ID)
				};
                this.db.GetReader(ref reader, "SP_MarryInfo_Single", para);
                if (reader.Read())
                {
                    return new MarryInfo
                    {
                        ID = (int)reader["ID"],
                        UserID = (int)reader["UserID"],
                        IsPublishEquip = (bool)reader["IsPublishEquip"],
                        Introduction = reader["Introduction"].ToString(),
                        RegistTime = (DateTime)reader["RegistTime"]
                    };
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetMarryInfoSingle", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public bool UpdateMarryInfo(MarryInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@IsPublishEquip", info.IsPublishEquip),
					new SqlParameter("@Introduction", info.Introduction),
					new SqlParameter("@RegistTime", info.RegistTime),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_MarryInfo_Update", para);
                int returnValue = (int)para[5].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public MarryInfo[] GetMarryInfoPage(int page, string name, bool sex, int size, ref int total)
        {
            List<MarryInfo> infos = new List<MarryInfo>();
            try
            {
                string sWhere;
                if (sex)
                {
                    sWhere = " IsExist=1 and Sex=1 and UserExist=1";
                }
                else
                {
                    sWhere = " IsExist=1 and Sex=0 and UserExist=1";
                }
                if (!string.IsNullOrEmpty(name))
                {
                    sWhere = sWhere + " and NickName like '%" + name + "%' ";
                }
                string sOrder = "State desc,IsMarried";
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@QueryStr", "V_Sys_Marry_Info"),
					new SqlParameter("@QueryWhere", sWhere),
					new SqlParameter("@PageSize", size),
					new SqlParameter("@PageCurrent", page),
					new SqlParameter("@FdShow", "*"),
					new SqlParameter("@FdOrder", sOrder),
					new SqlParameter("@FdKey", "ID"),
					new SqlParameter("@TotalRow", total)
				};
                para[7].Direction = ParameterDirection.Output;
                DataTable dt = this.db.GetDataTable("V_Sys_Marry_Info", "SP_CustomPage", para);
                total = (int)para[7].Value;
                foreach (DataRow dr in dt.Rows)
                {
                    infos.Add(new MarryInfo
                    {
                        ID = (int)dr["ID"],
                        UserID = (int)dr["UserID"],
                        IsPublishEquip = (bool)dr["IsPublishEquip"],
                        Introduction = dr["Introduction"].ToString(),
                        NickName = dr["NickName"].ToString(),
                        IsConsortia = (bool)dr["IsConsortia"],
                        ConsortiaID = (int)dr["ConsortiaID"],
                        Sex = (bool)dr["Sex"],
                        Win = (int)dr["Win"],
                        Total = (int)dr["Total"],
                        Escape = (int)dr["Escape"],
                        GP = (int)dr["GP"],
                        Honor = dr["Honor"].ToString(),
                        Style = dr["Style"].ToString(),
                        Colors = dr["Colors"].ToString(),
                        Hide = (int)dr["Hide"],
                        Grade = (int)dr["Grade"],
                        State = (int)dr["State"],
                        Repute = (int)dr["Repute"],
                        Skin = dr["Skin"].ToString(),
                        Offer = (int)dr["Offer"],
                        IsMarried = (bool)dr["IsMarried"],
                        ConsortiaName = dr["ConsortiaName"].ToString(),
                        DutyName = dr["DutyName"].ToString(),
                        Nimbus = (int)dr["Nimbus"],
                        FightPower = (int)dr["FightPower"],
                        typeVIP = Convert.ToByte(dr["typeVIP"]),
                        VIPLevel = (int)dr["VIPLevel"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return infos.ToArray();
        }
        public bool InsertPlayerMarryApply(MarryApplyInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@ApplyUserID", info.ApplyUserID),
					new SqlParameter("@ApplyUserName", info.ApplyUserName),
					new SqlParameter("@ApplyType", info.ApplyType),
					new SqlParameter("@ApplyResult", info.ApplyResult),
					new SqlParameter("@LoveProclamation", info.LoveProclamation),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[6].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Marry_Apply", para);
                result = ((int)para[6].Value == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("InsertPlayerMarryApply", e);
                }
            }
            return result;
        }
        public bool UpdatePlayerMarryApply(int UserID, string loveProclamation, bool isExist)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", UserID),
					new SqlParameter("@LoveProclamation", loveProclamation),
					new SqlParameter("@isExist", isExist),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[3].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Marry_Apply", para);
                result = ((int)para[3].Value == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdatePlayerMarryApply", e);
                }
            }
            return result;
        }
        public MarryApplyInfo[] GetPlayerMarryApply(int UserID)
        {
            SqlDataReader reader = null;
            List<MarryApplyInfo> infos = new List<MarryApplyInfo>();
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", UserID)
				};
                this.db.GetReader(ref reader, "SP_Get_Marry_Apply", para);
                while (reader.Read())
                {
                    infos.Add(new MarryApplyInfo
                    {
                        UserID = (int)reader["UserID"],
                        ApplyUserID = (int)reader["ApplyUserID"],
                        ApplyUserName = reader["ApplyUserName"].ToString(),
                        ApplyType = (int)reader["ApplyType"],
                        ApplyResult = (bool)reader["ApplyResult"],
                        LoveProclamation = reader["LoveProclamation"].ToString(),
                        ID = (int)reader["Id"]
                    });
                }
                return infos.ToArray();
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetPlayerMarryApply", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public bool InsertMarryRoomInfo(MarryRoomInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[20];
                para[0] = new SqlParameter("@ID", info.ID);
                para[0].Direction = ParameterDirection.InputOutput;
                para[1] = new SqlParameter("@Name", info.Name);
                para[2] = new SqlParameter("@PlayerID", info.PlayerID);
                para[3] = new SqlParameter("@PlayerName", info.PlayerName);
                para[4] = new SqlParameter("@GroomID", info.GroomID);
                para[5] = new SqlParameter("@GroomName", info.GroomName);
                para[6] = new SqlParameter("@BrideID", info.BrideID);
                para[7] = new SqlParameter("@BrideName", info.BrideName);
                para[8] = new SqlParameter("@Pwd", info.Pwd);
                para[9] = new SqlParameter("@AvailTime", info.AvailTime);
                para[10] = new SqlParameter("@MaxCount", info.MaxCount);
                para[11] = new SqlParameter("@GuestInvite", info.GuestInvite);
                para[12] = new SqlParameter("@MapIndex", info.MapIndex);
                para[13] = new SqlParameter("@BeginTime", info.BeginTime);
                para[14] = new SqlParameter("@BreakTime", info.BreakTime);
                para[15] = new SqlParameter("@RoomIntroduction", info.RoomIntroduction);
                para[16] = new SqlParameter("@ServerID", info.ServerID);
                para[17] = new SqlParameter("@IsHymeneal", info.IsHymeneal);
                para[18] = new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed);
                para[19] = new SqlParameter("@Result", SqlDbType.Int);
                para[19].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Marry_Room_Info", para);
                result = ((int)para[19].Value == 0);
                if (result)
                {
                    info.ID = (int)para[0].Value;
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("InsertMarryRoomInfo", e);
                }
            }
            return result;
        }
        public bool UpdateMarryRoomInfo(MarryRoomInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@AvailTime", info.AvailTime),
					new SqlParameter("@BreakTime", info.BreakTime),
					new SqlParameter("@roomIntroduction", info.RoomIntroduction),
					new SqlParameter("@isHymeneal", info.IsHymeneal),
					new SqlParameter("@Name", info.Name),
					new SqlParameter("@Pwd", info.Pwd),
					new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[8].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Marry_Room_Info", para);
                result = ((int)para[8].Value == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdateMarryRoomInfo", e);
                }
            }
            return result;
        }
        public bool DisposeMarryRoomInfo(int ID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", ID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Dispose_Marry_Room_Info", para);
                result = ((int)para[1].Value == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("DisposeMarryRoomInfo", e);
                }
            }
            return result;
        }
        public MarryRoomInfo[] GetMarryRoomInfo()
        {
            SqlDataReader reader = null;
            List<MarryRoomInfo> infos = new List<MarryRoomInfo>();
            try
            {
                this.db.GetReader(ref reader, "SP_Get_Marry_Room_Info");
                while (reader.Read())
                {
                    infos.Add(new MarryRoomInfo
                    {
                        ID = (int)reader["ID"],
                        Name = reader["Name"].ToString(),
                        PlayerID = (int)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        GroomID = (int)reader["GroomID"],
                        GroomName = reader["GroomName"].ToString(),
                        BrideID = (int)reader["BrideID"],
                        BrideName = reader["BrideName"].ToString(),
                        Pwd = reader["Pwd"].ToString(),
                        AvailTime = (int)reader["AvailTime"],
                        MaxCount = (int)reader["MaxCount"],
                        GuestInvite = (bool)reader["GuestInvite"],
                        MapIndex = (int)reader["MapIndex"],
                        BeginTime = (DateTime)reader["BeginTime"],
                        BreakTime = (DateTime)reader["BreakTime"],
                        RoomIntroduction = reader["RoomIntroduction"].ToString(),
                        ServerID = (int)reader["ServerID"],
                        IsHymeneal = (bool)reader["IsHymeneal"],
                        IsGunsaluteUsed = (bool)reader["IsGunsaluteUsed"]
                    });
                }
                return infos.ToArray();
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetMarryRoomInfo", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public MarryRoomInfo GetMarryRoomInfoSingle(int id)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", id)
				};
                this.db.GetReader(ref reader, "SP_Get_Marry_Room_Info_Single", para);
                if (reader.Read())
                {
                    return new MarryRoomInfo
                    {
                        ID = (int)reader["ID"],
                        Name = reader["Name"].ToString(),
                        PlayerID = (int)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        GroomID = (int)reader["GroomID"],
                        GroomName = reader["GroomName"].ToString(),
                        BrideID = (int)reader["BrideID"],
                        BrideName = reader["BrideName"].ToString(),
                        Pwd = reader["Pwd"].ToString(),
                        AvailTime = (int)reader["AvailTime"],
                        MaxCount = (int)reader["MaxCount"],
                        GuestInvite = (bool)reader["GuestInvite"],
                        MapIndex = (int)reader["MapIndex"],
                        BeginTime = (DateTime)reader["BeginTime"],
                        BreakTime = (DateTime)reader["BreakTime"],
                        RoomIntroduction = reader["RoomIntroduction"].ToString(),
                        ServerID = (int)reader["ServerID"],
                        IsHymeneal = (bool)reader["IsHymeneal"],
                        IsGunsaluteUsed = (bool)reader["IsGunsaluteUsed"]
                    };
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetMarryRoomInfo", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public bool UpdateBreakTimeWhereServerStop()
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[0].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_Marry_Room_Info_Sever_Stop", para);
                result = ((int)para[0].Value == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdateBreakTimeWhereServerStop", e);
                }
            }
            return result;
        }
        public MarryProp GetMarryProp(int id)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", id)
				};
                this.db.GetReader(ref reader, "SP_Select_Marry_Prop", para);
                if (reader.Read())
                {
                    return new MarryProp
                    {
                        IsMarried = (bool)reader["IsMarried"],
                        SpouseID = (int)reader["SpouseID"],
                        SpouseName = reader["SpouseName"].ToString(),
                        IsCreatedMarryRoom = (bool)reader["IsCreatedMarryRoom"],
                        SelfMarryRoomID = (int)reader["SelfMarryRoomID"],
                        IsGotRing = (bool)reader["IsGotRing"]
                    };
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetMarryProp", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public bool SavePlayerMarryNotice(MarryApplyInfo info, int answerId, ref int id)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[9];
                para[0] = new SqlParameter("@UserID", info.UserID);
                para[1] = new SqlParameter("@ApplyUserID", info.ApplyUserID);
                para[2] = new SqlParameter("@ApplyUserName", info.ApplyUserName);
                para[3] = new SqlParameter("@ApplyType", info.ApplyType);
                para[4] = new SqlParameter("@ApplyResult", info.ApplyResult);
                para[5] = new SqlParameter("@LoveProclamation", info.LoveProclamation);
                para[6] = new SqlParameter("@AnswerId", answerId);
                para[7] = new SqlParameter("@ouototal", SqlDbType.Int);
                para[7].Direction = ParameterDirection.Output;
                para[8] = new SqlParameter("@Result", SqlDbType.Int);
                para[8].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Insert_Marry_Notice", para);
                id = (int)para[7].Value;
                result = ((int)para[8].Value == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SavePlayerMarryNotice", e);
                }
            }
            return result;
        }
        public bool UpdatePlayerGotRingProp(int groomID, int brideID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@GroomID", groomID),
					new SqlParameter("@BrideID", brideID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_GotRing_Prop", para);
                result = ((int)para[2].Value == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdatePlayerGotRingProp", e);
                }
            }
            return result;
        }
        public HotSpringRoomInfo[] GetHotSpringRoomInfo()
        {
            SqlDataReader resultDataReader = null;
            List<HotSpringRoomInfo> list = new List<HotSpringRoomInfo>();
            try
            {
                this.db.GetReader(ref resultDataReader, "SP_Get_HotSpring_Room");
                while (resultDataReader.Read())
                {
                    HotSpringRoomInfo item = new HotSpringRoomInfo
                    {
                        RoomID = (int)resultDataReader["RoomID"],
                        RoomName = (resultDataReader["RoomName"] == null) ? "" : resultDataReader["RoomName"].ToString(),
                        PlayerID = (int)resultDataReader["PlayerID"],
                        PlayerName = (resultDataReader["PlayerName"] == null) ? "" : resultDataReader["PlayerName"].ToString(),
                        Pwd = (resultDataReader["Pwd"].ToString() == null) ? "" : resultDataReader["Pwd"].ToString(),
                        AvailTime = (int)resultDataReader["AvailTime"],
                        MaxCount = (int)resultDataReader["MaxCount"],
                        BeginTime = (DateTime)resultDataReader["BeginTime"],
                        BreakTime = (DateTime)resultDataReader["BreakTime"],
                        RoomIntroduction = (resultDataReader["RoomIntroduction"] == null) ? "" : resultDataReader["RoomIntroduction"].ToString(),
                        RoomType = (int)resultDataReader["RoomType"],
                        ServerID = (int)resultDataReader["ServerID"],
                        RoomNumber = (int)resultDataReader["RoomNumber"]
                    };
                    list.Add(item);
                }
                return list.ToArray();
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("HotSpringRoomInfo", exception);
                }
            }
            finally
            {
                if (resultDataReader != null && !resultDataReader.IsClosed)
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }
        public HotSpringRoomInfo GetHotSpringRoomInfoSingle(int id)
        {
            SqlDataReader resultDataReader = null;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@RoomID", id)
				};
                this.db.GetReader(ref resultDataReader, "SP_Get_HotSpringRoomInfo_Single", sqlParameters);
                if (resultDataReader.Read())
                {
                    return new HotSpringRoomInfo
                    {
                        RoomID = (int)resultDataReader["RoomID"],
                        RoomName = resultDataReader["RoomName"].ToString(),
                        PlayerID = (int)resultDataReader["PlayerID"],
                        PlayerName = resultDataReader["PlayerName"].ToString(),
                        Pwd = resultDataReader["Pwd"].ToString(),
                        AvailTime = (int)resultDataReader["AvailTime"],
                        MaxCount = (int)resultDataReader["MaxCount"],
                        MapIndex = (int)resultDataReader["MapIndex"],
                        BeginTime = (DateTime)resultDataReader["BeginTime"],
                        BreakTime = (DateTime)resultDataReader["BreakTime"],
                        RoomIntroduction = resultDataReader["RoomIntroduction"].ToString(),
                        RoomType = (int)resultDataReader["RoomType"],
                        ServerID = (int)resultDataReader["ServerID"],
                        RoomNumber = (int)resultDataReader["RoomNumber"]
                    };
                }
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("HotSpringRoomInfo", exception);
                }
            }
            finally
            {
                if (resultDataReader != null && !resultDataReader.IsClosed)
                {
                    resultDataReader.Close();
                }
            }
            return null;
        }
        public bool UpdateHotSpringRoomInfo(HotSpringRoomInfo info)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@RoomID", info.RoomID),
					new SqlParameter("@RoomName", info.RoomName),
					new SqlParameter("@Pwd", info.Pwd),
					new SqlParameter("@AvailTime", info.AvailTime.ToString()),
					new SqlParameter("@BreakTime", info.BreakTime.ToString()),
					new SqlParameter("@roomIntroduction", info.RoomIntroduction),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                sqlParameters[6].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_HotSpringRoomInfo", sqlParameters);
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("UpdateHotSpringRoomInfo", exception);
                }
            }
            return flag;
        }
        public bool UpdateLastVIPPackTime(int ID)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@UserID", ID),
					new SqlParameter("@LastVIPPackTime", DateTime.Now.Date),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                sqlParameters[2].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateUserLastVIPPackTime", sqlParameters);
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateUserLastVIPPackTime", exception);
                }
            }
            return flag;
        }
        public bool UpdateVIPInfo(PlayerInfo p)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", p.ID),
					new SqlParameter("@VIPLevel", p.VIPLevel),
					new SqlParameter("@VIPExp", p.VIPExp),
					new SqlParameter("@VIPOnlineDays", SqlDbType.BigInt),
					new SqlParameter("@VIPOfflineDays", SqlDbType.BigInt),
					new SqlParameter("@VIPExpireDay", p.VIPExpireDay.ToString()),
					new SqlParameter("@VIPLastDate", DateTime.Now),
					new SqlParameter("@VIPNextLevelDaysNeeded", SqlDbType.BigInt),
					new SqlParameter("@CanTakeVipReward", p.CanTakeVipReward),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                sqlParameters[9].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateVIPInfo", sqlParameters);
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateVIPInfo", exception);
                }
            }
            return flag;
        }
        public int VIPRenewal(string nickName, int renewalDays, ref DateTime ExpireDayOut)
        {
            int num = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@NickName", nickName),
					new SqlParameter("@RenewalDays", renewalDays),
					new SqlParameter("@ExpireDayOut", DateTime.Now),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                sqlParameters[2].Direction = ParameterDirection.Output;
                sqlParameters[3].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_VIPRenewal_Single", sqlParameters);
                ExpireDayOut = (DateTime)sqlParameters[2].Value;
                num = (int)sqlParameters[3].Value;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_VIPRenewal_Single", exception);
                }
            }
            return num;
        }
        public int VIPLastdate(int ID)
        {
            int num = 0;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@UserID", ID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                sqlParameters[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_VIPLastdate_Single", sqlParameters);
                num = (int)sqlParameters[1].Value;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_VIPLastdate_Single", exception);
                }
            }
            return num;
        }
        public bool Test(string DutyName)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@DutyName", DutyName)
				};
                result = this.db.RunProcedure("SP_Test1", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool TankAll()
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[0];
                result = this.db.RunProcedure("SP_Tank_All", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool RegisterUser(string UserName, string NickName, string Password, bool Sex, int Money, int GiftToken, int Gold)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserName", UserName),
					new SqlParameter("@Password", Password),
					new SqlParameter("@NickName", NickName),
					new SqlParameter("@Sex", Sex),
					new SqlParameter("@Money", Money),
					new SqlParameter("@GiftToken", GiftToken),
					new SqlParameter("@Gold", Gold),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[7].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Account_Register", para);
                if ((int)para[7].Value == 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init Register", e);
                }
            }
            return result;
        }
        public bool CheckEmailIsValid(string Email)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Email", Email),
					new SqlParameter("@count", SqlDbType.BigInt)
				};
                para[1].Direction = ParameterDirection.Output;
                this.db.RunProcedure("CheckEmailIsValid", para);
                if (int.Parse(para[1].Value.ToString()) == 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init CheckEmailIsValid", e);
                }
            }
            return result;
        }
        public bool RegisterUserInfo(UserInfo userinfo)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", userinfo.UserID),
					new SqlParameter("@UserEmail", userinfo.UserEmail),
					new SqlParameter("@UserPhone", (userinfo.UserPhone == null) ? string.Empty : userinfo.UserPhone),
					new SqlParameter("@UserOther1", (userinfo.UserOther1 == null) ? string.Empty : userinfo.UserOther1),
					new SqlParameter("@UserOther2", (userinfo.UserOther2 == null) ? string.Empty : userinfo.UserOther2),
					new SqlParameter("@UserOther3", (userinfo.UserOther3 == null) ? string.Empty : userinfo.UserOther3)
				};
                result = this.db.RunProcedure("[SP_User_Info_Add]", para);
                return result;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public UserInfo GetUserInfo(int UserId)
        {
            SqlDataReader reader = null;
            UserInfo user = new UserInfo
            {
                UserID = UserId
            };
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", UserId)
				};
                this.db.GetReader(ref reader, "SP_Get_User_Info", para);
                while (reader.Read())
                {
                    user.UserID = int.Parse(reader["UserID"].ToString());
                    user.UserEmail = ((reader["UserEmail"] == null) ? "" : reader["UserEmail"].ToString());
                    user.UserPhone = ((reader["UserPhone"] == null) ? "" : reader["UserPhone"].ToString());
                    user.UserOther1 = ((reader["UserOther1"] == null) ? "" : reader["UserOther1"].ToString());
                    user.UserOther2 = ((reader["UserOther2"] == null) ? "" : reader["UserOther2"].ToString());
                    user.UserOther3 = ((reader["UserOther3"] == null) ? "" : reader["UserOther3"].ToString());
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return user;
        }
        public LevelInfo[] GetAllLevel()
        {
            List<LevelInfo> infos = new List<LevelInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_Level_All");
                while (reader.Read())
                {
                    infos.Add(new LevelInfo
                    {
                        Grade = (int)reader["Grade"],
                        GP = (int)reader["GP"],
                        Blood = (int)reader["Blood"],
                        SpaGP = (int)reader["SpaGP"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllLevel", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public ExerciseInfo[] GetAllExercise()
        {
            List<ExerciseInfo> infos = new List<ExerciseInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_Exercise_All");
                while (reader.Read())
                {
                    infos.Add(new ExerciseInfo
                    {
                        Grage = (int)reader["Grage"],
                        GP = (int)reader["GP"],
                        ExerciseA = (int)reader["ExerciseA"],
                        ExerciseAG = (int)reader["ExerciseAG"],
                        ExerciseD = (int)reader["ExerciseD"],
                        ExerciseH = (int)reader["ExerciseH"],
                        ExerciseL = (int)reader["ExerciseL"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllExercise", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public LevelInfo GetUserLevelSingle(int Grade)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Grade", Grade)
				};
                this.db.GetReader(ref reader, "SP_Get_Level_By_Grade", para);
                if (reader.Read())
                {
                    return new LevelInfo
                    {
                        Grade = (int)reader["Grade"],
                        GP = (int)reader["GP"],
                        Blood = (int)reader["Blood"],
                        SpaGP = (int)reader["SpaGP"]
                    };
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetLevelInfoSingle", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public ExerciseInfo GetExerciseSingle(int Grade)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Grage", Grade)
				};
                this.db.GetReader(ref reader, "SP_Get_Exercise_By_Grade", para);
                if (reader.Read())
                {
                    return new ExerciseInfo
                    {
                        Grage = (int)reader["Grage"],
                        GP = (int)reader["GP"],
                        ExerciseA = (int)reader["ExerciseA"],
                        ExerciseAG = (int)reader["ExerciseAG"],
                        ExerciseD = (int)reader["ExerciseD"],
                        ExerciseH = (int)reader["ExerciseH"],
                        ExerciseL = (int)reader["ExerciseL"]
                    };
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetExerciseInfoSingle", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public TexpInfo GetUserTexpInfoSingle(int ID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", ID)
				};
                this.db.GetReader(ref reader, "SP_Get_UserTexp_By_ID", para);
                if (reader.Read())
                {
                    return new TexpInfo
                    {
                        UserID = (int)reader["UserID"],
                        attTexpExp = (int)reader["attTexpExp"],
                        defTexpExp = (int)reader["defTexpExp"],
                        hpTexpExp = (int)reader["hpTexpExp"],
                        lukTexpExp = (int)reader["lukTexpExp"],
                        spdTexpExp = (int)reader["spdTexpExp"],
                        texpCount = (int)reader["texpCount"],
                        texpTaskCount = (int)reader["texpTaskCount"],
                        texpTaskDate = (DateTime)reader["texpTaskDate"]
                    };
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetTexpInfoSingle", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public bool UpdateUserTexpInfo(TexpInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@attTexpExp", info.attTexpExp),
					new SqlParameter("@defTexpExp", info.defTexpExp),
					new SqlParameter("@hpTexpExp", info.hpTexpExp),
					new SqlParameter("@lukTexpExp", info.lukTexpExp),
					new SqlParameter("@spdTexpExp", info.spdTexpExp),
					new SqlParameter("@texpCount", info.texpCount),
					new SqlParameter("@texpTaskCount", info.texpTaskCount),
					new SqlParameter("@texpTaskDate", info.texpTaskDate.ToString()),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[9].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserTexp_Update", para);
                int returnValue = (int)para[9].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool InsertUserTexpInfo(TexpInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@attTexpExp", info.attTexpExp),
					new SqlParameter("@defTexpExp", info.defTexpExp),
					new SqlParameter("@hpTexpExp", info.hpTexpExp),
					new SqlParameter("@lukTexpExp", info.lukTexpExp),
					new SqlParameter("@spdTexpExp", info.spdTexpExp),
					new SqlParameter("@texpCount", info.texpCount),
					new SqlParameter("@texpTaskCount", info.texpTaskCount),
					new SqlParameter("@texpTaskDate", info.texpTaskDate.ToString()),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[9].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserTexp_Add", para);
                result = ((int)para[9].Value == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("InsertTexpInfo", e);
                }
            }
            return result;
        }
        public bool SaveUserPet(UsersPetinfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@TemplateID", info.TemplateID),
					new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name),
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@Attack", info.Attack),
					new SqlParameter("@Defence", info.Defence),
					new SqlParameter("@Luck", info.Luck),
					new SqlParameter("@Agility", info.Agility),
					new SqlParameter("@Blood", info.Blood),
					new SqlParameter("@Damage", info.Damage),
					new SqlParameter("@Guard", info.Guard),
					new SqlParameter("@AttackGrow", info.AttackGrow),
					new SqlParameter("@DefenceGrow", info.DefenceGrow),
					new SqlParameter("@LuckGrow", info.LuckGrow),
					new SqlParameter("@AgilityGrow", info.AgilityGrow),
					new SqlParameter("@BloodGrow", info.BloodGrow),
					new SqlParameter("@DamageGrow", info.DamageGrow),
					new SqlParameter("@GuardGrow", info.GuardGrow),
					new SqlParameter("@Level", info.Level),
					new SqlParameter("@GP", info.GP),
					new SqlParameter("@MaxGP", info.MaxGP),
					new SqlParameter("@Hunger", info.Hunger),
					new SqlParameter("@PetHappyStar", info.PetHappyStar),
					new SqlParameter("@MP", info.MP),
					new SqlParameter("@IsEquip", info.IsEquip),
					new SqlParameter("@Place", info.Place),
					new SqlParameter("@IsExit", info.IsExit),
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@Skill", info.Skill),
					new SqlParameter("@SkillEquip", info.SkillEquip),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[29].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserPet_Update", para);
                int returnValue = (int)para[29].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool RemoveUserPet(UsersPetinfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@TemplateID", info.TemplateID),
					new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name),
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@Attack", info.Attack),
					new SqlParameter("@Defence", info.Defence),
					new SqlParameter("@Luck", info.Luck),
					new SqlParameter("@Agility", info.Agility),
					new SqlParameter("@Blood", info.Blood),
					new SqlParameter("@Damage", info.Damage),
					new SqlParameter("@Guard", info.Guard),
					new SqlParameter("@AttackGrow", info.AttackGrow),
					new SqlParameter("@DefenceGrow", info.DefenceGrow),
					new SqlParameter("@LuckGrow", info.LuckGrow),
					new SqlParameter("@AgilityGrow", info.AgilityGrow),
					new SqlParameter("@BloodGrow", info.BloodGrow),
					new SqlParameter("@DamageGrow", info.DamageGrow),
					new SqlParameter("@GuardGrow", info.GuardGrow),
					new SqlParameter("@Level", info.Level),
					new SqlParameter("@GP", info.GP),
					new SqlParameter("@MaxGP", info.MaxGP),
					new SqlParameter("@Hunger", info.Hunger),
					new SqlParameter("@PetHappyStar", info.PetHappyStar),
					new SqlParameter("@MP", info.MP),
					new SqlParameter("@IsEquip", info.IsEquip),
					new SqlParameter("@Place", info.Place),
					new SqlParameter("@IsExit", info.IsExit),
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[27].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserPet_Remove", para);
                int returnValue = (int)para[27].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public UsersPetinfo RemovePetSingle(int PetID)
        {
            new UsersPetinfo();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
                para[0].Value = PetID;
                this.db.GetReader(ref reader, "SP_Remove_User_Pet", para);
                if (reader.Read())
                {
                    return this.InitPet(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public UsersPetinfo GetAdoptPetSingle(int PetID)
        {
            new UsersPetinfo();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
                para[0].Value = PetID;
                this.db.GetReader(ref reader, "SP_AdoptPet_By_Id", para);
                if (reader.Read())
                {
                    return this.InitPet(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public UsersPetinfo[] GetUserAdoptPetSingles(int UserID)
        {
            List<UsersPetinfo> items = new List<UsersPetinfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Get_User_AdoptPetList", para);
                while (reader.Read())
                {
                    items.Add(this.InitPet(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items.ToArray();
        }
        public bool RemoveUserAdoptPet(int ID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", ID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Remove_User_AdoptPet", para);
                int returnValue = (int)para[1].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdateUserAdoptPet(int ID)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", ID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[1].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Update_User_AdoptPet", para);
                int returnValue = (int)para[1].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool AddUserAdoptPet(UsersPetinfo info, bool isUse)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@TemplateID", info.TemplateID),
					new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name),
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@Attack", info.Attack),
					new SqlParameter("@Defence", info.Defence),
					new SqlParameter("@Luck", info.Luck),
					new SqlParameter("@Agility", info.Agility),
					new SqlParameter("@Blood", info.Blood),
					new SqlParameter("@Damage", info.Damage),
					new SqlParameter("@Guard", info.Guard),
					new SqlParameter("@AttackGrow", info.AttackGrow),
					new SqlParameter("@DefenceGrow", info.DefenceGrow),
					new SqlParameter("@LuckGrow", info.LuckGrow),
					new SqlParameter("@AgilityGrow", info.AgilityGrow),
					new SqlParameter("@BloodGrow", info.BloodGrow),
					new SqlParameter("@DamageGrow", info.DamageGrow),
					new SqlParameter("@GuardGrow", info.GuardGrow),
					new SqlParameter("@Skill", info.Skill),
					new SqlParameter("@SkillEquip", info.SkillEquip),
					new SqlParameter("@Place", info.Place),
					new SqlParameter("@IsExit", info.IsExit),
					new SqlParameter("@IsUse", isUse),
					new SqlParameter("@ID", info.ID)
				};
                para[22].Direction = ParameterDirection.Output;
                result = this.db.RunProcedure("SP_User_AdoptPet", para);
                info.ID = (int)para[22].Value;
                info.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public UsersPetinfo[] GetUserPetSingles(int UserID)
        {
            List<UsersPetinfo> items = new List<UsersPetinfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Get_UserPet_By_ID", para);
                while (reader.Read())
                {
                    items.Add(this.InitPet(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items.ToArray();
        }
        public List<UsersPetinfo> GetUserPetIsExitSingles(int UserID)
        {
            List<UsersPetinfo> items = new List<UsersPetinfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Get_UserPet_By_IsExit", para);
                while (reader.Read())
                {
                    items.Add(this.InitPet(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items;
        }
        public UsersPetinfo GetUserPetSingle(int ID)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = ID;
                this.db.GetReader(ref reader, "SP_Get_UserPet_By_ID", para);
                if (reader.Read())
                {
                    return this.InitPet(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetPetInfoSingle", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public UsersPetinfo InitPet(SqlDataReader reader)
        {
            return new UsersPetinfo
            {
                ID = (int)reader["ID"],
                TemplateID = (int)reader["TemplateID"],
                Name = reader["Name"].ToString(),
                UserID = (int)reader["UserID"],
                Attack = (int)reader["Attack"],
                AttackGrow = (int)reader["AttackGrow"],
                Agility = (int)reader["Agility"],
                AgilityGrow = (int)reader["AgilityGrow"],
                Defence = (int)reader["Defence"],
                DefenceGrow = (int)reader["DefenceGrow"],
                Luck = (int)reader["Luck"],
                LuckGrow = (int)reader["LuckGrow"],
                Blood = (int)reader["Blood"],
                BloodGrow = (int)reader["BloodGrow"],
                Damage = (int)reader["Damage"],
                DamageGrow = (int)reader["DamageGrow"],
                Guard = (int)reader["Guard"],
                GuardGrow = (int)reader["GuardGrow"],
                Level = (int)reader["Level"],
                GP = (int)reader["GP"],
                MaxGP = (int)reader["MaxGP"],
                Hunger = (int)reader["Hunger"],
                PetHappyStar = (int)reader["PetHappyStar"],
                MP = (int)reader["MP"],
                Place = (int)reader["Place"],
                IsEquip = (bool)reader["IsEquip"],
                IsExit = (bool)reader["IsExit"],
                Skill = reader["Skill"].ToString(),
                SkillEquip = reader["SkillEquip"].ToString()
            };
        }
        public PetConfig[] GetAllPetConfig()
        {
            List<PetConfig> infos = new List<PetConfig>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_PetConfig_All");
                while (reader.Read())
                {
                    infos.Add(new PetConfig
                    {
                        ID = (int)reader["ID"],
                        Name = reader["Name"].ToString(),
                        Value = reader["Value"].ToString()
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetConfig", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public PetLevel[] GetAllPetLevel()
        {
            List<PetLevel> infos = new List<PetLevel>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_PetLevel_All");
                while (reader.Read())
                {
                    infos.Add(new PetLevel
                    {
                        Level = (int)reader["Level"],
                        GP = (int)reader["GP"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetLevel", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public PetTemplateInfo[] GetAllPetTemplateInfo()
        {
            List<PetTemplateInfo> infos = new List<PetTemplateInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_PetTemplateInfo_All");
                while (reader.Read())
                {
                    infos.Add(new PetTemplateInfo
                    {
                        ID = (int)reader["ID"],
                        TemplateID = (int)reader["TemplateID"],
                        Name = reader["Name"].ToString(),
                        KindID = (int)reader["KindID"],
                        Description = reader["Description"].ToString(),
                        Pic = reader["Pic"].ToString(),
                        RareLevel = (int)reader["RareLevel"],
                        MP = (int)reader["MP"],
                        StarLevel = (int)reader["StarLevel"],
                        GameAssetUrl = reader["GameAssetUrl"].ToString(),
                        EvolutionID = (int)reader["EvolutionID"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetTemplateInfo", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public PetSkillTemplateInfo[] GetAllPetSkillTemplateInfo()
        {
            List<PetSkillTemplateInfo> infos = new List<PetSkillTemplateInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_PetSkillTemplateInfo_All");
                while (reader.Read())
                {
                    infos.Add(new PetSkillTemplateInfo
                    {
                        ID = (int)reader["ID"],
                        PetTemplateID = (int)reader["PetTemplateID"],
                        KindID = (int)reader["KindID"],
                        GetTypes = (int)reader["GetType"],
                        SkillID = (int)reader["SkillID"],
                        SkillBookID = (int)reader["SkillBookID"],
                        MinLevel = (int)reader["MinLevel"],
                        DeleteSkillIDs = reader["DeleteSkillIDs"].ToString()
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetSkillTemplateInfo", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public PetSkillInfo[] GetAllPetSkillInfo()
        {
            List<PetSkillInfo> infos = new List<PetSkillInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_PetSkillInfo_All");
                while (reader.Read())
                {
                    infos.Add(new PetSkillInfo
                    {
                        ID = (int)reader["ID"],
                        Name = reader["Name"].ToString(),
                        ElementIDs = reader["ElementIDs"].ToString(),
                        Description = reader["Description"].ToString(),
                        BallType = (int)reader["BallType"],
                        NewBallID = (int)reader["NewBallID"],
                        CostMP = (int)reader["CostMP"],
                        Pic = (int)reader["Pic"],
                        Action = reader["Action"].ToString(),
                        EffectPic = reader["EffectPic"].ToString(),
                        Delay = (int)reader["Delay"],
                        ColdDown = (int)reader["ColdDown"],
                        GameType = (int)reader["GameType"],
                        Probability = (int)reader["Probability"],
                        Damage = (int)reader["Damage"],
                        DamageCrit = (int)reader["DamageCrit"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetSkillInfo", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public PetSkillElementInfo[] GetAllPetSkillElementInfo()
        {
            List<PetSkillElementInfo> infos = new List<PetSkillElementInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_PetSkillElementInfo_All");
                while (reader.Read())
                {
                    infos.Add(new PetSkillElementInfo
                    {
                        ID = (int)reader["ID"],
                        Name = reader["Name"].ToString(),
                        EffectPic = reader["EffectPic"].ToString(),
                        Description = reader["Description"].ToString(),
                        Pic = (int)reader["Pic"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllPetSkillElementInfo", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public bool AddCards(UsersCardInfo item)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[16];
                para[0] = new SqlParameter("@CardID", item.CardID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@CardType", item.CardType);
                para[2] = new SqlParameter("@UserID", item.UserID);
                para[3] = new SqlParameter("@Place", item.Place);
                para[4] = new SqlParameter("@TemplateID", item.TemplateID);
                para[5] = new SqlParameter("@isFirstGet", false);
                para[6] = new SqlParameter("@Attack", item.Attack);
                para[7] = new SqlParameter("@Defence", item.Defence);
                para[8] = new SqlParameter("@Luck", item.Luck);
                para[9] = new SqlParameter("@Agility", item.Agility);
                para[10] = new SqlParameter("@Damage", item.Damage);
                para[11] = new SqlParameter("@Guard", item.Guard);
                para[12] = new SqlParameter("@IsExit", item.IsExit);
                para[13] = new SqlParameter("@Level", item.Level);
                para[14] = new SqlParameter("@CardGP", item.CardGP);
                para[15] = new SqlParameter("@Type", item.Type);
                result = this.db.RunProcedure("SP_Users_Cards_Add", para);
                item.CardID = (int)para[0].Value;
                item.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdateCards(UsersCardInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@CardType", info.CardType),
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@Place", info.Place),
					new SqlParameter("@TemplateID", info.TemplateID),
					new SqlParameter("@isFirstGet", info.isFirstGet),
					new SqlParameter("@Attack", info.Attack),
					new SqlParameter("@Defence", info.Defence),
					new SqlParameter("@Luck", info.Luck),
					new SqlParameter("@Agility", info.Agility),
					new SqlParameter("@Damage", info.Damage),
					new SqlParameter("@Guard", info.Guard),
					new SqlParameter("@IsExit", info.IsExit),
					new SqlParameter("@Level", info.Level),
					new SqlParameter("@CardGP", info.CardGP),
					new SqlParameter("@Type", info.Type),
					new SqlParameter("@CardID", info.CardID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[16].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UserCardProp_Update", para);
                int returnValue = (int)para[16].Value;
                result = (returnValue == 0);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public List<UsersCardInfo> GetUserCardEuqip(int UserID)
        {
            List<UsersCardInfo> items = new List<UsersCardInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_Items_Card_Equip", para);
                while (reader.Read())
                {
                    items.Add(this.InitCard(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items;
        }
        public UsersCardInfo GetUserCardByPlace(int Place)
        {
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Place", SqlDbType.Int, 4)
				};
                para[0].Value = Place;
                this.db.GetReader(ref reader, "SP_Get_UserCard_By_Place", para);
                if (reader.Read())
                {
                    return this.InitCard(reader);
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return null;
        }
        public UsersCardInfo[] GetUserCardSingles(int UserID)
        {
            List<UsersCardInfo> items = new List<UsersCardInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Get_UserCard_By_ID", para);
                while (reader.Read())
                {
                    items.Add(this.InitCard(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return items.ToArray();
        }
        public UsersCardInfo InitCard(SqlDataReader reader)
        {
            return new UsersCardInfo
            {
                UserID = (int)reader["UserID"],
                TemplateID = (int)reader["TemplateID"],
                CardID = (int)reader["CardID"],
                CardType = (int)reader["CardType"],
                Attack = (int)reader["Attack"],
                Agility = (int)reader["Agility"],
                Defence = (int)reader["Defence"],
                Luck = (int)reader["Luck"],
                Damage = (int)reader["Damage"],
                Guard = (int)reader["Guard"],
                Level = (int)reader["Level"],
                Place = (int)reader["Place"],
                isFirstGet = (bool)reader["isFirstGet"],
                Type = (int)reader["Type"],
                CardGP = (int)reader["CardGP"]
            };
        }
        public CardGrooveUpdateInfo[] GetAllCardGrooveUpdate()
        {
            List<CardGrooveUpdateInfo> infos = new List<CardGrooveUpdateInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_CardGrooveUpdate_All");
                while (reader.Read())
                {
                    infos.Add(this.InitCardGrooveUpdate(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllCardGrooveUpdate", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public CardGrooveUpdateInfo InitCardGrooveUpdate(SqlDataReader reader)
        {
            return new CardGrooveUpdateInfo
            {
                ID = (int)reader["ID"],
                Attack = (int)reader["Attack"],
                Defend = (int)reader["Defend"],
                Agility = (int)reader["Agility"],
                Lucky = (int)reader["Lucky"],
                Damage = (int)reader["Damage"],
                Guard = (int)reader["Guard"],
                Level = (int)reader["Level"],
                Type = (int)reader["Type"],
                Exp = (int)reader["Exp"]
            };
        }
        public CardTemplateInfo[] GetAllCardTemplate()
        {
            List<CardTemplateInfo> infos = new List<CardTemplateInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_CardTemplate_All");
                while (reader.Read())
                {
                    infos.Add(this.InitCardTemplate(reader));
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetAllCardTemplateInfo", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public CardTemplateInfo InitCardTemplate(SqlDataReader reader)
        {
            return new CardTemplateInfo
            {
                ID = (int)reader["ID"],
                CardID = (int)reader["CardID"],
                CardType = (int)reader["CardType"],
                probability = (int)reader["probability"],
                AttackRate = (int)reader["AttackRate"],
                AddAttack = (int)reader["AddAttack"],
                DefendRate = (int)reader["DefendRate"],
                AddDefend = (int)reader["AddDefend"],
                AgilityRate = (int)reader["AgilityRate"],
                AddAgility = (int)reader["AddAgility"],
                LuckyRate = (int)reader["LuckyRate"],
                AddLucky = (int)reader["AddLucky"],
                DamageRate = (int)reader["DamageRate"],
                AddDamage = (int)reader["AddDamage"],
                GuardRate = (int)reader["GuardRate"],
                AddGuard = (int)reader["AddGuard"]
            };
        }
        public bool AddFarm(UserFarmInfo item)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@FarmID", item.FarmID),
					new SqlParameter("@PayFieldMoney", item.PayFieldMoney),
					new SqlParameter("@PayAutoMoney", item.PayAutoMoney),
					new SqlParameter("@AutoPayTime", item.AutoPayTime.ToString()),
					new SqlParameter("@AutoValidDate", item.AutoValidDate),
					new SqlParameter("@VipLimitLevel", item.VipLimitLevel),
					new SqlParameter("@FarmerName", item.FarmerName),
					new SqlParameter("@GainFieldId", item.GainFieldId),
					new SqlParameter("@MatureId", item.MatureId),
					new SqlParameter("@KillCropId", item.KillCropId),
					new SqlParameter("@isAutoId", item.isAutoId),
					new SqlParameter("@isFarmHelper", item.isFarmHelper),
					new SqlParameter("@ID", item.ID)
				};
                para[12].Direction = ParameterDirection.Output;
                result = this.db.RunProcedure("SP_Users_Farm_Add", para);
                item.ID = (int)para[12].Value;
                item.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdateFarm(UserFarmInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@FarmID", info.FarmID),
					new SqlParameter("@PayFieldMoney", info.PayFieldMoney),
					new SqlParameter("@PayAutoMoney", info.PayAutoMoney),
					new SqlParameter("@AutoPayTime", info.AutoPayTime.ToString()),
					new SqlParameter("@AutoValidDate", info.AutoValidDate),
					new SqlParameter("@VipLimitLevel", info.VipLimitLevel),
					new SqlParameter("@FarmerName", info.FarmerName),
					new SqlParameter("@GainFieldId", info.GainFieldId),
					new SqlParameter("@MatureId", info.MatureId),
					new SqlParameter("@KillCropId", info.KillCropId),
					new SqlParameter("@isAutoId", info.isAutoId),
					new SqlParameter("@isFarmHelper", info.isFarmHelper),
					new SqlParameter("@buyExpRemainNum", info.buyExpRemainNum),
					new SqlParameter("@isArrange", info.isArrange)
				};
                result = this.db.RunProcedure("SP_Users_Farm_Update", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool AddFields(UserFieldInfo item)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@FarmID", item.FarmID),
					new SqlParameter("@FieldID", item.FieldID),
					new SqlParameter("@SeedID", item.SeedID),
					new SqlParameter("@PlantTime", item.PlantTime.ToString()),
					new SqlParameter("@AccelerateTime", item.AccelerateTime),
					new SqlParameter("@FieldValidDate", item.FieldValidDate),
					new SqlParameter("@PayTime", item.PayTime.ToString()),
					new SqlParameter("@GainCount", item.GainCount),
					new SqlParameter("@AutoSeedID", item.AutoSeedID),
					new SqlParameter("@AutoFertilizerID", item.AutoFertilizerID),
					new SqlParameter("@AutoSeedIDCount", item.AutoSeedIDCount),
					new SqlParameter("@AutoFertilizerCount", item.AutoFertilizerCount),
					new SqlParameter("@isAutomatic", item.isAutomatic),
					new SqlParameter("@AutomaticTime", item.AutomaticTime.ToString()),
					new SqlParameter("@IsExit", item.IsExit),
					new SqlParameter("@payFieldTime", item.payFieldTime),
					new SqlParameter("@ID", item.ID)
				};
                para[16].Direction = ParameterDirection.Output;
                result = this.db.RunProcedure("SP_Users_Fields_Add", para);
                item.ID = (int)para[16].Value;
                item.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdateFields(UserFieldInfo info)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@FarmID", info.FarmID),
					new SqlParameter("@FieldID", info.FieldID),
					new SqlParameter("@SeedID", info.SeedID),
					new SqlParameter("@PlantTime", info.PlantTime.ToString()),
					new SqlParameter("@AccelerateTime", info.AccelerateTime),
					new SqlParameter("@FieldValidDate", info.FieldValidDate),
					new SqlParameter("@PayTime", info.PayTime.ToString()),
					new SqlParameter("@GainCount", info.GainCount),
					new SqlParameter("@AutoSeedID", info.AutoSeedID),
					new SqlParameter("@AutoFertilizerID", info.AutoFertilizerID),
					new SqlParameter("@AutoSeedIDCount", info.AutoSeedIDCount),
					new SqlParameter("@AutoFertilizerCount", info.AutoFertilizerCount),
					new SqlParameter("@isAutomatic", info.isAutomatic),
					new SqlParameter("@AutomaticTime", info.AutomaticTime.ToString()),
					new SqlParameter("@IsExit", info.IsExit),
					new SqlParameter("@payFieldTime", info.payFieldTime)
				};
                result = this.db.RunProcedure("SP_Users_Fields_Update", para);
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public UserFarmInfo GetSingleFarm(int Id)
        {
            UserFarmInfo infos = null;
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
                para[0].Value = Id;
                this.db.GetReader(ref reader, "SP_Get_SingleFarm", para);
                while (reader.Read())
                {
                    infos = new UserFarmInfo();
                    infos.ID = (int)reader["ID"];
                    infos.FarmID = (int)reader["FarmID"];
                    infos.PayFieldMoney = (string)reader["PayFieldMoney"];
                    infos.PayAutoMoney = (string)reader["PayAutoMoney"];
                    infos.AutoPayTime = (DateTime)reader["AutoPayTime"];
                    infos.AutoValidDate = (int)reader["AutoValidDate"];
                    infos.VipLimitLevel = (int)reader["VipLimitLevel"];
                    infos.FarmerName = (string)reader["FarmerName"];
                    infos.GainFieldId = (int)reader["GainFieldId"];
                    infos.MatureId = (int)reader["MatureId"];
                    infos.KillCropId = (int)reader["KillCropId"];
                    infos.isAutoId = (int)reader["isAutoId"];
                    infos.isFarmHelper = (bool)reader["isFarmHelper"];
                    infos.buyExpRemainNum = (int)reader["buyExpRemainNum"];
                    infos.isArrange = (bool)reader["isArrange"];
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetSingleFarm", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos;
        }
        public UserFieldInfo[] GetSingleFields(int ID)
        {
            List<UserFieldInfo> infos = new List<UserFieldInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
                para[0].Value = ID;
                this.db.GetReader(ref reader, "SP_Get_SingleFields", para);
                while (reader.Read())
                {
                    infos.Add(new UserFieldInfo
                    {
                        ID = (int)reader["ID"],
                        FarmID = (int)reader["FarmID"],
                        FieldID = (int)reader["FieldID"],
                        SeedID = (int)reader["SeedID"],
                        PlantTime = (DateTime)reader["PlantTime"],
                        AccelerateTime = (int)reader["AccelerateTime"],
                        FieldValidDate = (int)reader["FieldValidDate"],
                        PayTime = (DateTime)reader["PayTime"],
                        GainCount = (int)reader["GainCount"],
                        AutoSeedID = (int)reader["AutoSeedID"],
                        AutoFertilizerID = (int)reader["AutoFertilizerID"],
                        AutoSeedIDCount = (int)reader["AutoSeedIDCount"],
                        AutoFertilizerCount = (int)reader["AutoFertilizerCount"],
                        isAutomatic = (bool)reader["isAutomatic"],
                        AutomaticTime = (DateTime)reader["AutomaticTime"],
                        IsExit = (bool)reader["IsExit"],
                        payFieldTime = (int)reader["payFieldTime"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleFields", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public List<UserGemStone> GetSingleGemStones(int ID)
        {
            List<UserGemStone> infos = new List<UserGemStone>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
                para[0].Value = ID;
                this.db.GetReader(ref reader, "SP_GetSingleGemStone", para);
                while (reader.Read())
                {
                    infos.Add(new UserGemStone
                    {
                        ID = (int)reader["ID"],
                        UserID = (int)reader["UserID"],
                        FigSpiritId = (int)reader["FigSpiritId"],
                        FigSpiritIdValue = (string)reader["FigSpiritIdValue"],
                        EquipPlace = (int)reader["EquipPlace"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_GetSingleUserGemStones", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos;
        }
        public bool AddUserGemStone(UserGemStone item)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@ID", item.ID);
                para[0].Direction = ParameterDirection.Output;
                para[1] = new SqlParameter("@UserID", item.UserID);
                para[2] = new SqlParameter("@FigSpiritId", item.FigSpiritId);
                para[3] = new SqlParameter("@FigSpiritIdValue", item.FigSpiritIdValue);
                para[4] = new SqlParameter("@EquipPlace", item.EquipPlace);
                para[5] = new SqlParameter("@Result", SqlDbType.Int);
                para[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Users_GemStones_Add", para);
                int returnValue = (int)para[5].Value;
                result = (returnValue == 0);
                item.ID = (int)para[0].Value;
                item.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdateGemStoneInfo(UserGemStone g)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@ID", g.ID),
					new SqlParameter("@UserID", g.UserID),
					new SqlParameter("@FigSpiritId", g.FigSpiritId),
					new SqlParameter("@FigSpiritIdValue", g.FigSpiritIdValue),
					new SqlParameter("@EquipPlace", g.EquipPlace),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                sqlParameters[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateGemStoneInfo", sqlParameters);
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateGemStoneInfo", exception);
                }
            }
            return flag;
        }
        public TotemInfo[] GetAllTotem()
        {
            List<TotemInfo> infos = new List<TotemInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_Totem_All");
                while (reader.Read())
                {
                    infos.Add(new TotemInfo
                    {
                        ID = (int)reader["ID"],
                        ConsumeExp = (int)reader["ConsumeExp"],
                        ConsumeHonor = (int)reader["ConsumeHonor"],
                        AddAttack = (int)reader["AddAttack"],
                        AddDefence = (int)reader["AddDefence"],
                        AddAgility = (int)reader["AddAgility"],
                        AddLuck = (int)reader["AddLuck"],
                        AddBlood = (int)reader["AddBlood"],
                        AddDamage = (int)reader["AddDamage"],
                        AddGuard = (int)reader["AddGuard"],
                        Random = (int)reader["Random"],
                        Page = (int)reader["Page"],
                        Layers = (int)reader["Layers"],
                        Location = (int)reader["Location"],
                        Point = (int)reader["Point"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetTotemAll", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public FightSpiritTemplateInfo[] GetAllFightSpiritTemplate()
        {
            List<FightSpiritTemplateInfo> infos = new List<FightSpiritTemplateInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_FightSpiritTemplate_All");
                while (reader.Read())
                {
                    infos.Add(new FightSpiritTemplateInfo
                    {
                        ID = (int)reader["ID"],
                        FightSpiritID = (int)reader["FightSpiritID"],
                        FightSpiritIcon = (string)reader["FightSpiritIcon"],
                        Level = (int)reader["Level"],
                        Exp = (int)reader["Exp"],
                        Attack = (int)reader["Attack"],
                        Defence = (int)reader["Defence"],
                        Agility = (int)reader["Agility"],
                        Lucky = (int)reader["Lucky"],
                        Blood = (int)reader["Blood"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetFightSpiritTemplateAll", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public TotemHonorTemplateInfo[] GetAllTotemHonorTemplate()
        {
            List<TotemHonorTemplateInfo> infos = new List<TotemHonorTemplateInfo>();
            SqlDataReader reader = null;
            try
            {
                this.db.GetReader(ref reader, "SP_TotemHonorTemplate_All");
                while (reader.Read())
                {
                    infos.Add(new TotemHonorTemplateInfo
                    {
                        ID = (int)reader["ID"],
                        NeedMoney = (int)reader["NeedMoney"],
                        Type = (int)reader["Type"],
                        AddHonor = (int)reader["AddHonor"]
                    });
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("GetTotemHonorTemplateInfo", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos.ToArray();
        }
        public Dictionary<int, UserDrillInfo> GetPlayerDrillByID(int UserID)
        {
            Dictionary<int, UserDrillInfo> infos = new Dictionary<int, UserDrillInfo>();
            SqlDataReader reader = null;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", SqlDbType.Int, 4)
				};
                para[0].Value = UserID;
                this.db.GetReader(ref reader, "SP_Users_Drill_All", para);
                while (reader.Read())
                {
                    UserDrillInfo info = new UserDrillInfo();
                    info.UserID = (int)reader["UserID"];
                    info.BeadPlace = (int)reader["BeadPlace"];
                    info.HoleLv = (int)reader["HoleLv"];
                    info.HoleExp = (int)reader["HoleExp"];
                    info.DrillPlace = (int)reader["DrillPlace"];
                    if (!infos.ContainsKey(info.DrillPlace))
                    {
                        infos.Add(info.DrillPlace, info);
                    }
                }
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return infos;
        }
        public bool AddUserUserDrill(UserDrillInfo item)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", item.UserID),
					new SqlParameter("@BeadPlace", item.BeadPlace),
					new SqlParameter("@HoleExp", item.HoleExp),
					new SqlParameter("@HoleLv", item.HoleLv),
					new SqlParameter("@DrillPlace", item.DrillPlace),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                para[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Users_UserDrill_Add", para);
                int returnValue = (int)para[5].Value;
                result = (returnValue == 0);
                item.IsDirty = false;
            }
            catch (Exception e)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;
        }
        public bool UpdateUserDrillInfo(UserDrillInfo g)
        {
            bool flag = false;
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@UserID", g.UserID),
					new SqlParameter("@BeadPlace", g.BeadPlace),
					new SqlParameter("@HoleExp", g.HoleExp),
					new SqlParameter("@HoleLv", g.HoleLv),
					new SqlParameter("@DrillPlace", g.DrillPlace),
					new SqlParameter("@Result", SqlDbType.Int)
				};
                sqlParameters[5].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_UpdateUserDrillInfo", sqlParameters);
                flag = true;
            }
            catch (Exception exception)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                {
                    BaseBussiness.log.Error("SP_UpdateUserDrillInfo", exception);
                }
            }
            return flag;
        }
    }
}
