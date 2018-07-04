using Bussiness;
using Game.Logic;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Data;
using System.Reflection;
using System.Threading;
namespace Game.Server.Statics
{
	public class LogMgr
	{
		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static object _syncStop;
		private static int _gameType;
		private static int _serverId;
		private static int _areaId;
		public static DataTable m_LogItem;
		public static DataTable m_LogMoney;
		public static DataTable m_LogFight;
		public static bool Setup(int gametype, int serverid, int areaid)
		{
			LogMgr._gameType = gametype;
			LogMgr._serverId = serverid;
			LogMgr._areaId = areaid;
			LogMgr._syncStop = new object();
			LogMgr.m_LogItem = new DataTable("Log_Item");
			LogMgr.m_LogItem.Columns.Add("ApplicationId", Type.GetType("System.Int32"));
			LogMgr.m_LogItem.Columns.Add("SubId", typeof(int));
			LogMgr.m_LogItem.Columns.Add("LineId", typeof(int));
			LogMgr.m_LogItem.Columns.Add("EnterTime", Type.GetType("System.DateTime"));
			LogMgr.m_LogItem.Columns.Add("UserId", typeof(int));
			LogMgr.m_LogItem.Columns.Add("Operation", typeof(int));
			LogMgr.m_LogItem.Columns.Add("ItemName", typeof(string));
			LogMgr.m_LogItem.Columns.Add("ItemID", typeof(int));
			LogMgr.m_LogItem.Columns.Add("AddItem", typeof(string));
			LogMgr.m_LogItem.Columns.Add("BeginProperty", typeof(string));
			LogMgr.m_LogItem.Columns.Add("EndProperty", typeof(string));
			LogMgr.m_LogItem.Columns.Add("Result", typeof(int));
			LogMgr.m_LogMoney = new DataTable("Log_Money");
			LogMgr.m_LogMoney.Columns.Add("ApplicationId", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("SubId", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("LineId", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("MastType", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("SonType", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("UserId", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("EnterTime", Type.GetType("System.DateTime"));
			LogMgr.m_LogMoney.Columns.Add("Moneys", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("SpareMoney", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("Gold", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("GiftToken", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("Medal", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("Offer", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("OtherPay", typeof(string));
			LogMgr.m_LogMoney.Columns.Add("GoodId", typeof(string));
			LogMgr.m_LogMoney.Columns.Add("GoodsType", typeof(string));
			LogMgr.m_LogFight = new DataTable("Log_Fight");
			LogMgr.m_LogFight.Columns.Add("ApplicationId", typeof(int));
			LogMgr.m_LogFight.Columns.Add("SubId", typeof(int));
			LogMgr.m_LogFight.Columns.Add("LineId", typeof(int));
			LogMgr.m_LogFight.Columns.Add("RoomId", typeof(int));
			LogMgr.m_LogFight.Columns.Add("RoomType", typeof(int));
			LogMgr.m_LogFight.Columns.Add("FightType", typeof(int));
			LogMgr.m_LogFight.Columns.Add("ChangeTeam", typeof(int));
			LogMgr.m_LogFight.Columns.Add("PlayBegin", Type.GetType("System.DateTime"));
			LogMgr.m_LogFight.Columns.Add("PlayEnd", Type.GetType("System.DateTime"));
			LogMgr.m_LogFight.Columns.Add("UserCount", typeof(int));
			LogMgr.m_LogFight.Columns.Add("MapId", typeof(int));
			LogMgr.m_LogFight.Columns.Add("TeamA", typeof(string));
			LogMgr.m_LogFight.Columns.Add("TeamB", typeof(string));
			LogMgr.m_LogFight.Columns.Add("PlayResult", typeof(string));
			LogMgr.m_LogFight.Columns.Add("WinTeam", typeof(int));
			LogMgr.m_LogFight.Columns.Add("Detail", typeof(string));
			return true;
		}
		public static void Reset()
		{
			DataTable logItem;
			Monitor.Enter(logItem = LogMgr.m_LogItem);
			try
			{
				LogMgr.m_LogItem.Clear();
			}
			finally
			{
				Monitor.Exit(logItem);
			}
			DataTable logMoney;
			Monitor.Enter(logMoney = LogMgr.m_LogMoney);
			try
			{
				LogMgr.m_LogMoney.Clear();
			}
			finally
			{
				Monitor.Exit(logMoney);
			}
			DataTable logFight;
			Monitor.Enter(logFight = LogMgr.m_LogFight);
			try
			{
				LogMgr.m_LogFight.Clear();
			}
			finally
			{
				Monitor.Exit(logFight);
			}
		}
		public static void Save()
		{
			if (LogMgr._syncStop != null)
			{
				object syncStop;
				Monitor.Enter(syncStop = LogMgr._syncStop);
				try
				{
					using (ItemRecordBussiness itemRecordBussiness = new ItemRecordBussiness())
					{
						LogMgr.SaveLogItem(itemRecordBussiness);
						LogMgr.SaveLogMoney(itemRecordBussiness);
						LogMgr.SaveLogFight(itemRecordBussiness);
					}
				}
				finally
				{
					Monitor.Exit(syncStop);
				}
			}
		}
		public static void SaveLogItem(ItemRecordBussiness db)
		{
			DataTable logItem;
			Monitor.Enter(logItem = LogMgr.m_LogItem);
			try
			{
				db.LogItemDb(LogMgr.m_LogItem);
			}
			finally
			{
				Monitor.Exit(logItem);
			}
		}
		public static void SaveLogMoney(ItemRecordBussiness db)
		{
			DataTable logMoney;
			Monitor.Enter(logMoney = LogMgr.m_LogMoney);
			try
			{
				db.LogMoneyDb(LogMgr.m_LogMoney);
			}
			finally
			{
				Monitor.Exit(logMoney);
			}
		}
		public static void SaveLogFight(ItemRecordBussiness db)
		{
			DataTable logFight;
			Monitor.Enter(logFight = LogMgr.m_LogFight);
			try
			{
				db.LogFightDb(LogMgr.m_LogFight);
			}
			finally
			{
				Monitor.Exit(logFight);
			}
		}
		public static void LogItemAdd(int userId, LogItemType itemType, string beginProperty, ItemInfo item, string AddItem, int result)
		{
			try
			{
				string text = "";
				if (item != null)
				{
					text = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", new object[]
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
				object[] values = new object[]
				{
					LogMgr._gameType,
					LogMgr._serverId,
					LogMgr._areaId,
					DateTime.Now,
					userId,
					(int)itemType,
					(item == null) ? "" : item.Template.Name,
					(item == null) ? 0 : item.ItemID,
					AddItem,
					beginProperty,
					text,
					result
				};
				DataTable logItem;
				Monitor.Enter(logItem = LogMgr.m_LogItem);
				try
				{
					LogMgr.m_LogItem.Rows.Add(values);
				}
				finally
				{
					Monitor.Exit(logItem);
				}
			}
			catch (Exception arg)
			{
				if (LogMgr.log.IsErrorEnabled)
				{
					LogMgr.log.Error("LogMgr Error：ItemAdd @ " + arg);
				}
			}
		}
		public static void LogMoneyAdd(LogMoneyType masterType, LogMoneyType sonType, int userId, int moneys, int SpareMoney, int gold, int giftToken, int offer, int medal, string otherPay, string goodId, string goodsType)
		{
			try
			{
				if (moneys != 0 && moneys <= SpareMoney)
				{
					if (sonType <= LogMoneyType.Shop_Present)
					{
						if (sonType != LogMoneyType.Auction_Update)
						{
							switch (sonType)
							{
							case LogMoneyType.Mail_Pay:
							case LogMoneyType.Mail_Send:
								break;

							default:
								switch (sonType)
								{
								case LogMoneyType.Shop_Buy:
								case LogMoneyType.Shop_Continue:
								case LogMoneyType.Shop_Card:
								case LogMoneyType.Shop_Present:
									break;

								default:
									goto IL_C1;
								}
								break;
							}
						}
					}
					else
					{
						switch (sonType)
						{
						case LogMoneyType.Marry_Spark:
						case LogMoneyType.Marry_Gift:
						case LogMoneyType.Marry_Unmarry:
						case LogMoneyType.Marry_Room:
						case LogMoneyType.Marry_RoomAdd:
						case LogMoneyType.Marry_Flower:
						case LogMoneyType.Marry_Hymeneal:
						case LogMoneyType.Consortia_Rich:
							break;

						case LogMoneyType.Marry_Stage:
						case LogMoneyType.Marry_Follow:
						case (LogMoneyType)409:
						case (LogMoneyType)411:
							goto IL_C1;

						default:
							switch (sonType)
							{
							case LogMoneyType.Item_Move:
							case LogMoneyType.Item_Color:
								break;

							default:
								switch (sonType)
								{
								case LogMoneyType.Game_Boos:
								case LogMoneyType.Game_PaymentTakeCard:
								case LogMoneyType.Game_TryAgain:
									break;

								default:
									goto IL_C1;
								}
								break;
							}
							break;
						}
					}
					moneys *= -1;
					IL_C1:
					object[] values = new object[]
					{
						LogMgr._gameType,
						LogMgr._serverId,
						LogMgr._areaId,
						masterType,
						sonType,
						userId,
						DateTime.Now,
						moneys,
						SpareMoney,
						gold,
						giftToken,
						offer,
						medal,
						otherPay,
						goodId,
						goodsType
					};
					DataTable logMoney;
					Monitor.Enter(logMoney = LogMgr.m_LogMoney);
					try
					{
						LogMgr.m_LogMoney.Rows.Add(values);
					}
					finally
					{
						Monitor.Exit(logMoney);
					}
				}
			}
			catch (Exception arg)
			{
				if (LogMgr.log.IsErrorEnabled)
				{
					LogMgr.log.Error("LogMgr Error：LogMoney @ " + arg);
				}
			}
		}
		public static void LogFightAdd(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA, string teamB, string playResult, int winTeam, string BossWar)
		{
			try
			{
				object[] values = new object[]
				{
					LogMgr._gameType,
					LogMgr._serverId,
					LogMgr._areaId,
					roomId,
					(int)roomType,
					(int)fightType,
					changeTeam,
					playBegin,
					playEnd,
					userCount,
					mapId,
					teamA,
					teamB,
					playResult,
					winTeam,
					BossWar
				};
				DataTable logFight;
				Monitor.Enter(logFight = LogMgr.m_LogFight);
				try
				{
					LogMgr.m_LogFight.Rows.Add(values);
				}
				finally
				{
					Monitor.Exit(logFight);
				}
			}
			catch (Exception arg)
			{
				if (LogMgr.log.IsErrorEnabled)
				{
					LogMgr.log.Error("LogMgr Error：Fight @ " + arg);
				}
			}
		}
	}
}
