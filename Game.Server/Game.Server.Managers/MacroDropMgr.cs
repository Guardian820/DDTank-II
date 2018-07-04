using Game.Base.Packets;
using Game.Logic;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Timers;
namespace Game.Server.Managers
{
	public class MacroDropMgr
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected static ReaderWriterLock m_lock = new ReaderWriterLock();
		public static bool Init()
		{
			MacroDropMgr.m_lock = new ReaderWriterLock();
			return MacroDropMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			try
			{
				DropInfoMgr.DropInfo = new Dictionary<int, MacroDropInfo>();
				return true;
			}
			catch (Exception exception)
			{
				if (MacroDropMgr.log.IsErrorEnabled)
				{
					MacroDropMgr.log.Error("DropInfoMgr", exception);
				}
			}
			return false;
		}
		private static void OnTimeEvent(object source, ElapsedEventArgs e)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			MacroDropMgr.m_lock.AcquireWriterLock(15000);
			try
			{
				foreach (KeyValuePair<int, MacroDropInfo> current in DropInfoMgr.DropInfo)
				{
					int key = current.Key;
					MacroDropInfo value = current.Value;
					if (value.SelfDropCount > 0)
					{
						dictionary.Add(key, value.SelfDropCount);
						value.SelfDropCount = 0;
					}
				}
			}
			catch (Exception exception)
			{
				if (MacroDropMgr.log.IsErrorEnabled)
				{
					MacroDropMgr.log.Error("DropInfoMgr OnTimeEvent", exception);
				}
			}
			finally
			{
				MacroDropMgr.m_lock.ReleaseWriterLock();
			}
			if (dictionary.Count > 0)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(178);
				gSPacketIn.WriteInt(dictionary.Count);
				foreach (KeyValuePair<int, int> current2 in dictionary)
				{
					gSPacketIn.WriteInt(current2.Key);
					gSPacketIn.WriteInt(current2.Value);
				}
				GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
			}
		}
		public static void Start()
		{
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Elapsed += new ElapsedEventHandler(MacroDropMgr.OnTimeEvent);
			timer.Interval = 5000.0;
			timer.Enabled = true;
		}
		public static void UpdateDropInfo(Dictionary<int, MacroDropInfo> temp)
		{
			MacroDropMgr.m_lock.AcquireWriterLock(15000);
			try
			{
				foreach (KeyValuePair<int, MacroDropInfo> current in temp)
				{
					if (DropInfoMgr.DropInfo.ContainsKey(current.Key))
					{
						DropInfoMgr.DropInfo[current.Key].DropCount = current.Value.DropCount;
						DropInfoMgr.DropInfo[current.Key].MaxDropCount = current.Value.MaxDropCount;
					}
					else
					{
						DropInfoMgr.DropInfo.Add(current.Key, current.Value);
					}
				}
			}
			catch (Exception exception)
			{
				if (MacroDropMgr.log.IsErrorEnabled)
				{
					MacroDropMgr.log.Error("MacroDropMgr UpdateDropInfo", exception);
				}
			}
			finally
			{
				MacroDropMgr.m_lock.ReleaseWriterLock();
			}
		}
	}
}
