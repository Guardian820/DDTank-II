using Game.Base.Config;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Bussiness
{
	public abstract class GameProperties
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		[ConfigProperty("FreeMoney", "µ±Ç°ÓÎÏ·°æ±¾", "0")]
		public static readonly string FreeMoney;
		[ConfigProperty("FreeExp", "µ±Ç°ÓÎÏ·°æ±¾", "11901")]
		public static readonly string FreeExp;
		[ConfigProperty("BigExp", "µ±Ç°ÓÎÏ·°æ±¾", "11906")]
		public static readonly string BigExp;
		[ConfigProperty("PetExp", "µ±Ç°ÓÎÏ·°æ±¾", "334103")]
		public static readonly string PetExp;
		[ConfigProperty("Edition", "µ±Ç°ÓÎÏ·°æ±¾", "11000")]
		public static readonly string EDITION;
		[ConfigProperty("MustComposeGold", "ºÏ³ÉÏûºÄ½ð±Ò¼Û¸ñ", 1000)]
		public static readonly int PRICE_COMPOSE_GOLD;
		[ConfigProperty("MustFusionGold", "ÈÛÁ¶ÏûºÄ½ð±Ò¼Û¸ñ", 1000)]
		public static readonly int PRICE_FUSION_GOLD;
		[ConfigProperty("MustStrengthenGold", "Ç¿»¯½ð±ÒÏûºÄ¼Û¸ñ", 1000)]
		public static readonly int PRICE_STRENGHTN_GOLD;
		[ConfigProperty("CheckRewardItem", "ÑéÖ¤Âë½±ÀøÎïÆ·", 11001)]
		public static readonly int CHECK_REWARD_ITEM;
		[ConfigProperty("CheckCount", "×î´óÑéÖ¤ÂëÊ§°Ü´ÎÊý", 2)]
		public static readonly int CHECK_MAX_FAILED_COUNT;
		[ConfigProperty("HymenealMoney", "Çó»éµÄ¼Û¸ñ", 300)]
		public static readonly int PRICE_PROPOSE;
		[ConfigProperty("DivorcedMoney", "Àë»éµÄ¼Û¸ñ", 1499)]
		public static readonly int PRICE_DIVORCED;
		[ConfigProperty("DivorcedDiscountMoney", "Àë»éµÄ¼Û¸ñ", 999)]
		public static readonly int PRICE_DIVORCED_DISCOUNT;
		[ConfigProperty("MarryRoomCreateMoney", "½á»é·¿¼äµÄ¼Û¸ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö¸ô", "2000,2700,3400")]
		public static readonly string PRICE_MARRY_ROOM;
		[ConfigProperty("BoxAppearCondition", "Ïä×ÓÎïÆ·ÌáÊ¾µÄµÈ¼¶", 4)]
		public static readonly int BOX_APPEAR_CONDITION;
		[ConfigProperty("DisableCommands", "½ûÖ¹Ê¹ÓÃµÄÃüÁî", "")]
		public static readonly string DISABLED_COMMANDS;
		[ConfigProperty("AssState", "·À³ÁÃÔÏµÍ³µÄ¿ª¹Ø,True´ò¿ª,False¹Ø±Õ", false)]
		public static bool ASS_STATE;
		[ConfigProperty("DailyAwardState", "Ã¿ÈÕ½±Àø¿ª¹Ø,True´ò¿ª,False¹Ø±Õ", true)]
		public static bool DAILY_AWARD_STATE;
		[ConfigProperty("Cess", "½»Ò×¿ÛË°", 0.1)]
		public static readonly double Cess;
		[ConfigProperty("BeginAuction", "ÅÄÂòÊ±ÆðÊ¼Ëæ»úÊ±¼ä", 20)]
		public static int BeginAuction;
		[ConfigProperty("EndAuction", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", 40)]
		public static int EndAuction;
		[ConfigProperty("HotSpringExp", "Kinh nghiệm Spa", "1|2")]
		public static readonly string HotSpringExp;
		[ConfigProperty("ConsortiaStrengthenEx", "Kinh nghiệm", "1|2")]
		public static readonly string ConsortiaStrengthenEx;
		[ConfigProperty("RuneLevelUpExp", "Kinh nghiệm châu báu", "1|2")]
		public static readonly string RuneLevelUpExp;
		[ConfigProperty("RunePackageID", "RunePackageID", "1|2")]
		public static readonly string RunePackageID;
		[ConfigProperty("OpenRunePackageMoney", "OpenRunePackageMoney", "1|2")]
		public static readonly string OpenRunePackageMoney;
		[ConfigProperty("OpenRunePackageRange", "OpenRunePackageRange", "1|2")]
		public static readonly string OpenRunePackageRange;
		[ConfigProperty("VIPExpForEachLv", "VIPExpForEachLv", "1|2")]
		public static readonly string VIPExpForEachLv;
		[ConfigProperty("HoleLevelUpExpList", "HoleLevelUpExpList", "1|2")]
		public static readonly string HoleLevelUpExpList;
		[ConfigProperty("VIPStrengthenEx", "VIPStrengthenEx", "1|2")]
		public static readonly string VIPStrengthenEx;
		private static void Load(Type type)
		{
			using (ServiceBussiness serviceBussiness = new ServiceBussiness())
			{
				FieldInfo[] fields = type.GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo fieldInfo = fields[i];
					if (fieldInfo.IsStatic)
					{
						object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
						if (customAttributes.Length != 0)
						{
							ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)customAttributes[0];
							fieldInfo.SetValue(null, GameProperties.LoadProperty(attrib, serviceBussiness));
						}
					}
				}
			}
		}
		private static void Save(Type type)
		{
			using (ServiceBussiness serviceBussiness = new ServiceBussiness())
			{
				FieldInfo[] fields = type.GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo fieldInfo = fields[i];
					if (fieldInfo.IsStatic)
					{
						object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
						if (customAttributes.Length != 0)
						{
							ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)customAttributes[0];
							GameProperties.SaveProperty(attrib, serviceBussiness, fieldInfo.GetValue(null));
						}
					}
				}
			}
		}
		private static object LoadProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb)
		{
			string key = attrib.Key;
			ServerProperty serverProperty = sb.GetServerPropertyByKey(key);
			if (serverProperty == null)
			{
				serverProperty = new ServerProperty();
				serverProperty.Key = key;
				serverProperty.Value = attrib.DefaultValue.ToString();
				GameProperties.log.Error("Cannot find server property " + key + ",keep it default value!");
			}
			object result;
			try
			{
				result = Convert.ChangeType(serverProperty.Value, attrib.DefaultValue.GetType());
			}
			catch (Exception exception)
			{
				GameProperties.log.Error("Exception in GameProperties Load: ", exception);
				result = null;
			}
			return result;
		}
		private static void SaveProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb, object value)
		{
			try
			{
				sb.UpdateServerPropertyByKey(attrib.Key, value.ToString());
			}
			catch (Exception exception)
			{
				GameProperties.log.Error("Exception in GameProperties Save: ", exception);
			}
		}
		public static void Refresh()
		{
			GameProperties.log.Info("Refreshing game properties!");
			GameProperties.Load(typeof(GameProperties));
		}
		public static List<int> getProp(string prop)
		{
			List<int> list = new List<int>();
			string[] array = prop.Split(new char[]
			{
				'|'
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string value = array2[i];
				list.Add(Convert.ToInt32(value));
			}
			return list;
		}
		public static List<int> VIPExp()
		{
			return GameProperties.getProp(GameProperties.VIPExpForEachLv);
		}
		public static List<int> RuneExp()
		{
			return GameProperties.getProp(GameProperties.RuneLevelUpExp);
		}
		public static int ConsortiaStrengExp(int Lv)
		{
			return GameProperties.getProp(GameProperties.ConsortiaStrengthenEx)[Lv];
		}
		public static int VIPStrengthenExp(int vipLv)
		{
			return GameProperties.getProp(GameProperties.VIPStrengthenEx)[vipLv];
		}
		public static int HoleLevelUpExp(int lv)
		{
			return GameProperties.getProp(GameProperties.HoleLevelUpExpList)[lv];
		}
		public static void Save()
		{
			GameProperties.log.Info("Saving game properties into db!");
			GameProperties.Save(typeof(GameProperties));
		}
	}
}
