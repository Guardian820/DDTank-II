using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class ProduceBussiness : BaseBussiness
	{
		public LoadUserBoxInfo[] GetAllTimeBoxAward()
		{
			List<LoadUserBoxInfo> list = new List<LoadUserBoxInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_TimeBox_Award_All");
				while (sqlDataReader.Read())
				{
					list.Add(new LoadUserBoxInfo
					{
						ID = (int)sqlDataReader["ID"],
						Type = (int)sqlDataReader["Type"],
						Level = (int)sqlDataReader["Level"],
						Condition = (int)sqlDataReader["Condition"],
						TemplateID = (int)sqlDataReader["TemplateID"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllDaily", exception);
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
		public GoldEquipTemplateLoadInfo[] GetAllGoldEquipTemplateLoad()
		{
			List<GoldEquipTemplateLoadInfo> list = new List<GoldEquipTemplateLoadInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_GoldEquipTemplateLoad_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitGoldEquipTemplateLoad(sqlDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllGoldEquipTemplateLoad", exception);
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
		public GoldEquipTemplateLoadInfo InitGoldEquipTemplateLoad(SqlDataReader reader)
		{
			return new GoldEquipTemplateLoadInfo
			{
				ID = (int)reader["ID"],
				OldTemplateId = (int)reader["OldTemplateId"],
				NewTemplateId = (int)reader["NewTemplateId"],
				CategoryID = (int)reader["CategoryID"],
				Strengthen = (int)reader["Strengthen"],
				Attack = (int)reader["Attack"],
				Defence = (int)reader["Defence"],
				Agility = (int)reader["Agility"],
				Luck = (int)reader["Luck"],
				Damage = (int)reader["Damage"],
				Guard = (int)reader["Guard"],
				Boold = (int)reader["Boold"],
				BlessID = (int)reader["BlessID"],
				Pic = (reader["pic"] == null) ? "" : reader["pic"].ToString()
			};
		}
		public AchievementInfo[] GetALlAchievement()
		{
			List<AchievementInfo> list = new List<AchievementInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Achievement_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitAchievement(sqlDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetALlAchievement:", exception);
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
		public AchievementConditionInfo[] GetALlAchievementCondition()
		{
			List<AchievementConditionInfo> list = new List<AchievementConditionInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Achievement_Condition_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitAchievementCondition(sqlDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetALlAchievementCondition:", exception);
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
		public AchievementDataInfo[] GetAllAchievementData(int userID)
		{
			List<AchievementDataInfo> list = new List<AchievementDataInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@UserID", userID)
				};
				this.db.GetReader(ref sqlDataReader, "SP_Achievement_Data_All", sqlParameters);
				while (sqlDataReader.Read())
				{
					list.Add(this.InitAchievementData(sqlDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllAchievementData", exception);
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
		public AchievementRewardInfo[] GetALlAchievementReward()
		{
			List<AchievementRewardInfo> list = new List<AchievementRewardInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Achievement_Reward_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitAchievementReward(sqlDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetALlAchievementReward", exception);
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
		public ActiveAwardInfo[] GetAllActiveAwardInfo()
		{
			List<ActiveAwardInfo> list = new List<ActiveAwardInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Active_Award");
				while (sqlDataReader.Read())
				{
					ActiveAwardInfo item = new ActiveAwardInfo
					{
						ID = (int)sqlDataReader["ID"],
						ActiveID = (int)sqlDataReader["ActiveID"],
						AgilityCompose = (int)sqlDataReader["AgilityCompose"],
						AttackCompose = (int)sqlDataReader["AttackCompose"],
						Count = (int)sqlDataReader["Count"],
						DefendCompose = (int)sqlDataReader["DefendCompose"],
						Gold = (int)sqlDataReader["Gold"],
						ItemID = (int)sqlDataReader["ItemID"],
						LuckCompose = (int)sqlDataReader["LuckCompose"],
						Mark = (int)sqlDataReader["Mark"],
						Money = (int)sqlDataReader["Money"],
						Sex = (int)sqlDataReader["Sex"],
						StrengthenLevel = (int)sqlDataReader["StrengthenLevel"],
						ValidDate = (int)sqlDataReader["ValidDate"],
						GiftToken = (int)sqlDataReader["GiftToken"]
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllActiveAwardInfo", exception);
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
		public ActiveConditionInfo[] GetAllActiveConditionInfo()
		{
			List<ActiveConditionInfo> list = new List<ActiveConditionInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Active_Condition");
				while (sqlDataReader.Read())
				{
					ActiveConditionInfo item = new ActiveConditionInfo
					{
						ID = (int)sqlDataReader["ID"],
						ActiveID = (int)sqlDataReader["ActiveID"],
						Conditiontype = (int)sqlDataReader["Conditiontype"],
						Condition = (int)sqlDataReader["Condition"],
						LimitGrade = (sqlDataReader["LimitGrade"].ToString() == null) ? "" : sqlDataReader["LimitGrade"].ToString(),
						AwardId = (sqlDataReader["AwardId"].ToString() == null) ? "" : sqlDataReader["AwardId"].ToString(),
						IsMult = (bool)sqlDataReader["IsMult"],
						StartTime = (DateTime)sqlDataReader["StartTime"],
						EndTime = (DateTime)sqlDataReader["EndTime"]
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllActiveConditionInfo", exception);
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
		public List<BigBugleInfo> GetAllAreaBigBugleRecord()
		{
			SqlDataReader sqlDataReader = null;
			List<BigBugleInfo> list = new List<BigBugleInfo>();
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Get_AreaBigBugle_Record");
				while (sqlDataReader.Read())
				{
					BigBugleInfo item = new BigBugleInfo
					{
						ID = (int)sqlDataReader["ID"],
						UserID = (int)sqlDataReader["UserID"],
						AreaID = (int)sqlDataReader["AreaID"],
						NickName = (sqlDataReader["NickName"] == null) ? "" : sqlDataReader["NickName"].ToString(),
						Message = (sqlDataReader["Message"] == null) ? "" : sqlDataReader["Message"].ToString(),
						State = (bool)sqlDataReader["State"],
						IP = (sqlDataReader["IP"] == null) ? "" : sqlDataReader["IP"].ToString()
					};
					list.Add(item);
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllAreaBigBugleRecord", exception);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return list;
		}
		public AchievementInfo InitAchievement(SqlDataReader reader)
		{
			return new AchievementInfo
			{
				ID = (int)reader["ID"],
				PlaceID = (int)reader["PlaceID"],
				Title = (reader["Title"] == null) ? "" : reader["Title"].ToString(),
				Detail = (reader["Detail"] == null) ? "" : reader["Detail"].ToString(),
				NeedMinLevel = (int)reader["NeedMinLevel"],
				NeedMaxLevel = (int)reader["NeedMaxLevel"],
				PreAchievementID = (reader["PreAchievementID"] == null) ? "" : reader["PreAchievementID"].ToString(),
				IsOther = (int)reader["IsOther"],
				AchievementType = (int)reader["AchievementType"],
				CanHide = (bool)reader["CanHide"],
				StartDate = (DateTime)reader["StartDate"],
				EndDate = (DateTime)reader["EndDate"],
				AchievementPoint = (int)reader["AchievementPoint"],
				IsActive = (int)reader["IsActive"],
				PicID = (int)reader["PicID"],
				IsShare = (bool)reader["IsShare"]
			};
		}
		public AchievementConditionInfo InitAchievementCondition(SqlDataReader reader)
		{
			return new AchievementConditionInfo
			{
				AchievementID = (int)reader["AchievementID"],
				CondictionID = (int)reader["CondictionID"],
				CondictionType = (int)reader["CondictionType"],
				Condiction_Para1 = (reader["Condiction_Para1"] == null) ? "" : reader["Condiction_Para1"].ToString(),
				Condiction_Para2 = (int)reader["Condiction_Para2"]
			};
		}
		public AchievementDataInfo InitAchievementData(SqlDataReader reader)
		{
			return new AchievementDataInfo
			{
				UserID = (int)reader["UserID"],
				AchievementID = (int)reader["AchievementID"],
				IsComplete = (bool)reader["IsComplete"],
				CompletedDate = (DateTime)reader["CompletedDate"]
			};
		}
		public AchievementRewardInfo InitAchievementReward(SqlDataReader reader)
		{
			return new AchievementRewardInfo
			{
				AchievementID = (int)reader["AchievementID"],
				RewardType = (int)reader["RewardType"],
				RewardPara = (reader["RewardPara"] == null) ? "" : reader["RewardPara"].ToString(),
				RewardValueId = (int)reader["RewardValueId"],
				RewardCount = (int)reader["RewardCount"]
			};
		}
		public ItemRecordTypeInfo[] GetAllItemRecordType()
		{
			List<ItemRecordTypeInfo> list = new List<ItemRecordTypeInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Item_Record_Type_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitItemRecordType(sqlDataReader));
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllItemRecordType:", exception);
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
		public ItemRecordTypeInfo InitItemRecordType(SqlDataReader reader)
		{
			return new ItemRecordTypeInfo
			{
				RecordID = (int)reader["RecordID"],
				Name = (reader["Name"] == null) ? "" : reader["Name"].ToString(),
				Description = (reader["Description"] == null) ? "" : reader["Description"].ToString()
			};
		}
		public ItemTemplateInfo[] GetAllGoodsASC()
		{
			List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Items_All_ASC");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitItemTemplateInfo(sqlDataReader));
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
		public ItemTemplateInfo[] GetAllGoods()
		{
			List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Items_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitItemTemplateInfo(sqlDataReader));
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
		public ShopGoodsShowListInfo InitShopGoodsShowListInfo(SqlDataReader reader)
		{
			return new ShopGoodsShowListInfo
			{
				Type = (int)reader["Type"],
				ShopId = (int)reader["ShopId"]
			};
		}
		public ShopGoodsShowListInfo[] GetAllShopGoodsShowList()
		{
			List<ShopGoodsShowListInfo> list = new List<ShopGoodsShowListInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_ShopGoodsShowList_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitShopGoodsShowListInfo(sqlDataReader));
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
		public ItemBoxInfo[] GetSingleItemsBox(int DataID)
		{
			List<ItemBoxInfo> list = new List<ItemBoxInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				array[0].Value = DataID;
				this.db.GetReader(ref sqlDataReader, "SP_ItemsBox_Single", array);
				while (sqlDataReader.Read())
				{
					list.Add(this.InitItemBoxInfo(sqlDataReader));
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
		public ItemTemplateInfo GetSingleGoods(int goodsID)
		{
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				array[0].Value = goodsID;
				this.db.GetReader(ref sqlDataReader, "SP_Items_Single", array);
				if (sqlDataReader.Read())
				{
					return this.InitItemTemplateInfo(sqlDataReader);
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
		public ItemTemplateInfo[] GetSingleCategory(int CategoryID)
		{
			List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@CategoryID", SqlDbType.Int, 4)
				};
				array[0].Value = CategoryID;
				this.db.GetReader(ref sqlDataReader, "SP_Items_Category_Single", array);
				while (sqlDataReader.Read())
				{
					list.Add(this.InitItemTemplateInfo(sqlDataReader));
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
		public ItemTemplateInfo[] GetFusionType()
		{
			List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Items_FusionType");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitItemTemplateInfo(sqlDataReader));
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
		public ItemTemplateInfo InitItemTemplateInfo(SqlDataReader reader)
		{
			return new ItemTemplateInfo
			{
				AddTime = reader["AddTime"].ToString(),
				Agility = (int)reader["Agility"],
				Attack = (int)reader["Attack"],
				CanDelete = (bool)reader["CanDelete"],
				CanDrop = (bool)reader["CanDrop"],
				CanEquip = (bool)reader["CanEquip"],
				CanUse = (bool)reader["CanUse"],
				CategoryID = (int)reader["CategoryID"],
				Colors = reader["Colors"].ToString(),
				Defence = (int)reader["Defence"],
				Description = reader["Description"].ToString(),
				Level = (int)reader["Level"],
				Luck = (int)reader["Luck"],
				MaxCount = (int)reader["MaxCount"],
				Name = reader["Name"].ToString(),
				NeedSex = (int)reader["NeedSex"],
				Pic = reader["Pic"].ToString(),
				Data = (reader["Data"] == null) ? "" : reader["Data"].ToString(),
				Property1 = (int)reader["Property1"],
				Property2 = (int)reader["Property2"],
				Property3 = (int)reader["Property3"],
				Property4 = (int)reader["Property4"],
				Property5 = (int)reader["Property5"],
				Property6 = (int)reader["Property6"],
				Property7 = (int)reader["Property7"],
				Property8 = (int)reader["Property8"],
				Quality = (int)reader["Quality"],
				Script = reader["Script"].ToString(),
				TemplateID = (int)reader["TemplateID"],
				CanCompose = (bool)reader["CanCompose"],
				CanStrengthen = (bool)reader["CanStrengthen"],
				NeedLevel = (int)reader["NeedLevel"],
				BindType = (int)reader["BindType"],
				FusionType = (int)reader["FusionType"],
				FusionRate = (int)reader["FusionRate"],
				FusionNeedRate = (int)reader["FusionNeedRate"],
				Hole = (reader["Hole"] == null) ? "" : reader["Hole"].ToString(),
				RefineryLevel = (int)reader["RefineryLevel"],
				ReclaimValue = (int)reader["ReclaimValue"],
				ReclaimType = (int)reader["ReclaimType"],
				CanRecycle = (int)reader["CanRecycle"],
				IsDirty = false
			};
		}
		public ItemBoxInfo[] GetItemBoxInfos()
		{
			List<ItemBoxInfo> list = new List<ItemBoxInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_ItemsBox_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitItemBoxInfo(sqlDataReader));
				}
			}
			catch (Exception arg)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init@Shop_Goods_Box：" + arg);
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
		public ItemBoxInfo InitItemBoxInfo(SqlDataReader reader)
		{
			return new ItemBoxInfo
			{
				Id = (int)reader["id"],
				DataId = (int)reader["DataId"],
				TemplateId = (int)reader["TemplateId"],
				IsSelect = (bool)reader["IsSelect"],
				IsBind = (bool)reader["IsBind"],
				ItemValid = (int)reader["ItemValid"],
				ItemCount = (int)reader["ItemCount"],
				StrengthenLevel = (int)reader["StrengthenLevel"],
				AttackCompose = (int)reader["AttackCompose"],
				DefendCompose = (int)reader["DefendCompose"],
				AgilityCompose = (int)reader["AgilityCompose"],
				LuckCompose = (int)reader["LuckCompose"],
				Random = (int)reader["Random"],
				IsTips = (int)reader["IsTips"],
				IsLogs = (bool)reader["IsLogs"]
			};
		}
		public bool UpdatePlayerInfoHistory(PlayerInfoHistory info)
		{
			bool result = false;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@LastQuestsTime", info.LastQuestsTime),
					new SqlParameter("@LastTreasureTime", info.LastTreasureTime),
					new SqlParameter("@OutPut", SqlDbType.Int)
				};
				array[3].Direction = ParameterDirection.Output;
				this.db.RunProcedure("SP_User_Update_History", array);
				result = ((int)array[6].Value == 1);
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("User_Update_BoxProgression", exception);
				}
			}
			return result;
		}
		public CategoryInfo[] GetAllCategory()
		{
			List<CategoryInfo> list = new List<CategoryInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Items_Category_All");
				while (sqlDataReader.Read())
				{
					list.Add(new CategoryInfo
					{
						ID = (int)sqlDataReader["ID"],
						Name = (sqlDataReader["Name"] == null) ? "" : sqlDataReader["Name"].ToString(),
						Place = (int)sqlDataReader["Place"],
						Remark = (sqlDataReader["Remark"] == null) ? "" : sqlDataReader["Remark"].ToString()
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
		public PropInfo[] GetAllProp()
		{
			List<PropInfo> list = new List<PropInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Prop_All");
				while (sqlDataReader.Read())
				{
					list.Add(new PropInfo
					{
						AffectArea = (int)sqlDataReader["AffectArea"],
						AffectTimes = (int)sqlDataReader["AffectTimes"],
						AttackTimes = (int)sqlDataReader["AttackTimes"],
						BoutTimes = (int)sqlDataReader["BoutTimes"],
						BuyGold = (int)sqlDataReader["BuyGold"],
						BuyMoney = (int)sqlDataReader["BuyMoney"],
						Category = (int)sqlDataReader["Category"],
						Delay = (int)sqlDataReader["Delay"],
						Description = sqlDataReader["Description"].ToString(),
						Icon = sqlDataReader["Icon"].ToString(),
						ID = (int)sqlDataReader["ID"],
						Name = sqlDataReader["Name"].ToString(),
						Parameter = (int)sqlDataReader["Parameter"],
						Pic = sqlDataReader["Pic"].ToString(),
						Property1 = (int)sqlDataReader["Property1"],
						Property2 = (int)sqlDataReader["Property2"],
						Property3 = (int)sqlDataReader["Property3"],
						Random = (int)sqlDataReader["Random"],
						Script = sqlDataReader["Script"].ToString()
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
		public BallInfo[] GetAllBall()
		{
			List<BallInfo> list = new List<BallInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Ball_All");
				while (sqlDataReader.Read())
				{
					list.Add(new BallInfo
					{
						Amount = (int)sqlDataReader["Amount"],
						ID = (int)sqlDataReader["ID"],
						Name = sqlDataReader["Name"].ToString(),
						Crater = (sqlDataReader["Crater"] == null) ? "" : sqlDataReader["Crater"].ToString(),
						Power = (double)sqlDataReader["Power"],
						Radii = (int)sqlDataReader["Radii"],
						AttackResponse = (int)sqlDataReader["AttackResponse"],
						BombPartical = sqlDataReader["BombPartical"].ToString(),
						FlyingPartical = sqlDataReader["FlyingPartical"].ToString(),
						IsSpin = (bool)sqlDataReader["IsSpin"],
						Mass = (int)sqlDataReader["Mass"],
						SpinV = (int)sqlDataReader["SpinV"],
						SpinVA = (double)sqlDataReader["SpinVA"],
						Wind = (int)sqlDataReader["Wind"],
						DragIndex = (int)sqlDataReader["DragIndex"],
						Weight = (int)sqlDataReader["Weight"],
						Shake = (bool)sqlDataReader["Shake"],
						Delay = (int)sqlDataReader["Delay"],
						ShootSound = (sqlDataReader["ShootSound"] == null) ? "" : sqlDataReader["ShootSound"].ToString(),
						BombSound = (sqlDataReader["BombSound"] == null) ? "" : sqlDataReader["BombSound"].ToString(),
						ActionType = (int)sqlDataReader["ActionType"],
						HasTunnel = (bool)sqlDataReader["HasTunnel"]
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
		public BallConfigInfo[] GetAllBallConfig()
		{
			List<BallConfigInfo> list = new List<BallConfigInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "[SP_Ball_Config_All]");
				while (sqlDataReader.Read())
				{
					list.Add(new BallConfigInfo
					{
						Common = (int)sqlDataReader["Common"],
						TemplateID = (int)sqlDataReader["TemplateID"],
						CommonAddWound = (int)sqlDataReader["CommonAddWound"],
						CommonMultiBall = (int)sqlDataReader["CommonMultiBall"],
						Special = (int)sqlDataReader["Special"],
						SpecialII = (int)sqlDataReader["SpecialII"]
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
		public ShopItemInfo[] GetALllShop()
		{
			List<ShopItemInfo> list = new List<ShopItemInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Shop_All");
				while (sqlDataReader.Read())
				{
					list.Add(new ShopItemInfo
					{
						ID = int.Parse(sqlDataReader["ID"].ToString()),
						ShopID = int.Parse(sqlDataReader["ShopID"].ToString()),
						GroupID = int.Parse(sqlDataReader["GroupID"].ToString()),
						TemplateID = int.Parse(sqlDataReader["TemplateID"].ToString()),
						BuyType = int.Parse(sqlDataReader["BuyType"].ToString()),
						Sort = int.Parse(sqlDataReader["Sort"].ToString()),
						IsVouch = int.Parse(sqlDataReader["IsVouch"].ToString()),
						Label = (float)int.Parse(sqlDataReader["Label"].ToString()),
						Beat = decimal.Parse(sqlDataReader["Beat"].ToString()),
						AUnit = int.Parse(sqlDataReader["AUnit"].ToString()),
						APrice1 = int.Parse(sqlDataReader["APrice1"].ToString()),
						AValue1 = int.Parse(sqlDataReader["AValue1"].ToString()),
						APrice2 = int.Parse(sqlDataReader["APrice2"].ToString()),
						AValue2 = int.Parse(sqlDataReader["AValue2"].ToString()),
						APrice3 = int.Parse(sqlDataReader["APrice3"].ToString()),
						AValue3 = int.Parse(sqlDataReader["AValue3"].ToString()),
						BUnit = int.Parse(sqlDataReader["BUnit"].ToString()),
						BPrice1 = int.Parse(sqlDataReader["BPrice1"].ToString()),
						BValue1 = int.Parse(sqlDataReader["BValue1"].ToString()),
						BPrice2 = int.Parse(sqlDataReader["BPrice2"].ToString()),
						BValue2 = int.Parse(sqlDataReader["BValue2"].ToString()),
						BPrice3 = int.Parse(sqlDataReader["BPrice3"].ToString()),
						BValue3 = int.Parse(sqlDataReader["BValue3"].ToString()),
						CUnit = int.Parse(sqlDataReader["CUnit"].ToString()),
						CPrice1 = int.Parse(sqlDataReader["CPrice1"].ToString()),
						CValue1 = int.Parse(sqlDataReader["CValue1"].ToString()),
						CPrice2 = int.Parse(sqlDataReader["CPrice2"].ToString()),
						CValue2 = int.Parse(sqlDataReader["CValue2"].ToString()),
						CPrice3 = int.Parse(sqlDataReader["CPrice3"].ToString()),
						CValue3 = int.Parse(sqlDataReader["CValue3"].ToString()),
						IsContinue = bool.Parse(sqlDataReader["IsContinue"].ToString()),
						IsCheap = bool.Parse(sqlDataReader["IsCheap"].ToString()),
						LimitCount = (float)int.Parse(sqlDataReader["LimitCount"].ToString()),
						StartDate = DateTime.Parse(sqlDataReader["StartDate"].ToString()),
						EndDate = DateTime.Parse(sqlDataReader["EndDate"].ToString())
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
		public FusionInfo[] GetAllFusionDesc()
		{
			List<FusionInfo> list = new List<FusionInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Fusion_All_Desc");
				while (sqlDataReader.Read())
				{
					list.Add(new FusionInfo
					{
						FusionID = (int)sqlDataReader["FusionID"],
						Item1 = (int)sqlDataReader["Item1"],
						Item2 = (int)sqlDataReader["Item2"],
						Item3 = (int)sqlDataReader["Item3"],
						Item4 = (int)sqlDataReader["Item4"],
						Formula = (int)sqlDataReader["Formula"],
						Reward = (int)sqlDataReader["Reward"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllFusion", exception);
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
		public FusionInfo[] GetAllFusion()
		{
			List<FusionInfo> list = new List<FusionInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Fusion_All");
				while (sqlDataReader.Read())
				{
					list.Add(new FusionInfo
					{
						FusionID = (int)sqlDataReader["FusionID"],
						Item1 = (int)sqlDataReader["Item1"],
						Item2 = (int)sqlDataReader["Item2"],
						Item3 = (int)sqlDataReader["Item3"],
						Item4 = (int)sqlDataReader["Item4"],
						Formula = (int)sqlDataReader["Formula"],
						Reward = (int)sqlDataReader["Reward"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllFusion", exception);
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
		public StrengthenInfo[] GetAllStrengthen()
		{
			List<StrengthenInfo> list = new List<StrengthenInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Item_Strengthen_All");
				while (sqlDataReader.Read())
				{
					list.Add(new StrengthenInfo
					{
						StrengthenLevel = (int)sqlDataReader["StrengthenLevel"],
						Random = (int)sqlDataReader["Random"],
						Rock = (int)sqlDataReader["Rock"],
						Rock1 = (int)sqlDataReader["Rock1"],
						Rock2 = (int)sqlDataReader["Rock2"],
						Rock3 = (int)sqlDataReader["Rock3"],
						StoneLevelMin = (int)sqlDataReader["StoneLevelMin"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllStrengthen", exception);
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
		public RuneTemplateInfo[] GetAllRuneTemplate()
		{
			List<RuneTemplateInfo> list = new List<RuneTemplateInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_RuneTemplate_All");
				while (sqlDataReader.Read())
				{
					list.Add(new RuneTemplateInfo
					{
						TemplateID = (int)sqlDataReader["TemplateID"],
						NextTemplateID = (int)sqlDataReader["NextTemplateID"],
						Name = (string)sqlDataReader["Name"],
						BaseLevel = (int)sqlDataReader["BaseLevel"],
						MaxLevel = (int)sqlDataReader["MaxLevel"],
						Type1 = (int)sqlDataReader["Type1"],
						Attribute1 = (string)sqlDataReader["Attribute1"],
						Turn1 = (int)sqlDataReader["Turn1"],
						Rate1 = (int)sqlDataReader["Rate1"],
						Type2 = (int)sqlDataReader["Type2"],
						Attribute2 = (string)sqlDataReader["Attribute2"],
						Turn2 = (int)sqlDataReader["Turn2"],
						Rate2 = (int)sqlDataReader["Rate2"],
						Type3 = (int)sqlDataReader["Type3"],
						Attribute3 = (string)sqlDataReader["Attribute3"],
						Turn3 = (int)sqlDataReader["Turn3"],
						Rate3 = (int)sqlDataReader["Rate3"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetRuneTemplateInfo", exception);
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
		public StrengThenExpInfo[] GetAllStrengThenExp()
		{
			List<StrengThenExpInfo> list = new List<StrengThenExpInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_StrengThenExp_All");
				while (sqlDataReader.Read())
				{
					list.Add(new StrengThenExpInfo
					{
						ID = (int)sqlDataReader["ID"],
						Lv = (int)sqlDataReader["Lv"],
						Exp = (int)sqlDataReader["Exp"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetStrengThenExpInfo", exception);
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
		public StrengthenGoodsInfo[] GetAllStrengthenGoodsInfo()
		{
			List<StrengthenGoodsInfo> list = new List<StrengthenGoodsInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Item_StrengthenGoodsInfo_All");
				while (sqlDataReader.Read())
				{
					list.Add(new StrengthenGoodsInfo
					{
						ID = (int)sqlDataReader["ID"],
						Level = (int)sqlDataReader["Level"],
						CurrentEquip = (int)sqlDataReader["CurrentEquip"],
						GainEquip = (int)sqlDataReader["GainEquip"],
						OrginEquip = (int)sqlDataReader["OrginEquip"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllStrengthenGoodsInfo", exception);
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
		public StrengthenInfo[] GetAllRefineryStrengthen()
		{
			List<StrengthenInfo> list = new List<StrengthenInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Item_Refinery_Strengthen_All");
				while (sqlDataReader.Read())
				{
					list.Add(new StrengthenInfo
					{
						StrengthenLevel = (int)sqlDataReader["StrengthenLevel"],
						Rock = (int)sqlDataReader["Rock"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllRefineryStrengthen", exception);
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
		public List<RefineryInfo> GetAllRefineryInfo()
		{
			List<RefineryInfo> list = new List<RefineryInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Item_Refinery_All");
				while (sqlDataReader.Read())
				{
					list.Add(new RefineryInfo
					{
						RefineryID = (int)sqlDataReader["RefineryID"],
						m_Equip = 
						{
							(int)sqlDataReader["Equip1"],
							(int)sqlDataReader["Equip2"],
							(int)sqlDataReader["Equip3"],
							(int)sqlDataReader["Equip4"]
						},
						Item1 = (int)sqlDataReader["Item1"],
						Item2 = (int)sqlDataReader["Item2"],
						Item3 = (int)sqlDataReader["Item3"],
						Item1Count = (int)sqlDataReader["Item1Count"],
						Item2Count = (int)sqlDataReader["Item2Count"],
						Item3Count = (int)sqlDataReader["Item3Count"],
						m_Reward = 
						{
							(int)sqlDataReader["Material1"],
							(int)sqlDataReader["Operate1"],
							(int)sqlDataReader["Reward1"],
							(int)sqlDataReader["Material2"],
							(int)sqlDataReader["Operate2"],
							(int)sqlDataReader["Reward2"],
							(int)sqlDataReader["Material3"],
							(int)sqlDataReader["Operate3"],
							(int)sqlDataReader["Reward3"],
							(int)sqlDataReader["Material4"],
							(int)sqlDataReader["Operate4"],
							(int)sqlDataReader["Reward4"]
						}
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllRefineryInfo", exception);
				}
			}
			finally
			{
				if (sqlDataReader != null && sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return list;
		}
		public QuestInfo[] GetALlQuest()
		{
			List<QuestInfo> list = new List<QuestInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Quest_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitQuest(sqlDataReader));
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
		public QuestAwardInfo[] GetAllQuestGoods()
		{
			List<QuestAwardInfo> list = new List<QuestAwardInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Quest_Goods_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitQuestGoods(sqlDataReader));
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
		public QuestConditionInfo[] GetAllQuestCondiction()
		{
			List<QuestConditionInfo> list = new List<QuestConditionInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Quest_Condiction_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitQuestCondiction(sqlDataReader));
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
		public QuestRateInfo[] GetAllQuestRate()
		{
			List<QuestRateInfo> list = new List<QuestRateInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Quest_Rate_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitQuestRate(sqlDataReader));
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
		public QuestInfo GetSingleQuest(int questID)
		{
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@QuestID", SqlDbType.Int, 4)
				};
				array[0].Value = questID;
				this.db.GetReader(ref sqlDataReader, "SP_Quest_Single", array);
				if (sqlDataReader.Read())
				{
					return this.InitQuest(sqlDataReader);
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
		public QuestInfo InitQuest(SqlDataReader reader)
		{
			return new QuestInfo
			{
				ID = (int)reader["ID"],
				QuestID = (int)reader["QuestID"],
				Title = (reader["Title"] == null) ? "" : reader["Title"].ToString(),
				Detail = (reader["Detail"] == null) ? "" : reader["Detail"].ToString(),
				Objective = (reader["Objective"] == null) ? "" : reader["Objective"].ToString(),
				NeedMinLevel = (int)reader["NeedMinLevel"],
				NeedMaxLevel = (int)reader["NeedMaxLevel"],
				PreQuestID = (reader["PreQuestID"] == null) ? "" : reader["PreQuestID"].ToString(),
				NextQuestID = (reader["NextQuestID"] == null) ? "" : reader["NextQuestID"].ToString(),
				IsOther = (int)reader["IsOther"],
				CanRepeat = (bool)reader["CanRepeat"],
				RepeatInterval = (int)reader["RepeatInterval"],
				RepeatMax = (int)reader["RepeatMax"],
				RewardGP = (int)reader["RewardGP"],
				RewardGold = (int)reader["RewardGold"],
				RewardBindMoney = (int)reader["RewardBindMoney"],
				RewardOffer = (int)reader["RewardOffer"],
				RewardRiches = (int)reader["RewardRiches"],
				RewardBuffID = (int)reader["RewardBuffID"],
				RewardBuffDate = (int)reader["RewardBuffDate"],
				RewardMoney = (int)reader["RewardMoney"],
				Rands = (decimal)reader["Rands"],
				RandDouble = (int)reader["RandDouble"],
				TimeMode = (bool)reader["TimeMode"],
				StartDate = (DateTime)reader["StartDate"],
				EndDate = (DateTime)reader["EndDate"]
			};
		}
		public QuestAwardInfo InitQuestGoods(SqlDataReader reader)
		{
			return new QuestAwardInfo
			{
				QuestID = (int)reader["QuestID"],
				RewardItemID = (int)reader["RewardItemID"],
				IsSelect = (bool)reader["IsSelect"],
				RewardItemValid = (int)reader["RewardItemValid"],
				RewardItemCount1 = (int)reader["RewardItemCount1"],
				RewardItemCount2 = (int)reader["RewardItemCount2"],
				RewardItemCount3 = (int)reader["RewardItemCount3"],
				RewardItemCount4 = (int)reader["RewardItemCount4"],
				RewardItemCount5 = (int)reader["RewardItemCount5"],
				StrengthenLevel = (int)reader["StrengthenLevel"],
				AttackCompose = (int)reader["AttackCompose"],
				DefendCompose = (int)reader["DefendCompose"],
				AgilityCompose = (int)reader["AgilityCompose"],
				LuckCompose = (int)reader["LuckCompose"],
				IsCount = (bool)reader["IsCount"],
				IsBind = (bool)reader["IsBind"]
			};
		}
		public QuestConditionInfo InitQuestCondiction(SqlDataReader reader)
		{
			return new QuestConditionInfo
			{
				QuestID = (int)reader["QuestID"],
				CondictionID = (int)reader["CondictionID"],
				CondictionTitle = (reader["CondictionTitle"] == null) ? "" : reader["CondictionTitle"].ToString(),
				CondictionType = (int)reader["CondictionType"],
				Para1 = (int)reader["Para1"],
				Para2 = (int)reader["Para2"],
				isOpitional = (bool)reader["isOpitional"]
			};
		}
		public QuestRateInfo InitQuestRate(SqlDataReader reader)
		{
			return new QuestRateInfo
			{
				BindMoneyRate = (reader["BindMoneyRate"] == null) ? "" : reader["BindMoneyRate"].ToString(),
				ExpRate = (reader["ExpRate"] == null) ? "" : reader["ExpRate"].ToString(),
				GoldRate = (reader["GoldRate"] == null) ? "" : reader["GoldRate"].ToString(),
				ExploitRate = (reader["ExploitRate"] == null) ? "" : reader["ExploitRate"].ToString(),
				CanOneKeyFinishTime = (int)reader["CanOneKeyFinishTime"]
			};
		}
		public DropCondiction[] GetAllDropCondictions()
		{
			List<DropCondiction> list = new List<DropCondiction>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Drop_Condiction_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitDropCondiction(sqlDataReader));
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
		public DropItem[] GetAllDropItems()
		{
			List<DropItem> list = new List<DropItem>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Drop_Item_All");
				while (sqlDataReader.Read())
				{
					list.Add(this.InitDropItem(sqlDataReader));
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
		public DropCondiction InitDropCondiction(SqlDataReader reader)
		{
			return new DropCondiction
			{
				DropId = (int)reader["DropID"],
				CondictionType = (int)reader["CondictionType"],
				Para1 = (string)reader["Para1"],
				Para2 = (string)reader["Para2"]
			};
		}
		public DropItem InitDropItem(SqlDataReader reader)
		{
			return new DropItem
			{
				Id = (int)reader["Id"],
				DropId = (int)reader["DropId"],
				ItemId = (int)reader["ItemId"],
				ValueDate = (int)reader["ValueDate"],
				IsBind = (bool)reader["IsBind"],
				Random = (int)reader["Random"],
				BeginData = (int)reader["BeginData"],
				EndData = (int)reader["EndData"]
			};
		}
		public AASInfo[] GetAllAASInfo()
		{
			List<AASInfo> list = new List<AASInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_AASInfo_All");
				while (sqlDataReader.Read())
				{
					list.Add(new AASInfo
					{
						UserID = (int)sqlDataReader["ID"],
						Name = sqlDataReader["Name"].ToString(),
						IDNumber = sqlDataReader["IDNumber"].ToString(),
						State = (int)sqlDataReader["State"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllAASInfo", exception);
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
		public bool AddAASInfo(AASInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@Name", info.Name),
					new SqlParameter("@IDNumber", info.IDNumber),
					new SqlParameter("@State", info.State),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				array[4].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ASSInfo_Add", array);
				result = ((int)array[4].Value == 0);
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdateAASInfo", exception);
				}
			}
			return result;
		}
		public string GetASSInfoSingle(int UserID)
		{
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@UserID", UserID)
				};
				this.db.GetReader(ref sqlDataReader, "SP_ASSInfo_Single", sqlParameters);
				if (sqlDataReader.Read())
				{
					return sqlDataReader["IDNumber"].ToString();
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetASSInfoSingle", exception);
				}
			}
			finally
			{
				if (sqlDataReader != null && !sqlDataReader.IsClosed)
				{
					sqlDataReader.Close();
				}
			}
			return "";
		}
		public bool AddDailyLogList(DailyLogListInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] array = new SqlParameter[5];
				array[0] = new SqlParameter("@UserID", info.UserID);
				array[1] = new SqlParameter("@UserAwardLog", info.UserAwardLog);
				array[2] = new SqlParameter("@DayLog", info.DayLog);
				array[3] = new SqlParameter("@Result", SqlDbType.Int);
				array[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_DailyLogList_Add", array);
				result = ((int)array[3].Value == 0);
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdateAASInfo", exception);
				}
			}
			return result;
		}
		public bool UpdateDailyLogList(DailyLogListInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] array = new SqlParameter[]
				{
					new SqlParameter("@UserID", info.UserID),
					new SqlParameter("@UserAwardLog", info.UserAwardLog),
					new SqlParameter("@DayLog", info.DayLog),
					new SqlParameter("@LastDate", info.LastDate.ToString()),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				array[4].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_DailyLogList_Update", array);
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("User_Update_BoxProgression", exception);
				}
			}
			return result;
		}
		public DailyLogListInfo GetDailyLogListSingle(int UserID)
		{
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlParameter[] sqlParameters = new SqlParameter[]
				{
					new SqlParameter("@UserID", UserID)
				};
				this.db.GetReader(ref sqlDataReader, "SP_DailyLogList_Single", sqlParameters);
				if (sqlDataReader.Read())
				{
					return new DailyLogListInfo
					{
						ID = (int)sqlDataReader["ID"],
						UserID = (int)sqlDataReader["UserID"],
						UserAwardLog = (int)sqlDataReader["UserAwardLog"],
						DayLog = (string)sqlDataReader["DayLog"],
						LastDate = (DateTime)sqlDataReader["LastDate"]
					};
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("DailyLogList", exception);
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
		public DailyAwardInfo[] GetAllDailyAward()
		{
			List<DailyAwardInfo> list = new List<DailyAwardInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Daily_Award_All");
				while (sqlDataReader.Read())
				{
					list.Add(new DailyAwardInfo
					{
						Count = (int)sqlDataReader["Count"],
						ID = (int)sqlDataReader["ID"],
						IsBinds = (bool)sqlDataReader["IsBinds"],
						TemplateID = (int)sqlDataReader["TemplateID"],
						Type = (int)sqlDataReader["Type"],
						ValidDate = (int)sqlDataReader["ValidDate"],
						Sex = (int)sqlDataReader["Sex"],
						Remark = (sqlDataReader["Remark"] == null) ? "" : sqlDataReader["Remark"].ToString(),
						CountRemark = (sqlDataReader["CountRemark"] == null) ? "" : sqlDataReader["CountRemark"].ToString(),
						GetWay = (int)sqlDataReader["GetWay"],
						AwardDays = (int)sqlDataReader["AwardDays"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllDaily", exception);
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
		public NpcInfo[] GetAllNPCInfo()
		{
			List<NpcInfo> list = new List<NpcInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_NPC_Info_All");
				while (sqlDataReader.Read())
				{
					list.Add(new NpcInfo
					{
						ID = (int)sqlDataReader["ID"],
						Name = (sqlDataReader["Name"] == null) ? "" : sqlDataReader["Name"].ToString(),
						Level = (int)sqlDataReader["Level"],
						Camp = (int)sqlDataReader["Camp"],
						Type = (int)sqlDataReader["Type"],
						Blood = (int)sqlDataReader["Blood"],
						X = (int)sqlDataReader["X"],
						Y = (int)sqlDataReader["Y"],
						Width = (int)sqlDataReader["Width"],
						Height = (int)sqlDataReader["Height"],
						MoveMin = (int)sqlDataReader["MoveMin"],
						MoveMax = (int)sqlDataReader["MoveMax"],
						BaseDamage = (int)sqlDataReader["BaseDamage"],
						BaseGuard = (int)sqlDataReader["BaseGuard"],
						Attack = (int)sqlDataReader["Attack"],
						Defence = (int)sqlDataReader["Defence"],
						Agility = (int)sqlDataReader["Agility"],
						Lucky = (int)sqlDataReader["Lucky"],
						ModelID = (sqlDataReader["ModelID"] == null) ? "" : sqlDataReader["ModelID"].ToString(),
						ResourcesPath = (sqlDataReader["ResourcesPath"] == null) ? "" : sqlDataReader["ResourcesPath"].ToString(),
						DropRate = (sqlDataReader["DropRate"] == null) ? "" : sqlDataReader["DropRate"].ToString(),
						Experience = (int)sqlDataReader["Experience"],
						Delay = (int)sqlDataReader["Delay"],
						Immunity = (int)sqlDataReader["Immunity"],
						Alert = (int)sqlDataReader["Alert"],
						Range = (int)sqlDataReader["Range"],
						Preserve = (int)sqlDataReader["Preserve"],
						Script = (sqlDataReader["Script"] == null) ? "" : sqlDataReader["Script"].ToString(),
						FireX = (int)sqlDataReader["FireX"],
						FireY = (int)sqlDataReader["FireY"],
						DropId = (int)sqlDataReader["DropId"],
						CurrentBallId = (int)sqlDataReader["CurrentBallId"],
						speed = (int)sqlDataReader["speed"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllNPCInfo", exception);
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
		public MissionInfo[] GetAllMissionInfo()
		{
			List<MissionInfo> list = new List<MissionInfo>();
			SqlDataReader sqlDataReader = null;
			try
			{
				this.db.GetReader(ref sqlDataReader, "SP_Mission_Info_All");
				while (sqlDataReader.Read())
				{
					list.Add(new MissionInfo
					{
						Id = (int)sqlDataReader["ID"],
						Name = (sqlDataReader["Name"] == null) ? "" : sqlDataReader["Name"].ToString(),
						TotalCount = (int)sqlDataReader["TotalCount"],
						TotalTurn = (int)sqlDataReader["TotalTurn"],
						Script = (sqlDataReader["Script"] == null) ? "" : sqlDataReader["Script"].ToString(),
						Success = (sqlDataReader["Success"] == null) ? "" : sqlDataReader["Success"].ToString(),
						Failure = (sqlDataReader["Failure"] == null) ? "" : sqlDataReader["Failure"].ToString(),
						Description = (sqlDataReader["Description"] == null) ? "" : sqlDataReader["Description"].ToString(),
						IncrementDelay = (int)sqlDataReader["IncrementDelay"],
						Delay = (int)sqlDataReader["Delay"],
						Title = (sqlDataReader["Title"] == null) ? "" : sqlDataReader["Title"].ToString(),
						Param1 = (int)sqlDataReader["Param1"],
						Param2 = (int)sqlDataReader["Param2"]
					});
				}
			}
			catch (Exception exception)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllMissionInfo", exception);
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
