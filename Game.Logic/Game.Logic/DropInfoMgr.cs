using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class DropInfoMgr
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected static ReaderWriterLock m_lock = new ReaderWriterLock();
		public static Dictionary<int, MacroDropInfo> DropInfo;
		public static bool CanDrop(int templateId)
		{
			if (DropInfoMgr.DropInfo == null)
			{
				return true;
			}
			DropInfoMgr.m_lock.AcquireWriterLock(15000);
			try
			{
				if (DropInfoMgr.DropInfo.ContainsKey(templateId))
				{
					MacroDropInfo macroDropInfo = DropInfoMgr.DropInfo[templateId];
					bool result;
					if (macroDropInfo.DropCount < macroDropInfo.MaxDropCount || macroDropInfo.SelfDropCount >= macroDropInfo.DropCount)
					{
						macroDropInfo.SelfDropCount++;
						macroDropInfo.DropCount++;
						result = true;
						return result;
					}
					result = false;
					return result;
				}
			}
			catch (Exception exception)
			{
				if (DropInfoMgr.log.IsErrorEnabled)
				{
					DropInfoMgr.log.Error("DropInfoMgr CanDrop", exception);
				}
			}
			finally
			{
				DropInfoMgr.m_lock.ReleaseWriterLock();
			}
			return true;
		}
	}
}
