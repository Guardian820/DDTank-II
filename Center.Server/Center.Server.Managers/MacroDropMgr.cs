using Bussiness;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Timers;
namespace Center.Server.Managers
{
	public class MacroDropMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ReaderWriterLock m_lock;
		private static Dictionary<int, DropInfo> m_DropInfo;
		private static string FilePath;
		private static int counter;
		public static bool Init()
		{
			MacroDropMgr.m_lock = new ReaderWriterLock();
			MacroDropMgr.FilePath = Directory.GetCurrentDirectory() + "\\macrodrop\\macroDrop.ini";
			return MacroDropMgr.Reload();
		}
		public static bool Reload()
		{
			try
			{
				Dictionary<int, DropInfo> dictionary = new Dictionary<int, DropInfo>();
				MacroDropMgr.m_DropInfo = new Dictionary<int, DropInfo>();
				dictionary = MacroDropMgr.LoadDropInfo();
				if (dictionary != null && dictionary.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, DropInfo>>(ref MacroDropMgr.m_DropInfo, dictionary);
				}
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
		private static void MacroDropReset()
		{
			MacroDropMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				foreach (KeyValuePair<int, DropInfo> current in MacroDropMgr.m_DropInfo)
				{
					int arg_27_0 = current.Key;
					DropInfo value = current.Value;
					if (MacroDropMgr.counter > value.Time && value.Time > 0 && MacroDropMgr.counter % value.Time == 0)
					{
						value.Count = value.MaxCount;
					}
				}
			}
			catch (Exception exception)
			{
				if (MacroDropMgr.log.IsErrorEnabled)
				{
					MacroDropMgr.log.Error("DropInfoMgr MacroDropReset", exception);
				}
			}
			finally
			{
				MacroDropMgr.m_lock.ReleaseWriterLock();
			}
		}
		private static void MacroDropSync()
		{
			bool flag = true;
			ServerClient[] allClients = CenterServer.Instance.GetAllClients();
			ServerClient[] array = allClients;
			for (int i = 0; i < array.Length; i++)
			{
				ServerClient serverClient = array[i];
				if (!serverClient.NeedSyncMacroDrop)
				{
					flag = false;
					break;
				}
			}
			if (allClients.Length > 0 && flag)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(178);
				int count = MacroDropMgr.m_DropInfo.Count;
				gSPacketIn.WriteInt(count);
				MacroDropMgr.m_lock.AcquireReaderLock(-1);
				try
				{
					foreach (KeyValuePair<int, DropInfo> current in MacroDropMgr.m_DropInfo)
					{
						DropInfo value = current.Value;
						gSPacketIn.WriteInt(value.ID);
						gSPacketIn.WriteInt(value.Count);
						gSPacketIn.WriteInt(value.MaxCount);
					}
				}
				catch (Exception exception)
				{
					if (MacroDropMgr.log.IsErrorEnabled)
					{
						MacroDropMgr.log.Error("DropInfoMgr MacroDropReset", exception);
					}
				}
				finally
				{
					MacroDropMgr.m_lock.ReleaseReaderLock();
				}
				ServerClient[] array2 = allClients;
				for (int j = 0; j < array2.Length; j++)
				{
					ServerClient serverClient2 = array2[j];
					serverClient2.NeedSyncMacroDrop = false;
					serverClient2.SendTCP(gSPacketIn);
				}
			}
		}
		private static void OnTimeEvent(object source, ElapsedEventArgs e)
		{
			MacroDropMgr.counter++;
			if (MacroDropMgr.counter % 12 == 0)
			{
				MacroDropMgr.MacroDropReset();
			}
			MacroDropMgr.MacroDropSync();
		}
		public static void Start()
		{
			MacroDropMgr.counter = 0;
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Elapsed += new ElapsedEventHandler(MacroDropMgr.OnTimeEvent);
			timer.Interval = 300000.0;
			timer.Enabled = true;
		}
		private static Dictionary<int, DropInfo> LoadDropInfo()
		{
			Dictionary<int, DropInfo> dictionary = new Dictionary<int, DropInfo>();
			if (File.Exists(MacroDropMgr.FilePath))
			{
				IniReader iniReader = new IniReader(MacroDropMgr.FilePath);
				int num = 1;
				while (iniReader.GetIniString(num.ToString(), "TemplateId") != "")
				{
					string section = num.ToString();
					int id = Convert.ToInt32(iniReader.GetIniString(section, "TemplateId"));
					int time = Convert.ToInt32(iniReader.GetIniString(section, "Time"));
					int num2 = Convert.ToInt32(iniReader.GetIniString(section, "Count"));
					DropInfo dropInfo = new DropInfo(id, time, num2, num2);
					dictionary.Add(dropInfo.ID, dropInfo);
					num++;
				}
				return dictionary;
			}
			return null;
		}
		public static void DropNotice(Dictionary<int, int> temp)
		{
			MacroDropMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				foreach (KeyValuePair<int, int> current in temp)
				{
					if (MacroDropMgr.m_DropInfo.ContainsKey(current.Key))
					{
						DropInfo dropInfo = MacroDropMgr.m_DropInfo[current.Key];
						if (dropInfo.Count > 0)
						{
							dropInfo.Count -= current.Value;
						}
					}
				}
			}
			catch (Exception exception)
			{
				if (MacroDropMgr.log.IsErrorEnabled)
				{
					MacroDropMgr.log.Error("DropInfoMgr CanDrop", exception);
				}
			}
			finally
			{
				MacroDropMgr.m_lock.ReleaseWriterLock();
			}
		}
	}
}
