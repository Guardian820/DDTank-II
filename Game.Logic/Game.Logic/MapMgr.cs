using Bussiness;
using Game.Logic.Phy.Maps;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class MapMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, MapPoint> _maps;
		private static Dictionary<int, Map> _mapInfos;
		private static Dictionary<int, List<int>> _serverMap;
		private static ThreadSafeRandom random;
		private static ReaderWriterLock m_lock;
		public static int GetWeekDay
		{
			get
			{
				int num = Convert.ToInt32(DateTime.Now.DayOfWeek);
				if (num != 0)
				{
					return num;
				}
				return 7;
			}
		}
		public static bool ReLoadMap()
		{
			try
			{
				Dictionary<int, MapPoint> maps = new Dictionary<int, MapPoint>();
				Dictionary<int, Map> mapInfos = new Dictionary<int, Map>();
				if (MapMgr.LoadMap(maps, mapInfos))
				{
					MapMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						MapMgr._maps = maps;
						MapMgr._mapInfos = mapInfos;
						return true;
					}
					catch
					{
					}
					finally
					{
						MapMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (MapMgr.log.IsErrorEnabled)
				{
					MapMgr.log.Error("ReLoadMap", exception);
				}
			}
			return false;
		}
		public static bool ReLoadMapServer()
		{
			try
			{
				Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
				if (MapMgr.InitServerMap(dictionary))
				{
					MapMgr.m_lock.AcquireWriterLock(15000);
					try
					{
						MapMgr._serverMap = dictionary;
						return true;
					}
					catch
					{
					}
					finally
					{
						MapMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (MapMgr.log.IsErrorEnabled)
				{
					MapMgr.log.Error("ReLoadMapWeek", exception);
				}
			}
			return false;
		}
		public static bool Init()
		{
			try
			{
				MapMgr.random = new ThreadSafeRandom();
				MapMgr.m_lock = new ReaderWriterLock();
				MapMgr._maps = new Dictionary<int, MapPoint>();
				MapMgr._mapInfos = new Dictionary<int, Map>();
				if (!MapMgr.LoadMap(MapMgr._maps, MapMgr._mapInfos))
				{
					bool result = false;
					return result;
				}
				MapMgr._serverMap = new Dictionary<int, List<int>>();
				if (!MapMgr.InitServerMap(MapMgr._serverMap))
				{
					bool result = false;
					return result;
				}
			}
			catch (Exception exception)
			{
				if (MapMgr.log.IsErrorEnabled)
				{
					MapMgr.log.Error("MapMgr", exception);
				}
				bool result = false;
				return result;
			}
			return true;
		}
		public static bool LoadMap(Dictionary<int, MapPoint> maps, Dictionary<int, Map> mapInfos)
		{
			try
			{
				MapBussiness mapBussiness = new MapBussiness();
				MapInfo[] allMap = mapBussiness.GetAllMap();
				MapInfo[] array = allMap;
				for (int i = 0; i < array.Length; i++)
				{
					MapInfo mapInfo = array[i];
					if (!string.IsNullOrEmpty(mapInfo.PosX))
					{
						if (!maps.Keys.Contains(mapInfo.ID))
						{
							string[] array2 = mapInfo.PosX.Split(new char[]
							{
								'|'
							});
							string[] array3 = mapInfo.PosX1.Split(new char[]
							{
								'|'
							});
							MapPoint mapPoint = new MapPoint();
							string[] array4 = array2;
							for (int j = 0; j < array4.Length; j++)
							{
								string text = array4[j];
								if (!string.IsNullOrEmpty(text.Trim()))
								{
									string[] array5 = text.Split(new char[]
									{
										','
									});
									mapPoint.PosX.Add(new Point(int.Parse(array5[0]), int.Parse(array5[1])));
								}
							}
							string[] array6 = array3;
							for (int k = 0; k < array6.Length; k++)
							{
								string text2 = array6[k];
								if (!string.IsNullOrEmpty(text2.Trim()))
								{
									string[] array7 = text2.Split(new char[]
									{
										','
									});
									mapPoint.PosX1.Add(new Point(int.Parse(array7[0]), int.Parse(array7[1])));
								}
							}
							maps.Add(mapInfo.ID, mapPoint);
						}
						if (!mapInfos.ContainsKey(mapInfo.ID))
						{
							Tile tile = null;
							string text3 = string.Format("map\\{0}\\fore.map", mapInfo.ID);
							if (File.Exists(text3))
							{
								tile = new Tile(text3, true);
							}
							Tile tile2 = null;
							text3 = string.Format("map\\{0}\\dead.map", mapInfo.ID);
							if (File.Exists(text3))
							{
								tile2 = new Tile(text3, false);
							}
							if (tile == null && tile2 == null)
							{
								if (MapMgr.log.IsErrorEnabled)
								{
									MapMgr.log.Error("Map's file is not exist!");
								}
								bool result = false;
								return result;
							}
							mapInfos.Add(mapInfo.ID, new Map(mapInfo, tile, tile2));
						}
					}
				}
				if (maps.Count == 0 || mapInfos.Count == 0)
				{
					if (MapMgr.log.IsErrorEnabled)
					{
						MapMgr.log.Error(string.Concat(new object[]
						{
							"maps:",
							maps.Count,
							",mapInfos:",
							mapInfos.Count
						}));
					}
					bool result = false;
					return result;
				}
			}
			catch (Exception exception)
			{
				if (MapMgr.log.IsErrorEnabled)
				{
					MapMgr.log.Error("MapMgr", exception);
				}
				bool result = false;
				return result;
			}
			return true;
		}
		public static Map CloneMap(int index)
		{
			if (MapMgr._mapInfos.ContainsKey(index))
			{
				return MapMgr._mapInfos[index].Clone();
			}
			return null;
		}
		public static MapInfo FindMapInfo(int index)
		{
			if (MapMgr._mapInfos.ContainsKey(index))
			{
				return MapMgr._mapInfos[index].Info;
			}
			return null;
		}
		public static int GetMapIndex(int index, byte type, int serverId)
		{
			if (index != 0 && !MapMgr._maps.Keys.Contains(index))
			{
				index = 0;
			}
			if (index != 0)
			{
				return index;
			}
			List<int> list = new List<int>();
			foreach (int current in MapMgr._serverMap[serverId])
			{
				MapInfo mapInfo = MapMgr.FindMapInfo(current);
				if ((type & mapInfo.Type) != 0)
				{
					list.Add(current);
				}
			}
			if (list.Count == 0)
			{
				int count = MapMgr._serverMap[serverId].Count;
				return MapMgr._serverMap[serverId][MapMgr.random.Next(count)];
			}
			int count2 = list.Count;
			return list[MapMgr.random.Next(count2)];
		}
		public static MapPoint GetMapRandomPos(int index)
		{
			MapPoint mapPoint = new MapPoint();
			if (index != 0 && !MapMgr._maps.Keys.Contains(index))
			{
				index = 0;
			}
			MapPoint mapPoint2;
			if (index == 0)
			{
				int[] array = MapMgr._maps.Keys.ToArray<int>();
				mapPoint2 = MapMgr._maps[array[MapMgr.random.Next(array.Length)]];
			}
			else
			{
				mapPoint2 = MapMgr._maps[index];
			}
			if (MapMgr.random.Next(2) == 1)
			{
				mapPoint.PosX.AddRange(mapPoint2.PosX);
				mapPoint.PosX1.AddRange(mapPoint2.PosX1);
			}
			else
			{
				mapPoint.PosX.AddRange(mapPoint2.PosX1);
				mapPoint.PosX1.AddRange(mapPoint2.PosX);
			}
			return mapPoint;
		}
		public static MapPoint GetPVEMapRandomPos(int index)
		{
			MapPoint mapPoint = new MapPoint();
			if (index != 0 && !MapMgr._maps.Keys.Contains(index))
			{
				index = 0;
			}
			MapPoint mapPoint2;
			if (index == 0)
			{
				int[] array = MapMgr._maps.Keys.ToArray<int>();
				mapPoint2 = MapMgr._maps[array[MapMgr.random.Next(array.Length)]];
			}
			else
			{
				mapPoint2 = MapMgr._maps[index];
			}
			mapPoint.PosX.AddRange(mapPoint2.PosX);
			mapPoint.PosX1.AddRange(mapPoint2.PosX1);
			return mapPoint;
		}
		public static bool InitServerMap(Dictionary<int, List<int>> servermap)
		{
			MapBussiness mapBussiness = new MapBussiness();
			ServerMapInfo[] allServerMap = mapBussiness.GetAllServerMap();
			try
			{
				int item = 0;
				ServerMapInfo[] array = allServerMap;
				for (int i = 0; i < array.Length; i++)
				{
					ServerMapInfo serverMapInfo = array[i];
					if (!servermap.Keys.Contains(serverMapInfo.ServerID))
					{
						string[] array2 = serverMapInfo.OpenMap.Split(new char[]
						{
							','
						});
						List<int> list = new List<int>();
						string[] array3 = array2;
						for (int j = 0; j < array3.Length; j++)
						{
							string text = array3[j];
							if (!string.IsNullOrEmpty(text) && int.TryParse(text, out item))
							{
								list.Add(item);
							}
						}
						servermap.Add(serverMapInfo.ServerID, list);
					}
				}
			}
			catch (Exception ex)
			{
				MapMgr.log.Error(ex.ToString());
			}
			return true;
		}
	}
}
